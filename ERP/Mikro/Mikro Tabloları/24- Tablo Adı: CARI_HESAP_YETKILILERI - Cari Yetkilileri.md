# TABLO NO: 33

## Tablo Adı: CARI_HESAP_YETKILILERI - Cari Yetkilileri

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | mye_Guid | Uniqueidentifier |  |  |
| 1 | mye_DBCno | Smallint |  |  |
| 2 | mye_SpecRECno | Integer |  |  |
| 3 | mye_iptal | Bit |  |  |
| 4 | mye_fileid | Smallint |  |  |
| 5 | mye_hidden | Bit |  |  |
| 6 | mye_kilitli | Bit |  |  |
| 7 | mye_degisti | Bit |  |  |
| 8 | mye_checksum | Integer |  |  |
| 9 | mye_create_user | Smallint |  |  |
| 10 | mye_create_date | DateTime |  |  |
| 11 | mye_lastup_user | Smallint |  |  |
| 12 | mye_lastup_date | DateTime |  |  |
| 13 | mye_special1 | Nvarchar(127) |  |  |
| 14 | mye_special2 | Nvarchar(127) |  |  |
| 15 | mye_special3 | Nvarchar(127) |  |  |
| 16 | mye_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | mye_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | mye_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | mye_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | mye_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | mye_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | mye_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | mye_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | mye_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | mye_cari_kod | Nvarchar(25) | Cari Kodu | Bkz. CARI_HESAPLAR |
| 26 | mye_adres_no | Integer | Adres No | Bkz. CARI_HESAP_ADRESLERI |
| 27 | mye_isim | Nvarchar(30) | Yetkili İsmi |  |
| 28 | mye_soyisim | Nvarchar(30) | Yetkili Soyismi |  |
| 29 | mye_dogum_tarihi | DateTime | Doğum Tarihi |  |
| 30 | mye_evlilik_tarih | DateTime | Evlilik Tarihi |  |
| 31 | mye_es_isim | Nvarchar(30) | Eş İsmi |  |
| 32 | mye_es_dogum_tarih | DateTime | Eş Doğum Tarihi |  |
| 33 | mye_unvan | Tinyint | Ünvan |  |
| 34 | mye_hitap | Tinyint | Hitap |  |
| 35 | mye_hisse | Tinyint | Hisse | 0:Yok 1:Ortak 2:Tüm |
| 36 | mye_tahsil | Tinyint | Tahsil | 0:Tahsili Yok 1:İlk 2:Orta 3:Lise 4:Yüksek 5:Fakülte 6:Yüksek Lisans 7:Doktora 8:Fakülte Temel Bilgi 9:Yüksek Lisans Temel Bilgi 10:Okul Öncesi |
| 37 | mye_dahili_telno | Nvarchar(5) | Dahili Tel |  |
| 38 | mye_email_adres | Nvarchar(127) | e-Posta Adresi |  |
| 39 | mye_cep_telno | Nvarchar(17) | Cep Telefonu |  |
| 40 | mye_tc_kimlikno | Nvarchar(20) | TC Kimlik Numarası |  |
| 41 | mye_vergi_dairesi | Nvarchar(20) | Vergi Dairesi |  |
| 42 | mye_vergi_kimlikno | Nvarchar(20) | Vergi Kimlik numarası |  |
| 43 | mye_dogum_yeri | Nvarchar(30) | Doğum Yeri |  |
| 44 | mye_ev_cadde | Nvarchar(127) | Cadde |  |
| 45 | mye_ev_mahalle | Nvarchar(127) | Mahalle |  |
| 46 | mye_ev_sokak | Nvarchar(127) | Sokak |  |
| 47 | mye_ev_Semt | Nvarchar(25) | Semt |  |
| 48 | mye_ev_Apt_No | Nvarchar(10) | Apartman No |  |
| 49 | mye_ev_Daire_No | Nvarchar(10) | Daire No |  |
| 50 | mye_ev_posta_kodu | Nvarchar(8) | Posta Kodu |  |
| 51 | mye_ev_ilce | Nvarchar(50) | İlçe |  |
| 52 | mye_ev_il | Nvarchar(50) | İl |  |
| 53 | mye_ev_ulke | Nvarchar(50) | Ülke |  |
| 54 | mye_ev_adres_kodu | Nvarchar(10) | Adres Kodu |  |
| 55 | mye_is_telno | Nvarchar(17) | İş Telefon Numarası |  |
| 56 | mye_ev_telno | Nvarchar(17) | Ev Telefon Numarası |  |
| 57 | mye_KEP_adresi | Nvarchar(80) | Kayıtlı Elektronik Posta (KEP) Adresi |  |
| 58 | mye_mutabakat_yetkilisi_fl | Bit | Mutabakat Yetkilisi Mi ? |  |
| 59 | mye_sosyal_linkedin | Nvarchar(50) | Linkedin Hesabı |  |
| 60 | mye_sosyal_webadresi | Nvarchar(50) | Web Adresi |  |
| 61 | mye_sosyal_youtube | Nvarchar(50) | Youtube Hesabı |  |
| 62 | mye_sosyal_twitter | Nvarchar(50) | Twitter Hesabı |  |
| 63 | mye_sosyal_facebook | Nvarchar(50) | Facebook Hesabı |  |
| 64 | mye_sosyal_google | Nvarchar(50) | Google Hesabı |  |
| 65 | mye_sosyal_pinterest | Nvarchar(50) | Pinterest Hesabı |  |
| 66 | mye_sosyal_instagram | Nvarchar(50) | Instagram Hesabı |  |
| 67 | mye_sosyal_snapchat | Nvarchar(50) | Snapchat Hesabı |  |
| 68 | mye_sosyal_pasaportno | Nvarchar(20) | Pasaport Numarası |  |
| 69 | mye_arac_plaka | Nvarchar(15) | Araç Plaka |  |


Güncellenme Tarihi : 08.05.2025 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**