# TABLO NO: 31

## Tablo Adı: CARI_HESAPLAR - Cari Kartları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | cari_Guid | Uniqueidentifier |  |  |
| 1 | cari_DBCno | Smallint |  |  |
| 2 | cari_SpecRECno | Integer |  |  |
| 3 | cari_iptal | Bit |  |  |
| 4 | cari_fileid | Smallint |  |  |
| 5 | cari_hidden | Bit |  |  |
| 6 | cari_kilitli | Bit |  |  |
| 7 | cari_degisti | Bit |  |  |
| 8 | cari_checksum | Integer |  |  |
| 9 | cari_create_user | Smallint |  |  |
| 10 | cari_create_date | DateTime |  |  |
| 11 | cari_lastup_user | Smallint |  |  |
| 12 | cari_lastup_date | DateTime |  |  |
| 13 | cari_special1 | Nvarchar(127) |  |  |
| 14 | cari_special2 | Nvarchar(127) |  |  |
| 15 | cari_special3 | Nvarchar(127) |  |  |
| 16 | cari_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | cari_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | cari_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | cari_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | cari_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | cari_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | cari_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | cari_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | cari_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | cari_kod | Nvarchar(25) | Cari Kodu |  |
| 26 | cari_unvan1 | Nvarchar(127) | Ünvan 1 |  |
| 27 | cari_unvan2 | Nvarchar(127) | Ünvan 2 |  |
| 28 | cari_hareket_tipi | Tinyint | Cari Hareket Tipi | 0:Mal ve Hizmet Alınır ve Satılır 1:Mal ve Hizmet Sadece Satılır 2:Mal ve Hizmet Sadece Alınır 3:Sadece Parasal Hareket Yapılır 4:Cari Hareket Yapılmaz |
| 29 | cari_baglanti_tipi | Tinyint | Cari Bağlantı Tipi | 0:Müşteri 1:Satıcı 2:Diğer Cari 3:Dağıtıcı 4:Bayi 5:Hedef Müşteri 6:Hedef Bayi 7:Alt Bayi 8:Bağlı Ortaklık |
| 30 | cari_stok_alim_cinsi | Tinyint | Stok Alım Cinsi | 0:Toptan ve Perakende 1:Toptan 2:Perakende |
| 31 | cari_stok_satim_cinsi | Tinyint | Stok Satım Cinsi | 0:Toptan ve Perakende 1:Toptan 2:Perakende |
| 32 | cari_muh_kod | Nvarchar(40) | Cari Hesap Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 33 | cari_muh_kod1 | Nvarchar(40) | Cari Hesap Muhasebe Kodu1 | Bkz. MUHASEBE_HESAP_PLANI |
| 34 | cari_muh_kod2 | Nvarchar(40) | Cari Hesap Muhasebe Kodu2 | Bkz. MUHASEBE_HESAP_PLANI |
| 35 | cari_doviz_cinsi | Tinyint | Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 36 | cari_doviz_cinsi1 | Tinyint | Döviz Cinsi1 | Bkz. DOVIZ_KURLARI |
| 37 | cari_doviz_cinsi2 | Tinyint | Döviz Cinsi2 | Bkz. DOVIZ_KURLARI |
| 38 | cari_vade_fark_yuz | Float | Vade Fark Yüzdesi |  |
| 39 | cari_vade_fark_yuz1 | Float | Vade Fark Yüzdesi 1 |  |
| 40 | cari_vade_fark_yuz2 | Float | Vade Fark Yüzdesi 2 |  |
| 41 | cari_KurHesapSekli | Tinyint | Kur Hesaplama Şekli | 1:Döviz Alış 2:Döviz Satış  3:Efektif Alış 4:Efektif Satış |
| 42 | cari_vdaire_adi | Nvarchar(50) | Vergi Dairesi |  |
| 43 | cari_vdaire_no | Nvarchar(15) | Vergi Dairesi No |  |
| 44 | cari_sicil_no | Nvarchar(15) | Sicil No |  |
| 45 | cari_VergiKimlikNo | Nvarchar(10) | Cari Vergi Kimlik No |  |
| 46 | cari_satis_fk | Integer | Cari Satış Fiyat Kodu | Satışlarda hangi stok satış fiyatının uygulanacağını belirtir.    Bkz. STOKLAR |
| 47 | cari_odeme_cinsi | Tinyint | Cari Ödeme Cinsi | 0:Serbest 1:Haftalık 2:Aylık 3:15 Günlük |
| 48 | cari_odeme_gunu | Tinyint | Ödeme Günü |  |
| 49 | cari_odemeplan_no | Integer | Ödeme Plan No | Bkz. ODEME_PLANLARI |
| 50 | cari_opsiyon_gun | Integer | Cari Opsiyon Gün |  |
| 51 | cari_cariodemetercihi | Tinyint | Cari Ödeme Tercihi | 0:Nakit 1:Müşteri Çeki 2:Firma Çeki 3:Müşteri Senedi 4:Firma Senedi 5:Havale 6:Ödeme Emri 7:Doğrudan Havale 8:Firma Kredi Kartı |
| 52 | cari_fatura_adres_no | Integer | Fatura Adres No | Bkz. CARI_HESAP_ADRESLERI |
| 53 | cari_sevk_adres_no | Integer | Sevk Adres No | Bkz. CARI_HESAP_ADRESLERI |
| 54 | cari_banka_tcmb_kod1 | Nvarchar(4) | Banka Kodu 1 |  |
| 55 | cari_banka_tcmb_subekod1 | Nvarchar(8) | Banka Şube Kodu 1 |  |
| 56 | cari_banka_tcmb_ilkod1 | Nvarchar(3) | Banka İl Kodu 1 |  |
| 57 | cari_banka_hesapno1 | Nvarchar(40) | Banka Hesap No 1 |  |
| 58 | cari_banka_swiftkodu1 | Nvarchar(25) | Swift Kodu 1 |  |
| 59 | cari_banka_tcmb_kod2 | Nvarchar(4) | Banka Kodu 2 |  |
| 60 | cari_banka_tcmb_subekod2 | Nvarchar(8) | Banka Şube Kod 2 |  |
| 61 | cari_banka_tcmb_ilkod2 | Nvarchar(3) | Banka İl Kodu 2 |  |
| 62 | cari_banka_hesapno2 | Nvarchar(40) | Banka Hesap No 2 |  |
| 63 | cari_banka_swiftkodu2 | Nvarchar(25) | Swift Kodu 2 |  |
| 64 | cari_banka_tcmb_kod3 | Nvarchar(4) | Banka Kodu 3 |  |
| 65 | cari_banka_tcmb_subekod3 | Nvarchar(8) | Banka Şube Kodu 3 |  |
| 66 | cari_banka_tcmb_ilkod3 | Nvarchar(3) | Banka İl Kodu 3 |  |
| 67 | cari_banka_hesapno3 | Nvarchar(40) | Banka Hesap No 3 |  |
| 68 | cari_banka_swiftkodu3 | Nvarchar(25) | Swift Kodu 3 |  |
| 69 | cari_banka_tcmb_kod4 | Nvarchar(4) | Banka Kodu 4 |  |
| 70 | cari_banka_tcmb_subekod4 | Nvarchar(8) | Banka Şube Kodu 4 |  |
| 71 | cari_banka_tcmb_ilkod4 | Nvarchar(3) | Banka İl Kodu 4 |  |
| 72 | cari_banka_hesapno4 | Nvarchar(40) | Banka Hesap No 4 |  |
| 73 | cari_banka_swiftkodu4 | Nvarchar(25) | Swift Kodu 4 |  |
| 74 | cari_banka_tcmb_kod5 | Nvarchar(4) | Banka Kodu 5 |  |
| 75 | cari_banka_tcmb_subekod5 | Nvarchar(8) | Banka Şube Kodu 5 |  |
| 76 | cari_banka_tcmb_ilkod5 | Nvarchar(3) | Banka İl Kodu 5 |  |
| 77 | cari_banka_hesapno5 | Nvarchar(40) | Banka Hesap No 5 |  |
| 78 | cari_banka_swiftkodu5 | Nvarchar(25) | Swift Kodu 5 |  |
| 79 | cari_banka_tcmb_kod6 | Nvarchar(4) | Banka Kodu 6 |  |
| 80 | cari_banka_tcmb_subekod6 | Nvarchar(8) | Banka Şube Kodu 6 |  |
| 81 | cari_banka_tcmb_ilkod6 | Nvarchar(3) | Banka İl Kodu 6 |  |
| 82 | cari_banka_hesapno6 | Nvarchar(40) | Banka Hesap No 6 |  |
| 83 | cari_banka_swiftkodu6 | Nvarchar(25) | Swift Kodu 6 |  |
| 84 | cari_banka_tcmb_kod7 | Nvarchar(4) | Banka Kodu 7 |  |
| 85 | cari_banka_tcmb_subekod7 | Nvarchar(8) | Banka Şube Kodu 7 |  |
| 86 | cari_banka_tcmb_ilkod7 | Nvarchar(3) | Banka İl Kodu 7 |  |
| 87 | cari_banka_hesapno7 | Nvarchar(40) | Banka Hesap No 7 |  |
| 88 | cari_banka_swiftkodu7 | Nvarchar(25) | Swift Kodu 7 |  |
| 89 | cari_banka_tcmb_kod8 | Nvarchar(4) | Banka Kodu 8 |  |
| 90 | cari_banka_tcmb_subekod8 | Nvarchar(8) | Banka Şube Kodu 8 |  |
| 91 | cari_banka_tcmb_ilkod8 | Nvarchar(3) | Banka İl Kodu 8 |  |
| 92 | cari_banka_hesapno8 | Nvarchar(40) | Banka Hesap No 8 |  |
| 93 | cari_banka_swiftkodu8 | Nvarchar(25) | Swift Kodu 8 |  |
| 94 | cari_banka_tcmb_kod9 | Nvarchar(4) | Banka Kodu 9 |  |
| 95 | cari_banka_tcmb_subekod9 | Nvarchar(8) | Banka Şube Kodu 9 |  |
| 96 | cari_banka_tcmb_ilkod9 | Nvarchar(3) | Banka İl Kodu 9 |  |
| 97 | cari_banka_hesapno9 | Nvarchar(40) | Banka Hesap No 9 |  |
| 98 | cari_banka_swiftkodu9 | Nvarchar(25) | Swift Kodu 9 |  |
| 99 | cari_banka_tcmb_kod10 | Nvarchar(4) | Banka Kodu 10 |  |
| 100 | cari_banka_tcmb_subekod10 | Nvarchar(8) | Banka Şube Kodu 10 |  |
| 101 | cari_banka_tcmb_ilkod10 | Nvarchar(3) | Banka İl Kodu 10 |  |
| 102 | cari_banka_hesapno10 | Nvarchar(40) | Banka Hesap No 10 |  |
| 103 | cari_banka_swiftkodu10 | Nvarchar(25) | Swift Kodu 10 |  |
| 104 | cari_EftHesapNum | Tinyint | Cari Eft Hesap Numarası |  |
| 105 | cari_Ana_cari_kodu | Nvarchar(25) | Ana Cari Kodu |  |
| 106 | cari_satis_isk_kod | Nvarchar(4) | Cari Satış İskonto Kodu | Bkz. STOK_CARI_ISKONTO_TANIMLARI |
| 107 | cari_sektor_kodu | Nvarchar(25) | Sektör Kodu | Bkz. STOK_SEKTORLERI |
| 108 | cari_bolge_kodu | Nvarchar(25) | Cari Bölge Kodu | Bkz. CARI_HESAP_BOLGELERI |
| 109 | cari_grup_kodu | Nvarchar(25) | Cari Grup Kodu | Bkz. CARI_HESAP_GRUPLARI |
| 110 | cari_temsilci_kodu | Nvarchar(25) | Cari Temsilci Kodu | Bkz. PERSONELLER |
| 111 | cari_muhartikeli | Nvarchar(10) | Muhasebe Kod Artikeli | Bkz. MUHASEBE_FISLERI |
| 112 | cari_firma_acik_kapal | Bit | Firma Açık / Kapalı? | 0:Açık 1:Kapalı |
| 113 | cari_BUV_tabi_fl | Bit |  |  |
| 114 | cari_cari_kilitli_flg | Bit | Cari Kilitli Mi? |  |
| 115 | cari_etiket_bas_fl | Bit | Etiket Basılsın Mı ? |  |
| 116 | cari_Detay_incele_flg | Bit | Detay İncelensin Mi ? |  |
| 117 | cari_efatura_fl | Bit | e-Fatura ? |  |
| 118 | cari_POS_ongpesyuzde | Float | POS Öngörülen Peşinat Yüzdesi |  |
| 119 | cari_POS_ongtaksayi | Float | POS Öngörülen Taksit Sayısı |  |
| 120 | cari_POS_ongIskOran | Float | POS Öngörülen İskonto Oranı |  |
| 121 | cari_kaydagiristarihi | DateTime | Kayıt Tarihi |  |
| 122 | cari_KabEdFCekTutar | Float | Kabul Ed. Firma Çek Tutarı |  |
| 123 | cari_hal_caritip | Tinyint | Hal Cari Tipi | 0:Tüccar 1:Müstahsil 2:Çiftçi |
| 124 | cari_HalKomYuzdesi | Float | Hal Komisyon Yüzdesi |  |
| 125 | cari_TeslimSuresi | Smallint | Cari Teslim Süresi |  |
| 126 | cari_wwwadresi | Nvarchar(30) | Cari Web Adresi |  |
| 127 | cari_EMail | Nvarchar(127) | Cari e-Mail Adresi |  |
| 128 | cari_CepTel | Nvarchar(20) | Cari Cep Telefonu |  |
| 129 | cari_VarsayilanGirisDepo | Integer | Varsayılan Giriş Depo |  |
| 130 | cari_VarsayilanCikisDepo | Integer | Varsayılan Çıkış Depo |  |
| 131 | cari_Portal_Enabled | Bit | Portal erişimi açık mı? |  |
| 132 | cari_Portal_PW | Nvarchar(127) | Portal Ulaşım Şifresi |  |
| 133 | cari_BagliOrtaklisa_Firma | Integer | Bağlı Ortaklık İse Firma No |  |
| 134 | cari_kampanyakodu | Nvarchar(4) | Kampanya Kodu |  |
| 135 | cari_b_bakiye_degerlendirilmesin_fl | Bit | Borç Bakiye Değerlendirilmesin ? |  |
| 136 | cari_a_bakiye_degerlendirilmesin_fl | Bit | Alacak Bakiye Değerlendirilmesin ? |  |
| 137 | cari_b_irsbakiye_degerlendirilmesin_fl | Bit | Borç İrsaliye Bakiye Değerlendirilmesin ? |  |
| 138 | cari_a_irsbakiye_degerlendirilmesin_fl | Bit | Alacak İrsaliye Bakiye Değerlendirilmesin ? |  |
| 139 | cari_b_sipbakiye_degerlendirilmesin_fl | Bit | Borç Sipariş Bakiye Değerlendirilmesin ? |  |
| 140 | cari_a_sipbakiye_degerlendirilmesin_fl | Bit | Alacak Sipariş Bakiye Değerlendirilmesin ? |  |
| 141 | cari_KrediRiskTakibiVar_flg | Bit | Kredi Risk Takibi Var Mı ? |  |
| 142 | cari_ufrs_fark_muh_kod | Nvarchar(40) | Ufrs Fark Muhasebe Kodu |  |
| 143 | cari_ufrs_fark_muh_kod1 | Nvarchar(40) | Ufrs Fark Muhasebe Kodu |  |
| 144 | cari_ufrs_fark_muh_kod2 | Nvarchar(40) | Ufrs Fark Muhasebe Kodu |  |
| 145 | cari_odeme_sekli | Tinyint | Ödeme Şekli | 0:Vadeye Göre 1:Satış Üzerinden |
| 146 | cari_TeminatMekAlacakMuhKodu | Nvarchar(40) | Teminat Mektubu Alacak Muhasebe Kodu |  |
| 147 | cari_TeminatMekAlacakMuhKodu1 | Nvarchar(40) | Teminat Mektubu Alacak Muhasebe Kodu |  |
| 148 | cari_TeminatMekAlacakMuhKodu2 | Nvarchar(40) | Teminat Mektubu Alacak Muhasebe Kodu |  |
| 149 | cari_TeminatMekBorcMuhKodu | Nvarchar(40) | Teminat Mektubu Borç Muhasebe Kodu |  |
| 150 | cari_TeminatMekBorcMuhKodu1 | Nvarchar(40) | Teminat Mektubu Borç Muhasebe Kodu |  |
| 151 | cari_TeminatMekBorcMuhKodu2 | Nvarchar(40) | Teminat Mektubu Borç Muhasebe Kodu |  |
| 152 | cari_VerilenDepozitoTeminatMuhKodu | Nvarchar(40) | Verilen Depozito Teminat Muhasebe Kodu |  |
| 153 | cari_VerilenDepozitoTeminatMuhKodu1 | Nvarchar(40) | Verilen Depozito Teminat Muhasebe Kodu |  |
| 154 | cari_VerilenDepozitoTeminatMuhKodu2 | Nvarchar(40) | Verilen Depozito Teminat Muhasebe Kodu |  |
| 155 | cari_AlinanDepozitoTeminatMuhKodu | Nvarchar(40) | Alınan Depozito Teminat Muhasebe Kodu |  |
| 156 | cari_AlinanDepozitoTeminatMuhKodu1 | Nvarchar(40) | Alınan Depozito Teminat Muhasebe Kodu |  |
| 157 | cari_AlinanDepozitoTeminatMuhKodu2 | Nvarchar(40) | Alınan Depozito Teminat Muhasebe Kodu |  |
| 158 | cari_def_efatura_cinsi | Tinyint | Varsayılan e-Fatura Cinsi | 0:Ticari Fatura 1:Temel Fatura 2:Yolcu Beraberinde Fatura 3:İhracat 4:Müstahsil 5:Smm 6:Kamu 7:Hal |
| 159 | cari_otv_tevkifatina_tabii_fl | Bit | Ötv Tevkifatına Tabi Mi ? |  |
| 160 | cari_KEP_adresi | Nvarchar(80) | Kayıtlı e-Posta (KEP) Adresi |  |
| 161 | cari_efatura_baslangic_tarihi | DateTime | e-Fatura Başlangıç Tarihi |  |
| 162 | cari_mutabakat_mail_adresi | Nvarchar(80) | Mutabakat e-Posta Adresi |  |
| 163 | cari_mersis_no | Nvarchar(25) | Mersis Numarası |  |
| 164 | cari_istasyon_cari_kodu | Nvarchar(25) | Akaryakıt İstasyonu Cari Kodu |  |
| 165 | cari_gonderionayi_sms | Bit | Sms Gönderme Onayı Var Mı ? |  |
| 166 | cari_gonderionayi_email | Bit | e-Posta Gönderme Onayı Var Mı ? |  |
| 167 | cari_eirsaliye_fl | Bit | e-İrsaliye Mi ? |  |
| 168 | cari_eirsaliye_baslangic_tarihi | DateTime | e-İrsaliye Başlangıç Tarihi |  |
| 169 | cari_vergidairekodu | Nvarchar(10) | Vergi Dairesi Kodu |  |
| 170 | cari_CRM_sistemine_aktar_fl | Bit | CRM Sistemine Aktarılsın Mı ? |  |
| 171 | cari_efatura_xslt_dosya | Nvarchar(127) | e-Fatura Xslt Dosyası (farklı cari ya da şubeler için farklı e-Fatura dizaynı tasarlanması) |  |
| 172 | cari_pasaport_no | Nvarchar(20) | Pasaport No |  |
| 173 | cari_kisi_kimlik_bilgisi_aciklama_turu | Tinyint | Kişi Kimlik Bilgisi Açıklama Türü | 0:Tanımsız 1:TC Kimlik No Var 2:Yabancı Kimlik No Var 3:Pasaport No Var 4:Yeni Doğan 5:Kimlik Bilinmiyor 6:Diğer 7:YUPASS No Var |
| 174 | cari_kisi_kimlik_bilgisi_diger_aciklama | Nvarchar(50) | Kişi Kimlik Bilgisi Diğer Tür Açıklaması |  |
| 175 | cari_uts_kurum_no | Nvarchar(15) | ÜTS (Ürün Takip Sistemi) Kurum No |  |
| 176 | cari_kamu_kurumu_fl | Bit | e-Fatura Uygulamasına Tabi Kamu Kurumu Mu ? |  |
| 177 | cari_earsiv_xslt_dosya | Nvarchar(127) | e-Arşiv Xslt Dosyası (farklı cari ya da şubeler için farklı e-Arşiv dizaynı tasarlanması) |  |
| 178 | cari_Perakende_fl | Bit | Perakende Carisi Mi ? |  |
| 179 | cari_yeni_dogan_mi | Bit | Yeni Doğan Mı ? |  |
| 180 | cari_eirsaliye_xslt_dosya | Nvarchar(127) | e-İrsaliye Xslt Dosyası |  |
| 181 | cari_def_eirsaliye_cinsi | Tinyint | e-İrsaliye Senaryo Tipi | 0:Temel 1:Hal |
| 182 | cari_ozel_butceli_kurum_carisi | Nvarchar(25) | Özel Bütçeli Kurum Carisi |  |
| 183 | cari_nakakincelenmesi | Bit | Nakit Akışta Göz Ardı Edilsin Mi ? |  |
| 184 | cari_vergimukellefidegil_mi | Bit | Vergi Mükellefi Değil Mi ? |  |
| 185 | cari_tasiyicifirma_cari_kodu | Nvarchar(25) | Taşıyıcı Firma Cari Kodu |  |
| 186 | cari_nacekodu_1 | Nvarchar(25) | Nace Kodu 1 |  |
| 187 | cari_nacekodu_2 | Nvarchar(25) | Nace Kodu 2 |  |
| 188 | cari_nacekodu_3 | Nvarchar(25) | Nace Kodu 3 |  |
| 189 | cari_sirket_turu | Tinyint | Şirket Türü | 0:Belirtilmemiş 1:Gerçek Kişi 2:Adi Ortaklık 3:Kollektif Şirket 4:Adi Komandit Şirket 5:Eshamlı Komandit Şirket 6:Limited Şirket 7:Anonim Şirket 8:Kooperatif 9:Diğer Şirket 10:Avukat Ortaklığı 11:Banka Şubesi |
| 190 | cari_baba_adi | Nvarchar(50) | Baba Adı |  |
| 191 | cari_faal_terk | Tinyint | Şirket Faal Mı Terk Mi ? | 0:Faal 1:Terk |
| 192 | cari_siparis_avans_muh_kod | Nvarchar(40) | Sipariş Avans Muhasebe Kodu |  |
| 193 | cari_siparis_avans_muh_kod1 | Nvarchar(40) | Sipariş Avans Muhasebe Kodu |  |
| 194 | cari_siparis_avans_muh_kod2 | Nvarchar(40) | Sipariş Avans Muhasebe Kodu |  |
| 195 | cari_SorumlulukMerkezi | Nvarchar(25) | Sorumluluk Merkezi |  |


Güncellenme Tarihi : 10.06.2025