$(function () {

    // =============================================
    // Sekme Yönetimi
    // =============================================
    var aktifTab = 'gelir';
    var grafiklerYuklendi = {};

    $('.analiz-nav-btn').on('click', function () {
        var tab = $(this).data('tab');
        if (tab === aktifTab) return;

        $('.analiz-nav-btn').removeClass('active');
        $(this).addClass('active');

        $('.analiz-tab-icerik').hide();
        $('#tab-' + tab).show();

        aktifTab = tab;

        // Her tab ilk açıldığında grafiklerini yükle
        if (!grafiklerYuklendi[tab]) {
            grafiklerYuklendi[tab] = true;
            setTimeout(function () {
                tabGrafikleriniYukle(tab);
            }, 50);
        }
    });

    function tabGrafikleriniYukle(tab) {
        if (tab === 'gelir')     yukleGelirTab();
        if (tab === 'nlp')       yukleNlpTab();
        if (tab === 'magaza')    yukleMagazaTab();
        if (tab === 'aktivite')  yukleAktiviteTab();
    }

    // =============================================
    // Dönem Filtresi
    // =============================================
    $('#donemFiltre').on('change', function () {
        grafiklerYuklendi = {};
        tabGrafikleriniYukle(aktifTab);
        grafiklerYuklendi[aktifTab] = true;
    });

    $('#yenileBtn').on('click', function () {
        var $ikon = $(this).find('i');
        $ikon.addClass('fa-spin');
        setTimeout(function () {
            $ikon.removeClass('fa-spin');
            tabGrafikleriniYukle(aktifTab);
        }, 800);
    });

    // =============================================
    // SEKME 1: GELİR ANALİZİ
    // =============================================
    var gelirZamanChart = null;
    var gelirPaketChart = null;

    function yukleGelirTab() {
        // KPI değerleri
        // TODO: abp.ajax({ url: abp.appPath + 'api/app/analytics/gelir-ozet' })
        $('#gelir_toplamKomisyon').text('₺ 412.800');
        $('#gelir_ortKomisyon').text('₺ 13.760');
        $('#gelir_enIyiMagaza').text('SportZone TR');
        $('#gelir_ortOran').text('%6.8');

        yukleGelirZamanChart('line');
        yukleGelirPaketChart();
    }

    function yukleGelirZamanChart(tip) {
        var ctx = document.getElementById('gelirZamanChart');
        if (!ctx) return;
        if (gelirZamanChart) gelirZamanChart.destroy();

        // TODO: API'den çekilecek - dönem filtresine göre değişecek
        var etiketler = [
            '1 May','3 May','5 May','7 May','9 May','11 May',
            '13 May','15 May','17 May','19 May','21 May','23 May',
            '25 May','27 May','29 May','31 May'
        ];
        var veriler = [8200,9400,7800,11200,10500,13800,12400,15600,14200,16800,15900,18200,17400,19800,18600,21200];

        gelirZamanChart = new Chart(ctx, {
            type: tip,
            data: {
                labels: etiketler,
                datasets: [{
                    label: 'Komisyon Geliri (₺)',
                    data: veriler,
                    borderColor: '#4f46e5',
                    backgroundColor: tip === 'line'
                        ? 'rgba(79,70,229,0.07)'
                        : 'rgba(79,70,229,0.18)',
                    borderWidth: 2.5,
                    fill: tip === 'line',
                    tension: 0.4,
                    pointRadius: tip === 'line' ? 3 : 0,
                    pointBackgroundColor: '#4f46e5',
                    borderRadius: tip === 'bar' ? 5 : 0
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                return '₺ ' + c.raw.toLocaleString('tr-TR');
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: false,
                        grid: { color: 'rgba(0,0,0,0.04)' },
                        ticks: {
                            callback: function (v) {
                                return '₺' + (v / 1000).toFixed(0) + 'K';
                            }
                        }
                    },
                    x: { grid: { display: false } }
                }
            }
        });
    }

    // Global erişim için
    window.gelirGrafikiDegistir = function (tip, btn) {
        $('.card-header .btn-xs').removeClass('active');
        $(btn).addClass('active');
        yukleGelirZamanChart(tip);
    };

    function yukleGelirPaketChart() {
        var ctx = document.getElementById('gelirPaketChart');
        if (!ctx) return;
        if (gelirPaketChart) gelirPaketChart.destroy();

        var paketler = [
            { ad: 'Premium',   deger: 198400, renk: '#f59e0b' },
            { ad: 'Standart',  deger: 152600, renk: '#4f46e5' },
            { ad: 'Başlangıç', deger:  61800, renk: '#10b981' }
        ];
        var toplam = paketler.reduce(function (t, p) { return t + p.deger; }, 0);

        $('#gelirPaketToplam').text((toplam / 1000).toFixed(0) + 'K');

        gelirPaketChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: paketler.map(function (p) { return p.ad; }),
                datasets: [{
                    data: paketler.map(function (p) { return p.deger; }),
                    backgroundColor: paketler.map(function (p) { return p.renk; }),
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
                            label: function (c) {
                                var pct = ((c.raw / toplam) * 100).toFixed(1);
                                return c.label + ': ₺' + c.raw.toLocaleString('tr-TR') + ' (%' + pct + ')';
                            }
                        }
                    }
                }
            }
        });

        // Legend
        var $leg = $('#gelirPaketLegend');
        $leg.empty();
        paketler.forEach(function (p) {
            var pct = ((p.deger / toplam) * 100).toFixed(1);
            $leg.append(
                '<div class="legend-item">' +
                    '<div class="legend-sol">' +
                        '<div class="legend-renk" style="background:' + p.renk + '"></div>' +
                        '<span class="legend-ad">' + p.ad + '</span>' +
                    '</div>' +
                    '<span class="legend-deger">%' + pct + '</span>' +
                '</div>'
            );
        });
    }

    // =============================================
    // SEKME 2: NLP & DUYGU
    // =============================================
    function yukleNlpTab() {
        // KPI
        $('#nlp_toplam').text('14.820');
        $('#nlp_pozitif').text('10.078');
        $('#nlp_negatif').text('2.371');
        $('#nlp_notr').text('2.371');

        yukleNlpTrendChart();
        yukleNlpDuyguChart();
        yukleKelimeBulutu();
        yukleKategoriMemnuniyetChart();
    }

    function yukleNlpTrendChart() {
        var ctx = document.getElementById('nlpTrendChart');
        if (!ctx) return;

        var aylar = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran'];
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: aylar,
                datasets: [
                    {
                        label: 'Pozitif',
                        data: [1200, 1450, 1380, 1620, 1890, 1780],
                        borderColor: '#10b981',
                        backgroundColor: 'rgba(16,185,129,0.08)',
                        fill: true,
                        tension: 0.4,
                        borderWidth: 2,
                        pointRadius: 3
                    },
                    {
                        label: 'Negatif',
                        data: [420, 380, 450, 310, 290, 340],
                        borderColor: '#ef4444',
                        backgroundColor: 'rgba(239,68,68,0.06)',
                        fill: true,
                        tension: 0.4,
                        borderWidth: 2,
                        pointRadius: 3
                    },
                    {
                        label: 'Nötr',
                        data: [380, 320, 410, 380, 420, 390],
                        borderColor: '#6b7280',
                        backgroundColor: 'rgba(107,114,128,0.05)',
                        fill: true,
                        tension: 0.4,
                        borderWidth: 2,
                        pointRadius: 3,
                        borderDash: [4, 3]
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                        labels: { font: { size: 11 }, boxWidth: 12 }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(0,0,0,0.04)' }
                    },
                    x: { grid: { display: false } }
                }
            }
        });
    }

    function yukleNlpDuyguChart() {
        var ctx = document.getElementById('nlpDuyguChart');
        if (!ctx) return;

        var duygular = [
            { ad: 'Pozitif', deger: 10078, renk: '#10b981' },
            { ad: 'Negatif', deger:  2371, renk: '#ef4444' },
            { ad: 'Nötr',    deger:  2371, renk: '#9ca3af' }
        ];
        var toplam = duygular.reduce(function (t, d) { return t + d.deger; }, 0);

        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: duygular.map(function (d) { return d.ad; }),
                datasets: [{
                    data: duygular.map(function (d) { return d.deger; }),
                    backgroundColor: duygular.map(function (d) { return d.renk; }),
                    borderWidth: 3,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: false,
                cutout: '68%',
                plugins: { legend: { display: false } }
            }
        });

        var $leg = $('#nlpDuyguLegend');
        $leg.empty();
        duygular.forEach(function (d) {
            var pct = ((d.deger / toplam) * 100).toFixed(1);
            $leg.append(
                '<div class="legend-item">' +
                    '<div class="legend-sol">' +
                        '<div class="legend-renk" style="background:' + d.renk + '"></div>' +
                        '<span class="legend-ad">' + d.ad + '</span>' +
                    '</div>' +
                    '<span class="legend-deger">%' + pct + '</span>' +
                '</div>'
            );
        });
    }

    function yukleKelimeBulutu() {
        // TODO: API'den çekilecek
        var kelimeler = [
            { kelime: 'kaliteli',    agirlik: 5, renk: '#4f46e5' },
            { kelime: 'beden uyumu', agirlik: 4, renk: '#10b981' },
            { kelime: 'rahat',       agirlik: 4, renk: '#0ea5e9' },
            { kelime: 'hızlı kargo', agirlik: 3, renk: '#f59e0b' },
            { kelime: 'şık',         agirlik: 3, renk: '#8b5cf6' },
            { kelime: 'küçük geldi', agirlik: 2, renk: '#ef4444' },
            { kelime: 'dayanıklı',   agirlik: 3, renk: '#10b981' },
            { kelime: 'renk güzel',  agirlik: 2, renk: '#0ea5e9' },
            { kelime: 'tavsiye',     agirlik: 4, renk: '#4f46e5' },
            { kelime: 'fiyat/perf.', agirlik: 3, renk: '#f59e0b' },
            { kelime: 'dikişli',     agirlik: 1, renk: '#ef4444' },
            { kelime: 'spor',        agirlik: 3, renk: '#10b981' }
        ];

        var $konteyner = $('#kelimeBulutu');
        $konteyner.empty();

        var boyutlar = [0.72, 0.82, 0.88, 0.98, 1.1];

        kelimeler.forEach(function (k) {
            var fontSize = boyutlar[Math.min(k.agirlik - 1, 4)];
            var opacity = 0.6 + (k.agirlik * 0.08);
            $konteyner.append(
                '<span class="kelime-chip" style="' +
                    'font-size:' + fontSize + 'rem;' +
                    'background:' + k.renk + '1A;' +
                    'color:' + k.renk + ';' +
                    'opacity:' + opacity +
                '">' + k.kelime + '</span>'
            );
        });
    }

    function yukleKategoriMemnuniyetChart() {
        var ctx = document.getElementById('kategoriMemnuniyetChart');
        if (!ctx) return;

        var kategoriler = ['Üst Giyim', 'Alt Giyim', 'Spor', 'Aksesuar', 'Dış Giyim'];
        var skorlar    = [4.3, 3.9, 4.6, 4.1, 3.7];
        var renkler    = skorlar.map(function (s) {
            if (s >= 4.4) return 'rgba(16,185,129,0.8)';
            if (s >= 4.0) return 'rgba(79,70,229,0.7)';
            return 'rgba(245,158,11,0.7)';
        });

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: kategoriler,
                datasets: [{
                    label: 'Ort. Memnuniyet Puanı',
                    data: skorlar,
                    backgroundColor: renkler,
                    borderRadius: 6,
                    borderSkipped: false
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                return '★ ' + c.raw + ' / 5.0';
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        min: 3,
                        max: 5,
                        grid: { color: 'rgba(0,0,0,0.04)' },
                        ticks: {
                            callback: function (v) { return '★' + v; }
                        }
                    },
                    y: { grid: { display: false } }
                }
            }
        });
    }

    // =============================================
    // SEKME 3: MAĞAZA KARŞILAŞTIRMA
    // =============================================
    var magazaKarsilastirmaChart = null;

    function yukleMagazaTab() {
        yukleRadarChart();
        yukleMagazaKarsilastirmaChart('komisyon');
        yukleMagazaSiralamaTablosu();

        $('#magazaMetrikSecim').off('change').on('change', function () {
            yukleMagazaKarsilastirmaChart($(this).val());
        });
    }

    function yukleRadarChart() {
        var ctx = document.getElementById('magazaRadarChart');
        if (!ctx) return;

        new Chart(ctx, {
            type: 'radar',
            data: {
                labels: ['Komisyon', 'Ürün Sayısı', 'Yorum Puanı', 'AI Skoru', 'Aktiflik'],
                datasets: [
                    {
                        label: 'SportZone TR',
                        data: [90, 85, 78, 92, 95],
                        borderColor: '#4f46e5',
                        backgroundColor: 'rgba(79,70,229,0.12)',
                        pointBackgroundColor: '#4f46e5',
                        borderWidth: 2
                    },
                    {
                        label: 'FashionHub',
                        data: [70, 92, 88, 74, 80],
                        borderColor: '#10b981',
                        backgroundColor: 'rgba(16,185,129,0.1)',
                        pointBackgroundColor: '#10b981',
                        borderWidth: 2
                    },
                    {
                        label: 'ActiveWear',
                        data: [60, 68, 82, 65, 70],
                        borderColor: '#f59e0b',
                        backgroundColor: 'rgba(245,158,11,0.08)',
                        pointBackgroundColor: '#f59e0b',
                        borderWidth: 2
                    }
                ]
            },
            options: {
                responsive: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: { font: { size: 11 }, boxWidth: 12, padding: 12 }
                    }
                },
                scales: {
                    r: {
                        beginAtZero: true,
                        max: 100,
                        ticks: { stepSize: 25, font: { size: 10 } },
                        grid: { color: 'rgba(0,0,0,0.06)' },
                        pointLabels: { font: { size: 11 } }
                    }
                }
            }
        });
    }

    function yukleMagazaKarsilastirmaChart(metrik) {
        var ctx = document.getElementById('magazaKarsilastirmaChart');
        if (!ctx) return;
        if (magazaKarsilastirmaChart) magazaKarsilastirmaChart.destroy();

        var veriler = {
            komisyon: {
                etiketler: ['SportZone TR', 'FashionHub', 'ActiveWear', 'FitStyle', 'RunnerShop'],
                veriler:   [82400, 61200, 48700, 39600, 31800],
                birim: '₺',
                renk: '#4f46e5'
            },
            urun: {
                etiketler: ['SportZone TR', 'FashionHub', 'ActiveWear', 'FitStyle', 'RunnerShop'],
                veriler:   [1840, 2210, 980, 760, 620],
                birim: '',
                renk: '#f59e0b'
            },
            yorum: {
                etiketler: ['SportZone TR', 'FashionHub', 'ActiveWear', 'FitStyle', 'RunnerShop'],
                veriler:   [3240, 4180, 1820, 1390, 980],
                birim: '',
                renk: '#10b981'
            }
        };

        var d = veriler[metrik];

        magazaKarsilastirmaChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: d.etiketler,
                datasets: [{
                    label: $('#magazaMetrikSecim option:selected').text(),
                    data: d.veriler,
                    backgroundColor: d.renk + '33',
                    borderColor: d.renk,
                    borderWidth: 2,
                    borderRadius: 6,
                    borderSkipped: false
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                return d.birim + c.raw.toLocaleString('tr-TR');
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(0,0,0,0.04)' },
                        ticks: {
                            callback: function (v) {
                                return d.birim ? d.birim + (v / 1000).toFixed(0) + 'K' : v;
                            }
                        }
                    },
                    x: { grid: { display: false } }
                }
            }
        });
    }

    function yukleMagazaSiralamaTablosu() {
        // TODO: API'den çekilecek
        var magazalar = [
            { sira: 1, ad: 'SportZone TR', paket: 'Premium',  komisyon: '₺82.400', urun: 1840, yorum: 3240, memnuniyet: 4.6, puan: 92 },
            { sira: 2, ad: 'FashionHub',   paket: 'Standart', komisyon: '₺61.200', urun: 2210, yorum: 4180, memnuniyet: 4.2, puan: 80 },
            { sira: 3, ad: 'ActiveWear',   paket: 'Premium',  komisyon: '₺48.700', urun:  980, yorum: 1820, memnuniyet: 4.4, puan: 74 },
            { sira: 4, ad: 'FitStyle',     paket: 'Standart', komisyon: '₺39.600', urun:  760, yorum: 1390, memnuniyet: 3.9, puan: 62 },
            { sira: 5, ad: 'RunnerShop',   paket: 'Başlangıç',komisyon: '₺31.800', urun:  620, yorum:  980, memnuniyet: 4.1, puan: 55 }
        ];

        var $tbody = $('#magazaSiralamaTablosu');
        $tbody.empty();

        magazalar.forEach(function (m) {
            var siraRozetSinif = m.sira <= 3 ? 'sira-' + m.sira : 'sira-diger';
            var paketBadge = m.paket === 'Premium'
                ? '<span class="badge bg-warning-soft text-warning"><i class="fas fa-crown me-1" style="font-size:.6rem"></i>' + m.paket + '</span>'
                : m.paket === 'Standart'
                ? '<span class="badge bg-primary-soft text-primary">' + m.paket + '</span>'
                : '<span class="badge bg-secondary-soft text-secondary">' + m.paket + '</span>';

            $tbody.append(
                '<tr>' +
                    '<td><span class="sira-rozeti ' + siraRozetSinif + '">' + m.sira + '</span></td>' +
                    '<td><strong>' + m.ad + '</strong></td>' +
                    '<td>' + paketBadge + '</td>' +
                    '<td><strong>' + m.komisyon + '</strong></td>' +
                    '<td>' + m.urun.toLocaleString('tr-TR') + '</td>' +
                    '<td>' + m.yorum.toLocaleString('tr-TR') + '</td>' +
                    '<td><span class="text-warning">★</span> ' + m.memnuniyet.toFixed(1) + '</td>' +
                    '<td>' +
                        '<div class="performans-bar">' +
                            '<div class="performans-bar-dolu" style="width:' + m.puan + '%"></div>' +
                        '</div>' +
                    '</td>' +
                '</tr>'
            );
        });
    }

    // =============================================
    // SEKME 4: AKTİVİTE HARİTASI
    // =============================================
    function yukleAktiviteTab() {
        yukleHeatmap();
        yukleSaatlikAktiviteChart();
        yukleGunlukAktiviteChart();
    }

    function yukleHeatmap() {
        var gunler = ['Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'];
        var saatler = ['08', '10', '12', '14', '16', '18', '20', '22'];

        // Heatmap verisi [saat][gun] = yoğunluk 0-100
        var veri = [
            [10, 15, 20, 18, 22, 8,  5],
            [25, 40, 52, 48, 55, 18, 12],
            [60, 72, 85, 80, 78, 35, 22],
            [75, 88, 95, 92, 90, 48, 30],
            [82, 90, 88, 85, 80, 55, 38],
            [65, 70, 72, 68, 65, 42, 28],
            [45, 50, 55, 52, 48, 30, 18],
            [20, 25, 28, 26, 22, 15, 8 ]
        ];

        var $konteyner = $('#heatmapKonteyner');
        $konteyner.empty();

        saatler.forEach(function (saat, si) {
            var $satir = $('<div class="heatmap-satir"></div>');
            $satir.append('<span class="heatmap-saat-etiket">' + saat + ':00</span>');

            gunler.forEach(function (gun, gi) {
                var yogunluk = veri[si][gi];
                var alfa = 0.08 + (yogunluk / 100) * 0.92;
                var hucre = $('<div class="heatmap-hucre"></div>');
                hucre.css('background', 'rgba(79,70,229,' + alfa.toFixed(2) + ')');
                hucre.attr('title', gun + ' ' + saat + ':00 — ' + yogunluk + ' sorgu');
                $satir.append(hucre);
            });

            $konteyner.append($satir);
        });

        // Gün etiketleri
        var $gunEtiketleri = $('#heatmapGunEtiketleri');
        $gunEtiketleri.empty();
        gunler.forEach(function (gun) {
            $gunEtiketleri.append('<span class="heatmap-gun-ad">' + gun + '</span>');
        });
    }

    function yukleSaatlikAktiviteChart() {
        var ctx = document.getElementById('saatlikAktiviteChart');
        if (!ctx) return;

        var saatler = ['08','09','10','11','12','13','14','15','16','17','18','19','20','21','22'];
        var veriler  = [120,280,520,680,740,820,890,850,810,760,640,480,320,180,90];

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: saatler.map(function (s) { return s + ':00'; }),
                datasets: [{
                    label: 'Widget Sorgu',
                    data: veriler,
                    backgroundColor: veriler.map(function (v) {
                        var alfa = 0.15 + (v / 890) * 0.65;
                        return 'rgba(14,165,233,' + alfa.toFixed(2) + ')';
                    }),
                    borderColor: '#0ea5e9',
                    borderWidth: 1.5,
                    borderRadius: 4,
                    borderSkipped: false
                }]
            },
            options: {
                responsive: true,
                plugins: { legend: { display: false } },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(0,0,0,0.04)' }
                    },
                    x: { grid: { display: false } }
                }
            }
        });
    }

    function yukleGunlukAktiviteChart() {
        var ctx = document.getElementById('gunlukAktiviteChart');
        if (!ctx) return;

        var gunler  = ['Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi', 'Pazar'];
        var veriler = [4820, 5340, 5980, 5620, 5180, 3240, 2180];
        var renkler = veriler.map(function (v, i) {
            return i < 5
                ? 'rgba(16,185,129,0.7)'
                : 'rgba(107,114,128,0.5)';
        });

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: gunler,
                datasets: [{
                    label: 'Günlük Sorgu',
                    data: veriler,
                    backgroundColor: renkler,
                    borderRadius: 5,
                    borderSkipped: false
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function (c) {
                                return c.raw.toLocaleString('tr-TR') + ' sorgu';
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(0,0,0,0.04)' },
                        ticks: {
                            callback: function (v) {
                                return (v / 1000).toFixed(0) + 'K';
                            }
                        }
                    },
                    x: { grid: { display: false } }
                }
            }
        });
    }

    // =============================================
    // Başlangıç - ilk tab yükle
    // =============================================
    grafiklerYuklendi['gelir'] = true;
    yukleGelirTab();

});