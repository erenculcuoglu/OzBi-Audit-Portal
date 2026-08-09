import os
import json
import base64
import urllib.request
import functions_framework
import pymysql
from datetime import datetime, timezone, timedelta
from google.cloud import storage

# MariaDB & Slack Konfigürasyonu
DB_HOST = "elegance.odeaweb.com"
DB_USER = "ozbird"
DB_PASS = "mW783p4t?"
DB_NAME = "ozbiappc_app"
SLACK_WEBHOOK_B64 = "aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVDM3R0xSSlRGL0IwQk40UDczQzc5L0NKNE9vcjVSaXZxbzZkZnYwOEJxQ1NNMA=="
WEBHOOK_URL = base64.b64decode(SLACK_WEBHOOK_B64).decode('utf-8')
BUCKET_NAME = "ozbi-login-monitor-state"

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
            print(f"✅ Slack notification delivered: {resp.read().decode('utf-8')}")
            return True
    except Exception as ex:
        print(f"❌ Slack error: {ex}")
        return False

@functions_framework.http
def check_mariadb_logins(request):
    """Google Cloud Function HTTP Entry Point"""
    # 1. State Yükle (GCS Bucket veya /tmp)
    state = {}
    storage_client = None
    try:
        storage_client = storage.Client()
        bucket = storage_client.bucket(BUCKET_NAME)
        blob = bucket.blob("login_state.json")
        if blob.exists():
            state = json.loads(blob.download_as_text())
    except Exception as ex:
        print(f"GCS State Load Warning: {ex}")
        if os.path.exists("/tmp/login_state.json"):
            try:
                with open("/tmp/login_state.json", "r") as f:
                    state = json.load(f)
            except:
                pass

    # 2. MariaDB Bağlantısı ve Tarama
    conn = pymysql.connect(
        host=DB_HOST, user=DB_USER, password=DB_PASS, database=DB_NAME,
        charset='utf8mb4', connect_timeout=5, read_timeout=5
    )
    with conn.cursor() as cur:
        cur.execute("""
            SELECT u.Id, u.NameSurname, u.Email, u.UserName, u.LoginCount, t.Name
            FROM aspnetusers u
            LEFT JOIN tenant t ON u.TenantId = t.Id
            WHERE u.IsDeleted = 0
        """)
        users = cur.fetchall()
    conn.close()

    updated = False
    new_logins = []

    for u in users:
        uid, name, email, uname, login_count, tenant_name = u
        uid_key = uid.lower()
        tenant = tenant_name or "Bilinmeyen Tenant"
        display = name or uname or email or uid
        email_str = email or uname or "E-posta yok"

        prev_count = state.get(uid_key)
        state[uid_key] = login_count

        if prev_count is not None and login_count > prev_count:
            print(f"🔥 YENİ GİRİŞ TESPİT EDİLDİ: {tenant} / {display} ({prev_count} -> {login_count})")
            send_slack_notification(tenant, display, email_str, login_count)
            new_logins.append(f"{tenant}: {display} (Count: {login_count})")
            updated = True
        elif prev_count is None:
            updated = True

    # 3. State Kaydet
    if updated:
        if storage_client:
            try:
                bucket = storage_client.bucket(BUCKET_NAME)
                if not bucket.exists():
                    bucket = storage_client.create_bucket(BUCKET_NAME)
                blob = bucket.blob("login_state.json")
                blob.upload_from_string(json.dumps(state, ensure_ascii=False, indent=2))
            except Exception as ex:
                print(f"GCS Save Warning: {ex}")
                with open("/tmp/login_state.json", "w") as f:
                    json.dump(state, f)
        else:
            with open("/tmp/login_state.json", "w") as f:
                json.dump(state, f)

    return json.dumps({
        "status": "success",
        "processed_users": len(users),
        "new_logins": new_logins
    }), 200, {'Content-Type': 'application/json'}
