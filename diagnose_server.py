#!/usr/bin/env python3
"""Directly create UserLoginSnapshots table in MonsterASP's ozbi_audit.db"""
import paramiko, sqlite3, os

SFTP_HOST = "site83172.siteasp.net"
SFTP_USER = "site83172"
SFTP_PASS = "Dw4_a!T2Qr3="
REMOTE_PATH = "/wwwroot/app/ozbi_audit.db"
LOCAL_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "patch_ozbi_audit.db")

def main():
    transport = paramiko.Transport((SFTP_HOST, 22))
    transport.connect(username=SFTP_USER, password=SFTP_PASS)
    sftp = paramiko.SFTPClient.from_transport(transport)

    print("📥 Downloading ozbi_audit.db from MonsterASP...")
    try:
        sftp.get(REMOTE_PATH, LOCAL_PATH)
    except FileNotFoundError:
        print("   Database file not found, creating new one...")

    conn = sqlite3.connect(LOCAL_PATH)
    cur = conn.cursor()

    print("🛠️ Creating UserLoginSnapshots table in SQLite...")
    cur.execute("""
        CREATE TABLE IF NOT EXISTS UserLoginSnapshots (
            UserId TEXT PRIMARY KEY,
            LastSeenLoginCount INTEGER NOT NULL,
            LastUpdatedAt TEXT NOT NULL
        )
    """)
    conn.commit()

    cur.execute("SELECT name FROM sqlite_master WHERE type='table'")
    tables = [r[0] for r in cur.fetchall()]
    print(f"✅ SQLite Tabloları: {tables}")

    conn.close()

    print("📤 Uploading patched ozbi_audit.db back to MonsterASP...")
    sftp.put(LOCAL_PATH, REMOTE_PATH)

    sftp.close()
    transport.close()

    if os.path.exists(LOCAL_PATH):
        os.remove(LOCAL_PATH)

    print("🎉 SQLite veritabanı başarıyla güncellendi!")

if __name__ == "__main__":
    main()
