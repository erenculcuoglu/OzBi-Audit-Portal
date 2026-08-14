# TABLO NO: 291

## Tablo Adı: ALINAN_TEKLIFLER - Alınan Teklifler

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | altkl_Guid | Uniqueidentifier |  |  |
| 1 | altkl_DBCno | Smallint |  |  |
| 2 | altkl_SpecRECno | Integer |  |  |
| 3 | altkl_iptal | Bit |  |  |
| 4 | altkl_fileid | Smallint |  |  |
| 5 | altkl_hidden | Bit |  |  |
| 6 | altkl_kilitli | Bit |  |  |
| 7 | altkl_degisti | Bit |  |  |
| 8 | altkl_checksum | Integer |  |  |
| 9 | altkl_create_user | Smallint |  |  |
| 10 | altkl_create_date | DateTime |  |  |
| 11 | altkl_lastup_user | Smallint |  |  |
| 12 | altkl_lastup_date | DateTime |  |  |
| 13 | altkl_special1 | Nvarchar(127) |  |  |
| 14 | altkl_special2 | Nvarchar(127) |  |  |
| 15 | altkl_special3 | Nvarchar(127) |  |  |
| 16 | altkl_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | altkl_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | altkl_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | altkl_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | altkl_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | altkl_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | altkl_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | altkl_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | altkl_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | altkl_firmano | Integer | Firma No |  |
| 26 | altkl_subeno | Integer | Şube No |  |
| 27 | altkl_teklif_kodu | Nvarchar(25) | Teklif Kodu |  |
| 28 | altkl_sira_no | Integer | Sıra No |  |
| 29 | altkl_satirno | Integer | Satır No |  |
| 30 | altkl_tarihi | DateTime | Tarihi |  |
| 31 | altkl_belge_no | dbo.belgeno_str | Belge No |  |
| 32 | altkl_belge_tarih | DateTime | Belge Tarihi |  |
| 33 | altkl_cari_kodu | Nvarchar(25) | Cari Kodu |  |
| 34 | altkl_cari_adres_no | Integer | Cari Adres No |  |
| 35 | altkl_cari_yetkili_uid | Uniqueidentifier | Cari Yetkili Uid |  |
| 36 | altkl_teslimat_tarihi | DateTime | Teslimat Tarihi |  |
| 37 | altkl_odeme_plani | Integer | Ödeme Planı |  |
| 38 | altkl_teslim_turu | Nvarchar(4) | Teslim Türü |  |
| 39 | altkl_proje_kodu | Nvarchar(25) | Proje Kodu |  |
| 40 | altkl_srmmrk_kodu | Nvarchar(25) | Sorumluluk Merkezi Kodu |  |
| 41 | altkl_sorumlu_kodu | Nvarchar(25) | Sorumlu Kodu |  |
| 42 | altkl_hareket_tipi | Tinyint | Hareket Tipi | 0:Stok 1:Hizmet 2:Gider 3:Demirbaş |
| 43 | altkl_hareket_kodu | Nvarchar(25) | Hareket Kodu |  |
| 44 | altkl_miktar | Float | Miktar |  |
| 45 | altkl_birim_fiyati | Float | Birim Fiyatı |  |
| 46 | altkl_tutar | Float | Tutar |  |
| 47 | altkl_doviz_cins | Tinyint | Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 48 | altkl_doviz_kur | Float | Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 49 | altkl_alt_doviz_kur | Float | Alternatif Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 50 | altkl_Iskonto1 | Float | İskonto |  |
| 51 | altkl_Iskonto2 | Float | İskonto |  |
| 52 | altkl_Iskonto3 | Float | İskonto |  |
| 53 | altkl_Iskonto4 | Float | İskonto |  |
| 54 | altkl_Iskonto5 | Float | İskonto |  |
| 55 | altkl_Iskonto6 | Float | İskonto |  |
| 56 | altkl_masraf1 | Float | Masraf |  |
| 57 | altkl_masraf2 | Float | Masraf |  |
| 58 | altkl_masraf3 | Float | Masraf |  |
| 59 | altkl_masraf4 | Float | Masraf |  |
| 60 | altkl_vergi_pntr | Tinyint | İlgili Vergi Bağlantısı |  |
| 61 | altkl_vergi | Float | İlgili Vergi Oranı |  |
| 62 | altkl_masraf_vergi_pnt | Tinyint | İlgili Masraf Vergi Bağlantısı |  |
| 63 | altkl_masraf_vergi | Float | İlgili Masraf Vergi Oranı |  |
| 64 | altkl_isk_mas1 | Tinyint | İskonto Masrafı |  |
| 65 | altkl_isk_mas2 | Tinyint | İskonto Masrafı |  |
| 66 | altkl_isk_mas3 | Tinyint | İskonto Masrafı |  |
| 67 | altkl_isk_mas4 | Tinyint | İskonto Masrafı |  |
| 68 | altkl_isk_mas5 | Tinyint | İskonto Masrafı |  |
| 69 | altkl_isk_mas6 | Tinyint | İskonto Masrafı |  |
| 70 | altkl_isk_mas7 | Tinyint | İskonto Masrafı |  |
| 71 | altkl_isk_mas8 | Tinyint | İskonto Masrafı |  |
| 72 | altkl_isk_mas9 | Tinyint | İskonto Masrafı |  |
| 73 | altkl_isk_mas10 | Tinyint | İskonto Masrafı |  |
| 74 | altkl_sat_iskmas1 | Bit | Satış İskonto Masrafı |  |
| 75 | altkl_sat_iskmas2 | Bit | Satış İskonto Masrafı |  |
| 76 | altkl_sat_iskmas3 | Bit | Satış İskonto Masrafı |  |
| 77 | altkl_sat_iskmas4 | Bit | Satış İskonto Masrafı |  |
| 78 | altkl_sat_iskmas5 | Bit | Satış İskonto Masrafı |  |
| 79 | altkl_sat_iskmas6 | Bit | Satış İskonto Masrafı |  |
| 80 | altkl_sat_iskmas7 | Bit | Satış İskonto Masrafı |  |
| 81 | altkl_sat_iskmas8 | Bit | Satış İskonto Masrafı |  |
| 82 | altkl_sat_iskmas9 | Bit | Satış İskonto Masrafı |  |
| 83 | altkl_sat_iskmas10 | Bit | Satış İskonto Masrafı |  |
| 84 | altkl_vergisiz_fl | Bit | Vergisiz Mi ? | 0:Hayır 1:Evet |
| 85 | altkl_fiyat_liste_no | Integer | Fiyat Liste No |  |
| 86 | altkl_paket_kod | Nvarchar(25) | Paket Kodu |  |
| 87 | altkl_teslimdepo | Integer | Teslim Depo |  |
| 88 | altkl_aciklama | Nvarchar(50) | Açıklama |  |
| 89 | altkl_onaylayan_kullanici | Smallint | Onaylayan Kullanıcı |  |
| 90 | altkl_durumu | Tinyint | Durumu | 0:Beklemede 1:Onaylandı 2:Kapatıldı |
| 91 | altkl_satal_talep_uid | Uniqueidentifier | Satın Alınan Talep Uid |  |
| 92 | altkl_siparis_uid | Uniqueidentifier | Sipariş Uid |  |
| 93 | altkl_prosiparis_uId | Uniqueidentifier | Proforma Sipariş Uid |  |
| 94 | altkl_birim_pntr | Tinyint | Birim |  |
| 95 | altkl_cari_tipi | Tinyint | Cari Tipi | 0:Cari 1:Aday |


Güncellenme Tarihi : 19.07.2025 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**