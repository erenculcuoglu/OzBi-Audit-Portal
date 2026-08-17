# TABLO NO: 107

## Tablo Adı: FIRMALAR - Firma Tanımları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | fir_Guid | Uniqueidentifier |  |  |
| 1 | fir_DBCno | Smallint |  |  |
| 2 | fir_SpecRECno | Integer |  |  |
| 3 | fir_iptal | Bit |  |  |
| 4 | fir_fileid | Smallint |  |  |
| 5 | fir_hidden | Bit |  |  |
| 6 | fir_kilitli | Bit |  |  |
| 7 | fir_degisti | Bit |  |  |
| 8 | fir_checksum | Integer |  |  |
| 9 | fir_create_user | Smallint |  |  |
| 10 | fir_create_date | DateTime |  |  |
| 11 | fir_lastup_user | Smallint |  |  |
| 12 | fir_lastup_date | DateTime |  |  |
| 13 | fir_special1 | Nvarchar(127) |  |  |
| 14 | fir_special2 | Nvarchar(127) |  |  |
| 15 | fir_special3 | Nvarchar(127) |  |  |
| 16 | fir_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | fir_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | fir_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | fir_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | fir_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | fir_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | fir_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | fir_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | fir_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | fir_sirano | Integer | Firma Sıra No |  |
| 26 | fir_unvan | Nvarchar(127) | Firma Ünvanı |  |
| 27 | fir_unvan2 | Nvarchar(127) | Firma Ünvanı 2 |  |
| 28 | fir_TCkimlik | Nvarchar(15) | TC Kimlik |  |
| 29 | fir_Yazisma | Nvarchar(10) | Yazışma Adresi |  |
| 30 | fir_Bordro | Nvarchar(10) | Bordro |  |
| 31 | fir_FHesapNo | Nvarchar(30) | Hesap No |  |
| 32 | fir_FVergiDaire | Nvarchar(10) | Vergi Dairesi |  |
| 33 | fir_FVergiNo | Nvarchar(20) | Vergi No |  |
| 34 | fir_FBolgeNo | Nvarchar(20) | Bölge No |  |
| 35 | fir_Muhtasar | Bit | Muhtasar? | 0:Üç Aylık 1:Aylık |
| 36 | fir_Isci | Bit | İşçi? | 0:Yok 1:Var |
| 37 | fir_BasTar | DateTime | Başlangıç Tarihi |  |
| 38 | fir_Istigal | Nvarchar(40) |  |  |
| 39 | fir_Defter | Bit | Defter | 0:Bilanço 1:İşletme |
| 40 | fir_TicSicilNo | Nvarchar(25) | Ticaret Sicil No |  |
| 41 | fir_IslIzn1 | Bit | İşlem İzni |  |
| 42 | fir_IslIzn2 | Bit | İşlem İzni |  |
| 43 | fir_IslIzn3 | Bit | İşlem İzni |  |
| 44 | fir_IslIzn4 | Bit | İşlem İzni |  |
| 45 | fir_IslIzn5 | Bit | İşlem İzni |  |
| 46 | fir_IslIzn6 | Bit | İşlem İzni |  |
| 47 | fir_IslIzn7 | Bit | İşlem İzni |  |
| 48 | fir_IslIzn8 | Bit | İşlem İzni |  |
| 49 | fir_IslIzn9 | Bit | İşlem İzni |  |
| 50 | fir_IslIzn10 | Bit | İşlem İzni |  |
| 51 | fir_IslIzn11 | Bit | İşlem İzni |  |
| 52 | fir_IslIzn12 | Bit | İşlem İzni |  |
| 53 | fir_IslIzn13 | Bit | İşlem İzni |  |
| 54 | fir_IslIzn14 | Bit | İşlem İzni |  |
| 55 | fir_IslIzn15 | Bit | İşlem İzni |  |
| 56 | fir_SmmEntegrasyon | Tinyint | Smm Entegrasyonu Yapılsın Mı? | 0:Evet 1:Hayır |
| 57 | fir_Mali_Mus_firmasi | Tinyint | Firma Tipi | 0:Normal 1:Mali Müşavir 2:Oda |
| 58 | fir_Iso_no | Nvarchar(15) | ISO Numarası |  |
| 59 | fir_maasBankaKod | Nvarchar(25) | Firma Maaş Banka Kodu |  |
| 60 | fir_baslangic_tarihi | DateTime | Başlangıç Tarihi |  |
| 61 | fir_OTVden_muaf | Bit | Firma ÖTV'den Muaf Mı? |  |
| 62 | fir_krediriskyuzde1 | Float | Kredi Risk Yüzdesi |  |
| 63 | fir_krediriskneolacak1 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 64 | fir_kredirisklimityuzde1hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 65 | fir_kredirisklimityuzde1 | Float | Kredi Risk Limit Yüzdesi |  |
| 66 | fir_kredirisklimittipi1 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 67 | fir_krediriskyuzde2 | Float | Kredi Risk Yüzdesi |  |
| 68 | fir_krediriskneolacak2 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 69 | fir_kredirisklimityuzde2hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 70 | fir_kredirisklimityuzde2 | Float | Kredi Risk Limit Yüzdesi |  |
| 71 | fir_kredirisklimittipi2 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 72 | fir_krediriskyuzde3 | Float | Kredi Risk Yüzdesi |  |
| 72 | fir_krediriskyuzde3 | Float | Kredi Risk Yüzdesi |  |
| 73 | fir_krediriskneolacak3 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 74 | fir_kredirisklimityuzde3hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 75 | fir_kredirisklimityuzde3 | Float | Kredi Risk Limit Yüzdesi |  |
| 76 | fir_kredirisklimittipi3 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 77 | fir_krediriskyuzde4 | Float | Kredi Risk Yüzdesi |  |
| 78 | fir_krediriskneolacak4 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 79 | fir_kredirisklimityuzde4hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 80 | fir_kredirisklimityuzde4 | Float | Kredi Risk Limit Yüzdesi |  |
| 81 | fir_kredirisklimittipi4 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 82 | fir_krediriskyuzde5 | Float | Kredi Risk Yüzdesi |  |
| 83 | fir_krediriskneolacak5 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 84 | fir_kredirisklimityuzde5hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 85 | fir_kredirisklimityuzde5 | Float | Kredi Risk Limit Yüzdesi |  |
| 86 | fir_kredirisklimittipi5 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 87 | fir_krediriskyuzde6 | Float | Kredi Risk Yüzdesi |  |
| 88 | fir_krediriskneolacak6 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 89 | fir_kredirisklimityuzde6hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 90 | fir_kredirisklimityuzde6 | Float | Kredi Risk Limit Yüzdesi |  |
| 91 | fir_kredirisklimittipi6 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 92 | fir_krediriskyuzde7 | Float | Kredi Risk Yüzdesi |  |
| 93 | fir_krediriskneolacak7 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 94 | fir_kredirisklimityuzde7hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 95 | fir_kredirisklimityuzde7 | Float | Kredi Risk Limit Yüzdesi |  |
| 96 | fir_kredirisklimittipi7 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 97 | fir_krediriskyuzde8 | Float | Kredi Risk Yüzdesi |  |
| 98 | fir_krediriskneolacak8 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 99 | fir_kredirisklimityuzde8hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 100 | fir_kredirisklimityuzde8 | Float | Kredi Risk Limit Yüzdesi |  |
| 101 | fir_kredirisklimittipi8 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 102 | fir_krediriskyuzde9 | Float | Kredi Risk Yüzdesi |  |
| 103 | fir_krediriskneolacak9 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 104 | fir_kredirisklimityuzde9hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 105 | fir_kredirisklimityuzde9 | Float | Kredi Risk Limit Yüzdesi |  |
| 106 | fir_kredirisklimittipi9 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 107 | fir_krediriskyuzde10 | Float | Kredi Risk Yüzdesi |  |
| 108 | fir_krediriskneolacak10 | Tinyint | Kredi Risk Ne Olacak? | 0: Risk ve Kredi Tutarına Dahil 1:Risk Tutarına Dahil 2:Kredi Tutarına Dahil |
| 109 | fir_kredirisklimityuzde10hesap | Tinyint | Kredi Risk Limit Yüzde Hesabı | 0:Girileni Al 1:Dinamik Hesapla |
| 110 | fir_kredirisklimityuzde10 | Float | Kredi Risk Limit Yüzdesi |  |
| 111 | fir_kredirisklimittipi10 | Tinyint | Kredi Risk Limit Tipi | 0:Kredi ve Risk Limitine Dahil 1:Sadece Risk Limitine Dahil 2:Sadece Teminat 3:Sadece Kredi Limitine Dahil |
| 112 | fir_def_kasakodu | Nvarchar(25) | Defter Kasa Kodu |  |
| 113 | fir_isletmetipi | Tinyint | Firma İşletme Tipi | 0:Mal Kar Haddi Toptancı 1:Hizmet Kar Haddi 2:Asgari Gayri Safi Hasılat Zirai |
| 114 | fir_isletmefaliyetturu | Nvarchar(20) | İşletme Faaliyet Türü |  |
| 115 | fir_kdvucaylik | Bit | Kdv Üç Aylık Mı? |  |
| 116 | fir_EAN_kodu | Nvarchar(7) | EAN Kodu |  |
| 117 | fir_mukellefiyet | Tinyint | Mükellefiyet |  |
| 118 | fir_kan_kab_ed_gider_kodu | Nvarchar(25) | Kanunen Kabul Edilmeyen Gider Hesap Kodu |  |
| 119 | fir_web_sayfasi | Nvarchar(50) | Web Sayfası |  |
| 120 | fir_web_kullanici_adi | Nvarchar(20) | Web Sayfası Kullanıcı Adı |  |
| 121 | fir_web_kullanici_sifresi | Nvarchar(127) | Web Sayfası Kullanıcı Şifresi |  |
| 122 | fir_web_musteri_no | Nvarchar(20) | Web Müşteri No |  |
| 123 | fir_OIVden_muaf | Bit | Özel İletişim Vergisinden Muaf Mı ? |  |
| 124 | fir_maassistemikodu | Nvarchar(25) | Maaş Sistemi Kodu |  |
| 125 | fir_genel_email | Nvarchar(80) | Genel e-Posta |  |
| 126 | fir_nace_kodu | Nvarchar(25) |  |  |
| 127 | fir_mali_muhur_sifre | Nvarchar(127) | Mali Mühür Şifresi |  |
| 128 | fir_zaman_damgasi_kullanici | Integer | Zaman Damgası Kullanıcı Kodu |  |
| 129 | fir_zaman_damgasi_sifre | Nvarchar(127) | Zaman Damgası Şifresi |  |
| 130 | fir_edefter_sube_adi | Nvarchar(127) | e-Defter Şube Adı |  |
| 131 | fir_edefter_sube_no | Integer | e-Defter Şube Numarası |  |
| 132 | fir_edefter_baslangic | DateTime | e-Defter Mükellefiyeti Başlangıç Tarihi |  |
| 133 | fir_edefter_max_boyut | Integer | e-Defter Maksimum Boyut |  |
| 134 | fir_edefter_doviz_tipi | Tinyint | e-Defter Döviz Tipi | 0:Ana dövizi TL 1:Ana dövizi Dövizli 2:Alternatif Dövizi Dövizli 3:Dövizli Nazım Hesap TL |
| 135 | fir_edefter_aktif_grup | Tinyint | e-Defter Aktif Grubu |  |
| 136 | fir_tuik_isyerikayitno | Nvarchar(20) | Tüik (Türkiye İstatistik Kurumu) İş Yeri Kayıt No |  |
| 137 | fir_efatura_baslangic | DateTime | e-Fatura Başlnagıç Tarihi |  |
| 138 | fir_earsiv_baslangic | DateTime | e-Arşiv Başlnagıç Tarihi |  |
| 139 | fir_KEP_adresi | Nvarchar(80) | Kayıtlı e-Posta (KEP) Adresi |  |
| 140 | fir_Ticaret_Sicil_Mudurlugu | Nvarchar(4) | Ticaret Sicil Müdürlüğü |  |
| 141 | fir_edefter_smm_kodu | Nvarchar(25) | e-Defter Smm Kodu |  |
| 142 | fir_edefter_ymm_kodu | Nvarchar(25) | e-Defter Ymm Kodu |  |
| 143 | fir_edefter_nace_kodu | Nvarchar(80) | e-Defter Nace Kodu |  |
| 144 | fir_TasfiyeTarihi | DateTime | Tasfiye Tarihi |  |
| 145 | fir_muhasebe_yetkilisi | Nvarchar(25) | Muhasebe Yetkilisi |  |
| 146 | fir_edefter_yetkilisi | Nvarchar(25) | e-Defter Yetkilisi |  |
| 147 | fir_mali_muhur_tipi | Tinyint | Mali Mühür Tipi | 0:Akış 1:GemSafe 2:Seimens 3:StarCOS 4:Aladdin 5:Gelişmiş |
| 148 | fir_mali_muhur_surucusu | Nvarchar(127) | Mali Mühür Sürücüsü |  |
| 149 | fir_edefter_sube_defteri_mi | Bit | e-Defter Şube Defteri Mi ? |  |
| 150 | fir_mikro_web_servis_kodu | Nvarchar(40) | Mikro Web Servis Kodu |  |
| 151 | fir_uyelik_hopi_firma_kodu | Nvarchar(40) | Hopi Üyelik Firma Kodu |  |
| 152 | fir_uyelik_hopi_kullanici | Nvarchar(40) | Hopi Üyelik Kullanıcı Adı |  |
| 153 | fir_uyelik_hopi_sifre | Nvarchar(127) | Hopi Üyelik Şifresi |  |
| 154 | fir_tasfiye_hali_firmasi_mi | Bit | Firma Tasfiye Halinde Mi ? |  |
| 155 | fir_smtp_host | Nvarchar(127) | Smtp (e-Posta Gönderim Portu) Host |  |
| 156 | fir_smtp_port | Integer | Smtp Port |  |
| 157 | fir_smtp_enable_ssl | Bit | Smtp Enabla Ssl |  |
| 158 | fir_smtp_username | Nvarchar(127) | Smtp Kullanıcı Adı |  |
| 159 | fir_smtp_sifre | Nvarchar(127) | Smtp Şifre |  |
| 160 | fir_BaslangicTarihiTipi | Tinyint | Başlangıç Tarihi Tipi | 0:Tanımsız 1:İlk Kuruluş 2:Yeni Nevi Başlangıcı 3:Tasfiye Hali Başlangıcı |
| 161 | fir_BitisTarihiTipi | Tinyint | Bitiş Tarihi Tipi | 0:Tanımsız 1:Yok 2:Eski Nevi Kapanışı 3:Tasfiye Hali 4:Tasfiye Hali Bitişi 5:Özel Durum |
| 162 | fir_VergiDetayTipi | Tinyint | Vergi Detay Tipi | 0:Tanımsız 1:Var 2:Yok |
| 163 | fir_SMMDefterBeyanApiKey | Nvarchar(50) | SMM Defter Beyaz Api Key |  |
| 164 | fir_SMMDefterBeyanApiSecret | Nvarchar(50) | SMM Defter Beyan Api Secret |  |
| 165 | fir_DefterBeyanTipi | Tinyint | Defter Beyan Tipi | 0:Tanımsız 1:SM 2:İşletme 3:Çiftçi 4:Basit Usül |
| 166 | fir_utssisteminekayitlimi | Bit | ÜTS Sistemine Kayıtlı Mı ? |  |
| 167 | fir_utskurumno | Nvarchar(15) | ÜTS Kurum No |  |
| 168 | fir_tescil_noktasi | Nvarchar(20) | Lisans Tescil Noktası |  |
| 169 | fir_DefBeyAmorDef_baslangic | DateTime | Defter Beyanda Amortisman Defteri Başlangıcı |  |
| 170 | fir_DefBeyStokDef_baslangic | DateTime | Defter Beyanda Stok Defteri Başlangıcı |  |
| 171 | fir_EBasvuruTakipSayfaNo | Tinyint | e-Başvuru Takip Sayfa Numarası |  |
| 172 | fir_posetucaylik | Tinyint | Geri Kazanım Katılım Payı (Poşet) Beyannamesi Dönem Tipi | 0:Aylık 1:Üç Aylık 2:Altı Aylık |
| 173 | fir_EDefterSaklamaSorguDurumu | Tinyint | e-Defter Saklama Sorgu Durumu |  |
| 174 | fir_IKPortalEntegrasyon_fl | Bit | İK Portal Entegrasyonu Aktif Edilsin Mi ? |  |
| 175 | fir_IKUser | Nvarchar(127) | İK Kullanıcı Adı |  |
| 176 | fir_IKPassword | Nvarchar(127) | İK Kullanıcı Şifresi |  |
| 177 | fir_turizmucaylik | Bit | Turizm Katkı Payı Beyannamesi Aylık Mı Üç Aylık Mı ? | 0:Aylık 1:3 Aylık |
| 178 | fir_edefterucaylik | Bit | e-Defter Aylık Mı Üç Aylık Mı ? | 0:Aylık 1:3 Aylık |
| 179 | fir_idm_id | Integer | Idm Id |  |
| 180 | fir_muhsgk_1003B_fl | Bit | 1003B SGK Muhtasar Beyannamesi |  |
| 181 | fir_eMustahsil_baslangic | DateTime | e-Müstahsil Başlangıç Tarihi |  |
| 182 | fir_eSMMM_baslangic | DateTime | e-SMMM Başlangıç Tarihi |  |
| 183 | fir_eirsaliye_baslangic | DateTime | e-İrsaliye Başlangıç Tarihi |  |
| 184 | fir_parasut_firmaid | Integer | Firma Id (Paraşüt) |  |
| 185 | fir_parasut_username | Nvarchar(127) | Kullanıcı Adı (Paraşüt) |  |
| 186 | fir_parasut_password | Nvarchar(127) | Şifre (Paraşüt) |  |
| 187 | fir_sinyal_tarihi | DateTime | Sinyal Tarihi |  |
| 188 | fir_TAKEP_username | Nvarchar(127) | TAKEP (Tarımsal Kesinti Programı) Kullanıcı Adı |  |
| 189 | fir_TAKEP_password | Nvarchar(127) | TAKEP (Tarımsal Kesinti Programı) Şifresi |  |
| 190 | fir_earsivportal_kullaniciadi | Nvarchar(50) | e-Arşiv Portalı Kullanıcı Adı |  |
| 191 | fir_earsivportal_sifre | Nvarchar(50) | e-Arşiv Portalı Şifresi |  |
| 192 | fir_defterbeyan_versiyon | Tinyint | Defter Beyan Versiyonu |  |
| 193 | fir_EDefterOtomatikGIByedek_fl | Bit | Gib Bildirim Uygulaması ile Otomatik Olarak e-Defter Yedekleri Alınabilir Mi ? |  |
| 194 | fir_EDefterOtomatikGIBKlasor | Nvarchar(127) | Otomatik Olarak Yedek Alınan e-Defter Dosyalarının GİB Klasörü |  |
| 195 | fir_FintechID1 | Nvarchar(50) | Fintech ID |  |
| 196 | fir_FintechID2 | Nvarchar(50) | Fintech ID |  |
| 197 | fir_FintechID3 | Nvarchar(50) | Fintech ID |  |
| 198 | fir_bizmu_firmaid | Integer | Bizmu Firma ID |  |
| 199 | fir_bizmu_username | Nvarchar(127) | Bizmu Kullanıcı Adı |  |
| 200 | fir_bizmu_password | Nvarchar(127) | Bizmu Şifresi |  |
| 201 | fir_MOH_Authentication | Tinyint | Online Hesabım'ın IDMv1'den IDMv2'ye Geçirilmesi |  |
| 202 | fir_edefter_muvafakatname_smm_kodu | Nvarchar(25) | e-Defter Muvafakatname SMM Kodu |  |
| 203 | fir_edefter_imzalama_sekli | Tinyint | e-Defter İmzalama Şekli | 0:SB 1:KAMUSM 2:HSM |
| 204 | fir_edefter_kamusm_mali_muhur_tipi | Tinyint | e-Defter Kamu SM Mali Mühür Şekli | 0:Akis 1:AkisKK 2:Cardos 3:Aladdin 4:Datakey 5:Gemplus 6:Keycorp 7:NCipher 8:Safesign 9:Sefirot 10:Aepkeyper 11:Utimaco 12:TKart 13:NetId |
| 205 | fir_edefter_mali_muhur_nitelikli_mi | Bit | e-Defter Mali Mühür Nitelikli Mi ? |  |


Güncellenme Tarihi : 25.10.2024 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**