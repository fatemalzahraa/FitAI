$(function () {

    // =============================================
    // Yardımcı
    // =============================================
    var renkler = ['#4f46e5', '#0ea5e9', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];
    function renk(id) { return renkler[id % renkler.length]; }

    var magazaAdi = { 1: 'SportZone TR', 2: 'FashionHub', 3: 'ActiveWear', 4: 'FitStyle', 5: 'RunnerShop' };

    function yildizlar(puan) {
        if (!puan) return '<span class="text-muted small">—</span>';
        var html = '';
        for (var i = 1; i <= 5; i++) {
            html += '<i class="fas fa-star' + (i <= puan ? '' : '-o') + ' puan-yildiz"></i>';
        }
        return html + ' <span class="text-muted small">(' + puan + ')</span>';
    }

    function nlpBadge(islendi) {
        return islendi
            ? '<span class="yorum-nlp-tag nlp-islendi"><i class="fas fa-robot me-1"></i>NLP Analiz Edildi</span>'
            : '<span class="yorum-nlp-tag nlp-bekliyor"><i class="fas fa-clock me-1"></i>NLP Bekliyor</span>';
    }

    // =============================================
    // Veri — Urun + Yorum entity alanlarına göre
    // =============================================
    var tumUrunler = [];
    var tumYorumlar = [];

    function yukleVeriler() {
        // TODO: abp.ajax ile ProductController + ReviewController'dan çekilecek

        tumUrunler = [
            { id: 1, magazaId: 1, ad: 'Slim Fit Spor Tayt',       aciklama: 'Yüksek bel, dört yönlü esnek kumaş ile maksimum hareket özgürlüğü sağlar.',  kesimTuru: 'Slim',     kumasEsnek: true,  silindiMi: false },
            { id: 2, magazaId: 1, ad: 'Regular Fit Koşu Şortu',   aciklama: 'Hafif ve nefes alabilir polyester, uzun koşular için ideal.',                  kesimTuru: 'Regular',  kumasEsnek: false, silindiMi: false },
            { id: 3, magazaId: 2, ad: 'Oversize Kapüşonlu Sweat', aciklama: 'Günlük kullanım için bol kesim, %100 pamuk.',                                  kesimTuru: 'Oversize', kumasEsnek: false, silindiMi: false },
            { id: 4, magazaId: 3, ad: 'Athletic Fit Tişört',      aciklama: 'Atletik vücut için özel tasarım, nem emici kumaş.',                             kesimTuru: 'Athletic', kumasEsnek: true,  silindiMi: false },
            { id: 5, magazaId: 4, ad: 'Slim Fit Yoga Pantolonu',  aciklama: null,                                                                            kesimTuru: 'Slim',     kumasEsnek: true,  silindiMi: false },
            { id: 6, magazaId: 5, ad: 'Regular Fit Antrenman Üstü', aciklama: 'Çok yönlü spor üstü, tüm spor dalları için uygundur.',                       kesimTuru: 'Regular',  kumasEsnek: true,  silindiMi: false },
            { id: 7, magazaId: 2, ad: 'Eski Model Eşofman',       aciklama: 'Kaldırılan ürün.',                                                             kesimTuru: 'Regular',  kumasEsnek: null,  silindiMi: true  },
        ];

        // Yorum entity: UrunId, MagazaId, YorumMetni, Puan, NlpIslendi
        tumYorumlar = [
            { id: 1, urunId: 1, magazaId: 1, yorumMetni: 'Çok rahat, spor yaparken harika hissettiriyor.',       puan: 5, nlpIslendi: true  },
            { id: 2, urunId: 1, magazaId: 1, yorumMetni: 'Beden biraz küçük geldi, bir beden büyük alın.',       puan: 3, nlpIslendi: true  },
            { id: 3, urunId: 1, magazaId: 1, yorumMetni: 'Kumaş kalitesi mükemmel, tekrar alacağım.',            puan: 5, nlpIslendi: false },
            { id: 4, urunId: 2, magazaId: 1, yorumMetni: 'Hafif ve dayanıklı, koşu için ideal.',                 puan: 4, nlpIslendi: true  },
            { id: 5, urunId: 2, magazaId: 1, yorumMetni: 'Rengi biraz soluk ama kalite iyi.',                    puan: 3, nlpIslendi: false },
            { id: 6, urunId: 3, magazaId: 2, yorumMetni: 'Tam aradığım oversize model, çok şık.',                puan: 5, nlpIslendi: true  },
            { id: 7, urunId: 4, magazaId: 3, yorumMetni: 'Athletic kesim vücudu güzel gösteriyor.',              puan: 4, nlpIslendi: true  },
            { id: 8, urunId: 4, magazaId: 3, yorumMetni: 'Dikişler sağlam, uzun ömürlü görünüyor.',              puan: 5, nlpIslendi: false },
            { id: 9, urunId: 5, magazaId: 4, yorumMetni: 'Yoga derslerinde çok rahat kullanıyorum.',             puan: 5, nlpIslendi: true  },
            { id:10, urunId: 6, magazaId: 5, yorumMetni: 'Her spora uyuyor, çok fonksiyonel.',                   puan: 4, nlpIslendi: false },
        ];

        tabloYenile();
    }

    // =============================================
    // Yorum istatistikleri hesapla
    // =============================================
    function urunYorumIstatistik(urunId) {
        var yorumlar = tumYorumlar.filter(function(y){ return y.urunId === urunId; });
        var puanlar  = yorumlar.filter(function(y){ return y.puan; });
        var nlpSayisi = yorumlar.filter(function(y){ return y.nlpIslendi; }).length;
        var ortPuan  = puanlar.length
            ? (puanlar.reduce(function(t,y){ return t + y.puan; }, 0) / puanlar.length).toFixed(1)
            : null;
        return { toplam: yorumlar.length, ortPuan: ortPuan, nlpSayisi: nlpSayisi, bekleyen: yorumlar.length - nlpSayisi };
    }

    // =============================================
    // Filtreleme
    // =============================================
    function filtreliUrunler() {
        var arama  = $('#aramaInput').val().toLowerCase();
        var magaza = $('#magazaFiltre').val();
        var kumas  = $('input[name="kumasRadio"]:checked').val();
        var nlp    = $('#nlpFiltre').val();
        var seciliKesimler = [];
        $('.kesim-check:checked').each(function(){ seciliKesimler.push($(this).val()); });
        var siralama = $('#siralamaSelect').val();

        var filtre = tumUrunler.filter(function(u) {
            var aramaUyumu  = !arama  || u.ad.toLowerCase().includes(arama);
            var magazaUyumu = !magaza || u.magazaId == magaza;
            var kesimUyumu  = seciliKesimler.length === 0 || seciliKesimler.includes(u.kesimTuru);
            var kumasUyumu  = !kumas  ||
                (kumas === 'esnek'    && u.kumasEsnek === true) ||
                (kumas === 'standart' && u.kumasEsnek === false);
            var nlpUyumu = true;
            if (nlp) {
                var ist = urunYorumIstatistik(u.id);
                nlpUyumu = nlp === 'islendi'   ? ist.nlpSayisi > 0
                         : nlp === 'islenmedi' ? ist.bekleyen > 0
                         : true;
            }
            return aramaUyumu && magazaUyumu && kesimUyumu && kumasUyumu && nlpUyumu;
        });

        // Sıralama
        if (siralama === 'ad') {
            filtre.sort(function(a,b){ return a.ad.localeCompare(b.ad, 'tr'); });
        } else if (siralama === 'puan') {
            filtre.sort(function(a,b){
                var pA = urunYorumIstatistik(a.id).ortPuan || 0;
                var pB = urunYorumIstatistik(b.id).ortPuan || 0;
                return pB - pA;
            });
        } else if (siralama === 'kesim') {
            filtre.sort(function(a,b){ return (a.kesimTuru||'').localeCompare(b.kesimTuru||'', 'tr'); });
        }

        return filtre;
    }

    // =============================================
    // Kart Render
    // =============================================
    function kartRender(urunler) {
        var $alan = $('#urunKartlari');
        $alan.empty();

        if (urunler.length === 0) {
            $alan.html('<div class="col-12 text-center py-5 text-muted"><i class="fas fa-search fa-2x mb-2 d-block"></i>Sonuç bulunamadı.</div>');
            return;
        }

        urunler.forEach(function(u) {
            var ist   = urunYorumIstatistik(u.id);
            var r     = renk(u.id);
            var badgeler = '';
            if (u.kesimTuru)     badgeler += '<span class="kesim-badge">' + u.kesimTuru + ' Fit</span>';
            if (u.kumasEsnek)    badgeler += '<span class="kumas-esnek-badge"><i class="fas fa-expand-arrows-alt me-1"></i>Esnek</span>';
            if (ist.nlpSayisi)   badgeler += '<span class="nlp-badge nlp-islendi"><i class="fas fa-robot me-1"></i>NLP</span>';
            else if (ist.toplam) badgeler += '<span class="nlp-badge nlp-bekliyor"><i class="fas fa-clock me-1"></i>NLP Bekliyor</span>';

            $alan.append(
                '<div class="col-xl-4 col-md-6">' +
                    '<div class="urun-kart' + (u.silindiMi ? ' silindi' : '') + ' urun-kart-ac" data-id="' + u.id + '">' +
                        '<div class="urun-kart-header">' +
                            '<div class="urun-kart-ikon" style="background:' + r + '20;color:' + r + '">' +
                                '<i class="fas fa-tshirt"></i>' +
                            '</div>' +
                            '<div>' +
                                '<div class="urun-kart-ad">' + u.ad + '</div>' +
                                '<div class="urun-kart-magaza">' +
                                    '<i class="fas fa-store me-1"></i>' + (magazaAdi[u.magazaId] || '—') +
                                '</div>' +
                            '</div>' +
                        '</div>' +
                        '<div class="urun-kart-body">' +
                            '<div class="urun-kart-aciklama">' + (u.aciklama ? u.aciklama.substring(0,80) + (u.aciklama.length>80?'...':'') : '<span class="text-muted fst-italic">Açıklama eklenmemiş.</span>') + '</div>' +
                            '<div class="urun-kart-badgeler">' + badgeler + '</div>' +
                        '</div>' +
                        '<div class="urun-kart-footer">' +
                            '<div>' + yildizlar(ist.ortPuan) + '</div>' +
                            '<span class="yorum-sayisi"><i class="fas fa-comment me-1"></i>' + ist.toplam + ' yorum</span>' +
                        '</div>' +
                    '</div>' +
                '</div>'
            );
        });
    }

    // =============================================
    // Liste Tablo Render
    // =============================================
    function listeRender(urunler) {
        var $tbody = $('#urunListeTablosu');
        $tbody.empty();

        if (urunler.length === 0) {
            $tbody.html('<tr><td colspan="8" class="text-center py-4 text-muted">Sonuç bulunamadı.</td></tr>');
            return;
        }

        urunler.forEach(function(u) {
            var ist = urunYorumIstatistik(u.id);
            $tbody.append(
                '<tr class="urun-kart-ac" data-id="' + u.id + '">' +
                    '<td><strong>' + u.ad + '</strong></td>' +
                    '<td class="text-muted small">' + (magazaAdi[u.magazaId]||'—') + '</td>' +
                    '<td>' + (u.kesimTuru ? '<span class="kesim-badge">' + u.kesimTuru + '</span>' : '—') + '</td>' +
                    '<td>' + (u.kumasEsnek ? '<span class="kumas-esnek-badge">Esnek</span>' : '<span class="text-muted small">Standart</span>') + '</td>' +
                    '<td>' + yildizlar(ist.ortPuan) + '</td>' +
                    '<td class="text-muted small">' + ist.toplam + '</td>' +
                    '<td>' + (ist.nlpSayisi > 0 ? '<span class="nlp-badge nlp-islendi">' + ist.nlpSayisi + ' analiz</span>' : '<span class="nlp-badge nlp-bekliyor">Bekliyor</span>') + '</td>' +
                    '<td><i class="fas fa-chevron-right text-muted small"></i></td>' +
                '</tr>'
            );
        });
    }

    // =============================================
    // Detay Modal
    // =============================================
    $(document).on('click', '.urun-kart-ac', function() {
        var id = $(this).data('id');
        var u  = tumUrunler.find(function(x){ return x.id === id; });
        if (!u) return;

        var ist = urunYorumIstatistik(u.id);
        var r   = renk(u.id);

        $('#detayModalBaslik').html('<i class="fas fa-tshirt me-2" style="color:' + r + '"></i>' + u.ad);
        $('#detayIkon').html('<div style="width:64px;height:64px;border-radius:14px;background:' + r + '20;color:' + r + ';display:flex;align-items:center;justify-content:center;font-size:1.6rem"><i class="fas fa-tshirt"></i></div>');
        $('#detayAd').text(u.ad);
        $('#detayMagaza').html('<i class="fas fa-store me-1"></i>' + (magazaAdi[u.magazaId]||'—'));
        $('#detayOrtPuan').text(ist.ortPuan ? '★ ' + ist.ortPuan : '—');
        $('#detayYorumSayisi').text(ist.toplam);
        $('#detayNlpSayisi').text(ist.nlpSayisi);
        $('#detayBekleyenNlp').text(ist.bekleyen);
        $('#detayDuzenleBtn').attr('href', '/Urunler?id=' + u.id);

        // Badge'ler
        var b = '';
        if (u.kesimTuru)  b += '<span class="kesim-badge">' + u.kesimTuru + ' Fit</span>';
        if (u.kumasEsnek === true)  b += '<span class="kumas-esnek-badge"><i class="fas fa-expand-arrows-alt me-1"></i>Esnek Kumaş</span>';
        if (u.silindiMi)  b += '<span class="badge bg-danger-soft text-danger">Silinmiş</span>';
        $('#detayBadgeler').html(b);

        $('#detayAciklama').text(u.aciklama || 'Bu ürün için açıklama eklenmemiş.');

        // Yorumlar
        yorumlariGoster(u.id, 'yeni');

        new bootstrap.Modal(document.getElementById('urunDetayModal')).show();
    });

    function yorumlariGoster(urunId, siralama) {
        var yorumlar = tumYorumlar.filter(function(y){ return y.urunId === urunId; });

        if (siralama === 'puan_yuksek') yorumlar.sort(function(a,b){ return (b.puan||0)-(a.puan||0); });
        else if (siralama === 'puan_dusuk') yorumlar.sort(function(a,b){ return (a.puan||0)-(b.puan||0); });
        else if (siralama === 'nlp') yorumlar.sort(function(a,b){ return a.nlpIslendi - b.nlpIslendi; });
        else yorumlar.sort(function(a,b){ return b.id - a.id; });

        var $liste = $('#yorumListesi');
        $liste.empty();

        if (yorumlar.length === 0) {
            $liste.html('<div class="text-center text-muted py-4"><i class="fas fa-comment-slash fa-2x mb-2 d-block"></i>Henüz yorum yok.</div>');
            return;
        }

        yorumlar.forEach(function(y) {
            var puan = '';
            for (var i=1; i<=5; i++) puan += '<i class="fas fa-star' + (i<=(y.puan||0)?'':'-o') + ' puan-yildiz"></i>';

            $liste.append(
                '<div class="yorum-kart">' +
                    '<div class="d-flex align-items-center justify-content-between mb-1">' +
                        '<span class="yorum-puan">' + puan + '</span>' +
                        nlpBadge(y.nlpIslendi) +
                    '</div>' +
                    '<div class="yorum-metin">"' + y.yorumMetni + '"</div>' +
                '</div>'
            );
        });
    }

    $('#yorumSirala').on('change', function() {
        var aktifUrunId = parseInt($('#urunDetayModal .urun-kart-ac').data('id') || 0);
        // Aktif modaldan id al
        var modalBaslik = $('#detayModalBaslik').text().trim();
        var bulunan = tumUrunler.find(function(u){ return u.ad === modalBaslik; });
        if (bulunan) yorumlariGoster(bulunan.id, $(this).val());
    });

    // =============================================
    // Görünüm Değiştir
    // =============================================
    $('#kartGorunumBtn').on('click', function() {
        $(this).addClass('active');
        $('#listeGorunumBtn').removeClass('active');
        $('#kartGorunum').show();
        $('#listeGorunum').hide();
    });

    $('#listeGorunumBtn').on('click', function() {
        $(this).addClass('active');
        $('#kartGorunumBtn').removeClass('active');
        $('#listeGorunum').show();
        $('#kartGorunum').hide();
    });

    // =============================================
    // Ana Render
    // =============================================
    function tabloYenile() {
        var filtre = filtreliUrunler();
        $('#sonucYazi').text(filtre.length + ' ürün gösteriliyor');
        kartRender(filtre);
        listeRender(filtre);
    }

    // =============================================
    // Filtre Dinleyicileri
    // =============================================
    $('#aramaInput').on('input', tabloYenile);
    $('#magazaFiltre, #nlpFiltre, #siralamaSelect').on('change', tabloYenile);
    $('input[name="kumasRadio"]').on('change', tabloYenile);
    $('.kesim-check').on('change', tabloYenile);
    $('#filtreTemizle').on('click', function() {
        $('#aramaInput').val('');
        $('#magazaFiltre, #nlpFiltre, #siralamaSelect').val('');
        $('input[name="kumasRadio"]').first().prop('checked', true);
        $('.kesim-check').prop('checked', false);
        tabloYenile();
    });

    // =============================================
    // Başlat
    // =============================================
    yukleVeriler();

});