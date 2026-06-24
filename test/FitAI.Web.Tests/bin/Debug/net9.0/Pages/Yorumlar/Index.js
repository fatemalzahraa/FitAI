'use strict';

$(function () {

    // =========================================================================
    // 1. SERVİS TANIMI — proxy varsa kullan, yoksa abp.ajax fallback
    // =========================================================================
    var _bildirimService = (window.fitAI && fitAI.bildirimler && fitAI.bildirimler.bildirim)
        ? fitAI.bildirimler.bildirim
        : null;

    if (!_bildirimService) {
        _bildirimService = {
            getList: function () {
                return abp.ajax({ type: 'GET', url: '/api/app/bildirim' });
            },
            okunduIsaretle: function (id) {
                return abp.ajax({ type: 'POST', url: '/api/app/bildirim/' + id + '/okundu-isaretle' });
            },
            tumunuOkunduIsaretle: function (kullaniciId) {
                return abp.ajax({ type: 'POST', url: '/api/app/bildirim/tumunu-okundu-isaretle', data: JSON.stringify(kullaniciId), contentType: 'application/json' });
            },
            delete: function (id) {
                return abp.ajax({ type: 'DELETE', url: '/api/app/bildirim/' + id });
            }
        };
    }

    // =========================================================================
    // 2. DURUM
    // =========================================================================
    var _tumBildirimler    = [];
    var _aktifOkunduFiltre = 'tumu';
    var _aktifKanal        = '';
    var _aktifTip          = '';
    var _silinecekId       = null;

    var _tipConfig = {
        'AI'    : { ikon: 'fa-robot',                css: 'primary' },
        'Sistem': { ikon: 'fa-cog',                  css: 'secondary' },
        'Uyari' : { ikon: 'fa-exclamation-triangle', css: 'warning' }
    };

    var _kanalConfig = {
        'InApp': { etiket: 'Uygulama İçi', css: 'info' },
        'Email': { etiket: 'E-posta',      css: 'secondary' },
        'Push' : { etiket: 'Push',         css: 'success' }
    };

    // =========================================================================
    // 3. VERİ ÇEKME
    // =========================================================================
    function _listeyiYukle() {
        _yukleniyor(true);

        _bildirimService.getList()
            .then(function (result) {
                _tumBildirimler = result.items || result;
                _render();
            })
            .catch(function (err) {
                abp.notify.error('Bildirimler yüklenirken bir hata oluştu.');
                console.error('Bildirim getList hatası:', err);
            })
            .always(function () {
                _yukleniyor(false);
            });
    }

    // =========================================================================
    // 4. FİLTRELEME
    // =========================================================================
    function _filtreUygula() {
        var liste = _tumBildirimler;

        if (_aktifOkunduFiltre === 'okunmamis') {
            liste = liste.filter(function (b) { return !b.okunduMu; });
        } else if (_aktifOkunduFiltre === 'okunmus') {
            liste = liste.filter(function (b) { return b.okunduMu; });
        }

        if (_aktifKanal) {
            liste = liste.filter(function (b) { return b.kanal === _aktifKanal; });
        }

        if (_aktifTip) {
            liste = liste.filter(function (b) { return b.tip === _aktifTip; });
        }

        return liste;
    }

    // =========================================================================
    // 5. RENDER
    // =========================================================================
    function _render() {
        var filtrelenmis = _filtreUygula();
        var $liste = $('#bildirimListesi').empty();
        $('#bosListeMesaji').addClass('d-none');

        if (!filtrelenmis.length) {
            $('#bosListeMesaji').removeClass('d-none');
            _okunmamisSayisiniGuncelle();
            return;
        }

        filtrelenmis.forEach(function (b) {
            $liste.append(_bildirimSatiri(b));
        });

        _okunmamisSayisiniGuncelle();
    }

    function _bildirimSatiri(b) {
        var tipCfg   = _tipConfig[b.tip]     || { ikon: 'fa-bell',       css: 'dark' };
        var kanalCfg = _kanalConfig[b.kanal] || { etiket: b.kanal || '', css: 'light' };
        var tarih    = b.gonderimTarihi
            ? new Date(b.gonderimTarihi).toLocaleString('tr-TR')
            : '';

        var ilgiliHtml = '';
        if (b.ilgiliKayitId && b.ilgiliKayitTipi) {
            ilgiliHtml = '<small class="text-muted ms-1">'
                + '<i class="fa fa-link fa-xs me-1"></i>'
                + b.ilgiliKayitTipi + ' #' + b.ilgiliKayitId
                + '</small>';
        }

        return $([
            '<div class="list-group-item list-group-item-action bildirim-item '
                + (b.okunduMu ? 'okunmus' : 'okunmamis') + '" data-id="' + b.id + '">',
            '  <div class="d-flex w-100 justify-content-between align-items-start">',
            '    <div class="d-flex align-items-start gap-3">',
            '      <div class="bildirim-ikon text-' + tipCfg.css + '">',
            '        <i class="fa ' + tipCfg.ikon + ' fa-lg"></i>',
            '      </div>',
            '      <div>',
            '        <div class="d-flex align-items-center gap-2 mb-1">',
            '          <span class="fw-semibold">' + (b.baslik || '') + '</span>',
            '          <span class="badge bg-' + tipCfg.css
                + ' bg-opacity-10 text-' + tipCfg.css
                + ' border border-' + tipCfg.css + '">' + (b.tip || '') + '</span>',
            '          <span class="badge bg-' + kanalCfg.css + '">' + kanalCfg.etiket + '</span>',
            '          ' + ilgiliHtml,
            '        </div>',
            '        <p class="mb-0 text-muted small">' + (b.icerik || '') + '</p>',
            '      </div>',
            '    </div>',
            '    <div class="d-flex flex-column align-items-end gap-1 ms-3 flex-shrink-0">',
            '      <small class="text-muted text-nowrap">' + tarih + '</small>',
            '      <div class="d-flex gap-1">',
            '        <button class="btn btn-link btn-sm p-0 text-secondary btnOku"'
                + ' data-id="' + b.id + '" title="Okundu İşaretle"'
                + (b.okunduMu ? ' disabled' : '') + '>',
            '          <i class="fa fa-check"></i>',
            '        </button>',
            '        <button class="btn btn-link btn-sm p-0 text-danger btnSil"'
                + ' data-id="' + b.id + '" title="Sil">',
            '          <i class="fa fa-trash"></i>',
            '        </button>',
            '      </div>',
            '    </div>',
            '  </div>',
            '</div>'
        ].join(''));
    }

    function _okunmamisSayisiniGuncelle() {
        var sayi = _tumBildirimler.filter(function (b) { return !b.okunduMu; }).length;
        var $r = $('#okunmamisSayi');
        sayi > 0 ? $r.text(sayi).removeClass('d-none') : $r.addClass('d-none');
    }

    function _yukleniyor(goster) {
        $('#yukleniyor').toggleClass('d-none', !goster);
        $('#bildirimListesi').toggleClass('d-none', goster);
    }

    // =========================================================================
    // 6. OLAYLAR
    // =========================================================================

    // Okundu/Okunmadı sekmeleri
    $('#bildirimTab a').on('click', function (e) {
        e.preventDefault();
        $('#bildirimTab a').removeClass('active');
        $(this).addClass('active');
        _aktifOkunduFiltre = $(this).data('filter');
        _render();
    });

    // Kanal filtresi
    $('#kanalFiltre').on('change', function () {
        _aktifKanal = $(this).val();
        _render();
    });

    // Tip filtresi
    $('#tipFiltre').on('change', function () {
        _aktifTip = $(this).val();
        _render();
    });

    // Tekil okundu işaretle
    $(document).on('click', '.btnOku', function (e) {
        e.stopPropagation();
        var id = parseInt($(this).data('id'));
        _bildirimService.okunduIsaretle(id)
            .then(function () {
                var b = _tumBildirimler.find(function (x) { return x.id === id; });
                if (b) { b.okunduMu = true; b.okunmaTarihi = new Date().toISOString(); }
                _render();
                abp.notify.success('Bildirim okundu olarak işaretlendi.');
            })
            .catch(function () {
                abp.notify.error('İşlem sırasında bir hata oluştu.');
            });
    });

    // Tümünü okundu işaretle
    $('#btnTumunuOku').on('click', function () {
        var okunmamislar = _tumBildirimler.filter(function (b) { return !b.okunduMu; });
        if (!okunmamislar.length) {
            abp.notify.info('Okunmamış bildirim bulunmuyor.');
            return;
        }
        var kullaniciId = okunmamislar[0].kullaniciId;
        _bildirimService.tumunuOkunduIsaretle(kullaniciId)
            .then(function () {
                _tumBildirimler.forEach(function (b) {
                    b.okunduMu = true;
                    b.okunmaTarihi = new Date().toISOString();
                });
                _render();
                abp.notify.success('Tüm bildirimler okundu olarak işaretlendi.');
            })
            .catch(function () {
                abp.notify.error('İşlem sırasında bir hata oluştu.');
            });
    });

    // Sil butonu → modal aç
    $(document).on('click', '.btnSil', function (e) {
        e.stopPropagation();
        _silinecekId = parseInt($(this).data('id'));
        $('#silmeModal').modal('show');
    });

    // Silme onayı
    $('#btnSilmeOnayla').on('click', function () {
        if (_silinecekId === null) return;
        _bildirimService.delete(_silinecekId)
            .then(function () {
                _tumBildirimler = _tumBildirimler.filter(function (b) { return b.id !== _silinecekId; });
                _silinecekId = null;
                $('#silmeModal').modal('hide');
                _render();
                abp.notify.success('Bildirim silindi.');
            })
            .catch(function () {
                abp.notify.error('Silme işlemi başarısız.');
            });
    });

    // =========================================================================
    // 7. BAŞLANGIÇ
    // =========================================================================
    _listeyiYukle();
});