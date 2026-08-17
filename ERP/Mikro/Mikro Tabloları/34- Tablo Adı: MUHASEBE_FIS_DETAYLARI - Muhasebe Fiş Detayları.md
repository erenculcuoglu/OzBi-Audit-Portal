# TABLO NO: 243

## Tablo Adı: MUHASEBE_FIS_DETAYLARI - Muhasebe Fiş Detayları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | mfd_Guid | Uniqueidentifier |  |  |
| 1 | mfd_DBCno | Smallint |  |  |
| 2 | mfd_SpecRECno | Integer |  |  |
| 3 | mfd_iptal | Bit |  |  |
| 4 | mfd_fileid | Smallint |  |  |
| 5 | mfd_hidden | Bit |  |  |
| 6 | mfd_kilitli | Bit |  |  |
| 7 | mfd_degisti | Bit |  |  |
| 8 | mfd_checksum | Integer |  |  |
| 9 | mfd_create_user | Smallint |  |  |
| 10 | mfd_create_date | DateTime |  |  |
| 11 | mfd_lastup_user | Smallint |  |  |
| 12 | mfd_lastup_date | DateTime |  |  |
| 13 | mfd_special1 | Nvarchar(127) |  |  |
| 14 | mfd_special2 | Nvarchar(127) |  |  |
| 15 | mfd_special3 | Nvarchar(127) |  |  |
| 16 | mfd_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | mfd_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | mfd_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | mfd_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | mfd_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | mfd_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | mfd_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | mfd_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | mfd_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | mfd_ticari_tip | Tinyint | Ticari Tip |  |
| 26 | mfd_evraktip | Tinyint | Evrak Tipi | 0:Tanımsız 1:Satış Belgesi 2:Alış Belgesi 3:Tahsilat Belgesi 4:Ödeme Belgesi 5:Virman 6:Dahili Hareket 7:Personel Bordro 8:Amortisman 9:SMM Hareket |
| 27 | mfd_evrak_seri | dbo.evrakseri_str | Evrak Seri No |  |
| 28 | mfd_evrak_sira | Integer | Evrak Sıra No |  |
| 29 | mfd_cariunvan | Nvarchar(127) | Cari Ünvanı |  |
| 30 | mfd_carivergidaireadi | Nvarchar(50) | Cari Vergi Daire Adı |  |
| 31 | mfd_carivergidaireno | Nvarchar(15) | Cari Vergi Daire No |  |
| 32 | mfd_bsbakonututar | Float | Bs-Ba Konu Tutarı |  |
| 33 | mfd_bsbatabii | Tinyint | Bs-Ba Tabi Mi ? |  |
| 34 | mfd_cariulkekodno | Nvarchar(4) | Cari Ülke Kod No |  |
| 35 | mfd_belgetarihi | DateTime | Belge Tarihi |  |
| 36 | mfd_tutarnereden | Tinyint | Ba-Bs'ye Tabi Tutar Nasıl Oluştu ? | 0:Tanımsız 1:Manuel 2:Oto Kdv'den 3:Sihirbazdan 4:Oto Hesaptan 5:Ticari Entegreden 6:Smm Entegreden |
| 37 | mfd_caritipi | Tinyint | Cari Tipi | 0:Tanımsız 1:Muhasebe Hesabı 2:Cari Hesap 3:Firma |
| 38 | mfd_carikodu | Nvarchar(25) | Cari Kodu |  |
| 39 | mfd_carimuhkodu | Nvarchar(25) | Cari Muhasebe Kodu |  |
| 40 | mfd_belgeno | dbo.belgeno_str | Belge No |  |
| 41 | mfd_kdvid | Tinyint | Kdv Id |  |
| 42 | mfd_kdvtutar | Float | Kdv Tutar |  |
| 43 | mfd_malhizmetkodu | Nvarchar(25) | Mal Hizmet Kodu |  |
| 44 | mfd_malhizmetcinsi | Nvarchar(120) | Mal Hizmet Cinsi |  |
| 45 | mfd_malhizmetmiktari | Float | Mal Hizmet Miktarı |  |
| 46 | mfd_malhizmetbirim | Nvarchar(10) | Mal Hizmet Birimi |  |
| 47 | mfd_ggb_gcb_no | Nvarchar(30) | GGB-GÇB No |  |
| 48 | mfd_aracivergidaireadi | Nvarchar(50) | Aracı Vergi Dairesi Adı |  |
| 49 | mfd_aracivergidaireno | Nvarchar(15) | Aracı Vergi Daire No |  |
| 50 | mfd_eximulkekodu | Nvarchar(4) | Exim Ülke Kodu |  |
| 51 | mfd_bsbakonuufrstutar | Float | Ba-Bs'ye Tabi Ufrs Tutarı |  |
| 52 | mfd_aciklama | Nvarchar(127) | Açıklama |  |
| 53 | mfd_caritutar | Float | Cari Tutar |  |
| 54 | mfd_kisaevraktipi | Tinyint | Kısa Evrak Tipi | 0:Tanımsız 1:Satış Belgesi 2:Alış Belgesi 3:Tahsilat Belgesi 4:Ödeme Belgesi 5:Virman 6:Dahili Hareket 7:Personel Bordro 8:Amortisman 9:SMM Hareket |
| 55 | mfd_satistipi | Tinyint | Satış Tipi | 0:Fatura 1:SMM Makbuzu 2:Z Raporu 3:Perakende Fişi 4:Bilet 5:Poliçe 6:Fon 7:Diğer 8:Navlun |
| 56 | mfd_alistipi | Tinyint | Alış Tipi | 0:Fatura 1:SMM Makbuzu 2:Gider Makbuzu 3:Gider Pusulası 4:Z Raporu 5:Perakende Fişi 6:Bilet 7:Poliçe 8:Fon 9:Müstahsil Makbuzu 10:Diğer 11:Navlun |
| 57 | mfd_tahtedtipi | Tinyint | Tahsil Tediye Tipi | 0:Nakit 1:Çek 2:Senet 3:Banka 4:Kredi Kartı 5:Diğer 6:Teminat Mektubu 7:Depozito Çeki 8:Depozito Senedi |
| 58 | mfd_digerevrakadi | Nvarchar(50) | Diğer Evrak Adı |  |
| 59 | mfd_evraktur | Tinyint | Evrak Türü |  |
| 60 | mfd_e_belgemi | Bit | e-Belge Mi ? |  |
| 61 | mfd_e_belgemi_nereden | Tinyint | e-Belge Mi Alanı Nereden Geldi ? | 0:Otomatik 1:Manuel |
| 62 | mfd_evrak_hubid | Nvarchar(50) | Evrak Hub Id |  |
| 63 | mfd_evrak_hubglbid | Nvarchar(50) | Evrak Hub Global Id |  |
| 64 | mfd_evrak_baglantisi | Tinyint | Evrak Bağlantısı | 0:Yok 1:Var |


Güncellenme Tarihi : 06.12.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**