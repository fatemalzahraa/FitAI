$(function () {

    // =============================================
    // MAĞAZA ID YÖNETİMİ - KRİTİK!
    // =============================================
    // TODO: Bu değer URL'den, session'dan veya kullanıcı seçiminden alınmalı
    var currentMagazaId = 1; // BUNU DİNAMİK YAPIN!
    
    // Örnek: URL'den magazaId almak için:
    // var currentMagazaId = new URLSearchParams(window.location.search).get('magazaId') || 1;
    
    // Örnek: Dropdown ile mağaza seçimi için:
    // $('#magazaSecici').on('change', function() {
    //     currentMagazaId = $(this).val();
    //     loadDashboard();
    // });

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

    function yukleGelirPaketChart() {
        var ctx = document.getElementById('gelirPaketChart');
        if (!ctx) return;
        if (gelirPaketChart) gelirPaketChart.destroy();

        var paketler = ['Premium', 'Standart', 'Başlangıç'];
        var veriler  = [245600, 132400, 34800];
        var toplam   = veriler.reduce(function (t, v) { return t + v; }, 0);

        gelirPaketChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: paketler,
                datasets: [{
                    data: veriler,
                    backgroundColor: ['#4f46e5', '#10b981', '#6b7280'],
                    borderWidth: 3,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: false,
                cutout: '68%',
                plugins: {
                    legend: { display: false }
                }
            }
        });

        var $leg = $('#gelirPaketLegend');
        $leg.empty();
        paketler.forEach(function (p, i) {
            var pct = ((veriler[i] / toplam) * 100).toFixed(1);
            var renk = ['#4f46e5', '#10b981', '#6b7280'][i];
            $leg.append(
                '<div class="legend-item">' +
                    '<div class="legend-sol">' +
                        '<div class="legend-renk" style="background:' + renk + '"></div>' +
                        '<span class="legend-ad">' + p + '</span>' +
                    '</div>' +
                    '<span class="legend-deger">%' + pct + '</span>' +
                '</div>'
            );
        });
    }

    $('#gelirGrafikTip .btn').on('click', function () {
        $('#gelirGrafikTip .btn').removeClass('active');
        $(this).addClass('active');
        yukleGelirZamanChart($(this).data('tip'));
    });

    // =============================================
    // SEKME 2: NLP & DUYGU - BACKEND ENTEGRE EDİLDİ ✅
    // =============================================
    var aktifDuyguFiltre = 'all';
    var gosterilecekYorumSayisi = 5;

    function yukleNlpTab() {
        // Backend'den KPI verilerini yükle
        yukleMagazaOzeti();
        
        // Backend'den duygu dağılımını yükle
        yukleDuyguDagilimi();
        
        // Backend'den tema verilerini yükle
        yukleTopTemalar();
        
        // Diğer statik grafikler (bunlar da backend'e bağlanabilir)
        yukleNlpTrendChart();
        yukleKelimeBulutu();
        yukleKategoriMemnuniyetChart();
        yorumListesiniRender('all', 5);
        nlpOzetiniGuncelle($('#urunSecici').val());
    }

    // ✅ BACKEND ENTEGRASYONU: Mağaza Özeti
    function yukleMagazaOzeti() {
        abp.ajax({
            url: '/api/app/analytics/store-summary',
            data: { magazaId: currentMagazaId }
        }).done(function (result) {
            $('#nlp_toplam').text(result.toplamYorumSayisi.toLocaleString('tr-TR'));
            $('#toplamUrun').text(result.toplamUrunSayisi.toLocaleString('tr-TR'));
            $('#ortalamaPuan').text(result.magazaPuanOrtalamasi.toFixed(1));
            $('#bekleyenYorum').text(result.islenmeyiBekleyenYorumlar.toLocaleString('tr-TR'));
        }).fail(function (error) {
            console.error('Mağaza özeti yüklenemedi:', error);
            // Hata durumunda varsayılan değerler
            $('#nlp_toplam').text('—');
            $('#toplamUrun').text('—');
            $('#ortalamaPuan').text('—');
            $('#bekleyenYorum').text('—');
        });
    }

    // ✅ BACKEND ENTEGRASYONU: Duygu Dağılımı
    function yukleDuyguDagilimi() {
        abp.ajax({
            url: '/api/app/analytics/sentiment-distribution',
            data: { magazaId: currentMagazaId }
        }).done(function (result) {
            yukleNlpDuyguChart(result);
            
            // KPI kartlarını güncelle
            var pozitif = result.find(function(r) { return r.etiket === 'Pozitif'; });
            var negatif = result.find(function(r) { return r.etiket === 'Negatif'; });
            var notr = result.find(function(r) { return r.etiket === 'Nötr' || r.etiket === 'Belirsiz'; });
            
            if (pozitif) $('#nlp_pozitif').text(pozitif.sayi.toLocaleString('tr-TR'));
            if (negatif) $('#nlp_negatif').text(negatif.sayi.toLocaleString('tr-TR'));
            if (notr) $('#nlp_notr').text(notr.sayi.toLocaleString('tr-TR'));
        }).fail(function (error) {
            console.error('Duygu dağılımı yüklenemedi:', error);
            // Hata durumunda varsayılan grafik
            yukleNlpDuyguChart([
                { etiket: 'Pozitif', sayi: 0, yuzde: 0 },
                { etiket: 'Negatif', sayi: 0, yuzde: 0 }
            ]);
        });
    }

    // ✅ BACKEND ENTEGRASYONU: Top Temalar
    function yukleTopTemalar() {
        abp.ajax({
            url: '/api/app/analytics/top-themes',
            data: { magazaId: currentMagazaId }
        }).done(function (result) {
            // Tema verilerini kelime bulutu olarak göster
            yukleKelimeBulutuFromBackend(result);
        }).fail(function (error) {
            console.error('Tema verileri yüklenemedi:', error);
        });
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

    // ✅ BACKEND VERİSİYLE ÇALIŞAN DUYGU GRAFİĞİ
    function yukleNlpDuyguChart(backendData) {
        var ctx = document.getElementById('nlpDuyguChart');
        if (!ctx) return;

        // Renk haritası
        var renkMap = {
            'Pozitif': '#10b981',
            'Negatif': '#ef4444',
            'Nötr': '#9ca3af',
            'Belirsiz': '#9ca3af'
        };

        var duygular = backendData.map(function(item) {
            return {
                ad: item.etiket,
                deger: item.sayi,
                renk: renkMap[item.etiket] || '#6b7280'
            };
        });

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
            var pct = toplam > 0 ? ((d.deger / toplam) * 100).toFixed(1) : '0.0';
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

    // ✅ BACKEND VERİSİYLE KELIME BULUTU
    function yukleKelimeBulutuFromBackend(temalar) {
        var $konteyner = $('#kelimeBulutu');
        $konteyner.empty();

        if (!temalar || temalar.length === 0) {
            $konteyner.append('<p class="text-muted text-center">Henüz tema verisi yok</p>');
            return;
        }

        var renkler = ['#4f46e5', '#10b981', '#0ea5e9', '#f59e0b', '#8b5cf6', '#ef4444'];
        var boyutlar = [1.1, 1.0, 0.9, 0.85, 0.8];

        temalar.forEach(function (tema, index) {
            var fontSize = boyutlar[Math.min(index, 4)];
            var renk = renkler[index % renkler.length];
            var opacity = 0.7 + (0.3 * (1 - index / temalar.length));
            
            $konteyner.append(
                '<span class="kelime-chip" style="' +
                    'font-size:' + fontSize + 'rem;' +
                    'background:' + renk + '1A;' +
                    'color:' + renk + ';' +
                    'opacity:' + opacity +
                '" title="' + tema.sayi + ' yorumda (%' + tema.yuzde + ')">' + 
                    tema.etiket + 
                '</span>'
            );
        });
    }

    // Yedek statik kelime bulutu (backend çalışmazsa)
    function yukleKelimeBulutu() {
        // Bu fonksiyon artık yukleTopTemalar() tarafından backend'den çekilecek
        // Ama fallback olarak kalabilir
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

    function yorumListesiniRender(duygu, limit) {
        var $liste = $('#yorumListesi');
        $liste.html('<div class="text-center py-3 text-muted"><i class="fas fa-spinner fa-pulse me-2"></i>Yükleniyor...</div>');

        var params = { magazaId: currentMagazaId, maxSayi: limit };
        if (duygu !== 'all') params.duyguEtiketi = duygu.charAt(0).toUpperCase() + duygu.slice(1);
        var urunId = $('#urunSecici').val();
        if (urunId !== 'all') params.urunId = parseInt(urunId);

        abp.ajax({
            url: '/api/app/analytics/reviews',
            data: params
        }).done(function (yorumlar) {
            $liste.empty();
            if (!yorumlar || yorumlar.length === 0) {
                $liste.html('<p class="text-muted text-center py-3">Bu filtre için yorum bulunamadı.</p>');
                $('#dahaFazlaYorumBtn').hide();
                return;
            }
            yorumlar.forEach(function (yorum) {
                var yildizHtml = '';
                for (var i = 0; i < 5; i++) {
                    yildizHtml += i < yorum.yildiz
                        ? '<i class="fas fa-star text-warning"></i>'
                        : '<i class="far fa-star text-muted"></i>';
                }
                var etiket = (yorum.duyguEtiketi || '').toLowerCase();
                var duygुBadge = etiket === 'olumlu' || etiket === 'pozitif'
                    ? '<span class="badge bg-success-soft text-success"><i class="fas fa-smile me-1"></i>Pozitif</span>'
                    : etiket === 'olumsuz' || etiket === 'negatif'
                    ? '<span class="badge bg-danger-soft text-danger"><i class="fas fa-frown me-1"></i>Negatif</span>'
                    : '<span class="badge bg-secondary-soft text-secondary"><i class="fas fa-meh me-1"></i>Nötr</span>';
                var zaman = yorum.zaman ? abp.timing.toUserTime(yorum.zaman) : '';
                $liste.append(
                    '<div class="yorum-item">' +
                        '<div class="yorum-ust">' +
                            '<div class="d-flex align-items-center gap-2">' +
                                '<span class="yorum-kullanici">' + yorum.kullanici + '</span>' +
                                '<span class="yorum-yildiz">' + yildizHtml + '</span>' +
                                duygुBadge +
                            '</div>' +
                            '<span class="yorum-zaman text-muted small">' + zaman + '</span>' +
                        '</div>' +
                        '<p class="yorum-metin mb-0">' + yorum.metin + '</p>' +
                    '</div>'
                );
            });
            if (yorumlar.length >= limit) {
                $('#dahaFazlaYorumBtn').show();
            } else {
                $('#dahaFazlaYorumBtn').hide();
            }
        }).fail(function () {
            $liste.html('<p class="text-danger text-center py-3"><i class="fas fa-exclamation-circle me-2"></i>Yorumlar yüklenemedi.</p>');
        });
    }

    function nlpOzetiniGuncelle(urunId) {
        var params = { magazaId: currentMagazaId };
        if (urunId && urunId !== 'all') params.urunId = parseInt(urunId);

        abp.ajax({
            url: '/api/app/analytics/top-themes',
            data: params
        }).done(function (temalar) {
            var $liste = $('#ozetListesi');
            $liste.empty();
            if (temalar && temalar.length > 0) {
                temalar.forEach(function (tema) {
                    $liste.append(
                        '<li><i class="fas fa-chart-line text-primary me-2"></i>' +
                        tema.etiket + ' — ' + tema.sayi + ' yorum (%' + tema.yuzde + ')' +
                        '</li>'
                    );
                });
                // En baskın temaya göre öneri üret
                var enCok = temalar[0];
                var oneriMap = {
                    'beden': 'Beden tablosunu güncelleyin ve farklı bedenler için ölçü rehberi ekleyin.',
                    'kumas': 'Kumaş kalitesi açıklamasını detaylandırın, bakım talimatları ekleyin.',
                    'iade': 'İade oranı yüksek; ürün açıklaması ve görseller gözden geçirilmeli.',
                    'kalip': 'Kalıp bilgisi ürün detayına eklenmeli, model ölçüleri paylaşılmalı.',
                    'genel': 'Genel müşteri geri bildirimlerini düzenli olarak inceleyin.'
                };
                var oneri = oneriMap[enCok.etiket.toLowerCase()] || oneriMap['genel'];
                $('.ozet-oneri span').text('AI Önerisi: ' + oneri);
            } else {
                $liste.append('<li class="text-muted">Henüz tema verisi yok.</li>');
                $('.ozet-oneri span').text('AI Önerisi: Yeterli veri birikmesi bekleniyor.');
            }
        }).fail(function () {
            $('#ozetListesi').html('<li class="text-danger">Veriler yüklenemedi.</li>');
        });
    }

    $('#duyguFiltreBtnGrubu .btn').on('click', function() {
        $('#duyguFiltreBtnGrubu .btn').removeClass('active');
        $(this).addClass('active');
        aktifDuyguFiltre = $(this).data('duygu');
        gosterilecekYorumSayisi = 5;
        yorumListesiniRender(aktifDuyguFiltre, gosterilecekYorumSayisi);
    });

    $('#dahaFazlaYorumBtn').on('click', function() {
        gosterilecekYorumSayisi += 5;
        yorumListesiniRender(aktifDuyguFiltre, gosterilecekYorumSayisi);
    });

    $(document).on('click', '.yorum-item', function() {
        var metin = $(this).find('.yorum-metin').text();
        var kullanici = $(this).find('.yorum-kullanici').text();
        abp.message.info('"' + metin + '"', kullanici + ' - Yorum Detayı');
    });

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
    // Ürün Seçici ve Aksiyon Butonları
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