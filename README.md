# FitAI — Akıllı Beden Öneri Sistemi

## 🚀 Kurulum

```bash
pip install -r requirements.txt
```

## ▶️ Çalıştırma Sırası

### 1. Modeli eğit (yalnızca ilk seferinde)
```bash
python fit_model.py
```

### 2. AI API'sini başlat (Terminal 1)
```bash
fitai_env/bin/uvicorn api:app --reload --port 8000
```

### 3. NLP Servisini başlat (Terminal 2)
```bash
fitai_env/bin/uvicorn main:app --reload --port 8001
```

### 4. Scheduler'ı başlat (Terminal 3)
```bash
python scheduler.py
```

### 5. Test
```bash
python api_client.py
```

---

## ⚙️ Ortam Değişkenleri

`.env.example` → `.env` olarak kopyala:
```bash
cp .env.example .env
```

| Değişken | Açıklama | Varsayılan |
|----------|----------|------------|
| `FITAI_AI_URL` | AI model servisi | `http://127.0.0.1:8000` |
| `FITAI_NLP_URL` | NLP servisi | `http://127.0.0.1:8001` |
| `FITAI_BACKEND_URL` | Gerçek backend | `http://localhost:44399` |
| `UYUM_SKORU_ARALIK_DK` | Skor güncelleme sıklığı | `60` dakika |
| `YORUM_ANALIZ_ARALIK_DK` | Yorum analiz sıklığı | `30` dakika |

---

## 📡 API Endpoint'leri

### AI Servisi (port 8000)
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | `/analiz` | Uyum skoru hesapla |

### NLP Servisi (port 8001)
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/yorum-analiz` | Mock yorumları analiz et |
| POST | `/yorum-analiz/toplu` | Toplu yorum analizi |

---

## 🔗 Backend Entegrasyonu

Backend ekibi şu endpoint'leri açtığında otomatik bağlanacak:

| Endpoint | Açıklama |
|----------|----------|
| `POST /api/vucut-uyum-skoru` | Uyum skorunu kaydet |
| `POST /api/yorum-analiz` | Yorum analizini kaydet |
| `GET /api/urunler` | Ürün listesini çek |
| `GET /api/yorumlar?durum=beklemede` | Bekleyen yorumları çek |

---

## 🧠 Desteklenen Vücut Tipleri
`Elma` · `Armut` · `Kum Saati` · `Dikdortgen` · `Ters Ucgen` · `Oval` · `Elmas` · `Uzun` · `Atletik`

## 👗 Desteklenen Kesim Tipleri
`Slim` · `Regular` · `Oversize`