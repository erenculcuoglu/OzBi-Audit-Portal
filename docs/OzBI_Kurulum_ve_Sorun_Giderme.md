# OzBI — Müşteri Kurulum & Sorun Giderme Dokümantasyonu

> **Versiyon**: 1.0  
> **Tarih**: 20 Ağustos 2026  
> **Uygulama**: AgentBi.exe  
> **Platform**: Windows Server / Windows 10+ (x64)  
> **Runtime**: .NET 8.0  
> **Varsayılan Port**: 8500

---

## İçindekiler

1. [Sistem Gereksinimleri](#1-sistem-gereksinimleri)
2. [Kurulum Öncesi Checklist](#2-kurulum-öncesi-checklist)
3. [Adım Adım Kurulum](#3-adım-adım-kurulum)
4. [Windows Servisi Yapılandırması](#4-windows-servisi-yapılandırması)
5. [Port ve Ağ Yapılandırması](#5-port-ve-ağ-yapılandırması)
6. [Veritabanı Bağlantısı](#6-veritabanı-bağlantısı)
7. [Sık Karşılaşılan Hatalar ve Çözümleri](#7-sık-karşılaşılan-hatalar-ve-çözümleri)
8. [Hızlı Tanı Akış Şeması](#8-hızlı-tanı-akış-şeması)
9. [Sağlık Kontrol Checklist'i](#9-sağlık-kontrol-checklisti)
10. [Güncelleme Prosedürü](#10-güncelleme-prosedürü)
11. [Uzak Destek Rehberi](#11-uzak-destek-rehberi)

---

## 1. Sistem Gereksinimleri

### Donanım

| Bileşen | Minimum | Önerilen |
|---|---|---|
| **İşlemci** | 2 çekirdek x64 | 4+ çekirdek x64 |
| **RAM** | 4 GB | 8+ GB |
| **Disk** | 10 GB boş alan | 50+ GB (loglar dahil) |
| **Ağ** | 100 Mbps | 1 Gbps |

### Yazılım

| Bileşen | Gereksinim | Kontrol Komutu |
|---|---|---|
| **İşletim Sistemi** | Windows Server 2016+ veya Windows 10+ (x64) | `systeminfo` |
| **.NET 8.0 Runtime** | ASP.NET Core Hosting Bundle 8.0.x (x64) | `dotnet --list-runtimes` |
| **Veritabanı** | MSSQL Server'a ağ erişimi (Logo/Mikro DB) | `Test-NetConnection <DB_IP> -Port 1433` |
| **Port** | 8500 (veya özel port) müsait olmalı | `netstat -ano \| findstr :8500` |

> ⚠️ **KRİTİK**: Sadece ".NET Desktop Runtime" kurmak **yetmez**. Web uygulaması çalıştırmak için **ASP.NET Core Hosting Bundle** şarttır. Bu paket .NET Runtime + ASP.NET Core Runtime + IIS modülünü tek seferde kurar.

---

## 2. Kurulum Öncesi Checklist

Müşteri lokasyonuna gitmeden önce bu listeyi tamamlayın:

- [ ] Sunucu IP adresi ve RDP erişim bilgileri alındı
- [ ] Sunucuda **Administrator** yetkisi var
- [ ] Logo/Mikro **veritabanı bağlantı bilgileri** alındı (IP, port, DB adı, kullanıcı, şifre)
- [ ] Logo/Mikro **firma numarası** (XXX) ve **dönem numarası** (YY) bilgisi alındı
- [ ] Sunucudan veritabanı sunucusuna **ağ erişimi** doğrulandı
- [ ] Port **8500** müsait (veya müşteriye özel alternatif port belirlendi)
- [ ] OzBI kurulum paketinin **son versiyonu** USB/paylaşıma hazır
- [ ] **Hosting Bundle** installer'ı yedek olarak USB'de mevcut (internet olmayabilir)

---

## 3. Adım Adım Kurulum

### Adım 1 — .NET 8.0 Hosting Bundle Kurulumu

> 🔴 **Bu adım mutlaka İLK yapılmalıdır.** Runtime olmadan uygulama çalışmaz.

**İndirme:**
1. https://dotnet.microsoft.com/en-us/download/dotnet/8.0 adresine gidin
2. **"ASP.NET Core Runtime 8.0.x"** bölümüne inin
3. **Windows** satırında **"Hosting Bundle"** linkine tıklayın
4. `dotnet-hosting-8.0.x-win.exe` dosyasını indirin

**Kurulum:**
```powershell
# Administrator olarak çalıştırın
.\dotnet-hosting-8.0.30-win.exe /install /quiet /norestart
```

**Doğrulama:**
```powershell
dotnet --list-runtimes
```

Beklenen çıktı:
```
Microsoft.AspNetCore.App 8.0.30 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
Microsoft.NETCore.App 8.0.30 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
```

> 💡 **İpucu**: Müşteri lokasyonunda internet erişimi olmayabilir. Hosting Bundle installer'ı her zaman USB'nizde bulundurun.

---

### Adım 2 — Uygulama Dosyalarını Kopyalama

```powershell
# Hedef dizini oluştur
New-Item -ItemType Directory -Path "C:\Program Files\OzBi" -Force

# Kurulum dosyalarını kopyala
Copy-Item -Path "D:\Kurulum\OzBi\*" -Destination "C:\Program Files\OzBi\" -Recurse -Force
```

Beklenen dizin yapısı:
```
C:\Program Files\OzBi\
├── AgentBi.exe               ← Ana uygulama
├── appsettings.json           ← Yapılandırma (DB, port vb.)
├── appsettings.Production.json
├── wwwroot\                   ← Statik dosyalar
├── *.dll                      ← Uygulama kütüphaneleri
└── logs\                      ← Log dizini (otomatik oluşur)
```

---

### Adım 3 — Yapılandırma (appsettings.json)

`C:\Program Files\OzBi\appsettings.json` dosyasını düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<MSSQL_IP>,1433;Database=<LOGO_DB_ADI>;User Id=<KULLANICI>;Password=<SIFRE>;TrustServerCertificate=True;Encrypt=False;"
  },
  "AppSettings": {
    "Port": 8500,
    "FirmaNo": "001",
    "DonemNo": "01"
  }
}
```

| Alan | Açıklama | Örnek |
|---|---|---|
| `<MSSQL_IP>` | Veritabanı sunucu IP | `192.168.1.100` |
| `<LOGO_DB_ADI>` | Logo veritabanı adı | `TIGERDB` |
| `<KULLANICI>` | SQL Server kullanıcısı | `sa` |
| `<SIFRE>` | SQL Server şifresi | `Logo1234!` |
| `FirmaNo` | Logo firma numarası (3 hane) | `001` |
| `DonemNo` | Logo dönem numarası (2 hane) | `01` |

---

### Adım 4 — Manuel Test (Servis Kurmadan Önce!)

> ⚠️ Servisi kurmadan önce **mutlaka elle çalıştırıp test edin**. Servis olarak çalıştırmak hata mesajlarını gizler.

```powershell
cd "C:\Program Files\OzBi"
.\AgentBi.exe
```

**Başarılı çıktı:**
```
info: Now listening on: http://0.0.0.0:8500
info: Application started. Press Ctrl+C to shut down.
```

Tarayıcıda `http://localhost:8500` açın. Ekran geliyorsa → Adım 5'e geçin.

Hata alıyorsanız → Bölüm 7: Sık Karşılaşılan Hatalar kısmına bakın.

---

## 4. Windows Servisi Yapılandırması

### Servis Oluşturma

```powershell
# Administrator PowerShell'de:
sc.exe create OzBiService binPath= "\"C:\Program Files\OzBi\AgentBi.exe\"" DisplayName= "OzBi Agent Service" start= auto obj= "LocalSystem"
```

> ⚠️ `binPath=` den sonraki **boşluk** zorunludur. `sc.exe` sözdizimi bunu gerektirir.

### Kurtarma (Recovery) Ayarları

Crash sonrası otomatik restart:

```powershell
sc.exe failure OzBiService reset= 86400 actions= restart/60000/restart/60000/restart/60000
```

(60 sn bekle, 3 kez dene, 24 saat sonra sayaç sıfırla)

### Başlatma

```powershell
Start-Service OzBiService
Get-Service OzBiService    # Durum kontrolü
```

### Servis Silme (Gerekirse)

```powershell
Stop-Service OzBiService
sc.exe delete OzBiService
```

---

## 5. Port ve Ağ Yapılandırması

### Port Kontrolü

```powershell
# 8500 portunu kim kullanıyor?
netstat -ano | findstr :8500

# Sonuç boşsa → Kimse kullanmıyor (iyi)
# LISTENING varsa → PID'den işlemi bul:
tasklist /FI "PID eq <PID>"
```

### Firewall Kuralı (Uzak Erişim Gerekiyorsa)

```powershell
New-NetFirewallRule -DisplayName "OzBi Agent (8500)" -Direction Inbound -Protocol TCP -LocalPort 8500 -Action Allow -Profile Any
```

### Firewall Kural Silme

```powershell
Remove-NetFirewallRule -DisplayName "OzBi Agent (8500)"
```

---

## 6. Veritabanı Bağlantısı

### Bağlantı Testi

```powershell
# Port erişim testi
Test-NetConnection -ComputerName <DB_IP> -Port 1433
```

Beklenen: `TcpTestSucceeded : True`

### Logo Tablo Erişim Testi

```sql
-- Cari kart erişim kontrolü (firma 001, dönem 01)
SELECT TOP 5 C.[CODE], C.[DEFINITION_]
FROM LG_001_CLCARD C WITH (NOLOCK)
WHERE C.[ACTIVE] = 0
ORDER BY C.[CODE];
```

### Yaygın DB Sorunları

| Belirti | Sebep | Çözüm |
|---|---|---|
| `Login failed for user` | Yanlış kullanıcı/şifre | Connection string kontrol edin |
| `Cannot open database` | DB adı yanlış veya yetki yok | DB adını ve kullanıcı yetkilerini kontrol edin |
| `A network-related error` | DB sunucusuna erişilemiyor | IP/port, firewall, SQL Browser servisi kontrol |
| `TCP/IP not enabled` | SQL Server TCP/IP kapalı | SQL Server Configuration Manager → Protocols → TCP/IP → Enable |
| Tablo bulunamıyor | Firma/dönem no yanlış | `LG_001_01_CLCARD` → 001=firma, 01=dönem |

---

## 7. Sık Karşılaşılan Hatalar ve Çözümleri

---

### 🔴 HATA 1: `hostfxr.dll not found` (Error 0x80008083)

**Ekran:**
```
You must install .NET to run this application.
App: C:\Program Files\OzBi\AgentBi.exe
Architecture: x64
App host version: 8.0.30
.NET location: Not found
Failed to resolve hostfxr.dll [not found].
Error code: 0x80008083
```

**Sebep**: .NET 8.0 Runtime sunucuya hiç kurulmamış.

**Çözüm**:
1. https://dotnet.microsoft.com/en-us/download/dotnet/8.0 → **Hosting Bundle** indir
2. Administrator olarak kur
3. Sunucuyu **restart** et
4. `dotnet --list-runtimes` ile doğrula

**Dikkat**: Sadece ".NET Desktop Runtime" kurmak yetmez. **Hosting Bundle** veya **ASP.NET Core Runtime** şart.

---

### 🔴 HATA 2: Windows Hata 1053 — Servis Zamanında Yanıt Vermedi

**Ekran:**
```
Windows Yerel Bilgisayar üzerindeki OzBiService hizmetini başlatamadı.
Hata 1053: Hizmet, belirli aralıklarla yapılan başlama veya
denetim isteğine yanıt vermedi.
```

**Sebep**: Servis başlatma sırasında crash oluyor veya timeout'a düşüyor.

**Tanı sırası:**

| # | Kontrol | Komut | Aranan |
|---|---|---|---|
| 1 | .NET Runtime yüklü mü? | `dotnet --list-runtimes` | AspNetCore.App 8.0.x olmalı |
| 2 | Elle çalışıyor mu? | `cd "C:\Program Files\OzBi" && .\AgentBi.exe` | Konsol hatası oku |
| 3 | Port müsait mi? | `netstat -ano \| findstr :8500` | Boş olmalı |
| 4 | Config doğru mu? | `appsettings.json` aç | JSON syntax + DB bilgileri |
| 5 | Event Log | `eventvwr.msc → Application` | OzBi/AgentBi hataları |
| 6 | Dosyalar tam mı? | `dir "C:\Program Files\OzBi"` | AgentBi.exe + DLL'ler |

**En hızlı teşhis**: Elle çalıştırma (`.\AgentBi.exe`) — konsola düşen hata direkt sebebi gösterir.

---

### 🔴 HATA 3: ERR_CONNECTION_REFUSED (Tarayıcı)

**Ekran:**
```
Bu siteye ulaşılamıyor
localhost bağlanmayı reddetti.
ERR_CONNECTION_REFUSED
```

**Sebep**: İstenen portta hiçbir uygulama dinlemiyor.

**Tanı:**
```powershell
# 1. Servis çalışıyor mu?
Get-Service OzBiService

# 2. 8500'de kim dinliyor?
netstat -ano | findstr :8500

# 3. Belki farklı portta açılmıştır?
netstat -ano | findstr LISTENING
```

**Olası durumlar:**

| Durum | Anlam | Çözüm |
|---|---|---|
| Servis: Stopped | Servis hiç başlamamış | Servisi başlat, hata alırsan Hata 2'ye bak |
| Port 8500: Boş | Uygulama o portta değil | Config'deki port ayarını kontrol et |
| Farklı portta LISTENING | Port yanlış yapılandırılmış | `appsettings.json`'da doğru portu ayarla |
| Servis: Running + Port: Boş | Başladı ama hemen crash oldu | Log dosyasını kontrol et |

---

### 🟡 HATA 4: Uygulama Açılıyor Ama Veriler Gelmiyor

| # | Sebep | Kontrol | Çözüm |
|---|---|---|---|
| 1 | Firma numarası yanlış | `appsettings.json` → FirmaNo | Doğru 3 haneli firma no gir (ör: 001) |
| 2 | Dönem numarası yanlış | `appsettings.json` → DonemNo | Doğru 2 haneli dönem no gir (ör: 01) |
| 3 | SQL kullanıcı yetkisi yok | SSMS'de test sorgusu çalıştır | DBO veya SELECT yetkisi ver |
| 4 | Logo tabloları farklı | `SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE 'LG_%'` | Gerçek tablo adlarını doğrula |

---

### 🟡 HATA 5: Servis Başlıyor, Birkaç Dakika Sonra Duruyor

**Kontrol:**
```powershell
# Log dosyasını oku
Get-Content "C:\Program Files\OzBi\logs\*.log" -Tail 50

# Event Viewer
Get-EventLog -LogName Application -Newest 20 -EntryType Error |
    Where-Object { $_.Source -like "*OzBi*" -or $_.Source -like "*AgentBi*" -or $_.Source -like "*.NET*" } |
    Format-List TimeGenerated, Source, Message
```

---

### 🟡 HATA 6: `Access Denied` veya Yetki Sorunları

**Çözüm:**
```powershell
# Servis hesabını LocalSystem yap
sc.exe config OzBiService obj= "LocalSystem"

# Kurulum dizinine tam yetki ver
icacls "C:\Program Files\OzBi" /grant "NETWORK SERVICE:(OI)(CI)F" /T
```

---

### 🟡 HATA 7: Port Çakışması

**Teşhis:**
```powershell
netstat -ano | findstr :8500
# TCP  0.0.0.0:8500  LISTENING  <PID>

tasklist /FI "PID eq <PID>"
```

**Çözüm**: Çakışan uygulamayı kapat veya OzBI'yi farklı porta taşı.

---

## 8. Hızlı Tanı Akış Şeması

```
localhost:8500 açılmıyor
        │
        ▼
   Servis çalışıyor mu?
   (Get-Service OzBiService)
        │
    ┌───┴───┐
    │       │
  Evet    Hayır
    │       │
    ▼       ▼
  Port     Servisi başlat
  dinliyor  (Start-Service)
  mu?       │
    │       ├── Başladı → Port kontrolüne geç
    │       │
    │       └── HATA 1053 alındı
    │               │
    │               ▼
    │          .NET Runtime yüklü mü?
    │          (dotnet --list-runtimes)
    │               │
    │           ┌───┴───┐
    │         Evet    Hayır
    │           │       │
    │           ▼       ▼
    │       Elle      Hosting Bundle
    │       çalıştır  kur + restart
    │       (AgentBi.exe)
    │           │
    │           ▼
    │       Konsol hatasını oku
    │       (DB? Config? DLL?)
    │
    ▼
  netstat -ano | findstr :8500
        │
    ┌───┴───┐
  Boş   LISTENING
    │       │
    ▼       ▼
  Uygulama  http://localhost:8500
  crash     dene (https değil!)
  olmuş.
  Log'a bak.
```

---

## 9. Sağlık Kontrol Checklist'i

### Altyapı
- [ ] `dotnet --list-runtimes` → `AspNetCore.App 8.0.x` mevcut
- [ ] `AgentBi.exe` dosyası mevcut
- [ ] `appsettings.json` müşteriye özel yapılandırıldı
- [ ] DB bağlantı testi başarılı
- [ ] Port 8500 müsait

### Servis
- [ ] `Get-Service OzBiService` → Status: Running
- [ ] Başlangıç türü: Automatic
- [ ] Recovery ayarları yapıldı

### Fonksiyonel
- [ ] `http://localhost:8500` açılıyor
- [ ] Login ekranı gözüküyor
- [ ] Giriş yapılabiliyor
- [ ] Cari listesi yükleniyor
- [ ] Sorgu sonuçları dönüyor

### Uzak Erişim (Gerekiyorsa)
- [ ] Firewall kuralı eklendi
- [ ] `http://<SUNUCU_IP>:8500` başka makineden açılıyor

---

## 10. Güncelleme Prosedürü

```powershell
# 1. Servisi durdur
Stop-Service OzBiService

# 2. Yedek al
$backup = "C:\OzBi_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
Copy-Item -Path "C:\Program Files\OzBi" -Destination $backup -Recurse

# 3. Config'i koru
Copy-Item "C:\Program Files\OzBi\appsettings.json" "$env:TEMP\appsettings_backup.json"

# 4. Yeni dosyaları kopyala
Copy-Item -Path "D:\Guncelleme\OzBi\*" -Destination "C:\Program Files\OzBi\" -Recurse -Force

# 5. Config'i geri koy
Copy-Item "$env:TEMP\appsettings_backup.json" "C:\Program Files\OzBi\appsettings.json" -Force

# 6. Servisi başlat
Start-Service OzBiService
Get-Service OzBiService
```

> ⚠️ **appsettings.json** dosyasını kesinlikle koruyun! Müşteriye özel ayarlar buradadır.

---

## 11. Uzak Destek Rehberi

### Tanı Script'i (Müşteriye Gönder)

```powershell
# Müşteriye tek script olarak çalıştırtın — masaüstüne rapor oluşturur
$r = @()
$r += "=== .NET RUNTIME ===" + "`n" + (dotnet --list-runtimes 2>&1 | Out-String)
$r += "=== SERVİS ===" + "`n" + (Get-Service OzBiService 2>&1 | Format-List | Out-String)
$r += "=== PORT 8500 ===" + "`n" + (netstat -ano | findstr ":8500" | Out-String)
$r += "=== SON HATALAR ===" + "`n" + (Get-EventLog -LogName Application -Newest 10 -EntryType Error 2>&1 | Select-Object TimeGenerated, Source, Message | Format-List | Out-String)
$p = "$env:USERPROFILE\Desktop\OzBi_Tani.txt"
$r | Out-File $p -Encoding UTF8
notepad $p
```

### Telefon Desteği Hızlı Referans

| Müşteri Ne Diyor? | İlk Söylenecek |
|---|---|
| "Sayfa açılmıyor" | `Get-Service OzBiService` çalıştır |
| "Servis başlamıyor" | `dotnet --list-runtimes` çalıştır |
| "Dün çalışıyordu bugün durdu" | Servis StartType: Automatic mı? |
| "Giriş yapamıyorum" | `Test-NetConnection <DB_IP> -Port 1433` |
| "Veri gelmiyor" | Firma/dönem no kontrol |
| "Çok yavaş" | `taskmgr` ile RAM/CPU kontrol |

---

> 📌 **Bu doküman OzBI kurulum ve destek ekibinin iç kullanımı içindir.**  
> Son güncelleme: 20 Ağustos 2026
