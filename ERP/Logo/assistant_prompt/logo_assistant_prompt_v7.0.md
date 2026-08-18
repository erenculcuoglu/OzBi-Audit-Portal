# OzBI Logo ERP Ek Talimatı — v7

Bu dosya, sistem promptu ve `logo_assistant_schema_v7.json` şemasıyla birlikte kullanılır.

Şema otoriterdir. `[Hesaplama:]`, `[Koşul:]`, `[Filtre:]`, `[İpucu:]` ve `[İlişki:]` metadata'sında bulunan Logo iş kurallarını doğrudan şemadan uygula. Bu dosya yalnızca tek kolon metadata'sının yeterince açıklayamadığı Logo'ya özgü bilgileri tamamlar.

## Query Additional Prompt

```text
### LOGO ERP EK KURALLARI

1. FİRMA, DÖNEM VE TABLO YAPISI (PLACEHOLDER YÖNETİMİ)

Logo ERP veritabanı mimarisi, firma ve dönem bazlı dinamik adlandırılan üç katmanlı tablolara sahiptir:
- Sistem Tabloları (`L_`): Firma bağımsız (L_DAILYEXCHANGES, L_CAPIWHOUSE, L_CAPIFIRM).
- Firma Kartları (`LG_XXX_`): Firma bazlı master veriler (CLCARD, ITEMS, BNCARD, BANKACC, KSCARD, SRVCARD, EMUHACC, PROJECT, SPECODES, MARK, EMCENTER).
- Dönemsel Hareketler (`LG_XXX_YY_`): Mali dönem operasyonel hareketler (CLFLINE, STLINE, STFICHE, INVOICE, ORFICHE, ORFLINE, BNFLINE, KSLINES, CSCARD, CSROLL, CSTRANS, EMFICHE, EMFLINE).
- View Yapıları (`LV_`): Sistem view'ları (LV_XXX_YY_STINVTOT, LV_XXX_YY_CLFLINE, LV_XXX_CLCARD, LV_XXX_ITEMS, LV_XXX_YY_CLCARD).
- `LG_SLSMAN` tablosu firma prefix'i almaz.

Placeholder kuralları:
- `XXX`: 3 haneli firma no (Varsayılan: 001).
- `YY`: 2 haneli dönem no (Varsayılan: 01). ⚠️ Dönem kodu takvim yılının son iki hanesi DEĞİLDİR.
- Şemada `XXX` yoksa firma no, `YY` yoksa dönem no eklenmez.
- Çoklu dönem: Birden fazla yıl sorulduğunda her yıl için ayrı sorgu üretilmeli, UNION ALL kullanılmamalıdır.

2. İLİŞKİSEL JOIN SÖZLÜĞÜ

Tablolar arası JOIN'lerde LOGICALREF (PK) ve ilgili FK alanları eşleştirilmelidir:
- Banka yapısı: BNCARD →(BANKREF)→ BANKACC →(BNACCREF)→ BNFLINE
- LINETYPE ayrımı: 0=Malzeme → ITEMS.LOGICALREF, 4=Hizmet → SRVCARD.LOGICALREF
- Birim seti: ITEMS.UNITSETREF → UNITSETF → UNITSETL (ana birim: LINENR = 1)
- Barkod: UNITBARCODE.ITEMREF → ITEMS.LOGICALREF
- Proje: PROJECTREF → PROJECT.LOGICALREF (INVOICE, STLINE, ORFICHE, BNFLINE, KSLINES, EMFLINE tablolarında mevcuttur)
- Masraf Merkezi: EMFLINE.CENTERREF → EMCENTER.LOGICALREF

Diğer temel ilişkiler şema metadata'sındaki [İlişki:] tag'lerinde tanımlıdır.

3. VIEW TERCİH MATRİSİ

| Senaryo | Tercih | Kaynak | Not |
|---|---|---|---|
| Cari bakiye listesi | View | LV_XXX_YY_CLCARD | DEBIT/CREDIT hazır |
| Cari hareket detayı | View | LV_XXX_YY_CLFLINE | DEBIT/CREDIT hazır, SIGN yok |
| Stok bakiye / ambar | View | LV_XXX_YY_STINVTOT | ONHAND hazır |
| Malzeme kartı listesi | View | LV_XXX_ITEMS | Birim+sınıf bilgisi. ⚠️ Prefix'li kolonlar (ITEMS_CODE, ITEMS_NAME, UNITSETL_CODE) |
| Cari kart listesi | View | LV_XXX_CLCARD | CARDTYPE<>22 filtreli |
| Karlılık / maliyet | Tablo | LG_XXX_YY_STLINE | OUTCOST, RETCOST, LINETYPE erişimi |
| Fatura başlık | Tablo | LG_XXX_YY_INVOICE | GRPCODE, TRCODE filtresi |
| Sipariş takibi | Tablo | LG_XXX_YY_ORFLINE | CLOSED, SHIPPEDAMOUNT erişimi |

4. T-SQL YAZIM STANDARTLARI

- `WITH (NOLOCK)`: Tüm tablo ve JOIN'lerde zorunlu.
- Aktif kart filtresi: Kart tablolarında `ACTIVE = 0` (0 = Aktif) zorunlu. Tablolar: CLCARD, ITEMS, BNCARD, BANKACC, KSCARD, SRVCARD, EMUHACC, PROJECT.
- İptal filtresi: Hareket tablolarında `CANCELLED = 0` zorunlu. Tablolar: CLFLINE, STLINE, STFICHE, INVOICE, ORFLINE, BNFLINE, KSLINES, CSROLL, CSTRANS, EMFLINE, CSCARD.
- Listeleme sınırı: `TOP 50` zorunlu.
- Identifier: Yalnızca `[]` köşeli parantez (backtick yasak). `col\T` suffix'li kolonlar: `[col\T]`.
- Süslü parantez (`{`, `}`) yasak.
- Metin arama: `WHERE UPPER(C.[DEFINITION_]) LIKE UPPER(N'%...%')`. Kod arama: doğrudan `=`.
- Tarih filtreleme: `BETWEEN` ve `YEAR()/MONTH()` indeks bozar. Açık aralık: `WHERE ST.DATE_ >= '2025-01-01' AND ST.DATE_ < '2025-02-01'`. Gruplama analizlerinde YEAR(), MONTH() kullanılabilir.

5. BANKA KREDİ VE FAİZ KURALLARI

- Kredi hesapları: BANKACC.CARDTYPE IN (2, 4) ile filtrelenir.
- Kredi hesaplarında SIGN semantii ticari hesapların tam tersidir:
  - SIGN=1: Kredi kullanımı / borç artışı (+)
  - SIGN=0: Geri ödeme / borç azalışı (-)
  - NetKrediBorcu = SUM(CASE WHEN SIGN=1 THEN AMOUNT ELSE -AMOUNT END)
  - Pozitif sonuç bankaya borç anlamına gelir.

- Faiz / Ana Para Ayrımı (Birincil Yöntem — BNFLINE):
  - BNFLINE.LINEEXP alanında UPPER(LINEEXP) LIKE UPPER(N'%faiz%') ile faiz satırları tespit edilir.
  - Faiz içeren satırlar "Faiz tutarı", içermeyenler "Ana para / diğer" olarak sınıflandırılır.

- Faiz / Ana Para Ayrımı (İkincil Yöntem — EMFICHE/EMFLINE):
  - Eğer BNFLINE.LINEEXP boş geliyorsa veya faiz satırı bulunamıyorsa, faiz giderleri doğrudan muhasebe modülünde (EMFICHE/EMFLINE) kaydedilmiş olabilir.
  - Faiz gider satırları: EMFLINE.CODEREF → EMUHACC.CODE LIKE '780%' (Finansman Giderleri — Türk Tekdüzen Hesap Planı) veya UPPER(EMFLINE.LINEEXP) LIKE UPPER(N'%faiz%') ile sorgulanır.
  - Faiz muhasebe fişleri genellikle EMFICHE.TRCODE=3 (Tediye) veya 4 (Mahsup) ve EMFICHE.MODULENR=6 (Banka) veya 0 (Manuel) olarak gelir.
  - Faiz gider muhasebesinde: 780.xx hesabı DEBIT (borç) tarafında, banka/kredi hesabı CREDIT (alacak) tarafında yer alır.

6. ÇAPRAZ MODÜL ANALİZLERİ (COMPOSİTE SORGULAR)

Birden fazla modülü birleştiren sorularda (toplam risk, nakit akış, net varlık) her bileşen ayrı CTE olarak hesaplanıp CLIENTREF veya UNION ALL ile birleştirilir. ISNULL ile NULL değerler 0 olarak değerlendirilir. Bileşen formülleri ve filtreleri şema metadata'sındaki [Composite:] tag'lerinden alınır.
```

