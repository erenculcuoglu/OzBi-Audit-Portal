# TABLO NO: 10

## Tablo Adı: STOK_DEPO_DETAYLARI - Stok Depo Detayları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | sdp_Guid | Uniqueidentifier |  |  |
| 1 | sdp_DBCno | Smallint |  |  |
| 2 | sdp_SpecRECno | Integer |  |  |
| 3 | sdp_iptal | Bit |  |  |
| 4 | sdp_fileid | Smallint |  |  |
| 5 | sdp_hidden | Bit |  |  |
| 6 | sdp_kilitli | Bit |  |  |
| 7 | sdp_degisti | Bit |  |  |
| 8 | sdp_checksum | Integer |  |  |
| 9 | sdp_create_user | Smallint |  |  |
| 10 | sdp_create_date | DateTime |  |  |
| 11 | sdp_lastup_user | Smallint |  |  |
| 12 | sdp_lastup_date | DateTime |  |  |
| 13 | sdp_special1 | Nvarchar(127) |  |  |
| 14 | sdp_special2 | Nvarchar(127) |  |  |
| 15 | sdp_special3 | Nvarchar(127) |  |  |
| 16 | sdp_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | sdp_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | sdp_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | sdp_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | sdp_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | sdp_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | sdp_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | sdp_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | sdp_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | sdp_depo_kod | Nvarchar(25) | Stok Depo Kodu |  |
| 26 | sdp_depo_no | Tinyint | Stok Depo No |  |
| 27 | sdp_kar_orani | Float | Stok Depo Kar Oranı |  |
| 28 | sdp_min_stok | Float | Stok Depo Minimum Seviyesi |  |
| 29 | sdp_sip_stok | Float | Stok Depo Sipariş Seviyesi |  |
| 30 | sdp_max_stok | Float | Stok Depo Maksimum Seviyesi |  |
| 31 | sdp_ver_sipbirimpntr | Tinyint | Verilen Sipariş Birimi | Bkz. Tablo STOKLAR |
| 32 | sdp_al_sipbirimpntr | Tinyint | Alınan Sipariş Birimi | Bkz. Tablo STOKLAR |
| 33 | sdp_sipsure | Smallint | Sipariş Süresi (Gün) |  |
| 34 | sdp_yerkodu | Nvarchar(10) | Depo Yer Kodu - Ambar Adresi |  |
| 35 | sdp_satisdursun | Tinyint | Satış Dursun Mu ? | 0:Durmasın 1:Dursun |
| 36 | sdp_sipdursun | Tinyint | Sipariş Dursun Mu ? | 0:Durmasın 1:Dursun |
| 37 | sdp_malkabuldursun | Tinyint | Mal Kabul Dursun Mu ? | 0:Durmasın 1:Dursun |
| 38 | sdp_MalKabulGun1 | Bit | Mal Kabul Günü | Pazartesi |
| 39 | sdp_MalKabulGun2 | Bit | Mal Kabul Günü | Salı |
| 40 | sdp_MalKabulGun3 | Bit | Mal Kabul Günü | Çarşamba |
| 41 | sdp_MalKabulGun4 | Bit | Mal Kabul Günü | Perşembe |
| 42 | sdp_MalKabulGun5 | Bit | Mal Kabul Günü | Cuma |
| 43 | sdp_MalKabulGun6 | Bit | Mal Kabul Günü | Cumartesi |
| 44 | sdp_MalKabulGun7 | Bit | Mal Kabul Günü | Pazar |
| 45 | sdp_siparisGun1 | Bit | Sipariş Günleri | Pazartesi |
| 46 | sdp_siparisGun2 | Bit | Sipariş Günleri | Salı |
| 47 | sdp_siparisGun3 | Bit | Sipariş Günleri | Çarşamba |
| 48 | sdp_siparisGun4 | Bit | Sipariş Günleri | Perşembe |
| 49 | sdp_siparisGun5 | Bit | Sipariş Günleri | Cuma |
| 50 | sdp_siparisGun6 | Bit | Sipariş Günleri | Cumartesi |
| 51 | sdp_siparisGun7 | Bit | Sipariş Günleri | Pazar |
| 52 | sdp_IskontoYapilamaz | Bit | İskonto Yapılamaz Mı ? | 0:Evet 1:Hayır |
| 53 | sdp_Tasfiyede_Fl | Bit | Tasfiyede Mi ? | 0:Evet 1:Hayır |
| 54 | sdp_Pasif_fl | Bit | Aktif/Pasif | 0:Pasif 1:Aktif |
| 55 | sdp_sat_cari_kod | Nvarchar(25) | Stok Depo Satıcı Cari Kodu |  |
| 56 | sdpKasaIskontoOrani | Float | Kasa İskonto Oranı |  |
| 57 | sdpKasaIskontoTutari | Float | Kasa İskonto Tutarı |  |
| 58 | sdp_eksiyedusebilir_fl | Bit | Eksiye Düşebilir Mi ? |  |
| 59 | sdp_UrunSorumlusuKodu | Nvarchar(25) | Ürün Sorumlusu Kodu |  |
| 60 | sdp_KasadaTaksitlenebilir_fl | Bit | Kasada Taksitlenebilir Mi ? |  |
| 61 | sdp_siparisyeri | Tinyint | Sipariş Yeri | 0:Genel 1:Cariden 2:Şubeden |
| 62 | sdp_muhkod_artikeli | Nvarchar(10) | Muhasebe Kodu Artikeli |  |
| 63 | sdp_pozisyonbayrak_kodu | Nvarchar(25) | Pozisyon Bayrak Kodu |  |
| 64 | sdp_min_stok_belirleme_gun | Smallint | Minimum Seviye Belirleme Operasyonu İçin Gün Bilgisi |  |
| 65 | sdp_sip_stok_belirleme_gun | Smallint | Sipariş Seviye Belirleme Operasyonu İçin Gün Bilgisi |  |
| 66 | sdp_max_stok_belirleme_gun | Smallint | Maksimum Seviye Belirleme Operasyonu İçin Gün Bilgisi |  |
| 67 | sdp_sev_bel_opr_degerlendime_fl | Bit | Seviye Belirleme Operasyonu Değerlendirmesi Yapılacak Mı? |  |


Güncellenme Tarihi : 27.11.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**