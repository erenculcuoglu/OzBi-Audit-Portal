# TABLO NO: 21

## Tablo Adı: SIPARISLER - Sipariş Hareketleri Tablosu

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | sip_Guid | Uniqueidentifier |  |  |
| 1 | sip_DBCno | Smallint |  |  |
| 2 | sip_SpecRECno | Integer |  |  |
| 3 | sip_iptal | Bit |  |  |
| 4 | sip_fileid | Smallint |  |  |
| 5 | sip_hidden | Bit |  |  |
| 6 | sip_kilitli | Bit |  |  |
| 7 | sip_degisti | Bit |  |  |
| 8 | sip_checksum | Integer |  |  |
| 9 | sip_create_user | Smallint |  |  |
| 10 | sip_create_date | DateTime |  |  |
| 11 | sip_lastup_user | Smallint |  |  |
| 12 | sip_lastup_date | DateTime |  |  |
| 13 | sip_special1 | Nvarchar(127) |  |  |
| 14 | sip_special2 | Nvarchar(127) |  |  |
| 15 | sip_special3 | Nvarchar(127) |  |  |
| 16 | sip_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | sip_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | sip_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | sip_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | sip_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | sip_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | sip_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | sip_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | sip_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | sip_firmano | Integer | Firma No |  |
| 26 | sip_subeno | Integer | Şube No |  |
| 27 | sip_tarih | DateTime | Sipariş Tarihi |  |
| 28 | sip_teslim_tarih | DateTime | Sipariş Teslim Tarihi |  |
| 29 | sip_tip | Tinyint | Sipariş Tipi | 0:Talep 1:Temin |
| 30 | sip_cins | Tinyint | Sipariş Cinsi | 0:Normal Sipariş 1:Konsinye Sipariş 2:Proforma Sipariş   3:Dış Ticaret Siparişi 4:Fason Siparişi 5:Dahili Sarf Siparişi 6:Depolar Arası Sipariş 7:Satın Alma Talebi 8:Üretim Talebi 9:İş Emirleri 10:Fason Talebi |
| 31 | sip_evrakno_seri | dbo.evrakseri_str | Sipariş Evrak Seri No |  |
| 32 | sip_evrakno_sira | Integer | Sipariş Evrak Sıra No |  |
| 33 | sip_satirno | Integer | Sipariş Satır No |  |
| 34 | sip_belgeno | dbo.belgeno_str | Sipariş Belge No |  |
| 35 | sip_belge_tarih | DateTime | Belge Tarihi |  |
| 36 | sip_satici_kod | Nvarchar(25) | Satıcı Kodu | Bkz. CARI_HESAPLAR |
| 37 | sip_musteri_kod | Nvarchar(25) | Müşteri Kodu | Bkz. CARI_HESAPLAR |
| 38 | sip_stok_kod | Nvarchar(25) | Stok Kodu | Bkz. STOKLAR |
| 39 | sip_b_fiyat | Float | Sipariş Birim Fiyatı | Bkz. STOKLAR |
| 40 | sip_miktar | Float | Sipariş Miktarı |  |
| 41 | sip_birim_pntr | Tinyint | Siparişin Birim ile Bağlantısı |  |
| 42 | sip_teslim_miktar | Float | Sipariş Teslim Miktarı |  |
| 43 | sip_tutar | Float | Sipariş Tutarı |  |
| 44 | sip_iskonto_1 | Float | İskonto |  |
| 45 | sip_iskonto_2 | Float | İskonto |  |
| 46 | sip_iskonto_3 | Float | İskonto |  |
| 47 | sip_iskonto_4 | Float | İskonto |  |
| 48 | sip_iskonto_5 | Float | İskonto |  |
| 49 | sip_iskonto_6 | Float | İskonto |  |
| 50 | sip_masraf_1 | Float | Masraf |  |
| 51 | sip_masraf_2 | Float | Masraf |  |
| 52 | sip_masraf_3 | Float | Masraf |  |
| 53 | sip_masraf_4 | Float | Masraf |  |
| 54 | sip_vergi_pntr | Tinyint | Siparişle İlgili Vergi Bağlantısı |  |
| 55 | sip_vergi | Float | Sipariş Vergisi |  |
| 56 | sip_masvergi_pntr | Tinyint | Siparişle İlgili Ana Vergi Bağlantısı |  |
| 57 | sip_masvergi | Float | Sipariş Ana Vergisi |  |
| 58 | sip_opno | Integer | Ödeme Planı No |  |
| 59 | sip_aciklama | Nvarchar(50) | Sipariş Açıklaması |  |
| 60 | sip_aciklama2 | Nvarchar(50) | Sipariş 2. Açıklaması |  |
| 61 | sip_depono | Tinyint | Sipariş Depo No | Bkz. DEPOLAR |
| 62 | sip_OnaylayanKulNo | Tinyint | Onaylayan Kullanıcı No | Bkz. KULLANICILAR |
| 63 | sip_vergisiz_fl | Bit | Vergisiz Mi ? |  |
| 64 | sip_kapat_fl | Bit | Sipariş Kapandı Mı ? |  |
| 65 | sip_promosyon_fl | Bit | Promosyon Var Mı ? |  |
| 66 | sip_cari_sormerk | Nvarchar(25) | Cari Sorumluluk Merkezi |  |
| 67 | sip_stok_sormerk | Nvarchar(25) | Stok Sorumluluk Merkezi |  |
| 68 | sip_cari_grupno | Tinyint | Cari Grup No |  |
| 69 | sip_doviz_cinsi | Tinyint | Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 70 | sip_doviz_kuru | Float | Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 71 | sip_alt_doviz_kuru | Float | Alternatif Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 72 | sip_adresno | Integer | Adres No |  |
| 73 | sip_teslimturu | Nvarchar(4) | Teslim Türü |  |
| 74 | sip_cagrilabilir_fl | Bit | Çağrılabilir Sipariş Mi ? |  |
| 75 | sip_prosip_uid | Uniqueidentifier | Proforma Sipariş Uid | Bkz. PROFORMA_SIPARISLER |
| 76 | sip_iskonto1 | Tinyint | İskonto | 0:Brüt toplamdan yüzde, 1:Ara toplamdan yüzde, 2:Tutar İskonto masraf, 3:Miktar başına tutar, 4:Miktar2 başına tutar, 5:Miktar3 başına tutar, 6:Bedelsiz miktar, 7:İskonto1 yuzde, 8:İskonto1 aratop yüzde, 9:İskonto2 yüzde, 10:İskonto2 aratop yüzde, 11:İskonto3 yüzde, 12:İskonto3 aratop yüzde, 13:İskonto4 yüzde, 14:İskonto4 aratop yüzde, 15:İskonto5 yüzde, 16:İskonto5 aratop yüzde, 17:İskonto6 yüzde, 18:İskonto6 aratop yüzde, 19:Masraf1 yüzde, 20:Masraf1 aratop yüzde, 21:Masraf2 yüzde, 22:Masraf2 aratop yüzde, 23:Masraf3 yüzde, 24:Masraf3 aratop yüzde |
| 77 | sip_iskonto2 | Tinyint | İskonto | Yukarıdaki ile Aynı |
| 78 | sip_iskonto3 | Tinyint | İskonto | Yukarıdaki ile Aynı |
| 79 | sip_iskonto4 | Tinyint | İskonto | Yukarıdaki ile Aynı |
| 80 | sip_iskonto5 | Tinyint | İskonto | Yukarıdaki ile Aynı |
| 81 | sip_iskonto6 | Tinyint | İskonto | Yukarıdaki ile Aynı |
| 82 | sip_masraf1 | Tinyint | Masraf | Yukarıdaki ile Aynı |
| 83 | sip_masraf2 | Tinyint | Masraf | Yukarıdaki ile Aynı |
| 84 | sip_masraf3 | Tinyint | Masraf | Yukarıdaki ile Aynı |
| 85 | sip_masraf4 | Tinyint | Masraf | Yukarıdaki ile Aynı |
| 86 | sip_isk1 | Bit | Satır İskonto/Masraf? |  |
| 87 | sip_isk2 | Bit | Satır İskonto/Masraf? |  |
| 88 | sip_isk3 | Bit | Satır İskonto/Masraf? |  |
| 89 | sip_isk4 | Bit | Satır İskonto/Masraf? |  |
| 90 | sip_isk5 | Bit | Satır İskonto/Masraf? |  |
| 91 | sip_isk6 | Bit | Satır İskonto/Masraf? |  |
| 92 | sip_mas1 | Bit | Satır İskonto/Masraf? |  |
| 93 | sip_mas2 | Bit | Satır İskonto/Masraf? |  |
| 94 | sip_mas3 | Bit | Satır İskonto/Masraf? |  |
| 95 | sip_mas4 | Bit | Satır İskonto/Masraf? |  |
| 96 | sip_Exp_Imp_Kodu | Nvarchar(25) | Export Import Kodu (EXIM) |  |
| 97 | sip_kar_orani | Float | Kar Oranı |  |
| 98 | sip_durumu | Tinyint | Sipariş Durumu | 0:Stoktan Sevk Edilecek 1:Üretilecek   2:Satın Alınacak 3:Stoktan Sevk Edilecek (Rezerve Edildi) |
| 99 | sip_stal_uid | Uniqueidentifier | Satın Alma Şartları Uid | Bkz. SATINALMA_SARTLARI |
| 100 | sip_planlananmiktar | Float | Planlanan Miktar |  |
| 101 | sip_teklif_uid | Uniqueidentifier | Sipariş Teklif Uid | Bkz. VERILEN_TEKLIFLER |
| 102 | sip_parti_kodu | Nvarchar(25) | Parti Kodu |  |
| 103 | sip_lot_no | Integer | Lot No |  |
| 104 | sip_projekodu | Nvarchar(25) | Proje Kodu |  |
| 105 | sip_fiyat_liste_no | Integer | Fiyat Liste No |  |
| 106 | sip_Otv_Pntr | Tinyint |  |  |
| 107 | sip_Otv_Vergi | Float | Vergi |  |
| 108 | sip_otvtutari | Float | Ötv Tutarı |  |
| 109 | sip_OtvVergisiz_Fl | Tinyint | ÖTV Vergisiz Mi ? | 0:Vergili 1:Vergisiz |
| 110 | sip_paket_kod | Nvarchar(25) | Sipariş Paket Kodu |  |
| 111 | sip_Rez_uid | Uniqueidentifier | Rezervasyon Uid |  |
| 112 | sip_harekettipi | Tinyint | Hareket Tipi | 0:Stok 1:Hizmet 2:Gider 3:Demirbaş |
| 113 | sip_yetkili_uid | Uniqueidentifier | Yetkili Uid |  |
| 114 | sip_kapatmanedenkod | Nvarchar(25) | Kapatma Nedeni Kodu |  |
| 115 | sip_gecerlilik_tarihi | DateTime | Geçerlilik Tarihi |  |
| 116 | sip_onodeme_evrak_tip | Tinyint | Ön Ödeme Evrak Tipi | 0:Alış Faturası  1:Tahsilat Makbuzu  2:Kasa Tahsilat Fişi  3:Senet Giriş Bordrosu  4:Çek Giriş Bordrosu  5:Portföydeki Çek Karşılığı Nakit Kasa Tahsilat Makbuzu  6:Portföydeki Senet Karşılığı Nakit Tahsilat Makbuzu  7:Bankadan Kasaya Nakit Çekme Makbuzu  8:Kasadan Bankaya Nakit Yatırma Makbuzu  9:Kredi Virman Fişi  10:Kredi Kabul Fişi  11:Takas Çek Çıkış Bordrosu  12:Takas Çek Karşılıksız İade Bordrosu  13:Takas Çek İade Bordrosu  14:Takas Çek Ödeme Bordrosu  15:Tahsile Senet Çıkış Bordrosu  16:Tahsilden Protestolu Portföye Senet İade Bordrosu  17:Tahsil Senet İade Bordrosu  18:Tahsildeki Senet Ödeme Bordrosu  19:Teminata Çek Çıkış Bordrosu  20:Teminat Çek Karşılıksız İade Bordrosu  21:Teminat Çek İade Bordrosu  22:Teminattaki Çek Ödeme Bordrosu  23:Teminata Senet Çıkış Bordrosu  24:Teminatdan Protestolu Portföye Senet İade Bordrosu  25:Teminat Senet İade Bordrosu  26:Teminat Senet Ödeme Bordrosu  27:Verilen Firma Çeki Ödeme Bordrosu  28:Verilen Firma Senedi Ödeme Bordrosu  29:Açılış Fişi  30:Değerli Kağıtlar Açılış Fişi  31:Borç Dekontu  32:Alacak Dekontu  33:Genel Virman Dekontu  34:Gelen Havale  35:Gonderilen Havale  36:Bankadan Firma Senet ödeme  37:Kasa Masraf Fişi  38:Bankadan Firma Çek Ödeme  39:Protestolu Senet İade Giriş Bordrosu  40:Karşılıksız Çek İade Giriş Bordrosu  41:Mevduat Çek Karşılıksız İade  42:Mevduat Senet prot İade  43:Çek İade Giriş Bordrosu  44:Senet İade Giriş Bordrosu  45:Protestolu Senet İade Çıkış Bordrosu  46:Karşılıksız Çek İade Çıkış Bordrosu  47:Çek İade Çıkış Bordrosu  48:Senet İade Çıkış Bordrosu  49:Kasadan Kendi Ödeme Emrimizi Kapatma  50:Bankadan Kendi Ödeme Emrimizi Kapatma  51:Firma Kredi Kartı Ödeme  52:Kasadan Müşteri Ödeme Sözü Kapatma  53:Bankadan Müşteri Ödeme Sözü Kapatma  54:Cari Hesap Kredi Kartı Ödeme  55:Giriş Gider Makbuzu  56:Giriş Serbest Meslek Makbuzu  57:Müşteri Satıcı Virman Dekontu  58:Bankalar Virman Dekontu  59:Kur Farkı Virman Dekontu  60:Pos Satış Virman Dekontu  61:Stok Gider Pusulası  62:Karşılıksız Portföyden Portföye Transfer  63:Satış Faturası  64:Tediye Makbuzu  65:Kasa Tediye Fişi  66:Senet Çıkış Bordrosu  67:Çek Çıkış Bordrosu  68:Protestolu Portföydeki Senet Karşılığı Nakit  68:Karşılıksız Portföydeki Çek Karşılığı Nakit  70:Kasalar Arası Çek Transfer Bordrosu  71:Kasalar Arası Senet Transfer Bordrosu  72:Kasalar Arası Karşılıksız Çek Transfer Bordrosu  73:Kasalar Arası Protestolu Senet Transfer Bordrosu  74:Karşılıksız Çıkan Çek Transfer Bordrosu  75:Ödenmeyen Senet Transfer Bordrosu  76:Açılış Çek Portföye Giriş Bordrosu  77:Kredi Kartı Masraf Virman Dekontu  78:Bankada Tahsildeki Senedi Cariye İade Bordrosu  79:Bankada Teminattaki Senedi Cariye İade Bordrosu  80:Bankada Tahsildeki Çeki Cariye İade Bordrosu  81:Bankada Teminattaki Çeki Cariye İade Bordrosu  82:Ödeme Emri Giriş Bordrosu  83:Ödeme Emri Çıkış Bordrosu  84:Bankada Tahsildeki Protestolu Senedi Cariye İade Bordrosu  85:Bankada Teminattaki Protestolu Senedi Cariye İade Bordrosu  86:Bankada Tahsildeki Karşılıksız Çeki Cariye İade Bordrosu  87:Bankada Teminattaki Karşılıksız Çeki Cariye İade Bordrosu  88:Satis Serbest Meslek Makbuzu  89:Döviz Alış Belgesi  90:Döviz Satış Belgesi  91:Grup Şirketler Arası Virman Dekontu  92:Firma Havale Emri Kapatma  93:Müşteri Havale Sözü Kapatma  94:Personel Tahakkuk Virman Dekontu  95:İthalat Masraf Yansıtma Dekontu  96:Finansal Kiralama Sözleşme Evrağı  97:Cari Vade Farkı Sıfırlama Fişi  98:Tahsil Edilen Avans Makbuzu  99:Ödenen Avans Makbuzu  100:Cari Borç Dekontu  101:Cari Alacak Dekontu  102:Cari Değerli Kağıt Değerleme Virman Dekontu  103:Müşteri Kredi Kartı İade Çıkış Bordrosu  104:Bankalar Arası Kredi Kartı Transferi  105:Alternatif Döviz Dönüşüm Farkı Virman Dekontu  106:Amortisman Giderleştirme Virman Dekontu  107:Hizmet Maliyeti Yansıtma Virman Dekontu  108:Kredi Sözleşmesi Kabul Fişi  109:Kredi Sözleşmesi Taksit Ödeme Fişi  110:Kasalar Arası Virman Dekontu  111:Kredi Kabul Virman Dekontu  112:Kredi Geri Ödeme Virman Dekontu  113:Kredi Gider Tahakkuku Dekontu  114:Kredi Ana Para Vadesi Değişim Dekontu  115:Kredi Gider Vadesi Değişim Dekontu  116:Dönemsel Hizmet Giderleştirme Gelirleştirme Dekontu  117:Dönemsel Hizmet Gelecek Yıldan Gelecek Aya Aktarma Dekontu  118:Firma Kredi Kartı İade Giriş Bordrosu  119:Teminat Mektubu Giriş Bordrosu  120:Teminat Mektubu Çıkış Bordrosu  121:Depozito Giriş Bordrosu  122:Depozito Çıkış Bordrosu  123:Depozito Çekleri Portföye Transfer  124:Depozito Senetleri Portföye Transfer  125:Teminat Mektubu İade Çıkış Bordrosu  126:Teminat Mektubu İade Giriş Bordrosu  127:Depozito İade Çıkış Bordrosu  128:Depozito İade Giriş Bordrosu  129:Ödendiden Tahsile Müşteri Kredi Kartı İade Bordrosu  130:Firma Reel Kredi Kartı Kesinleştirme Virman Dekontu  131:Firma Reel Kredi Kartı Ödeme Virman Dekontu  132:Müşteri Ödeme Sözü İade Çıkış Bordrosu  133:Kısmen Ödenen Senet Transfer Bordrosu  134:Müşteri Havale Sözü İade Çıkış Bordrosu  135:Kısmen Ödenen Çek Kasaları Arası Transfer  136:Kısmen Ödenen Karşılıksız Çek Kasaları Arası Transfer  137:Stoktan Demirbaşa Virman Dekontu |
| 117 | sip_onodeme_evrak_seri | dbo.evrakseri_str | Ön Ödeme Evrak Seri Numarası |  |
| 118 | sip_onodeme_evrak_sira | Integer | Ön Ödeme Evrak Sıra Numarası |  |
| 119 | sip_rezervasyon_miktari | Float | Rezervasyon Miktarı |  |
| 120 | sip_rezerveden_teslim_edilen | Float | Rezerveden Teslim Edilen Miktar |  |
| 121 | sip_HareketGrupKodu1 | Nvarchar(25) | Hareket Grup Kodu 1 |  |
| 122 | sip_HareketGrupKodu2 | Nvarchar(25) | Hareket Grup Kodu 2 |  |
| 123 | sip_HareketGrupKodu3 | Nvarchar(25) | Hareket Grup Kodu 3 |  |
| 124 | sip_Olcu1 | Float | Ölçü 1 |  |
| 125 | sip_Olcu2 | Float | Ölçü 2 |  |
| 126 | sip_Olcu3 | Float | Ölçü 3 |  |
| 127 | sip_Olcu4 | Float | Ölçü 4 |  |
| 128 | sip_Olcu5 | Float | Ölçü 5 |  |
| 129 | sip_FormulMiktarNo | Tinyint | Formül Numarası |  |
| 130 | sip_FormulMiktar | Float | Formülle Hesaplanan Miktar |  |
| 131 | sip_satis_fiyat_doviz_cinsi | Tinyint | Satış Fiyatı Döviz Cinsi |  |
| 132 | sip_satis_fiyat_doviz_kuru | Float | Satış Fiyatı Döviz Kuru |  |
| 133 | sip_eticaret_kanal_kodu | Nvarchar(25) | e-Ticaret Kanal Kodu |  |
| 134 | sip_Tevkifat_turu | Tinyint | Tevkifat Türü | 0:Yok 1:10'da 3 2:10'da 9 3:21 4:32 5:61 6:45 7:Tam 8:10'da 2 9:10'da 5 10:10'da 7 |
| 135 | sip_otv_tevkifat_turu | Tinyint | ÖTV Tevkifat Türü | 0:Yok 1:Tam |
| 136 | sip_otv_tevkifat_tutari | Float | ÖTV Tevkifat Tutarı |  |
| 137 | sip_tevkifat_sifirlandi_fl | Bit | Tevkifat Tutarı Sıfırlansın Mı ? |  |
| 138 | sip_miktar2 | Float | Bağımsız Miktar |  |
| 139 | sip_avans_tutari | Float | Sipariş Avans Tutarı |  |


Güncellenme Tarihi : 26.11.2024 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**