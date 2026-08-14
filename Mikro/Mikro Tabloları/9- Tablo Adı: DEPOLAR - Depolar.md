# TABLO NO: 111

## Tablo Adı: DEPOLAR - Depolar

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | dep_Guid | Uniqueidentifier |  |  |
| 1 | dep_DBCno | Smallint |  |  |
| 2 | dep_SpecRECno | Integer |  |  |
| 3 | dep_iptal | Bit |  |  |
| 4 | dep_fileid | Smallint |  |  |
| 5 | dep_hidden | Bit |  |  |
| 6 | dep_kilitli | Bit |  |  |
| 7 | dep_degisti | Bit |  |  |
| 8 | dep_checksum | Integer |  |  |
| 9 | dep_create_user | Smallint |  |  |
| 10 | dep_create_date | DateTime |  |  |
| 11 | dep_lastup_user | Smallint |  |  |
| 12 | dep_lastup_date | DateTime |  |  |
| 13 | dep_special1 | Nvarchar(127) |  |  |
| 14 | dep_special2 | Nvarchar(127) |  |  |
| 15 | dep_special3 | Nvarchar(127) |  |  |
| 16 | dep_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | dep_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | dep_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | dep_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | dep_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | dep_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | dep_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | dep_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | dep_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | dep_firmano | Integer | Firma No |  |
| 26 | dep_subeno | Integer | Şube No |  |
| 27 | dep_no | Integer | Depo No |  |
| 28 | dep_adi | Nvarchar(50) | Depo Adı |  |
| 29 | dep_grup_kodu | Nvarchar(25) | Depo Grup Kodu |  |
| 30 | dep_tipi | Tinyint | Depo Tipi | 0:Merkez Depo 1:Şube Depo 2:Mağaza Depo 3:Market Depo 4:Satıcı Depo 5:Gümrük Depo 6:Mal Kabul Depo 7:Ham Madde Depo 8:Yarı Mamül Depo 9:Üretim Koltuk Depo 10:Fason Depo 11:Mamül Depo 12:Sevk Depo 13:Kalite Kontrol Depo 14:Konsinye Depo 15:Nakliye Depo 16:Kiralama Depo 17:Araç Depo 18:e-Ticaret Depo |
| 31 | dep_DepoSevkOtoFiyat | Tinyint | Depo Oto Sevkiyat Fiyat Tipi | 0:Maliyet değeri 1:Satış fiyatı 2:Satın alma şartları |
| 32 | dep_hareket_tipi | Tinyint | Depo Hareket Tipi | 0:Hareket girilir 1:Hareket girilemez 2:Sadece giriş yapılır 3:Sadece çıkış yapılır |
| 33 | dep_muh_kodu | Nvarchar(10) | Depo Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 34 | dep_sor_mer_kodu | Nvarchar(25) | Sorumluluk Merkezi Kodu | Bkz. SORUMLULUK_MERKEZLERI |
| 35 | dep_proje_kodu | Nvarchar(25) | Proje Kodu |  |
| 36 | dep_DepoSevkUygFiyat | Integer | Depo Sevk Uygun Fiyat Kodu | Bkz. STOKLAR |
| 37 | dep_KilitTarihi | DateTime | Kilit Tarihi |  |
| 38 | dep_cadde | Nvarchar(50) | Depo Cadde |  |
| 39 | dep_mahalle | Nvarchar(50) | Depo Mahalle |  |
| 40 | dep_sokak | Nvarchar(50) | Depo Sokak |  |
| 41 | dep_Semt | Nvarchar(25) | Depo Semt |  |
| 42 | dep_Apt_No | Nvarchar(10) | Depo Apartman Numarası |  |
| 43 | dep_Daire_No | Nvarchar(10) | Depo Daire Numarası |  |
| 44 | dep_posta_Kodu | Nvarchar(8) | Depo Posta Kodu |  |
| 45 | dep_Ilce | Nvarchar(50) | Depo İlçe |  |
| 46 | dep_Il | Nvarchar(50) | Depo İl |  |
| 47 | dep_Ulke | Nvarchar(50) | Depo Ülke |  |
| 48 | dep_Adres_kodu | Nvarchar(10) | Adres Kodu |  |
| 49 | dep_gps_enlem | Float | GPS Enlem |  |
| 50 | dep_gps_boylam | Float | GPS Boylam |  |
| 51 | dep_alani | Float | Depo Alanı |  |
| 52 | dep_rafhacmi | Float | Depo Raf Hacmi |  |
| 53 | dep_yetkili_email | Nvarchar(50) | Depo Yetkili e-Posta Adresi |  |
| 54 | dep_satis_alani | Float | Depo Satış Alanı |  |
| 55 | dep_sergi_alani | Float | Depo Sergi Alanı |  |
| 56 | dep_otopark_alani | Float | Depo Otopark Alanı |  |
| 57 | dep_otopark_kapasite | Integer | Depo Otopark Kapasitesi |  |
| 58 | dep_kasa_sayisi | Integer | Depo Kasa Sayısı |  |
| 59 | dep_kamyon_kasa_hacmi | Float | Depo Kamyon Kasa Hacmi |  |
| 60 | dep_kamyon_istiab_haddi | Float | Depo Kamyon İstiab Haddi |  |
| 61 | dep_dizin_adi | Nvarchar(50) | Depo Dizin Adı |  |
| 62 | dep_tel_ulke_kodu | Nvarchar(5) | Depo Telefon Ülke Kodu |  |
| 63 | dep_tel_bolge_kodu | Nvarchar(5) | Depo Telefon Bölge Kodu |  |
| 64 | dep_tel_no1 | Nvarchar(10) | Depo Telefon Numarası1 |  |
| 65 | dep_tel_no2 | Nvarchar(10) | Depo Telefon Numarası2 |  |
| 66 | dep_tel_faxno | Nvarchar(10) | Depo Telefon Fax Numarası |  |
| 67 | dep_tel_modem | Nvarchar(10) | Depo Telefon Modemi |  |
| 68 | dep_envanter_harici_fl | Bit | Depo Envanter Deposu Mu ? |  |
| 69 | dep_detay_takibi | Tinyint | Detay Takibi Var Mı ? | 0:Var 1:Yok |
| 70 | dep_barkod_yazici_yolu | Nvarchar(50) | Barkod Yazıcı Yolu |  |
| 71 | dep_fason_sor_mer_kodu | Nvarchar(25) | Fason Sorumluluk Merkezi Kodu |  |
| 72 | dep_EksiyeDusurenStkHar | Tinyint | Eksiye Düşüren Stok Hareketinde Depo Uyarı Tipi | 0:Genel Ayarlar 1:Devam Et 2:Uyar Devam Et 3:Uyar Devam Etme |
| 73 | dep_BagliOrtakliklaraSatisUygFiyat | Tinyint | Bağlı Ortaklıklara Satışlarda Uygulanacak Fiyat | 0:Çıkış Depo Satış Fiyatı 1:Giriş Depo Satış Fiyatı 2:Çıkış Depo Satın Alma Şartı 3:Giriş Depo Satın Alma Şartı |
| 74 | dep_bolge_kodu | Nvarchar(25) | Bölge Kodu |  |
| 75 | dep_NakliyefisiSatisFiyatTipi | Tinyint | Nakliye Fişi Satış Fiyat Tipi | 0:Çıkış Depo Satış Fiyatı 1:Giriş Depo Satış Fiyatı 2:Çıkış Depo Satın Alma Şartı 3:Giriş Depo Satın Alma Şartı |
| 76 | dep_gidiste_eirsaliye_fl | Bit | Gidişte e-İrsaliye |  |
| 77 | dep_geliste_eirsaliye_fl | Bit | Gelişte e-İrsaliye |  |
| 78 | dep_fytdegfis_kullanilmaz_fl | Bit | Fiyat Değişiklik Fişinde Kullanılamaz |  |
| 79 | dep_seribag_detay_takibi | Tinyint | Stok Detaylı Takip | 0:Var 1:Yok |
| 80 | dep_dikeycozum_raftakibi_zorunlu_fl | Bit | Dikey Çözüm Raf Takibi Zorunlu Mu ? |  |


Güncellenme Tarihi : 05.12.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**