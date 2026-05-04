$(function () {

    // =============================================
    // Tarih göster
    // =============================================
    var bugun = new Date();
    var gunler = ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
    var aylar = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran',
                 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    $('#currentDate').text(
        gunler[bugun.getDay()] + ', ' +
        bugun.getDate() + ' ' +
        aylar[bugun.getMonth()] + ' ' +
        bugun.getFullYear()
    );

    // =============================================
    // KPI Kartları
    // =============================================
    function yukleKpiVerileri() {
        // TODO: AnalyticsController hazır olduğunda:
        // abp.ajax({ url: abp.appPath + 'api/app/analytics/dashboard-ozet' })
        //     .done(function(data) {
        //         animasyonluSayac('#toplamMagaza', 0, data.toplamMagaza, 1200);
        //         animasyonluSayac('#toplamKullanici', 0, data.toplamKullanici, 1500);
        //         animasyonluSayac('#toplamUrun', 0, data.toplamUrun, 1800);
        //         $('#aktifMagazaSayisi').html('<i class="fas fa-circle text-success" style="font-size:.5rem"></i> ' + data.aktifMagaza + ' aktif');
        //         $('#toplamKomisyon').text('₺ ' + data.buAyKomisyon.toLocaleString('tr-TR'));
        //     });

        animasyonluSayac('#toplamMagaza', 0, 124, 1200);
        animasyonluSayac('#toplamKullanici', 0, 3842, 1500);
        animasyonluSayac('#toplamUrun', 0, 9210, 1800);
        $('#aktifMagazaSayisi').html(
            '<i class="fas fa-circle text-success" style="font-size:.5rem"></i> 97 aktif'
        );
        $('#toplamKomisyon').text('₺ 68.400');
        $('#toplamUrunDonut').text('9.210');
    }

    // =============================================
    // Sayaç animasyonu
    // =============================================
    function animasyonluSayac(selector, baslangic, bitis, sure) {
        var $el = $(selector);
        var adim = Math.ceil((bitis - baslangic) / (sure / 16));
        var mevcut = baslangic;
        var interval = setInterval(function () {
            mevcut += adim;
            if (mevcut >= bitis) {
                mevcut = bitis;
                clearInterval(interval);
            }
            $el.text(mevcut.toLocaleString('tr-TR'));
        }, 16);
    }

    // =============================================
    // Komisyon Grafiği (Bar / Line toggle)
    // =============================================
    var komisyonChartOrnek = null;
    var komisyonVerileri = {
        etiketler: ['Aralık', 'Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs'],
        veriler: [42000, 55000, 48000, 61000, 73000, 68000]
    };

    function yukleKomisyonGrafigi(tip) {
        var ctx = document.getElementById('komisyonChart');
        if (!ctx) return;

        if (komisyonChartOrnek) {
            komisyonChartOrnek.destroy();
        }

        tip = tip || 'bar';

        var dataset = {
            label: 'Komisyon Geliri (₺)',
            data: komisyonVerileri.veriler,
            borderColor: 'rgba(79, 70, 229, 0.8)',
            borderWidth: 2,
            tension: 0.4,
            pointBackgroundColor: 'rgba(79, 70, 229, 1)',
            pointRadius: 4
        };

        if (tip === 'bar') {
            dataset.backgroundColor = 'rgba(79, 70, 229, 0.15)';
            dataset.borderRadius = 6;
            dataset.borderSkipped = false;
        } else {
            dataset.backgroundColor = 'rgba(79, 70, 229, 0.08)';
            dataset.fill = true;
        }

        komisyonChartOrnek = new Chart(ctx, {
            type: tip,
            data: {
                labels: komisyonVerileri.etiketler,
                datasets: [dataset]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function (ctx) {
                                return '₺ ' + ctx.raw.toLocaleString('tr-TR');
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(0,0,0,0.05)' },
                        ticks: {
                            callback: function (val) {
                                return '₺' + (val / 1000) + 'K';
                            }
                        }
                    },
                    x: {
                        grid: { display: false }
                    }
                }
            }
        });
    }

    // Grafik tip toggle - global erişim için window'a bağla
    window.grafikiDegistir = function (tip) {
        yukleKomisyonGrafigi(tip);
        if (tip === 'bar') {
            $('#barBtn').addClass('active');
            $('#lineBtn').removeClass('active');
        } else {
            $('#lineBtn').addClass('active');
            $('#barBtn').removeClass('active');
        }
    };

    // =============================================
    // YENİ: Ürün Kategorisi Donut Chart
    // =============================================
    function yukleKategoriChart() {
        var ctx = document.getElementById('kategoriChart');
        if (!ctx) return;

        // TODO: API'den çekilecek
        // abp.ajax({ url: abp.appPath + 'api/app/analytics/kategori-dagilimi' })
        var kategoriler = [
            { ad: 'Üst Giyim',   deger: 3240, renk: '#4f46e5' },
            { ad: 'Alt Giyim',   deger: 2180, renk: '#0ea5e9' },
            { ad: 'Spor',        deger: 1860, renk: '#10b981' },
            { ad: 'Aksesuar',    deger: 1120, renk: '#f59e0b' },
            { ad: 'Dış Giyim',   deger:  810, renk: '#f43f5e' }
        ];

        var toplam = kategoriler.reduce(function (t, k) { return t + k.deger; }, 0);

        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: kategoriler.map(function (k) { return k.ad; }),
                datasets: [{
                    data: kategoriler.map(function (k) { return k.deger; }),
                    backgroundColor: kategoriler.map(function (k) { return k.renk; }),
                    borderWidth: 3,
                    borderColor: '#fff',
                    hoverBorderWidth: 3
                }]
            },
            options: {
                responsive: false,
                cutout: '68%',
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function (ctx) {
                                var pct = ((ctx.raw / toplam) * 100).toFixed(1);
                                return ctx.label + ': ' + ctx.raw.toLocaleString('tr-TR') + ' (%' + pct + ')';
                            }
                        }
                    }
                }
            }
        });

        // Legend oluştur
        var $legend = $('#kategoriLegend');
        $legend.empty();
        kategoriler.forEach(function (k) {
            var pct = ((k.deger / toplam) * 100).toFixed(1);
            $legend.append(
                '<div class="legend-item">' +
                    '<div class="legend-sol">' +
                        '<div class="legend-renk" style="background:' + k.renk + '"></div>' +
                        '<span class="legend-ad">' + k.ad + '</span>' +
                    '</div>' +
                    '<span class="legend-deger">%' + pct + '</span>' +
                '</div>'
            );
        });
    }

    // =============================================
    // YENİ: Paket Dağılımı Bar'ları
    // =============================================
    function yuklePaketDagilimi() {
        // TODO: API'den çekilecek
        var paketler = [
            { ad: 'Premium',    icon: 'fa-crown',         renk: '#f59e0b', barRenk: '#f59e0b', sayi: 38, toplam: 124 },
            { ad: 'Standart',   icon: 'fa-star',          renk: '#4f46e5', barRenk: '#4f46e5', sayi: 52, toplam: 124 },
            { ad: 'Başlangıç',  icon: 'fa-seedling',      renk: '#10b981', barRenk: '#10b981', sayi: 27, toplam: 124 },
            { ad: 'Deneme',     icon: 'fa-flask',         renk: '#6b7280', barRenk: '#6b7280', sayi:  7, toplam: 124 }
        ];

        var $konteyner = $('#paketDagilimi');
        $konteyner.empty();

        paketler.forEach(function (p) {
            var pct = Math.round((p.sayi / p.toplam) * 100);
            $konteyner.append(
                '<div class="paket-satir">' +
                    '<div class="paket-satir-ust">' +
                        '<div class="paket-ad">' +
                            '<i class="fas ' + p.icon + '" style="color:' + p.renk + '; font-size:0.8rem"></i>' +
                            p.ad +
                        '</div>' +
                        '<span class="paket-sayisi">' + p.sayi + ' mağaza (%' + pct + ')</span>' +
                    '</div>' +
                    '<div class="paket-bar">' +
                        '<div class="paket-bar-dolu" style="width: 0%; background:' + p.barRenk + '" ' +
                             'data-genislik="' + pct + '"></div>' +
                    '</div>' +
                '</div>'
            );
        });

        // Animasyonlu bar genişletme
        setTimeout(function () {
            $('.paket-bar-dolu').each(function () {
                $(this).css('width', $(this).data('genislik') + '%');
            });
        }, 200);
    }

    // =============================================
    // YENİ: AI Analiz Özeti
    // =============================================
    function yukleAiOzeti() {
        // TODO: abp.ajax ile AIController'dan çekilecek
        var metrikler = [
            {
                ikon: 'fa-brain',
                renk: 'bg-primary-soft text-primary',
                ad: 'NLP Analiz Edilen Yorum',
                deger: '14.820',
                degisim: '+12%',
                yukari: true
            },
            {
                ikon: 'fa-tshirt',
                renk: 'bg-info-soft text-info',
                ad: 'Beden Uyum Tahmini',
                deger: '8.430',
                degisim: '+8%',
                yukari: true
            },
            {
                ikon: 'fa-exclamation-triangle',
                renk: 'bg-warning-soft text-warning',
                ad: 'İade Riski Tespiti',
                deger: '1.240',
                degisim: '-3%',
                yukari: false
            },
            {
                ikon: 'fa-check-circle',
                renk: 'bg-success-soft text-success',
                ad: 'AI Doğru Tahmin Oranı',
                deger: '%91.4',
                degisim: '+1.2%',
                yukari: true
            }
        ];

        var $liste = $('#aiMetrikListesi');
        $liste.empty();

        metrikler.forEach(function (m) {
            var degisimSinif = m.yukari ? 'degisim-yukari' : 'degisim-asagi';
            var degisimOk = m.yukari
                ? '<i class="fas fa-arrow-up me-1"></i>'
                : '<i class="fas fa-arrow-down me-1"></i>';

            $liste.append(
                '<div class="ai-metrik-satir">' +
                    '<div class="ai-metrik-sol">' +
                        '<div class="ai-metrik-ikon ' + m.renk + '">' +
                            '<i class="fas ' + m.ikon + '"></i>' +
                        '</div>' +
                        '<span class="ai-metrik-ad">' + m.ad + '</span>' +
                    '</div>' +
                    '<div class="d-flex align-items-center gap-2">' +
                        '<span class="ai-metrik-deger">' + m.deger + '</span>' +
                        '<span class="ai-metrik-degisim ' + degisimSinif + '">' +
                            degisimOk + m.degisim +
                        '</span>' +
                    '</div>' +
                '</div>'
            );
        });
    }

    // =============================================
    // Vücut Uyum Skoru
    // =============================================
    function yukleUyumSkoru() {
        var skor = 78.4;
        $('#ortalamaUyumSkoru').text(skor.toFixed(1) + ' / 100');
        setTimeout(function () {
            $('#uyumSkoruBar').css('width', skor + '%');
        }, 400);
    }

    // =============================================
    // Widget İstatistikleri
    // =============================================
    function yukleWidgetIstatistikleri() {
        // TODO: API'den çekilecek
        setTimeout(function () {
            animasyonluSayac('#widgetSorguSayisi', 0, 24830, 1600);
        }, 300);
        $('#satinAlmaOrani').text('%34.7');
        setTimeout(function () {
            animasyonluSayac('#aiAttributionSayi', 0, 8612, 1800);
        }, 300);
    }

    // =============================================
    // Son Bildirimler
    // =============================================
    function yukleBildirimler() {
        var ornekBildirimler = [
            { baslik: 'Yeni AI analizi tamamlandı', zaman: '5 dk önce',  icon: 'fa-robot',               renk: 'bg-info-soft text-info' },
            { baslik: 'Sistem bakımı tamamlandı',   zaman: '1 saat önce', icon: 'fa-cog',                renk: 'bg-success-soft text-success' },
            { baslik: 'Komisyon eşiği aşıldı',      zaman: '3 saat önce', icon: 'fa-exclamation-triangle', renk: 'bg-warning-soft text-warning' },
            { baslik: 'NLP analizi bitti',           zaman: 'Dün',         icon: 'fa-brain',              renk: 'bg-primary-soft text-primary' }
        ];

        var $liste = $('#bildirimListesi');
        $liste.empty();

        ornekBildirimler.forEach(function (b) {
            $liste.append(
                '<li class="bildirim-item">' +
                    '<div class="bildirim-icon ' + b.renk + '">' +
                        '<i class="fas ' + b.icon + '"></i>' +
                    '</div>' +
                    '<div class="bildirim-icerik">' +
                        '<div class="bildirim-baslik">' + b.baslik + '</div>' +
                        '<div class="bildirim-zaman text-muted small">' + b.zaman + '</div>' +
                    '</div>' +
                '</li>'
            );
        });
    }

    // =============================================
    // Aktif Mağazalar Tablosu
    // =============================================
    function yukleMagazaTablosu() {
        var ornekMagazalar = [
            { ad: 'SportZone TR', paket: 'Premium',   komisyon: '%8', aktif: true },
            { ad: 'FashionHub',   paket: 'Standart',  komisyon: '%5', aktif: true },
            { ad: 'ActiveWear',   paket: 'Premium',   komisyon: '%8', aktif: false },
            { ad: 'FitStyle',     paket: 'Standart',  komisyon: '%5', aktif: true }
        ];

        var $tbody = $('#magazaTablosu');
        $tbody.empty();

        ornekMagazalar.forEach(function (m) {
            var durum = m.aktif
                ? '<span class="badge bg-success-soft text-success">Aktif</span>'
                : '<span class="badge bg-danger-soft text-danger">Pasif</span>';
            var paketBadge = m.paket === 'Premium'
                ? '<span class="badge bg-warning-soft text-warning"><i class="fas fa-crown me-1" style="font-size:.65rem"></i>' + m.paket + '</span>'
                : '<span class="badge bg-secondary-soft text-secondary">' + m.paket + '</span>';

            $tbody.append(
                '<tr>' +
                    '<td><strong>' + m.ad + '</strong></td>' +
                    '<td>' + paketBadge + '</td>' +
                    '<td>' + m.komisyon + '</td>' +
                    '<td>' + durum + '</td>' +
                '</tr>'
            );
        });
    }

    // =============================================
    // Tümünü Başlat
    // =============================================
    yukleKpiVerileri();
    yukleKomisyonGrafigi('bar');
    yukleUyumSkoru();
    yukleWidgetIstatistikleri();
    yukleKategoriChart();
    yuklePaketDagilimi();
    yukleAiOzeti();
    yukleBildirimler();
    yukleMagazaTablosu();

});
// =============================================
// 14. Gün UX İyileştirmeleri
// =============================================

