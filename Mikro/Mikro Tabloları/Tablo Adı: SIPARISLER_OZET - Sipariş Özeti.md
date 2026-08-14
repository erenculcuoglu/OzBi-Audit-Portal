# TABLO NO: 1181

## Tablo Adı: SIPARISLER_OZET - Sipariş Özeti

| No | Alan Adı | Tip | Açıklama | Detay |
|  --- | --- | --- | --- | --- |
| 0 | so_RECno | Integer IDENTITY |  |  |
| 1 | so_firmano | Integer | Firma No |  |
| 2 | so_subeno | Integer | Şube No |  |
| 3 | so_Tipi | Tinyint | Hareket Tipi | 0:Stok 1:Hizmet 2:Gider 3:Demirbaş |
| 4 | so_Kodu | Nvarchar(25) | Stok Kodu |  |
| 5 | so_SrmMerkezi | Nvarchar(25) | Sorumluluk Merkezi |  |
| 6 | so_ProjeKodu | Nvarchar(25) | Proje Kodu |  |
| 7 | so_Depo | Integer | Depo |  |
| 8 | so_MaliYil | Integer | Mali Yıl |  |
| 9 | so_Donem | Tinyint | Dönem |  |
| 10 | so_HareketCins | Tinyint | Hareket Cinsi | 0:Normal Sipariş 1:Konsinye Sipariş 2:Proforma Sipariş   3:Dış Ticaret Siparişi 4:Fason Siparişi 5:Dahili Sarf Siparişi 6:Depolar Arası Sipariş 7:Satın Alma Talebi 8:Üretim Talebi 9:İş Emirleri 10:Fason Talebi |
| 11 | so_TalepMiktar | Float | Talep Edilen Miktar |  |
| 12 | so_TalepKarsilanan | Float | Karşılanan Talep |  |
| 13 | so_TalepKapanan | Float | Kapanan Talep |  |
| 14 | so_TeminMiktar | Float | Temin Edilen Miktar |  |
| 15 | so_TeminKarsilanan | Float | Karşılanan Temin |  |
| 16 | so_TeminKapanan | Float | Kapanan Temin |  |


Güncellenme Tarihi : 23.08.2024 - Bu doküman ile ilgili
**©2025 Mikro Yazılımevi A.Ş. Tüm Hakları Saklıdır.**