import requests

# هلق: عنوان وهمي — لما يجهز فريق الويب تبدليه بالعنوان الحقيقي
BACKEND_URL = "http://127.0.0.1:5000" # مؤقتاً API تاعتك أنت

def احسب_وحفظ_السكور(urun_id, vucut_tipi, urun_kesim, kumas_esnek):
    """
    1. تحسب السكور من نموذجك
    2. ترسله لـ API الخلفي ليحفظه في VucutUyumSkorlari
    """

    # الخطوة 1: احسبي السكور من نموذجك
    response = requests.post(f"{BACKEND_URL}/analiz", json={
        "urun_id":     urun_id,
        "vucut_tipi":  vucut_tipi,
        "urun_kesim":  urun_kesim,
        "kumas_esnek": kumas_esnek
    })

    sonuc = response.json()
    print(f"✅ Skor hesaplandı: {sonuc}")

    # الخطوة 2: لما يجهز فريق الويب، أضيفي هون
    # requests.post("http://BACKEND/api/vucut-uyum-skoru", json=sonuc)

    return sonuc

# اختبار على 3 منتجات
urunler = [
    {"urun_id": 101, "vucut_tipi": "Kum Saati", "urun_kesim": "Slim",    "kumas_esnek": 1},
    {"urun_id": 102, "vucut_tipi": "Elma",       "urun_kesim": "Oversize","kumas_esnek": 1},
    {"urun_id": 103, "vucut_tipi": "Armut",      "urun_kesim": "Regular", "kumas_esnek": 0},
]

print("─" * 50)
for urun in urunler:
    احسب_وحفظ_السكور(**urun)
    print("─" * 50)