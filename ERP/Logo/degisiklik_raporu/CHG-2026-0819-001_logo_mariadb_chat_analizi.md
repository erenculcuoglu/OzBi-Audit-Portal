# Logo ERP Değişiklik Raporu — 19.08.2026

**Rapor No:** CHG-2026-0819-001  
**Hazırlayan:** Antigravity AI  
**Onaylayan:** Eren Culcuoglu  
**Referans:** MariaDB tenant chat logları — Logo 200 sorgu analizi (Q Bilgi, Ahmet Göküş)  
**Tarih:** 19.08.2026  
**Versiyon:** Logo ERP v7.4 Prompt (Saf ERP Karar Destek Mimarisi)

---

## 1. Değişiklik Kapsamı ve Yönetici Özeti

MariaDB'deki Logo tenant chat loglarından çıkarılan 200 gerçek T-SQL sorgusu analiz edilerek 3 potansiyel hata noktası tespit edilmiş ve v7.4 promptunda önleyici olarak düzeltilmiştir.

---

## 2. Oluşturulan ve Korunan Dosyalar

| # | Dosya Yolu | Türü / Durum | Açıklama |
|---|---|---|---|
| 1 | `ERP/Logo/assistant_prompt/logo_assistant_prompt_v7.3.md` | Orijinal Prompt / 🔒 Korundu | 69 satırlık v7.3 promptu referans olarak saklandı. |
| 2 | `ERP/Logo/json/logo_assistant_schema_v7.2.json` | Şema / 🔒 Değişiklik Yok | 793 satırlık saf ERP şeması aynen korundu. |
| 3 | `ERP/Logo/assistant_prompt/logo_assistant_prompt_v7.4.md` | Yeni Prompt / 🚀 Oluşturuldu | Kasa SIGN tekrarı, INVOICE.TRCODE eşleşme tablosu ve ISNULL(LINEEXP) koruması eklenmiş yeni prompt. |

---

## 3. Yapılan İyileştirmelerin Detayları

### 3.1 — Kasa & Banka SIGN Kuralının Birleştirilmesi
* **Bulgu:** Q Bilgi testlerinde (11.08.2026) model, kasa nakit akışında `SIGN = 1` → Giriş ve `SIGN = 0` → Çıkış olarak ters yazmıştır.
* **Düzeltme:** Madde 5'te banka ve kasa için ortak nakit akışı formülü açıkça tanımlandı:
  - `SUM(CASE WHEN [SIGN] = 0 THEN [AMOUNT] ELSE 0 END)` = Giriş
  - `SUM(CASE WHEN [SIGN] = 1 THEN [AMOUNT] ELSE 0 END)` = Çıkış

### 3.2 — INVOICE.TRCODE Eşleşme Tablosu
* **Bulgu:** Q Bilgi testlerinde (17.08.2026) model `TRCODE IN (7, 8, 9, 14)` kullanarak Proforma faturayı gerçek satışa dahil etmiştir.
* **Düzeltme:** Madde 6'da tam TRCODE eşleşme tablosu eklendi:
  - Satış: `TRCODE IN (7, 8, 9)` — Proforma (`TRCODE IN (10, 13, 14)`) hariç tutuldu.

### 3.3 — LINEEXP NULL Koruması
* **Bulgu:** Ahmet Göküş testlerinde (23.07.2026) banka kredi faiz/ana para ayrımında `LINEEXP` NULL kontrolü yapılmamıştır.
* **Düzeltme:** Madde 5'te faiz ayrımı kuralı `ISNULL(LINEEXP, N'')` korumalı olarak güncellendi.

---

## 4. Doğrulama
* Şema dosyasında değişiklik yapılmamıştır — tüm düzeltmeler prompt seviyesindedir.
* Token tasarrufu ve mimari bütünlük korunmuştur.
