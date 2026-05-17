$(function () {

    // =============================================
    // Renk Paleti (avatar için)
    // =============================================
    var renkler = ['#4f46e5', '#0ea5e9', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899'];
    function rastgeleRenk(index) {
        return renkler[index % renkler.length];
    }
    function basSesHarfler(ad) {
        return ad ? ad.charAt(0).toUpperCase() : '?';
    }

    // =============================================
    // Rol Badge HTML
    // =============================================
    function rolBadge(rol) {
        var cls = 'rol-kullanici';
        if (rol === 'Admin') cls = 'rol-admin';
        else if (rol === 'MagazaSahibi') cls = 'rol-magazasahibi';
        return '<span class="rol-badge ' + cls + '">' + (rol || '—') + '</span>';
    }

    // =============================================
    // Durum Badge HTML
    // =============================================
    function durumBadge(aktif) {
        return aktif
            ? '<span class="badge durum-aktif"><i class="fas fa-circle me-1" style="font-size:0.5rem"></i>Aktif</span>'
            : '<span class="badge durum-pasif"><i class="fas fa-circle me-1" style="font-size:0.5rem"></i>Pasif</span>';
    }

    // =============================================
    // Tarih Formatla
    // =============================================
    function tarihFormatla(tarih) {
        if (!tarih) return '<span class="text-muted">—</span>';
        var d = new Date(tarih);
        return d.toLocaleDateString('tr-TR', { day: '2-digit', month: 'short', year: 'numeric' });
    }

    // =============================================
    // Kullanıcı Listesini Yükle
    // =============================================
    var tumKullanicilar = [];

    function yukleKullanicilar() {
        // TODO: abp.ajax ile KullaniciController'dan çekilecek
        // abp.ajax({ url: abp.appPath + 'api/app/kullanici' })
        //     .done(function(data) { tumKullanicilar = data.items; tabloYenile(); });

        // Placeholder veri — Kullanici entity alanlarına göre
        tumKullanicilar = [
            { id: 1, ad: 'Ayşe Kaya',     eposta: 'ayse@sporzone.com',   rol: 'MagazaSahibi', magazaId: 1, magazaAdi: 'SportZone TR', aktifMi: true,  sonGirisTarihi: '2026-05-02T10:30:00', vucutTipi: 'Atletik',   boy: 168, kilo: 62 },
            { id: 2, ad: 'Mehmet Demir',  eposta: 'mehmet@fitai.com',    rol: 'Admin',        magazaId: null, magazaAdi: '—',          aktifMi: true,  sonGirisTarihi: '2026-05-03T08:15:00', vucutTipi: null,       boy: null, kilo: null },
            { id: 3, ad: 'Fatma Çelik',   eposta: 'fatma@fashionhub.com',rol: 'MagazaSahibi', magazaId: 2, magazaAdi: 'FashionHub',  aktifMi: true,  sonGirisTarihi: '2026-04-30T14:22:00', vucutTipi: 'İnce',     boy: 162, kilo: 54 },
            { id: 4, ad: 'Ali Yıldız',    eposta: 'ali@activewear.com',  rol: 'MagazaSahibi', magazaId: 3, magazaAdi: 'ActiveWear',  aktifMi: false, sonGirisTarihi: '2026-04-15T09:00:00', vucutTipi: 'Büyük Boy', boy: 182, kilo: 90 },
            { id: 5, ad: 'Zeynep Arslan', eposta: 'zeynep@fitstyle.com', rol: 'MagazaSahibi', magazaId: 4, magazaAdi: 'FitStyle',    aktifMi: true,  sonGirisTarihi: '2026-05-01T16:45:00', vucutTipi: 'Orta Boy',  boy: 165, kilo: 60 },
            { id: 6, ad: 'Can Öztürk',    eposta: 'can@kullanici.com',   rol: 'Kullanici',    magazaId: null, magazaAdi: '—',         aktifMi: true,  sonGirisTarihi: '2026-05-03T07:00:00', vucutTipi: 'Atletik',  boy: 178, kilo: 75 },
        ];

        tabloYenile();
        $('#toplamKayitYazi').text('Toplam ' + tumKullanicilar.length + ' kullanıcı');
    }

    // =============================================
    // Tabloyu Render Et
    // =============================================
    function tabloYenile() {
        var arama  = $('#aramaInput').val().toLowerCase();
        var rol    = $('#rolFiltre').val();
        var durum  = $('#durumFiltre').val();

        var filtrelenmis = tumKullanicilar.filter(function (k) {
            var aramaUyumu = !arama ||
                k.ad.toLowerCase().includes(arama) ||
                k.eposta.toLowerCase().includes(arama);
            var rolUyumu   = !rol    || k.rol === rol;
            var durumUyumu = !durum  ||
                (durum === 'aktif' && k.aktifMi) ||
                (durum === 'pasif' && !k.aktifMi);
            return aramaUyumu && rolUyumu && durumUyumu;
        });

        var $tbody = $('#kullaniciTablosu');
        $tbody.empty();

        if (filtrelenmis.length === 0) {
            $tbody.html('<tr><td colspan="8" class="text-center py-4 text-muted">Sonuç bulunamadı.</td></tr>');
            return;
        }

        filtrelenmis.forEach(function (k, i) {
            var renk = rastgeleRenk(k.id);
            $tbody.append(
                '<tr data-id="' + k.id + '">' +
                    '<td class="text-muted small">' + k.id + '</td>' +
                    '<td>' +
                        '<div class="kullanici-bilgi">' +
                            '<div class="kullanici-avatar" style="background:' + renk + '">' + basSesHarfler(k.ad) + '</div>' +
                            '<span class="kullanici-ad">' + k.ad + '</span>' +
                        '</div>' +
                    '</td>' +
                    '<td class="text-muted">' + k.eposta + '</td>' +
                    '<td>' + rolBadge(k.rol) + '</td>' +
                    '<td class="text-muted small">' + (k.magazaAdi || '—') + '</td>' +
                    '<td class="text-muted small">' + tarihFormatla(k.sonGirisTarihi) + '</td>' +
                    '<td>' + durumBadge(k.aktifMi) + '</td>' +
                    '<td class="text-center">' +
                        '<button class="btn-islem btn-detay btn-detay-ac" data-id="' + k.id + '" title="Detay Görüntüle">' +
                            '<i class="fas fa-eye"></i>' +
                        '</button>' +
                    '</td>' +
                '</tr>'
            );
        });

        $('#toplamKayitYazi').text(filtrelenmis.length + ' / ' + tumKullanicilar.length + ' kullanıcı gösteriliyor');
    }

    // =============================================
    // Detay Modal Aç
    // =============================================
    $(document).on('click', '.btn-detay-ac', function () {
        var id = $(this).data('id');
        var k  = tumKullanicilar.find(function (x) { return x.id === id; });
        if (!k) return;

        var renk = rastgeleRenk(k.id);

        var html =
            '<div class="profil-kart-header">' +
                '<div class="profil-avatar-buyuk" style="background:' + renk + '">' + basSesHarfler(k.ad) + '</div>' +
                '<div>' +
                    '<div style="font-size:1.2rem;font-weight:700;color:#1a1a2e">' + k.ad + '</div>' +
                    '<div class="text-muted small">' + k.eposta + '</div>' +
                    '<div class="mt-1">' + rolBadge(k.rol) + ' ' + durumBadge(k.aktifMi) + '</div>' +
                '</div>' +
            '</div>' +

            '<p class="profil-section-baslik">Hesap Bilgileri</p>' +
            '<div class="profil-detay-grid mb-3">' +
                '<div class="profil-detay-item">' +
                    '<div class="profil-detay-label">Mağaza</div>' +
                    '<div class="profil-detay-deger">' + (k.magazaAdi || '—') + '</div>' +
                '</div>' +
                '<div class="profil-detay-item">' +
                    '<div class="profil-detay-label">Son Giriş</div>' +
                    '<div class="profil-detay-deger">' + tarihFormatla(k.sonGirisTarihi) + '</div>' +
                '</div>' +
            '</div>' +

            '<p class="profil-section-baslik">Profil Bilgileri (KullaniciProfil)</p>' +
            '<div class="profil-detay-grid">' +
                '<div class="profil-detay-item">' +
                    '<div class="profil-detay-label">Vücut Tipi</div>' +
                    '<div class="profil-detay-deger">' + (k.vucutTipi || '—') + '</div>' +
                '</div>' +
                '<div class="profil-detay-item">' +
                    '<div class="profil-detay-label">Boy / Kilo</div>' +
                    '<div class="profil-detay-deger">' +
                        (k.boy ? k.boy + ' cm / ' + k.kilo + ' kg' : '—') +
                    '</div>' +
                '</div>' +
            '</div>';

        $('#kullaniciDetayIcerik').html(html);
        new bootstrap.Modal(document.getElementById('kullaniciDetayModal')).show();
    });

    // =============================================
    // Filtre Dinleyicileri
    // =============================================
    $('#aramaInput').on('input', tabloYenile);
    $('#rolFiltre, #durumFiltre').on('change', tabloYenile);
    $('#filtreTemizle').on('click', function () {
        $('#aramaInput').val('');
        $('#rolFiltre').val('');
        $('#durumFiltre').val('');
        tabloYenile();
    });

    // =============================================
    // Başlat
    // =============================================
    yukleKullanicilar();

});