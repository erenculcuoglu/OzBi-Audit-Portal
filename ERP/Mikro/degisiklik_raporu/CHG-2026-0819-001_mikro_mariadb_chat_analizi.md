# Mikro ERP Değişiklik Raporu — 19.08.2026

**Rapor No:** CHG-2026-0819-001  
**Hazırlayan:** Antigravity AI  
**Onaylayan:** Eren Culcuoglu  
**Referans:** MariaDB tenant chat logları — Mikro 150 sorgu analizi (Mevlana Petrol)  
**Tarih:** 19.08.2026  
**Versiyon:** Mikro ERP v27.2 Prompt (Saf ERP Karar Destek Mimarisi)

---

## 1. Değişiklik Kapsamı ve Yönetici Özeti

MariaDB'deki Mikro tenant chat loglarından çıkarılan 150 gerçek T-SQL sorgusu analiz edilerek 3 potansiyel hata noktası tespit edilmiş ve v27.2 promptunda önleyici olarak düzeltilmiştir.

---

## 2. Oluşturulan ve Korunan Dosyalar

| # | Dosya Yolu | Türü / Durum | Açıklama |
|---|---|---|---|
| 1 | `ERP/Mikro/mikro_assistant_prompt_v27.1.md` | Orijinal Prompt / 🔒 Korundu | 64 satırlık v27.1 promptu referans olarak saklandı. |
| 2 | `ERP/Mikro/mikro_assistant_schema_v27.1.json` | Şema / 🔒 Değişiklik Yok | 853 satırlık saf ERP şeması aynen korundu. |
| 3 | `ERP/Mikro/mikro_assistant_prompt_v27.2.md` | Yeni Prompt / 🚀 Oluşturuldu | Fatura türü, vadesi geçmiş alacak kuralı ve çek/senet pozisyon eşleşme tablosu eklenmiş yeni prompt. |

---

## 3. Yapılan İyileştirmelerin Detayları

### 3.1 — Fatura Türü Eşleşme Tablosu (`cha_evrak_tip`)
* **Bulgu:** Mevlana Petrol testlerinde (05-06.08.2026) model `cha_evrak_tip = 63` (Satış Faturası) ve `cha_evrak_tip = 0` (Alış Faturası) kodlarını kullanmış ama bu kodlar v27.1 promptunda tanımlı değildi.
* **Düzeltme:** Yeni Madde 2'de fatura türü eşleşme tablosu eklendi:
  - `cha_evrak_tip = 63 AND cha_tip = 0`: Satış Faturası (müşteriye satış, alacak artışı)
  - `cha_evrak_tip = 0 AND cha_tip = 1`: Alış Faturası (tedarikçiden alım, borç artışı)

### 3.2 — Vadesi Geçmiş Alacak Kuralına `cha_evrak_tip` Eklenmesi
* **Bulgu:** Mevlana Petrol testlerinde model CTE ile açık fatura yaşlandırması yaparken `cha_evrak_tip = 63` filtresini kullanmış ama v27.1'deki kural bu filtreyi içermiyordu.
* **Düzeltme:** Madde 8'deki vadesi geçmiş alacak kuralına `cha_evrak_tip = 63` dahil edildi:
  - `cha_cari_cins = 0 AND cha_evrak_tip = 63 AND cha_tip = 0 AND cha_tpoz = 0 AND cha_iptal = 0 AND cha_hidden = 0`

### 3.3 — Çek/Senet Pozisyon Eşleşme Tablosu (`sck_sonpoz`)
* **Bulgu:** Mevlana Petrol testlerinde model `CASE WHEN sck_sonpoz = 0 THEN N'Portföyde' WHEN 2 THEN N'Tahsilde'...` yazmış ama v27.1'de sadece `sck_sonpoz IN (5, 6)` tanımlıydı.
* **Düzeltme:** Madde 7'ye tam pozisyon eşleşme tablosu eklendi:
  - `0:Portföyde, 1:Ciro Edildi, 2:Tahsilde, 3:Teminatta, 5:Karşılıksız, 6:Protestolu, 9:Kısmen Ödendi, 10:Ödendi`

---

## 4. Doğrulama
* Şema dosyasında değişiklik yapılmamıştır — tüm düzeltmeler prompt seviyesindedir.
* Token tasarrufu ve mimari bütünlük korunmuştur.
