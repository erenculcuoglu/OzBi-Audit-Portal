#!/usr/bin/env python3
"""Extended MariaDB watcher for demo.ozbiapp.com.tr login"""
import time, pymysql

def get_data():
    conn = pymysql.connect(
        host='elegance.odeaweb.com', user='ozbird', password='mW783p4t?',
        database='ozbiappc_app', charset='utf8mb4'
    )
    with conn.cursor() as cur:
        cur.execute("SELECT LoginCount FROM aspnetusers WHERE Id='08deb4b8-4c6b-46be-857c-a0ff0b151bf7'")
        count = cur.fetchone()[0]
    conn.close()
    return count

start_count = get_data()
print(f"👀 MariaDB ozbidemo LoginCount başlangıç: {start_count}")

for i in range(24): # 120s
    time.sleep(5)
    current = get_data()
    if current != start_count:
        print(f"🎉 GİRİŞ DETECTED! LoginCount {start_count} -> {current} oldu! (Slack bildirimi tetiklendi)")
        break

