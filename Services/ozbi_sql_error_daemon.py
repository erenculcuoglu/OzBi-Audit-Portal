#!/usr/bin/env python3
"""
OzBI MariaDB SQL & System Error Monitor Daemon
----------------------------------------------
MariaDB (elegance.odeaweb.com) veritabanını salt okunur (SELECT) sorgularla 10 saniyede bir tarar.
Model tarafından üretilen hatalı SQL veya sistem hatalarını (ErrorMessage IS NOT NULL)
anında Slack #ozbi-sql-errors kanalına zengin Block Kit kartı olarak push eder.
"""

import os
import sys
import time
import json
import base64
import logging
import urllib.request
import urllib.error
from datetime import datetime, timezone, timedelta
import pymysql

# Log Ayarları
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s [%(levelname)s] %(message)s',
    handlers=[
        logging.StreamHandler(sys.stdout)
    ]
)
logger = logging.getLogger("OzBiSqlErrorDaemon")

# Konfigürasyon
DB_HOST = "elegance.odeaweb.com"
DB_USER = "ozbird"
DB_PASS = "mW783p4t?"
DB_NAME = "ozbiappc_app"
DB_PORT = 3306
POLL_INTERVAL_SECONDS = 10

# Slack Webhook URL (Fallback / Default for #ozbi-sql-errors)
SLACK_WEBHOOK_B64 = os.environ.get(
    "SLACK_SQL_ERROR_WEBHOOK_B64",
    "aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVDM3R0xSSlRGL0IwQlJBMFNSUFJDL3lvR1JpOEVWNHdaUEVrbWV5ejFHVWltUw=="
)
WEBHOOK_URL = os.environ.get("SLACK_SQL_ERROR_WEBHOOK_URL") or base64.b64decode(SLACK_WEBHOOK_B64).decode('utf-8')

STATE_FILE = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "app", "sql_error_daemon_state.json")

def load_state():
    if os.path.exists(STATE_FILE):
        try:
            with open(STATE_FILE, "r", encoding="utf-8") as f:
                return json.load(f)
        except Exception as e:
            logger.warning(f"State dosyası okunamadı: {e}")
    return {}

def save_state(state):
    try:
        os.makedirs(os.path.dirname(STATE_FILE), exist_ok=True)
        with open(STATE_FILE, "w", encoding="utf-8") as f:
            json.dump(state, f, ensure_ascii=False, indent=2)
    except Exception as e:
        logger.error(f"State dosyası kaydedilemedi: {e}")

def format_sql(raw_query):
    if not raw_query:
        return ""
    raw = raw_query.strip()
    if (raw.startswith("[") and raw.endswith("]")) or (raw.startswith("{") and raw.endswith("}")):
        try:
            parsed = json.loads(raw)
            sqls = []
            if isinstance(parsed, list):
                for item in parsed:
                    if isinstance(item, dict):
                        desc = item.get("description") or item.get("Description") or item.get("summary") or ""
                        sql = item.get("sql") or item.get("Sql") or item.get("query") or item.get("Query") or item.get("result") or ""
                        if sql:
                            sqls.append(f"-- {desc}\n{sql}" if desc else sql)
            elif isinstance(parsed, dict):
                desc = parsed.get("description") or parsed.get("Description") or parsed.get("summary") or ""
                sql = parsed.get("sql") or parsed.get("Sql") or parsed.get("query") or parsed.get("Query") or parsed.get("result") or ""
                if sql:
                    sqls.append(f"-- {desc}\n{sql}" if desc else sql)
            if sqls:
                return "\n\n-- ----------------------------\n\n".join(sqls)
        except:
            pass
    return raw

