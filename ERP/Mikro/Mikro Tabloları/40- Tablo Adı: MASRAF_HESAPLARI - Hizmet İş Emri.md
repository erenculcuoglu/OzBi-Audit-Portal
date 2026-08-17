# TABLO NO: 62

## Tablo Adı: MASRAF_HESAPLARI - Hizmet İş Emri

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | his_Guid | Uniqueidentifier |  |  |
| 1 | his_DBCno | Smallint |  |  |
| 2 | his_Spec_Rec_no | Integer |  |  |
| 3 | his_iptal | Bit |  |  |
| 4 | his_fileid | Smallint |  |  |
| 5 | his_hidden | Bit |  |  |
| 6 | his_kilitli | Bit |  |  |
| 7 | his_degisti | Bit |  |  |
| 8 | his_checksum | Integer |  |  |
| 9 | his_create_user | Smallint |  |  |
| 10 | his_create_date | DateTime |  |  |
| 11 | his_lastup_user | Smallint |  |  |
| 12 | his_lastup_date | DateTime |  |  |
| 13 | his_special1 | Nvarchar(127) |  |  |
| 14 | his_special2 | Nvarchar(127) |  |  |
| 15 | his_special3 | Nvarchar(127) |  |  |
| 16 | his_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | his_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | his_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | his_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | his_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | his_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | his_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | his_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | his_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | his_kod | Nvarchar(25) | Hizmet İş Emri Kodu |  |
| 26 | his_isim | Nvarchar(40) | Hizmet İş Emri İsmi |  |
| 27 | his_yabanci_isim | Nvarchar(50) | Yabancı İsim |  |
| 28 | his_tipkod | Nvarchar(25) | Tip Kodu |  |
| 29 | his_sinifkod | Nvarchar(25) | Sınıf Kodu |  |
| 30 | his_grupkod | Nvarchar(25) | Grup Kodu |  |
| 31 | his_muhkod | Nvarchar(40) | Hesap Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 32 | his_mal_yan_muhkod | Nvarchar(40) | Maliyet Yansıtma Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 33 | his_kkeg_muhkod | Nvarchar(40) | KKEG Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 34 | his_dovcinsi | Tinyint | Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 35 | his_muh_sat_isk_kod | Nvarchar(40) | Satış İskonto Muhasebe Kodu |  |
| 36 | his_muh_aIiskmuhkod | Nvarchar(40) | Alış İskonto Muhasebe Kodu |  |
| 37 | his_ilavemasmuhkod | Nvarchar(40) | İlave Masraf Muhasebe Kodu |  |
| 38 | his_oivuygulama | Tinyint | Özet İletişim Vergisi Uygulaması | 0:Yok 1:Alışta Tutardan 2:Alışta Yüzdeyle 3:Satışta Tutardan 4:Satışta Yüzdeyle 5:Alışta Ve Satışta Tutardan 6:Alışta Ve Satışta Yüzdeyle |
| 39 | his_oivtutar | Float | Özet İletişim Vergisi Tutarı |  |
| 40 | his_oivturu | Tinyint | Özet İletişim Vergisi Türü | 0:Yok 1:ÖİV 2:5035 sayılı kanuna göre ÖİV |
| 41 | his_ufrs_fark_muhkod | Nvarchar(40) | Ufrs Fark Muhasebe Kodu |  |
| 42 | his_mal_yan_ufrs_fark_muhkod | Nvarchar(40) | Maliyet Yansıtma Ufrs Fark Muhasebe Kodu |  |
| 43 | his_kkeg_ufrs_fark_muhkod | Nvarchar(40) | KKEG Ufrs Fark Muhasebe Kodu |  |
| 44 | his_muh_sat_isk_ufrs_fark_kod | Nvarchar(40) | Satış İskonto Ufrs Fark Muhasebe Kodu |  |
| 45 | his_muh_aIiskufrs_fark_muhkod | Nvarchar(40) | Alış İskonto Ufrs Fark Muhasebe Kodu |  |
| 46 | his_his_ilavemasufrs_fark_muhkod | Nvarchar(40) | İlave Masraf Ufrs Fark Muhasebe Kodu |  |
| 47 | his_birim_ad | Nvarchar(10) | Birim Adı |  |
| 48 | his_bsbayadahil | Tinyint | Bs-Ba'ya Dahil Mi ? | 0:Evet 1:Hayır |
| 49 | his_vergifonid_1 | Smallint | Vergi Fon Id |  |
| 50 | his_vergifonid_2 | Smallint | Vergi Fon Id |  |
| 51 | his_vergifonid_3 | Smallint | Vergi Fon Id |  |
| 52 | his_vergifonid_4 | Smallint | Vergi Fon Id |  |
| 53 | his_vergifonid_5 | Smallint | Vergi Fon Id |  |
| 54 | his_KDV | Tinyint | Masraf Kdv |  |
| 55 | his_kkegtipi | Tinyint | KKEG Tipi | 0:Tanımsız 1:Oto Kira 2:Oto Bakım |
| 56 | his_nazim_hesap_isleyis | Tinyint | Nazım Hesap İşleyişi | 0:Tanımsız 1:KKEG'den 2:ÖİV'den 3:Orandan |
| 57 | his_nazim_oran | Float | Nazım Oran |  |
| 58 | his_nazim_muhkod_borc | Nvarchar(40) | Nazım Borç Muhasebe Kodu |  |
| 59 | his_nazim_muhkod_alacak | Nvarchar(40) | Nazım Alacak Muhasebe Kodu |  |


Güncellenme Tarihi : 18.12.2024 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**