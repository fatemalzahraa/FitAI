$(function () {

    // =============================================
    // Tarih Gösterimi
    // =============================================
    function tarihGoster() {
        var tr = new Date().toLocaleDateString('tr-TR', {
            year: 'numeric', month: 'long', day: 'numeric'
        });
        $('#currentDate').text(tr);
    }
    tarihGoster();

    // =============================================
    // Örnek Veriler (API'den gelecek)
    // =============================================
    var dashboardVerileri = {
        toplamMagaza: 12,
        toplamKullanici: 845,
        toplamUrun: 1240,
        buAyKomisyon: 189500,
        ortalamaUyum: 86.5,
        widgetSorgu: 18750,
        donusumOrani: 24.8,
        aiAttributionSatis: 62400
    };

    // Mağazalar
    var magazalar = [
        { ad: 'SportZone TR', paket: 'Premium', komisyon: 82400, durum: 'aktif' },
        { ad: 'FashionHub', paket: 'Standart', komisyon: 61200, durum: 'aktif' },
        { ad: 'ActiveWear', paket: 'Premium', komisyon: 48700, durum: 'aktif' },
        { ad: 'FitStyle', paket: 'Standart', komisyon: 39600, durum: 'pasif' },
        { ad: 'RunnerShop', paket: 'Baslangic', komisyon: 31800, durum: 'aktif' }
    ];

    // Bildirimler
    var bildirimler = [
        { baslik: 'SportZone TR mağazası için yeni yorum analizi tamamlandı.', zaman: '2 dakika önce', icon: 'fas fa-robot', renk: 'info' },
        { baslik: 'Aylık komisyon raporu hazırlandı. PDF olarak indirebilirsiniz.', zaman: '1 saat önce', icon: 'fas fa-file-alt', renk: 'primary' },
        { baslik: 'Premium pakete geçiş yapan mağazalar: ActiveWear, FitStyle', zaman: '3 saat önce', icon: 'fas fa-crown', renk: 'warning' },
        { baslik: 'NLP analizinde %78 pozitif yorum oranı yakalandı.', zaman: '5 saat önce', icon: 'fas fa-brain', renk: 'success' },
        { baslik: 'Yeni ürün ekleme limiti güncellendi. Detaylar için tıklayın.', zaman: '1 gün önce', icon: 'fas fa-info-circle', renk: 'secondary' }
    ];

    // =============================================
    // KPI Güncelle
    // =============================================
    function kpiGuncelle() {
        $('#toplamMagaza').text(dashboardVerileri.toplamMagaza);
        $('#toplamKullanici').text(dashboardVerileri.toplamKullanici.toLocaleString('tr-TR'));
        $('#toplamUrun').text(dashboardVerileri.toplamUrun.toLocaleString('tr-TR'));
        $('#toplamKomisyon').text('₺ ' + dashboardVerileri.buAyKomisyon.toLocaleString('tr-TR'));

        var uyumYuzde = (dashboardVerileri.ortalamaUyum / 100);
        $('#ortalamaUyumSkoru').text(dashboardVerileri.ortalamaUyum.toFixed(1) + '/100');
        $('#uyumSkoruBar').css('width', dashboardVerileri.ortalamaUyum + '%');

        $('#widgetSorguSayisi').text(dashboardVerileri.widgetSorgu.toLocaleString('tr-TR'));
        $('#satinAlmaOrani').text('%' + dashboardVerileri.donusumOrani);
        $('#aiAttributionSayi').text('₺ ' + dashboardVerileri.aiAttributionSatis.toLocaleString('tr-TR'));
    }

    // =============================================
    // Grafik - Aylık Komisyon
    // =============================================
    var komisyonChart = null;

    function komisyonGrafikYukle() {
        var ctx = document.getElementById('komisyonChart');
        if (!ctx) return;
        if (komisyonChart) komisyonChart.destroy();

        var aylar = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran'];
        var veriler = [152000, 168000, 185000, 198000, 212000, 234000];

        komisyonChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: aylar,
                datasets: [{
                    label: 'Komisyon Geliri (₺)',
                    data: veriler,
                    backgroundColor: 'rgba(79, 70, 229, 0.3)',
                    borderColor: '#4f46e5',
                    borderWidth: 2,
                    borderRadius: 8,
                    barPercentage: 0.65,
                    categoryPercentage: 0.8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function(c) {
                                return '₺ ' + c.raw.toLocaleString('tr-TR');
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(0,0,0,0.04)' },
                        ticks: {
                            callback: function(v) {
                                return '₺' + (v / 1000).toFixed(0) + 'K';
                            }
                        }
                    },
                    x: { grid: { display: false } }
                }
            }
        });
    }

    // =============================================
    // Mağaza Tablosu
    // =============================================
    function magazaTablosuRender() {
        var $tbody = $('#magazaTablosu');
        $tbody.empty();

        if (magazalar.length === 0) {
            $tbody.html('<tr><td colspan="4" class="text-center py-4 text-muted">Veri bulunamadı</td></tr>');
            return;
        }

        magazalar.forEach(function(m) {
            var paketBadge = m.paket === 'Premium' 
                ? '<span class="badge bg-warning-soft text-warning"><i class="fas fa-crown me-1"></i>' + m.paket + '</span>'
                : m.paket === 'Standart'
                ? '<span class="badge bg-primary-soft text-primary">' + m.paket + '</span>'
                : '<span class="badge bg-secondary-soft text-secondary">' + m.paket + '</span>';
            
            var durumBadge = m.durum === 'aktif'
                ? '<span class="badge bg-success-soft text-success"><i class="fas fa-circle me-1" style="font-size:0.4rem"></i>Aktif</span>'
                : '<span class="badge bg-secondary-soft text-secondary">Pasif</span>';

            $tbody.append(
                '<tr>' +
                    '<td class="fw-semibold">' + m.ad + '</td>' +
                    '<td>' + paketBadge + '</td>' +
                    '<td class="fw-semibold">₺ ' + m.komisyon.toLocaleString('tr-TR') + '</td>' +
                    '<td>' + durumBadge + '</td>' +
                '</tr>'
            );
        });
    }

    // =============================================
    // Bildirim Listesi
    // =============================================
    function bildirimListesiRender() {
        var $liste = $('#bildirimListesi');
        $liste.empty();

        if (bildirimler.length === 0) {
            $liste.html('<li class="bildirim-item"><div class="text-muted">Bildirim bulunmuyor</div></li>');
            return;
        }

        bildirimler.forEach(function(b) {
            var iconClass = b.renk === 'primary' ? 'text-primary' 
                          : b.renk === 'success' ? 'text-success'
                          : b.renk === 'warning' ? 'text-warning'
                          : b.renk === 'info' ? 'text-info'
                          : 'text-secondary';
            $liste.append(
                '<li class="bildirim-item">' +
                    '<div class="bildirim-icon bg-' + b.renk + '-soft">' +
                        '<i class="' + b.icon + ' ' + iconClass + '"></i>' +
                    '</div>' +
                    '<div class="bildirim-icerik">' +
                        '<div class="bildirim-baslik">' + b.baslik + '</div>' +
                        '<div class="bildirim-zaman text-muted">' + b.zaman + '</div>' +
                    '</div>' +
                '</li>'
            );
        });
    }

    // =============================================
    // Yenileme Butonu
    // =============================================
    $('#refreshDashboardBtn').on('click', function() {
        var $btn = $(this);
        $btn.find('i').addClass('fa-spin');
        
        setTimeout(function() {
            // Simüle edilmiş yenileme
            komisyonGrafikYukle();
            kpiGuncelle();
            magazaTablosuRender();
            bildirimListesiRender();
            $btn.find('i').removeClass('fa-spin');
            abp.message.success('Dashboard yenilendi.', 'Başarılı');
        }, 800);
    });

    // Rapor indirme simülasyonu
    $('#exportDashboardBtn').on('click', function() {
        abp.message.info('Dashboard raporu hazırlanıyor...', 'Rapor');
    });

    // =============================================
    // Başlangıç
    // =============================================
    komisyonGrafikYukle();
    kpiGuncelle();
    magazaTablosuRender();
    bildirimListesiRender();

});