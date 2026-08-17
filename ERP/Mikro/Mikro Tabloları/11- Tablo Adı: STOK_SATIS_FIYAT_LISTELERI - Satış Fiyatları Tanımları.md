# TABLO NO: 228

## Tablo Adı: STOK_SATIS_FIYAT_LISTELERI - Satış Fiyatları Tanımları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | sfiyat_Guid | Uniqueidentifier |  |  |
| 1 | sfiyat_DBCno | Smallint |  |  |
| 2 | sfiyat_SpecRECno | Integer |  |  |
| 3 | sfiyat_iptal | Bit |  |  |
| 4 | sfiyat_fileid | Smallint |  |  |
| 5 | sfiyat_hidden | Bit |  |  |
| 6 | sfiyat_kilitli | Bit |  |  |
| 7 | sfiyat_degisti | Bit |  |  |
| 8 | sfiyat_checksum | Integer |  |  |
| 9 | sfiyat_create_user | Smallint |  |  |
| 10 | sfiyat_create_date | DateTime |  |  |
| 11 | sfiyat_lastup_user | Smallint |  |  |
| 12 | sfiyat_lastup_date | DateTime |  |  |
| 13 | sfiyat_special1 | Nvarchar(127) |  |  |
| 14 | sfiyat_special2 | Nvarchar(127) |  |  |
| 15 | sfiyat_special3 | Nvarchar(127) |  |  |
| 16 | sfiyat_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | sfiyat_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | sfiyat_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | sfiyat_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | sfiyat_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | sfiyat_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | sfiyat_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | sfiyat_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | sfiyat_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | sfiyat_stokkod | Nvarchar(25) | Stok Kodu |  |
| 26 | sfiyat_listesirano | Integer | Liste Sıra No |  |
| 27 | sfiyat_deposirano | Integer | Depo Sıra No |  |
| 28 | sfiyat_odemeplan | Integer | Ödeme Planı |  |
| 29 | sfiyat_birim_pntr | Tinyint | Birim |  |
| 30 | sfiyat_fiyati | Float | Fiyatı |  |
| 31 | sfiyat_doviz | Tinyint | Döviz Cinsi |  |
| 32 | sfiyat_iskontokod | Nvarchar(4) | Iskonto Kodu |  |
| 33 | sfiyat_deg_nedeni | Tinyint | Satış Fiyatı Değişim Nedeni | 0:Serbest 1:Satın Alma Şartı 2:Kar Oranı 3:Promosyon Başlangıcı 4:Promosyon Bitişi |
| 34 | sfiyat_primyuzdesi | Float | Prim Yüzdesi |  |
| 35 | sfiyat_kampanyakod | Kampanya Kodu | Nvarchar(4) |  |
| 36 | sfiyat_doviz_kuru | Döviz Kuru | Float |  |


Güncellenme Tarihi : 27.11.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**