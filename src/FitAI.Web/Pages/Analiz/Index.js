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
// =============================================
// 16. Gün Eklentileri
// =============================================

// Ürün seçici değişikliği
$('#urunSecici').on('change', function() {
    var secilenUrun = $(this).val();
    var secilenUrunAd = $(this).find('option:selected').text();
    
    abp.message.info('"' + secilenUrunAd + '" için analiz yükleniyor...', 'Ürün Değişti');
    
    // Tüm grafikleri yeniden yükle
    grafiklerYuklendi = {};
    tabGrafikleriniYukle(aktifTab);
    grafiklerYuklendi[aktifTab] = true;
});

// AI Skoru Gauge Chart (gelir tabına eklenebilir)
function yukleAiSkorGauge(skor) {
    var canvas = document.getElementById('aiSkorGauge');
    if (!canvas) return;
    
    var ctx = canvas.getContext('2d');
    var width = canvas.width;
    var height = canvas.height;
    var centerX = width / 2;
    var centerY = height / 2;
    var radius = width * 0.4;
    
    // Gauge'u temizle
    ctx.clearRect(0, 0, width, height);
    
    // Dairesel background (gri)
    ctx.beginPath();
    ctx.arc(centerX, centerY, radius, 0, Math.PI * 2);
    ctx.strokeStyle = '#e5e7eb';
    ctx.lineWidth = 10;
    ctx.stroke();
    
    // Skor yüzdesine göre progress arc
    var startAngle = -Math.PI / 2;
    var endAngle = startAngle + (Math.PI * 2 * (skor / 100));
    
    ctx.beginPath();
    ctx.arc(centerX, centerY, radius, startAngle, endAngle);
    ctx.strokeStyle = skor >= 70 ? '#10b981' : (skor >= 40 ? '#f59e0b' : '#ef4444');
    ctx.lineWidth = 10;
    ctx.stroke();
    
    // İç merkezde skor yazısı
    ctx.font = 'bold 18px "Inter", sans-serif';
    ctx.fillStyle = '#1a1a2e';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(skor + '', centerX, centerY);
}

// Ürün bazlı NLP analizi gösterme (seçilen ürüne göre)
function urunBazliNlpGuncelle(urunId) {
    if (!urunId || urunId === 'all') return;
    
    // Simüle edilmiş ürün bazlı veri
    var urunVerileri = {
        1: { pozitif: 342, negatif: 48, notr: 86, skor: 86, kelimeler: ['rahat', 'dayanıklı', 'kaliteli'] },
        2: { pozitif: 218, negatif: 32, notr: 54, skor: 79, kelimeler: ['yumuşak', 'rahat', 'kaymaz'] },
        3: { pozitif: 156, negatif: 24, notr: 38, skor: 82, kelimeler: ['dayanıklı', 'terletmiyor', 'iyi'] },
        4: { pozitif: 98,  negatif: 12, notr: 24, skor: 85, kelimeler: ['şık', 'pratik', 'sağlam'] }
    };
    
    var data = urunVerileri[urunId];
    if (data) {
        $('#nlp_pozitif').text(data.pozitif.toLocaleString('tr-TR'));
        $('#nlp_negatif').text(data.negatif.toLocaleString('tr-TR'));
        $('#nlp_notr').text(data.notr.toLocaleString('tr-TR'));
        $('#nlp_toplam').text((data.pozitif + data.negatif + data.notr).toLocaleString('tr-TR'));
        
        // Kelime bulutunu güncelle
        var $konteyner = $('#kelimeBulutu');
        $konteyner.empty();
        data.kelimeler.forEach(function(kelime, index) {
            var boyut = [0.85, 0.95, 1.1][index % 3];
            $konteyner.append(
                '<span class="kelime-chip" style="font-size:' + boyut + 'rem; background:#4f46e51A; color:#4f46e5;">' + kelime + '</span>'
            );
        });
    }
}

// Ürün seçici değiştiğinde NLP verilerini güncelle (eğer NLP tab'ı aktifse)
$('#urunSecici').on('change', function() {
    if (aktifTab === 'nlp') {
        urunBazliNlpGuncelle($(this).val());
    }
});
// =============================================
// 17. Gün - Analiz Sayfası Geliştirmeleri
// =============================================

