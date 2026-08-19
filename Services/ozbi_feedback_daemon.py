#!/usr/bin/env python3
"""
OzBI MariaDB Customer Feedback & Disliked SQL Monitor Daemon
------------------------------------------------------------
MariaDB (elegance.odeaweb.com) veritabanını salt okunur (SELECT) sorgularla 10 saniyede bir tarar.
Müşteri beğenmediğinde (IsLiked = 0) veya yorum/eleştiri girdiğinde (FeedbackReason != '')
anında Slack #customer-feedback kanalına zengin Block Kit kartı olarak push eder.
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
logger = logging.getLogger("OzBiFeedbackDaemon")

# Konfigürasyon
DB_HOST = "elegance.odeaweb.com"
DB_USER = "ozbird"
DB_PASS = "mW783p4t?"
DB_NAME = "ozbiappc_app"
DB_PORT = 3306
POLL_INTERVAL_SECONDS = 10

# Slack Webhook URL (Fallback / Default for #customer-feedback)
SLACK_WEBHOOK_B64 = os.environ.get(
    "SLACK_FEEDBACK_WEBHOOK_B64",
    "aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVDM3R0xSSlRGL0IwQlI3VFlRUTNZLzVjNldKNnBRd0lHRGNEcnJhaVJEOG5vVQ=="
)
WEBHOOK_URL = os.environ.get("SLACK_FEEDBACK_WEBHOOK_URL") or base64.b64decode(SLACK_WEBHOOK_B64).decode('utf-8')

STATE_FILE = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "app", "feedback_daemon_state.json")

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
                        sql = item.get("sql") or item.get("Sql") or item.get("query") or item.get("Query") or ""
                        if sql:
                            sqls.append(f"-- {desc}\n{sql}" if desc else sql)
            elif isinstance(parsed, dict):
                desc = parsed.get("description") or parsed.get("Description") or parsed.get("summary") or ""
                sql = parsed.get("sql") or parsed.get("Sql") or parsed.get("query") or parsed.get("Query") or ""
                if sql:
                    sqls.append(f"-- {desc}\n{sql}" if desc else sql)
            if sqls:
                return "\n\n-- ----------------------------\n\n".join(sqls)
        except:
            pass
    return raw

def send_slack_feedback_notification(msg_id, chat_id, tenant_name, display_name, email_str, model_str, date_created_str, is_liked, feedback_reason, prompt_text, sql_text, ai_response, error_msg, duration_ms):
    user_display = display_name or "Kullanıcı"
    if email_str and email_str != "E-posta yok" and email_str not in user_display:
        user_display += f" ({email_str})"

    criticism_text = feedback_reason.strip() if feedback_reason else "Beğenilmedi (Yazılı yorum girilmedi)"

    blocks = [
        {
            "type": "header",
            "text": {"type": "plain_text", "text": "🔴 OzBI - Müşteri Geri Bildirimi", "emoji": True}
        },
        {
            "type": "section",
            "fields": [
                {"type": "mrkdwn", "text": f"*🏢 Firma:*\n*{tenant_name}*"},
                {"type": "mrkdwn", "text": f"*👤 Kullanıcı:*\n{user_display}"}
            ]
        },
        {
            "type": "section",
            "text": {"type": "mrkdwn", "text": f"💬 *Müşteri Eleştirisi:*\n> *“{criticism_text}”*"}
        }
    ]

    if prompt_text:
        blocks.append({
            "type": "section",
            "text": {"type": "mrkdwn", "text": f"❓ *Kullanıcı Sorusu:*\n{prompt_text.strip()}"}
        })

    payload = {
        "text": f"🔴 *OzBI Geri Bildirim:* {tenant_name} ({user_display}) - {criticism_text}",
        "blocks": blocks
    }

    data = json.dumps(payload).encode('utf-8')
    req = urllib.request.Request(WEBHOOK_URL, data=data, headers={'Content-Type': 'application/json'})
    try:
        with urllib.request.urlopen(req, timeout=10) as resp:
            body = resp.read().decode('utf-8')
            logger.info(f"✅ Slack geri bildirim bildirimi gönderildi! [{tenant_name} / {msg_id}] Yanıt: {body}")
            return True
    except Exception as ex:
        logger.error(f"❌ Slack bildirimi gönderilemedi: {ex}")
        return False

def get_mariadb_connection():
    return pymysql.connect(
        host=DB_HOST,
        user=DB_USER,
        password=DB_PASS,
        database=DB_NAME,
        port=DB_PORT,
        charset='utf8mb4',
        connect_timeout=5,
        read_timeout=5
    )

def main():
    logger.info("==================================================================")
    logger.info("🚀 OzBI MariaDB Müşteri Geri Bildirimi & SQL Daemon'ı Başlatıldı")
    logger.info(f"   Hedef Veritabanı: {DB_HOST} / {DB_NAME}")
    logger.info(f"   Kanal: #customer-feedback | Tarama Sıklığı: {POLL_INTERVAL_SECONDS}s")
    logger.info("==================================================================")

    state = load_state()
    logger.info(f"💾 Kalıcı snapshot durumu yüklendi: {len(state)} geri bildirim kayıtlı.")

    is_first_run = len(state) == 0

    while True:
        conn = None
        try:
            conn = get_mariadb_connection()
            with conn.cursor() as cur:
                cur.execute("""
                    SELECT 
                        m.Id, m.ChatId, m.Message, m.Query, m.Prompt, m.ErrorMessage, 
                        m.Summary, m.FeedbackReason, m.IsLiked, m.TotalDurationMs, m.DateCreated,
                        c.Title, t.Name, u.NameSurname, u.UserName, u.Email,
                        ai.Name, asi.Name
                    FROM chatmessage m
                    INNER JOIN chat c ON m.ChatId = c.Id
                    LEFT JOIN tenant t ON c.TenantId = t.Id
                    LEFT JOIN aspnetusers u ON c.CreatedByUserId = u.Id
                    LEFT JOIN aimodel ai ON m.AIModelId = ai.Id
                    LEFT JOIN asistant asi ON m.AsistantId = asi.Id
                    WHERE m.IsLiked = 0 OR (m.FeedbackReason IS NOT NULL AND TRIM(m.FeedbackReason) != '')
                    ORDER BY m.DateCreated DESC
                    LIMIT 100
                """)
                messages = cur.fetchall()

            state_updated = False
            for row in messages:
                (msg_id, chat_id, message, query, prompt, error_msg, 
                 summary, feedback_reason, is_liked, duration_ms, date_created,
                 chat_title, tenant_name, user_name, uname, email,
                 aimodel_name, assistant_name) = row

                tenant = tenant_name or "Bilinmeyen Tenant"
                display = user_name or uname or email or "Kullanıcı"
                email_str = email or uname or "E-posta yok"
                model_str = f"{aimodel_name or 'AI'}" + (f" · {assistant_name}" if assistant_name else "")
                date_str = date_created.strftime("%d.%m.%Y %H:%M:%S") if date_created else ""
                sql_formatted = format_sql(query)
                ai_resp = message or summary or ""
                prompt_str = prompt or chat_title or ""

                if msg_id not in state:
                    if is_first_run:
                        # İlk açılışta eski kayıtları snapshot'a alarak spam olmasını engelle
                        state[msg_id] = "InitialSeeded"
                        state_updated = True
                    else:
                        logger.info(f"🔥 YENİ GERİ BİLDİRİM / BEĞENİLMEYEN SQL! Tenant={tenant}, ID={msg_id}")
                        sent = send_slack_feedback_notification(
                            msg_id, chat_id, tenant, display, email_str, model_str,
                            date_str, bool(is_liked) if is_liked is not None else None,
                            feedback_reason, prompt_str, sql_formatted, ai_resp, error_msg, duration_ms
                        )
                        state[msg_id] = "Success" if sent else "Failed"
                        state_updated = True

            if is_first_run:
                logger.info(f"✅ İlk snapshot tamamlandı ({len(state)} kayıt). Canlı takip aktif!")
                is_first_run = False

            if state_updated:
                save_state(state)

        except pymysql.MySQLError as ex:
            logger.error(f"⚠️ MariaDB Hatası: {ex}")
        except Exception as ex:
            logger.error(f"⚠️ Genel Hata: {ex}")
        finally:
            if conn:
                try:
                    conn.close()
                except:
                    pass

        time.sleep(POLL_INTERVAL_SECONDS)

if __name__ == "__main__":
    main()
