$(function () {

    // =============================================
    // Yardımcı
    // =============================================
    var magazaAdi = { 1: 'SportZone TR', 2: 'FashionHub', 3: 'ActiveWear', 4: 'FitStyle', 5: 'RunnerShop' };
    var urunAdi   = { 1: 'Slim Fit Spor Tayt', 2: 'Regular Fit Koşu Şortu', 3: 'Oversize Kapüşonlu Sweat', 4: 'Athletic Fit Tişört', 5: 'Slim Fit Yoga Pantolonu', 6: 'Regular Fit Antrenman Üstü' };

    function yildizlar(puan) {
        if (!puan) return '<span class="text-muted">—</span>';
        var html = '';
        for (var i = 1; i <= 5; i++)
            html += '<i class="fas fa-star puan-yildiz" style="opacity:' + (i <= puan ? '1' : '0.25') + '"></i>';
        return html;
    }

    function nlpBadge(islendi) {
        return islendi
            ? '<span class="nlp-badge nlp-islendi"><i class="fas fa-robot me-1"></i>Analiz Edildi</span>'
            : '<span class="nlp-badge nlp-bekliyor"><i class="fas fa-clock me-1"></i>Bekliyor</span>';
    }

    function duyguBadge(etiket) {
        if (!etiket) return '<span class="text-muted small">—</span>';
        var cls = etiket === 'Pozitif' ? 'duygu-pozitif' : etiket === 'Negatif' ? 'duygu-negatif' : 'duygu-notr';
        var icon = etiket === 'Pozitif' ? 'fa-smile' : etiket === 'Negatif' ? 'fa-frown' : 'fa-meh';
        return '<span class="nlp-badge ' + cls + '"><i class="fas ' + icon + ' me-1"></i>' + etiket + '</span>';
    }

    // =============================================
    // Veri — Yorum + NlpBulgusu entity
    // =============================================
    var tumYorumlar = [
        { id: 1,  urunId: 1, magazaId: 1, yorumMetni: 'Çok rahat, spor yaparken harika hissettiriyor.',       puan: 5, nlpIslendi: true  },
        { id: 2,  urunId: 1, magazaId: 1, yorumMetni: 'Beden biraz küçük geldi, bir beden büyük alın.',       puan: 3, nlpIslendi: true  },
        { id: 3,  urunId: 1, magazaId: 1, yorumMetni: 'Kumaş kalitesi mükemmel, tekrar alacağım.',            puan: 5, nlpIslendi: false },
        { id: 4,  urunId: 1, magazaId: 1, yorumMetni: 'Renk fotoğraftakinden farklı ama yine de güzel.',     puan: 4, nlpIslendi: false },
        { id: 5,  urunId: 2, magazaId: 1, yorumMetni: 'Hafif ve dayanıklı, koşu için ideal.',                puan: 4, nlpIslendi: true  },
        { id: 6,  urunId: 2, magazaId: 1, yorumMetni: 'Rengi biraz soluk ama kalite iyi.',                   puan: 3, nlpIslendi: false },
        { id: 7,  urunId: 3, magazaId: 2, yorumMetni: 'Tam aradığım oversize model, çok şık.',               puan: 5, nlpIslendi: true  },
        { id: 8,  urunId: 4, magazaId: 3, yorumMetni: 'Athletic kesim vücudu güzel gösteriyor.',             puan: 4, nlpIslendi: true  },
        { id: 9,  urunId: 4, magazaId: 3, yorumMetni: 'Dikişler sağlam, uzun ömürlü görünüyor.',             puan: 5, nlpIslendi: false },
        { id: 10, urunId: 5, magazaId: 4, yorumMetni: 'Yoga derslerinde çok rahat kullanıyorum.',            puan: 5, nlpIslendi: true  },
        { id: 11, urunId: 5, magazaId: 4, yorumMetni: 'Beklentilerimi karşılamadı maalesef.',                puan: 2, nlpIslendi: true  },
        { id: 12, urunId: 6, magazaId: 5, yorumMetni: 'Her spora uyuyor, çok fonksiyonel.',                  puan: 4, nlpIslendi: false },
    ];

    // NlpBulgusu entity: UrunId, MagazaId, Tema, TekrarSayisi, DuyguSkoru, DuyguEtiketi, OneriMetni, Durum
    var tumNlpBulgular = [
        { id: 1, urunId: 1, magazaId: 1, tema: 'Kumaş Kalitesi',   tekrarSayisi: 8,  duyguSkoru: 0.82, duyguEtiketi: 'Pozitif', oneriMetni: 'Kumaş kalitesi öne çıkıyor, pazarlama materyallerinde vurgulanabilir.',   durum: 'Acik'   },
        { id: 2, urunId: 1, magazaId: 1, tema: 'Beden Uyumu',      tekrarSayisi: 5,  duyguSkoru: -0.3, duyguEtiketi: 'Negatif', oneriMetni: 'Beden tablosu güncellenmeli, küçük geldiğine dair şikayetler var.',        durum: 'Acik'   },
        { id: 3, urunId: 1, magazaId: 1, tema: 'Renk Tutarlılığı', tekrarSayisi: 3,  duyguSkoru: 0.10, duyguEtiketi: 'Notr',    oneriMetni: 'Ürün fotoğrafları gerçek renge daha yakın çekilmeli.',                     durum: 'Acik'   },
        { id: 4, urunId: 2, magazaId: 1, tema: 'Rahatlık',         tekrarSayisi: 6,  duyguSkoru: 0.75, duyguEtiketi: 'Pozitif', oneriMetni: 'Rahatlık vurgusu satışları artırabilir.',                                   durum: 'Kapali' },
        { id: 5, urunId: 3, magazaId: 2, tema: 'Tasarım',          tekrarSayisi: 4,  duyguSkoru: 0.90, duyguEtiketi: 'Pozitif', oneriMetni: 'Tasarım çok beğeniliyor, benzer ürünler geliştirilebilir.',                 durum: 'Kapali' },
        { id: 6, urunId: 4, magazaId: 3, tema: 'Dayanıklılık',     tekrarSayisi: 7,  duyguSkoru: 0.65, duyguEtiketi: 'Pozitif', oneriMetni: 'Dayanıklılık ön plana çıkarılmalı.',                                        durum: 'Acik'   },
        { id: 7, urunId: 5, magazaId: 4, tema: 'Beklenti Uyumu',   tekrarSayisi: 2,  duyguSkoru: -0.5, duyguEtiketi: 'Negatif', oneriMetni: 'Ürün açıklaması daha detaylı yapılmalı.',                                  durum: 'Acik'   },
    ];

    // =============================================
    // KPI
    // =============================================
    function kpiGuncelle() {
        var islendi = tumYorumlar.filter(function(y){ return y.nlpIslendi; }).length;
        var puanlar = tumYorumlar.filter(function(y){ return y.puan; });
        var ort     = puanlar.length
            ? (puanlar.reduce(function(t,y){ return t + y.puan; }, 0) / puanlar.length).toFixed(1)
            : '—';

        $('#toplamYorum').text(tumYorumlar.length);
        $('#nlpIslendi').text(islendi);
        $('#nlpBekliyor').text(tumYorumlar.length - islendi);
        $('#ortPuan').html('★ ' + ort);
    }

    // =============================================
    // Yorum Tablosu
    // =============================================
    function yorumTabloYenile() {
        var arama   = $('#yorumArama').val().toLowerCase();
        var magaza  = $('#magazaFiltre').val();
        var puan    = $('#puanFiltre').val();
        var nlp     = $('#nlpFiltre').val();
        var sirala  = $('#yorumSirala').val();

        var filtre = tumYorumlar.filter(function(y) {
            var aramaUyumu  = !arama  || y.yorumMetni.toLowerCase().includes(arama);
            var magazaUyumu = !magaza || y.magazaId == magaza;
            var puanUyumu   = !puan   || y.puan == puan;
            var nlpUyumu    = !nlp    ||
                (nlp === 'islendi'  &&  y.nlpIslendi) ||
                (nlp === 'bekliyor' && !y.nlpIslendi);
            return aramaUyumu && magazaUyumu && puanUyumu && nlpUyumu;
        });

        if (sirala === 'puan_yuksek') filtre.sort(function(a,b){ return (b.puan||0)-(a.puan||0); });
        else if (sirala === 'puan_dusuk') filtre.sort(function(a,b){ return (a.puan||0)-(b.puan||0); });
        else filtre.sort(function(a,b){ return b.id - a.id; });

        var $tbody = $('#yorumTablosu');
        $tbody.empty();

        if (filtre.length === 0) {
            $tbody.html('<tr><td colspan="7" class="text-center py-4 text-muted">Sonuç bulunamadı.</td></tr>');
            $('#yorumKayitYazi').text('0 sonuç');
            return;
        }

        filtre.forEach(function(y) {
            $tbody.append(
                '<tr>' +
                    '<td class="text-muted small">' + y.id + '</td>' +
                    '<td class="text-muted small">' + (urunAdi[y.urunId] || '—') + '</td>' +
                    '<td class="text-muted small">' + (magazaAdi[y.magazaId] || '—') + '</td>' +
                    '<td><div class="yorum-metin-kisalt" title="' + y.yorumMetni + '">' + y.yorumMetni + '</div></td>' +
                    '<td>' + yildizlar(y.puan) + '</td>' +
                    '<td>' + nlpBadge(y.nlpIslendi) + '</td>' +
                    '<td class="text-center">' +
                        '<button class="btn-islem btn-detay btn-yorum-detay" data-id="' + y.id + '" title="Detay">' +
                            '<i class="fas fa-eye"></i>' +
                        '</button>' +
                    '</td>' +
                '</tr>'
            );
        });

        $('#yorumKayitYazi').text(filtre.length + ' / ' + tumYorumlar.length + ' yorum gösteriliyor');
    }

    // =============================================
    // Yorum Detay Modal
    // =============================================
    $(document).on('click', '.btn-yorum-detay', function() {
        var id = $(this).data('id');
        var y  = tumYorumlar.find(function(x){ return x.id === id; });
        if (!y) return;

        var nlpBulgular = tumNlpBulgular.filter(function(b){ return b.urunId === y.urunId; });

        var nlpHtml = '';
        if (nlpBulgular.length > 0) {
            nlpHtml = '<hr><p class="fw-600 mb-2"><i class="fas fa-brain me-1 text-info"></i>Bu Ürünün NLP Bulguları</p>';
            nlpBulgular.forEach(function(b) {
                nlpHtml +=
                    '<div class="d-flex align-items-center justify-content-between mb-1 p-2 rounded" style="background:rgba(0,0,0,0.02)">' +
                        '<span class="small fw-600">' + b.tema + '</span>' +
                        '<div class="d-flex gap-1">' +
                            duyguBadge(b.duyguEtiketi) +
                            '<span class="nlp-badge ' + (b.durum === 'Acik' ? 'durum-acik' : 'durum-kapali') + '">' + b.durum + '</span>' +
                        '</div>' +
                    '</div>';
            });
        }

        $('#yorumDetayIcerik').html(
            '<div class="mb-3">' +
                '<div class="d-flex align-items-center gap-2 mb-2">' +
                    '<span class="fw-600">' + (urunAdi[y.urunId] || '—') + '</span>' +
                    '<span class="text-muted small">· ' + (magazaAdi[y.magazaId] || '—') + '</span>' +
                '</div>' +
                '<div class="p-3 rounded mb-3" style="background:rgba(79,70,229,0.04);border:1px solid rgba(79,70,229,0.1);font-size:0.95rem;line-height:1.7">' +
                    '"' + y.yorumMetni + '"' +
                '</div>' +
                '<div class="d-flex gap-3">' +
                    '<div><span class="text-muted small">Puan: </span>' + yildizlar(y.puan) + '</div>' +
                    '<div>' + nlpBadge(y.nlpIslendi) + '</div>' +
                '</div>' +
            '</div>' +
            nlpHtml
        );

        new bootstrap.Modal(document.getElementById('yorumDetayModal')).show();
    });

    // =============================================
    // NLP Bulgular Grid
    // =============================================
    function nlpGridYenile() {
        var magaza = $('#nlpMagazaFiltre').val();
        var duygu  = $('#nlpDuyguFiltre').val();
        var durum  = $('#nlpDurumFiltre').val();

        var filtre = tumNlpBulgular.filter(function(b) {
            var mUyumu = !magaza || b.magazaId == magaza;
            var dUyumu = !duygu  || b.duyguEtiketi === duygu;
            var stUyumu = !durum || b.durum === durum;
            return mUyumu && dUyumu && stUyumu;
        });

        var $grid = $('#nlpBulgularGrid');
        $grid.empty();

        if (filtre.length === 0) {
            $grid.html('<div class="col-12 text-center py-5 text-muted"><i class="fas fa-search fa-2x mb-2 d-block"></i>Sonuç bulunamadı.</div>');
            return;
        }

        filtre.forEach(function(b) {
            var skorRenk = b.duyguSkoru > 0 ? '#059669' : b.duyguSkoru < 0 ? '#dc2626' : '#6b7280';
            var durumCls = b.durum === 'Acik' ? 'durum-acik' : 'durum-kapali';

            $grid.append(
                '<div class="col-xl-4 col-md-6">' +
                    '<div class="nlp-bulgu-kart">' +
                        '<div class="nlp-bulgu-header">' +
                            '<div>' +
                                '<div class="nlp-tema">' + b.tema + '</div>' +
                                '<div class="text-muted small mt-1">' + (urunAdi[b.urunId]||'—') + ' · ' + (magazaAdi[b.magazaId]||'—') + '</div>' +
                            '</div>' +
                            '<span class="nlp-badge ' + durumCls + '">' + b.durum + '</span>' +
                        '</div>' +
                        '<div class="nlp-bulgu-body">' +
                            '<div class="nlp-satir">' +
                                '<span class="nlp-satir-label">Duygu</span>' +
                                duyguBadge(b.duyguEtiketi) +
                            '</div>' +
                            '<div class="nlp-satir">' +
                                '<span class="nlp-satir-label">Duygu Skoru</span>' +
                                '<span class="nlp-satir-deger fw-700" style="color:' + skorRenk + '">' + (b.duyguSkoru !== null ? b.duyguSkoru.toFixed(2) : '—') + '</span>' +
                            '</div>' +
                            '<div class="nlp-satir">' +
                                '<span class="nlp-satir-label">Tekrar Sayısı</span>' +
                                '<span class="nlp-satir-deger">' + b.tekrarSayisi + ' yorum</span>' +
                            '</div>' +
                            (b.oneriMetni
                                ? '<div class="nlp-oneri"><i class="fas fa-lightbulb me-1 text-warning"></i>' + b.oneriMetni + '</div>'
                                : '') +
                        '</div>' +
                    '</div>' +
                '</div>'
            );
        });
    }

    // =============================================
    // Sekme Geçişi
    // =============================================
    $('#yorumTabs .nav-link').on('click', function() {
        $('#yorumTabs .nav-link').removeClass('active');
        $(this).addClass('active');
        var tab = $(this).data('tab');
        $('#tab-yorumlar, #tab-nlp').hide();
        $('#tab-' + tab).show();
    });

    // =============================================
    // Filtre Dinleyicileri
    // =============================================
    $('#yorumArama').on('input', yorumTabloYenile);
    $('#magazaFiltre, #puanFiltre, #nlpFiltre, #yorumSirala').on('change', yorumTabloYenile);
    $('#filtreTemizle').on('click', function() {
        $('#yorumArama').val('');
        $('#magazaFiltre, #puanFiltre, #nlpFiltre').val('');
        $('#yorumSirala').val('yeni');
        yorumTabloYenile();
    });

    $('#nlpMagazaFiltre, #nlpDuyguFiltre, #nlpDurumFiltre').on('change', nlpGridYenile);
    $('#nlpFiltreTemizle').on('click', function() {
        $('#nlpMagazaFiltre, #nlpDuyguFiltre, #nlpDurumFiltre').val('');
        nlpGridYenile();
    });

    // =============================================
    // Başlat
    // =============================================
    kpiGuncelle();
    yorumTabloYenile();
    nlpGridYenile();

});