// Sayfa yükleme animasyonu için body'ye class ekle
$('body').addClass('fitai-page');

// Yenileme butonu
$('#refreshDashboardBtn').on('click', function() {
    var $btn = $(this);
    $btn.find('i').addClass('fa-spin');
    
    setTimeout(function() {
        location.reload();
    }, 300);
});

// Rapor indirme simülasyonu
$('#exportDashboardBtn').on('click', function() {
    abp.message.info('Rapor hazırlanıyor ve indirilecek...', 'Bilgi');
});

// Tablo satırlarına tıklanabilirlik
$(document).on('click', '.dashboard-table tbody tr', function() {
    var magazaAdi = $(this).find('td:first strong').text();
    if(magazaAdi && magazaAdi !== '—') {
        window.location.href = '/Magazalar/Detay?name=' + encodeURIComponent(magazaAdi);
    }
});

// Kartlara hover animasyonu için gradient border effect
$('.kpi-card').addClass('gradient-border glow-hover');
$('.card').addClass('gradient-border');

// Card header gradient çizgi
$('.card-header').addClass('card-header-gradient');

// Yükleme sırasında loading göstergesi (veri çekilirken)
function showLoading($element) {
    if($element.find('.loading-spinner').length === 0) {
        $element.addClass('loading-overlay');
        $element.append('<div class="loading-spinner"></div>');
    }
}

function hideLoading($element) {
    $element.removeClass('loading-overlay');
    $element.find('.loading-spinner').remove();
}

// Örnek: KPI verileri yüklenirken loading göster
// showLoading($('.kpi-card').parent());
// ... veri geldikten sonra hideLoading(...)

// Bildirimlere hover efekti
$(document).on('mouseenter', '.bildirim-item', function() {
    $(this).css('transform', 'translateX(4px)');
}).on('mouseleave', '.bildirim-item', function() {
    $(this).css('transform', '');
});

// Tooltip desteği (isteğe bağlı)
if($.fn.tooltip) {
    $('[data-bs-toggle="tooltip"]').tooltip();
}