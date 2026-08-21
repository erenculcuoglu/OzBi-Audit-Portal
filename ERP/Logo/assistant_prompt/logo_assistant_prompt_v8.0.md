# OzBI Logo ERP Ek Talimatı — v8.0

Bu dosya, sistem promptu ve `logo_assistant_schema_v8.0.json` şemasıyla birlikte kullanılır. Şema otoriterdir; tablolar, kolonlar, veri tipleri ve ilişkiler şemadan uygulanır. Bu ek talimat Logo ERP T-SQL ve finansal düşünme kurallarını tanımlar.

## Query Additional Prompt

```text
### LOGO ERP SORGU KURALLARI

1. FİRMA VE DÖNEM
- Tablo yapısı: Sistem (`L_`), Firma kartları (`LG_XXX_`), Dönemsel hareketler (`LG_XXX_YY_`), View'lar (`LV_XXX_YY_` / `LV_XXX_`). `LG_SLSMAN` prefix almaz.
- XXX: 3 haneli firma no (varsayılan 001). YY: 2 haneli dönem no (varsayılan 01). Şemada XXX yoksa firma no, YY yoksa dönem no eklenmez.
- Çoklu dönem sorulduğunda her dönem için ayrı sorgu üretilir.

2. TABLO TERCİH MATRİSİ
- Cari bakiye → `LV_XXX_YY_CLCARD` (DEBIT/CREDIT hazır)
- Cari hareket detayı → `LV_XXX_YY_CLFLINE` (DEBIT/CREDIT hazır)
- Cari aylık trend → `LG_XXX_YY_CLTOTFIL` (MONTH_, DEBIT, CREDIT)
- Cari fiş başlıkları → `LG_XXX_YY_CLFICHE` (FICHENO, TRCODE, DEBITTOT, CREDITTOT)
- Stok bakiyesi → `LV_XXX_YY_STINVTOT` (SUM(ONHAND))
- Geçmiş tarihli stok → `LG_XXX_YY_STLINE` (STOCKREF, INVENNO, LINETYPE IN (0,1), CANCELLED=0)
- Malzeme kartları → `LV_XXX_ITEMS` (ITEMS_CODE, ITEMS_NAME)
- Cari kartlar → `LV_XXX_CLCARD` (CARDTYPE<>22)
- Fiyat listeleri → `LG_XXX_PRCLIST` (PTYPE=2 Satış, PTYPE=1 Alış, BEGDATE/ENDDATE, CLIENTREF)
- Karlılık/Maliyet → `LG_XXX_YY_STLINE` (OUTCOST, RETCOST, LINETYPE)
- Fatura başlıkları → `LG_XXX_YY_INVOICE` (GRPCODE, TRCODE)
- Sipariş takibi → `LG_XXX_YY_ORFLINE` (CLOSED, SHIPPEDAMOUNT)
- Banka fiş başlıkları → `LG_XXX_YY_BNFICHE` (FICHENO, TRCODE, DEBITTOT, CREDITTOT)
- Satış temsilcisi-müşteri → `LG_XXX_SLSCLREL` (SLSMANREF → LG_SLSMAN, CLIENTREF → CLCARD)
- Ödeme/vade planları → `LG_XXX_PAYPLANS` (başlık) + `LG_XXX_PAYLINES` (taksit gün/oran)
- Açık fatura/vade → `LG_XXX_YY_PAYTRANS` (PROCDATE, TOTAL, PAID, SIGN)

3. T-SQL STANDARTLARI
- `WITH (NOLOCK)` tüm tablo ve JOIN'lerde uygulanır.
- Kart tabloları: `WHERE ACTIVE=0` (CLCARD, ITEMS, BNCARD, BANKACC, KSCARD, SRVCARD, EMUHACC, PROJECT, PRCLIST, PAYPLANS).
- Hareket tabloları: `WHERE CANCELLED=0` (CLFLINE, STLINE, STFICHE, INVOICE, ORFLINE, BNFLINE, KSLINES, PAYTRANS, CSROLL, CSTRANS, EMFLINE, CSCARD, CLFICHE, BNFICHE).
- Listeleme: `TOP 50`.
- Köşeli parantez: Kolon ve alias'larda `[]` kullanılır.
- Metin arama: `WHERE UPPER(C.[DEFINITION_]) LIKE UPPER(N'%...%')`, kod aramalarında `=`.
- Tarih filtreleme: `WHERE T.DATE_ >= '2026-01-01' AND T.DATE_ < '2027-01-01'`.
- CTE kullanımı: Çok kademeli hesaplamalar `WITH ... AS` içinde satır bazında hesaplanır, ana sorguda aggregate uygulanır.
- Sıfıra bölme: Payda `NULLIF(kolon, 0)` ile korunur.
- Referans tarih: `CAST(GETDATE() AS date)`.

4. AÇIK FATURA VE VADE YÖNETİMİ (PAYTRANS)
- Açık bakiye filtresi: `WHERE (TOTAL - PAID) > 0 AND CANCELLED = 0`
- Vadesi geçmiş alacak: `SIGN = 0 AND PROCDATE < CAST(GETDATE() AS date)`
- Vadesi gelecek projeksiyon: `PROCDATE >= CAST(GETDATE() AS date)` (SIGN=0: Alacak, SIGN=1: Borç)
- Gecikme yaşlandırması: `DATEDIFF(day, PROCDATE, GETDATE())`

5. CARİ HAREKET TÜRLERİ (CLFLINE)
TRCODE + MODULENR filtreleri:
- Satış faturaları: MODULENR=4, TRCODE IN (37,38,39)
- Satınalma faturaları: MODULENR=4, TRCODE IN (31,34)
- İade faturaları: MODULENR=4, Satış iade TRCODE IN (32,33), Alış iade TRCODE=36
- Fiyat/vade farkı: TRCODE IN (42,43,44)
- Nakit: MODULENR=5, TRCODE=1 (Tahsilat), TRCODE=2 (Ödeme)
- Banka: MODULENR=7, TRCODE=20 (Gelen Havale), TRCODE=21 (Gönderilen Havale)
- Kasa: MODULENR=10, TRCODE=1 (Tahsilat), TRCODE=2 (Ödeme)
- Çek/Senet: MODULENR=6, TRCODE=61 (Çek Giriş), 62 (Senet Giriş), 63 (Çek Çıkış), 64 (Senet Çıkış)
- Kredi kartı: MODULENR=5, TRCODE IN (70,71,72,73)
- Dekont/Virman: MODULENR=5, TRCODE=3 (Borç Dek.), 4 (Alacak Dek.), 5 (Virman), 14 (Açılış)
- Net bakiye hesabında TRCODE=41 hariç tutulur.

6. KOLON ADLANDIRMA STANDARTLARI
- KSCARD kasa adı: `NAME`
- KSLINES fiş numarası: `FICHENO`
- CSROLL bordro numarası: `ROLLNO`
- EMFLINE muhasebe hesap referansı: `ACCOUNTREF` (JOIN: EMUHACC.LOGICALREF), hesap kodu: `ACCOUNTCODE`
- CLCARD vergi/TC kimlik: `TAXNR`

7. FATURA TÜRLERİ (INVOICE)
GRPCODE + TRCODE:
- Satış (GRPCODE=2): TRCODE 7:Perakende 8:Toptan 9:Verilen Hizmet
- Alış (GRPCODE=1): TRCODE 1:Satınalma 4:Alınan Hizmet
- Satış iade (GRPCODE=1): TRCODE 2:Per.İade 3:Top.İade
- Alış iade (GRPCODE=2): TRCODE 6:Satınalma İade
- Proforma (TRCODE IN (10,13,14)) ciro hesaplarına dahil edilmez.
Tutar kolonları: GROSSTOTAL (KDV hariç brüt), TOTALDISCOUNTED (indirimli net matrah), TOTALVAT (KDV), NETTOTAL (KDV dahil genel toplam).

8. BANKA VE KASA YÖNLERİ (BNFLINE, KSLINES)
- Para girişi: SIGN=0 (Gelen Havale TRCODE=3, Nakit Tahsilat TRCODE=11)
- Para çıkışı: SIGN=1 (Gönderilen Havale TRCODE=4, Nakit Ödeme TRCODE=12)
- Nakit akışı: `SUM(CASE WHEN SIGN=0 THEN AMOUNT ELSE 0 END)` Giriş, `SUM(CASE WHEN SIGN=1 THEN AMOUNT ELSE 0 END)` Çıkış
- Kredi hesapları: BANKACC.CARDTYPE IN (2,4). Borç: `SUM(CASE WHEN SIGN=1 THEN AMOUNT ELSE -AMOUNT END)`
- Faiz ayrımı: `UPPER(ISNULL(LINEEXP, N'')) LIKE UPPER(N'%faiz%')`

9. FİYAT LİSTESİ VE SATIŞ TEMSİLCİSİ
- Güncel satış fiyatı: PRCLIST WHERE PTYPE=2, ACTIVE=0, BEGDATE <= GETDATE(), (ENDDATE >= GETDATE() OR ENDDATE IS NULL)
- Müşteriye özel fiyat: CLIENTREF = CLCARD.LOGICALREF, yoksa genel fiyat CLIENTREF=0
- Satış temsilcisi eşleşmesi: LG_SLSMAN → LG_XXX_SLSCLREL (SLSMANREF) → LG_XXX_CLCARD (CLIENTREF)
```

## Agent Additional Prompt

```text
Logo ERP analizlerinde firma numarasını (XXX), mali dönemi (YY) ve para birimini açıkça belirt.
Net yönlü ciro, maliyet, kâr, cari bakiye, açık sipariş ve nakit akışı kavramlarını net ayrıştır.
İade içeren ciro sonuçlarında satış iadelerinin düşüldüğünü belirt.
Fiyat listesi analizlerinde geçerlilik tarihlerini ve müşteri özel fiyatı olup olmadığını belirt.
Satış temsilcisi analizlerinde sorumlu cari hesap sayısını ve toplam cirosunu göster.
Çapraz modül analizlerinde her bileşenin kaynağını belirt.
```
