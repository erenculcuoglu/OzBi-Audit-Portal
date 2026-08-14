# TABLO NO: 3

## Tablo Adı: SORUMLULUK_MERKEZLERI - Sorumluluk Merkezleri

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | som_Guid | Uniqueidentifier |  |  |
| 1 | som_DBCno | Smallint |  |  |
| 2 | som_SpecRECno | Integer |  |  |
| 3 | som_iptal | Bit |  |  |
| 4 | som_fileid | Smallint |  |  |
| 5 | som_hidden | Bit |  |  |
| 6 | som_kilitli | Bit |  |  |
| 7 | som_degisti | Bit |  |  |
| 8 | som_checksum | Integer |  |  |
| 9 | som_create_user | Smallint |  |  |
| 10 | som_create_date | DateTime |  |  |
| 11 | som_lastup_user | Smallint |  |  |
| 12 | som_lastup_date | DateTime |  |  |
| 13 | som_special1 | Nvarchar(127) |  |  |
| 14 | som_special2 | Nvarchar(127) |  |  |
| 15 | som_special3 | Nvarchar(127) |  |  |
| 16 | som_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | som_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | som_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | som_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | som_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | som_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | som_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | som_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | som_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | som_kod | Nvarchar(25) | Sorumluluk Merkezi Kodu |  |
| 26 | som_isim | Nvarchar(40) | Sorumluluk Merkezi Adı |  |
| 27 | som_DogrudanUrtSrmMrk | Bit | Doğrudan Üretim Sorumluluk Merkezi |  |
| 28 | som_MasrafNereyeYuklenecek | Tinyint | Masraf Nereye Yüklenecek ? | 0:İş Merkezine 1:İş Emrine 2:Ürüne 3:Operasyona 4:Kalıba |
| 29 | som_DagAnahKodu | Nvarchar(25) | Dağıtım Anahtarı Kodu |  |
| 30 | som_MuhArtikeli | Nvarchar(10) | Muhasebe Artikeli |  |
| 31 | som_MaliyetDagitimSekli | Tinyint | Maliyet Dağıtım Şekli | 0:Süreye göre 1:Miktara göre 2:Ağırlığa göre 3:Alana göre 4:Hacme göre 5:Adam saate göre 6:Miktar 2'ye göre 7:Miktar3'e göre 8:Miktar4'e göre 9:Enerji1'e göre 10:Enerji2'ye göre 11:Miktar bölü safha sayısına göre 12:Miktar bölü safha sayısı çarpı standart maliyete göre |
| 32 | som_MaliyetDagitimKaynak | Tinyint | Maliyet Dağıtım Kaynağı | 0:Hesaptan 1:Sorumluluk Merkezi Özel |
| 33 | som_tipi | Tinyint | Sorumluluk Merkezi Tipi | 0:Genel Amaçlı Masraf Merkezi 1:Genel Amaçlı Kar Merkezi 2:Doğrudan Üretim Yeri Masraf Merkezi 3:Dolaylı Üretim Yeri Masraf Merkezi 4:Satış Kar Merkezi 5:Kampanya Satış Kar Merkezi 6:Yatırım Merkezi 7:Ödenmeyen Değerli Kağıtlar Merkezi |
| 34 | som_satis_fiyat_liste_no | Integer | Satış Fiyatı Liste No |  |


Güncellenme Tarihi : 27.11.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**