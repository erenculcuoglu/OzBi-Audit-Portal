# TABLO NO: 15

## Tablo Adı: BARKOD_TANIMLARI - Barkod Kartları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | bar_Guid | Uniqueidentifier |  |  |
| 1 | bar_DBCno | Smallint |  |  |
| 2 | bar_SpecRECno | Integer |  |  |
| 3 | bar_iptal | Bit |  |  |
| 4 | bar_fileid | Smallint |  |  |
| 5 | bar_hidden | Bit |  |  |
| 6 | bar_kilitli | Bit |  |  |
| 7 | bar_degisti | Bit |  |  |
| 8 | bar_checksum | Integer |  |  |
| 9 | bar_create_user | Smallint |  |  |
| 10 | bar_create_date | DateTime |  |  |
| 11 | bar_lastup_user | Smallint |  |  |
| 12 | bar_lastup_date | DateTime |  |  |
| 13 | bar_special1 | Nvarchar(127) |  |  |
| 14 | bar_special2 | Nvarchar(127) |  |  |
| 15 | bar_special3 | Nvarchar(127) |  |  |
| 16 | bar_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | bar_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | bar_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | bar_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | bar_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | bar_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | bar_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | bar_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | bar_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | bar_kodu | dbo.barkod_str | Barkod Kodu |  |
| 26 | bar_stokkodu | Nvarchar(25) | Barkod Stok Kodu | Bkz. Tablo STOKLAR |
| 27 | bar_partikodu | Nvarchar(25) | Parti Kodu |  |
| 28 | bar_lotno | Integer | Lot Numarası |  |
| 29 | bar_serino_veya_bagkodu | dbo.cihazseri_str | Seri Numarası veya Bağ Kodu |  |
| 30 | bar_barkodtipi | Tinyint | Barkod Tipi | 0:Ean13 1:Ean8 2:Ascii   3:Upca 4:Upce 5:Code39 |
| 31 | bar_icerigi | Tinyint | Barkod İçerik Bilgisi Türü | 0:Kod 1:Kod+miktar  2:Kod+birim fiyat 3:Kod+tutar |
| 32/td> |  |  |  |  |
| bar_birimpntr |  |  |  |  |
| Tinyint |  |  |  |  |
| Barkodun ilgili stoğun hangi birimine ait olduğunu |  |  |  |  |
| gösterir. |  |  |  |  |
| Bkz. Tablo STOKLAR | bar_birimpntr | Tinyint | Barkodun ilgili stoğun hangi birimine ait olduğunu   gösterir. | Bkz. Tablo STOKLAR |
| 33 | bar_master | Bit | Ana Barkod Mu ? | 0:Evet 1:Hayır |
| 34 | bar_bedenpntr | Tinyint | Hangi stok bedenine ait olduğunu gösterir. | Bkz. Tablo STOK_BEDEN_TANIMLARI |
| 35 | bar_renkpntr | Tinyint | Hangi stok rengine ait olduğunu gösterir. | Bkz. Tablo STOK_RENK_TANIMLARI |
| 36 | bar_baglantitipi | Tinyint | Bağlantı Tipi | 0:Stok Barkodu 1:Paket Barkodu 2:Asorti Barkodu 3:Stok Detay Barkodu 4:Hediye Çeki Barkodu 5:Hediye Kartı Barkodu 6:Sabit Kıymet Barkodu |
| 37 | bar_har_uid | Uniqueidentifier |  |  |
| 38 | bar_asortitanimkodu | Nvarchar(25) | Asorti Tanım Kodu |  |
| 39 | bar_VarBaglantiUId1 | Uniqueidentifier | Ana Renk Guidi |  |
| 40 | bar_VarBaglantiUId2 | Uniqueidentifier | Ana Beden Guidi |  |
| 41 | bar_VarBaglantiUId3 | Uniqueidentifier | Ana Beden Guidi |  |
| 42 | bar_VarBaglantiUId4 | Uniqueidentifier | Ana Beden Guidi |  |
| 43 | bar_VarBaglantiUId5 | Uniqueidentifier | Ana Beden Guidi |  |


Güncellenme Tarihi : 25.10.2024 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**