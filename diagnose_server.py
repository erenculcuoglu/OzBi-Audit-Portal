#!/usr/bin/env python3
"""Check SQLite on MonsterASP after deploy and ping for 136 update"""
import paramiko, sqlite3, os

SFTP_HOST = "site83172.siteasp.net"
SFTP_USER = "site83172"
SFTP_PASS = "Dw4_a!T2Qr3="
REMOTE_PATH = "/wwwroot/app/ozbi_audit.db"
LOCAL_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "final_136_check.db")

def main():
    transport = paramiko.Transport((SFTP_HOST, 22))
    transport.connect(username=SFTP_USER, password=SFTP_PASS)
    sftp = paramiko.SFTPClient.from_transport(transport)
    sftp.get(REMOTE_PATH, LOCAL_PATH)
    sftp.close()
    transport.close()

    conn_sqlite = sqlite3.connect(LOCAL_PATH)
    cur_sqlite = conn_sqlite.cursor()
    cur_sqlite.execute("SELECT UserId, LastSeenLoginCount, LastUpdatedAt FROM UserLoginSnapshots WHERE UserId='08deb4b8-4c6b-46be-857c-a0ff0b151bf7'")
    r = cur_sqlite.fetchone()
    print(f"💾 SQLite ozbidemo: {r}")
    conn_sqlite.close()

    if os.path.exists(LOCAL_PATH):
        os.remove(LOCAL_PATH)

if __name__ == "__main__":
    main()
