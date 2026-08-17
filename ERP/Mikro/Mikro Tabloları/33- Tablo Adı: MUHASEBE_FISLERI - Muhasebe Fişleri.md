# TABLO NO: 2

## Tablo Adı: MUHASEBE_FISLERI - Muhasebe Fişleri

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | fis_Guid | Uniqueidentifier |  |  |
| 1 | fis_DBCno | Smallint |  |  |
| 2 | fis_SpecRECno | Integer |  |  |
| 3 | fis_iptal | Bit |  |  |
| 4 | fis_fileid | Smallint |  |  |
| 5 | fis_hidden | Bit |  |  |
| 6 | fis_kilitli | Bit |  |  |
| 7 | fis_degisti | Bit |  |  |
| 8 | fis_checksum | Integer |  |  |
| 9 | fis_create_user | Smallint |  |  |
| 10 | fis_create_date | DateTime |  |  |
| 11 | fis_lastup_user | Smallint |  |  |
| 12 | fis_lastup_date | DateTime |  |  |
| 13 | fis_special1 | Nvarchar(127) |  |  |
| 14 | fis_special2 | Nvarchar(127) |  |  |
| 15 | fis_special3 | Nvarchar(127) |  |  |
| 16 | fis_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | fis_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | fis_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | fis_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | fis_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | fis_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | fis_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | fis_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | fis_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | fis_firmano | Integer | Firma No |  |
| 26 | fis_subeno | Integer | Şube No |  |
| 27 | fis_maliyil | Integer | Mali Yıl |  |
| 28 | fis_tarih | DateTime | Fiş Tarihi |  |
| 29 | fis_sira_no | Integer | Fiş Sıra No |  |
| 30 | fis_tur | Tinyint | Fiş Türü | 0:Mahsup 1:Tahsil 2:Tediye 3:Açılış 4:Kapanış |
| 31 | fis_hesap_kod | Nvarchar(25) | Fiş Hesap Kodu | Bkz. MUHASEBE_HESAP_PLANI |
| 32 | fis_satir_no | Integer | Fiş Satır No |  |
| 33 | fis_aciklama1 | Nvarchar(127) | Açıklama |  |
| 34 | fis_meblag0 | Float | Yerli döviz cinsinden meblağ | 0'dan büyük ise Borç, küçük ise Alacaktır. |
| 35 | fis_meblag1 | Float | Alternatif döviz cinsinden meblağ | 0'dan büyük ise Borç, küçük ise Alacaktır. |
| 36 | fis_meblag2 | Float | Orjinal döviz cinsinden meblağ | 0'dan büyük ise Borç, küçük ise Alacaktır. |
| 37 | fis_meblag3 | Float | Stok hesabı ise ana birimden miktarı | 0'dan büyük ise Borç, küçük ise Alacaktır. |
| 38 | fis_meblag4 | Float | Stok hesabı ise 2. birimden miktarı | 0'dan büyük ise Borç, küçük ise Alacaktır. |
| 39 | fis_meblag5 | Float | Stok hesabı ise 3. birimden miktarı | 0'dan büyük ise Borç, küçük ise Alacaktır. |
| 40 | fis_meblag6 | Float | Stok hesabı ise 4. birimden miktarı | 0'dan büyük ise Borç, küçük ise Alacaktır. |
| 41 | fis_sorumluluk_kodu | Nvarchar(25) | Sorumluluk Merkezi | Bkz. SORUMLULUK_MERKEZLERI |
| 42 | fis_ticari_tip | Tinyint | Ticari Tip Kodu | 0:Ticari ilişki yok 1:Stok Hareket 2:Cari Hareket 3:Sipariş     4:Personel Tahakkuk 5:Akaryakıt hareket 6:Demirbaş Hareket 7:Smm Hareket |
| 43 | fis_ticari_uid | Uniqueidentifier | İlgili Ticari Kayda Bağlı Uid | Bkz. İLGİLİ TİCARİ TABLO |
| 44 | fis_kurfarkifl | Bit | Kur Farkı Var mı? |  |
| 45 | fis_ticari_evraktip | Tinyint | Ticari Evrak Tipi | Bağlı olduğu ticari kayıttaki evrak tipini gösterir.    Bkz. İLGİLİ TİCARİ TABLO |
| 46 | fis_tic_evrak_seri | dbo.evrakseri_str | Ticari Evrak Seri no |  |
| 47 | fis_tic_evrak_sira | Integer | Ticari Evrak Sıra No |  |
| 48 | fis_tic_belgeno | dbo.belgeno_str | Ticari Evrak Belge No |  |
| 49 | fis_tic_belgetarihi | DateTime | Belge Tarihi |  |
| 50 | fis_yevmiye_no | Integer | Fiş Yevmiye No |  |
| 51 | fis_katagori | Smallint | Fiş Kategori |  |
| 52 | fis_evrak_DBCno | Smallint |  |  |
| 53 | fis_fmahsup_tipi | Tinyint | Fiş Mahsup Tipi | 0:Standart Mahsup 1:Yansıtma Hesap Açılış 2:Yansıtma Hesap Kapanışı 3:Dönem Kar Zarar 4:Vergilendirme 5:Peşin Ödenen Vergi 6:Dönem Net Kar Zarar 7:Dönem Net Kar Zarar Devri 8:Açılış Mahsubu 9:Kapanış Mahsubu 10:Dönem İçi Vergilendirme 11:Özel Mahsuplar 12:Dönem Sonu Enflasyon Farkı 13:Önceki Dönem Enflasyon Farkı Maliyete Yansıtma 14:E2003 Sonu Enflasyon Farkı 15:E2003 Sonu Enflasyon Farkı Maliyete Yansıtma 16:Maliyet Dağıtım Mahsubu 17:Satılan Mal Maliyeti Mahsubu 18:Kur Farkı Mahsubu 19:Alternatif Döviz Dönüştürme Farkı Mahsubu 20:Kredi Uzun Vadenin Kısa Vadeye Dönüşmesi Mahsubu 21:Ödeme Emri Reeskont Mahsubu 22:Şüpheli Alacaklar Mahsubu |
| 54 | fis_fozelmahkod | Nvarchar(25) | Özel Mahsup Kodu |  |
| 55 | fis_grupkodu | Nvarchar(4) | Fiş Grup Kodu |  |
| 56 | fis_aktif_pasif | Tinyint | Aktif Fiş - Pasif Fiş | 0:Aktif Fiş 1:Pasif Fiş |
| 57 | fis_proje_kodu | Nvarchar(25) | Fiş Proje Kodu | Bkz. PROJELER |
| 58 | fis_HareketGrupKodu1 | Nvarchar(25) | Hareket Grup Kodu 1 |  |
| 59 | fis_HareketGrupKodu2 | Nvarchar(25) | Hareket Grup Kodu 2 |  |
| 60 | fis_HareketGrupKodu3 | Nvarchar(25) | Hareket Grup Kodu 3 |  |
| 61 | fis_ticari_hubid | Nvarchar(50) | Ticari Hub Id |  |
| 62 | fis_ticari_hubglbid | Nvarchar(50) | Ticari Hub Global Id |  |


Güncellenme Tarihi : 06.12.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**