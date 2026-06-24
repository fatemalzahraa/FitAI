$(function () {

    var _magazaService = (window.fitAI && fitAI.magazalar && fitAI.magazalar.magaza) 
                          ? fitAI.magazalar.magaza 
                          : null;

    if (!_magazaService) {
        console.warn("ABP Servis Proxy yüklenemedi. Standart JQuery AJAX moduna geçiliyor.");
    }

    var renkler = ['#4f46e5', '#0ea5e9', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];
    function rastgeleRenk(id) { return renkler[id % renkler.length]; }
    function ilkHarf(ad) { return ad ? ad.charAt(0).toUpperCase() : '?'; }

    var paketBilgileri = {
        'Baslangic': 'Başlangıç paketi: Temel widget erişimi, aylık 500 sorgu limiti.',
        'Standart':  'Standart paket: Gelişmiş widget, aylık 5.000 sorgu, NLP analizi.',
        'Premium':   'Premium paket: Sınırsız sorgu, AI attribution, öncelikli destek.'
    };

    function paketBadge(paket) {
        if (!paket) return '<span class="text-muted">—</span>';
        var cls = paket === 'Premium' ? 'paket-premium' : paket === 'Standart' ? 'paket-standart' : 'paket-baslangic';
        return '<span class="paket-badge ' + cls + '">' + paket + '</span>';
    }

    function durumBadge(aktif) {
        return aktif 
            ? '<span class="badge bg-success-subtle text-success border border-success-subtle rounded-pill px-2"><i class="fas fa-check-circle me-1"></i>Aktif</span>'
            : '<span class="badge bg-danger-subtle text-danger border border-danger-subtle rounded-pill px-2"><i class="fas fa-times-circle me-1"></i>Pasif</span>';
    }

    var tumMagazalar = [];

    function magazalariYukle() {
        if (_magazaService) {
            _magazaService.getList({ maxResultCount: 1000 })
                .then(function (result) {
                    tumMagazalar = result.items || [];
                    tabloYenile();
                    kpiGuncelle();
                })
                .catch(function (err) {
                    abp.notify.error('Veriler yüklenirken hata oluştu.');
                    console.error(err);
                });
        } else {
            $.ajax({
                url: '/api/app/magaza?maxResultCount=1000',
                type: 'GET',
                success: function (result) {
                    tumMagazalar = result.items || [];
                    tabloYenile();
                    kpiGuncelle();
                }
            });
        }
    }

    function tabloYenile() {
        var arama = $('#aramaInput').val().toLowerCase();
        var paket = $('#paketFiltre').val();
        var durum = $('#durumFiltre').val();

        var filtrelenmis = tumMagazalar.filter(function (m) {
            var mAd = (m.magazaAdi || '').toLowerCase();
            var mEposta = (m.eposta || '').toLowerCase();
            var uyarArama = !arama || mAd.includes(arama) || mEposta.includes(arama);
            var uyarPaket = !paket || m.paketTuru === paket;
            var uyarDurum = !durum || (durum === 'aktif' ? m.aktifMi : !m.aktifMi);
            return uyarArama && uyarPaket && uyarDurum;
        });

        var tbody = $('#magazaTableBody');
        tbody.empty();

        if (filtrelenmis.length === 0) {
            tbody.append('<tr><td colspan="5" class="text-center py-4 text-muted"><i class="fas fa-box-open me-2"></i>Kayıtlı mağaza bulunamadı.</td></tr>');
            return;
        }

        filtrelenmis.forEach(function (m) {
            var tr = $('<tr>');
            var tdInfo = $('<td>').html(
                '<div class="d-flex align-items-center">' +
                    '<div class="avatar-circle me-3" style="background-color:' + rastgeleRenk(m.id) + '">' + ilkHarf(m.magazaAdi) + '</div>' +
                    '<div><div class="fw-bold text-dark">' + m.magazaAdi + '</div><div class="text-muted small">' + m.eposta + '</div></div>' +
                '</div>'
            );
            var tdPaket = $('<td>').html(paketBadge(m.paketTuru));
            var tdKomisyon = $('<td>').html('<span class="fw-semibold text-dark">%' + m.komisyonOrani + '</span>');
            var tdDurum = $('<td>').html(durumBadge(m.aktifMi));
            var tdAksiyon = $('<td>').addClass('text-end').html(
                '<div class="btn-group">' +
                    '<button class="btn btn-sm btn-icon border btn-duzenle-ac" data-id="' + m.id + '"><i class="fas fa-edit text-primary"></i></button>' +
                    '<button class="btn btn-sm btn-icon border btn-sil-ac" data-id="' + m.id + '" data-ad="' + m.magazaAdi + '"><i class="fas fa-trash-alt text-danger"></i></button>' +
                '</div>'
            );
            tr.append(tdInfo, tdPaket, tdKomisyon, tdDurum, tdAksiyon);
            tbody.append(tr);
        });
    }

    function kpiGuncelle() {
        $('#kpiToplam').text(tumMagazalar.length);
        $('#kpiAktif').text(tumMagazalar.filter(function(m){ return m.aktifMi; }).length);
        $('#kpiPremium').text(tumMagazalar.filter(function(m){ return m.paketTuru === 'Premium'; }).length);
    }

    $('#yeniMagazaBtn').on('click', function() {
        new bootstrap.Modal(document.getElementById('yeniMagazaModal')).show();
    });

    // =============================================
    // YENİ MAĞAZA EKLEME - DÜZELTİLDİ
    // =============================================
// ... diğer kodlar aynı

$('#yeniMagazaForm').on('submit', function (e) {
    e.preventDefault();
    
    var sifre = $('#magazaSifre').val();
    var magazaAdi = $('#magazaAdi').val().trim();
    var eposta = $('#magazaEposta').val().trim();
    var paket = $('#magazaPaket').val();
    var komisyon = parseFloat($('#magazaKomisyon').val()) || 0;
    var aktif = $('#magazaAktif').is(':checked');

    // Frontend validasyon
    if (!magazaAdi) { abp.message.error('Mağaza adı gereklidir.'); return; }
    if (!eposta) { abp.message.error('E-posta gereklidir.'); return; }
    if (!sifre || sifre.length < 6) { abp.message.error('Şifre en az 6 karakter olmalıdır.'); return; }
    if (!paket) { abp.message.error('Paket türü gereklidir.'); return; }

    var inputDto = {
        MagazaAdi: magazaAdi,
        Eposta: eposta,
        SifreHash: sifre,
        PaketTuru: paket,
        KomisyonOrani: komisyon,
        MinimumKomisyonEsigi: 0,
        AktifMi: aktif
    };

    console.log('Gönderilen JSON:', JSON.stringify(inputDto));

    var savePromise = _magazaService
        ? _magazaService.create(inputDto)
        : $.ajax({
            url: '/api/app/magaza',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(inputDto),
            headers: { 'Accept': 'application/json' }
        });

    Promise.resolve(savePromise)
        .then(function (result) {
            abp.notify.success('Mağaza başarıyla eklendi.');
            bootstrap.Modal.getInstance(document.getElementById('yeniMagazaModal')).hide();
            $('#yeniMagazaForm')[0].reset();
            magazalariYukle();
        })
        .catch(function(err) {
            console.error('HATA DETAYI:', err);
            // Tüm hata objesini stringify edip alert ile göster
            alert('Hata detayı:\n' + JSON.stringify(err, null, 2));
            // Ayrıca abp.message.error da dene
            var hataMsg = err.responseJSON?.message || err.responseJSON?.error?.message || err.responseText || err.statusText || 'Bilinmeyen hata';
            abp.message.error(hataMsg, 'Hata');
        });
});
    // =============================================
    // GÜNCELLEME - DÜZELTİLDİ
    // =============================================
    var duzenlenecekId = null;

    $(document).on('click', '.btn-duzenle-ac', function () {
        duzenlenecekId = $(this).data('id');
        var m = tumMagazalar.find(function(item){ return item.id === duzenlenecekId; });
        if (!m) return;

        $('#editMagazaAdi').val(m.magazaAdi);
        $('#editMagazaEposta').val(m.eposta);
        $('#editMagazaPaket').val(m.paketTuru).trigger('change');
        $('#editMagazaKomisyon').val(m.komisyonOrani);
        $('#editMagazaAktif').prop('checked', m.aktifMi);

        new bootstrap.Modal(document.getElementById('editMagazaModal')).show();
    });

$('#editMagazaForm').on('submit', function (e) {
    e.preventDefault();
    if (!duzenlenecekId) return;

    var updateDto = {
        MagazaAdi: $('#editMagazaAdi').val().trim(),
        Eposta: $('#editMagazaEposta').val().trim(),
        SifreHash: "degismedi",
        PaketTuru: $('#editMagazaPaket').val(),
        KomisyonOrani: parseFloat($('#editMagazaKomisyon').val()) || 0,
        MinimumKomisyonEsigi: 0,
        AktifMi: $('#editMagazaAktif').is(':checked')
    };

    var updatePromise = _magazaService
        ? _magazaService.update(duzenlenecekId, updateDto)
        : $.ajax({
            url: '/api/app/magaza/' + duzenlenecekId,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(updateDto),
            headers: { 'Accept': 'application/json' }
        });

    Promise.resolve(updatePromise)
        .then(function () {
            abp.notify.info('Mağaza güncellendi.');
            bootstrap.Modal.getInstance(document.getElementById('editMagazaModal')).hide();
            magazalariYukle();
            duzenlenecekId = null;
        })
        .catch(function(err) {
            console.error('GÜNCELLEME HATASI:', err);
            var hataMsg = err.responseJSON?.message || err.responseJSON?.error?.message || err.responseText || err.statusText || 'Bilinmeyen hata';
            abp.message.error(hataMsg, 'Hata');
        });
});
    // =============================================
    // SİLME
    // =============================================
    var silinecekId = null;

    $(document).on('click', '.btn-sil-ac', function () {
        silinecekId = $(this).data('id');
        $('#silMagazaAdi').text($(this).data('ad'));
        new bootstrap.Modal(document.getElementById('silOnayModal')).show();
    });

    $('#silOnayBtn').on('click', function () {
        if (!silinecekId) return;

        var deletePromise = _magazaService 
            ? _magazaService.delete(silinecekId) 
            : $.ajax({ url: '/api/app/magaza/' + silinecekId, type: 'DELETE', headers: { 'Accept': 'application/json' } });

        Promise.resolve(deletePromise)
            .then(function () {
                abp.notify.warn('Mağaza silindi.');
                bootstrap.Modal.getInstance(document.getElementById('silOnayModal')).hide();
                magazalariYukle();
                silinecekId = null;
            })
            .catch(function(err) {
                console.error('SİLME HATASI:', err);
                var hataMsg = err.responseJSON?.message || err.responseJSON?.error?.message || err.responseText || err.statusText || 'Bilinmeyen hata';
                abp.message.error(hataMsg, 'Hata');
            });
    });

    // Filtreler
    $('#aramaInput').on('input', tabloYenile);
    $('#paketFiltre, #durumFiltre').on('change', tabloYenile);
    $('#filtreTemizle').on('click', function () {
        $('#aramaInput').val('');
        $('#paketFiltre').val('');
        $('#durumFiltre').val('');
        tabloYenile();
    });

    $('#magazaPaket, #editMagazaPaket').on('change', function () {
        var p = $(this).val();
        $(this).closest('.mb-3').find('.paket-yardim').text(p ? paketBilgileri[p] : '');
    });

    magazalariYukle();
});