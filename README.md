# FitAI-ai
# FitAI — Akıllı Beden Öneri Sistemi

## 🚀 Kurulum ve Çalıştırma

### 1. Gereksinimleri yükle
```bash
pip install -r requirements.txt
```

### 2. Modeli eğit ve kaydet (yalnızca bir kez)
```bash
python fit_model.py
```
Üç dosya oluşturur: `model.pkl`, `le_vucut.pkl`, `le_kesim.pkl`

### 3. Ana API'yi başlat
```bash
uvicorn api:app --reload --port 8000
```

### 4. NLP servisini başlat (yorum analizi)
```bash
uvicorn main:app --reload --port 8001
```

---

## ⚙️ Ortam Değişkenleri
`.env.example` dosyasını `.env` olarak kopyalayın ve düzenleyin:
```bash
cp .env.example .env
```

| Değişken | Açıklama | Varsayılan |
|----------|----------|------------|
| `FITAI_BACKEND_URL` | API adresi | `http://127.0.0.1:8000` |

---

## 📡 API Uç Noktaları

### `POST /analiz`
Bir ürün için uyum skoru hesaplar.

**Giriş:**
```json
{
  "urun_id": 101,
  "vucut_tipi": "Kum Saati",
  "urun_kesim": "Slim",
  "kumas_esnek": 1
}
```

**Çıkış:**
```json
{
  "urun_id": 101,
  "vucut_tipi": "Kum Saati",
  "uyum_skoru": 88.0,
  "iade_riski": "Düşük",
  "tavsiye": "Kum Saati vücut tipiniz için Slim kesim mükemmel uyum sağlar."
}
```

---

## 🧠 Desteklenen Vücut Tipleri
`Elma` · `Armut` · `Kum Saati` · `Dikdortgen` · `Ters Ucgen` · `Oval` · `Elmas` · `Uzun` · `Atletik`

## 👗 Desteklenen Kesim Tipleri
`Slim` · `Regular` · `Oversize`