$(function () {

    // =============================================
    // Yardımcı
    // =============================================
    var kesimBilgileri = {
        'Regular':  'Regular Fit: Standart kesim, vücuda yapışmaz, rahat kullanım için idealdir.',
        'Slim':     'Slim Fit: Vücudu saran kesim, sportif görünüm için tercih edilir.',
        'Oversize': 'Oversize: Bol kesim, günlük ve rahat kullanım için tasarlanmıştır.',
        'Athletic': 'Athletic Fit: Geniş omuz ve dar bel kesimi, atletik vücut yapısı için.'
    };

    var magazaAdi = {
        1: 'SportZone TR',
        2: 'FashionHub',
        3: 'ActiveWear',
        4: 'FitStyle',
        5: 'RunnerShop'
    };

    function durumBadge(silindiMi) {
        return silindiMi
            ? '<span class="badge durum-silindi"><i class="fas fa-times me-1"></i>Silinmiş</span>'
            : '<span class="badge durum-aktif"><i class="fas fa-check me-1"></i>Aktif</span>';
    }

    function kumasBadge(esnek) {
        if (esnek === null || esnek === undefined)
            return '<span class="text-muted">—</span>';
        return esnek
            ? '<span class="kumas-esnek"><i class="fas fa-expand-arrows-alt me-1"></i>Esnek</span>'
            : '<span class="kumas-sert"><i class="fas fa-minus me-1"></i>Standart</span>';
    }

    function kesimBadge(kesim) {
        if (!kesim) return '<span class="text-muted">—</span>';
        return '<span class="kesim-badge">' + kesim + ' Fit</span>';
    }

    function kisalt(metin, limit) {
        if (!metin) return '<span class="text-muted">—</span>';
        return metin.length > limit
            ? '<span title="' + metin + '">' + metin.substring(0, limit) + '...</span>'
            : metin;
    }

    // =============================================
    // Veri (Urun entity alanlarına göre)
    // =============================================
    var tumUrunler = [];
    var duzenlenecekId = null;

    function yukleUrunler() {
        // TODO: ProductController hazır olduğunda abp.ajax ile değiştirilecek
        // abp.ajax({ url: abp.appPath + 'api/app/urun' })
        //     .done(function(data) { tumUrunler = data.items; tabloYenile(); kpiGuncelle(); });

        tumUrunler = [
            { id: 1, magazaId: 1, ad: 'Slim Fit Spor Tayt',       aciklama: 'Yüksek bel, dört yönlü esnek kumaş.',   kesimTuru: 'Slim',     kumasEsnek: true,  silindiMi: false },
            { id: 2, magazaId: 1, ad: 'Regular Fit Koşu Şortu',   aciklama: 'Hafif ve nefes alabilir polyester.',    kesimTuru: 'Regular',  kumasEsnek: false, silindiMi: false },
            { id: 3, magazaId: 2, ad: 'Oversize Kapüşonlu Sweat', aciklama: 'Günlük kullanım için bol kesim.',       kesimTuru: 'Oversize', kumasEsnek: false, silindiMi: false },
            { id: 4, magazaId: 3, ad: 'Athletic Fit Tişört',      aciklama: 'Atletik vücut için özel tasarım.',      kesimTuru: 'Athletic', kumasEsnek: true,  silindiMi: false },
            { id: 5, magazaId: 4, ad: 'Slim Fit Yoga Pantolonu',  aciklama: null,                                    kesimTuru: 'Slim',     kumasEsnek: true,  silindiMi: false },
            { id: 6, magazaId: 2, ad: 'Eski Model Eşofman',       aciklama: 'Kaldırılan ürün.',                      kesimTuru: 'Regular',  kumasEsnek: null,  silindiMi: true  },
        ];

        tabloYenile();
        kpiGuncelle();
    }

    // =============================================
    // KPI
    // =============================================
    function kpiGuncelle() {
        var aktif  = tumUrunler.filter(function(u){ return !u.silindiMi; }).length;
        var esnek  = tumUrunler.filter(function(u){ return u.kumasEsnek === true; }).length;
        var kesimler = [...new Set(tumUrunler.filter(function(u){ return u.kesimTuru; }).map(function(u){ return u.kesimTuru; }))].length;

        $('#toplamUrun').text(tumUrunler.length);
        $('#aktifUrun').text(aktif);
        $('#eskekUrun').text(esnek);
        $('#kesimSayisi').text(kesimler);
    }

    // =============================================
    // Tablo Render
    // =============================================
    function tabloYenile() {
        var arama = $('#aramaInput').val().toLowerCase();
        var kesim = $('#kesimFiltre').val();
        var kumas = $('#kumasFiltre').val();
        var durum = $('#durumFiltre').val();

        var filtre = tumUrunler.filter(function (u) {
            var aramaUyumu = !arama || u.ad.toLowerCase().includes(arama);
            var kesimUyumu = !kesim || u.kesimTuru === kesim;
            var kumasUyumu = !kumas ||
                (kumas === 'esnek'      && u.kumasEsnek === true) ||
                (kumas === 'esnek_degil' && u.kumasEsnek === false);
            var durumUyumu = !durum ||
                (durum === 'aktif'   && !u.silindiMi) ||
                (durum === 'silindi' &&  u.silindiMi);
            return aramaUyumu && kesimUyumu && kumasUyumu && durumUyumu;
        });

        var $tbody = $('#urunTablosu');
        $tbody.empty();

        if (filtre.length === 0) {
            $tbody.html('<tr><td colspan="8" class="text-center py-4 text-muted">Sonuç bulunamadı.</td></tr>');
            $('#toplamKayitYazi').text('0 sonuç');
            return;
        }

        filtre.forEach(function (u) {
            $tbody.append(
                '<tr>' +
                    '<td class="text-muted small">' + u.id + '</td>' +
                    '<td>' +
                        '<div class="urun-bilgi">' +
                            '<div class="urun-ikon"><i class="fas fa-tshirt"></i></div>' +
                            '<span class="urun-ad">' + u.ad + '</span>' +
                        '</div>' +
                    '</td>' +
                    '<td class="text-muted small">' + (magazaAdi[u.magazaId] || '—') + '</td>' +
                    '<td>' + kesimBadge(u.kesimTuru) + '</td>' +
                    '<td>' + kumasBadge(u.kumasEsnek) + '</td>' +
                    '<td class="text-muted small">' + kisalt(u.aciklama, 40) + '</td>' +
                    '<td>' + durumBadge(u.silindiMi) + '</td>' +
                    '<td class="text-center">' +
                        '<div class="d-flex gap-1 justify-content-center">' +
                            '<button class="btn-islem btn-duzenle btn-duzenle-ac" data-id="' + u.id + '" title="Düzenle">' +
                                '<i class="fas fa-pencil-alt"></i>' +
                            '</button>' +
                            (!u.silindiMi
                                ? '<button class="btn-islem btn-sil btn-sil-ac" data-id="' + u.id + '" data-ad="' + u.ad + '" title="Sil"><i class="fas fa-trash"></i></button>'
                                : '') +
                        '</div>' +
                    '</td>' +
                '</tr>'
            );
        });

        $('#toplamKayitYazi').text(filtre.length + ' / ' + tumUrunler.length + ' ürün gösteriliyor');
    }

    // =============================================
    // Modal: Yeni Ürün
    // =============================================
    $('#yeniUrunBtn').on('click', function () {
        duzenlenecekId = null;
        formTemizle();
        $('#urunModalBaslik').html('<i class="fas fa-tshirt me-2 text-primary"></i>Yeni Ürün Ekle');
        new bootstrap.Modal(document.getElementById('urunModal')).show();
    });

    // =============================================
    // Modal: Düzenle
    // =============================================
    $(document).on('click', '.btn-duzenle-ac', function () {
        var id = $(this).data('id');
        var u  = tumUrunler.find(function(x){ return x.id === id; });
        if (!u) return;

        duzenlenecekId = id;
        formTemizle();
        $('#urunModalBaslik').html('<i class="fas fa-tshirt me-2 text-warning"></i>Ürünü Düzenle');

        $('#f_ad').val(u.ad);
        $('#f_magazaId').val(u.magazaId);
        $('#f_kesimTuru').val(u.kesimTuru || '');
        $('#f_kumasEsnek').prop('checked', u.kumasEsnek === true);
        $('#f_aciklama').val(u.aciklama || '');
        $('#aciklamaKarakter').text((u.aciklama || '').length + ' karakter');
        kesimBilgiGoster(u.kesimTuru);

        new bootstrap.Modal(document.getElementById('urunModal')).show();
    });

    // =============================================
    // Kesim seçince bilgi göster
    // =============================================
    $('#f_kesimTuru').on('change', function () {
        kesimBilgiGoster($(this).val());
    });

    function kesimBilgiGoster(kesim) {
        if (kesimBilgileri[kesim]) {
            $('#kesimBilgiText').text(kesimBilgileri[kesim]);
            $('#kesimBilgiKarti').show();
        } else {
            $('#kesimBilgiKarti').hide();
        }
    }

    // Açıklama karakter sayacı
    $('#f_aciklama').on('input', function () {
        $('#aciklamaKarakter').text($(this).val().length + ' karakter');
    });

    // =============================================
    // Kaydet
    // =============================================
    $('#urunKaydetBtn').on('click', function () {
        if (!formDogrula()) return;

        var veri = {
            magazaId:   parseInt($('#f_magazaId').val()),
            ad:         $('#f_ad').val().trim(),
            aciklama:   $('#f_aciklama').val().trim() || null,
            kesimTuru:  $('#f_kesimTuru').val() || null,
            kumasEsnek: $('#f_kumasEsnek').is(':checked'),
            silindiMi:  false
        };

        if (duzenlenecekId) {
            // TODO: abp.ajax PUT
            var idx = tumUrunler.findIndex(function(u){ return u.id === duzenlenecekId; });
            if (idx !== -1) Object.assign(tumUrunler[idx], veri);
            abp.notify.success('Ürün başarıyla güncellendi.', 'Başarılı');
        } else {
            // TODO: abp.ajax POST
            veri.id = tumUrunler.length + 1;
            tumUrunler.push(veri);
            abp.notify.success('Ürün başarıyla eklendi.', 'Başarılı');
        }

        bootstrap.Modal.getInstance(document.getElementById('urunModal')).hide();
        tabloYenile();
        kpiGuncelle();
    });

    function formDogrula() {
        var gecerli = true;
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').text('');

        if (!$('#f_ad').val().trim()) {
            $('#f_ad').addClass('is-invalid');
            $('#err_ad').text('Ürün adı zorunludur.');
            gecerli = false;
        }

        if (!$('#f_magazaId').val()) {
            $('#f_magazaId').addClass('is-invalid');
            $('#err_magazaId').text('Mağaza seçimi zorunludur.');
            gecerli = false;
        }

        return gecerli;
    }

    function formTemizle() {
        $('#f_ad, #f_aciklama').val('');
        $('#f_magazaId, #f_kesimTuru').val('');
        $('#f_kumasEsnek').prop('checked', false);
        $('#aciklamaKarakter').text('0 karakter');
        $('#kesimBilgiKarti').hide();
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').text('');
    }

    // =============================================
    // Sil
    // =============================================
    var silinecekId = null;

    $(document).on('click', '.btn-sil-ac', function () {
        silinecekId = $(this).data('id');
        $('#silUrunAdi').text($(this).data('ad'));
        new bootstrap.Modal(document.getElementById('silOnayModal')).show();
    });

    $('#silOnayBtn').on('click', function () {
        if (!silinecekId) return;
        // TODO: abp.ajax DELETE (soft delete — SilindiMi = true)
        var idx = tumUrunler.findIndex(function(u){ return u.id === silinecekId; });
        if (idx !== -1) tumUrunler[idx].silindiMi = true;
        abp.notify.warn('Ürün silindi.', 'Silindi');
        bootstrap.Modal.getInstance(document.getElementById('silOnayModal')).hide();
        tabloYenile();
        kpiGuncelle();
        silinecekId = null;
    });

    // =============================================
    // Filtreler
    // =============================================
    $('#aramaInput').on('input', tabloYenile);
    $('#kesimFiltre, #kumasFiltre, #durumFiltre').on('change', tabloYenile);
    $('#filtreTemizle').on('click', function () {
        $('#aramaInput').val('');
        $('#kesimFiltre, #kumasFiltre, #durumFiltre').val('');
        tabloYenile();
    });

    // =============================================
    // Başlat
    // =============================================
    yukleUrunler();

});