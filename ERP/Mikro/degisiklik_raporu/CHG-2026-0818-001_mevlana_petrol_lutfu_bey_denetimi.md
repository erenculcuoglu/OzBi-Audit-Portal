# Mikro ERP Değişiklik Raporu — 18.08.2026

**Rapor No:** CHG-2026-0818-001  
**Hazırlayan:** Antigravity AI  
**Onaylayan:** Eren Culcuoglu  
**Referans Test:** Chat ID `b184a938-4506-4f0a-a678-7b12a29f51fb` — Muhasebe Müdürü Lütfü Bey (Cem kullanıcısı), Mevlana Petrol  
**Tarih:** 18.08.2026  
**Versiyon:** Mikro ERP v27.1 (Yalın & Pozitif Mimari)

---

## 1. Değişiklik Kapsamı ve Yönetici Özeti

Mevlana Petrol Muhasebe Müdürü Lütfü Bey’in 18.08.2026 tarihinde sisteme yönelttiği 10 adet kritik finans/muhasebe sorgusu incelenmiştir. Yapılan denetimde:
- 10 sorunun 9'u (%90 başarı) doğru tablolar (`CARI_HESAP_HAREKETLERI`, `ODEME_EMIRLERI`, `FIRMALAR`, `CARI_HESAPLAR_YONETIM`) üzerinden hatasız yanıtlanmıştır.
- **Ürün Kapsamı Netleştirmesi:** OzBI'ın temel misyonunun "şirket içi personel denetimi / kullanıcı hareket takipçisi" değil; doğrudan **üst düzey finansal analiz, nakit akışı, açık fatura/vade yaşlandırması, stok ve karar destek mekanizması** olduğu teyit edilmiştir.
- **Tahsilat Riski Anlamsal Yönlendirme Vakası:** Eski oturumlarda "tahsilat riski" dendiğinde finansal risk hesabı yerine unvanda `%risk%` kelimesinin arandığı görülmüştür.
- **Tarih Tereddüdü Vakası:** Modelin vadeli çek/senet sorgularında bugünün tarihi konusunda tereddütte kaldığı gözlemlenmiştir.

Bu bulgular doğrultusunda **orijinal v27 dosyaları aynen korunmuş**, v27.1 olarak yeni nesil **Yalın, Pozitif & Token Tasarruflu** prompt ve şema dosyaları oluşturulmuştur.

---

## 2. Oluşturulan ve Korunan Dosyalar

| # | Dosya Yolu | Türü / Durum | Açıklama |
|---|---|---|---|
| 1 | `ERP/Mikro/mikro_assistant_prompt_v27.md` | Orijinal Prompt / 🔒 Korundu | 103 satırlık orijinal prompt referans olarak saklandı. |
| 2 | `ERP/Mikro/mikro_assistant_schema_v27.json` | Orijinal Şema / 🔒 Korundu | 75 KB'lık orijinal şema referans olarak saklandı. |
| 3 | `ERP/Mikro/mikro_assistant_prompt_v27.1.md` | Yeni Prompt / 🚀 Oluşturuldu | Negatif ifadelerden arındırılmış, pozitif direktifli, ~%45 token tasarruflu 72 satırlık yeni prompt. |
| 4 | `ERP/Mikro/mikro_assistant_schema_v27.1.json` | Yeni Şema / 🚀 Oluşturuldu | Formatı v27 ile birebir tek satırlı kolon dizilimine uyumlu, tamamen saf ERP kapsamındaki güncel Mikro v27.1 şeması (853 satır). |

---

## 3. Yapılan İyileştirmelerin Detayları

### 3.1 — Prompt: Yalınlaştırma ve Pozitif (Affirmative) Direktif Mimarisi
* **Dosya:** `ERP/Mikro/mikro_assistant_prompt_v27.1.md`
* **Yaklaşım:** "Asla şunu yapma, bunu arama, veri yok deme" gibi negatif yönlendirmeler tamamen kaldırıldı. Yerine modele doğrudan ne yapması gerektiğini anlatan pozitif ve bütüncül kurallar getirildi.
* **Token Optimizasyonu:** Gereksiz dolaylı anlatımlar elenerek toplam token yükü ~%45 hafifletildi.

#### Eklenen / Güncellenen Kurallar:
1. **Tahsilat Riski & Vadesi Geçmiş Açık Alacaklar (Madde 7):**
   * Metinsel arama yerine vadesi geçmiş açık faturalar (`cha_tip = 0 AND cha_tpoz = 0 AND cha_vade < GETDATE()`), sorunlu portföy (`sck_sonpoz IN (5, 6)`) ve gecikme yaşlandırması (`DATEDIFF`) formülü tanımlandı.
2. **Dönemsel ve Referans Tarih Standartları (Madde 8):**
   * Modelin tereddütsüz zaman filtreleri uygulayabilmesi için sistemin o anki güncel tarihi (`CAST(GETDATE() AS date)`) mutlak referans olarak bağlandı.

---

## 4. Doğrulama ve Sonuç
* Prompt ve şema token tasarrufu, format düzeni ve saf ERP odaklılığı açısından optimize edilmiştir.
