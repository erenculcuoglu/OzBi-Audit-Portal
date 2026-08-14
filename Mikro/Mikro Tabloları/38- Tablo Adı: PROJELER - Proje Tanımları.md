# TABLO NO: 176

## Tablo Adı: PROJELER - Proje Tanımları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | pro_Guid | Uniqueidentifier |  |  |
| 1 | pro_DBCno | Smallint |  |  |
| 2 | pro_SpecRECno | Integer |  |  |
| 3 | pro_iptal | Bit |  |  |
| 4 | pro_fileid | Smallint |  |  |
| 5 | pro_hidden | Bit |  |  |
| 6 | pro_kilitli | Bit |  |  |
| 7 | pro_degisti | Bit |  |  |
| 8 | pro_checksum | Integer |  |  |
| 9 | pro_create_user | Smallint |  |  |
| 10 | pro_create_date | DateTime |  |  |
| 11 | pro_lastup_user | Smallint |  |  |
| 12 | pro_lastup_date | DateTime |  |  |
| 13 | pro_special1 | Nvarchar(127) |  |  |
| 14 | pro_special2 | Nvarchar(127) |  |  |
| 15 | pro_special3 | Nvarchar(127) |  |  |
| 16 | pro_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | pro_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | pro_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | pro_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | pro_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | pro_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | pro_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | pro_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | pro_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | pro_kodu | Nvarchar(25) | Proje Kodu |  |
| 26 | pro_adi | Nvarchar(40) | Proje Adı |  |
| 27 | pro_musterikodu | Nvarchar(25) | Müşteri Kodu | Bkz. Tablo CARI_HESAPLAR |
| 28 | pro_sormerkodu | Nvarchar(25) | Sorumluluk Merkezi Kodu |  |
| 29 | pro_bolgekodu | Nvarchar(25) | Bölge Kodu |  |
| 30 | pro_sektorkodu | Nvarchar(25) | Sektör Kodu |  |
| 31 | pro_grupkodu | Nvarchar(25) | Grup Kodu |  |
| 32 | pro_muh_kod_artikeli | Nvarchar(10) | Muhasebe Kod Artikeli |  |
| 33 | pro_durumu | Tinyint | Durumu | 0:Teklif Verildi 1:Kaybedildi 2:Tamamlandi 3:İptal Edildi |
| 34 | pro_aciklama | Nvarchar(50) | Açıklama |  |
| 35 | pro_ana_projekodu | Nvarchar(25) | Ana Proje Kodu |  |
| 36 | pro_planlanan_sure | Integer | Planlanan Süre |  |
| 37 | pro_planlanan_bastarih | DateTime | Planlanan Başlangıç Tarihi |  |
| 38 | pro_planlanan_bittarih | DateTime | Planlanan Bitiş Tarihi |  |
| 39 | pro_gerceklesen_bastarih | DateTime | Gerçekleşen Başlangıç Tarihi |  |
| 40 | pro_gerceklesen_bittarih | DateTime | Gerçekleşen Bitiş Tarihi |  |
| 41 | pro_baslangic_gecikmesebep | Nvarchar(50) | Başlangıç Gecikme Sebebi |  |
| 42 | pro_bitis_gecikmesebep | Nvarchar(50) | Bitiş Gecikme Sebebi |  |
| 43 | pro_performans_orani | Float | Performans Oranı |  |
| 44 | pro_teminat_sekli | Tinyint | Teminat Şekli | 0:Yüzde 1:Tutar |
| 45 | pro_teminat_doviz_cinsi | Tinyint | Teminat Döviz Cinsi |  |
| 46 | pro_teminat | Float | Teminat |  |
| 47 | pro_isavansi_sekli | Tinyint | İş Avansı Şekli | 0:Yüzde 1:Tutar |
| 48 | pro_isavansi_doviz_cinsi | Tinyint | İş Avansı Döviz Cinsi |  |
| 49 | pro_isavansi | Float | İş Avansı |  |


Güncellenme Tarihi : 28.11.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**