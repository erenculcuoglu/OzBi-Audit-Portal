# TABLO NO: 61

## Tablo Adı: HIZMET_HESAPLARI - Hizmet Kartları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | hiz_Guid | Uniqueidentifier |  |  |
| 1 | hiz_DBCno | Smallint |  |  |
| 2 | hiz_SpecRecno | Integer |  |  |
| 3 | hiz_iptal | Bit |  |  |
| 4 | hiz_fileid | Smallint |  |  |
| 5 | hiz_hidden | Bit |  |  |
| 6 | hiz_kilitli | Bit |  |  |
| 7 | hiz_degisti | Bit |  |  |
| 8 | hiz_checksum | Integer |  |  |
| 9 | hiz_create_user | Smallint |  |  |
| 10 | hiz_create_date | DateTime |  |  |
| 11 | hiz_lastup_user | Smallint |  |  |
| 12 | hiz_lastup_date | DateTime |  |  |
| 13 | hiz_special1 | Nvarchar(127) |  |  |
| 14 | hiz_special2 | Nvarchar(127) |  |  |
| 15 | hiz_special3 | Nvarchar(127) |  |  |
| 16 | hiz_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | hiz_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | hiz_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | hiz_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | hiz_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | hiz_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | hiz_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | hiz_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | hiz_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | hiz_tip | Tinyint | Hizmet Tipi | İlerde kullanmak üzere tanımlanmıştır. |
| 26 | hiz_kod | Nvarchar(25) | Hizmet Kodu |  |
| 27 | hiz_isim | Nvarchar(127) | Hizmet İsmi |  |
| 28 | hiz_yabanci_isim | Nvarchar(127) | Yabancı İsim |  |
| 29 | hiz_tipkod | Nvarchar(25) | Tip Kodu |  |
| 30 | hiz_sinifkod | Nvarchar(25) | Sınıf Kodu |  |
| 31 | hiz_grupkod | Nvarchar(25) | Grup Kodu |  |
| 32 | hiz_sat_muh_kod | Nvarchar(40) | Satış Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 33 | hiz_sat_iade_muh_kod | Nvarchar(40) | Satış İade Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 34 | hiz_mal_muh_kod | Nvarchar(40) | Maliyet Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 35 | hiz_sat_mal_muh_kod | Nvarchar(40) | Satılan Hizmet Maliyeti Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 36 | hiz_mal_yan_muh_kod | Nvarchar(40) | Maliyet Yansıtma Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 37 | hiz_fiyat | Float | Fiyatı |  |
| 38 | hiz_doviz_cinsi | Tinyint | Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 39 | hiz_isk_grup | Nvarchar(4) | İskonto Grubu | Bkz. STOK_CARI_ISKONTO_TANIMLARI |
| 40 | hiz_KDV | Tinyint | Kdv Oranı |  |
| 41 | hiz_muh_sat_isk_kod | Nvarchar(40) | Satış İskonto Muhasebe Kodu |  |
| 42 | hiz_muh_aIiskmuhkod | Nvarchar(40) | Alış İskonto Muhasebe Kodu |  |
| 43 | hiz_ilavemasmuhkod | Nvarchar(40) | İlave Masraf Muhasebe Kodu |  |
| 44 | hiz_operasyon_suresi | Integer | Operasyon Süresi |  |
| 45 | hiz_oivuygulama | Tinyint | Özet İletişim Vergisi Uygulaması | 0:Yok 1:Alışta Tutardan 2:Alışta Yüzdeyle 3:Satışta Tutardan 4:Satışta Yüzdeyle 5:Alışta Ve Satışta Tutardan 6:Alışta Ve Satışta Yüzdeyle |
| 46 | hiz_oivtutar | Float | Özet İletişim Vergisi Tutarı |  |
| 47 | hiz_oivturu | Tinyint | Özet İletişim Vergisi Türü | 0:Yok 1:ÖİV 2:5035 sayılı kanuna göre ÖİV |
| 48 | hiz_sat_ufrs_fark_muh_kod | Nvarchar(40) | Satış Ufrs Fark Muhasebe Kodu |  |
| 49 | hiz_sat_iade_ufrs_fark_muh_kod | Nvarchar(40) | Satış İade Ufrs Fark Muhasebe Kodu |  |
| 50 | hiz_mal_ufrs_fark_muh_kod | Nvarchar(40) | Maliyet Ufrs Fark Muhasebe Kodu |  |
| 51 | hiz_sat_mal_ufrs_fark_muh_kod | Nvarchar(40) | Satılan Hizmet Maliyeti Ufrs Fark Muhasebe Kodu |  |
| 52 | hiz_mal_yan_ufrs_fark_muh_kod | Nvarchar(40) | Maliyet Yansıtma Ufrs Fark Muhasebe Kodu |  |
| 53 | hiz_muh_sat_ufrs_fark_isk_kod | Nvarchar(40) | Satış İskonto Ufrs Fark Muhasebe Kodu |  |
| 54 | hiz_muh_aIiskufrs_fark_muhkod | Nvarchar(40) | Alış İskonto Ufrs Fark Muhasebe Kodu |  |
| 55 | hiz_ilavemasufrs_fark_muhkod | Nvarchar(40) | İlave Masraf Ufrs Fark Muhasebe Kodu |  |
| 56 | hiz_birim_ad | Nvarchar(10) | Birim Adı |  |
| 57 | hiz_bsbayadahil | Tinyint | Bs-Ba'ya Dahil Mi ? | 0:Evet 1:Hayır |
| 58 | hiz_vergifonid_1 | Smallint | Vergi Fon Id |  |
| 59 | hiz_vergifonid_2 | Smallint | Vergi Fon Id |  |
| 60 | hiz_vergifonid_3 | Smallint | Vergi Fon Id |  |
| 61 | hiz_vergifonid_4 | Smallint | Vergi Fon Id |  |
| 62 | hiz_vergifonid_5 | Smallint | Vergi Fon Id |  |
| 63 | hiz_efat_sinif_kodu | Nvarchar(20) | e-Fatura Sınıf Kodu |  |
| 64 | hiz_efat_sinif_listesi | Nvarchar(15) | e-Fatura Sınıf Listesi |  |
| 65 | hiz_efat_sinif_versiyonu | Nvarchar(15) | e-Fatura Sınıf Versiyonu |  |
| 66 | hiz_Tevkifat_turu | Tinyint | Tevkifat Türü | 0:Yok 1:10da3 2:10da9 3:10da4 4:32 5:61 6:45 7:Tam 8:10da2 9:10da5 10:10da7 |


Güncellenme Tarihi : 01.12.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**