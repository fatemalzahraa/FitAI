$(function () {

    // =============================================
    // Yardımcı Fonksiyonlar
    // =============================================
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
        var cls = paket === 'Premium' ? 'paket-premium'
                : paket === 'Standart' ? 'paket-standart'
                : 'paket-baslangic';
        var icon = paket === 'Premium' ? '<i class="fas fa-crown me-1"></i>' : '';
        return '<span class="paket-badge ' + cls + '">' + icon + paket + '</span>';
    }

    function durumBadge(aktif) {
        return aktif
            ? '<span class="durum-aktif"><i class="fas fa-circle me-1" style="font-size:0.4rem;"></i>Aktif</span>'
            : '<span class="durum-pasif"><i class="fas fa-circle me-1" style="font-size:0.4rem;"></i>Pasif</span>';
    }

    // =============================================
    // Veri (Placeholder)
    // =============================================
    var tumMagazalar = [];
    var duzenlenecekId = null;

    function yukleMagazalar() {
        tumMagazalar = [
            { id: 1, magazaAdi: 'SportZone TR', eposta: 'info@sporzone.com', komisyonOrani: 8,   minimumKomisyonEsigi: 500,  paketTuru: 'Premium',   aktifMi: true  },
            { id: 2, magazaAdi: 'FashionHub',   eposta: 'info@fashionhub.com', komisyonOrani: 5, minimumKomisyonEsigi: 300,  paketTuru: 'Standart',  aktifMi: true  },
            { id: 3, magazaAdi: 'ActiveWear',   eposta: 'info@activewear.com', komisyonOrani: 5, minimumKomisyonEsigi: null, paketTuru: 'Standart',  aktifMi: false },
            { id: 4, magazaAdi: 'FitStyle',     eposta: 'info@fitstyle.com',   komisyonOrani: 3, minimumKomisyonEsigi: null, paketTuru: 'Baslangic', aktifMi: true  },
            { id: 5, magazaAdi: 'RunnerShop',   eposta: 'info@runner.com',     komisyonOrani: 8, minimumKomisyonEsigi: 750,  paketTuru: 'Premium',   aktifMi: true  },
        ];

        tabloYenile();
        kpiGuncelle();
    }

    // =============================================
    // KPI Güncelle
    // =============================================
    function kpiGuncelle() {
        var aktif   = tumMagazalar.filter(function(m){ return m.aktifMi; }).length;
        var premium = tumMagazalar.filter(function(m){ return m.paketTuru === 'Premium'; }).length;
        var ortKom  = tumMagazalar.length
            ? (tumMagazalar.reduce(function(t,m){ return t + m.komisyonOrani; }, 0) / tumMagazalar.length).toFixed(1)
            : 0;

        $('#toplamMagaza').text(tumMagazalar.length);
        $('#aktifMagaza').text(aktif);
        $('#premiumMagaza').text(premium);
        $('#ortKomisyon').text('%' + ortKom);
    }

    // =============================================
    // Tabloyu Render Et
    // =============================================
    function tabloYenile() {
        var arama = $('#aramaInput').val().toLowerCase();
        var paket = $('#paketFiltre').val();
        var durum = $('#durumFiltre').val();

        var filtre = tumMagazalar.filter(function (m) {
            var aramaUyumu = !arama ||
                m.magazaAdi.toLowerCase().includes(arama) ||
                m.eposta.toLowerCase().includes(arama);
            var paketUyumu = !paket || m.paketTuru === paket;
            var durumUyumu = !durum ||
                (durum === 'aktif' && m.aktifMi) ||
                (durum === 'pasif' && !m.aktifMi);
            return aramaUyumu && paketUyumu && durumUyumu;
        });

        var $tbody = $('#magazaTablosu');
        $tbody.empty();

        if (filtre.length === 0) {
            $tbody.html(
                '<tr class="empty-row">' +
                    '<td colspan="8" class="text-center py-5">' +
                        '<i class="fas fa-store-slash fa-2x text-muted mb-2 d-block"></i>' +
                        '<span class="text-muted">Mağaza bulunamadı</span>' +
                    '</td>' +
                '</tr>'
            );
            $('#toplamKayitYazi').text('0 mağaza');
            return;
        }

        filtre.forEach(function (m) {
            var renk = rastgeleRenk(m.id);
            $tbody.append(
                '<tr>' +
                    '<td class="text-muted small">' + m.id + '</td>' +
                    '<td>' +
                        '<div class="magaza-bilgi">' +
                            '<div class="magaza-ikon" style="background:' + renk + '">' + ilkHarf(m.magazaAdi) + '</div>' +
                            '<span class="magaza-ad">' + m.magazaAdi + '</span>' +
                        '</div>' +
                    '</td>' +
                    '<td class="text-muted">' + m.eposta + '</td>' +
                    '<td>' + paketBadge(m.paketTuru) + '</td>' +
                    '<td><strong>%' + m.komisyonOrani + '</strong></td>' +
                    '<td class="text-muted">' + (m.minimumKomisyonEsigi ? '₺' + m.minimumKomisyonEsigi.toLocaleString('tr-TR') : '—') + '</td>' +
                    '<td>' + durumBadge(m.aktifMi) + '</td>' +
                    '<td class="text-center">' +
                        '<div class="d-flex gap-1 justify-content-center">' +
                            '<button class="btn-islem btn-duzenle btn-duzenle-ac" data-id="' + m.id + '" title="Düzenle">' +
                                '<i class="fas fa-pencil-alt"></i>' +
                            '</button>' +
                            '<button class="btn-islem btn-sil btn-sil-ac" data-id="' + m.id + '" data-ad="' + m.magazaAdi + '" title="Sil">' +
                                '<i class="fas fa-trash"></i>' +
                            '</button>' +
                        '</div>' +
                    '</td>' +
                '</tr>'
            );
        });

        $('#toplamKayitYazi').text(filtre.length + ' / ' + tumMagazalar.length + ' mağaza');
    }

    // =============================================
    // Modal: Yeni Mağaza Aç
    // =============================================
    $('#yeniMagazaBtn').on('click', function () {
        duzenlenecekId = null;
        formTemizle();
        $('#magazaModalBaslik').html('<i class="fas fa-store me-2 text-primary"></i>Yeni Mağaza Ekle');
        $('#sifreAlani').show();
        new bootstrap.Modal(document.getElementById('magazaModal')).show();
    });

    // =============================================
    // Modal: Düzenle Aç
    // =============================================
    $(document).on('click', '.btn-duzenle-ac', function () {
        var id = $(this).data('id');
        var m  = tumMagazalar.find(function(x){ return x.id === id; });
        if (!m) return;

        duzenlenecekId = id;
        formTemizle();
        $('#magazaModalBaslik').html('<i class="fas fa-store me-2 text-warning"></i>Mağazayı Düzenle');
        $('#sifreAlani').hide();

        $('#f_magazaAdi').val(m.magazaAdi);
        $('#f_eposta').val(m.eposta);
        $('#f_paketTuru').val(m.paketTuru || '');
        $('#f_komisyonOrani').val(m.komisyonOrani);
        $('#f_minimumEsik').val(m.minimumKomisyonEsigi || '');
        $('#f_aktifMi').prop('checked', m.aktifMi);
        paketBilgiGoster(m.paketTuru);

        new bootstrap.Modal(document.getElementById('magazaModal')).show();
    });

    // =============================================
    // Paket Seçilince Bilgi Göster
    // =============================================
    $('#f_paketTuru').on('change', function () {
        paketBilgiGoster($(this).val());
        var oranlar = { 'Baslangic': 3, 'Standart': 5, 'Premium': 8 };
        if (oranlar[$(this).val()]) {
            $('#f_komisyonOrani').val(oranlar[$(this).val()]);
        }
    });

    function paketBilgiGoster(paket) {
        if (paketBilgileri[paket]) {
            $('#paketBilgiText').text(paketBilgileri[paket]);
            $('#paketBilgiKarti').slideDown(200);
        } else {
            $('#paketBilgiKarti').slideUp(200);
        }
    }

    // =============================================
    // Şifre Göster/Gizle
    // =============================================
    $('#sifreGoster').on('click', function () {
        var inp = $('#f_sifre');
        var tip = inp.attr('type') === 'password' ? 'text' : 'password';
        inp.attr('type', tip);
        $(this).find('i').toggleClass('fa-eye fa-eye-slash');
    });

    // =============================================
    // Form Doğrulama & Kaydet
    // =============================================
    $('#magazaKaydetBtn').on('click', function () {
        if (!formDogrula()) return;

        var veri = {
            magazaAdi:             $('#f_magazaAdi').val().trim(),
            eposta:                $('#f_eposta').val().trim(),
            sifre:                 $('#f_sifre').val(),
            paketTuru:             $('#f_paketTuru').val() || null,
            komisyonOrani:         parseFloat($('#f_komisyonOrani').val()),
            minimumKomisyonEsigi:  $('#f_minimumEsik').val() ? parseFloat($('#f_minimumEsik').val()) : null,
            aktifMi:               $('#f_aktifMi').is(':checked')
        };

        if (duzenlenecekId) {
            var idx = tumMagazalar.findIndex(function(m){ return m.id === duzenlenecekId; });
            if (idx !== -1) Object.assign(tumMagazalar[idx], veri);
            abp.notify.success('Mağaza başarıyla güncellendi.', 'Başarılı');
        } else {
            veri.id = tumMagazalar.length + 1;
            tumMagazalar.push(veri);
            abp.notify.success('Mağaza başarıyla oluşturuldu.', 'Başarılı');
        }

        bootstrap.Modal.getInstance(document.getElementById('magazaModal')).hide();
        tabloYenile();
        kpiGuncelle();
    });

    function formDogrula() {
        var gecerli = true;

        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').text('');

        if (!$('#f_magazaAdi').val().trim()) {
            $('#f_magazaAdi').addClass('is-invalid');
            $('#err_magazaAdi').text('Mağaza adı zorunludur.');
            gecerli = false;
        }

        var eposta = $('#f_eposta').val().trim();
        if (!eposta || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(eposta)) {
            $('#f_eposta').addClass('is-invalid');
            $('#err_eposta').text('Geçerli bir e-posta giriniz.');
            gecerli = false;
        }

        if (!duzenlenecekId && $('#f_sifre').val().length < 8) {
            $('#f_sifre').addClass('is-invalid');
            $('#err_sifre').text('Şifre en az 8 karakter olmalıdır.');
            gecerli = false;
        }

        var oran = parseFloat($('#f_komisyonOrani').val());
        if (isNaN(oran) || oran < 0 || oran > 100) {
            $('#f_komisyonOrani').addClass('is-invalid');
            $('#err_komisyonOrani').text('Geçerli bir oran girin (0-100).');
            gecerli = false;
        }

        return gecerli;
    }

    function formTemizle() {
        $('#f_magazaAdi, #f_eposta, #f_sifre, #f_minimumEsik, #f_komisyonOrani').val('');
        $('#f_paketTuru').val('');
        $('#f_aktifMi').prop('checked', true);
        $('#paketBilgiKarti').hide();
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').text('');
    }

    // =============================================
    // Silme
    // =============================================
    var silinecekId = null;

    $(document).on('click', '.btn-sil-ac', function () {
        silinecekId = $(this).data('id');
        $('#silMagazaAdi').text($(this).data('ad'));
        new bootstrap.Modal(document.getElementById('silOnayModal')).show();
    });

    $('#silOnayBtn').on('click', function () {
        if (!silinecekId) return;
        tumMagazalar = tumMagazalar.filter(function(m){ return m.id !== silinecekId; });
        abp.notify.warn('Mağaza silindi.', 'Silindi');
        bootstrap.Modal.getInstance(document.getElementById('silOnayModal')).hide();
        tabloYenile();
        kpiGuncelle();
        silinecekId = null;
    });

    // =============================================
    // Filtreler
    // =============================================
    $('#aramaInput').on('input', function() {
        tabloYenile();
    });
    $('#paketFiltre, #durumFiltre').on('change', function() {
        tabloYenile();
    });
    $('#filtreTemizle').on('click', function () {
        $('#aramaInput').val('');
        $('#paketFiltre').val('');
        $('#durumFiltre').val('');
        tabloYenile();
    });

    // =============================================
    // Başlat
    // =============================================
    yukleMagazalar();

});