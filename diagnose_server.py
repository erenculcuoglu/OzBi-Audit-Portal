#!/usr/bin/env python3
"""Check connection strings in appsettings.json and appsettings.Production.json on MonsterASP"""
import paramiko, json

SFTP_HOST = "site83172.siteasp.net"
SFTP_USER = "site83172"
SFTP_PASS = "Dw4_a!T2Qr3="
REMOTE_DIR = "/wwwroot"

def main():
    transport = paramiko.Transport((SFTP_HOST, 22))
    transport.connect(username=SFTP_USER, password=SFTP_PASS)
    sftp = paramiko.SFTPClient.from_transport(transport)

    for fname in ["appsettings.json", "appsettings.Production.json"]:
        print(f"📄 {fname}:")
        try:
            with sftp.file(f"{REMOTE_DIR}/{fname}", 'r') as f:
                content = f.read()
                print(f"   İçerik:\n{content}")
        except Exception as e:
            print(f"   ❌ Hata: {e}")

    sftp.close()
    transport.close()

if __name__ == "__main__":
    main()