// Ürün seçici değiştiğinde detay kartını güncelle
$('#urunSecici').on('change', function() {
    var urunId = $(this).val();
    var urunAd = $(this).find('option:selected').text();
    
    if (urunId !== 'all') {
        $('#urunDetayKarti').show();
        $('#urunAdi').text(urunAd);
        
        // Simüle edilmiş ürün verileri
        var urunVerileri = {
            1: { kategori: 'Spor', stok: 1240, fiyat: '₺ 1,299', skor: 86, durum: 'Stokta Var' },
            2: { kategori: 'Fitness', stok: 580, fiyat: '₺ 899', skor: 79, durum: 'Stokta Var' },
            3: { kategori: 'Aksesuar', stok: 2100, fiyat: '₺ 349', skor: 82, durum: 'Stokta Var' },
            4: { kategori: 'Aksesuar', stok: 95, fiyat: '₺ 129', skor: 85, durum: 'Son 5 Ürün!' }
        };
        
        var data = urunVerileri[urunId];
        if (data) {
            $('#urunKategori').text(data.kategori);
            $('#urunStok').text(data.stok.toLocaleString('tr-TR'));
            $('#urunFiyat').text(data.fiyat);
            $('#aiPerformansSkoru').text(data.skor);
            $('#aiSkorProgress').css('width', data.skor + '%');
            $('#urunStokDurumu').text(data.durum);
            if (data.durum.includes('Son')) {
                $('#urunStokDurumu').removeClass('bg-success-soft').addClass('bg-warning-soft text-warning');
            } else {
                $('#urunStokDurumu').removeClass('bg-warning-soft').addClass('bg-success-soft text-success');
            }
        }
    } else {
        $('#urunDetayKarti').hide();
    }
});

// Başlangıçta ürün detayını göster (ilk seçili ürün "Premium Spor Ayakkabı")
setTimeout(function() {
    $('#urunSecici').trigger('change');
}, 100);

// Skeleton loader gösterme fonksiyonu
function showSkeleton($element) {
    var originalHeight = $element.height();
    $element.addClass('skeleton-loader');
    $element.css('min-height', originalHeight + 'px');
}

function hideSkeleton($element) {
    $element.removeClass('skeleton-loader');
    $element.css('min-height', '');
}

// Tam ekran grafik modu
$('.fullscreen-chart-btn').on('click', function() {
    var targetCanvasId = $(this).data('target');
    if (!targetCanvasId) return;
    
    var $canvas = $('#' + targetCanvasId);
    var canvas = $canvas[0];
    if (!canvas) return;
    
    // Modal içinde canvas'ı büyüt
    var modal = $('<div class="chart-fullscreen-modal"></div>');
    var $cloneCanvas = $canvas.clone();
    $cloneCanvas.attr('width', $canvas.width());
    $cloneCanvas.attr('height', $canvas.height());
    
    modal.append($cloneCanvas);
    $('body').append(modal);
    
    modal.on('click', function() {
        modal.remove();
    });
    
    // ESC tuşu ile kapatma
    $(document).on('keyup.chartFullscreen', function(e) {
        if (e.key === 'Escape') {
            modal.remove();
            $(document).off('keyup.chartFullscreen');
        }
    });
});

// PDF Dışa Aktar (simülasyon)
$('#exportPdfBtn').on('click', function() {
    abp.message.info('PDF raporu hazırlanıyor ve indirilecek...', 'Rapor Dışa Aktar');
    // Gerçek implementasyonda html2canvas + jsPDF kullanılabilir
});

// Excel Dışa Aktar (simülasyon)
$('#exportExcelBtn').on('click', function() {
    abp.message.info('Excel raporu hazırlanıyor...', 'Rapor Dışa Aktar');
});

// Rapor Paylaş (simülasyon)
$('#shareReportBtn').on('click', function() {
    abp.message.info('Rapor paylaşım linki oluşturuluyor...', 'Paylaş');
});

// Sekme geçişlerinde animasyon iyileştirmesi
var originalTabGrafikleriniYukle = tabGrafikleriniYukle;
window.tabGrafikleriniYukle = function(tab) {
    // Skeleton göster
    var $aktifTab = $('#tab-' + tab);
    $aktifTab.find('.card-body').each(function() {
        if ($(this).find('canvas').length > 0) {
            showSkeleton($(this));
        }
    });
    
    // Orijinal fonksiyonu çağır
    originalTabGrafikleriniYukle(tab);
    
    // 1 saniye sonra skeleton'u kaldır
    setTimeout(function() {
        $aktifTab.find('.card-body').each(function() {
            hideSkeleton($(this));
        });
    }, 800);
};

// Tooltip desteği (varsayılan olarak ekle)
$('[data-tooltip]').each(function() {
    // data-tooltip attribute otomatik tooltip sağlar
});

