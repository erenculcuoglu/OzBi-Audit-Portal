# Logo ERP Değişiklik Raporu — 18.08.2026

**Rapor No:** CHG-2026-0818-002  
**Hazırlayan:** Antigravity AI  
**Onaylayan:** Eren Culcuoglu  
**Referans Testler:** Q Bilgi Teknolojileri LTD ŞTİ (`2aba580b-b495-461f-a56d-bdc4570e6d24`) & Mevlana Petrol Logo ERP Denetimleri  
**Tarih:** 18.08.2026  
**Versiyon:** Logo ERP v7.3 Prompt & v7.2 Şema (Saf ERP Karar Destek Mimarisi)

---

## 1. Değişiklik Kapsamı ve Yönetici Özeti

Logo ERP çözüm ortağımız Q Bilgi Teknolojileri (Semih Bey) ve genel Logo ERP denetimleri kapsamında yapılan sorgulamalar incelenmiştir. Yapılan analizlerde:
- **Banka/Kasa `SIGN` Semantiği:** Para girişlerinde `SIGN = 0` (Gelen Havale `TRCODE = 3`), para çıkışlarında `SIGN = 1` (Gönderilen Havale `TRCODE = 4`) kuralları teyit edilmiştir.
- **Tahsilat Riski ve Yaşlandırma:** `LG_XXX_YY_PAYTRANS` tablosunun (`CLOSED = 0`, `PROCDATE < GETDATE()`) fatura bazlı açık bakiye ve tahsilat riski analizlerinde temel teşkil ettiği netleştirilmiştir.
- **Geçmiş Tarihli Stok Güvenliği:** 30+ saniyelik sorgu gecikmelerini önlemek adına `LV_XXX_YY_STINVTOT` (anlık) ve indeksli `LG_XXX_YY_STLINE` ayrımı kurallaştırılmıştır.
- **Saf ERP Odağı:** OzBI'ın temel misyonunun "şirket içi personel denetimi / kullanıcı hareket takipçisi" değil; doğrudan **üst düzey finansal analiz, nakit akışı, açık fatura/vade yaşlandırması, stok ve karar destek mekanizması** olduğu teyit edilmiş, şema ve prompt tamamen bu odak doğrultusunda saf ERP kapsamında tutulmuştur.

---

## 2. Oluşturulan ve Korunan Dosyalar

| # | Dosya Yolu | Türü / Durum | Açıklama |
|---|---|---|---|
| 1 | `ERP/Logo/assistant_prompt/logo_assistant_prompt_v7.2.md` | Orijinal Prompt / 🔒 Korundu | 83 satırlık v7.2 promptu referans olarak saklandı. |
| 2 | `ERP/Logo/json/logo_assistant_schema_v7.1.json` | Orijinal Şema / 🔒 Korundu | 772 satırlık v7.1 şeması referans olarak saklandı. |
| 3 | `ERP/Logo/assistant_prompt/logo_assistant_prompt_v7.3.md` | Yeni Prompt / 🚀 Oluşturuldu | Tahsilat riski (`PAYTRANS`), banka `SIGN` kutupları, geçmiş stok indeks koruması ve dinamik referans tarihi içeren yalın & pozitif mimarili prompt. |
| 4 | `ERP/Logo/json/logo_assistant_schema_v7.2.json` | Yeni Şema / 🚀 Oluşturuldu | `LG_XXX_YY_PAYTRANS` vade/ödeme tablosu eklenmiş, tamamen ERP işlem tabloları kapsamında tutulan 793 satırlık güncel Logo v7.2 şeması. |

---

## 3. Yapılan İyileştirmelerin Detayları

### 3.1 — Tahsilat Riski, Açık Faturalar ve Yaşlandırma
* **Şema Değişikliği:** Logo'nun fatura ve cari ödeme planı tablosu olan `LG_XXX_YY_PAYTRANS` şemaya dahil edildi.
* **Prompt Kuralı:** "Vadesi geçmiş faturalar / tahsilat riski" sorulduğunda `CLOSED = 0 AND SIGN = 0 AND PROCDATE < CAST(GETDATE() AS date)` şartı getirildi.

### 3.2 — Banka & Kasa Yön Standartları (`SIGN = 0` vs `SIGN = 1`)
* **Prompt Kuralı:** Gelen havale/tahsilat için `SIGN = 0`, giden havale/ödeme için `SIGN = 1` kuralı mutlak kural olarak netleştirildi.

### 3.3 — Tarihli Stok Sorgularında Timeout Koruması
* **Prompt Kuralı:** Anlık stok için `LV_XXX_YY_STINVTOT`, geçmiş tarihli hareketler için indeksli `LG_XXX_YY_STLINE` tablosu zorunlu kılındı.

---

## 4. Doğrulama ve Sonuç
* `dotnet build` başarıyla tamamlanmıştır.
* Şema ve prompt dosyaları saf ERP karar destek vizyonuna tam uyumlu hale getirilmiştir.
