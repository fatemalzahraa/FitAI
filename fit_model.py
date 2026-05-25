import pandas as pd
from sklearn.ensemble import RandomForestRegressor
from sklearn.preprocessing import LabelEncoder

# ── 9 أنواع جسم × 3 قصات × مرونة = بيانات أكثر وأدق ──
data = {
    'vucut_tipi': [
        'Elma',       'Elma',       'Elma',
        'Armut',      'Armut',      'Armut',
        'Kum Saati',  'Kum Saati',  'Kum Saati',
        'Dikdortgen', 'Dikdortgen', 'Dikdortgen',
        'Ters Ucgen', 'Ters Ucgen', 'Ters Ucgen',
        'Oval',       'Oval',       'Oval',
        'Elmas',      'Elmas',      'Elmas',
        'Uzun',       'Uzun',       'Uzun',
        'Atletik',    'Atletik',    'Atletik',
    ],
    'urun_kesim': [
        'Slim', 'Regular', 'Oversize',
        'Slim', 'Regular', 'Oversize',
        'Slim', 'Regular', 'Oversize',
        'Slim', 'Regular', 'Oversize',
        'Slim', 'Regular', 'Oversize',
        'Slim', 'Regular', 'Oversize',
        'Slim', 'Regular', 'Oversize',
        'Slim', 'Regular', 'Oversize',
        'Slim', 'Regular', 'Oversize',
    ],
    'kumas_esnek': [
        1, 0, 1,
        1, 0, 1,
        1, 1, 0,
        0, 1, 1,
        1, 0, 1,
        1, 0, 0,
        0, 1, 1,
        1, 1, 0,
        1, 0, 1,
    ],
    'uyum_skoru': [
        # Elma - الجسم التفاحي يناسب Regular و Oversize أكثر
        48, 72, 78,
        # Armut - يناسب Slim فوق Regular تحت
        65, 75, 70,
        # Kum Saati - ساعة رملية تناسب كل شيء
        88, 82, 70,
        # Dikdortgen - مستطيل يناسب Oversize
        60, 70, 80,
        # Ters Ucgen - مثلث معكوس يناسب Slim
        72, 65, 60,
        # Oval - قريب من Elma
        50, 70, 75,
        # Elmas - ماسة
        68, 74, 72,
        # Uzun - طويل ونحيف
        75, 70, 65,
        # Atletik - رياضي
        82, 78, 72,
    ]
}

df = pd.DataFrame(data)

le_vucut = LabelEncoder()
le_kesim  = LabelEncoder()
df['vucut_encoded'] = le_vucut.fit_transform(df['vucut_tipi'])
df['kesim_encoded']  = le_kesim.fit_transform(df['urun_kesim'])

model = RandomForestRegressor(n_estimators=200, random_state=42)
model.fit(df[['vucut_encoded', 'kesim_encoded', 'kumas_esnek']], df['uyum_skoru'])

def tahmin_et(vucut_tipi, urun_kesim, kumas_esnek):
    v = le_vucut.transform([vucut_tipi])[0]
    k = le_kesim.transform([urun_kesim])[0]
    input_df = pd.DataFrame(
        [[v, k, kumas_esnek]],
        columns=['vucut_encoded', 'kesim_encoded', 'kumas_esnek']
    )
    skor = round(model.predict(input_df)[0], 1)
    if skor >= 75:   risk = 'Düşük'
    elif skor >= 55: risk = 'Orta'
    else:            risk = 'Yüksek'
    return skor, risk

# ── اختبار الـ 9 أنواع ──────────────────────────────────
print(f"{'Vücut Tipi':<14} {'Kesim':<10} {'Skor':>6}  {'Risk'}")
print("─" * 45)
for vucut in ['Elma','Armut','Kum Saati','Dikdortgen','Ters Ucgen','Oval','Elmas','Uzun','Atletik']:
    for kesim in ['Slim', 'Regular', 'Oversize']:
        skor, risk = tahmin_et(vucut, kesim, 1)
        print(f"{vucut:<14} {kesim:<10} {skor:>6}  {risk}")
    print()



    import joblib

# حفظ النموذج والـ encoders
joblib.dump(model,    'model.pkl')
joblib.dump(le_vucut, 'le_vucut.pkl')
joblib.dump(le_kesim, 'le_kesim.pkl')

print("\n✅ Model kaydedildi: model.pkl")