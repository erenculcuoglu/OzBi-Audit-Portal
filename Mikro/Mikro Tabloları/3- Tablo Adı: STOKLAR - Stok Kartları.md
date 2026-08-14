# TABLO NO: 13

## Tablo Adı: STOKLAR - Stok Kartları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | sto_Guid | Uniqueidentifier |  |  |
| 1 | sto_DBCno | Smallint |  |  |
| 2 | sto_SpecRECno | Integer |  |  |
| 3 | sto_iptal | Bit |  |  |
| 4 | sto_fileid | Smallint |  |  |
| 5 | sto_hidden | Bit |  |  |
| 6 | sto_kilitli | Bit |  |  |
| 7 | sto_degisti | Bit |  |  |
| 8 | sto_checksum | Integer |  |  |
| 9 | sto_create_user | Smallint |  |  |
| 10 | sto_create_date | DateTime |  |  |
| 11 | sto_lastup_user | Smallint |  |  |
| 12 | sto_lastup_date | DateTime |  |  |
| 13 | sto_special1 | Nvarchar(127) |  |  |
| 14 | sto_special2 | Nvarchar(127) |  |  |
| 15 | sto_special3 | Nvarchar(127) |  |  |
| 16 | sto_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | sto_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | sto_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | sto_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | sto_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | sto_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | sto_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | sto_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | sto_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | sto_kod | Nvarchar(25) | Stok Kodu |  |
| 26 | sto_isim | Nvarchar(127) | Stok Adı |  |
| 27 | sto_kisa_ismi | Nvarchar(50) | Stok Kısa Adı |  |
| 28 | sto_yabanci_isim | Nvarchar(127) | Stok Yabancı İsim |  |
| 29 | sto_sat_cari_kod | Nvarchar(25) | Satıcı Cari Kodu | Bkz. Tablo CARI_HESAPLAR |
| 30 | sto_cins | Tinyint | Stok Cinsi | 0:Ticari Mal 1:İlk Madde 2:Ara Mamül  3:Yarı Mamül 4:Mamül 5:Yan Mamül   6:İşletme Malzemesi 7:Tüketim Malzemesi 8:Yedek Parça   9:Akaryakıt Stok 10:Montaj Reçeteli Mamül 11:Temel Hammadde |
| 31 | sto_doviz_cinsi | Tinyint | Döviz No | Bkz. DOVIZ_KURLARI |
| 32 | sto_detay_takip | Tinyint | Detay Takibi | 0:Detay takip yok 1:Parti bazında   2:Parti+lot bazında 3:Seri no bazında 4:Bağ bazında 5:Parti+lot+bağ bazında |
| 33 | sto_birim1_ad | Nvarchar(10) | Birim Adı |  |
| 34 | sto_birim1_katsayi | Float | Birim Katsayı |  |
| 35 | sto_birim1_agirlik | Float | Birim Net Ağırlık (kg) |  |
| 36 | sto_birim1_en | Float | Birim En (mm) |  |
| 37 | sto_birim1_boy | Float | Birim Boy (mm) |  |
| 38 | sto_birim1_yukseklik | Float | Birim Yükseklik (mm) |  |
| 39 | sto_birim1_dara | Float |  |  |
| 40 | sto_birim2_ad | Nvarchar(10) | Birim Adı |  |
| 41 | sto_birim2_katsayi | Float | Birim Katsayı |  |
| 42 | sto_birim2_agirlik | Float | Birim Net Ağırlık (kg) |  |
| 43 | sto_birim2_en | Float | Birim En (mm) |  |
| 44 | sto_birim2_boy | Float | Birim Boy (mm) |  |
| 45 | sto_birim2_yukseklik | Float | Birim Yükseklik (mm) |  |
| 46 | sto_birim2_dara | Float |  |  |
| 47 | sto_birim3_ad | Nvarchar(10) | Birim Adı |  |
| 48 | sto_birim3_katsayi | Float | Birim Katsayı |  |
| 49 | sto_birim3_agirlik | Float | Birim Net Ağırlık (kg) |  |
| 50 | sto_birim3_en | Float | Birim En (mm) |  |
| 51 | sto_birim3_boy | Float | Birim Boy (mm) |  |
| 52 | sto_birim3_yukseklik | Float | Birim Yükseklik (mm) |  |
| 53 | sto_birim3_dara | Float |  |  |
| 54 | sto_birim4_ad | Nvarchar(10) | Birim Adı |  |
| 55 | sto_birim4_katsayi | Float | Birim Katsayı |  |
| 56 | sto_birim4_agirlik | Float | Birim Net Ağırlık (kg) |  |
| 57 | sto_birim4_en | Float | Birim En (mm) |  |
| 58 | sto_birim4_boy | Float | Birim Boy (mm) |  |
| 59 | sto_birim4_yukseklik | Float | Birim Yükseklik (mm) |  |
| 60 | sto_birim4_dara | Float |  |  |
| 61 | sto_muh_kod | Nvarchar(40) | Stok Muh. Hesap Kodu |  |
| 62 | sto_muh_Iade_kod | Nvarchar(40) | Stok Muh. İade Kodu |  |
| 63 | sto_muh_sat_muh_kod | Nvarchar(40) | Stok Muh. Satış Kodu |  |
| 64 | sto_muh_satIadmuhkod | Nvarchar(40) | Stok Muh. Satış İade Kodu |  |
| 65 | sto_muh_sat_isk_kod | Nvarchar(40) | Stok Muh. İskonto Kodu |  |
| 66 | sto_muh_aIiskmuhkod | Nvarchar(40) | Stok Muh. Alış İskonto Kodu |  |
| 67 | sto_muh_satmalmuhkod | Nvarchar(40) | Stok Muh. Satış Maliyet Kodu |  |
| 68 | sto_yurtdisi_satmuhk | Nvarchar(40) | Stok Muh. Yurt Dışı Satış Kodu |  |
| 69 | sto_ilavemasmuhkod | Nvarchar(40) | Stok Muh. İlave Masraflar Kodu |  |
| 70 | sto_yatirimtesmuhkod | Nvarchar(40) | Yatırım Teşvik Muh. Kodu |  |
| 71 | sto_depsatmuhkod | Nvarchar(40) | Depolar Arası Satış Muh. Kodu |  |
| 72 | sto_depsatmalmuhkod | Nvarchar(40) | Depolar Arası Satış Maliyeti Muh. Kodu |  |
| 73 | sto_bagortsatmuhkod | Nvarchar(40) | Bağlı Ortaklılara Satış Muh. Kodu |  |
| 74 | sto_bagortsatIadmuhkod | Nvarchar(40) | Bağlı Ortaklılara Satış İade Muh. Kodu |  |
| 75 | sto_bagortsatIskmuhkod | Nvarchar(40) | Bağlı Ortaklılara Satış İskonto Muh. Kodu |  |
| 76 | sto_satfiyfarksmuhkod | Nvarchar(40) | Satış Fiyat Farkı Muh. Kodu |  |
| 77 | sto_yurtdisisatmalmuhkod | Nvarchar(40) | Yurt Dışı Satış Maliyeti Muh. Kodu |  |
| 78 | sto_bagortsatmalmuhkod | Nvarchar(40) | Bağlı Ortaklık Satış Maliyeti Muh. Kodu |  |
| 79 | sto_sifirbedsatmalmuhkod | Nvarchar(40) | Sıfır Bedelli Satış Maliyeti Muh. Kodu |  |
| 80 | sto_ihrackayitlisatismuhkod | Nvarchar(40) | İhraç Kayıtlı Satış Muh. Kodu |  |
| 81 | sto_ihrackayitlisatismaliyetimuhkod | Nvarchar(40) | İhraç Kayıtlı Satış Maliyeti Muh. Kodu |  |
| 82 | sto_karorani | Float | Kar Oranı |  |
| 83 | sto_min_stok | Float | Stok Minimum Seviye |  |
| 84 | sto_siparis_stok | Float | Stok Sipariş Seviyesi |  |
| 85 | sto_max_stok | Float | Stok Maksimum Seviye |  |
| 86 | sto_ver_sip_birim | Tinyint | Verilen Sipariş Birimi |  |
| 87 | sto_al_sip_birim | Tinyint | Alınan Sipariş Birimi |  |
| 88 | sto_siparis_sure | Smallint | Sipariş Süresi (Gün) |  |
| 89 | sto_perakende_vergi | Tinyint | Perakende KDV Oranı |  |
| 90 | sto_toptan_vergi | Tinyint | Toptan KDV Oranı |  |
| 91 | sto_yer_kod | Nvarchar(25) | Ambar Adresi |  |
| 92 | sto_elk_etk_tipi | Tinyint | Elektronik Etiket Tipi | 0:Standart Etiket 1:Küçük Etiket 2:Meyve Sebze Etiketi |
| 93 | sto_raf_etiketli | Tinyint | Raf Etiketi Var Mı ? | 0:Yok 1:Var |
| 94 | sto_etiket_bas | Tinyint | Etiket Basılsın Mı ? | 0:Basılmasın 1:Basılsın |
| 95 | sto_satis_dursun | Tinyint | Satış Dursun Mu ? | 0:Durmasın 1:Dursun |
| 96 | sto_siparis_dursun | Tinyint | Sipariş Dursun Mu ? | 0:Durmasın 1:Dursun |
| 97 | sto_malkabul_dursun | Tinyint | Mal Kabul Dursun Mu ? | 0:Durmasın 1:Dursun |
| 98 | sto_malkabul_gun1 | Bit | Mal Kabul Günü | Pazartesi |
| 99 | sto_malkabul_gun2 | Bit | Mal Kabul Günü | Salı |
| 100 | sto_malkabul_gun3 | Bit | Mal Kabul Günü | Çarşamba |
| 101 | sto_malkabul_gun4 | Bit | Mal Kabul Günü | Perşembe |
| 102 | sto_malkabul_gun5 | Bit | Mal Kabul Günü | Cuma |
| 103 | sto_malkabul_gun6 | Bit | Mal Kabul Günü | Cumartesi |
| 104 | sto_malkabul_gun7 | Bit | Mal Kabul Günü | Pazar |
| 105 | sto_siparis_gun1 | Bit | Sipariş Günleri | Pazartesi |
| 106 | sto_siparis_gun2 | Bit | Sipariş Günleri | Salı |
| 107 | sto_siparis_gun3 | Bit | Sipariş Günleri | Çarşamba |
| 108 | sto_siparis_gun4 | Bit | Sipariş Günleri | Perşembe |
| 109 | sto_siparis_gun5 | Bit | Sipariş Günleri | Cuma |
| 110 | sto_siparis_gun6 | Bit | Sipariş Günleri | Cumartesi |
| 111 | sto_siparis_gun7 | Bit | Sipariş Günleri | Pazar |
| 112 | sto_iskon_yapilamaz | Bit | İskonto Yapılamaz ? | 0:Evet 1:Hayır |
| 113 | sto_tasfiyede | Bit | Tasfiyede? | 0:Evet 1:Hayır |
| 114 | sto_alt_grup_no | Smallint | Alt Grup No |  |
| 115 | sto_kategori_kodu | Nvarchar(25) | Stok Kategori Kodu |  |
| 116 | sto_urun_sorkod | Nvarchar(25) | Ürün Sorumlusu Kodu | Bkz. Tablo PERSONELLER |
| 117 | sto_altgrup_kod | Nvarchar(25) | Stok Alt Grup Kodu | Bkz. Tablo STOK_ALT_GRUPLARI |
| 118 | sto_anagrup_kod | Nvarchar(25) | Stok Ana Grup Kodu | Bkz. Tablo STOK_ANA_GRUPLARI |
| 119 | sto_uretici_kodu | Nvarchar(25) | Üretici Kodu | Bkz. Tablo STOK_URETICILERI |
| 120 | sto_sektor_kodu | Nvarchar(25) | Sektör Kodu | Bkz. Tablo STOK_SEKTORLERI |
| 121 | sto_reyon_kodu | Nvarchar(25) | Reyon Kodu | Bkz. Tablo STOK_REYONLARI |
| 122 | sto_muhgrup_kodu | Nvarchar(25) | Muh. Grup Kodu | Bkz. Tablo STOK_MUHASEBE_GRUPLARI |
| 123 | sto_ambalaj_kodu | Nvarchar(25) | Ambalaj Kodu | Bkz. Tablo STOK_AMBALAJLARI |
| 124 | sto_marka_kodu | Nvarchar(25) | Marka Kodu | Bkz. Tablo STOK_MARKALARI |
| 125 | sto_model_kodu | Nvarchar(25) | Model Kodu | Bkz. Tablo STOK_MODEL_TANIMLARI |
| 126 | sto_sezon_kodu | Nvarchar(25) | Sezon Kodu | Bkz. Tablo STOK_YILSEZON_TANIMLARI |
| 127 | sto_hammadde_kodu | Nvarchar(25) | Hammadde Kodu | Bkz. Tablo STOK_ANAHAMMADDELERI |
| 128 | sto_prim_kodu | Nvarchar(25) | Prim Kodu |  |
| 129 | sto_kalkon_kodu | Nvarchar(25) | Kalite Kontrol Kodu | Bkz. Tablo STOK_KALITE_KONTROL_TANIMLARI |
| 130 | sto_paket_kodu | Nvarchar(25) | Paket Kodu | Bkz. Tablo STOK_PAKET_TANIMLARI |
| 131 | sto_pozisyonbayrak_kodu | Nvarchar(25) | Pozisyon Bayrak Kodu |  |
| 132 | sto_mkod_artik | Nvarchar(10) | Stok Muhasebe Kod Artıkeli |  |
| 133 | sto_kasa_tarti_fl | Bit | Kasada Tartılan Mal Mı ? | 0:Evet 1:Hayır |
| 134 | sto_miktarondalikli_fl | Bit | Ondalıklı Üretebiliyor Mu ? |  |
| 135 | sto_pasif_fl | Bit | Aktif/Pasif | 0:Pasif 1:Aktif |
| 136 | sto_eksiyedusebilir_fl | Bit | Stok Eksiye Düşebilir Mi ? |  |
| 137 | sto_GtipNo | Nvarchar(25) | Gümrük Tarifesi İstatistik Pozisyon No |  |
| 138 | sto_puan | Float |  |  |
| 139 | sto_komisyon_hzmkodu | Nvarchar(25) | Komisyon Hizmet Kodu |  |
| 140 | sto_komisyon_orani | Float | Komisyon Oranı |  |
| 141 | sto_otvuygulama | Tinyint | ÖTV Uygulama | 0:ÖTV Yok 1:Alışta tutardan 2:Alışta yüzdeyle 3:Satışta tutardan 4:Satışta yüzdeyle 5:Alışta ve satışta tutardan 6:Alışta ve satışta yüzdeyle |
| 142 | sto_otvtutar | Float | ÖTV Tutar |  |
| 143 | sto_otvliste | Tinyint | ÖTV Tipi | 0:Yok 1:ÖTV1 2:ÖTV2 3:ÖTV3 4:ÖTV4 5:ÖTV3a 6:ÖTV3b 7:ÖTV3c |
| 144 | sto_otvbirimi | Tinyint | Stok ÖTV Birimi |  |
| 145 | sto_prim_orani | Float | Prim Oranı |  |
| 146 | sto_garanti_sure | Smallint | Öngörülen Garanti Süresi |  |
| 147 | sto_garanti_sure_tipi | Tinyint | Garanti Süre Tipi | 0:Ay 1:Gün 2:Yıl |
| 148 | sto_iplik_Ne_no | Float | İplik Ne Numarası |  |
| 149 | sto_standartmaliyet | Float | Standart Maliyet |  |
| 150 | sto_kanban_kasa_miktari | Float | Stok Kanban Kasa Miktarı |  |
| 151 | sto_oivuygulama | Tinyint | Özel İletişim Vergisi Uygulaması Var Mı ? | 0:Yok 1:Var |
| 152 | sto_zraporu_stoku_fl | Bit | Z Raporu ? |  |
| 153 | sto_maxiskonto_orani | Float | Maksimum İskonto Oranı |  |
| 154 | sto_detay_takibinde_depo_kontrolu_fl | Bit | Detay Takibinde Depo Kontrolü Var Mı ? |  |
| 155 | sto_tamamlayici_kodu | Nvarchar(25) | Tamamlayıcı Kodu |  |
| 156 | sto_oto_barkod_acma_sekli | Tinyint | Otomatik Barkod Açma Şekli | 0:Otomatik Barkod Oluşturulmasın 1:Detay Takip Şekline Göre Otomatik Barkod Oluşturulsun 2:Her Giriş Kaydı İçin Barkod Oluşturulsun |
| 157 | sto_oto_barkod_kod_yapisi | dbo.barkod_str | Otomatik Barkod Kod Yapısı |  |
| 158 | sto_KasaIskontoOrani | Float | Kasa İskonto Oranı |  |
| 159 | sto_KasaIskontoTutari | Float | Kasa İskonto Tutarı |  |
| 160 | sto_gelirpayi | Float | Gelir Payı |  |
| 161 | sto_oivtutar | Float | Özet İletişim Vergisi Tutarı |  |
| 162 | sto_oivturu | Tinyint | Özet İletişim Vergisi Türü | 0:Yok 1:ÖİV 2:5035 sayılı kanuna göre ÖİV |
| 163 | sto_giderkodu | Nvarchar(25) | Gider Kodu |  |
| 164 | sto_oivvergipntr | Tinyint | ÖİV |  |
| 165 | sto_Tevkifat_turu | Tinyint | Tevkifat Türü | 0:Yok 1:10'da 3 2:10'da 9 3:21 4:32 5:61 6:45 7:Tam 8:10'da 2 9:10'da 5 10:10'da 7 |
| 166 | sto_SKT_fl | Bit | Son Kullanma Tarihi Var Mı ? |  |
| 167 | sto_terazi_SKT | Smallint | Terazi Son Kullanma Tarihi |  |
| 168 | sto_RafOmru | Smallint | Raf Ömrü |  |
| 169 | sto_KasadaTaksitlenebilir_fl | Bit | Kasada Taksitlenebilir Mi ? |  |
| 170 | sto_ufrsfark_kod | Nvarchar(40) | Ufrs Fark Muh. Kodu |  |
| 171 | sto_iade_ufrsfark_kod | Nvarchar(40) | İade Ufrs Fark Muh. Kodu |  |
| 172 | sto_yurticisat_ufrsfark_kod | Nvarchar(40) | Yurt İçi Satış Ufrs Fark Muh. Kodu |  |
| 173 | sto_satiade_ufrsfark_kod | Nvarchar(40) | Satış İade Ufrs Fark Muh. Kodu |  |
| 174 | sto_satisk_ufrsfark_kod | Nvarchar(40) | Satış İskonto Ufrs Fark Muh. Kodu |  |
| 175 | sto_alisk_ufrsfark_kod | Nvarchar(40) | Alış İskonto Ufrs Fark Muh. Kodu |  |
| 176 | sto_satmal_ufrsfark_kod | Nvarchar(40) | Satış Maliyeti Ufrs Fark Muh. Kodu |  |
| 177 | sto_yurtdisisat_ufrsfark_kod | Nvarchar(40) | Yurt Dışı Satış Ufrs Fark Muh. Kodu |  |
| 178 | sto_ilavemas_ufrsfark_kod | Nvarchar(40) | İlave Masraflar Ufrs Fark Muh. Kodu |  |
| 179 | sto_yatirimtes_ufrsfark_kod | Nvarchar(40) | Yatırım Teşvik Ufrs Fark Muh. Kodu |  |
| 180 | sto_depsat_ufrsfark_kod | Nvarchar(40) | Depolar Arası Satış Ufrs Fark Muh. Kodu |  |
| 181 | sto_depsatmal_ufrsfark_kod | Nvarchar(40) | Depolar Arası Satış Maliyeti Ufrs Fark Muh. Kodu |  |
| 182 | sto_bagortsat_ufrsfark_kod | Nvarchar(40) | Bağlı Ortaklıklara Satış Ufrs Fark Muh. Kodu |  |
| 183 | sto_bagortsatiade_ufrsfark_kod | Nvarchar(40) | Bağlı Ortaklıklara Satış İade Ufrs Fark Muh. Kodu |  |
| 184 | sto_bagortsatisk_ufrsfark_kod | Nvarchar(40) | Bağlı Ortaklıklara Satış İskonto Ufrs Fark Muh. Kodu |  |
| 185 | sto_satfiyfark_ufrsfark_kod | Nvarchar(40) | Satış Fiyat Farkı Ufrs Fark Muh. Kodu |  |
| 186 | sto_yurtdisisatmal_ufrsfark_kod | Nvarchar(40) | Yurt Dışı Satış Maliyeti Ufrs Fark Muh. Kodu |  |
| 187 | sto_bagortsatmal_ufrsfark_kod | Nvarchar(40) | Bağlı Ortaklıklara Satış Maliyeti Ufrs Fark Muh. Kodu |  |
| 188 | sto_sifirbedsatmal_ufrsfark_kod | Nvarchar(40) | Sıfır Bedelli Satış Maliyeti Ufrs Fark Muh. Kodu |  |
| 189 | sto_uretimmaliyet_ufrsfark_kod | Nvarchar(40) | Üretim Maliyeti Ufrs Fark Muh. Kodu |  |
| 190 | sto_uretimkapasite_ufrsfark_kod | Nvarchar(40) | Üretim Kapasite Ufrs Fark Muh. Kodu |  |
| 191 | sto_degerdusuklugu_ufrsfark_kod | Nvarchar(40) | Değer Düşüklüğü Ufrs Fark Muh. Kodu |  |
| 192 | sto_halrusumyudesi | Float | Hal Rüsum Yüzdesi |  |
| 193 | sto_webe_gonderilecek_fl | Bit | Webe Gönderilecek Mi ? |  |
| 194 | sto_min_stok_belirleme_gun | Smallint | Minimum Seviye Belirleme Operasyonu İçin Gün Bilgisi |  |
| 195 | sto_sip_stok_belirleme_gun | Smallint | Sipariş Seviye Belirleme Operasyonu İçin Gün Bilgisi |  |
| 196 | sto_max_stok_belirleme_gun | Smallint | Maksimum Seviye Belirleme Operasyonu İçin Gün Bilgisi |  |
| 197 | sto_sev_bel_opr_degerlendime_fl | Bit | Seviye Belirleme Operasyonu Değerlendirmesi Yapılacak Mı ? |  |
| 198 | sto_otv_tevkifat_turu | Tinyint | Ötv Tevkifat Türü | 0:Tevkifat Yok 1:Tevkifat Tam |
| 199 | sto_kay_plan_degerlendir | Tinyint | Kaynak Planlama Operasyonunda Değerlendirilecek Mi ? | 0:Evet 1:Hayır |
| 200 | sto_CRM_sistemine_aktar_fl | Bit | CRM Sistemine Aktarılsın Mı ? |  |
| 201 | sto_plu_no | Integer Identity | Plu No |  |
| 202 | sto_yerli_yabanci_fl | Tinyint | Barkod Menşei Tipi Yerli Mi Yabancı Mı ? | 0:Yerli 1:Yabancı |
| 203 | sto_mensei | Nvarchar(30) | Menşei |  |
| 204 | sto_oto_parti_lot_kod_fl | Bit | Parti-Lot Kodları Otomatik Oluşturulsun Mu ? |  |
| 205 | sto_efat_sinif_kodu | Nvarchar(20) | e-Fatura Sınıf Kodu |  |
| 206 | sto_efat_sinif_listesi | Nvarchar(15) | e-Fatura Sınıf Listesi |  |
| 207 | sto_efat_sinif_versiyonu | Nvarchar(15) | e-Fatura Sınıf Versiyonu |  |
| 208 | sto_utssisteminegonderilsin_fl | Bit | Ürün Takip Sistemi (ÜTS)'ne Gönderilsin Mi ? |  |
| 209 | sto_posetbeyannamekonusu_fl | Bit | Poşet Beyanname Konusu |  |
| 210 | sto_STT_oncesi_kaldirma | Smallint | Son Tüketim Tarihi Öncesi Kaldırma |  |
| 211 | sto_toplam_rafomru | Smallint | Toplam Raf Ömrü |  |
| 212 | sto_fiyat_kasada_belirlenir_fl | Bit | Fiyat Kasada Belirlensin Mi ? |  |
| 213 | sto_franchise_siparis_dursun | Tinyint | Franchise Sipariş Dursun  Mu ? | 0:Durmasın 1:Dursun |
| 214 | sto_GEKAP | Nvarchar(5) | GEKAP Kodu (Boş=Poşet) |  |
| 215 | sto_GEKAP_birim | Tinyint | GEKAP Birim |  |
| 216 | sto_resim_url | Nvarchar(127) | Resim URL'si |  |
| 217 | sto_GEKAP_depozitoonaykodu | Nvarchar(10) | GEKAP Depozito Onay Kodu |  |
| 218 | sto_cabuk_bozulabilen_urun_fl | Bit | Çabuk Bozulabilen Ürün Mü ? |  |
| 219 | sto_satin_alma_talep_birim | Tinyint | Satın Alma Talep Birimi |  |
| 220 | sto_bagimsiz_miktar_takip_fl | Bit | Bağımsız Miktar Takibi Var Mı ? |  |
| 221 | sto_varyant_detayli_fl1 | Bit | Varyant Detaylı Mı ? |  |
| 222 | sto_varyant_detayli_fl2 | Bit | Varyant Detaylı Mı ? |  |
| 223 | sto_varyant_detayli_fl3 | Bit | Varyant Detaylı Mı ? |  |
| 224 | sto_varyant_detayli_fl4 | Bit | Varyant Detaylı Mı ? |  |
| 225 | sto_varyant_detayli_fl5 | Bit | Varyant Detaylı Mı ? |  |
| 226 | sto_varyant_kod_arr1 | Nvarchar(25) | Varyant Kodu |  |
| 227 | sto_varyant_kod_arr2 | Nvarchar(25) | Varyant Kodu |  |
| 228 | sto_varyant_kod_arr3 | Nvarchar(25) | Varyant Kodu |  |
| 229 | sto_varyant_kod_arr4 | Nvarchar(25) | Varyant Kodu |  |
| 230 | sto_varyant_kod_arr5 | Nvarchar(25) | Varyant Kodu |  |
| 231 | sto_urun_niteligi | Tinyint | Ürün Niteliği | 0:Diğer 1:Telefon 2:Tablet |
| 232 | sto_kuresel_urun_numarasi | Nvarchar(50) | Küresel Ürün Numarası |  |


Güncellenme Tarihi : 24.04.2025 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**