// Sayfa yüklendiğinde ürün seçici tetikle
$(document).ready(function() {
    // Ürün detay kartını başlat
    if ($('#urunSecici').val() !== 'all') {
        $('#urunDetayKarti').show();
    }
    
    // Tooltip'leri başlat
    if ($.fn.tooltip) {
        $('[data-bs-toggle="tooltip"]').tooltip();
    }
});
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

    // Dönem Filtresi
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

        var etiketler = ['1 May','3 May','5 May','7 May','9 May','11 May','13 May','15 May','17 May','19 May','21 May','23 May','25 May','27 May','29 May','31 May'];
        var veriler = [8200,9400,7800,11200,10500,13800,12400,15600,14200,16800,15900,18200,17400,19800,18600,21200];

        gelirZamanChart = new Chart(ctx, {
            type: tip,
            data: {
                labels: etiketler,
                datasets: [{
                    label: 'Komisyon Geliri (₺)',
                    data: veriler,
                    borderColor: '#4f46e5',
                    backgroundColor: tip === 'line' ? 'rgba(79,70,229,0.07)' : 'rgba(79,70,229,0.18)',
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
                    tooltip: { callbacks: { label: function (c) { return '₺ ' + c.raw.toLocaleString('tr-TR'); } } }
                },
                scales: {
                    y: { beginAtZero: false, grid: { color: 'rgba(0,0,0,0.04)' }, ticks: { callback: function (v) { return '₺' + (v / 1000).toFixed(0) + 'K'; } } },
                    x: { grid: { display: false } }
                }
            }
        });
    }

    window.gelirGrafikiDegistir = function (tip, btn) {
        $('.card-header .btn-xs').removeClass('active');
        $(btn).addClass('active');
        yukleGelirZamanChart(tip);
    };

    function yukleGelirPaketChart() {
        var ctx = document.getElementById('gelirPaketChart');
        if (!ctx) return;
        if (gelirPaketChart) gelirPaketChart.destroy();

        var paketler = [{ ad: 'Premium', deger: 198400, renk: '#f59e0b' }, { ad: 'Standart', deger: 152600, renk: '#4f46e5' }, { ad: 'Başlangıç', deger: 61800, renk: '#10b981' }];
        var toplam = paketler.reduce(function (t, p) { return t + p.deger; }, 0);

        $('#gelirPaketToplam').text((toplam / 1000).toFixed(0) + 'K');

        gelirPaketChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: paketler.map(function (p) { return p.ad; }),
                datasets: [{ data: paketler.map(function (p) { return p.deger; }), backgroundColor: paketler.map(function (p) { return p.renk; }), borderWidth: 3, borderColor: '#fff' }]
            },
            options: { responsive: false, cutout: '68%', plugins: { legend: { display: false }, tooltip: { callbacks: { label: function (c) { var pct = ((c.raw / toplam) * 100).toFixed(1); return c.label + ': ₺' + c.raw.toLocaleString('tr-TR') + ' (%' + pct + ')'; } } } } }
        });

        var $leg = $('#gelirPaketLegend');
        $leg.empty();
        paketler.forEach(function (p) {
            var pct = ((p.deger / toplam) * 100).toFixed(1);
            $leg.append('<div class="legend-item"><div class="legend-sol"><div class="legend-renk" style="background:' + p.renk + '"></div><span class="legend-ad">' + p.ad + '</span></div><span class="legend-deger">%' + pct + '</span></div>');
        });
    }

    // =============================================
    // SEKME 2: NLP & DUYGU (18. Gün ile geliştirildi)
    // =============================================
    var yorumVerileri = [
        { id: 1, kullanici: 'Ayşe Y.', tarih: '2 gün önce', duygu: 'pozitif', metin: 'Ürün gerçekten çok kaliteli, beklediğimden daha iyi çıktı. Kesinlikle tavsiye ederim!', anahtarKelimeler: ['kaliteli', 'tavsiye'] },
        { id: 2, kullanici: 'Mehmet D.', tarih: '3 gün önce', duygu: 'negatif', metin: 'Beden uyumu sorunlu. Normalde L giyerim ama bu ürünün L\'si çok küçük geldi.', anahtarKelimeler: ['beden sorunu', 'küçük'] },
        { id: 3, kullanici: 'Zeynep K.', tarih: '4 gün önce', duygu: 'pozitif', metin: 'Kargo çok hızlıydı, ürün de görseldeki gibi. Tekrar alacağım.', anahtarKelimeler: ['hızlı kargo', 'kaliteli'] },
        { id: 4, kullanici: 'Ali R.', tarih: '5 gün önce', duygu: 'notr', metin: 'Ürün fiyatına göre orta seviyede. İdare eder.', anahtarKelimeler: ['fiyat/performans', 'orta'] },
        { id: 5, kullanici: 'Elif T.', tarih: '1 hafta önce', duygu: 'pozitif', metin: 'Harika bir ürün! Arkadaşıma da aldım, o da çok memnun kaldı.', anahtarKelimeler: ['harika', 'memnun'] },
        { id: 6, kullanici: 'Can A.', tarih: '1 hafta önce', duygu: 'negatif', metin: 'Ürün beklediğim gibi değil. İade süreci de biraz karmaşık.', anahtarKelimeler: ['hayal kırıklığı', 'iade'] },
        { id: 7, kullanici: 'Selin Ö.', tarih: '1 hafta önce', duygu: 'pozitif', metin: 'Çok şık ve rahat. Günlük kullanım için ideal.', anahtarKelimeler: ['şık', 'rahat'] },
        { id: 8, kullanici: 'Burak K.', tarih: '2 hafta önce', duygu: 'notr', metin: 'Normal bir ürün. Beklentimi karşıladı ama şaşırtmadı.', anahtarKelimeler: ['normal', 'beklenti'] }
    ];

    var gosterilecekYorumSayisi = 5;
    var aktifDuyguFiltre = 'all';

    function yorumListesiniRender(duyguFiltre, limit) {
        var filtrelenmisYorumlar = yorumVerileri;
        if (duyguFiltre !== 'all') filtrelenmisYorumlar = yorumVerileri.filter(function(y) { return y.duygu === duyguFiltre; });
        
        var gosterilecekYorumlar = filtrelenmisYorumlar.slice(0, limit);
        var $container = $('#yorumListesi');
        
        if (gosterilecekYorumlar.length === 0) { $('#yorumListesi').hide(); $('#yorumBos').show(); return; }
        
        $('#yorumListesi').show();
        $('#yorumBos').hide();
        $container.empty();
        
        gosterilecekYorumlar.forEach(function(yorum) {
            var duyguClass = yorum.duygu === 'pozitif' ? 'bg-success-soft text-success' : (yorum.duygu === 'negatif' ? 'bg-danger-soft text-danger' : 'bg-secondary-soft text-secondary');
            var duyguIcon = yorum.duygu === 'pozitif' ? 'fa-smile' : (yorum.duygu === 'negatif' ? 'fa-frown' : 'fa-meh');
            var duyguBadgeClass = yorum.duygu === 'pozitif' ? 'bg-success-soft text-success' : (yorum.duygu === 'negatif' ? 'bg-danger-soft text-danger' : 'bg-secondary-soft text-secondary');
            var duyguText = yorum.duygu === 'pozitif' ? 'Pozitif' : (yorum.duygu === 'negatif' ? 'Negatif' : 'Nötr');
            
            var kelimeBadges = '';
            yorum.anahtarKelimeler.forEach(function(kelime) { kelimeBadges += '<span class="badge bg-light me-1">' + kelime + '</span>'; });
            
            $container.append('<div class="yorum-item yorum-' + yorum.duygu + '"><div class="yorum-avatar ' + duyguClass + '"><i class="fas ' + duyguIcon + '"></i></div><div class="yorum-icerik"><div class="yorum-ust"><span class="yorum-kullanici">' + yorum.kullanici + '</span><span class="yorum-tarih text-muted small">' + yorum.tarih + '</span><span class="badge ' + duyguBadgeClass + ' ms-2">' + duyguText + '</span></div><div class="yorum-metin">' + yorum.metin + '</div><div class="yorum-analiz mt-1"><span class="small text-muted"><i class="fas fa-brain me-1"></i>Anahtar kelimeler: ' + kelimeBadges + '</span></div></div></div>');
        });
        
        if (filtrelenmisYorumlar.length > limit) $('#dahaFazlaYorumBtn').show();
        else $('#dahaFazlaYorumBtn').hide();
    }

    function nlpOzetiniGuncelle(urunId) {
        var ozetVerileri = {
            all: { trendler: ['Kalite memnuniyeti yüksek', 'Beden uyumu sorunları var', 'Kargo hızı beğeniliyor', 'Renk seçenekleri yetersiz'], oneri: 'Beden tablosunu güncelleyin ve renk seçeneklerini artırın.' },
            1: { trendler: ['Ayakkabı kalitesi çok beğeniliyor', 'Konfor ve rahatlık öne çıkıyor', 'Beden uyumunda sorunlar var', 'Fiyat performans olumlu'], oneri: 'Beden tablosunu detaylandırın ve farklı renk seçenekleri ekleyin.' },
            2: { trendler: ['Matın kalınlığı beğeniliyor', 'Kaymaz yüzey memnun ediyor', 'Renk seçenekleri yeterli'], oneri: 'Farklı ebat seçenekleri ekleyerek ürün gamını genişletin.' },
            3: { trendler: ['Eldiven dayanıklılığı iyi', 'Terletmiyor yorumları öne çıkıyor', 'Beden seçiminde kararsızlık var'], oneri: 'Detaylı beden ölçü tablosu ekleyin ve video inceleme koyun.' },
            4: { trendler: ['Şık tasarım beğeniliyor', 'Pratik kullanım öne çıkıyor', 'Fiyat biraz yüksek bulunuyor'], oneri: 'Farklı renk ve desen seçenekleri ile ürünü çeşitlendirin.' }
        };
        var data = ozetVerileri[urunId] || ozetVerileri.all;
        var $liste = $('#ozetListesi');
        $liste.empty();
        data.trendler.forEach(function(trend) { $liste.append('<li><i class="fas fa-chart-line text-primary me-2"></i>' + trend + '</li>'); });
        $('.ozet-oneri span').text('AI Önerisi: ' + data.oneri);
    }

    function yukleNlpTab() {
        $('#nlp_toplam').text('14.820');
        $('#nlp_pozitif').text('10.078');
        $('#nlp_negatif').text('2.371');
        $('#nlp_notr').text('2.371');
        yukleNlpTrendChart();
        yukleNlpDuyguChart();
        yukleKelimeBulutu();
        yukleKategoriMemnuniyetChart();
        yorumListesiniRender('all', 5);
        nlpOzetiniGuncelle($('#urunSecici').val());
    }

    function yukleNlpTrendChart() {
        var ctx = document.getElementById('nlpTrendChart');
        if (!ctx) return;
        new Chart(ctx, {
            type: 'line',
            data: { labels: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran'], datasets: [
                { label: 'Pozitif', data: [1200, 1450, 1380, 1620, 1890, 1780], borderColor: '#10b981', backgroundColor: 'rgba(16,185,129,0.08)', fill: true, tension: 0.4, borderWidth: 2, pointRadius: 3 },
                { label: 'Negatif', data: [420, 380, 450, 310, 290, 340], borderColor: '#ef4444', backgroundColor: 'rgba(239,68,68,0.06)', fill: true, tension: 0.4, borderWidth: 2, pointRadius: 3 },
                { label: 'Nötr', data: [380, 320, 410, 380, 420, 390], borderColor: '#6b7280', backgroundColor: 'rgba(107,114,128,0.05)', fill: true, tension: 0.4, borderWidth: 2, pointRadius: 3, borderDash: [4, 3] }
            ] },
            options: { responsive: true, plugins: { legend: { position: 'top', labels: { font: { size: 11 }, boxWidth: 12 } } }, scales: { y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.04)' } }, x: { grid: { display: false } } } }
        });
    }

    function yukleNlpDuyguChart() {
        var ctx = document.getElementById('nlpDuyguChart');
        if (!ctx) return;
        var duygular = [{ ad: 'Pozitif', deger: 10078, renk: '#10b981' }, { ad: 'Negatif', deger: 2371, renk: '#ef4444' }, { ad: 'Nötr', deger: 2371, renk: '#9ca3af' }];
        var toplam = duygular.reduce(function (t, d) { return t + d.deger; }, 0);
        new Chart(ctx, { type: 'doughnut', data: { labels: duygular.map(function (d) { return d.ad; }), datasets: [{ data: duygular.map(function (d) { return d.deger; }), backgroundColor: duygular.map(function (d) { return d.renk; }), borderWidth: 3, borderColor: '#fff' }] }, options: { responsive: false, cutout: '68%', plugins: { legend: { display: false } } } });
        var $leg = $('#nlpDuyguLegend');
        $leg.empty();
        duygular.forEach(function (d) { var pct = ((d.deger / toplam) * 100).toFixed(1); $leg.append('<div class="legend-item"><div class="legend-sol"><div class="legend-renk" style="background:' + d.renk + '"></div><span class="legend-ad">' + d.ad + '</span></div><span class="legend-deger">%' + pct + '</span></div>'); });
    }

    function yukleKelimeBulutu() {
        var kelimeler = [{ kelime: 'kaliteli', renk: '#4f46e5' }, { kelime: 'beden uyumu', renk: '#10b981' }, { kelime: 'rahat', renk: '#0ea5e9' }, { kelime: 'hızlı kargo', renk: '#f59e0b' }, { kelime: 'şık', renk: '#8b5cf6' }, { kelime: 'küçük geldi', renk: '#ef4444' }, { kelime: 'dayanıklı', renk: '#10b981' }, { kelime: 'renk güzel', renk: '#0ea5e9' }, { kelime: 'tavsiye', renk: '#4f46e5' }, { kelime: 'fiyat/perf.', renk: '#f59e0b' }, { kelime: 'spor', renk: '#10b981' }];
        var $konteyner = $('#kelimeBulutu');
        $konteyner.empty();
        kelimeler.forEach(function (k) { $konteyner.append('<span class="kelime-chip" style="background:' + k.renk + '1A; color:' + k.renk + ';">' + k.kelime + '</span>'); });
    }

    function yukleKategoriMemnuniyetChart() {
        var ctx = document.getElementById('kategoriMemnuniyetChart');
        if (!ctx) return;
        new Chart(ctx, { type: 'bar', data: { labels: ['Üst Giyim', 'Alt Giyim', 'Spor', 'Aksesuar', 'Dış Giyim'], datasets: [{ label: 'Ort. Memnuniyet Puanı', data: [4.3, 3.9, 4.6, 4.1, 3.7], backgroundColor: ['rgba(16,185,129,0.8)', 'rgba(79,70,229,0.7)', 'rgba(16,185,129,0.8)', 'rgba(79,70,229,0.7)', 'rgba(245,158,11,0.7)'], borderRadius: 6 }] }, options: { indexAxis: 'y', responsive: true, plugins: { legend: { display: false }, tooltip: { callbacks: { label: function (c) { return '★ ' + c.raw + ' / 5.0'; } } } }, scales: { x: { min: 3, max: 5, grid: { color: 'rgba(0,0,0,0.04)' }, ticks: { callback: function (v) { return '★' + v; } } }, y: { grid: { display: false } } } } });
    }

    $('#duyguFiltreBtnGrubu .btn').on('click', function() {
        $('#duyguFiltreBtnGrubu .btn').removeClass('active');
        $(this).addClass('active');
        aktifDuyguFiltre = $(this).data('duygu');
        gosterilecekYorumSayisi = 5;
        yorumListesiniRender(aktifDuyguFiltre, gosterilecekYorumSayisi);
    });

    $('#dahaFazlaYorumBtn').on('click', function() { gosterilecekYorumSayisi += 5; yorumListesiniRender(aktifDuyguFiltre, gosterilecekYorumSayisi); });
    $(document).on('click', '.yorum-item', function() { var metin = $(this).find('.yorum-metin').text(); var kullanici = $(this).find('.yorum-kullanici').text(); abp.message.info('"' + metin + '"', kullanici + ' - Yorum Detayı'); });

    // =============================================
    // SEKME 3: MAĞAZA KARŞILAŞTIRMA
    // =============================================
    var magazaKarsilastirmaChart = null;

    function yukleMagazaTab() {
        yukleRadarChart();
        yukleMagazaKarsilastirmaChart('komisyon');
        yukleMagazaSiralamaTablosu();
        $('#magazaMetrikSecim').off('change').on('change', function () { yukleMagazaKarsilastirmaChart($(this).val()); });
    }

    function yukleRadarChart() {
        var ctx = document.getElementById('magazaRadarChart');
        if (!ctx) return;
        new Chart(ctx, { type: 'radar', data: { labels: ['Komisyon', 'Ürün Sayısı', 'Yorum Puanı', 'AI Skoru', 'Aktiflik'], datasets: [{ label: 'SportZone TR', data: [90, 85, 78, 92, 95], borderColor: '#4f46e5', backgroundColor: 'rgba(79,70,229,0.12)', pointBackgroundColor: '#4f46e5', borderWidth: 2 }, { label: 'FashionHub', data: [70, 92, 88, 74, 80], borderColor: '#10b981', backgroundColor: 'rgba(16,185,129,0.1)', pointBackgroundColor: '#10b981', borderWidth: 2 }, { label: 'ActiveWear', data: [60, 68, 82, 65, 70], borderColor: '#f59e0b', backgroundColor: 'rgba(245,158,11,0.08)', pointBackgroundColor: '#f59e0b', borderWidth: 2 }] }, options: { responsive: false, plugins: { legend: { position: 'bottom', labels: { font: { size: 11 }, boxWidth: 12, padding: 12 } } }, scales: { r: { beginAtZero: true, max: 100, ticks: { stepSize: 25, font: { size: 10 } }, grid: { color: 'rgba(0,0,0,0.06)' }, pointLabels: { font: { size: 11 } } } } } });
    }

    function yukleMagazaKarsilastirmaChart(metrik) {
        var ctx = document.getElementById('magazaKarsilastirmaChart');
        if (!ctx) return;
        if (magazaKarsilastirmaChart) magazaKarsilastirmaChart.destroy();
        var veriler = { komisyon: { etiketler: ['SportZone TR', 'FashionHub', 'ActiveWear', 'FitStyle', 'RunnerShop'], veriler: [82400, 61200, 48700, 39600, 31800], birim: '₺', renk: '#4f46e5' }, urun: { etiketler: ['SportZone TR', 'FashionHub', 'ActiveWear', 'FitStyle', 'RunnerShop'], veriler: [1840, 2210, 980, 760, 620], birim: '', renk: '#f59e0b' }, yorum: { etiketler: ['SportZone TR', 'FashionHub', 'ActiveWear', 'FitStyle', 'RunnerShop'], veriler: [3240, 4180, 1820, 1390, 980], birim: '', renk: '#10b981' } };
        var d = veriler[metrik];
        magazaKarsilastirmaChart = new Chart(ctx, { type: 'bar', data: { labels: d.etiketler, datasets: [{ label: $('#magazaMetrikSecim option:selected').text(), data: d.veriler, backgroundColor: d.renk + '33', borderColor: d.renk, borderWidth: 2, borderRadius: 6 }] }, options: { responsive: true, plugins: { legend: { display: false }, tooltip: { callbacks: { label: function (c) { return d.birim + c.raw.toLocaleString('tr-TR'); } } } }, scales: { y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.04)' }, ticks: { callback: function (v) { return d.birim ? d.birim + (v / 1000).toFixed(0) + 'K' : v; } } }, x: { grid: { display: false } } } } });
    }

    function yukleMagazaSiralamaTablosu() {
        var magazalar = [{ sira: 1, ad: 'SportZone TR', paket: 'Premium', komisyon: '₺82.400', urun: 1840, yorum: 3240, memnuniyet: 4.6, puan: 92 }, { sira: 2, ad: 'FashionHub', paket: 'Standart', komisyon: '₺61.200', urun: 2210, yorum: 4180, memnuniyet: 4.2, puan: 80 }, { sira: 3, ad: 'ActiveWear', paket: 'Premium', komisyon: '₺48.700', urun: 980, yorum: 1820, memnuniyet: 4.4, puan: 74 }, { sira: 4, ad: 'FitStyle', paket: 'Standart', komisyon: '₺39.600', urun: 760, yorum: 1390, memnuniyet: 3.9, puan: 62 }, { sira: 5, ad: 'RunnerShop', paket: 'Başlangıç', komisyon: '₺31.800', urun: 620, yorum: 980, memnuniyet: 4.1, puan: 55 }];
        var $tbody = $('#magazaSiralamaTablosu');
        $tbody.empty();
        magazalar.forEach(function (m) {
            var siraSinif = m.sira === 1 ? 'sira-1' : (m.sira === 2 ? 'sira-2' : (m.sira === 3 ? 'sira-3' : 'sira-diger'));
            var paketBadge = m.paket === 'Premium' ? '<span class="badge bg-warning-soft text-warning"><i class="fas fa-crown me-1" style="font-size:.6rem"></i>' + m.paket + '</span>' : (m.paket === 'Standart' ? '<span class="badge bg-primary-soft text-primary">' + m.paket + '</span>' : '<span class="badge bg-secondary-soft text-secondary">' + m.paket + '</span>');
            $tbody.append('<tr><td><span class="sira-rozeti ' + siraSinif + '">' + m.sira + '</span></td><td><strong>' + m.ad + '</strong></td><td>' + paketBadge + '</td><td><strong>' + m.komisyon + '</strong></td><td>' + m.urun.toLocaleString('tr-TR') + '</td><td>' + m.yorum.toLocaleString('tr-TR') + '</td><td><span class="text-warning">★</span> ' + m.memnuniyet.toFixed(1) + '</td><td><div class="performans-bar"><div class="performans-bar-dolu" style="width:' + m.puan + '%"></div></div></td></tr>');
        });
    }

    // =============================================
    // SEKME 4: AKTİVİTE HARİTASI
    // =============================================
    function yukleAktiviteTab() {
        var gunler = ['Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'];
        var saatler = ['08', '10', '12', '14', '16', '18', '20', '22'];
        var veri = [[10,15,20,18,22,8,5],[25,40,52,48,55,18,12],[60,72,85,80,78,35,22],[75,88,95,92,90,48,30],[82,90,88,85,80,55,38],[65,70,72,68,65,42,28],[45,50,55,52,48,30,18],[20,25,28,26,22,15,8]];
        var $konteyner = $('#heatmapKonteyner');
        $konteyner.empty();
        saatler.forEach(function (saat, si) {
            var $satir = $('<div class="heatmap-satir"></div>');
            $satir.append('<span class="heatmap-saat-etiket">' + saat + ':00</span>');
            gunler.forEach(function (gun, gi) { var yogunluk = veri[si][gi]; var alfa = 0.08 + (yogunluk / 100) * 0.92; $satir.append('<div class="heatmap-hucre" style="background:rgba(79,70,229,' + alfa.toFixed(2) + ')" title="' + gun + ' ' + saat + ':00 — ' + yogunluk + ' sorgu"></div>'); });
            $konteyner.append($satir);
        });
        var $gunEtiketleri = $('#heatmapGunEtiketleri');
        $gunEtiketleri.empty();
        $gunEtiketleri.append('<span class="heatmap-saat-etiket" style="visibility:hidden"></span>');
        gunler.forEach(function (gun) { $gunEtiketleri.append('<span class="heatmap-gun-ad">' + gun + '</span>'); });
        yukleSaatlikAktiviteChart();
        yukleGunlukAktiviteChart();
    }

    function yukleSaatlikAktiviteChart() {
        var ctx = document.getElementById('saatlikAktiviteChart');
        if (!ctx) return;
        new Chart(ctx, { type: 'bar', data: { labels: ['08:00','09:00','10:00','11:00','12:00','13:00','14:00','15:00','16:00','17:00','18:00','19:00','20:00','21:00','22:00'], datasets: [{ label: 'Widget Sorgu', data: [120,280,520,680,740,820,890,850,810,760,640,480,320,180,90], backgroundColor: [120,280,520,680,740,820,890,850,810,760,640,480,320,180,90].map(function(v){ var alfa=0.15+(v/890)*0.65; return 'rgba(14,165,233,'+alfa.toFixed(2)+')'; }), borderColor: '#0ea5e9', borderWidth: 1.5, borderRadius: 4 }] }, options: { responsive: true, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.04)' } }, x: { grid: { display: false } } } } });
    }

    function yukleGunlukAktiviteChart() {
        var ctx = document.getElementById('gunlukAktiviteChart');
        if (!ctx) return;
        new Chart(ctx, { type: 'bar', data: { labels: ['Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi', 'Pazar'], datasets: [{ label: 'Günlük Sorgu', data: [4820, 5340, 5980, 5620, 5180, 3240, 2180], backgroundColor: ['rgba(16,185,129,0.7)','rgba(16,185,129,0.7)','rgba(16,185,129,0.7)','rgba(16,185,129,0.7)','rgba(16,185,129,0.7)','rgba(107,114,128,0.5)','rgba(107,114,128,0.5)'], borderRadius: 5 }] }, options: { responsive: true, plugins: { legend: { display: false }, tooltip: { callbacks: { label: function (c) { return c.raw.toLocaleString('tr-TR') + ' sorgu'; } } } }, scales: { y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.04)' }, ticks: { callback: function (v) { return (v / 1000).toFixed(0) + 'K'; } } }, x: { grid: { display: false } } } } });
    }

    // =============================================
    // 17. Gün: Ürün Seçici ve Aksiyon Butonları
    // =============================================
    $('#urunSecici').on('change', function() {
        var urunId = $(this).val();
        var urunAd = $(this).find('option:selected').text();
        if (urunId !== 'all') {
            $('#urunDetayKarti').show();
            $('#urunAdi').text(urunAd);
            var urunVerileri = { 1: { kategori: 'Spor', stok: 1240, fiyat: '₺ 1,299', skor: 86, durum: 'Stokta Var' }, 2: { kategori: 'Fitness', stok: 580, fiyat: '₺ 899', skor: 79, durum: 'Stokta Var' }, 3: { kategori: 'Aksesuar', stok: 2100, fiyat: '₺ 349', skor: 82, durum: 'Stokta Var' }, 4: { kategori: 'Aksesuar', stok: 95, fiyat: '₺ 129', skor: 85, durum: 'Son 5 Ürün!' } };
            var data = urunVerileri[urunId];
            if (data) {
                $('#urunKategori').text(data.kategori);
                $('#urunStok').text(data.stok.toLocaleString('tr-TR'));
                $('#urunFiyat').text(data.fiyat);
                $('#aiPerformansSkoru').text(data.skor);
                $('#aiSkorProgress').css('width', data.skor + '%');
                $('#urunStokDurumu').text(data.durum);
                if (data.durum.includes('Son')) $('#urunStokDurumu').removeClass('bg-success-soft').addClass('bg-warning-soft text-warning');
                else $('#urunStokDurumu').removeClass('bg-warning-soft').addClass('bg-success-soft text-success');
            }
            if (aktifTab === 'nlp') nlpOzetiniGuncelle(urunId);
        } else { $('#urunDetayKarti').hide(); if (aktifTab === 'nlp') nlpOzetiniGuncelle('all'); }
    });
    setTimeout(function() { if ($('#urunSecici').val() !== 'all') $('#urunDetayKarti').show(); }, 100);

    $('#exportPdfBtn').on('click', function() { abp.message.info('PDF raporu hazırlanıyor ve indirilecek...', 'Rapor Dışa Aktar'); });
    $('#exportExcelBtn').on('click', function() { abp.message.info('Excel raporu hazırlanıyor...', 'Rapor Dışa Aktar'); });
    $('#shareReportBtn').on('click', function() { abp.message.info('Rapor paylaşım linki oluşturuluyor...', 'Paylaş'); });

    $('.fullscreen-chart-btn').on('click', function() {
        var targetId = $(this).data('target');
        if (!targetId) return;
        var $element = $('#' + targetId);
        if ($element.length === 0) return;
        var modal = $('<div class="chart-fullscreen-modal"></div>');
        if ($element.is('canvas')) {
            var $clone = $element.clone();
            $clone.attr('width', $element.width());
            $clone.attr('height', $element.height());
            modal.append($clone);
        } else {
            var $clone = $element.clone();
            $clone.css({ 'max-width': '90%', 'max-height': '90%' });
            modal.append($clone);
        }
        $('body').append(modal);
        modal.on('click', function() { modal.remove(); });
        $(document).on('keyup.chartFullscreen', function(e) { if (e.key === 'Escape') { modal.remove(); $(document).off('keyup.chartFullscreen'); } });
    });

    // Başlangıç
    grafiklerYuklendi['gelir'] = true;
    yukleGelirTab();

});
