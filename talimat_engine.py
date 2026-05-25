import re
import joblib
import pandas as pd

# Modeli yükle
model    = joblib.load('model.pkl')
le_vucut = joblib.load('le_vucut.pkl')
le_kesim = joblib.load('le_kesim.pkl')

def talimat_isle(talimat_metni):
    """
    Türkçe komutu anlar ve modele uygular
    """
    talimat = talimat_metni.lower()
    
    # 1. Kesim türünü bul
    kesim = None
    for k in ['slim', 'regular', 'oversize']:
        if k in talimat:
            kesim = k.capitalize()
            break
    
    # 2. Vücut tipini bul
    vucut = None
    for v in ['elma', 'armut', 'kum saati', 'dikdortgen', 
              'ters ucgen', 'oval', 'elmas', 'uzun', 'atletik']:
        if v in talimat:
            vucut = v.title()
            break
    
    # 3. Yüzde ve işlem yönünü bul
    sayi = re.search(r'%(\d+)', talimat)
    yuzde = int(sayi.group(1)) if sayi else 10
    
    artir = 'artır' in talimat or 'artir' in talimat
    azalt = 'azalt' in talimat
    
    # 4. Skor güncellemesi uygula
    sonuclar = []
    test_cases = []
    
    if kesim:
        test_cases = [(v, kesim, 1) for v in le_vucut.classes_]
    elif vucut:
        test_cases = [(vucut, k, 1) for k in le_kesim.classes_]
    
    for v, k, esnek in test_cases:
        input_df = pd.DataFrame(
            [[le_vucut.transform([v])[0], 
              le_kesim.transform([k])[0], 
              esnek]],
            columns=['vucut_encoded', 'kesim_encoded', 'kumas_esnek']
        )
        
        skor = model.predict(input_df)[0]
        
        # Skor güncelle
        if artir:
            yeni_skor = min(100, round(skor * (1 + yuzde/100), 1))
        elif azalt:
            yeni_skor = max(0, round(skor * (1 - yuzde/100), 1))
        else:
            yeni_skor = round(skor, 1)
        
        sonuclar.append({
            'vucut_tipi': v,
            'urun_kesim': k,
            'eski_skor': round(skor, 1),
            'yeni_skor': yeni_skor,
            'degisim': round(yeni_skor - skor, 1)
        })
    
    return {
        'talimat': talimat_metni,
        'anlasilan': (
            f"{'Artır' if artir else 'Azalt'} "
            f"%{yuzde} → kesim: {kesim or 'Tümü'}, "
            f"vücut: {vucut or 'Tümü'}"
        ),
        'etkilenen': len(sonuclar),
        'sonuclar': sonuclar
    }

# Test komutları
talimatlar = [
    "Oversize tişörtlerin skorunu %20 artır",
    "Slim kesimin skorunu %10 azalt",
    "Kum Saati vücut tipinin skorunu %15 artır",
]

for t in talimatlar:
    sonuc = talimat_isle(t)
    
    print(f"\n📝 Talimat: {sonuc['talimat']}")
    print(f"🧠 Anlaşılan: {sonuc['anlasilan']}")
    print(f"📊 Etkilenen ürün sayısı: {sonuc['etkilenen']}")
    
    for s in sonuc['sonuclar'][:3]:
        print(
            f"   {s['vucut_tipi']:<14} "
            f"{s['urun_kesim']:<10} "
            f"{s['eski_skor']} → {s['yeni_skor']} "
            f"({'+' if s['degisim'] > 0 else ''}{s['degisim']})"
        )

# Undo işlemleri için geçmiş kayıt listesi
tarihce = []

def talimat_kaydet(sonuc):
    tarihce.append(sonuc)
    print(f"💾 İşlem kaydedildi. Toplam geçmiş: {len(tarihce)}")

def geri_al():
    if not tarihce:
        print("⚠️ Geri alınacak işlem bulunamadı.")
        return
    
    son = tarihce.pop()
    
    print(f"↩️ Geri alındı: {son['talimat']}")
    print(f"   {len(son['sonuclar'])} ürün eski skoruna döndürüldü.")
    
    return son

# Undo testi
print("\n─── Undo Testi ───")

sonuc = talimat_isle("Oversize tişörtlerin skorunu %20 artır")

talimat_kaydet(sonuc)

geri_al()

geri_al()  # Tekrar çalıştırılırsa işlem olmadığını söylemeli