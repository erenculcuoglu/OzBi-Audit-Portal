# TABLO NO: 16

## Tablo Adı: STOK_HAREKETLERI - Stok Hareketleri

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | sth_Guid | Uniqueidentifier |  |  |
| 1 | sth_DBCno | Smallint |  |  |
| 2 | sth_SpecRECno | Integer |  |  |
| 3 | sth_iptal | Bit |  |  |
| 4 | sth_fileid | Smallint |  |  |
| 5 | sth_hidden | Bit |  |  |
| 6 | sth_kilitli | Bit |  |  |
| 7 | sth_degisti | Bit |  |  |
| 8 | sth_checksum | Integer |  |  |
| 9 | sth_create_user | Smallint |  |  |
| 10 | sth_create_date | DateTime |  |  |
| 11 | sth_lastup_user | Smallint |  |  |
| 12 | sth_lastup_date | DateTime |  |  |
| 13 | sth_special1 | Nvarchar(127) |  |  |
| 14 | sth_special2 | Nvarchar(127) |  |  |
| 15 | sth_special3 | Nvarchar(127) |  |  |
| 16 | sth_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | sth_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | sth_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | sth_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | sth_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | sth_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | sth_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | sth_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | sth_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | sth_firmano | Integer | Firma No |  |
| 26 | sth_subeno | Integer | Şube No |  |
| 27 | sth_tarih | DateTime | Hareket Tarihi |  |
| 28 | sth_tip | Tinyint | Hareket Tipi | 0:Giriş 1:Çıkış 2:Depo Transfer |
| 29 | sth_cins | Tinyint | Hareket Cinsi | 0:Toptan 1:Perakende 2:Dış Ticaret 3:Stok Virman 4:Fire 5:Sarf   6:Transfer 7:Üretim 8:Fason 9:Değer Farkı 10:Sayım 11:Stok Açılış   12:İthalat-İhracat 13:Hal 14:Müstahsil 15:Müstahsil Değer Farkı 14:Kabzımal 15:Gider Pusulası |
| 30 | sth_normal_iade | Tinyint | Normal / İade ? | 0:Normal 1:İade |
| 31 | sth_evraktip | Tinyint | Evrak Tipi | 0:Depo Çıkış Fişi 1:Çıkış İrsaliyesi 2:Depo Transfer Fişi   3:Giriş Faturası 4:Çıkış Faturası 5:Stoklara İthalat Masraf Yansıtma Dekontu  6:Stok Virman Fişi 7:Üretim Fişi 8:İlave Enflasyon Maliyet Fişi 9:Stoklara İlave Maliyet Yedirme Fişi 10:Antrepolardan Mal Millileştirme Fişi 11:Antrepolar Arası Transfer Fişi 12:Depo Giriş Fişi 13:Giriş İrsaliyesi 14:Fason Giriş Çıkış Fişi 15:Depolar Arası Satış Fişi 16:Stok Gider Pusulası Fişi 17:Depolar Arası Nakliye Fişi 18:Demirbaşa Virman Dekontu |
| 32 | sth_evrakno_seri | dbo.evrakseri_str | Evrak Seri No |  |
| 33 | sth_evrakno_sira | Integer | Evrak Sıra No |  |
| 34 | sth_satirno | Integer | Hareket Satır No |  |
| 35 | sth_belge_no | dbo.belgeno_str | Hareket Belge No |  |
| 36 | sth_belge_tarih | DateTime | Hareket Belge Tarihi |  |
| 37 | sth_stok_kod | Nvarchar(25) | Stok Kodu |  |
| 38 | sth_isk_mas1 | Tinyint | İskonto Masraf Tipi | 0:Brüt toplamdan yüzde, 1:Ara toplamdan yüzde, 2:Tutar İskonto masraf, 3:Miktar başına tutar, 4:Miktar2 başına tutar, 5:Miktar3 başına tutar, 6:Bedelsiz miktar, 7:İskonto1 yuzde, 8:İskonto1 aratop yüzde, 9:İskonto2 yüzde, 10:İskonto2 aratop yüzde, 11:İskonto3 yüzde, 12:İskonto3 aratop yüzde, 13:İskonto4 yüzde, 14:İskonto4 aratop yüzde, 15:İskonto5 yüzde, 16:İskonto5 aratop yüzde, 17:İskonto6 yüzde, 18:İskonto6 aratop yüzde, 19:Masraf1 yüzde, 20:Masraf1 aratop yüzde, 21:Masraf2 yüzde, 22:Masraf2 aratop yüzde, 23:Masraf3 yüzde, 24:Masraf3 aratop yüzde |
| 39 | sth_isk_mas2 | Tinyint | İskonto Masraf Tipi | Yukardaki ile Aynı |
| 40 | sth_isk_mas3 | Tinyint | İskonto Masraf Tipi | Yukardaki ile Aynı |
| 41 | sth_isk_mas4 | Tinyint | İskonto Masraf Tipi | Yukardaki ile Aynı |
| 42 | sth_isk_mas5 | Tinyint | İskonto Masraf Tipi | Yukardaki ile Aynı |
| 43 | sth_isk_mas6 | Tinyint | İskonto Masraf Tipi | Yukardaki ile Aynı |
| 44 | sth_isk_mas7 | Tinyint | İskonto Masraf Tipi | Yukardaki ile Aynı |
| 45 | sth_isk_mas8 | Tinyint | İskonto Masraf Tipi | Yukardaki ile Aynı |
| 46 | sth_isk_mas9 | Tinyint | İskonto Masraf Tipi | Yukardaki ile Aynı |
| 47 | sth_isk_mas10 | Tinyint | İskonto Masraf Tipi | Yukardaki ile Aynı |
| 48 | sth_sat_iskmas1 | Bit | Satır İskonto Masraf mı? |  |
| 49 | sth_sat_iskmas2 | Bit | Satır İskonto Masraf mı? |  |
| 50 | sth_sat_iskmas3 | Bit | Satır İskonto Masraf mı? |  |
| 51 | sth_sat_iskmas4 | Bit | Satır İskonto Masraf mı? |  |
| 52 | sth_sat_iskmas5 | Bit | Satır İskonto Masraf mı? |  |
| 53 | sth_sat_iskmas6 | Bit | Satır İskonto Masraf mı? |  |
| 54 | sth_sat_iskmas7 | Bit | Satır İskonto Masraf mı? |  |
| 55 | sth_sat_iskmas8 | Bit | Satır İskonto Masraf mı? |  |
| 56 | sth_sat_iskmas9 | Bit | Satır İskonto Masraf mı? |  |
| 57 | sth_sat_iskmas10 | Bit | Satır İskonto Masraf mı? |  |
| 58 | sth_pos_satis | Tinyint | Pos Satış Hareketi | 0:Standart Faturalar 1:Dış Kasadan Gelen Faturalar 2:Hızlı Satıştan Gelen Faturalar 3:Shopside'dan Gelen Faturalar |
| 59 | sth_promosyon_fl | Bit | Promosyon Var mı? |  |
| 60 | sth_cari_cinsi | Tinyint | Cari Cinsi | 0:Carimiz 1:Cari Personelimiz 2:Bankamız 3:Hizmetimiz 4:Kasamız   5:Giderimiz 6:Muhasebe Hesabımız 7:Personelimiz 8:Demirbaşımız 9:İthalat Dosyamız 10:Finansal Sözleşmemiz 11:Kredi Sözleşmemiz 12:Dönemsel Hizmetimiz 13:Kredi Kartımız |
| 61 | sth_cari_kodu | Nvarchar(25) | Cari Kodu |  |
| 62 | sth_cari_grup_no | Tinyint | Cari Grup No |  |
| 63 | sth_isemri_gider_kodu | Nvarchar(10) | İşemri Gider Kodu |  |
| 64 | sth_plasiyer_kodu | Nvarchar(25) | Plasiyer Kodu |  |
| 65 | sth_har_doviz_cinsi | Tinyint | Hareket Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 66 | sth_har_doviz_kuru | Float | Hareket Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 67 | sth_alt_doviz_kuru | Float | Alternatif Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 68 | sth_stok_doviz_cinsi | Tinyint | Stok Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 69 | sth_stok_doviz_kuru | Float | Stok Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 70 | sth_miktar | Float | Hareket Miktarı |  |
| 71 | sth_miktar2 | Float | Hareket 2. Miktarı |  |
| 72 | sth_birim_pntr | Tinyint | Barkodun ilgili stoğun hangi birimine ait olduğunu   gösterir. | Bkz. Tablo STOKLAR |
| 73 | sth_birimfiyat | Float | Birim Fiyat |  |
| 74 | sth_tutar | Float | Hareket Tutarı |  |
| 75 | sth_iskonto1 | Float | 1.İskonto Miktarı |  |
| 76 | sth_iskonto2 | Float | 2.İskonto Miktarı |  |
| 77 | sth_iskonto3 | Float | 3.İskonto Miktarı |  |
| 78 | sth_iskonto4 | Float | 4.İskonto Miktarı |  |
| 79 | sth_iskonto5 | Float | 5.İskonto Miktarı |  |
| 80 | sth_iskonto6 | Float | 6.İskonto Miktarı |  |
| 81 | sth_masraf1 | Float | 1.Masraf Miktarı |  |
| 82 | sth_masraf2 | Float | 2.Masraf Miktarı |  |
| 83 | sth_masraf3 | Float | 3.Masraf Miktarı |  |
| 84 | sth_masraf4 | Float | 4.Masraf Miktarı |  |
| 85 | sth_vergi_pntr | Tinyint | Hareketle İlgili Vergi Bağlantısı |  |
| 86 | sth_vergi | Float | Hareket Vergi Oranı |  |
| 87 | sth_masraf_vergi_pntr | Tinyint | Masraf Vergisiyle İlgili Vergi Bağlantısı |  |
| 88 | sth_masraf_vergi | Float | Masraf Vergi Oranı |  |
| 89 | sth_netagirlik | Float | Net Ağırlık |  |
| 90 | sth_odeme_op | Integer | Ödeme Planı |  |
| 91 | sth_aciklama | Nvarchar(50) | Hareketle İlgili Açıklama |  |
| 92 | sth_sip_uid | Uniqueidentifier | Sipariş Uid | Bkz. SIPARISLER |
| 93 | sth_fat_uid | Uniqueidentifier | Fatura Uid | Bkz. CARI_HESAP_HAREKETLERI |
| 94 | sth_giris_depo_no | Integer | Giriş Depo No |  |
| 95 | sth_cikis_depo_no | Integer | Çıkış Depo No |  |
| 96 | sth_malkbl_sevk_tarihi | DateTime | Mal Kabul Sevkiyat Tarihi |  |
| 97 | sth_cari_srm_merkezi | Nvarchar(25) | Cari Sorumluluk Merkezi | Bkz. SORUMLULUK_MERKEZLERI |
| 98 | sth_stok_srm_merkezi | Nvarchar(25) | Stok Sorumluluk Merkezi | Bkz. SORUMLULUK_MERKEZLERI |
| 99 | sth_fis_tarihi | DateTime | Fiş Tarihi |  |
| 100 | sth_fis_sirano | Integer | Fiş Sıra No | Bkz. MUHASEBE_FISLERI |
| 101 | sth_vergisiz_fl | Bit | Vergisiz? |  |
| 102 | sth_maliyet_ana | Float | Ana Maliyet |  |
| 103 | sth_maliyet_alternatif | Float | Alternatif Maliyet |  |
| 104 | sth_maliyet_orjinal | Float | Orjinal Maliyet |  |
| 105 | sth_adres_no | Integer | Adres No |  |
| 106 | sth_parti_kodu | Nvarchar(25) | Parti Kodu | Bkz. PARTILOT |
| 107 | sth_lot_no | Integer | Parti Lot No | Bkz. PARTILOT |
| 108 | sth_kons_uid | Uniqueidentifier | Konsinye Uid | Bkz. KONSINYE_HAREKETLERI |
| 109 | sth_proje_kodu | Nvarchar(25) | Proje Kodu | Bkz. PROJELER |
| 110 | sth_exim_kodu | Nvarchar(25) | Exim Kodu |  |
| 111 | sth_otv_pntr | Tinyint | ÖTV İle İlgili Bağlantıısı |  |
| 112 | sth_otv_vergi | Float | ÖTV Vergisi |  |
| 113 | sth_brutagirlik | Float | Brüt Ağırlık |  |
| 114 | sth_disticaret_turu | Tinyint | Dış Ticaret Türü | 0:Toptan Yurtiçi Ticaret 1:Perakende Yurtiçi Ticaret 2:İhraç Kayıtlı Yurtiçi Ticaret 3:Yurtdışı Ticaret 4:Yurtdışı Nitelikli İhraç Kayıtlı Ticaret 5:Yurtdışı Nitelikli Yurtiçi Ticaret |
| 115 | sth_otvtutari | Float | Ötv Tutarı |  |
| 116 | sth_otvvergisiz_fl | Bit | Ötv Vergili Mi ? | 0:Vergili 1:Vergisiz |
| 117 | sth_oiv_pntr | Tinyint |  |  |
| 118 | sth_oiv_vergi | Float |  |  |
| 119 | sth_oivvergisiz_fl | Bit |  |  |
| 120 | sth_fiyat_liste_no | Integer | Fiyat Liste No |  |
| 121 | sth_oivtutari | Float | Özel İletişim Vergisi Tutarı |  |
| 122 | sth_Tevkifat_turu | Tinyint | Tevkifat Türü | 0:Yok 1:10'da 3 2:10'da 9 3:21 4:32 5:61 6:45 7:Tam 8:10'da 2 9:10'da 5 10:10'da 7 |
| 123 | sth_nakliyedeposu | Integer | Nakliye Deposu |  |
| 124 | sth_nakliyedurumu | Tinyint | Nakliye Durumu | 0:Yolda 1:Teslim Edildi |
| 125 | sth_yetkili_uid | Uniqueidentifier | Yetkili Uid |  |
| 126 | sth_taxfree_fl | Bit |  |  |
| 127 | sth_ilave_edilecek_kdv | Float | İlave Edilecek Kdv |  |
| 128 | sth_ismerkezi_kodu | Nvarchar(25) | İş Merkezi Kodu |  |
| 129 | sth_HareketGrupKodu1 | Nvarchar(25) | Hareket Grup Kodu 1 |  |
| 130 | sth_HareketGrupKodu2 | Nvarchar(25) | Hareket Grup Kodu 2 |  |
| 131 | sth_HareketGrupKodu3 | Nvarchar(25) | Hareket Grup Kodu 3 |  |
| 132 | sth_Olcu1 | Float | Ölçü 1 |  |
| 133 | sth_Olcu2 | Float | Ölçü 2 |  |
| 134 | sth_Olcu3 | Float | Ölçü 3 |  |
| 135 | sth_Olcu4 | Float | Ölçü 4 |  |
| 136 | sth_Olcu5 | Float | Ölçü 5 |  |
| 137 | sth_FormulMiktarNo | Tinyint | Formül Numarası |  |
| 138 | sth_FormulMiktar | Float | Formülle Hesaplanan Miktar |  |
| 139 | sth_eirs_senaryo | Tinyint | e-İrsaliye Senaryo Tipi | 0:Temel 1:Hal |
| 140 | sth_eirs_tipi | Tinyint | e-İrsaliye Tipi | 0:Sevk 1:Matbu |
| 141 | sth_teslim_tarihi | DateTime | Teslim Tarihi |  |
| 142 | sth_matbu_fl | Bit | Matbu Evrak Mı ? |  |
| 143 | sth_satis_fiyat_doviz_cinsi | Tinyint | Satış Fiyatı Döviz Cinsi |  |
| 144 | sth_satis_fiyat_doviz_kuru | Float | Satış Fiyatı Döviz Kuru |  |
| 145 | sth_eticaret_kanal_kodu | Nvarchar(25) | e-Ticaret Kanal Kodu |  |
| 146 | sth_bagli_ithalat_kodu | Nvarchar(25) | Bağlı İthalat Kodu |  |
| 147 | sth_tevkifat_sifirlandi_fl | Bit | Tevkifat Tutarı Sıfırlansın Mı ? |  |


Güncellenme Tarihi : 27.11.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**