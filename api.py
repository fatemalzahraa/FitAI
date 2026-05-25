from flask import Flask, request, jsonify
import pandas as pd
import joblib

app = Flask(__name__)

# تحميل النموذج من الملف
model    = joblib.load('model.pkl')
le_vucut = joblib.load('le_vucut.pkl')
le_kesim = joblib.load('le_kesim.pkl')


def olustur_tavsiye(vucut_tipi, urun_kesim, skor):
    if skor >= 75:
        return f"{vucut_tipi} vücut tipiniz için {urun_kesim} kesim mükemmel uyum sağlar."
    elif skor >= 55:
        return f"{vucut_tipi} vücut tipiniz için {urun_kesim} kesim orta düzeyde uygundur."
    else:
        return f"{vucut_tipi} vücut tipiniz için {urun_kesim} kesim önerilmez."

@app.route('/analiz', methods=['POST'])
def analiz():
    body = request.get_json()

    for field in ['vucut_tipi', 'urun_kesim', 'kumas_esnek']:
        if field not in body:
            return jsonify({'hata': f'{field} eksik'}), 400

    try:
        v = le_vucut.transform([body['vucut_tipi']])[0]
        k = le_kesim.transform([body['urun_kesim']])[0]
    except ValueError:
        return jsonify({'hata': 'Geçersiz vucut_tipi veya urun_kesim'}), 400

    input_df = pd.DataFrame(
        [[v, k, body['kumas_esnek']]],
        columns=['vucut_encoded', 'kesim_encoded', 'kumas_esnek']
    )

    skor = round(model.predict(input_df)[0], 1)

    if skor >= 75:   risk = 'Düşük'
    elif skor >= 55: risk = 'Orta'
    else:            risk = 'Yüksek'

    return jsonify({
    'urun_id':    body.get('urun_id'),
    'vucut_tipi': body['vucut_tipi'],
    'uyum_skoru': skor,
    'iade_riski': risk,
    'tavsiye':    olustur_tavsiye(body['vucut_tipi'], body['urun_kesim'], skor)
})

if __name__ == '__main__':
    app.run(debug=True, port=5000)