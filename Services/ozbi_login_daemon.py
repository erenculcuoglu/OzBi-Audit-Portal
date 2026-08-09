#!/usr/bin/env python3
"""
OzBI MariaDB Tenant Login Monitor Daemon (Yaklaşım 2B)
------------------------------------------------------
MariaDB (elegance.odeaweb.com) veritabanını salt okunur (SELECT) sorgularla 3 saniyede bir tarar.
Herhangi bir kullanıcı/tenant giriş yaptığında (LoginCount arttığında) anında Slack #ozbi-login kanalına bildirim gönderir.
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
logger = logging.getLogger("OzBiLoginDaemon")

# Konfigürasyon
DB_HOST = "elegance.odeaweb.com"
DB_USER = "ozbird"
DB_PASS = "mW783p4t?"
DB_NAME = "ozbiappc_app"
DB_PORT = 3306
POLL_INTERVAL_SECONDS = 3

SLACK_WEBHOOK_B64 = "aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVDM3R0xSSlRGL0IwQk40UDczQzc5L0NKNE9vcjVSaXZxbzZkZnYwOEJxQ1NNMA=="
WEBHOOK_URL = base64.b64decode(SLACK_WEBHOOK_B64).decode('utf-8')

STATE_FILE = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "app", "login_daemon_state.json")

def load_state():
    if os.path.exists(STATE_FILE):
        try:
            with open(STATE_FILE, "r", encoding="utf-8") as f:
                return json.load(f)
        except Exception as e:
            logger.warning(f"State dosyası okunamadı, yeniden oluşturuluyor: {e}")
    return {}

def save_state(state):
    try:
        os.makedirs(os.path.dirname(STATE_FILE), exist_ok=True)
        with open(STATE_FILE, "w", encoding="utf-8") as f:
            json.dump(state, f, ensure_ascii=False, indent=2)
    except Exception as e:
        logger.error(f"State dosyası kaydedilemedi: {e}")

def send_slack_notification(tenant_name, display_name, email_str, login_count):
    turkey_time = datetime.now(timezone(timedelta(hours=3))).strftime("%d.%m.%Y %H:%M:%S")
    payload = {
        "text": f"🟢 *OzBI App Kullanıcı Girişi:* {tenant_name} / {display_name} ({email_str})",
        "blocks": [
            {
                "type": "header",
                "text": {"type": "plain_text", "text": "🚀 OzBI Uygulaması - Kullanıcı Girişi", "emoji": True}
            },
            {
                "type": "section",
                "fields": [
                    {"type": "mrkdwn", "text": f"*Firma / Tenant:*\n*{tenant_name}*"},
                    {"type": "mrkdwn", "text": f"*Kullanıcı:*\n{display_name}"},
                    {"type": "mrkdwn", "text": f"*E-Posta:*\n`{email_str}`"},
                    {"type": "mrkdwn", "text": f"*Toplam Giriş Sayısı:*\n`{login_count}`"},
                    {"type": "mrkdwn", "text": f"*Giriş Zamanı:*\n{turkey_time} (TSI)"}
                ]
            }
        ]
    }

    data = json.dumps(payload).encode('utf-8')
    req = urllib.request.Request(WEBHOOK_URL, data=data, headers={'Content-Type': 'application/json'})
    try:
        with urllib.request.urlopen(req, timeout=10) as resp:
            body = resp.read().decode('utf-8')
            logger.info(f"✅ Slack bildirimi gönderildi! [{tenant_name} / {display_name} -> {login_count}] Yanıt: {body}")
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
    logger.info("🚀 OzBI MariaDB Tenant Giriş Takip Daemon'ı (Yaklaşım 2B) Başlatıldı")
    logger.info(f"   Hedef Veritabanı: {DB_HOST} / {DB_NAME}")
    logger.info(f"   Tarama Sıklığı: Her {POLL_INTERVAL_SECONDS} saniyede bir")
    logger.info("==================================================================")

    state = load_state()
    logger.info(f"💾 Kalıcı snapshot durumu yüklendi: {len(state)} kullanıcı takip ediliyor.")

    is_first_run = len(state) == 0

    while True:
        conn = None
        try:
            conn = get_mariadb_connection()
            with conn.cursor() as cur:
                cur.execute("""
                    SELECT u.Id, u.NameSurname, u.Email, u.UserName, u.LoginCount, t.Name
                    FROM aspnetusers u
                    LEFT JOIN tenant t ON u.TenantId = t.Id
                    WHERE u.IsDeleted = 0
                """)
                users = cur.fetchall()

            state_updated = False
            for user in users:
                uid, name, email, uname, login_count, tenant_name = user
                uid_key = uid.lower()
                tenant = tenant_name or "Bilinmeyen Tenant"
                display = name or uname or email or uid
                email_str = email or uname or "E-posta yok"

                prev_count = state.get(uid_key)

                if prev_count is not None:
                    if login_count > prev_count:
                        logger.info(f"🔥 YENİ GİRİŞ TESPİT EDİLDİ! Tenant={tenant}, Kullanıcı={display}, Eski={prev_count} -> Yeni={login_count}")
                        state[uid_key] = login_count
                        state_updated = True
                        send_slack_notification(tenant, display, email_str, login_count)
                else:
                    # İlk defa görülen kullanıcı
                    state[uid_key] = login_count
                    state_updated = True
                    if not is_first_run and login_count > 0:
                        logger.info(f"🔥 YENİ KULLANICI GİRİŞİ! Tenant={tenant}, Kullanıcı={display}, LoginCount={login_count}")
                        send_slack_notification(tenant, display, email_str, login_count)

            if is_first_run:
                logger.info(f"✅ İlk kurulum tamamlandı. {len(state)} kullanıcı snapshot'a kaydedildi. Takip aktif!")
                is_first_run = False

            if state_updated:
                save_state(state)

        except pymysql.MySQLError as ex:
            logger.error(f"⚠️ MariaDB Bağlantı Hatası: {ex}")
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
