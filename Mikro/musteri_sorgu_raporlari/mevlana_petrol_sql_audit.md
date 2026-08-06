# Mevlana Petrol — OzBI SQL Değerlendirme Raporu (v2 — Güncellenmiş)

**Tenant:** Mevlana Petrol (`a0daf95f-6d82-4feb-a351-1b0aabcfef20`)
**Kayıt E-posta:** `bim@mevlanapetrol.com.tr`
**Rapor Güncelleme:** 05.08.2026
**Kapsam:** 04.08.2026 14:12 – 05.08.2026 16:23 (2 gün)
**ERP Türü:** Mikro ERP (MS SQL Server)
**Toplam Mesaj:** 48 (24 kullanıcı sorusu + 24 AI yanıtı)
**Toplam SQL Sorgusu:** 27 (24 mesajda, 3 tanesi çoklu sorgu JSON array içinde)
**Şema Referansı:** [mikro_assistant_schema_20260730.json](file:///Users/eren.culcuoglu/Desktop/Desktop/OzBI%20Portal%20CRM/Mikro/mikro_assistant_schema_20260730.json) (47 tablo)
**Prompt Referansı:** [mikro_assistant_prompt_20260730.md](file:///Users/eren.culcuoglu/Desktop/Desktop/OzBI%20Portal%20CRM/Mikro/mikro_assistant_prompt_20260730.md) (v27)
**Kapsamlı Denetim Raporu:** [mevlana_petrol_kapsamli_denetim.md](file:///Users/eren.culcuoglu/.gemini/antigravity-ide/brain/ccd67cd8-f45d-4d7c-b77e-cc26fdc66c6f/mevlana_petrol_kapsamli_denetim.md)

---

## Genel Skorlar

| Metrik | Sonuç |
|---|---| 
| **Toplam SQL Sorgusu** | 27 (10 sandbox + 17 Mikro ERP) |
| **Tablo/View Doğruluğu** | ✅ %94,1 — 1 choose-view referansı (CARI_HESAP_HAREKETLERI_CHOOSE_30) |
| **Kolon Doğruluğu** | ✅ %100 — Şemada olmayan kolon referansı yok |
| **ERP İş Mantığı Uyumu** | ✅ %94,1 — Mikro v27 kurallarına uygun |
| **TL Kur Koruması** | ✅ %100 — 3/3 uygulanmış |
| **Halüsinasyon** | ❌ %0 — Sıfır halüsinasyon |
| **Gerçek Hata (ErrorMessage)** | 4 adet (3 yetki + 1 sentaks) |
| **Şemasal Hata** | ❌ 0 adet |
| **Motor Tutarlılığı** | %82,4 (14/17 tutarlı, 3 küçük fark) |
| **Müşteri Beğeni** | 1 adet (Sorgu #14: Tahsilat) |

---

## Önceki Rapora Göre Güncellemeler (v1 → v2)

| Önceki Rapor (v1) | Bu Rapor (v2) |
|---|---|
| 11 sorgu analiz edildi | **27 sorgu** analiz edildi |
| Yalnızca Oturum 1 (04.08 14:12-14:39) | **2 gün** kapsam (04.08-05.08) |
| Motor tutarlılığı analiz edilmedi | **Motor tutarlılık analizi** eklendi (%82,4) |
| Sandbox sorguları sadece not olarak belirtilmişti | **10 sandbox sorgusu** ayrıntılı analiz edildi |
| 2. gün sorguları yoktu | **11 yeni sorgu** analiz edildi (Sorgu #18-27) |
| Risk limiti analizi yoktu | **Çoklu CTE risk analizi** denetlendi (Sorgu #26) |

---

## Kritik Bulgular Özeti

1. **En İyi Sorgu:** #26 (Ödenmemiş Fatura Risk Özeti) — 2 katmanlı CTE, v27 kurallarının %100'üne uygun
2. **Tek T-SQL Sentaks Hatası:** #12 (ORDER BY/GROUP BY uyumsuzluğu) — Motor yakalayamıyor
3. **Tek Tablo Riski:** #19 (`CARI_HESAP_HAREKETLERI_CHOOSE_30`) — Şemada tanımsız choose-view
4. **Motor Eksiklikleri:** T-SQL sentaks doğrulaması yok, tablo adı substring-bazlı kontrol
5. **IsSucceeded Flag:** 48 mesajın tamamında `0` — pipeline durum takibi gözden geçirilmeli

---

## Sonuç

Mevlana Petrol müşterisinin OzBI platformundaki Mikro ERP deneyimi **genel olarak başarılıdır**. AI'ın ürettiği T-SQL sorguları şema doğruluğu, iş mantığı uyumu ve Mikro v27 kurallarına uyum açısından **96/100 (A+)** düzeyindedir. Portal'ın Mikro skorlama motoru %82,4 tutarlılıkla doğru çalışmakta olup, T-SQL sentaks kontrolü ve tablo adı eşleşme mantığının güçlendirilmesi önerilmektedir.