def send_slack_sql_error_notification(msg_id, chat_id, tenant_name, display_name, email_str, date_created_str, error_msg, prompt_text, sql_text):
    turkey_time = datetime.now(timezone(timedelta(hours=3))).strftime("%d.%m.%Y %H:%M:%S")
    
    user_display = display_name or "Kullanıcı"
    if email_str and email_str != "E-posta yok" and email_str not in user_display:
        user_display += f" ({email_str})"

    blocks = [
        {
            "type": "header",
            "text": {"type": "plain_text", "text": "⚠️ OzBI - SQL / Sistem Hatası Tespit Edildi", "emoji": True}
        },
        {
            "type": "section",
            "fields": [
                {"type": "mrkdwn", "text": f"*🏢 Firma:*\n*{tenant_name}*"},
                {"type": "mrkdwn", "text": f"*👤 Kullanıcı:*\n{user_display}"}
            ]
        }
    ]

    if prompt_text:
        blocks.append({
            "type": "section",
            "text": {"type": "mrkdwn", "text": f"❓ *Kullanıcı Sorusu:*\n{prompt_text.strip()}"}
        })

    err_text = (error_msg or "Bilinmeyen hata").strip()
    if len(err_text) > 800:
        err_text = err_text[:800] + "..."
    blocks.append({
        "type": "section",
        "text": {"type": "mrkdwn", "text": f"🔴 *Hata Detayı:*\n```{err_text}```"}
    })

    if sql_text:
        short_sql = sql_text[:1000] + "\n-- (...kesildi...)" if len(sql_text) > 1000 else sql_text
        blocks.append({
            "type": "section",
            "text": {"type": "mrkdwn", "text": f"💻 *Hatalı SQL / Query:*\n```sql\n{short_sql}\n```"}
        })

    blocks.append({
        "type": "context",
        "elements": [
            {"type": "mrkdwn", "text": f"🔍 *Kayıt ID:* `{msg_id}` | *Sohbet ID:* `{chat_id}` | *Tarih (TSI):* {date_created_str or turkey_time}"}
        ]
    })

    payload = {
        "text": f"⚠️ *OzBI SQL Hatası:* {tenant_name} ({user_display}) - {err_text}",
        "blocks": blocks
    }

    data = json.dumps(payload).encode('utf-8')
    req = urllib.request.Request(WEBHOOK_URL, data=data, headers={'Content-Type': 'application/json'})
    try:
        with urllib.request.urlopen(req, timeout=10) as resp:
            body = resp.read().decode('utf-8')
            logger.info(f"✅ Slack SQL hata bildirimi gönderildi! [{tenant_name} / {msg_id}] Yanıt: {body}")
            return True
    except Exception as ex:
        logger.error(f"❌ Slack bildirimi gönderilemedi: {ex}")
        return False

