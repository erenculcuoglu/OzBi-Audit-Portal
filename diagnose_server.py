#!/usr/bin/env python3
"""Compare MariaDB aspnetusers LoginCount vs SQLite UserLoginSnapshots"""
import paramiko, sqlite3, os, pymysql

SFTP_HOST = "site83172.siteasp.net"
SFTP_USER = "site83172"
SFTP_PASS = "Dw4_a!T2Qr3="
REMOTE_PATH = "/wwwroot/app/ozbi_audit.db"
LOCAL_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "cmp_ozbi_audit.db")

def main():
    # 1. Fetch SQLite snapshots
    transport = paramiko.Transport((SFTP_HOST, 22))
    transport.connect(username=SFTP_USER, password=SFTP_PASS)
    sftp = paramiko.SFTPClient.from_transport(transport)
    sftp.get(REMOTE_PATH, LOCAL_PATH)
    sftp.close()
    transport.close()

    conn_sqlite = sqlite3.connect(LOCAL_PATH)
    cur_sqlite = conn_sqlite.cursor()
    cur_sqlite.execute("SELECT UserId, LastSeenLoginCount FROM UserLoginSnapshots")
    sqlite_map = {r[0]: r[1] for r in cur_sqlite.fetchall()}
    conn_sqlite.close()

    if os.path.exists(LOCAL_PATH):
        os.remove(LOCAL_PATH)

    print(f"📊 SQLite'ta {len(sqlite_map)} kullanıcı kaydedilmiş.")

    # 2. Fetch MariaDB users
    conn_mariadb = pymysql.connect(
        host='elegance.odeaweb.com', user='ozbird', password='mW783p4t?',
        database='ozbiappc_app', charset='utf8mb4'
    )
    with conn_mariadb.cursor() as cur:
        cur.execute("""
            SELECT u.Id, u.NameSurname, u.Email, u.UserName, u.LoginCount, t.Name
            FROM aspnetusers u
            LEFT JOIN tenant t ON u.TenantId = t.Id
            WHERE u.IsDeleted = 0
        """)
        mariadb_users = cur.fetchall()
    conn_mariadb.close()

    print(f"📊 MariaDB'de {len(mariadb_users)} kullanıcı var.")
    print()

    diff_found = False
    for u in mariadb_users:
        uid, name, email, uname, m_count, tenant = u
        s_count = sqlite_map.get(uid)
        
        display = name or uname or email or uid
        tenant_name = tenant or "Bilinmeyen Tenant"
        
        if s_count is None:
            print(f"🆕 DİKKAT: Yeni Kullanıcı! Firma: {tenant_name}, Kullanıcı: {display}, MariaDB LoginCount: {m_count} (SQLite'ta henüz yok)")
            diff_found = True
        elif m_count > s_count:
            print(f"🔔 DİKKAT: Giriş Yapıldı! Firma: {tenant_name}, Kullanıcı: {display}, SQLite: {s_count} -> MariaDB: {m_count}")
            diff_found = True

    if not diff_found:
        print("✅ MariaDB ile SQLite %100 birebir senkronize! Son snapshot alındığından beri MariaDB'de YENİ HİÇBİR GİRİŞ YAPILMAMIŞ.")

if __name__ == "__main__":
    main()
