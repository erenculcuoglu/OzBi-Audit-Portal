# Mikro ERP Ek Talimatı — v27

Şema otoriterdir. `[Hesaplama:]`, `[Koşul:]`, `[Filtre:]`, `[İlişki:]` ve `Bkz.` metadata’sında bulunan Mikro kurallarını doğrudan şemadan uygula. Bu dosya yalnızca tek kolon metadata’sının yeterince açıklayamadığı Mikro’ya özgü bilgileri tamamlar.

## Query Additional Prompt

```text
### MİKRO ERP EK KURALLARI

1. ANA HESAP VE KARŞI HESAP AYRIMI

`CARI_HESAP_HAREKETLERI` içinde `cha_cari_cins`, `cha_kod` alanının temsil ettiği ana hesap türünü belirler:
- `cha_cari_cins = 0`: cari hesap
- `cha_cari_cins = 2`: banka
- `cha_cari_cins = 4`: kasa

Banka ve kasa bakiyelerinde ana hesap bağlantıları:
- Banka: `cha.cha_cari_cins = 2 AND cha.cha_kod = ban.ban_kod`
- Kasa: `cha.cha_cari_cins = 4 AND cha.cha_kod = kas.kas_kod`

`cha_kasa_hizkod`, hareketin karşı kasa/hizmet kodudur. Ana banka veya kasa bakiyesini hesaplarken `cha_kod` yerine kullanılmaz.

2. TL KUR KORUMASI

Şema metadata’sındaki bir TL formülü tutarı döviz kuruyla çarpıyorsa, TL satırlarında kurun `0` veya `NULL` olabilmesine karşı kur çarpanını şöyle uygula:
`CASE WHEN doviz_cins_kolonu = 0 THEN 1.0 ELSE kur_kolonu END`

Cari hareket örneği:
`cha.cha_meblag * CASE WHEN cha.cha_d_cins = 0 THEN 1.0 ELSE cha.cha_d_kur END`

Sipariş tutarı TL dönüşümünde aynı koruma `sip_doviz_cinsi` ve `sip_doviz_kuru` için uygulanabilir.

Bu kural yalnızca şema metadata’sında kur dönüşümü bulunan hesaplara uygulanır. `sth_tutar` gibi metadata formülünde kur çarpımı bulunmayan alanlara kendiliğinden kur dönüşümü ekleme.

3. STOK, DEPO VE SEVİYE AYRIMI

`STOKLAR.sto_min_stok`, stok kartının genel minimum seviyesidir.

`STOK_DEPO_DETAYLARI.sdp_min_stok`, belirli bir depodaki minimum seviyedir. `sdp_depo_kod`, adına rağmen ilgili stok kodunu tutar.

Depo bazlı stok bağlantısı:
`sh.sth_stok_kod = sdp.sdp_depo_kod AND sh.sth_depono = sdp.sdp_depo_no`

Depo kartı bağlantısı:
`sdp.sdp_depo_no = dep.dep_no`

Mevcut/net depo stoku için `STOK_HAREKETLERI_GIRIS_CIKIS` view’ındaki `sth_giris_cikis` yönünü kullan.

4. GUID İLİŞKİLERİ

- Sipariş satırı → stok hareketi: `sip.sip_Guid = sh.sth_sip_uid`
- Fatura cari hareketi → stok hareketi: `cha.cha_Guid = sh.sth_fat_uid`
- Stok hareketi → ilave maliyet: `sh.sth_Guid = shmy.shmy_har_uid`

`shmy_har_uid`, `sth_fat_uid` ile eşleştirilmez. `sth_Guid` stok hareketinin, `sth_fat_uid` ise bağlı faturanın UID’idir.

İlave maliyet, landed cost veya toplam sahiplik maliyetinde `STOK_HAREKETLERINE_MALIYET_YANSITMA` kullanılır ve iptal edilmiş yansıtma kayıtları dahil edilmez.

5. YÖNETİM VIEW EŞLEMELERİ

- Cari bakiye — `CARI_HESAPLAR_YONETIM`: kod `[msg_S_0078]`, unvan `[msg_S_1022]`, TL bakiye `[msg_S_0957\T]`
- Cari detay — `CARIDETAY`: grup `[msg_S_0472]`, bölge `[msg_S_1101]`, temsilci `[msg_S_0978]`
- Stok detay — `STOKDETAY`: stok kodu `[msg_S_0001]`, isim `[msg_S_0002]`, kategori `[msg_S_0012]`, marka `[msg_S_0025]`, fiyat `[msg_S_0006]`
- Banka bakiye — `BANKALAR_YONETIM`: banka adı `[msg_S_0070]`, mevduat bakiyesi `[msg_S_0833\T]`
- Kasa bakiye — `KASALAR_YONETIM`: kasa kodu `[msg_S_0955]`, ad `[msg_S_0956]`, TL bakiye `[msg_S_0957\T]`, döviz `[msg_S_0254]`, kasa tipi adı `[msg_S_0954]`
- Çek/senet — `ODEME_EMIRLERI_YONETIM`: kalan tutar `[msg_S_0301\T]`, vade `[msg_S_0300]`, pozisyon adı `[msg_S_0297]`
- Fiyat listesi — `STOK_SATIS_FIYAT_LISTELERI_YONETIM`: liste no `[msg_S_1264]`, stok kodu `[msg_S_0001]`, fiyat `[msg_S_0984]`
- Sipariş özeti — `SIPARISLER_OZET`: stok kodu `[so_Kodu]`, talep `[so_TalepMiktar]`, karşılanan `[so_TalepKarsilanan]`, kapanan `[so_TalepKapanan]`, temin `[so_TeminMiktar]`, karşılanan temin `[so_TeminKarsilanan]`

`KASALAR_YONETIM` view’ında sayısal `kas_tip` bulunmaz. Nakit kasa gerektiğinde `KASALAR_YONETIM.[msg_S_0955] = KASALAR.kas_kod` bağlantısını kur ve `KASALAR.kas_tip = 0` uygula. `[msg_S_0954]` kasa tipi adıdır; `[msg_S_0254]` döviz cinsidir.

6. CİRO, İSKONTO VE MALİYET BELİRSİZLİKLERİ

`sth_tutar`, Mikro dokümantasyonunda hareket tutarı olarak tanımlanır. İskonto öncesi veya sonrası olduğuna dair ek varsayım yapma; şemadaki net ciro formülünü uygula ve iskontoları ikinci kez cirodan düşme.

Kümülatif iskonto istendiğinde şemadaki altı iskonto alanının tamamını kullan.

`sth_maliyet_ana` ile normal satış maliyeti şemadaki formüle göre hesaplanır. Satış iadelerinin maliyet işareti kesin olarak tanımlanmadığından, iadeli kârlılık sorgularında normal satış maliyeti ile iade maliyetini ayrı kolonlarda döndür; iade maliyetini varsayımla ikinci kez ters işaretleme.

7. T-SQL YAZIM STANDARTLARI

- "Bu çeyrek", "geçen çeyrek" gibi ifadelerde mali çeyrek hesaplaması: Q1: 01.01–31.03, Q2: 01.04–30.06, Q3: 01.07–30.09, Q4: 01.10–31.12. Bugünün tarihine göre aktif çeyreğin tamamını kapsayan tarih aralığı kullan; yalnızca mevcut ayı değil.
- Dinamik tarih gerektiren sorgularda (gecikme gün hesabı, vade karşılaştırması, "son X gün/ay") prompt'ta sağlanan bugünün tarihini sabit tarih olarak kullan.
- Sözel isim aramalarında UPPER(kolon) LIKE UPPER(N'%...%') pattern'ını kullan.
```

## Agent Additional Prompt

```text
Mikro ERP analizlerinde para birimini ve analiz dönemini açıkça belirt.

- Net yönlü ciro, iskonto, maliyet, kâr, cari bakiye, açık sipariş ve nakit akışı kavramlarını birbirine karıştırma.
- İade içeren ciro sonuçlarında satış iadelerinin düşüldüğünü belirt.
- Satış ve iade maliyetleri ayrı getirildiyse kullanılan netleştirme yöntemini açıkla; veri tarafından doğrulanmayan maliyet işareti varsayımı yapma.
- Fatura-ödeme kapama verisine dayalı alacak yaşlandırması gösterildiğinde şu notu ekle:
  “Alacak yaşlandırması, Mikro ERP’de fatura-ödeme kapama kayıtlarının güncel ve eksiksiz olduğu varsayımıyla hesaplanmıştır. Kapama yapılmamış veya kısmen kapatılmış kayıtlarda sonuç, gerçek açık bakiyeden farklı olabilir.”
```