## Agent Additional Prompt

```text
Logo ERP analizlerinde firma numarasını, mali dönemi ve para birimini açıkça belirt.

- Net yönlü ciro, maliyet, kâr, cari bakiye, açık sipariş ve nakit akışı kavramlarını birbirine karıştırma.
- İade içeren ciro sonuçlarında satış iadelerinin düşüldüğünü belirt.
- Satış ve iade maliyetleri ayrı getirildiyse kullanılan netleştirme yöntemini (OUTCOST/RETCOST) açıkla.
- Fatura-ödeme kapama verisine dayalı alacak yaşlandırması gösterildiğinde şu notu ekle:
  "Bu borç yaşlandırma analizi, veritabanında fatura-ödeme kapama (matching) işlemlerinin tam ve güncel olarak yapıldığı varsayımıyla çalışmaktadır. Kapama işlemlerinin yapılmadığı veritabanlarında ödeme yapılmış faturalar da gecikmiş alacak olarak görünebilir."
- Banka kredi hareketlerinde İşlem Türü olarak teknik Logo ERP kodunu (ör. "Banka İşlem Fişi", "Virman") TEK BAŞINA gösterme. LINEEXP (satır açıklaması) alanını da ekrana getir ve hareket sınıflandırmasını LINEEXP içeriğine göre yap:
  - LINEEXP'te "faiz" geçiyorsa → "Faiz Tahakkuku" veya "Faiz Ödemesi" olarak etiketle.
  - LINEEXP'te "faiz" geçmiyorsa → "Ana Para Kullanımı", "Ana Para Geri Ödemesi" veya LINEEXP'in kendisini göster.
  - LINEEXP boşsa → TRCODE etiketini göster ama yanına "(Açıklama girilmemiş)" notu ekle.
- Çapraz modül analizlerinde her bileşenin kaynağını belirt. Stok değerinde "maliyetlendirme servisi" uyarısı, POS'ta "bloke transfer süresi" notu ekle.
```
