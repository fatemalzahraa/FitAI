$(function () {

    // =========================================================================
    // 1. SERVİS TANIMLARI
    // ABP proxy auto-generated isimleri: fitAI.yorumlar.yorum  /  fitAI.ai.aiApp
    // =========================================================================
    var _yorumService = (window.fitAI && fitAI.yorumlar && fitAI.yorumlar.yorum)
        ? fitAI.yorumlar.yorum
        : null;

    // Fallback: proxy üretilmemişse doğrudan AJAX
    if (!_yorumService) {
        _yorumService = {
            getList: function (params) {
                return abp.ajax({ type: 'GET', url: '/api/app/yorum', data: params });
            },
            delete: function (id) {
                return abp.ajax({ type: 'DELETE', url: '/api/app/yorum/' + id });
            },
            // TriggerNlp: YorumAppService.TriggerNlpAsync → POST /api/app/yorum/{id}/trigger-nlp
            triggerNlp: function (id) {
                return abp.ajax({ type: 'POST', url: '/api/app/yorum/' + id + '/trigger-nlp' });
            }
        };
    } else if (!_yorumService.triggerNlp) {
        // Proxy var ama triggerNlp metodu yoksa ekle
        _yorumService.triggerNlp = function (id) {
            return abp.ajax({ type: 'POST', url: '/api/app/yorum/' + id + '/trigger-nlp' });
        };
    }

    // =========================================================================
    // 2. YARDIMCI FONKSİYONLAR
    // =========================================================================

    /** 1–5 arası puan için dolu/boş yıldız HTML'i */
    function yildizRender(puan) {
        if (!puan) return '<span class="text-muted small">—</span>';
        var html = '<div class="text-warning" title="' + puan + ' / 5">';
        for (var i = 1; i <= 5; i++) {
            html += (i <= puan)
                ? '<i class="fas fa-star"></i>'
                : '<i class="far fa-star"></i>';
        }
        html += '</div>';
        return html;
    }

    /** NLP durum badge'i */
    function nlpBadge(nlpIslendi, duygu, guvenSkoru) {
        if (!nlpIslendi) {
            return '<span class="badge bg-light text-muted border">'
                 + '<i class="fas fa-clock me-1"></i>Bekliyor</span>';
        }

        var cls  = 'bg-secondary';
        var ikon = 'fa-meh';
        var etiket = duygu || 'Nötr';

        if (duygu) {
            var d = duygu.toLowerCase();
            if (d.includes('olumlu') || d.includes('pozitif') || d.includes('positive')) {
                cls  = 'bg-success text-white';
                ikon = 'fa-smile';
            } else if (d.includes('olumsuz') || d.includes('negatif') || d.includes('negative')) {
                cls  = 'bg-danger text-white';
                ikon = 'fa-frown';
            } else {
                cls  = 'bg-secondary text-white';
                ikon = 'fa-meh';
            }
        }

        var skor = guvenSkoru ? ' (%' + Math.round(guvenSkoru * 100) + ')' : '';
        return '<span class="badge ' + cls + ' d-inline-flex align-items-center gap-1">'
             + '<i class="fas ' + ikon + '"></i>' + etiket + skor + '</span>';
    }

    // =========================================================================
    // 3. DURUM
    // =========================================================================
    var tumYorumlar = [];

    // =========================================================================
    // 4. VERİ ÇEKME
    // =========================================================================
    function yukleYorumlar() {
        $('#yorumTablosu').html(
            '<tr><td colspan="7" class="text-center py-4">'
          + '<div class="spinner-border spinner-border-sm text-primary me-2"></div>'
          + '<span class="text-muted">Yorumlar yükleniyor...</span></td></tr>'
        );

        _yorumService.getList({ maxResultCount: 1000, skipCount: 0 })
            .then(function (result) {
                tumYorumlar = result.items || [];
                kpiGuncelle();
                tabloYenile();
                nlpSekmesiYenile();
            })
            .catch(function (err) {
                console.warn('Canlı veri çekilemedi, mock veriler kullanılıyor.', err);
                tumYorumlar = [
                    { id: 1, urunId: 1, urunAdi: 'Premium Spor Ayakkabı', kullaniciAdi: 'ahmet_k', yorumMetni: 'Kumaşı çok esnek ve rahat, tam numaramı aldım çok memnunum.', puan: 5, nlpIslendi: true,  duygu: 'Olumlu',  guvenSkoru: 0.96 },
                    { id: 2, urunId: 2, urunAdi: 'Yoga Matı',             kullaniciAdi: 'selin_y', yorumMetni: 'Ürün güzel ama rengi fotoğraftakinden biraz daha koyu geldi.',  puan: 3, nlpIslendi: true,  duygu: 'Nötr',    guvenSkoru: 0.72 },
                    { id: 3, urunId: 1, urunAdi: 'Premium Spor Ayakkabı', kullaniciAdi: 'mert_d',  yorumMetni: 'Kesimi çok dar, ayağımı sıktı iade etmek zorunda kaldım.',    puan: 2, nlpIslendi: false, duygu: null,      guvenSkoru: 0    },
                    { id: 4, urunId: 3, urunAdi: 'Koşu Bandı Pro',        kullaniciAdi: 'elif_s',  yorumMetni: 'Harika bir ürün, her sabah kullanıyorum.',                      puan: 5, nlpIslendi: false, duygu: null,      guvenSkoru: 0    }
                ];
                kpiGuncelle();
                tabloYenile();
                nlpSekmesiYenile();
            });
    }

    // =========================================================================
    // 5. KPI KARTLARI
    // =========================================================================
    function kpiGuncelle() {
        var toplam   = tumYorumlar.length;
        var islendi  = tumYorumlar.filter(function (y) { return y.nlpIslendi; }).length;
        var bekliyor = toplam - islendi;
        var puanlilar = tumYorumlar.filter(function (y) { return y.puan; });
        var ortPuan  = puanlilar.length
            ? (puanlilar.reduce(function (s, y) { return s + y.puan; }, 0) / puanlilar.length).toFixed(1)
            : '—';

        $('#toplamYorum').text(toplam);
        $('#nlpIslendi').text(islendi);
        $('#nlpBekliyor').text(bekliyor);
        $('#ortPuan').text(ortPuan + (ortPuan !== '—' ? ' ★' : ''));
    }

    // =========================================================================
    // 6. TABLO RENDER + FİLTRELEME
    // =========================================================================
    function tabloYenile() {
        // ── Filtre değerleri (HTML'deki gerçek ID'ler) ──
        var arama      = ($('#yorumArama').val()   || '').toLowerCase();
        var puanFiltre = $('#puanFiltre').val()    || '';
        var nlpFiltre  = $('#nlpFiltre').val()     || '';
        var siralama   = $('#yorumSirala').val()   || 'yeni';

        // ── Filtrele ──
        var filtre = tumYorumlar.filter(function (y) {
            var aramaUyumu = !arama
                || (y.yorumMetni && y.yorumMetni.toLowerCase().includes(arama))
                || (y.urunAdi    && y.urunAdi.toLowerCase().includes(arama))
                || (y.kullaniciAdi && y.kullaniciAdi.toLowerCase().includes(arama));

            var puanUyumu = !puanFiltre || String(y.puan) === puanFiltre;

            var nlpUyumu = !nlpFiltre
                || (nlpFiltre === 'islendi'   && y.nlpIslendi === true)
                || (nlpFiltre === 'bekliyor'  && y.nlpIslendi === false);

            return aramaUyumu && puanUyumu && nlpUyumu;
        });

        // ── Sırala ──
        if (siralama === 'puan_yuksek') {
            filtre.sort(function (a, b) { return (b.puan || 0) - (a.puan || 0); });
        } else if (siralama === 'puan_dusuk') {
            filtre.sort(function (a, b) { return (a.puan || 0) - (b.puan || 0); });
        }
        // 'yeni' → API sırasına bırak (genellikle creationTime DESC)

        var $tbody = $('#yorumTablosu');
        $tbody.empty();

        if (filtre.length === 0) {
            $tbody.html('<tr><td colspan="7" class="text-center py-4 text-muted">Arama kriterlerine uygun yorum bulunamadı.</td></tr>');
            $('#yorumKayitYazi').text('0 yorum');
            return;
        }

        filtre.forEach(function (y) {
            var nlpButon = !y.nlpIslendi
                ? '<button class="btn btn-sm btn-outline-primary btn-nlp-tetikle me-1" data-id="' + y.id + '" title="AI Analizi Başlat">'
                  + '<i class="fas fa-robot me-1"></i>NLP İşle</button>'
                : '';

            $tbody.append(
                '<tr>' +
                    '<td class="text-muted small">#' + y.id + '</td>' +
                    '<td><span class="fw-semibold">' + (y.urunAdi || 'Ürün #' + y.urunId) + '</span></td>' +
                    '<td class="text-muted small">' + (y.magazaAdi || '—') + '</td>' +
                    '<td class="small text-secondary" style="max-width:280px;white-space:normal;">' + escapeHtml(y.yorumMetni) + '</td>' +
                    '<td>' + yildizRender(y.puan) + '</td>' +
                    '<td>' + nlpBadge(y.nlpIslendi, y.duygu, y.guvenSkoru) + '</td>' +
                    '<td class="text-center">' +
                        '<div class="d-flex gap-1 justify-content-center">' +
                            nlpButon +
                            '<button class="btn btn-sm btn-outline-danger btn-yorum-sil" data-id="' + y.id + '" title="Yorumu Sil">' +
                                '<i class="fas fa-trash-alt"></i>' +
                            '</button>' +
                        '</div>' +
                    '</td>' +
                '</tr>'
            );
        });

        $('#yorumKayitYazi').text(filtre.length + ' / ' + tumYorumlar.length + ' yorum');
    }

    /** XSS önlemi */
    function escapeHtml(str) {
        if (!str) return '';
        return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }

    // =========================================================================
    // 7. NLP SEKMESI (tab-nlp)
    // =========================================================================
    function nlpSekmesiYenile() {
        var duyguFiltre = $('#nlpDuyguFiltre').val() || '';
        var islenenler  = tumYorumlar.filter(function (y) { return y.nlpIslendi; });

        if (duyguFiltre) {
            islenenler = islenenler.filter(function (y) {
                return y.duygu && y.duygu.toLowerCase().includes(duyguFiltre.toLowerCase());
            });
        }

        var $grid = $('#nlpBulgularGrid');
        $grid.empty();

        if (islenenler.length === 0) {
            $grid.html('<div class="col-12 text-center py-5 text-muted"><i class="fas fa-brain fa-2x mb-3 d-block opacity-25"></i>Henüz analiz edilmiş yorum yok.</div>');
            return;
        }

        islenenler.forEach(function (y) {
            var dLower   = (y.duygu || '').toLowerCase();
            var cardCls  = dLower.includes('olumlu') || dLower.includes('pozitif') ? 'border-success'
                         : dLower.includes('olumsuz') || dLower.includes('negatif') ? 'border-danger'
                         : 'border-secondary';
            var skor     = y.guvenSkoru ? Math.round(y.guvenSkoru * 100) : 0;

            $grid.append(
                '<div class="col-md-4">' +
                    '<div class="card h-100 border-2 ' + cardCls + '">' +
                        '<div class="card-body">' +
                            '<div class="d-flex justify-content-between align-items-start mb-2">' +
                                '<span class="fw-semibold small">' + (y.urunAdi || 'Ürün #' + y.urunId) + '</span>' +
                                nlpBadge(true, y.duygu, y.guvenSkoru) +
                            '</div>' +
                            '<p class="small text-muted mb-2">' + escapeHtml(y.yorumMetni) + '</p>' +
                            '<div class="d-flex justify-content-between align-items-center">' +
                                yildizRender(y.puan) +
                                '<span class="small text-muted">Güven: %' + skor + '</span>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>'
            );
        });
    }

    // =========================================================================
    // 8. NLP TETİKLEME — Backend API entegrasyonu
    // =========================================================================
    $(document).on('click', '.btn-nlp-tetikle', function () {
        var $btn = $(this);
        var id   = parseInt($btn.data('id'));
        var yorum = tumYorumlar.find(function (x) { return x.id === id; });
        if (!yorum) return;

        $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i>Analiz...');

        // ── Canlı backend çağrısı: POST /api/app/yorum/{id}/trigger-nlp ──
        _yorumService.triggerNlp(id)
            .then(function (guncelDto) {
                // Backend'den gelen güncel DTO ile yerel diziyi güncelle
                var idx = tumYorumlar.findIndex(function (x) { return x.id === id; });
                if (idx !== -1) {
                    tumYorumlar[idx] = $.extend(tumYorumlar[idx], {
                        nlpIslendi : guncelDto.nlpIslendi,
                        duygu      : guncelDto.duygu,
                        guvenSkoru : guncelDto.guvenSkoru
                    });
                }
                abp.notify.success('Analiz tamamlandı: ' + guncelDto.duygu, 'AI Analizi');
                kpiGuncelle();
                tabloYenile();
                nlpSekmesiYenile();
            })
            .catch(function (err) {
                abp.notify.error('AI servisi yanıt vermedi, lütfen tekrar deneyin.', 'Hata');
                console.error('NLP tetikleme hatası:', err);
                $btn.prop('disabled', false).html('<i class="fas fa-robot me-1"></i>NLP İşle');
            });
    });

    // =========================================================================
    // 9. SİLME
    // =========================================================================
    $(document).on('click', '.btn-yorum-sil', function () {
        var id = parseInt($(this).data('id'));

        abp.message.confirm(
            'Bu yorumu kalıcı olarak silmek istediğinize emin misiniz?',
            'Yorumu Sil',
            function (isConfirmed) {
                if (!isConfirmed) return;

                _yorumService.delete(id)
                    .then(function () {
                        abp.notify.warn('Yorum silindi.', 'Başarılı');
                        tumYorumlar = tumYorumlar.filter(function (x) { return x.id !== id; });
                        kpiGuncelle();
                        tabloYenile();
                        nlpSekmesiYenile();
                    })
                    .catch(function () {
                        abp.notify.error('Yorum silinemedi.');
                    });
            }
        );
    });

    // =========================================================================
    // 10. SEKMELEŞTİRME
    // =========================================================================
    $(document).on('click', '.analiz-nav-btn', function () {
        var tab = $(this).data('tab');
        $('.analiz-nav-btn').removeClass('active');
        $(this).addClass('active');
        $('.analiz-tab-icerik').hide();
        $('#tab-' + tab).show();
    });

    // =========================================================================
    // 11. FİLTRE DİNLEYİCİLERİ
    // =========================================================================
    $('#yorumArama').on('input', tabloYenile);
    $('#puanFiltre, #nlpFiltre, #yorumSirala').on('change', tabloYenile);

    $('#filtreTemizle').on('click', function () {
        $('#yorumArama').val('');
        $('#puanFiltre, #nlpFiltre, #yorumSirala').val('');
        tabloYenile();
    });

    $('#nlpDuyguFiltre, #nlpMagazaFiltre, #nlpDurumFiltre').on('change', nlpSekmesiYenile);

    $('#nlpFiltreTemizle').on('click', function () {
        $('#nlpDuyguFiltre, #nlpMagazaFiltre, #nlpDurumFiltre').val('');
        nlpSekmesiYenile();
    });

    // =========================================================================
    // 12. BAŞLANGIÇ
    // =========================================================================
    yukleYorumlar();
});