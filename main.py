from fastapi import FastAPI

from services.sentiment import (
    duygu_skoru_hesapla,
    tema_bul,
    tekrar_kontrol
)

from data.mock_reviews import yorumlar

app = FastAPI()


@app.get("/")
def home():

    return {
        "message": "FitAI NLP Service Running"
    }


@app.get("/yorum-analiz")
def yorum_analiz():

    bulgular = []

    for yorum in yorumlar:

        metin = yorum["yorumMetni"]

        duygu = duygu_skoru_hesapla(metin)

        tema = tema_bul(metin)

        bulgu = {

            "urunId": yorum["urunId"],

            "magazaId": 1,

            "yorum": metin,

            "tema": tema,

            "duyguSkoru": duygu["skor"],

            "duyguEtiketi": duygu["etiket"],

            "tekrarSayisi": 1,

            "oneriMetni": "",

            "durum": "Beklemede"
        }

        if tema == "beden":

            bulgu["oneriMetni"] = (
                "Beden sorunu tekrar ediyor, açıklama güncellenmeli."
            )

        elif tema == "kumas":

            bulgu["oneriMetni"] = (
                "Kumaş kalitesi kontrol edilmeli."
            )

        else:

            bulgu["oneriMetni"] = (
                "Genel müşteri geri bildirimi incelenmeli."
            )

        bulgular.append(bulgu)

    uyarilar = tekrar_kontrol(bulgular)

    return {

        "analizSonuclari": bulgular,

        "uyarilar": uyarilar
    }