import os
import requests

# API adresini ortam değişkeninden oku; tanımlı değilse varsayılanı kullan
BACKEND_URL = os.getenv("FITAI_BACKEND_URL", "http://127.0.0.1:8000")


def skoru_hesapla_ve_kaydet(urun_id, vucut_tipi, urun_kesim, kumas_esnek):
    """
    1. Modelden uyum skoru hesaplar
    2. Sonucu backend API'sine göndererek VucutUyumSkorlari tablosuna kaydeder
    """

    yanit = requests.post(f"{BACKEND_URL}/analiz", json={
        "urun_id":     urun_id,
        "vucut_tipi":  vucut_tipi,
        "urun_kesim":  urun_kesim,
        "kumas_esnek": kumas_esnek
    })

    yanit.raise_for_status()
    sonuc = yanit.json()
    print(f"✅ Skor hesaplandı: {sonuc}")

    # Web ekibi hazır olduğunda buraya eklenecek:
    # requests.post(f"{BACKEND_URL}/api/vucut-uyum-skoru", json=sonuc)

    return sonuc


# 3 ürün üzerinde test
urunler = [
    {"urun_id": 101, "vucut_tipi": "Kum Saati", "urun_kesim": "Slim",     "kumas_esnek": 1},
    {"urun_id": 102, "vucut_tipi": "Elma",       "urun_kesim": "Oversize", "kumas_esnek": 1},
    {"urun_id": 103, "vucut_tipi": "Armut",      "urun_kesim": "Regular",  "kumas_esnek": 0},
]

print("─" * 50)
for urun in urunler:
    skoru_hesapla_ve_kaydet(**urun)
    print("─" * 50)