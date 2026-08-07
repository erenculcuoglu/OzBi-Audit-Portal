#!/usr/bin/env python3
"""Check web.config on MonsterASP via SFTP"""
import paramiko

SFTP_HOST = "site83172.siteasp.net"
SFTP_USER = "site83172"
SFTP_PASS = "Dw4_a!T2Qr3="
REMOTE_PATH = "/wwwroot/web.config"

def main():
    transport = paramiko.Transport((SFTP_HOST, 22))
    transport.connect(username=SFTP_USER, password=SFTP_PASS)
    sftp = paramiko.SFTPClient.from_transport(transport)

    print("📥 Reading web.config from MonsterASP...")
    with sftp.file(REMOTE_PATH, 'r') as f:
        content = f.read()
        print(f"   İçerik:\n{content}")

    sftp.close()
    transport.close()

if __name__ == "__main__":
    main()
