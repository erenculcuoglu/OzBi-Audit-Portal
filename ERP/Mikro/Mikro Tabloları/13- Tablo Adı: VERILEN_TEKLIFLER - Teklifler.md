# TABLO NO: 100

## Tablo Adı: VERILEN_TEKLIFLER - Teklifler

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | tkl_Guid | Uniqueidentifier |  |  |
| 1 | tkl_DBCno | Smallint |  |  |
| 2 | tkl_SpecRECno | Integer |  |  |
| 3 | tkl_iptal | Bit |  |  |
| 4 | tkl_fileid | Smallint |  |  |
| 5 | tkl_hidden | Bit |  |  |
| 6 | tkl_kilitli | Bit |  |  |
| 7 | tkl_degisti | Bit |  |  |
| 8 | tkl_checksum | Integer |  |  |
| 9 | tkl_create_user | Smallint |  |  |
| 10 | tkl_create_date | DateTime |  |  |
| 11 | tkl_lastup_user | Smallint |  |  |
| 12 | tkl_lastup_date | DateTime |  |  |
| 13 | tkl_special1 | Nvarchar(127) |  |  |
| 14 | tkl_special2 | Nvarchar(127) |  |  |
| 15 | tkl_special3 | Nvarchar(127) |  |  |
| 16 | tkl_MainProgramNo | Smallint | Ana Program Numarası |  |
| 17 | tkl_VersionNo | Nvarchar(10) | Versiyon Numarası |  |
| 18 | tkl_MenuNo | Nvarchar(10) | Menü Numarası |  |
| 19 | tkl_MikroSpecial1 | Nvarchar(40) | Özel Alan |  |
| 20 | tkl_MikroSpecial2 | Nvarchar(40) | Özel Alan |  |
| 21 | tkl_MikroSpecial3 | Nvarchar(40) | Özel Alan |  |
| 22 | tkl_ExternalProgramType | Tinyint | Dış Yazılım Aktarım Tipi | 0:Genel 1:Paraşüt 2:Irgat |
| 23 | tkl_ExternalProgramId | Nvarchar(127) | Dış Yazılım Id'si |  |
| 24 | tkl_Hash | Bigint | Kayıt Özel Anahtarı |  |
| 25 | tkl_firmano | Integer | Firma No |  |
| 26 | tkl_subeno | Integer | Şube No |  |
| 27 | tkl_stok_kod | Nvarchar(25) | Stok Kodu | Bkz. STOKLAR |
| 28 | tkl_cari_kod | Nvarchar(25) | Cari Kodu | Bkz. CARI_HESAPLAR |
| 29 | tkl_evrakno_seri | dbo.evrakseri_str | Evrak Seri No | Bkz. EVRAK_ACIKLAMALARI |
| 30 | tkl_evrakno_sira | Integer | Evrak Sıra No | Bkz. EVRAK_ACIKLAMALARI |
| 31 | tkl_evrak_tarihi | DateTime | Evrak Tarihi |  |
| 32 | tkl_satirno | Integer | Satır No |  |
| 33 | tkl_belge_no | dbo.belgeno_str | Belge No |  |
| 34 | tkl_belge_tarih | DateTime | Belge Tarihi |  |
| 35 | tkl_asgari_miktar | Float | Asgari Miktar |  |
| 36 | tkl_teslimat_suresi | Smallint | Teslimat Süresi |  |
| 37 | tkl_baslangic_tarihi | DateTime | Başlangıç Tarihi |  |
| 38 | tkl_Gecerlilik_Sures | DateTime | Geçerlilik Süresi |  |
| 39 | tkl_Brut_fiyat | Float | Brüt Fiyat |  |
| 40 | tkl_Odeme_Plani | Integer | Ödeme Planı |  |
| 41 | tkl_Alisfiyati | Float | Alış Fiyatı |  |
| 42 | tkl_karorani | Float | Kar Oranı |  |
| 43 | tkl_miktar | Float | Miktar |  |
| 44 | tkl_Aciklama | Nvarchar(40) | Açıklama |  |
| 45 | tkl_doviz_cins | Tinyint | Döviz Cinsi | Bkz. DOVIZ_KURLARI |
| 46 | tkl_doviz_kur | Float | Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 47 | tkl_alt_doviz_kur | Float | Alternatif Döviz Kuru | Bkz. DOVIZ_KURLARI |
| 48 | tkl_iskonto1 | Float | İskonto |  |
| 49 | tkl_iskonto2 | Float | İskonto |  |
| 50 | tkl_iskonto3 | Float | İskonto |  |
| 51 | tkl_iskonto4 | Float | İskonto |  |
| 52 | tkl_iskonto5 | Float | İskonto |  |
| 53 | tkl_iskonto6 | Float | İskonto |  |
| 54 | tkl_masraf1 | Float | Masraf |  |
| 55 | tkl_masraf2 | Float | Masraf |  |
| 56 | tkl_masraf3 | Float | Masraf |  |
| 57 | tkl_masraf4 | Float | Masraf |  |
| 58 | tkl_vergi_pntr | Tinyint | İlgili Vergi Bağlantısı |  |
| 59 | tkl_vergi | Float | İlgili Vergi Oranı |  |
| 60 | tkl_masraf_vergi_pnt | Tinyint | İlgili Masraf Vergi Bağlantısı |  |
| 61 | tkl_masraf_vergi | Float | İlgili Masraf Vergi Oranı |  |
| 62 | tkl_isk_mas1 | Tinyint | İskonto Masrafı |  |
| 63 | TKL_ISK_MAS2 | Tinyint | İskonto Masrafı |  |
| 64 | TKL_ISK_MAS3 | Tinyint | İskonto Masrafı |  |
| 65 | TKL_ISK_MAS4 | Tinyint | İskonto Masrafı |  |
| 66 | TKL_ISK_MAS5 | Tinyint | İskonto Masrafı |  |
| 67 | TKL_ISK_MAS6 | Tinyint | İskonto Masrafı |  |
| 68 | TKL_ISK_MAS7 | Tinyint | İskonto Masrafı |  |
| 69 | TKL_ISK_MAS8 | Tinyint | İskonto Masrafı |  |
| 70 | TKL_ISK_MAS9 | Tinyint | İskonto Masrafı |  |
| 71 | TKL_ISK_MAS10 | Tinyint | İskonto Masrafı |  |
| 72 | TKL_SAT_ISKMAS1 | Bit | Satış İskonto Masrafı |  |
| 73 | TKL_SAT_ISKMAS2 | Bit | Satış İskonto Masrafı |  |
| 74 | TKL_SAT_ISKMAS3 | Bit | Satış İskonto Masrafı |  |
| 75 | TKL_SAT_ISKMAS4 | Bit | Satış İskonto Masrafı |  |
| 76 | TKL_SAT_ISKMAS5 | Bit | Satış İskonto Masrafı |  |
| 77 | TKL_SAT_ISKMAS6 | Bit | Satış İskonto Masrafı |  |
| 78 | TKL_SAT_ISKMAS7 | Bit | Satış İskonto Masrafı |  |
| 79 | TKL_SAT_ISKMAS8 | Bit | Satış İskonto Masrafı |  |
| 80 | TKL_SAT_ISKMAS9 | Bit | Satış İskonto Masrafı |  |
| 81 | TKL_SAT_ISKMAS10 | Bit | Satış İskonto Masrafı |  |
| 82 | TKL_VERGISIZ_FL | Bit | Vergisiz? | 0:Hayır 1:Evet |
| 83 | TKL_KAPAT_FL | Bit | Kapatılsın Mı ? | 0:Hayır 1:Evet |
| 84 | TKL_TESLIMTURU | Nvarchar(4) | Teslim Türü |  |
| 85 | tkl_ProjeKodu | Nvarchar(25) | Proje Kodu |  |
| 86 | tkl_Sorumlu_Kod | Nvarchar(25) | Sorumlu Kodu |  |
| 87 | tkl_adres_no | Integer | Adres Numarası |  |
| 88 | tkl_yetkili_uid | Uniqueidentifier | Yetkili Uid |  |
| 89 | tkl_durumu | Tinyint | Durumu | 0:Stoktan sevk edilecek 1:Üretilecek 2:Satın alınacak 3:Stoktan rezervasyon ile sevk edilecek |
| 90 | tkl_TedarikEdilecekCari | Nvarchar(25) | Tedarik Edilecek Cari |  |
| 91 | tkl_fiyat_liste_no | Integer | Fiyat Liste No |  |
| 92 | tkl_Birimfiyati | Float | Birim Fiyatı |  |
| 93 | tkl_paket_kod | Nvarchar(25) | Paket Kodu |  |
| 94 | tkl_teslim_miktar | Float | Teslim Edilen Miktar |  |
| 95 | tkl_OnaylayanKulNo | Smallint | Onaylayan Kullanıcı No |  |
| 96 | tkl_cagrilabilir_fl | Bit | Çağrılabilir Mi ? |  |
| 97 | tkl_harekettipi | Tinyint | Hareket Tipi | 0:Stok 1:Hizmet 2:Gider 3:Demirbaş |
| 98 | tkl_cari_sormerk | Nvarchar(25) | Cari Sorumluluk Merkezi Kodu |  |
| 99 | tkl_stok_sormerk | Nvarchar(25) | Stok Sorumluluk Merkezi Kodu |  |
| 100 | tkl_kapatmanedenkod | Nvarchar(25) | Kapatma Nedeni Kodu |  |
| 101 | tkl_servisisemrikodu | Nvarchar(25) | Servis İş Emri Kodu |  |
| 102 | tkl_birim_pntr | Tinyint | Birim |  |
| 103 | tkl_cari_tipi | Tinyint | Cari Tipi | 0:Cari 1:Aday |
| 104 | tkl_HareketGrupKodu1 | Nvarchar(25) | Hareket Grup Kodu 1 |  |
| 105 | tkl_HareketGrupKodu2 | Nvarchar(25) | Hareket Grup Kodu 2 |  |
| 106 | tkl_HareketGrupKodu3 | Nvarchar(25) | Hareket Grup Kodu 3 |  |
| 107 | tkl_Olcu1 | Float | Ölçü 1 |  |
| 108 | tkl_Olcu2 | Float | Ölçü 2 |  |
| 109 | tkl_Olcu3 | Float | Ölçü 3 |  |
| 110 | tkl_Olcu4 | Float | Ölçü 4 |  |
| 111 | tkl_Olcu5 | Float | Ölçü 5 |  |
| 112 | tkl_FormulMiktarNo | Tinyint | Formüllü Miktar Numarası |  |
| 113 | tkl_FormulMiktar | Float | Formüllü Miktar |  |
| 114 | tkl_Tevkifat_turu | Tinyint | Tevkifat Türü | 0:Yok 1:10'da 3 2:10'da 9 3:21 4:32 5:61 6:45 7:Tam 8:10'da 2 9:10'da 5 10:10'da 7 |
| 115 | tkl_tevkifat_sifirlandi_fl | Bit | Tevkifat Tutarı Sıfırlansın Mı ? |  |


Güncellenme Tarihi : 23.11.2023 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**