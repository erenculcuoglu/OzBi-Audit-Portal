# Mikro ERP Ek Talimatı — v27.2 (Yalın & Pozitif Mimari)

Şema otoriterdir. Metadata'daki hesaplama, filtre ve ilişkileri doğrudan uygula. Bu yönerge, Mikro ERP v27.2 iş mantığını tamamlar.

## Query Additional Prompt

```text
### MİKRO ERP İŞ MANTIĞI VE SORGU KURALLARI

1. HESAP TÜRÜ VE CARİ CİNS
`CARI_HESAP_HAREKETLERI` tablosunda ana hesap bağlantısı `cha_cari_cins` ve `cha_kod` üzerinden kurulur:
- Cari Hesaplar: `cha_cari_cins = 0`
- Banka Hesapları: `cha_cari_cins = 2 AND cha_kod = ban_kod`
- Kasa Hesapları: `cha_cari_cins = 4 AND cha_kod = kas_kod`
`cha_kasa_hizkod`, hareketin karşı hesabıdır; ana bakiye hesaplamasında `cha_kod` kullanılır.

2. FATURA TÜRÜ VE CARİ HAREKET SEMANTİĞİ
Fatura türü `cha_evrak_tip` ile belirlenir:
- Satış Faturası: `cha_evrak_tip = 63 AND cha_tip = 0` (müşteriye satış, alacak artışı).
- Alış Faturası: `cha_evrak_tip = 0 AND cha_tip = 1` (tedarikçiden alım, borç artışı).
Vadesi geçmiş açık satış faturası filtresinde: `cha_evrak_tip = 63 AND cha_tip = 0 AND cha_tpoz = 0 AND cha_iptal = 0 AND cha_hidden = 0`.

3. TL VE DÖVİZ KUR DÖNÜŞÜMÜ
Dövizli tutarları TL'ye çevirirken kur çarpanını güvenli formatta uygula:
`CASE WHEN doviz_cinsi = 0 THEN 1.0 ELSE kur_kolonu END`
Örnek: `cha_meblag * CASE WHEN cha_d_cins = 0 THEN 1.0 ELSE cha_d_kur END`

4. STOK, DEPO VE ENVANTER
- Genel Minimum Stok: `STOKLAR.sto_min_stok`
- Depo Bazlı Minimum Stok: `STOK_DEPO_DETAYLARI.sdp_min_stok` (`sdp_depo_kod = sth_stok_kod AND sdp_depo_no = sth_depono`)
- Net Depo Bakiyesi: `STOK_HAREKETLERI_GIRIS_CIKIS` view'ı ve `sth_giris_cikis` yönüyle hesaplanır.

5. GUID İLİŞKİ ZİNCİRİ
- Sipariş → Stok Hareketi: `sip_Guid = sth_sip_uid`
- Fatura Cari Hareketi → Stok Hareketi: `cha_Guid = sth_fat_uid`
- Stok Hareketi → İlave Maliyet: `sth_Guid = shmy_har_uid` (`STOK_HAREKETLERINE_MALIYET_YANSITMA`, iptal kayıtlar hariç)

6. YÖNETİM VIEW EŞLEMELERİ
- Cari Bakiye: `CARI_HESAPLAR_YONETIM` (Kod: `[msg_S_0078]`, Unvan: `[msg_S_1022]`, TL Bakiye: `[msg_S_0957\T]`)
- Çek/Senet: `ODEME_EMIRLERI_YONETIM` (Kalan: `[msg_S_0301\T]`, Ödenen: `[msg_S_0238\T]`, Vade: `[msg_S_0300]`, Borçlu: `[msg_S_1407]`, Pozisyon: `[msg_S_0297]`)
- Banka Bakiye: `BANKALAR_YONETIM` (Ad: `[msg_S_0070]`, Bakiye: `[msg_S_0833\T]`)
- Kasa Bakiye: `KASALAR_YONETIM` (Kod: `[msg_S_0955]`, Ad: `[msg_S_0956]`, Bakiye: `[msg_S_0957\T]`, Tip: `KASALAR.kas_tip = 0` nakit)

7. ÇEK VE SENET PORTFÖYÜ
- Müşteri/Keşideci Taraması: `(UPPER(c.cari_unvan1) LIKE UPPER(N'%...%') OR UPPER(oe.sck_borclu) LIKE UPPER(N'%...%'))`
- Çek/Senet Pozisyon Eşleşme Tablosu (`sck_sonpoz`): 0:Portföyde, 1:Ciro Edildi, 2:Tahsilde, 3:Teminatta, 5:Karşılıksız, 6:Protestolu, 9:Kısmen Ödendi, 10:Ödendi.
- Tahsil Edilen / Ödenen Evraklar: `sck_odenen > 0 OR sck_sonpoz = 10` (veya view'da `[msg_S_0238\T] > 0 OR [msg_S_0297] = N'Ödendi'`)
- Sorunlu Portföy: `sck_sonpoz IN (5, 6)` (karşılıksız / protestolu).
- Aktif Portföy: `sck_sonpoz IN (0, 2, 3)` (portföyde / tahsilde / teminatta).

8. TAHSİLAT RİSKİ VE VADESİ GEÇMİŞ ALACAKLAR
Tahsilat riski ve geciken alacakları finansal vade yaşlandırması ve açık bakiye üzerinden analiz et:
- Açık ve Vadesi Geçmiş Satış Faturaları: `cha_cari_cins = 0 AND cha_evrak_tip = 63 AND cha_tip = 0 AND cha_tpoz = 0 AND cha_iptal = 0 AND cha_hidden = 0 AND TRY_CONVERT(date, CONVERT(char(8), cha_vade)) < CAST(GETDATE() AS date)`
- Sorunlu Portföy: `ODEME_EMIRLERI` içinde `sck_tip = 0 AND sck_sonpoz IN (5, 6)`
- Yaşlandırma Sıralaması: `DATEDIFF(day, TRY_CONVERT(date, CONVERT(char(8), cha_vade)), GETDATE()) DESC`

9. DÖNEMSEL VE REFERANS TARİH STANDARTLARI
- Mali Çeyrekler: Q1 (01.01–31.03), Q2 (01.04–30.06), Q3 (01.07–30.09), Q4 (01.10–31.12).
- Göreceli Zaman Filtreleri: "Bugün", "bu hafta", "son 30 gün" ifadelerinde sistem referans tarihini (`CAST(GETDATE() AS date)`) doğrudan baz al.
- Metin Eşleşmeleri: `UPPER(kolon) LIKE UPPER(N'%...%')`
```

## Agent Additional Prompt

```text
Mikro ERP analizlerinde para birimini ve analiz dönemini açıkça belirt.

- Ciro, iskonto, maliyet, net kâr, cari bakiye ve açık sipariş kavramlarını ayrıştırarak sun.
- Satış iadelerinin (`sth_normal_iade = 1`) cirodan düşüldüğünü raporda açıkla.
- Fatura-ödeme kapama verisine dayalı alacak yaşlandırmasında güncel kapama durumunun dikkate alındığını ifade et.
```
