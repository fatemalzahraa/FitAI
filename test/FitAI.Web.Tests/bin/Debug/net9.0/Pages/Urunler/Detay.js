$(function () {

    // =============================================
    // URL'den ID al
    // =============================================
    var pathParcalar = window.location.pathname.split('/');
    var urunId = parseInt(pathParcalar[pathParcalar.length - 1]) || 0;

    var renkler = ['#4f46e5', '#0ea5e9', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];
    function renk(id) { return renkler[id % renkler.length]; }

    var magazaAdi = { 1: 'SportZone TR', 2: 'FashionHub', 3: 'ActiveWear', 4: 'FitStyle', 5: 'RunnerShop' };

    function yildizlar(puan) {
        if (!puan) return '<span class="text-muted">—</span>';
        var html = '';
        for (var i = 1; i <= 5; i++) {
            html += '<i class="fas fa-star' + (i <= puan ? '' : '') + ' yorum-puan-yildiz"></i>';
        }
        return html;
    }

    // =============================================
    // Veri — Urun + Yorum entity
    // =============================================
    var tumUrunler = [
        { id: 1, magazaId: 1, ad: 'Slim Fit Spor Tayt',         aciklama: 'Yüksek bel, dört yönlü esnek kumaş ile maksimum hareket özgürlüğü sağlar. Uzun süre konforlu kullanım için tasarlanmıştır.',  kesimTuru: 'Slim',     kumasEsnek: true,  silindiMi: false },
        { id: 2, magazaId: 1, ad: 'Regular Fit Koşu Şortu',     aciklama: 'Hafif ve nefes alabilir polyester kumaş, uzun koşular için ideal. Cepli tasarım.',                                              kesimTuru: 'Regular',  kumasEsnek: false, silindiMi: false },
        { id: 3, magazaId: 2, ad: 'Oversize Kapüşonlu Sweat',   aciklama: 'Günlük kullanım için bol kesim, %100 pamuk içeriği ile doğal his.',                                                             kesimTuru: 'Oversize', kumasEsnek: false, silindiMi: false },
        { id: 4, magazaId: 3, ad: 'Athletic Fit Tişört',        aciklama: 'Atletik vücut için özel tasarım, nem emici kumaş teknolojisi.',                                                                  kesimTuru: 'Athletic', kumasEsnek: true,  silindiMi: false },
        { id: 5, magazaId: 4, ad: 'Slim Fit Yoga Pantolonu',    aciklama: null,                                                                                                                             kesimTuru: 'Slim',     kumasEsnek: true,  silindiMi: false },
        { id: 6, magazaId: 5, ad: 'Regular Fit Antrenman Üstü', aciklama: 'Çok yönlü spor üstü, tüm spor dalları için uygundur.',                                                                          kesimTuru: 'Regular',  kumasEsnek: true,  silindiMi: false },
        { id: 7, magazaId: 2, ad: 'Eski Model Eşofman',         aciklama: 'Kaldırılan ürün.',                                                                                                              kesimTuru: 'Regular',  kumasEsnek: null,  silindiMi: true  },
    ];

    // Yorum entity: UrunId, MagazaId, YorumMetni, Puan, NlpIslendi
    var tumYorumlar = [
        { id: 1,  urunId: 1, magazaId: 1, yorumMetni: 'Çok rahat, spor yaparken harika hissettiriyor.',        puan: 5, nlpIslendi: true  },
        { id: 2,  urunId: 1, magazaId: 1, yorumMetni: 'Beden biraz küçük geldi, bir beden büyük alın.',        puan: 3, nlpIslendi: true  },
        { id: 3,  urunId: 1, magazaId: 1, yorumMetni: 'Kumaş kalitesi mükemmel, tekrar alacağım.',             puan: 5, nlpIslendi: false },
        { id: 4,  urunId: 1, magazaId: 1, yorumMetni: 'Renk fotoğraftakinden farklı ama yine de güzel.',       puan: 4, nlpIslendi: false },
        { id: 5,  urunId: 1, magazaId: 1, yorumMetni: 'Dikişler sağlam, uzun ömürlü görünüyor.',               puan: 5, nlpIslendi: true  },
        { id: 6,  urunId: 1, magazaId: 1, yorumMetni: 'Fiyatına göre çok iyi kalite.',                         puan: 4, nlpIslendi: false },
        { id: 7,  urunId: 1, magazaId: 1, yorumMetni: 'Beklentilerimi karşılamadı maalesef.',                   puan: 2, nlpIslendi: true  },
        { id: 8,  urunId: 2, magazaId: 1, yorumMetni: 'Hafif ve dayanıklı, koşu için ideal.',                  puan: 4, nlpIslendi: true  },
        { id: 9,  urunId: 2, magazaId: 1, yorumMetni: 'Rengi biraz soluk ama kalite iyi.',                     puan: 3, nlpIslendi: false },
        { id: 10, urunId: 3, magazaId: 2, yorumMetni: 'Tam aradığım oversize model, çok şık.',                 puan: 5, nlpIslendi: true  },
        { id: 11, urunId: 4, magazaId: 3, yorumMetni: 'Athletic kesim vücudu güzel gösteriyor.',               puan: 4, nlpIslendi: true  },
        { id: 12, urunId: 5, magazaId: 4, yorumMetni: 'Yoga derslerinde çok rahat kullanıyorum.',              puan: 5, nlpIslendi: true  },
    ];

    // =============================================
    // Sayfayı Yükle
    // =============================================
    function sayfaYukle() {
        // TODO: abp.ajax ile ProductController + ReviewController'dan çekilecek
        // abp.ajax({ url: abp.appPath + 'api/app/urun/' + urunId })
        //     .done(function(u) { urunGoster(u); });

        var urun = tumUrunler.find(function(u){ return u.id === urunId; });

        $('#yukleniyor').hide();

        if (!urun) {
            $('#bulunamadi').show();
            return;
        }

        $('#anaIcerik').show();
        urunGoster(urun);
    }

    // =============================================
    // Ürün Bilgilerini Doldur
    // =============================================
    function urunGoster(u) {
        var r = renk(u.id);

        // Başlık
        $('#baslikIkon').html(
            '<div style="width:56px;height:56px;border-radius:14px;background:' + r + '20;color:' + r +
            ';display:flex;align-items:center;justify-content:center;font-size:1.4rem">' +
            '<i class="fas fa-tshirt"></i></div>'
        );
        $('#baslikAd').text(u.ad);
        $('#baslikMagaza').html('<i class="fas fa-store me-1"></i>' + (magazaAdi[u.magazaId] || '—'));
        $('#duzenleBtn').attr('href', '/Urunler?id=' + u.id);

        // Bilgi satırları
        $('#bilgiId').text('#' + u.id);
        $('#bilgiMagaza').text(magazaAdi[u.magazaId] || '—');
        $('#bilgiKesim').html(u.kesimTuru
            ? '<span class="kesim-badge">' + u.kesimTuru + ' Fit</span>'
            : '<span class="text-muted">Belirtilmemiş</span>');
        $('#bilgiKumas').html(u.kumasEsnek === true
            ? '<span class="kumas-esnek-badge"><i class="fas fa-expand-arrows-alt me-1"></i>Esnek</span>'
            : u.kumasEsnek === false
                ? '<span class="text-muted small">Standart</span>'
                : '<span class="text-muted">—</span>');
        $('#bilgiDurum').html(u.silindiMi
            ? '<span class="durum-silindi"><i class="fas fa-times me-1"></i>Silinmiş</span>'
            : '<span class="durum-aktif"><i class="fas fa-check me-1"></i>Aktif</span>');

        // Açıklama
        $('#aciklamaMetni').text(u.aciklama || 'Bu ürün için açıklama eklenmemiş.');

        // İstatistikler
        var yorumlar = tumYorumlar.filter(function(y){ return y.urunId === u.id; });
        var puanlar  = yorumlar.filter(function(y){ return y.puan; });
        var ortPuan  = puanlar.length
            ? (puanlar.reduce(function(t,y){ return t + y.puan; }, 0) / puanlar.length).toFixed(1)
            : null;
        var nlpSayisi = yorumlar.filter(function(y){ return y.nlpIslendi; }).length;

        $('#istatOrtPuan').html(ortPuan ? '★ ' + ortPuan : '—');
        $('#istatYorumSayisi').text(yorumlar.length);
        $('#istatNlpIslendi').text(nlpSayisi);
        $('#istatNlpBekliyor').text(yorumlar.length - nlpSayisi);
        $('#toplamYorumYazi').text(yorumlar.length + ' yorum');

        // Puan Dağılımı
        puanDagilimGoster(u.id);

        // Yorumlar
        yorumlariGoster(u.id);
    }

    // =============================================
    // Puan Dağılımı
    // =============================================
    function puanDagilimGoster(urunId) {
        var yorumlar = tumYorumlar.filter(function(y){ return y.urunId === urunId && y.puan; });
        var toplam   = yorumlar.length || 1;
        var $alan    = $('#puanDagilimi');
        $alan.empty();

        for (var puan = 5; puan >= 1; puan--) {
            var sayi   = yorumlar.filter(function(y){ return y.puan === puan; }).length;
            var yuzde  = Math.round((sayi / toplam) * 100);
            var yildiz = '';
            for (var i = 0; i < puan; i++) yildiz += '★';

            $alan.append(
                '<div class="puan-satir">' +
                    '<span class="puan-etiket text-warning fw-600">' + yildiz + '</span>' +
                    '<div class="puan-bar-wrapper">' +
                        '<div class="puan-bar" style="width:' + yuzde + '%"></div>' +
                    '</div>' +
                    '<span class="puan-adet text-muted">' + sayi + '</span>' +
                '</div>'
            );
        }
    }

    // =============================================
    // Yorum Listesi
    // =============================================
    function yorumlariGoster(urunId) {
        var filtre   = $('#yorumFiltre').val();
        var siralama = $('#yorumSirala').val();

        var yorumlar = tumYorumlar.filter(function(y){ return y.urunId === urunId; });

        // Filtre
        if (filtre === 'nlp_islendi')  yorumlar = yorumlar.filter(function(y){ return y.nlpIslendi; });
        if (filtre === 'nlp_bekliyor') yorumlar = yorumlar.filter(function(y){ return !y.nlpIslendi; });
        if (filtre === 'puan_5')       yorumlar = yorumlar.filter(function(y){ return y.puan === 5; });
        if (filtre === 'puan_1')       yorumlar = yorumlar.filter(function(y){ return y.puan === 1; });

        // Sıralama
        if (siralama === 'puan_yuksek') yorumlar.sort(function(a,b){ return (b.puan||0)-(a.puan||0); });
        else if (siralama === 'puan_dusuk') yorumlar.sort(function(a,b){ return (a.puan||0)-(b.puan||0); });
        else yorumlar.sort(function(a,b){ return b.id - a.id; });

        var $liste = $('#yorumListesi');
        $liste.empty();

        if (yorumlar.length === 0) {
            $liste.html(
                '<div class="text-center text-muted py-4">' +
                '<i class="fas fa-comment-slash fa-2x mb-2 d-block"></i>Bu filtreye uygun yorum bulunamadı.</div>'
            );
            return;
        }

        yorumlar.forEach(function(y) {
            var nlpHtml = y.nlpIslendi
                ? '<span class="yorum-nlp-tag nlp-islendi"><i class="fas fa-robot me-1"></i>NLP Analiz Edildi</span>'
                : '<span class="yorum-nlp-tag nlp-bekliyor"><i class="fas fa-clock me-1"></i>NLP Bekliyor</span>';

            $liste.append(
                '<div class="yorum-item">' +
                    '<div class="d-flex align-items-center justify-content-between mb-1">' +
                        '<span>' + yildizlar(y.puan) + '</span>' +
                        nlpHtml +
                    '</div>' +
                    '<div class="yorum-metin">"' + y.yorumMetni + '"</div>' +
                '</div>'
            );
        });
    }

    // Filtre & sıralama değişince yorumları yenile
    $('#yorumFiltre, #yorumSirala').on('change', function() {
        yorumlariGoster(urunId);
    });

    // =============================================
    // Silme
    // =============================================
    $('#silBtn').on('click', function() {
        new bootstrap.Modal(document.getElementById('silOnayModal')).show();
    });

    $('#silOnayBtn').on('click', function() {
        // TODO: abp.ajax DELETE
        abp.notify.warn('Ürün silindi.', 'Silindi');
        bootstrap.Modal.getInstance(document.getElementById('silOnayModal')).hide();
        setTimeout(function(){ window.location.href = '/Urunler/Liste'; }, 1000);
    });

    // =============================================
    // Başlat
    // =============================================
    sayfaYukle();

});