def check_sql_errors():
    state = load_state()
    is_initial_run = (len(state) == 0)

    try:
        conn = pymysql.connect(
            host=DB_HOST,
            user=DB_USER,
            password=DB_PASS,
            database=DB_NAME,
            port=DB_PORT,
            charset='utf8mb4',
            cursorclass=pymysql.cursors.DictCursor,
            connect_timeout=10,
            read_timeout=15
        )
    except Exception as e:
        logger.error(f"MariaDB bağlantı hatası: {e}")
        return

    try:
        with conn.cursor() as cur:
            query = """
                SELECT 
                    m.Id, m.ChatId, m.Message, m.Query, m.Prompt, m.ErrorMessage, 
                    m.Summary, m.DateCreated,
                    c.Title as ChatTitle,
                    t.Name as TenantName,
                    u.NameSurname, u.UserName, u.Email
                FROM chatmessage m
                INNER JOIN chat c ON m.ChatId = c.Id
                LEFT JOIN tenant t ON c.TenantId = t.Id
                LEFT JOIN aspnetusers u ON c.CreatedByUserId = u.Id
                WHERE (m.ErrorMessage IS NOT NULL AND TRIM(m.ErrorMessage) != '')
                  AND (m.Role = 'Model' OR m.Role = 'model' OR m.Role = 'assistant')
                ORDER BY m.DateCreated DESC
                LIMIT 100
            """
            cur.execute(query)
            rows = cur.fetchall()

            if is_initial_run:
                logger.info(f"İlk başlangıç: Mevcut {len(rows)} geçmiş hata kaydı hafızaya alınıyor (Spam koruması).")
                for r in rows:
                    state[r['Id']] = {
                        "date_created": str(r['DateCreated']),
                        "pushed_at": datetime.now(timezone.utc).isoformat(),
                        "tenant": r['TenantName'] or "Bilinmeyen"
                    }
                save_state(state)
                return

            missing_prompt_chat_ids = [r['ChatId'] for r in rows if not r.get('Prompt') and r.get('ChatId')]
            user_prompts = {}
            if missing_prompt_chat_ids:
                format_strings = ','.join(['%s'] * len(set(missing_prompt_chat_ids)))
                cur.execute(f"""
                    SELECT ChatId, Message, Prompt, DateCreated 
                    FROM chatmessage 
                    WHERE ChatId IN ({format_strings}) AND (Role = 'user' OR Role = 'User')
                    ORDER BY DateCreated DESC
                """, tuple(set(missing_prompt_chat_ids)))
                user_msgs = cur.fetchall()
                for um in user_msgs:
                    if um['ChatId'] not in user_prompts:
                        user_prompts[um['ChatId']] = um.get('Message') or um.get('Prompt') or ""

            pushed_count = 0
            for r in rows:
                msg_id = r['Id']
                if msg_id in state:
                    continue

                tenant = r['TenantName'] or "Bilinmeyen Tenant"
                display = r['NameSurname'] or r['UserName'] or r['Email'] or "Kullanıcı"
                email_str = r['Email'] or r['UserName'] or "E-posta yok"
                
                prompt_text = r['Prompt']
                if not prompt_text:
                    prompt_text = user_prompts.get(r['ChatId']) or r['ChatTitle'] or ""

                sql_text = format_sql(r['Query'])

                turkey_time = ""
                if r['DateCreated']:
                    try:
                        dt = r['DateCreated']
                        turkey_time = dt.strftime("%d.%m.%Y %H:%M:%S")
                    except:
                        pass

                sent = send_slack_sql_error_notification(
                    msg_id=msg_id,
                    chat_id=r['ChatId'],
                    tenant_name=tenant,
                    display_name=display,
                    email_str=email_str,
                    date_created_str=turkey_time,
                    error_msg=r['ErrorMessage'],
                    prompt_text=prompt_text,
                    sql_text=sql_text
                )

                state[msg_id] = {
                    "date_created": str(r['DateCreated']),
                    "pushed_at": datetime.now(timezone.utc).isoformat(),
                    "tenant": tenant,
                    "success": sent
                }
                save_state(state)
                pushed_count += 1
                time.sleep(0.5)

            if pushed_count > 0:
                logger.info(f"✨ Toplam {pushed_count} adet yeni SQL hata bildirimi Slack'e push edildi.")

    except Exception as e:
        logger.error(f"SQL hata tarama sırasında istisna oluştu: {e}")
    finally:
        conn.close()

def main():
    logger.info("==================================================================")
    logger.info("OzBI SQL & System Error Monitor Daemon Başlatıldı")
    logger.info(f"MariaDB: {DB_HOST}:{DB_PORT}/{DB_NAME}")
    logger.info(f"Slack Webhook: {WEBHOOK_URL[:35]}... (Kanal: #ozbi-sql-errors)")
    logger.info(f"Tarama Aralığı: {POLL_INTERVAL_SECONDS} saniye")
    logger.info("==================================================================")

    while True:
        try:
            check_sql_errors()
        except Exception as e:
            logger.error(f"Beklenmeyen döngü hatası: {e}")
        time.sleep(POLL_INTERVAL_SECONDS)

if __name__ == "__main__":
    main()
