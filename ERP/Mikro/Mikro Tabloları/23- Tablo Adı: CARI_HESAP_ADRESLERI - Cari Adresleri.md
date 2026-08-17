# TABLO NO: 32

## Tablo Adı: CARI_HESAP_ADRESLERI - Cari Adresleri

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | adr_Guid | Uniqueidentifier |  |  |
| 1 | adr_DBCno | Smallint |  |  |
| 2 | adr_SpecRECno | Integer |  |  |
| 3 | adr_iptal | Bit |  |  |
| 4 | adr_fileid | Smallint |  |  |
| 5 | adr_hidden | Bit |  |  |
| 6 | adr_kilitli | Bit |  |  |
| 7 | adr_degisti | Bit |  |  |
| 8 | adr_checksum | Integer |  |  |
| 9 | adr_create_user | Smallint |  |  |
| 10 | adr_create_date | DateTime |  |  |
| 11 | adr_lastup_user | Smallint |  |  |
| 12 | adr_lastup_date | DateTime |  |  |
| 13 | adr_special1 | Nvarchar(127) |  |  |
| 14 | adr_special2 | Nvarchar(127) |  |  |
| 15 | adr_special3 | Nvarchar(127) |  |  |
| 16 | adr_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | adr_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | adr_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | adr_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | adr_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | adr_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | adr_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | adr_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | adr_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | adr_cari_kod | Nvarchar(25) | Cari Kodu | Bkz. CARI_HESAPLAR |
| 26 | adr_adres_no | Integer | Adres No |  |
| 27 | adr_aprint_fl | Bit | Yazıcıya Basılacak Mı? | 0:Hayır 1:Evet    Yazıcıya basıldıktan sonra bu alan tekrar 0 durumuna gelir. |
| 28 | adr_cadde | Nvarchar(127) | Cadde |  |
| 29 | adr_mahalle | Nvarchar(127) | Mahalle |  |
| 30 | adr_sokak | Nvarchar(127) | Sokak |  |
| 31 | adr_Semt | Nvarchar(25) | Semt |  |
| 32 | adr_Apt_No | Nvarchar(10) | Apartman Numarası |  |
| 33 | adr_Daire_No | Nvarchar(10) | Daire Numarası |  |
| 34 | adr_posta_kodu | Nvarchar(8) | Posta Kodu |  |
| 35 | adr_ilce | Nvarchar(50) | İlçe |  |
| 36 | adr_il | Nvarchar(50) | İl |  |
| 37 | adr_ulke | Nvarchar(50) | Ülke |  |
| 38 | adr_Adres_kodu | Nvarchar(10) | Adres Kodu |  |
| 39 | adr_tel_ulke_kodu | Nvarchar(5) | Ülke Telefon Kodu |  |
| 40 | adr_tel_bolge_kodu | Nvarchar(5) | Bölge Telefon Kodu |  |
| 41 | adr_tel_no1 | Nvarchar(10) | Telefon No 1 |  |
| 42 | adr_tel_no2 | Nvarchar(10) | Telefon No 2 |  |
| 43 | adr_tel_faxno | Nvarchar(10) | Fax No |  |
| 44 | adr_tel_modem | Nvarchar(10) | Modem No |  |
| 45 | adr_yon_kodu | Nvarchar(4) | Yön Kodu |  |
| 46 | adr_uzaklik_kodu | Smallint | Uzaklık Kodu |  |
| 47 | adr_temsilci_kodu | Nvarchar(25) | Temsilci Kodu |  |
| 48 | adr_ozel_not | Nvarchar(127) | Özel Not |  |
| 49 | adr_ziyaretperyodu | Tinyint | Ziyaret Peryodu | 0:Ziyaret edilmeyecek 1:Her gün 2:Haftada bir 3:Onbeş günde bir 4:Ayda bir 5:Üç ayda bir 6:Altı ayda bir 7:Yılda bir 8:Üç haftada bir 9:Haftada iki 10:Haftada üç 11:Haftada dört 12:Haftada beş |
| 50 | adr_ziyaretgunu | Float | Ziyaret Günü | Ziyaret peryodundan yaptığınız tercihe göre seçim yapabileceksiniz. Mesela ziyaret peryodunu "Haftada bir" olarak seçerseniz, ziyaret günü olarak haftanın yedi gününden birini seçme hakkına sahip olacaksınız. (1:Pazartesi 2:Salı 3:Çarşamba 4:Perşembe 5:Cuma 6:Cumartesi 7:Pazar) |
| 51 | adr_gps_enlem | Float | GPS Enlem | +Kuzey -Guney |
| 52 | adr_gps_boylam | Float | GPS Boylam | +Doğu -Batı |
| 53 | adr_ziyarethaftasi | Tinyint | Ziyaret Haftası | 0:İlk Hafta 1:İkinci Hafta 2:Üçüncü Hafta |
| 54 | adr_ziygunu2_1 | Bit | 2.Ziyaret Günü |  |
| 55 | adr_ziygunu2_2 | Bit | 2.Ziyaret Günü |  |
| 56 | adr_ziygunu2_3 | Bit | 2.Ziyaret Günü |  |
| 57 | adr_ziygunu2_4 | Bit | 2.Ziyaret Günü |  |
| 58 | adr_ziygunu2_5 | Bit | 2.Ziyaret Günü |  |
| 59 | adr_ziygunu2_6 | Bit | 2.Ziyaret Günü |  |
| 60 | adr_ziygunu2_7 | Bit | 2.Ziyaret Günü |  |
| 61 | adr_efatura_alias | Nvarchar(120) | e-Fatura Etiket Tanımları |  |
| 62 | adr_eirsaliye_alias | Nvarchar(120) | e-İrsaliye Etiket Tanımları |  |


Güncellenme Tarihi : 08.05.2025 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**