# TABLO NO: 54

## Tablo Adı: ODEME_EMIRLERI - Senet Çek Kartları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | sck_Guid | Uniqueidentifier |  |  |
| 1 | sck_DBCno | Smallint |  |  |
| 2 | sck_SpecRECno | Integer |  |  |
| 3 | sck_iptal | Bit |  |  |
| 4 | sck_fileid | Smallint |  |  |
| 5 | sck_hidden | Bit |  |  |
| 6 | sck_kilitli | Bit |  |  |
| 7 | sck_degisti | Bit |  |  |
| 8 | sck_checksum | Integer |  |  |
| 9 | sck_create_user | Smallint |  |  |
| 10 | sck_create_date | DateTime |  |  |
| 11 | sck_lastup_user | Smallint |  |  |
| 12 | sck_lastup_date | DateTime |  |  |
| 13 | sck_special1 | Nvarchar(127) |  |  |
| 14 | sck_special2 | Nvarchar(127) |  |  |
| 15 | sck_special3 | Nvarchar(127) |  |  |
| 16 | sck_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | sck_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | sck_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | sck_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | sck_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | sck_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | sck_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | sck_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | sck_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | sck_firmano | Integer | Firma No |  |
| 26 | sck_subeno | Integer | Şube No |  |
| 27 | sck_tip | Tinyint | Senet Çek Tipi | 0:Müşteri Çeki 1:Müşteri Senedi 2:Kendi Çekimiz 3:Kendi Senedimiz   4:Müşteri Havale Sözü 5:Müşteri Ödeme Sözü 6:Müşteri Kredi Kartı   7:Kendi Havale Emrimiz 8:Kendi Ödeme Emrimiz 9:Kendi Kredi Kartımız 10:Müşteri Teminat Mektubu 11:Firma Teminat Mektubu 12:Depozito Çeki 13:Depozito Senedi |
| 28 | sck_refno | Nvarchar(25) | Senet Çek Referans No |  |
| 29 | sck_bankano | Nvarchar(25) | Banka No | Bkz. BANKALAR |
| 30 | sck_borclu | Nvarchar(50) | Borçlu Adı |  |
| 31 | sck_vdaire_no | Nvarchar(40) | Vergi Daire Numarası |  |
| 32 | sck_vade | DateTime | Vade Tarihi |  |
| 33 | sck_tutar | Float | Tutar |  |
| 34 | sck_doviz | Tinyint | Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 35 | sck_odenen | Float | Ödenen Miktar |  |
| 36 | sck_degerleme_islendi | Tinyint | Değerleme İşlendi Mi ? | 0:Kur farkı değerlenmedi 1:Kur farkı hesaba işlendi |
| 37 | sck_banka_adres1 | Nvarchar(50) | Banka Adresi |  |
| 38 | sck_sube_adres2 | Nvarchar(50) | Şube Adresi |  |
| 39 | sck_borclu_tel | Nvarchar(15) | Borçlı Telefon Numarası |  |
| 40 | sck_hesapno_sehir | Nvarchar(30) | Hesap No (Şehir) |  |
| 41 | sck_no | Nvarchar(25) | Senet Çek No |  |
| 42 | sck_duzen_tarih | DateTime | Düzenlenme Tarihi |  |
| 43 | sck_sahip_cari_cins | Tinyint | Senet-Çek Sahibi Cari Cinsi | 0:Carimiz 1:Cari Personelimiz 2:Bankamız 3:Hizmetimiz 4:Kasamız   5:Giderimiz 6:Muhasebe Hesabımız 7:Personelimiz 8:Demirbaşımız 9:İthalat Dosyamız 10:Finansal Sözleşmemiz 11:Kredi Sözleşmemiz 12:Dönemsel Hizmetimiz 13:Kredi Kartımız |
| 44 | sck_sahip_cari_kodu | Nvarchar(25) | Senet-Çek Sahibi Cari Kodu | Bkz. CARI_HESAPLAR |
| 45 | sck_sahip_cari_grupno | Tinyint | Senet-Çek Sahibi Grup No |  |
| 46 | sck_nerede_cari_cins | Tinyint | Nerede Cari Cinsi | 0:Carimiz 1:Cari Personelimiz 2:Bankamız 3:Hizmetimiz 4:Kasamız   5:Giderimiz 6:Muhasebe Hesabımız 7:Personelimiz 8:Demirbaşımız 9:İthalat Dosyamız 10:Finansal Sözleşmemiz 11:Kredi Sözleşmemiz 12:Dönemsel Hizmetimiz 13:Kredi Kartımız |
| 47 | sck_nerede_cari_kodu | Nvarchar(25) | Nerede Cari Kodu |  |
| 48 | sck_nerede_cari_grupno | Tinyint | Nerede Grup No |  |
| 49 | sck_ilk_hareket_tarihi | DateTime | İlk Hareket Tarihi |  |
| 50 | sck_ilk_evrak_seri | dbo.evrakseri_str | İlk Evrak Seri No |  |
| 51 | sck_ilk_evrak_sira_no | Integer | İlk Evrak Sıra No |  |
| 52 | sck_ilk_evrak_satir_no | Integer | İlk Evrak Satır No |  |
| 53 | sck_son_hareket_tarihi | DateTime | Son Hareket Tarihi |  |
| 54 | sck_doviz_kur | Float | Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 55 | sck_sonpoz | Tinyint | Senet Çek Pozisyonu | 0:Portföyde 1:Ciro 2:Tahsilde 3:Teminatta 4:İade Edilen 5:Diğer3 6:Ödenmedi Portföyde 7:Ödenmedi İade 8:İcrada 9:Kısmen Ödendi 10:Ödendi |
| 56 | sck_imza | Tinyint | İmza Sahibi | 0:Kendisi 1:Müşterisi |
| 57 | sck_srmmrk | Nvarchar(25) | Sorumluluk Merkezi | Bkz. SORUMLULUK_MERKEZLERI |
| 58 | sck_kesideyeri | Nvarchar(14) | Keşide Yeri |  |
| 59 | Sck_TCMB_Banka_kodu | Nvarchar(4) | TCMB Banka Kodu |  |
| 60 | Sck_TCMB_Sube_kodu | Nvarchar(8) | TCMB Şube Kodu |  |
| 61 | Sck_TCMB_İL_kodu | Nvarchar(3) | TCMB İl Kodu |  |
| 62 | SckTasra_fl | Bit | Senet Çek Taşra Mı ? |  |
| 63 | sck_projekodu | Nvarchar(25) | Proje Kodu |  |
| 64 | sck_masraf1 | Float | Masraf1 |  |
| 65 | sck_masraf1_isleme | Tinyint | Masraf1 İşleme | 0:Müşteri Ödeyecek 1:Masrafa İşlenecek |
| 66 | sck_masraf2 | Float | Masraf2 |  |
| 67 | sck_masraf2_isleme | Tinyint | Masraf2 İşleme | 0:Müşteri Ödeyecek 1:Masrafa İşlenecek |
| 68 | sck_odul_katkisi_tutari | Float | Ödül Katkısı Tutarı |  |
| 69 | sck_servis_komisyon_tutari | Float | Servis Komisyon Tutarı |  |
| 70 | sck_erken_odeme_faiz_tutari | Float | Erken Ödeme Faiz Tutarı |  |
| 71 | sck_odul_katkisi_tutari_islendi_fl | Bit | Ödül Katkısı Tutarı İşlendi Mi ? |  |
| 72 | sck_servis_komisyon_tutari_islendi_fl | Bit | Servis Komisyon Tutarı İşlendi Mi ? |  |
| 73 | sck_erken_odeme_faiz_tutari_islendi_fl | Bit | Erken Ödeme Faiz Tutarı İşlendi Mi ? |  |
| 74 | sck_kredi_karti_tipi | Tinyint | Kredi Kartı Tipi | 0:Kendi Kredi Kartı 1:Başka Banka Kredi Kartı 2:Bonus Puan Kullanımı |
| 75 | sck_taksit_sayisi | Smallint | Taksit Sayısı |  |
| 76 | sck_kacinci_taksit | Smallint | Kaçıncı Taksit |  |
| 77 | sck_uye_isyeri_no | Nvarchar(25) | Üye İş Yeri No |  |
| 78 | sck_kredi_karti_no | Nvarchar(16) | Kredi Kartı No |  |
| 79 | sck_provizyon_kodu | Nvarchar(10) | Provizyon Kodu |  |


Güncellenme Tarihi : 06.12.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**