#!/usr/bin/env python3
"""Download ozbi_audit.db from MonsterASP and check UserLoginSnapshots table"""
import paramiko, sqlite3, os

SFTP_HOST = "site83172.siteasp.net"
SFTP_USER = "site83172"
SFTP_PASS = "Dw4_a!T2Qr3="
REMOTE_PATH = "/wwwroot/app/ozbi_audit.db"
LOCAL_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "downloaded_ozbi_audit.db")

def main():
    transport = paramiko.Transport((SFTP_HOST, 22))
    transport.connect(username=SFTP_USER, password=SFTP_PASS)
    sftp = paramiko.SFTPClient.from_transport(transport)

    print("📥 Downloading ozbi_audit.db from MonsterASP...")
    sftp.get(REMOTE_PATH, LOCAL_PATH)
    sftp.close()
    transport.close()

    print("🔍 Reading downloaded SQLite DB...")
    conn = sqlite3.connect(LOCAL_PATH)
    cur = conn.cursor()

    cur.execute("SELECT name FROM sqlite_master WHERE type='table'")
    tables = [r[0] for r in cur.fetchall()]
    print(f"   Tablolar: {tables}")

    if "UserLoginSnapshots" in tables:
        cur.execute("SELECT UserId, LastSeenLoginCount, LastUpdatedAt FROM UserLoginSnapshots")
        rows = cur.fetchall()
        print(f"   UserLoginSnapshots Kayıt Sayısı: {len(rows)}")
        for r in rows:
            print(f"     - UserId: {r[0]}, LastSeenLoginCount: {r[1]}, LastUpdatedAt: {r[2]}")
    else:
        print("   ❌ UserLoginSnapshots TABLOSU YOK!")

    conn.close()
    if os.path.exists(LOCAL_PATH):
        os.remove(LOCAL_PATH)

if __name__ == "__main__":
    main()
