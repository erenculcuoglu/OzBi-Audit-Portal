# TABLO NO: 71

## Tablo Adı: PERSONELLER - Personel Kartları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | per_Guid | Uniqueidentifier |  |  |
| 1 | per_DBCno | Smallint |  |  |
| 2 | per_SpecRECno | Integer |  |  |
| 3 | per_iptal | Bit |  |  |
| 4 | per_fileid | Smallint |  |  |
| 5 | per_hidden | Bit |  |  |
| 6 | per_kilitli | Bit |  |  |
| 7 | per_degisti | Bit |  |  |
| 8 | per_checksum | Integer |  |  |
| 9 | per_create_user | Smallint |  |  |
| 10 | per_create_date | DateTime |  |  |
| 11 | per_lastup_user | Smallint |  |  |
| 12 | per_lastup_date | DateTime |  |  |
| 13 | per_special1 | Nvarchar(127) |  |  |
| 14 | per_special2 | Nvarchar(127) |  |  |
| 15 | per_special3 | Nvarchar(127) |  |  |
| 16 | per_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | per_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | per_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | per_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | per_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | per_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | per_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | per_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | per_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | per_kod | Nvarchar(25) | Personel Kodu |  |
| 26 | per_adi | Nvarchar(50) | Adı |  |
| 27 | per_soyadi | Nvarchar(50) | Soyadı |  |
| 28 | per_orjdildeadisoyadi | Nvarchar(80) | Orjinal Dilde Adı-Soyadı |  |
| 29 | per_sicil_no | Nvarchar(25) | SSK Sicil No |  |
| 30 | per_firma_no | Integer | Firma No |  |
| 31 | per_sube_no | Integer | Şube No |  |
| 32 | per_caripers_kodu | Nvarchar(25) | Cari Personel Kodu |  |
| 33 | per_tip | Tinyint | Personel Tipi | 0:Satıcı Eleman 1:Satın Almacı   2:Diğer Eleman |
| 34 | per_dept_kod | Nvarchar(25) | Departman Kodu | Bkz. DEPARTMANLAR |
| 35 | per_is_grup | Tinyint | İş Grubu | 0:Memur 1:İşci 2:16 Yaşından Küçük   3:Yönetim Kurulu 4:Sanatçı 5:ARGE 6:TUGS (Türk Uluslararası Gemi Sicili) 7:Kapıcı 8:Uçuş Personeli 9:Dalış Personeli 10:Amatör Sporcu 11:Profesyonel Sporcu |
| 36 | per_giris_tar | DateTime | İşe Giriş Tarihi |  |
| 37 | per_cikis_tar | DateTime | İşten Ayrılma Tarihi |  |
| 38 | per_cikis_neden | Nvarchar(40) | Çıkış Nedeni |  |
| 39 | per_muh_kod | Nvarchar(40) | Personel Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 40 | per_kim_tahsil | Tinyint | Tahsil | 0:Yok 1:İlk 2:Orta 3:Lise 4:Yüksek 5:Fakülte 6:Yüksek Lisans 7:Doktora 8:Fakülte Temel Bilgi 9:Yüksek Lisans Temel Bilgi 10:Okul Öncesi |
| 41 | per_kim_meslek | Nvarchar(20) | Mesleği |  |
| 42 | per_kim_gorev | Nvarchar(25) | Görevi |  |
| 43 | per_kim_sakat_derece | Tinyint | Sakatlık Derecesi | 0:Sağlam 1:Birinci Derece 2:İkinci Derece 3:Üçüncü Derece 4:Vergi İndirimi Dışı |
| 44 | per_kim_gocmen | Tinyint | Göçmen Kodu | 0:Değil 1:Göçmen |
| 45 | per_kim_gorev_kod | Tinyint | Görev Kodu | 0:İşveren Vekili 1:Diğer İdari Görevliler 2:Güvenlik Görevlileri   3:Diğerleri |
| 46 | per_kim_SGK_kod | Tinyint | Sosyal Güvenlik Kodu | 0:SSK 1:Emekli Sandığı   2:Banka ve Diğer 3:Bağ kur |
| 47 | per_kim_cocuk | Tinyint | Çocuk Sayısı |  |
| 48 | per_kim_okuloncesi | Tinyint | Okul Öncesi |  |
| 49 | per_kim_ilkokul | Tinyint | İlk Okul |  |
| 50 | per_kim_ortaokul | Tinyint | Orta Okul |  |
| 51 | per_kim_lise | Tinyint | Lise |  |
| 52 | per_kim_yuksek | Tinyint | Yüksek Okul |  |
| 53 | per_nuf_uyruk | Nvarchar(15) | Uyruğu |  |
| 54 | per_nuf_cinsiyet | Tinyint | Cinsiyet | 0:Erkek 1:Kadın |
| 55 | per_nuf_medeni_hal | Tinyint | Medeni Hali | 0:Evli 1:Bekar 2:Dul |
| 56 | per_nuf_din | Nvarchar(15) | Dini |  |
| 57 | per_nuf_dogum_tarih | DateTime | Doğum Tarihi |  |
| 58 | per_nuf_dogum_yer | Nvarchar(40) | Doğum Yeri |  |
| 59 | per_nuf_kangrup | Tinyint | Kan Grubu | 0:ARh+ 1:ARh- 2:BRh+ 3:BRh- 4:ABRh+ 5:ABRh- 6:0Rh+ 7:0Rh- 8:Tanımsız |
| 60 | per_nuf_seri_no | Nvarchar(15) | Nüfüs Cüzdanı Seri No |  |
| 61 | per_nuf_il | Nvarchar(20) | İl |  |
| 62 | per_nuf_ilce | Nvarchar(20) | İlçe |  |
| 63 | per_nuf_mahalle | Nvarchar(20) | Mahalle |  |
| 64 | per_nuf_koy | Nvarchar(20) | Köy |  |
| 65 | per_nuf_ciltno | Nvarchar(10) | Cilt No |  |
| 66 | per_nuf_sayfano | Nvarchar(10) | Sayfa No |  |
| 67 | per_nuf_kutukno | Nvarchar(10) | Kütük No |  |
| 68 | per_nuf_ver_neden | Nvarchar(20) | Veriliş Nedeni |  |
| 69 | per_nuf_ver_yer | Nvarchar(20) | Verildiği Yer |  |
| 70 | per_nuf_ver_tarih | DateTime | Veriliş Tarihi |  |
| 71 | per_nuf_cuz_kayitno | Nvarchar(15) | Cüzdan Kayıt No |  |
| 72 | per_ucr_tip | Tinyint | Ücret Tipi | 0:Aylık 1:Günlük 2:Saatlik |
| 73 | per_ucret | Float | Ücret Tutarı |  |
| 74 | per_Brut_net | Tinyint | Brüt? Net? | 0:Brüt 1:Net |
| 75 | per_ucr_send_durum | Tinyint | Ücret Sendika Durumu | 0:Sendikasız 1:Sendikalı 2:Dayanışmalı |
| 76 | per_ucr_send | Tinyint | Ücret Sendikası No |  |
| 77 | per_ucr_PSSK_sube | Tinyint | SSK Şubesi No |  |
| 78 | per_ucr_hesapno | Nvarchar(30) | Banka Hesap No |  |
| 79 | per_ucr_sig_yuzde_gr | Tinyint | Sigorta Yüzde Grubu | 0:Normal 1:Emekli 2:Çırak   3:Yabancı 4:Öğrenci |
| 80 | per_ucr_bode_yapilma | Tinyint | Bankadan Ödeme Yapılsın Mı? | 0:Yapılmasın 1:Yapılsın |
| 81 | per_ucr_vdaire | Nvarchar(14) | Vergi Dairesi |  |
| 82 | per_ucr_vkarneno | Nvarchar(12) | Vergi Karne No |  |
| 83 | per_ucr_vkarne_tarih | DateTime | Vergi Karne Tarihi |  |
| 84 | per_ucr_konutfon | Tinyint | Konut Fonu | 0:Yok 1:Var |
| 85 | per_ucr_onceod | Smallint | Önce Ödm. KF. Ayı |  |
| 86 | per_ozelavansorani | Float | Özel Avans Oranı |  |
| 87 | per_sgk_deger_kodu | Nvarchar(25) | SGK Değer Kodu |  |
| 88 | per_yard_yol | Tinyint | Yol Yardımı | 0:Yok 1:Var |
| 89 | per_yard_yemek | Tinyint | Yemek Yardımı | 0:Yok 1:Var |
| 90 | per_yard_yakacak | Tinyint | Yakacak Yardımı | 0:Yok 1:Var |
| 91 | per_yard_bayram | Tinyint | Bayram Yardımı | 0:Yok 1:Var |
| 92 | per_yard_cocuk | Tinyint | Cocuk Yardımı | 0:Yok 1:Var |
| 93 | per_yard_aile | Tinyint | Aile Yardımı | 0:Yok 1:Var |
| 94 | per_yard_ozelindirim | Tinyint | Özel İndirim Yardımı | 0:Yok 1:Var |
| 95 | per_adr_cadde | Nvarchar(50) | Adres (Cadde) |  |
| 96 | per_adr_mahalle | Nvarchar(50) | Mahalle |  |
| 97 | per_adr_sokak | Nvarchar(50) | Adres (Sokak) |  |
| 98 | per_adr_semt | Nvarchar(25) | Semt |  |
| 99 | per_adr_apartman_no | Nvarchar(10) | Apartman No |  |
| 100 | per_adr_daire_no | Nvarchar(10) | Daire No |  |
| 101 | per_adr_posta_kod | Nvarchar(8) | Posta Kodu |  |
| 102 | per_adr_ilce | Nvarchar(50) | İlçe |  |
| 103 | per_adr_il | Nvarchar(50) | İl |  |
| 104 | per_adr_ulke | Nvarchar(50) | Ülke |  |
| 105 | per_adr_adres_kodu | Nvarchar(10) | Adres Kodu |  |
| 106 | per_tel_ulke_kod | Nvarchar(5) | Ülke Telefon Kodu |  |
| 107 | per_tel_bolge_kod | Nvarchar(5) | Bölge Telefon Kodu |  |
| 108 | per_tel_no1 | Nvarchar(10) | Telefon No 1 |  |
| 109 | per_tel_no2 | Nvarchar(10) | Telefon No 2 |  |
| 110 | per_tel_faxno | Nvarchar(10) | Fax No |  |
| 111 | per_tel_cepno | Nvarchar(10) | Cep Telefon No |  |
| 112 | per_doviz_cinsi | Tinyint | Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 113 | per_muh_grpkod | Nvarchar(25) | Muhasebe Grup Kodu | Bkz. PERSONEL_MUHASEBE_GRUPLARI |
| 114 | per_muh_ozelc1 | Nvarchar(25) | Sendika Aidatı Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 115 | per_muh_ozelc2 | Nvarchar(25) | Borç Taksidi Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 116 | per_muh_ozelc3 | Nvarchar(25) | İcra Taksidi Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 117 | per_muh_ozelc4 | Nvarchar(25) | Kredi Taksidi Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 118 | per_muh_ozelc5 | Nvarchar(25) | Avans Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 119 | per_muh_ozelc6 | Nvarchar(25) | İzin Avansı Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 120 | per_muh_ozelc7 | Nvarchar(25) | İkramiye Avansı Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 121 | per_muh_ozelc8 | Nvarchar(25) | Yakacak Avansı Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 122 | per_muh_ozelc9 | Nvarchar(25) | Bayram Avansı Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 123 | per_muh_ozelc10 | Nvarchar(25) | Yardım Sandığı Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 124 | per_muh_ozelc11 | Nvarchar(25) | Fon Kesintisi Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 125 | per_muh_ozelc12 | Nvarchar(25) | Ceza Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 126 | per_muh_ozelc13 | Nvarchar(25) | Geçen Yuvarlama Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 127 | per_muh_ozelc14 | Nvarchar(25) | Ters Bakiye Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 128 | per_muh_ozelc15 | Nvarchar(25) | Diğer 1 Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 129 | per_muh_ozelc16 | Nvarchar(25) | Diğer 2 Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 130 | per_muh_ozelc17 | Nvarchar(25) | Diğer 3 Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 131 | per_muh_ozelc18 | Nvarchar(25) | Askerlik Borçlanması Muh. Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 132 | per_muh_ozelc19 | Nvarchar(25) | Özel Kesintiler |  |
| 133 | per_muh_ozelc20 | Nvarchar(25) | Özel Kesintiler |  |
| 134 | per_muh_ozelc21 | Nvarchar(25) | Özel Kesintiler |  |
| 135 | per_muh_ozelc22 | Nvarchar(25) | Özel Kesintiler |  |
| 136 | per_muh_ozelc23 | Nvarchar(25) | Özel Kesintiler |  |
| 137 | per_muh_ozelc24 | Nvarchar(25) | Özel Kesintiler |  |
| 138 | per_old_ucret | Float | Eski Ücret |  |
| 139 | per_old_tarih | DateTime | Eski Ücret Tarihi |  |
| 140 | per_maas_ikramiye | Tinyint | İkramiye? | 0:Yok 1:Var |
| 141 | per_ozel_not | Nvarchar(12) | Özel Not |  |
| 142 | per_VkfKesOd_fl | Bit | Vakıf Kesintisi Uygula? | 0:Hayır 1:Evet |
| 143 | per_Kiper_Tarih | DateTime | Kıdem Tarihi |  |
| 144 | per_iszlksig | Tinyint | İşsizlik Sigortası? | 0:Yok 1:Var |
| 145 | per_Calismatipi | Tinyint | Çalışma Tipi | 0:Genel 1:İmalat 2:Pazarlama |
| 146 | per_dil1 | Bit | Dil1 | Türkçe |
| 147 | per_dil2 | Bit | Dil2 | İngilizce |
| 148 | per_dil3 | Bit | Dil3 | Almanca |
| 149 | per_dil4 | Bit | Dil4 | Fransızca |
| 150 | per_dil5 | Bit | Dil5 | Italyanca |
| 151 | per_dil6 | Bit | Dil6 | İspanyolca |
| 152 | per_dil7 | Bit | Dil7 | Rusça |
| 153 | per_dil8 | Bit | Dil8 | Arapça |
| 154 | per_dil9 | Bit | Dil9 | Diğer 1 |
| 155 | per_dil10 | Bit | Dil10 | Diğer 2 |
| 156 | per_dil11 | Bit | Dil11 | Diğer 3 |
| 157 | per_dil12 | Bit | Dil12 | Diğer 4 |
| 158 | per_mevsim | Tinyint | Personel Çalışma Mevsimi | 0:Sürekli 1:Süreksiz 2:Part Time |
| 159 | per_kapsam | Tinyint | Personel Çalışma Kapsamı | 0:Normal 1:Eski Hükümlü 2:Terörle Mücadele |
| 160 | per_asgari_ucretli | Bit | Personel Asgari Ücretli mi? | 0:Hayır 1:Evet |
| 161 | Per_PerCariCins1 | Tinyint | Personel Cari Cinsi | 0:Gider Cins 1:Personel Cins 2:Diğer Cari Cins 3:Cari Personel Cins 4:Muhasebe Hesabı Cinsi |
| 162 | Per_PerCariCins2 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 163 | Per_PerCariCins3 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 164 | Per_PerCariCins4 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 165 | Per_PerCariCins5 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 166 | Per_PerCariCins6 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 167 | Per_PerCariCins7 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 168 | Per_PerCariCins8 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 169 | Per_PerCariCins9 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 170 | Per_PerCariCins10 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 171 | Per_PerCariCins11 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 172 | Per_PerCariCins12 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 173 | Per_PerCariCins13 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 174 | Per_PerCariCins14 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 175 | Per_PerCariCins15 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 176 | Per_PerCariCins16 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 177 | Per_PerCariCins17 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 178 | Per_PerCariCins18 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 179 | Per_PerCariCins19 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 180 | Per_PerCariCins20 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 181 | Per_PerCariCins21 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 182 | Per_PerCariCins22 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 183 | Per_PerCariCins23 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 184 | Per_PerCariCins24 | Tinyint | Personel Cari Cinsi | Yukardakiyle aynı |
| 185 | Per_PerCariCins_grupno1 | Tinyint | Personel Cari Cinsi Grup No |  |
| 186 | Per_PerCariCins_grupno2 | Tinyint | Personel Cari Cinsi Grup No |  |
| 187 | Per_PerCariCins_grupno3 | Tinyint | Personel Cari Cinsi Grup No |  |
| 188 | Per_PerCariCins_grupno4 | Tinyint | Personel Cari Cinsi Grup No |  |
| 189 | Per_PerCariCins_grupno5 | Tinyint | Personel Cari Cinsi Grup No |  |
| 190 | Per_PerCariCins_grupno6 | Tinyint | Personel Cari Cinsi Grup No |  |
| 191 | Per_PerCariCins_grupno7 | Tinyint | Personel Cari Cinsi Grup No |  |
| 192 | Per_PerCariCins_grupno8 | Tinyint | Personel Cari Cinsi Grup No |  |
| 193 | Per_PerCariCins_grupno9 | Tinyint | Personel Cari Cinsi Grup No |  |
| 194 | Per_PerCariCins_grupno10 | Tinyint | Personel Cari Cinsi Grup No |  |
| 195 | Per_PerCariCins_grupno11 | Tinyint | Personel Cari Cinsi Grup No |  |
| 196 | Per_PerCariCins_grupno12 | Tinyint | Personel Cari Cinsi Grup No |  |
| 197 | Per_PerCariCins_grupno13 | Tinyint | Personel Cari Cinsi Grup No |  |
| 198 | Per_PerCariCins_grupno14 | Tinyint | Personel Cari Cinsi Grup No |  |
| 199 | Per_PerCariCins_grupno15 | Tinyint | Personel Cari Cinsi Grup No |  |
| 200 | Per_PerCariCins_grupno16 | Tinyint | Personel Cari Cinsi Grup No |  |
| 201 | Per_PerCariCins_grupno17 | Tinyint | Personel Cari Cinsi Grup No |  |
| 202 | Per_PerCariCins_grupno18 | Tinyint | Personel Cari Cinsi Grup No |  |
| 203 | Per_PerCariCins_grupno19 | Tinyint | Personel Cari Cinsi Grup No |  |
| 204 | Per_PerCariCins_grupno20 | Tinyint | Personel Cari Cinsi Grup No |  |
| 205 | Per_PerCariCins_grupno21 | Tinyint | Personel Cari Cinsi Grup No |  |
| 206 | Per_PerCariCins_grupno22 | Tinyint | Personel Cari Cinsi Grup No |  |
| 207 | Per_PerCariCins_grupno23 | Tinyint | Personel Cari Cinsi Grup No |  |
| 208 | Per_PerCariCins_grupno24 | Tinyint | Personel Cari Cinsi Grup No |  |
| 209 | Per_TCKimlikNo | Nvarchar(11) | TC Kimlik No |  |
| 210 | Per_PersMailAddress | Nvarchar(50) | e-Posta Adresi |  |
| 211 | Per_Aylik_calisma_saati | Float | Aylık Çalışma Saati |  |
| 212 | Per_Muh_Grup_Kodu | Nvarchar(25) | Muhasebe Grup Kodu |  |
| 213 | per_bolge_kodu | Nvarchar(25) | Bölge Kodu |  |
| 214 | per_okul_ad | Nvarchar(40) | Okul Adı |  |
| 215 | per_IdariAmirKodu | Nvarchar(25) | İdari Amir Kodu |  |
| 216 | per_TeknikAmirKodu | Nvarchar(25) | Teknik Amir Kodu |  |
| 217 | per_CikisSebebiSecimli | Tinyint | İşten Çıkış Sebebi | 0:Sebep yok 1:Deneme süreli işverence feshi 2:Deneme süreli sigortalı feshi 3:Belirsiz süreli sigortalı feshi 4:Belirsiz süreli işverence feshi 5:Belirli süreli sözleşme sonu 6:Haklı sigortalı feshi 7:Haklı işverence feshi 8:Emeklilik 9:Malulen emeklilik 10:Ölüm 11:İş kazası ölüm 12:Askerlik 13:Kadın evlilik 14:Yaş dışında emekliliği hakediş 15:Toplu işci çıkarma 16:Nakil 17:İş yerinin kapanması 18:İşin sona ermesi 19:Mevsim bitimi 20:Kampanya bitimi 21:Statü değişimi 22:Diğer çıkış 23:İşci tarafından zorunlu nedenle fesih 24:İşci tarafından sağlık nedeniyle fesih 25:İşci tarafından işverenin ahlak nedeni ile fesih 26:Disiplin kurulu kararı ile fesih 27:İş veren tarafından zorunlu nedenlerle fesih 28:İş veren tarafından sağlık nedeni ile fesih 29:İş veren tarafından işcinin ahlak nedeni ile fesih 30:Vize süresinin bitimi 31:Borçlar kanunu 32:Kanun 4046 nedeni ile feshi 33:Gazeteci tarafından sözleşmenin feshi 34:İş yerinin devri 35:6495 sayılı kanun nedeniyle devlet memurluğuna geçenler 36:KHK (Kanun Hükmünde Kararname) ile iş yerinin kapatılması 37:KHK ile kamudan çıkarma 38:Doğum nedeniyle işten ayrılma 39:KHK 696 ile kamuya geçiş 40:KHK 696 ile kamuya geçilememe 41:Resen işten ayrılış bildirgesi 42:Gerçeğe uygun olmayan bilgilerle yanıltma 43:Şeref ve namusa dönük davranışlar 44:Cinsel taciz 45:Sataşma, sarhoşluk, uyuşturucu 46:Güven sorunu, hırsızlık 47:Hapisle sonuçlanan suç işleme 48:Habersiz işe gelmeme 49:Görevini yapmamakta ısrar 50:Güvenlik tehlikesi, hasar kayıp oluşturma |
| 218 | per_ilksoyad | Nvarchar(25) | İlk Soyadı |  |
| 219 | per_tabiioldugukanun | Tinyint | Kanun Tipi | 0:5510 Sayılı SGK Kanuna Tabi 1:Hazine Kanununa (5084) Tabi (%80) 2:Hazine Kanununa (5084) Tabi (%100) 3:Olağan Üstü Hal Kanununa Tabi 4:Sendika İndirimi Kanununa Tabi 5:Sakatlık İndirimi Kanununa Tabi 6:Borç Erteleme Kanununa Tabi 7:Sendikalı Borç Erteleme Kanununa Tabi 8:Sakatlık -Eski Hükümlü- Terör Kanununa Tabi 9:Hazine Kanununa (5350) Tabi (%80) 10:Hazine Kanununa (5350) Tabi (%100) 11:Hazine Kanununa (5615) Tabi (%80) 12:Hazine Kanununa (5615) Tabi (%100) 13:5746 Sayılı ARGE Faaliyetlerinin Desteklenmesi Kanununa Tabi 14:5763 Sayılı İş Kanununa Tabi (Sağlam) (%100) 15:5763 Sayılı İş Kanununa Tabi (Kontenjan Dahili Özürlü) 16:5763 Sayılı İş Kanununa Tabi (Kontenjan Harici Özürlü) 17:Hiçbir Kanuna Tabi Değil 18:5763 Sayılı İş Kanununa Tabi (Sağlam) (%80) 19:5763 Sayılı İş Kanununa Tabi (Sağlam) (%60) 20:5763 Sayılı İş Kanununa Tabi (Sağlam) (%40) 21:5763 Sayılı İş Kanununa Tabi (Sağlam) (%20) 22:5921 Sayılı SGK Kanununa Tabi 23:6111 Sayılı SGK Kanununa Tabi 24:25510 Sayılı SGK Kanununa Tabi 25:6322 Sayılı SGK Kanununa Tabi (1..5 inci bölge) 26:6322 Sayılı SGK Kanununa Tabi (6. bölge) 27:5225 Sayılı SGK Kültür Yatırımı ve Girişimi Kanununa Tabi (Yatırım aşaması) 28:5225 Sayılı SGK Kültür Yatırımı ve Girişimi Kanununa Tabi (İşletme aşaması) 29:06486 Sayılı SGK Kanuna Tabi 30:46486 Sayılı SGK Kanuna Tabi 31:56486 Sayılı SGK Kanuna Tabi 32:66486 Sayılı SGK Kanuna Tabi 33:6645 Sayılı SGK Kanuna Tabi 34:687 Sayılı İstihdam Teşvik Kanuna Tabi 35:1687 Sayılı İstihdam Teşvik Kanuna Tabi 36:Yeni Nesil 7103 Sayılı Kanundaki İmalat veya Bilişim 37:Yeni Nesil 7103 Sayılı Kanundaki Diğer 38:Yeni Nesil 7103 Sayılı Kanundaki Bir Senden Bir Benden 39:7166 Sayılı SGK Teşvikleri İmalat ve Bilişim 40:7166 Sayılı SGK Teşvikleri Diğer 41:2828 Sayılı SGK Kanununa Tabi 42:17256 Sayılı Teşvik Kanununa Tabi 43:27256 Sayılı Teşvik Kanununa Tabi 44:3294 Sayılı Sosyal Yardımlaşma ve Dayanışmayı Teşvik Kanununa Tabi 45:7316 Sayılı Kanuna Tabi 46:7319 Sayılı SGK Kanununa Tabi 47:36322 Sayılı Teşvik Kanununa Tabi |
| 220 | per_tabiioldugukanun_diger | Tinyint | Kanun Tipi | 0:5510 Sayılı SGK Kanuna Tabi 1:Hazine Kanununa (5084) Tabi (%80) 2:Hazine Kanununa (5084) Tabi (%100) 3:Olağan Üstü Hal Kanununa Tabi 4:Sendika İndirimi Kanununa Tabi 5:Sakatlık İndirimi Kanununa Tabi 6:Borç Erteleme Kanununa Tabi 7:Sendikalı Borç Erteleme Kanununa Tabi 8:Sakatlık -Eski Hükümlü- Terör Kanununa Tabi 9:Hazine Kanununa (5350) Tabi (%80) 10:Hazine Kanununa (5350) Tabi (%100) 11:Hazine Kanununa (5615) Tabi (%80) 12:Hazine Kanununa (5615) Tabi (%100) 13:5746 Sayılı ARGE Faaliyetlerinin Desteklenmesi Kanununa Tabi 14:5763 Sayılı İş Kanununa Tabi (Sağlam) (%100) 15:5763 Sayılı İş Kanununa Tabi (Kontenjan Dahili Özürlü) 16:5763 Sayılı İş Kanununa Tabi (Kontenjan Harici Özürlü) 17:Hiçbir Kanuna Tabi Değil 18:5763 Sayılı İş Kanununa Tabi (Sağlam) (%80) 19:5763 Sayılı İş Kanununa Tabi (Sağlam) (%60) 20:5763 Sayılı İş Kanununa Tabi (Sağlam) (%40) 21:5763 Sayılı İş Kanununa Tabi (Sağlam) (%20) 22:5921 Sayılı SGK Kanununa Tabi 23:6111 Sayılı SGK Kanununa Tabi 24:25510 Sayılı SGK Kanununa Tabi 25:6322 Sayılı SGK Kanununa Tabi (1..5 inci bölge) 26:6322 Sayılı SGK Kanununa Tabi (6. bölge) 27:5225 Sayılı SGK Kültür Yatırımı ve Girişimi Kanununa Tabi (Yatırım aşaması) 28:5225 Sayılı SGK Kültür Yatırımı ve Girişimi Kanununa Tabi (İşletme aşaması) 29:06486 Sayılı SGK Kanuna Tabi 30:46486 Sayılı SGK Kanuna Tabi 31:56486 Sayılı SGK Kanuna Tabi 32:66486 Sayılı SGK Kanuna Tabi 33:6645 Sayılı SGK Kanuna Tabi 34:687 Sayılı İstihdam Teşvik Kanuna Tabi 35:1687 Sayılı İstihdam Teşvik Kanuna Tabi 36:Yeni Nesil 7103 Sayılı Kanundaki İmalat veya Bilişim 37:Yeni Nesil 7103 Sayılı Kanundaki Diğer 38:Yeni Nesil 7103 Sayılı Kanundaki Bir Senden Bir Benden 39:7166 Sayılı SGK Teşvikleri İmalat ve Bilişim 40:7166 Sayılı SGK Teşvikleri Diğer 41:2828 Sayılı SGK Kanununa Tabi 42:17256 Sayılı Teşvik Kanununa Tabi 43:27256 Sayılı Teşvik Kanununa Tabi 44:3294 Sayılı Sosyal Yardımlaşma ve Dayanışmayı Teşvik Kanununa Tabi 45:7316 Sayılı Kanuna Tabi 46:7319 Sayılı SGK Kanununa Tabi 47:36322 Sayılı Teşvik Kanununa Tabi |
| 221 | per_semada_gosterme_fl | Bit | Şemada gösterilsin mi ? |  |
| 222 | per_Ehl_Bel_No | Nvarchar(20) | Ehliyet Belge No |  |
| 223 | per_Ehl_Bel_Tar | DateTime | Ehliyet Belge Tarihi |  |
| 224 | per_Ehl_Sinif | Nvarchar(10) | Ehliyet Sınıfı |  |
| 225 | per_Ehl_Ver_Tar | DateTime | Ehliyet Veriliş Tarihi |  |
| 226 | per_Ehl_Ver_Il | Nvarchar(25) | Ehliyet Verilen İl |  |
| 227 | per_Ehl_Ver_Ilce | Nvarchar(25) | Ehliyet Verilen İlçe |  |
| 228 | per_Ehl_Kart_No | Nvarchar(20) | Ehliyet Kart No |  |
| 229 | per_Pasaprot_No | Nvarchar(20) | Pasaport No |  |
| 230 | per_Pas_Alindigi_Tar | DateTime | Pasaport Alındığı Tarih |  |
| 231 | per_Pas_Gec_Tar | DateTime | Pasaport Geçerlilik Tarihi |  |
| 232 | per_nuf_asker_cuzdan | Nvarchar(20) | Askerlik Cüzdanı No |  |
| 233 | per_nuf_asker_bastarih | DateTime | Askerlik Başlangıç Tarihi |  |
| 234 | per_nuf_asker_bittarih | DateTime | Askerlik Bitiş Tarihi |  |
| 235 | per_nuf_asker_durum | Tinyint | Askerlik Durumu | 0:Muaf 1:Tecilli 2:Er 3:Yedek Subay 4:Bedelli 5:Kısa |
| 236 | per_Isy_KimlikNo | Nvarchar(20) | Kimlik No |  |
| 237 | per_calismaizni_no | Nvarchar(20) | Çalışma İzni Belge Numarası |  |
| 238 | per_calismaizni_alindigi_tar | DateTime | Çalışma İzni Başlangıç (Alındığı) Tarihi |  |
| 239 | per_calismaizni_gec_tar | DateTime | Çalışma İzni Bitiş (Geçerlilik) Tarihi |  |
| 240 | per_boyu | Float | Boyu |  |
| 241 | per_kilo | Float | Kilosu |  |
| 242 | per_gomlek_bed | Nvarchar(10) | Gömlek Bedeni |  |
| 243 | per_pant_bed | Nvarchar(10) | Pantolon Bedeni |  |
| 244 | per_etek_bed | Nvarchar(10) | Etek Bedeni |  |
| 245 | per_ayak_no | Nvarchar(10) | Ayak Numarası |  |
| 246 | per_sapka_bed | Nvarchar(10) | Şapka Bedeni |  |
| 247 | per_onluk_bed | Nvarchar(10) | Önlük Bedeni |  |
| 248 | per_diger_bed1 | Nvarchar(10) | Diğer Beden1 |  |
| 249 | per_diger_bed2 | Nvarchar(10) | Diğer Beden2 |  |
| 250 | per_diger_bed3 | Nvarchar(10) | Diğer Beden3 |  |
| 251 | per_UserNo | Integer | Kullanıcı No |  |
| 252 | per_uye_dernek | Nvarchar(50) | Dernek |  |
| 253 | per_uye_dernek_sicil_no | Nvarchar(25) | Üye Dernek Sicil No |  |
| 254 | per_sinyority_uygulamasi_fl | Bit | Sinyority Uygulaması Var Mı ? |  |
| 255 | per_izinparasi_uygulamasi_fl | Bit | İzin Parası Uygulaması Var Mı ? |  |
| 256 | per_YemekKarti_ID | Nvarchar(30) | Yemek Kartı ID |  |
| 257 | per_srmmrkbaglanti_tip | Tinyint | Sorumluluk Merkezi Bağlantı Tipi | 0:Departmandan 1:Girilen Sorumluluk Merkezinden 2:Dağıtım Anahtarından 3:İş Merkezlerinden |
| 258 | per_srmmrkdaganah_kodu | Nvarchar(25) | Sorumluluk Merkezi Dağıtım Anahtar Kodu |  |
| 259 | per_maas_banka | Tinyint | Personel kartına, maaşının hangi bankaya yattığı bilgisi | 0:Tanımsız Banka 1:Yapı Kredi Bankası 2:İş Bankası 3:Halk Bankası 4:Garanti 5:Vakıfbank 6:Akbank 7:Finans Bank 8:Koç Bank 9:TEB 10:Deniz Bank 11:Dubai WPS 12:Kuveyt Türk 13:Odea Bank |
| 260 | per_calisma_kodu | Nvarchar(25) | Çalışma Kodu |  |
| 261 | per_meslek_kodu | Nvarchar(25) | Meslek Kodu |  |
| 262 | per_servis_guzergahi | Nvarchar(60) | Servis Güzergahı |  |
| 263 | per_vize_no | Nvarchar(25) | Vize No |  |
| 264 | per_vize_alindigi_tarih | DateTime | Vize Alındığı Tarih |  |
| 265 | per_vize_tarihi | DateTime | Vize Tarihi |  |
| 266 | per_sskbelge_turu | Tinyint | SSK Belge Türü | 0:Tanımsız Bildirge 1:Aylık sigorta prim bildirgesi 2:Sosyal güvenlik destek prim bildirgesi 3:Deniz Basım Azot Şeker İtibari hizmet bildirgesi 4:Yeraltı sürekli bildirgesi 5:Yeraltı gruplu bildirgesi 6:Yerüstü gruplu bildirge 7:Çırak Stajyer Öğrenci bildirgesi 8:Anlaşmaya tabi olmayan yabancı bildirge 9:YOK kısmi istihdam öğr. bildirgesi 10:Aylık sigorta prim işsizlik hariç bildirgesi 11:Libya bildirgesi 12:Anlaşmalı ülke yabancı uyruk bildirgesi 13:İşsizlik hariç kanun 2098 bildirgesi 14:Malul aylığı kanun 2098 bildirgesi 15:Görev malulluk aylığı alış bildirgesi 16:İş kazası mes. hastalık analık bildirgesi 17:Topluluk bildirgesi 18:Almanyaya götürülen Türk İşciler 19:Sözleşmesi imzalanmamış ülkelerde çalıştırılan Türk İşciler 20:Meslek liselerinde staja tabi tutulan öğrenciler 21:Kısa vadeli harp malulleri 22:Kısa ve uzun vadeli harp malulleri 23:4447 nolu kanuna göre ödenek almayan kursiyer 24:4447 nolu kanuna göre ödenek alan kursiyer 25:4447 nolu kanuna göre ödenek alan katılmayan kursiyer 26:4046 nolu kanuna göre iş kaybı tazminatı alanlar 27:Tüm sigorta kollarına tabi 60 gün 28:İşsizlik sigortası hariç 60 gün 29:Harp malulleri 3713 60 gün 30:Tüm sigorta kollarına tabi 90 gün 31:İşsizlik sigortası hariç 90 gün 32:Harp malulleri 3713 90 gün 33:Tüm sigorta kollarına tabi 180 gün 34:İşsizlik sigortası hariç 180 gün 35:Harp malulleri 3713 180 gün 36:Birleşik krallıkta ikameti talep edenler 37:Birleşik krallıkta ikameti talep etmeyenler 38:Kısa çalışma ödeneği alanlar 39:Geçici 20.maddeye tabi olanlar 40:Kamu idarelerinde iş akdi askıda olanlar 41:İtibari hizmet süresine tabi olarak çalışanlar 42:Altmış gün fiili hizmet süresi zammına tabi olanlardan itibari hizmet süresine tabi 43:Doksan gün fiili hizmet süresi zammına tabi olanlardan itibari hizmet süresine tabi 44:Bakmakla yükümlü olunmayan çıraklar, mesleki öğrenciler 45:Bakmakla yükümlü olunmayan stajyer öğrenciler 46:Bakmakla yükümlü olunmayan İŞKUR kursiyerleri 47:İŞKUR kursiyerleri 48:Yarım çalışma ödeneği 49:Emekli yer altında çalışan 50:Tamamlayıcı bakmakla yükümlü 51:Tamamlayıcı bakmakla yükümlü olmayan 52:Güvenlik korucuları Ek 15 53:Ml. ayl. bağlanmamış 670KHK |
| 267 | per_sskbelge_turu_diger | Tinyint | SSK Belge Türü | 0:Tanımsız Bildirge 1:Aylık sigorta prim bildirgesi 2:Sosyal güvenlik destek prim bildirgesi 3:Deniz Basım Azot Şeker İtibari hizmet bildirgesi 4:Yeraltı sürekli bildirgesi 5:Yeraltı gruplu bildirgesi 6:Yerüstü gruplu bildirge 7:Çırak Stajyer Öğrenci bildirgesi 8:Anlaşmaya tabi olmayan yabancı bildirge 9:YOK kısmi istihdam öğr. bildirgesi 10:Aylık sigorta prim işsizlik hariç bildirgesi 11:Libya bildirgesi 12:Anlaşmalı ülke yabancı uyruk bildirgesi 13:İşsizlik hariç kanun 2098 bildirgesi 14:Malul aylığı kanun 2098 bildirgesi 15:Görev malulluk aylığı alış bildirgesi 16:İş kazası mes. hastalık analık bildirgesi 17:Topluluk bildirgesi 18:Almanyaya götürülen Türk İşciler 19:Sözleşmesi imzalanmamış ülkelerde çalıştırılan Türk İşciler 20:Meslek liselerinde staja tabi tutulan öğrenciler 21:Kısa vadeli harp malulleri 22:Kısa ve uzun vadeli harp malulleri 23:4447 nolu kanuna göre ödenek almayan kursiyer 24:4447 nolu kanuna göre ödenek alan kursiyer 25:4447 nolu kanuna göre ödenek alan katılmayan kursiyer 26:4046 nolu kanuna göre iş kaybı tazminatı alanlar 27:Tüm sigorta kollarına tabi 60 gün 28:İşsizlik sigortası hariç 60 gün 29:Harp malulleri 3713 60 gün 30:Tüm sigorta kollarına tabi 90 gün 31:İşsizlik sigortası hariç 90 gün 32:Harp malulleri 3713 90 gün 33:Tüm sigorta kollarına tabi 180 gün 34:İşsizlik sigortası hariç 180 gün 35:Harp malulleri 3713 180 gün 36:Birleşik krallıkta ikameti talep edenler 37:Birleşik krallıkta ikameti talep etmeyenler 38:Kısa çalışma ödeneği alanlar 39:Geçici 20.maddeye tabi olanlar 40:Kamu idarelerinde iş akdi askıda olanlar 41:İtibari hizmet süresine tabi olarak çalışanlar 42:Altmış gün fiili hizmet süresi zammına tabi olanlardan itibari hizmet süresine tabi 43:Doksan gün fiili hizmet süresi zammına tabi olanlardan itibari hizmet süresine tabi 44:Bakmakla yükümlü olunmayan çıraklar, mesleki öğrenciler 45:Bakmakla yükümlü olunmayan stajyer öğrenciler 46:Bakmakla yükümlü olunmayan İŞKUR kursiyerleri 47:İŞKUR kursiyerleri 48:Yarım çalışma ödeneği 49:Emekli yer altında çalışan 50:Tamamlayıcı bakmakla yükümlü 51:Tamamlayıcı bakmakla yükümlü olmayan 52:Güvenlik korucuları Ek 15 53:Ml. ayl. bağlanmamış 670KHK |
| 268 | per_agine_tabii | Tinyint | Personel AGI'ne tabi ? | 0:Evet 1:Hayır |
| 269 | per_ozur_5763_kont_dahili_fl | Bit | 5763 Sayılı  Kanun Özürlü Kontenjana Dahil Mi ? |  |
| 270 | per_yabanci_ulke | Nvarchar(30) | Yabancı Ülke |  |
| 271 | per_sigortalilik_turu | Tinyint | Sigortalılık Türü | 0:Mecburi Sigortalı 1:Muhtar 2:Sosyal Güvenlik Sözleşmesi Bulunmayan Ülkelerde Çalışan 3:Vergiden Muaf 4:Şirket Ortakları 5:Göçmenler 6:Tarım 7:Yurt Dışı Borçlanması Yapanlar 8:İsteğe Bağlı 9:Çıraklar ve Stajyer Öğrenciler 10:Jokey ve Antrenörler 11:Sosyal Güvenlik Destek Primine Tabi Çalışanlar 12:Avukat ve Noterler 13:Tahsis Talepli İsteğe Bağlı 14:Cezaevi Çalışanı 15:Sanatçılar, Düşünürler ve Yazarlar 16:Sözleşmesiz Ülkeler, Yabancı Uyruklu Sigortalılar 17:4081 Sayılı Kanuna Göre Çalışanlar 18:Umumi Kadınlar 19:Usta ve Sözleşmeli Öğretici 20:Sendika Konfederasyon Başkanlıkları, Yönetim Kurulu Üyeleri 21:657 Sayılı Kanunun 4/B'sine Tabi Olanlar 22:657 Sayılı Kanunun 4/C'sine Tabi Olanlar 23:İşkur Kursiyerleri 24:İş Kaybı Tazminatı Alanlar 25:YÖK ve ÖSYM Kısmi İstihdam 26:Stajyer 27:İntörn Öğrenci 28:Harp Malulleri, Vazife Malulleri 2330 ve 3713 Sayılı Kanuna Göre Aylık Alan 29:Bursiyer 30:Güvenlik Korucusu 31:Tamamlayıcı Ya Da Alan Eğitimi Gören Öğrenciler |
| 272 | per_eski_sicil_no | Nvarchar(25) | Eski Sicil No |  |
| 273 | per_tabiioldugukanun2 | Tinyint | Tabi Olduğu Kanun | 0:Hiçbir Kanuna Tabi Değil 1:5225 Sayılı GV Kültür Yatırımı ve Girişimi Kanununa Tabi (Yatırım Aşaması) 2:5225 Sayılı GV Kültür Yatırımı ve Girişimi Kanununa Tabi (İşletme Aşaması) 3:4691 Sayılı Teknoloji Geliştirme Kanunu 4:5746 Sayılı ARGE Faaliyetlerinin Desteklenmesi Kanununa Tabi 5:6550 Sayılı Araştırma Alt Yapılarının Desteklenmesi Kanununa (ARGE) Tabi |
| 274 | per_tabiioldugukanun2_diger | Tinyint | Tabi Olduğu Kanun | 0:Hiçbir Kanuna Tabi Değil 1:5225 Sayılı GV Kültür Yatırımı ve Girişimi Kanununa Tabi (Yatırım Aşaması) 2:5225 Sayılı GV Kültür Yatırımı ve Girişimi Kanununa Tabi (İşletme Aşaması) 3:4691 Sayılı Teknoloji Geliştirme Kanunu 4:5746 Sayılı ARGE Faaliyetlerinin Desteklenmesi Kanununa Tabi 5:6550 Sayılı Araştırma Alt Yapılarının Desteklenmesi Kanununa (ARGE) Tabi |
| 275 | per_maaskiminhesabina | Tinyint | Maaş Kimin Hesabına ? | 0:Kendi Hesabına 1:Eşinin Hesabına 2:Annesinin Hesabına 3:Babasının Hesabına 4:1.Çocuk Hesabına 5:2.Çocuk Hesabına 6:3.Çocuk Hesabına |
| 276 | per_maassistemikodu | Nvarchar(25) | Maaş Sistemi Kodu |  |
| 277 | per_is_grup_kodu | Nvarchar(25) | İş Grup Kodu |  |
| 278 | per_unvan_kodu | Nvarchar(25) | Ünvan Kodu |  |
| 279 | per_raporlama_yapacagi_per_kod | Nvarchar(25) | Raporlama Yapacağı Personel Kodu |  |
| 280 | per_okul_kodu | Nvarchar(25) | Okul Kodu |  |
| 281 | per_okul_bolum_kodu | Nvarchar(25) | Okul Bölüm Kodu |  |
| 282 | per_bolum_kodu | Nvarchar(25) | Bölüm Kodu |  |
| 283 | per_alt_dept_kod | Nvarchar(25) | Alt Departman Kodu |  |
| 284 | per_kanun_gecerlilik_tarihi | DateTime | Kanun Geçerlilik Tarihi |  |
| 285 | per_kanun_gecerlilik_tarihi_diger | DateTime | Kanun Geçerlilik Tarihi |  |
| 286 | per_tabiioldugukanun3 | Tinyint | Tabi Olduğu Kanun | 0:Tanımsız 1:102_1'e tabi 2:102_2'ye tabi 3:102_3'e tabi 4:102_4'e tabi 5:102_5'e tabi |
| 287 | per_mezuniyetyili | Smallint | Mezuniyet Yılı |  |
| 288 | per_proje_kodu | Nvarchar(25) | Proje Kodu |  |
| 289 | per_hazine_destegine_tabi_fl | Bit | Hazine Desteğine Tabi Mi ? |  |
| 290 | per_KEP_adresi | Nvarchar(80) | Kayıtlı Elektronik Posta (KEP) Adresi |  |
| 291 | per_sigara_fl | Bit | Sigara Kullanıyor Mu ? |  |
| 292 | per_otobes_fl | Bit | Otomatik Katılımlı Bireysel Emeklilik Sistemi (BES) Var Mı ? |  |
| 293 | per_otobes_sigorta | Tinyint | Bireysel Emeklilik Sistemi (BES) Sigorta Şirketleri | 0:Tanımsız 1:Aegon 2:Allianz Hayat 3:Allianz 4:Anadolu 5:Asya 6:Avivasa 7:Axa 8:BNP 9:Cigna 10:Fiba 11:Garanti 12:Groupama 13:Halk 14:NN 15:Katılım 16:Metlife 17:Vakıf 18:Ziraat |
| 294 | per_otobes_orani | Float | Bireysel Emeklilik Sistemi (BES) Oranı |  |
| 295 | per_otobes_hesap_no | Nvarchar(30) | Bireysel Emeklilik Sistemi (BES) Hesap Numarası |  |
| 296 | per_otobes_grup_sozlesme_no | Nvarchar(25) | Bireysel Emeklilik Sistemi (BES) Grup Sözleşme Numarası |  |
| 297 | per_otobes_fon_tercihi | Tinyint | Bireysel Emeklilik Sistemi (BES) Fon Tercihi | 0:Faizli Fon 1:Faizsiz Fon |
| 298 | per_otobes_giris | DateTime | Bireysel Emeklilik Sistemi (BES) Giriş Tarihi |  |
| 299 | per_otobes_ayrilis | DateTime | Bireysel Emeklilik Sistemi (BES) Ayrılış Tarihi |  |
| 300 | per_sosyal_linkedin | Nvarchar(50) | Linkedin Hesabı |  |
| 301 | per_sosyal_webadresi | Nvarchar(50) | Web Adresi |  |
| 302 | per_sosyal_youtube | Nvarchar(50) | Youtube Hesabı |  |
| 303 | per_sosyal_twitter | Nvarchar(50) | Twitter Hesabı |  |
| 304 | per_sosyal_facebook | Nvarchar(50) | Facebook Hesabı |  |
| 305 | per_sosyal_google | Nvarchar(50) | Google Hesabı |  |
| 306 | per_sosyal_pinterest | Nvarchar(50) | Pinterest Hesabı |  |
| 307 | per_sosyal_instagram | Nvarchar(50) | Instagram Hesabı |  |
| 308 | per_sosyal_snapchat | Nvarchar(50) | Snapchat Hesabı |  |
| 309 | per_vergiden_muhaf_odenek1 | Float | Vergiden Muaf Ödenek 1 |  |
| 310 | per_vergiden_muhaf_odenek2 | Float | Vergiden Muaf Ödenek 2 |  |
| 311 | per_sabit_gelirversi_orani | Float | Sabit Gelir Vergisi Oranı |  |
| 312 | per_spor_dali | Tinyint | Spor Dalı |  |
| 313 | per_kisacalisma_fl | Bit | Kısa Çalışma Ödeneği Var Mı ? |  |
| 314 | per_kisacalisma_baslangic | DateTime | Kısa Çalışma Ödeneği Başlangıç Tarihi |  |
| 315 | per_kisacalisma_bitis | DateTime | Kısa Çalışma Ödeneği Bitiş Tarihi |  |
| 316 | per_kisacalisma_haftaliksaat | Float | Kısa Çalışma Ödeneği İçin Haftalık Çalışma Saati |  |
| 317 | per_7252_ortalama_gun | Tinyint | 7252 Sayılı Teşvik Kanununa Ait Ortalama Gün |  |
| 318 | per_engellilik_orani | Tinyint | Engellilik Oranı |  |
| 319 | per_cikis_nakil_ssk_sube | Integer | Personel Çıkış Bildirgesinde "Nakil" Seçildiğinde, Nakil Gideceği Şube |  |
| 320 | per_4691_calisma_tipi | Tinyint | MUHSGK 4691 Sayılı Kanun Çalışma Tipi | 0:AR-GE 1:Destek |
| 321 | per_teknokentproje | Nvarchar(25) | Teknokent Proje Kodu |  |
| 322 | per_asgucrverind_yapilmasin_fl | Tinyint | Asgari Ücret İstisna Tipi (Asgari Ücret İstisna Hesabındaki Kümülatif Hesabı, Hangi Tarihte Başlasın ?) | 0:Giriş Kıdem Tarihinden 1:Uygulanmasın 2:Girilen Tarihten |
| 323 | per_asgucrverind_tarihi | DateTime | Asgari Ücret Vergi İndirimi Tarihi |  |
| 324 | per_calisma_gunu | Smallint | Personel Bazında Çalışma Günü |  |
| 325 | per_digerkanunaktif | Tinyint | Diğer Kanun Aktif Mi ? | 0:Hayır 1:Evet |


Güncellenme Tarihi : 11.12.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**