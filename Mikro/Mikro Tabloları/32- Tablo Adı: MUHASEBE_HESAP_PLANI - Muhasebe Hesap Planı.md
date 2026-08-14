# TABLO NO: 1

## Tablo Adı: MUHASEBE_HESAP_PLANI - Muhasebe Hesap Planı

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | muh_Guid | Uniqueidentifier |  |  |
| 1 | muh_DBCno | Smallint |  |  |
| 2 | muh_SpecRECno | Integer |  |  |
| 3 | muh_iptal | Bit |  |  |
| 4 | muh_fileid | Smallint |  |  |
| 5 | muh_hidden | Bit |  |  |
| 6 | muh_kilitli | Bit |  |  |
| 7 | muh_degisti | Bit |  |  |
| 8 | muh_checksum | Integer |  |  |
| 9 | muh_create_user | Smallint |  |  |
| 10 | muh_create_date | DateTime |  |  |
| 11 | muh_lastup_user | Smallint |  |  |
| 12 | muh_lastup_date | DateTime |  |  |
| 13 | muh_special1 | Nvarchar(127) |  |  |
| 14 | muh_special2 | Nvarchar(127) |  |  |
| 15 | muh_special3 | Nvarchar(127) |  |  |
| 16 | muh_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | muh_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | muh_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | muh_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | muh_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | muh_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | muh_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | muh_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | muh_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | muh_hesap_kod | Nvarchar(25) | Hesap Kodu |  |
| 26 | muh_hesap_isim1 | dbo.nvarchar_maxhesapisimno | Hesap Adı |  |
| 27 | dbo.nvarchar_maxhesapisimno | Nvarchar(40) | Yabancı Hesap Adı |  |
| 28 | muh_hesap_tip | Tinyint | Hesap Tipi | 0:Aktif 1:Pasif 2:Gelir 3:Gider 4:Nazım |
| 29 | muh_doviz_cinsi | Tinyint | Hesap Orj. Döviz Cinsi |  |
| 30 | muh_kurfarki_fl | Bit | Kur Farki Hesabı Mı? |  |
| 31 | muh_sorum_merk | Tinyint | Sorumluluk Merkezi | 0:Serbest 1:Gereksiz 2:Gerekli |
| 32 | muh_kilittarihi | DateTime | Kilit Tarihi |  |
| 33 | muh_hes_dav_bicimi | Tinyint | Hesap Davranış Biçimi | 0:Parasal 1:Parasal Olmayan |
| 34 | muh_kdv_tipi | Tinyint | KDV Tipi |  |
| 35 | muh_calisma_sekli | Tinyint | Çalışma Şekli | 0:Borç 1:Alacak 2:Borç-Alacak |
| 36 | muh_maliyet_dagitim_sekli | Tinyint | Maliyet Dağıtım Şekli | 0:Süreye göre 1:Miktara göre 2:Ağırlığa göre 3:Alana göre 4:Hacme göre 5:Adam saate göre 6:Miktar 2'ye göre 7:Miktar3'e göre 8:Miktar4'e göre 9:Enerji1'e göre 10:Enerji2'ye göre 11:Miktar bölü safha sayısına göre 12:Miktar bölü safha sayısı çarpı standart maliyete göre |
| 37 | muh_grupkodu | Nvarchar(4) | Grup Kodu |  |
| 38 | muh_enf_fark_maliyet_fl | Bit | Enflasyon Fark Maliyetli Mi? |  |
| 39 | muh_kdv_dagitim_sekli | Tinyint | KDV Dağıtım Şekli | 0:Kdv Manuel 1:Kdv Ayır 2:Kdv Hesapla 3:Kdv Ayır İade 4:Kdv Hesapla İade |
| 40 | muh_miktar_oto_fl | Bit | Fişte Otomatik F8 Girişi |  |
| 41 | muh_ticariden_bilgi_girisi_fl | Bit | Hesap Kartlarına Ticariden Bilgi Giriş Yapılsın Mı? |  |
| 42 | muh_proje_detayi | Tinyint | Proje Detayı | 0:Serbest 1:Gereksiz 2:Geçerli |
| 43 | muh_kesin_mizan_hesap_kodu | Nvarchar(25) | Kesin Mizan Hesap Kodu |  |
| 44 | muh_enf_calisma_sekli | Tinyint | Enflasyon Düzeltmesinde Çalışma Şekli | 0:Davranış Biçimine Göre 1:Düzeltmeye Tabi Değil 2:Ön Hazırlıkta Sıfırlanır |
| 45 | muh_yansitma_hesap_kodu | Nvarchar(25) | Yansıtma Hesap Kodu |  |


Güncellenme Tarihi : 09.08.2024 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**