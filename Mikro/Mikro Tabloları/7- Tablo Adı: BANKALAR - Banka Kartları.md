# TABLO NO: 52

## Tablo Adı: BANKALAR - Banka Kartları

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | ban_Guid | Uniqueidentifier |  |  |
| 1 | ban_DBCno | Smallint |  |  |
| 2 | ban_SpecRECNo | Integer |  |  |
| 3 | ban_iptal | Bit |  |  |
| 4 | ban_fileid | Smallint |  |  |
| 5 | ban_hidden | Bit |  |  |
| 6 | ban_kilitli | Bit |  |  |
| 7 | ban_degisti | Bit |  |  |
| 8 | ban_CheckSum | Integer |  |  |
| 9 | ban_create_user | Smallint |  |  |
| 10 | ban_create_date | DateTime |  |  |
| 11 | ban_lastup_user | Smallint |  |  |
| 12 | ban_lastup_date | DateTime |  |  |
| 13 | ban_special1 | Nvarchar(127) |  |  |
| 14 | ban_special2 | Nvarchar(127) |  |  |
| 15 | ban_special3 | Nvarchar(127) |  |  |
| 16 | ban_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | ban_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | ban_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | ban_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | ban_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | ban_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | ban_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | ban_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | ban_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | ban_kod | Nvarchar(25) | Banka Kodu |  |
| 26 | ban_ismi | Nvarchar(50) | Banka İsmi |  |
| 27 | ban_sube | Nvarchar(50) | Banka Şubesi |  |
| 28 | ban_SwiftKodu | Nvarchar(25) | Swift Kodu |  |
| 29 | ban_IBANKodu | Nvarchar(40) | IBAN Kodu |  |
| 30 | ban_hesapno | Nvarchar(30) | Banka Hesap No |  |
| 31 | ban_firma_no | Integer | Firma No |  |
| 32 | ban_hesap_tip | Tinyint | Banka Hesap Tipi | 0:Mevduat 1:Kredi |
| 33 | ban_mevduat_tip | Tinyint | Mevduat Cinsi | 0:Vadesiz ticari 1:Vadesiz tasarruf   2:Vadeli tasarruf 3:Yatırım fonu 4:Repo 5:Bloke 6:Diğer mevduat |
| 34 | ban_kredi_tip | Tinyint | Kredi Cinsi | 0:Kısa vadeli açık kredi   1:Kısa vadeli teminatlı kredi 2:Uzun vadeli açık kredi   3:Uzun vadeli teminatlı kredi 4:Exim kredileri 5:Diğer kredi |
| 35 | ban_muh_kod | Nvarchar(40) | Banka Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 36 | ban_ver_cek_muh_kod | Nvarchar(40) | Verilen Çek Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 37 | ban_tah_cek_muh_kod | Nvarchar(40) | Tahsil Çek Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 38 | ban_tah_sen_muh_kod | Nvarchar(40) | Tahsil Senet Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 39 | ban_tem_cek_muh_kod | Nvarchar(40) | Teminat Çek Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 40 | ban_tem_sen_muh_kod | Nvarchar(40) | Teminat Senet Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 41 | ban_mus_krdrkart_muh_kod | Nvarchar(40) | Müşteri Kredi Kartı Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 42 | ban_frm_krdrkart_muh_kod | Nvarchar(40) | Firma Kredi Kartı Muhasebe Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 43 | ban_must_havale_sozu_muh_kodu | Nvarchar(40) | Müşteri Havale Sözü Muhasebe Kodu |  |
| 44 | ban_firma_havale_emri_muh_kodu | Nvarchar(40) | Firma Havale Emri Muhasebe Kodu |  |
| 45 | ban_tem_muh_kodu | Nvarchar(40) | Banka Teminat Muhasebe Kodu |  |
| 46 | ban_doviz_cinsi | Tinyint | Banka Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 47 | ban_vade_fark_yuzde | Float | Banka Vade Fark Yüzdesi |  |
| 48 | ban_kredi_tavan | Float | Kredi Tavanı |  |
| 49 | ban_risk_tavan | Float | Risk Tavanı |  |
| 50 | ban_nakakincelenmesi | Bit | Nakit Akışta Gözardı Edilecek Mi ? | Bu alanın "evet" olduğu bankalar ile ilgili mevduat    bakiyeleri, nakit akış raporlarında rapora alınmaz. |
| 51 | ban_TCMB_Kodu | Nvarchar(4) | TCMB Kodu |  |
| 52 | ban_TCMB_Sube_Kodu | Nvarchar(8) | TCMB Şube Kodu |  |
| 53 | ban_TCMB_Il_Kodu | Nvarchar(3) | TCMB İl Kodu |  |
| 54 | ban_musteri_no | Nvarchar(30) | Müşteri Numarası |  |
| 55 | ban_Ayni_banka_tahsil_suresi | Tinyint | Aynı Banka Tahsil Süresi |  |
| 56 | ban_baska_banka_tahsil_suresi | Tinyint | Başka Banka Tahsil Süresi |  |
| 57 | ban_odul_katkisi_hesap_cinsi | Tinyint | Ödül Katkısı Hesap Cinsi | 0:Carimiz 1:Cari Personelimiz 2:Bankamız 3:Hizmetimiz 4:Kasamız   5:Giderimiz 6:Muhasebe Hesabımız 7:Personelimiz 8:Demirbaşımız 9:İthalat Dosyamız 10:Finansal Sözleşmemiz 11:Kredi Sözleşmemiz 12:Dönemsel Hizmetimiz 13:Kredi Kartımız |
| 58 | ban_odul_katkisi_hesabi | Nvarchar(25) | Ödül Katkısı Hesabı |  |
| 59 | ban_servis_komisyon_hesap_cinsi | Tinyint | Servis Komisyon Hesap Cinsi | 0:Carimiz 1:Cari Personelimiz 2:Bankamız 3:Hizmetimiz 4:Kasamız   5:Giderimiz 6:Muhasebe Hesabımız 7:Personelimiz 8:Demirbaşımız 9:İthalat Dosyamız 10:Finansal Sözleşmemiz 11:Kredi Sözleşmemiz 12:Dönemsel Hizmetimiz 13:Kredi Kartımız |
| 60 | ban_servis_komisyon_hesabi | Nvarchar(25) | Servis Komisyon Hesabı |  |
| 61 | ban_erken_odm_faiz_hesap_cinsi | Tinyint | Erken Ödeme Faiz Hesap Cinsi | 0:Carimiz 1:Cari Personelimiz 2:Bankamız 3:Hizmetimiz 4:Kasamız   5:Giderimiz 6:Muhasebe Hesabımız 7:Personelimiz 8:Demirbaşımız 9:İthalat Dosyamız 10:Finansal Sözleşmemiz 11:Kredi Sözleşmemiz 12:Dönemsel Hizmetimiz 13:Kredi Kartımız |
| 62 | ban_erken_odm_faiz_hesabi | Nvarchar(25) | Erken Ödeme Faiz Hesabı |  |
| 63 | ban_pos_tahsilat_cari_hesabi | Nvarchar(25) | POS Tahsilat Cari Hesabı |  |
| 64 | ban_adr_cadde | Nvarchar(50) | Cadde |  |
| 65 | ban_adr_mahalle | Nvarchar(50) | Mahalle |  |
| 66 | ban_adr_sokak | Nvarchar(50) | Sokak |  |
| 67 | ban_adr_Semt | Nvarchar(25) | Semt |  |
| 68 | ban_adr_Apt_No | Nvarchar(10) | Apartman Numarası |  |
| 69 | ban_adr_Daire_No | Nvarchar(10) | Daire Numarası |  |
| 70 | ban_adr_posta_kodu | Nvarchar(8) | Posta Kodu |  |
| 71 | ban_adr_ilce | Nvarchar(50) | İlçe |  |
| 72 | ban_adr_il | Nvarchar(50) | İl |  |
| 73 | ban_adr_ulke | Nvarchar(50) | Ülke |  |
| 74 | ban_adr_adres_kodu | Nvarchar(10) | Adres Kodu |  |
| 75 | ban_tel_ulke_kodu | Nvarchar(5) | Ülke Telefon Kodu |  |
| 76 | ban_tel_bolge_kodu | Nvarchar(5) | Bölge Telefon Kodu |  |
| 77 | ban_tel_no1 | Nvarchar(10) | Telefon No 1 |  |
| 78 | ban_tel_no2 | Nvarchar(10) | Telefon No 2 |  |
| 79 | ban_tel_faxno | Nvarchar(10) | Fax No |  |
| 80 | ban_tel_modem | Nvarchar(10) | Modem No |  |
| 81 | ban_temsilci | Nvarchar(50) | Temsilci |  |
| 82 | ban_temsilci_email | Nvarchar(80) | Temsilci e-Postası |  |
| 83 | ban_ufrs_muh_kod | Nvarchar(40) | Ufrs Muhasebe Kodu |  |
| 84 | ban_ufrs_ver_cek_muh_kod | Nvarchar(40) | Ufrs Verilen Çek Muhasebe Kodu |  |
| 85 | ban_ufrs_tah_cek_muh_kod | Nvarchar(40) | Ufrs Tahsil Çek Muhasebe Kodu |  |
| 86 | ban_ufrs_tah_sen_muh_kod | Nvarchar(40) | Ufrs Tahsil Senet Muhasebe Kodu |  |
| 87 | ban_ufrs_tem_cek_muh_kod | Nvarchar(40) | Ufrs Teminat Çek Muhasebe Kodu |  |
| 88 | ban_ufrs_tem_sen_muh_kod | Nvarchar(40) | Ufrs Teminat Senet Muhasebe Kodu |  |
| 89 | ban_ufrs_mus_krdrkart_muh_kod | Nvarchar(40) | Ufrs Müşteri Kredi Kartı Muhasebe Kodu |  |
| 90 | ban_ufrs_frm_krdrkart_muh_kod | Nvarchar(40) | Ufrs Firma Kredi Kartı Muhasebe Kodu |  |
| 91 | ban_ufrs_must_havale_sozu_muh_kodu | Nvarchar(40) | Ufrs Müşteri Havale Sözü Muhasebe Kodu |  |
| 92 | ban_ufrs_firma_havale_emri_muh_kodu | Nvarchar(40) | Ufrs Firma Havale Emri Muhasebe Kodu |  |
| 93 | ban_ufrs_ver_cek_sinif_muh_kod | Nvarchar(40) | Ufrs Verilen Çek Sınıf Muhasebe Kodu |  |
| 94 | ban_ufrs_frm_hav_emri_sinif_muh_kodu | Nvarchar(40) | Ufrs Firma Havale Emri Sınıf Muhasebe Kodu |  |
| 95 | ban_ufrs_tem_muh_kodu | Nvarchar(40) | Ufrs Banka Teminat Muhasebe Kodu |  |
| 96 | ban_online_entegrasyon_durumu | Tinyint | Banka Online Entegrasyon Durumu | 0:Yok 1:Onay Bekleniyor 2:Aktif 3:Pasif |
| 97 | ban_online_entegrasyon_ID | Bigint | Banka Online Entegrasyon ID |  |
| 98 | ban_online_son_guncelleme | DateTime | Banka Online Son Güncelleme Zamanı |  |


Güncellenme Tarihi : 29.11.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**