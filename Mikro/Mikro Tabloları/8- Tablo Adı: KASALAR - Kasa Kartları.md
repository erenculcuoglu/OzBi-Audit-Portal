# TABLO NO: 53

## Tablo Adı: KASALAR - Kasa Kartları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | kas_Guid | Uniqueidentifier |  |  |
| 1 | kas_DBCno | Smallint |  |  |
| 2 | kas_SpecRecno | Integer |  |  |
| 3 | kas_iptal | Bit |  |  |
| 4 | kas_fileid | Smallint |  |  |
| 5 | kas_hidden | Bit |  |  |
| 6 | kas_kilitli | Bit |  |  |
| 7 | kas_degisti | Bit |  |  |
| 8 | kas_checksum | Integer |  |  |
| 9 | kas_create_user | Smallint |  |  |
| 10 | kas_create_date | DateTime |  |  |
| 11 | kas_lastup_user | Smallint |  |  |
| 12 | kas_lastup_date | DateTime |  |  |
| 13 | kas_special1 | Nvarchar(127) |  |  |
| 14 | kas_special2 | Nvarchar(127) |  |  |
| 15 | kas_special3 | Nvarchar(127) |  |  |
| 16 | kas_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | kas_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | kas_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | kas_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | kas_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | kas_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | kas_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | kas_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | kas_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | kas_tip | Tinyint | Kasa Tipi | 0:Nakit Kasası 1:Çek Kasası 2:Karşılıksız Çek Kasası   3:Senet Kasası 4:Protestolu Senet Kasası 5:Verilen Senet Kasası 6:Verilen Ödeme Emirleri Kasası   7:Müşteri Ödeme Sözleri Kasası |
| 26 | kas_firma_no | Integer | Firma No |  |
| 27 | kas_kod | Nvarchar(25) | Kasa Kodu |  |
| 28 | kas_isim | Nvarchar(40) | Kasa İsmi |  |
| 29 | kas_muh_kod | Nvarchar(40) | Kasa Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 30 | kas_doviz_cinsi | Tinyint | Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 31 | kas_bankakodu | Nvarchar(25) | Banka Kodu | Bkz. BANKALAR |
| 32 | kas_nakakincelenmesi | Bit | Nakit Akışta İncelenmesin? |  |
| 33 | kas_ufrs_muh_kod | Nvarchar(40) | Ufrs Muhasebe Kodu |  |


Güncellenme Tarihi : 05.12.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**