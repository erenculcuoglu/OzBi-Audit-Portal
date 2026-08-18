# OzBI Logo ERP Ek Talimatı — v7.3 (Yalın & Pozitif Mimari)

Bu dosya, sistem promptu ve `logo_assistant_schema_v7.2.json` şemasıyla birlikte kullanılır.

Şema otoriterdir. Tablolar arası ilişkiler (`[İlişki:]`), kolon formülleri (`[Hesaplama:]`), filtreler (`[Filtre:]`, `[Koşul:]`) ve ipuçları (`[İpucu:]`) doğrudan şemadan uygulanır. Bu ek talimat sistemik T-SQL ve iş mantığı kurallarını tanımlar.

## Query Additional Prompt

```text
### LOGO ERP İŞ MANTIĞI VE SORGU KURALLARI

1. FİRMA, DÖNEM VE TABLO YAPISI (PLACEHOLDER YÖNETİMİ)
- Tablo İsimlendirmesi: Sistem tabloları (`L_`), Firma kartları (`LG_XXX_`), Dönemsel hareketler (`LG_XXX_YY_`), View'lar (`LV_XXX_YY_` veya `LV_XXX_`). `LG_SLSMAN` prefix almaz.
- Placeholder Yönetimi: `XXX`: 3 haneli firma no (Varsayılan: 001). `YY`: 2 haneli dönem no (Varsayılan: 01; takvim yılı sonu değildir). Şemada `XXX` yoksa firma no, `YY` yoksa dönem no eklenmez.
- Çoklu Dönem: Birden fazla yıl/dönem sorulduğunda her dönem için ayrı sorgu üretilir.

2. VIEW VE TABLO TERCİH MATRİSİ
- Cari Bakiye: `LV_XXX_YY_CLCARD` (DEBIT / CREDIT kolonları hazır).
- Cari Hareket Detayı: `LV_XXX_YY_CLFLINE` (DEBIT / CREDIT hazır, SIGN gerekmez).
- Güncel Stok Bakiyesi / Ambar: `LV_XXX_YY_STINVTOT` (ONHAND alanı kullanılır; büyük view taraması yapılmaz).
- Geçmiş Tarihli Stok: `LG_XXX_YY_STLINE` tablosu üzerinde `WITH (NOLOCK)`, `STOCKREF`, `INVENNO`, `LINETYPE IN (0, 1)` ve `CANCELLED = 0` indeksli alanlarıyla filtrelenir.
- Malzeme Kartları: `LV_XXX_ITEMS` (`ITEMS_CODE`, `ITEMS_NAME`).
- Cari Kartlar: `LV_XXX_CLCARD` (CARDTYPE <> 22 filtreli).
- Karlılık & Maliyet: `LG_XXX_YY_STLINE` (OUTCOST, RETCOST, LINETYPE).
- Fatura Başlıkları: `LG_XXX_YY_INVOICE` (GRPCODE, TRCODE).
- Sipariş Takibi: `LG_XXX_YY_ORFLINE` (CLOSED, SHIPPEDAMOUNT).

3. T-SQL YAZIM STANDARTLARI
- `WITH (NOLOCK)`: Tüm tablo ve JOIN'lerde uygulanır.
- Aktif Kart Filtresi: Kart tablolarında `ACTIVE = 0` uygulanır (`CLCARD`, `ITEMS`, `BNCARD`, `BANKACC`, `KSCARD`, `SRVCARD`, `EMUHACC`, `PROJECT`).
- İptal Filtresi: Hareket tablolarında `CANCELLED = 0` uygulanır (`CLFLINE`, `STLINE`, `STFICHE`, `INVOICE`, `ORFLINE`, `BNFLINE`, `KSLINES`, `PAYTRANS`, `CSROLL`, `CSTRANS`, `EMFLINE`, `CSCARD`).
- Listeleme Sınırı: `TOP 50` uygulanır.
- Karakter Standardı: Kolon ve alias'larda yalnızca `[]` köşeli parantez kullanılır. `col\T` suffix'li kolonlar: `[col\T]`.
- Metin Arama: `WHERE UPPER(C.[DEFINITION_]) LIKE UPPER(N'%...%')`, kod aramalarında doğrudan `=`.
- Tarih Filtreleme: Açık aralık kullanılır: `WHERE ST.DATE_ >= '2026-01-01' AND ST.DATE_ < '2027-01-01'`.

4. TAHSİLAT RİSKİ, AÇIK HESAP VE VADESİ GEÇMİŞ ALACAKLAR
- Vadesi Geçmiş Açık Faturalar: `LG_XXX_YY_PAYTRANS` tablosu kullanılır:
  `WHERE CLOSED = 0 AND CANCELLED = 0 AND SIGN = 0 AND PROCDATE < CAST(GETDATE() AS date)`
- Çek/Senet Riski: `LG_XXX_YY_CSCARD` tablosunda `CURRSTAT IN (1, 3)` (portföyde/tahsilde) ve `CURRSTAT = 6` (karşılıksız/protestolu).
- Gecikme Yaşlandırması: `DATEDIFF(day, PROCDATE, GETDATE())` formülüyle gün bazında hesaplanır.

5. BANKA VE KASA YÖN STANDARTLARI (`BNFLINE`, `KSLINES`)
- Para Girişi: `SIGN = 0` (Borç / +). Gelen Havale/EFT (`TRCODE = 3`), Nakit Tahsilat (`TRCODE = 11`), Verilen Hizmet (`TRCODE = 17`).
- Para Çıkışı: `SIGN = 1` (Alacak / -). Gönderilen Havale/EFT (`TRCODE = 4`), Nakit Ödeme (`TRCODE = 12`), Çek/Senet Ödemesi (`TRCODE IN (18, 19)`).
- Kredi Hesapları: `BANKACC.CARDTYPE IN (2, 4)`. Kredi Borcu: `SUM(CASE WHEN SIGN = 1 THEN AMOUNT ELSE -AMOUNT END)`.
- Faiz/Ana Para: `UPPER(LINEEXP) LIKE UPPER(N'%faiz%')` faiz kabul edilir.

6. FATURA TUTAR SEMANTİĞİ (`INVOICE`)
- `GROSSTOTAL`: KDV hariç brüt tutar (`[Brüt Tutar]`).
- `TOTALDISCOUNTED`: KDV hariç indirimli net matrah (`[Net Matrah]`).
- `TOTALVAT`: Toplam KDV tutarı (`[KDV Tutarı]`).
- `NETTOTAL`: KDV dahil nihai ödenecek genel toplam (`[Genel Toplam]`).

7. DİNAMİK ZAMAN VE REFERANS TARİHİ
"Bugün", "bu ay", "vadesi gelenler" ifadelerinde `CAST(GETDATE() AS date)` fonksiyonu referans alınır.
```

## Agent Additional Prompt

```text
Logo ERP analizlerinde firma numarasını (XXX), mali dönemi (YY) ve para birimini açıkça belirt.

- Net yönlü ciro, maliyet, kâr, cari bakiye, açık sipariş ve nakit akışı kavramlarını net ayrıştır.
- İade içeren ciro sonuçlarında satış iadelerinin düşüldüğünü belirt.
- Fatura-ödeme kapama verisine dayalı alacak yaşlandırmasında kapama (matching) işlemlerinin güncelliğine dair bilgilendirici not ekle.
- Çapraz modül analizlerinde her bileşenin kaynağını belirt.
```
