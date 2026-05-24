from transformers import pipeline
from collections import Counter

# Türkçe sentiment modeli
duygu_modeli = pipeline(
    "sentiment-analysis",
    model="savasy/bert-base-turkish-sentiment-cased"
)

# Tema sözlüğü
temalar = {
    'beden': ['beden', 'numara', 'küçük', 'büyük', 'dar', 'bol'],
    'kumas': ['kumaş', 'kalite', 'ince', 'kalın', 'sağlam'],
    'kalip': ['kalıp', 'kesim', 'model', 'şekil'],
    'iade':  ['iade', 'geri gönderdim', 'iade ettim']
}


# Duygu analizi
def duygu_skoru_hesapla(metin):

    sonuc = duygu_modeli(metin)[0]

    if sonuc['label'].lower() == 'positive':
        return {
            "etiket": "Olumlu",
            "skor": round(sonuc['score'], 2)
        }

    return {
        "etiket": "Olumsuz",
        "skor": round(-sonuc['score'], 2)
    }


# Tema bulma
def tema_bul(metin):

    metin_lower = metin.lower()

    for tema, kelimeler in temalar.items():

        if any(kelime in metin_lower for kelime in kelimeler):
            return tema

    return "genel"


# Tekrar kontrol
def tekrar_kontrol(bulgular, esik=3):

    temalar_liste = [b['tema'] for b in bulgular]

    sayac = Counter(temalar_liste)

    uyarilar = []

    for tema, sayi in sayac.items():

        if sayi >= esik:

            uyarilar.append(
                f"UYARI: {tema} teması {sayi} kez tekrarlandı!"
            )

    return uyarilar