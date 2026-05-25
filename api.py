from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import Optional
import pandas as pd
import joblib

app = FastAPI()

# Model dosyalarını yükle
model    = joblib.load('model.pkl')
le_vucut = joblib.load('le_vucut.pkl')
le_kesim = joblib.load('le_kesim.pkl')


class AnalizIstegi(BaseModel):
    urun_id: Optional[int] = None
    vucut_tipi: str
    urun_kesim: str
    kumas_esnek: int


def tavsiye_olustur(vucut_tipi, urun_kesim, skor):
    if skor >= 75:
        return f"{vucut_tipi} vücut tipiniz için {urun_kesim} kesim mükemmel uyum sağlar."
    elif skor >= 55:
        return f"{vucut_tipi} vücut tipiniz için {urun_kesim} kesim orta düzeyde uygundur."
    else:
        return f"{vucut_tipi} vücut tipiniz için {urun_kesim} kesim önerilmez."


@app.post('/analiz')
def analiz(istek: AnalizIstegi):
    try:
        v = le_vucut.transform([istek.vucut_tipi])[0]
        k = le_kesim.transform([istek.urun_kesim])[0]
    except ValueError:
        raise HTTPException(status_code=400, detail='Geçersiz vucut_tipi veya urun_kesim değeri')

    girdi = pd.DataFrame(
        [[v, k, istek.kumas_esnek]],
        columns=['vucut_encoded', 'kesim_encoded', 'kumas_esnek']
    )

    skor = round(model.predict(girdi)[0], 1)

    if skor >= 75:   risk = 'Düşük'
    elif skor >= 55: risk = 'Orta'
    else:            risk = 'Yüksek'

    return {
        'urun_id':    istek.urun_id,
        'vucut_tipi': istek.vucut_tipi,
        'uyum_skoru': skor,
        'iade_riski': risk,
        'tavsiye':    tavsiye_olustur(istek.vucut_tipi, istek.urun_kesim, skor)
    }