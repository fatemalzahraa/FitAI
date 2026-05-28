$(function () {

    // =============================================
    // Örnek Komisyon Verileri
    // =============================================
    var komisyonVerileri = [
        { id: 1, magaza: 'SportZone TR', paket: 'Premium', oran: 8, buAy: 12400, toplam: 82400, durum: 'beklemede', sonOdeme: '2024-04-15' },
        { id: 2, magaza: 'FashionHub', paket: 'Standart', oran: 5, buAy: 8900, toplam: 61200, durum: 'odendi', sonOdeme: '2024-05-05' },
        { id: 3, magaza: 'ActiveWear', paket: 'Premium', oran: 8, buAy: 7200, toplam: 48700, durum: 'beklemede', sonOdeme: '2024-04-28' },
        { id: 4, magaza: 'FitStyle', paket: 'Standart', oran: 5, buAy: 5600, toplam: 39600, durum: 'odendi', sonOdeme: '2024-05-01' },
        { id: 5, magaza: 'RunnerShop', paket: 'Başlangıç', oran: 3, buAy: 2800, toplam: 31800, durum: 'beklemede', sonOdeme: '2024-04-10' },
        { id: 6, magaza: 'YogaLife', paket: 'Standart', oran: 5, buAy: 4300, toplam: 21500, durum: 'odendi', sonOdeme: '2024-05-03' },
        { id: 7, magaza: 'FitnessPro', paket: 'Premium', oran: 8, buAy: 10500, toplam: 56200, durum: 'beklemede', sonOdeme: '2024-04-20' }
    ];

    var sayfaBoyutu = 10;
    var aktifSayfa = 1;
    var aktifDurumFiltre = 'all';
    var aramaKelimesi = '';

    // Komisyon Trend Grafiği
    var komisyonTrendChart = null;

    function komisyonTrendChartYukle(tip) {
        var ctx = document.getElementById('komisyonTrendChart');
        if (!ctx) return;
        if (komisyonTrendChart) komisyonTrendChart.destroy();

        var aylar = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran'];
        var gelirler = [152000, 168000, 185000, 198000, 212000, 234000];

        komisyonTrendChart = new Chart(ctx, {
            type: tip,
            data: {
                labels: aylar,
                datasets: [{
                    label: 'Komisyon Geliri (₺)',
                    data: gelirler,
                    borderColor: '#10b981',
                    backgroundColor: tip === 'line' ? 'rgba(16,185,129,0.08)' : 'rgba(16,185,129,0.25)',
                    borderWidth: 3,
                    fill: tip === 'line',
                    tension: 0.4,
                    pointRadius: tip === 'line' ? 4 : 0,
                    pointBackgroundColor: '#10b981',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2,
                    pointHoverRadius: 6,
                    borderRadius: tip === 'bar' ? 8 : 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function(c) { return '₺ ' + c.raw.toLocaleString('tr-TR'); }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(0,0,0,0.04)' },
                        ticks: { callback: function(v) { return '₺' + (v / 1000).toFixed(0) + 'K'; } }
                    },
                    x: { grid: { display: false } }
                }
            }
        });
    }

    window.komisyonGrafikTipiDegistir = function(tip, btn) {
        $('.card-header .btn-group .btn').removeClass('active');
        $(btn).addClass('active');
        komisyonTrendChartYukle(tip);
    };

    // KPI verilerini güncelle
    function kpiGuncelle() {
        var toplamKomisyon = komisyonVerileri.reduce(function(t, m) { return t + m.toplam; }, 0);
        var aylikKomisyon = komisyonVerileri.reduce(function(t, m) { return t + m.buAy; }, 0);
        var bekleyenOdeme = komisyonVerileri.filter(function(m) { return m.durum === 'beklemede'; }).reduce(function(t, m) { return t + m.buAy; }, 0);
        var odendiKomisyon = komisyonVerileri.filter(function(m) { return m.durum === 'odendi'; }).reduce(function(t, m) { return t + m.buAy; }, 0);

        $('#toplamKomisyon').text('₺ ' + toplamKomisyon.toLocaleString('tr-TR'));
        $('#aylikKomisyon').text('₺ ' + aylikKomisyon.toLocaleString('tr-TR'));
        $('#bekleyenOdeme').text('₺ ' + bekleyenOdeme.toLocaleString('tr-TR'));
        $('#odendiKomisyon').text('₺ ' + odendiKomisyon.toLocaleString('tr-TR'));
    }

    // Filtreleme
    function filtrelenmisMagazalar() {
        var filtered = komisyonVerileri.slice();
        
        if (aktifDurumFiltre !== 'all') {
            filtered = filtered.filter(function(m) { return m.durum === aktifDurumFiltre; });
        }
        
        if (aramaKelimesi.trim() !== '') {
            filtered = filtered.filter(function(m) {
                return m.magaza.toLowerCase().includes(aramaKelimesi.toLowerCase());
            });
        }
        
        return filtered;
    }

    // Tabloyu render et
    function komisyonTablosunuRender() {
        var filtered = filtrelenmisMagazalar();
        var toplamSayfa = Math.max(1, Math.ceil(filtered.length / sayfaBoyutu));
        
        if (aktifSayfa > toplamSayfa) aktifSayfa = 1;
        
        var start = (aktifSayfa - 1) * sayfaBoyutu;
        var sayfaMagazalar = filtered.slice(start, start + sayfaBoyutu);
        
        var $tbody = $('#komisyonListesiTablosu');
        $tbody.empty();
        
        if (sayfaMagazalar.length === 0) {
            $tbody.append(
                '<tr>' +
                    '<td colspan="9" class="text-center py-5">' +
                        '<i class="fas fa-inbox fa-2x text-muted mb-2 d-block"></i>' +
                        '<span class="text-muted">Mağaza bulunamadı</span>' +
                    '</td>' +
                '</tr>'
            );
            $('#komisyonSayac').text('0 mağaza gösteriliyor');
            return;
        }
        
        sayfaMagazalar.forEach(function(m, index) {
            var durumClass = m.durum === 'odendi' ? 'odeme-odendi' : (m.durum === 'beklemede' ? 'odeme-beklemede' : 'odeme-iptal');
            var durumIcon = m.durum === 'odendi' ? 'fa-check-circle' : (m.durum === 'beklemede' ? 'fa-clock' : 'fa-times-circle');
            var durumText = m.durum === 'odendi' ? 'Ödendi' : (m.durum === 'beklemede' ? 'Beklemede' : 'İptal');
            var paketBadge = m.paket === 'Premium' ? 
                '<span class="badge bg-warning-soft text-warning"><i class="fas fa-crown me-1"></i>' + m.paket + '</span>' : 
                (m.paket === 'Standart' ? 
                    '<span class="badge bg-primary-soft text-primary">' + m.paket + '</span>' : 
                    '<span class="badge bg-secondary-soft text-secondary">' + m.paket + '</span>');
            
            $tbody.append(
                '<tr data-id="' + m.id + '">' +
                    '<td><span class="fw-semibold">' + (start + index + 1) + '</span></td>' +
                    '<td><strong>' + m.magaza + '</strong></td>' +
                    '<td>' + paketBadge + '</td>' +
                    '<td><span class="badge bg-light text-dark">%' + m.oran + '</span></td>' +
                    '<td class="fw-bold text-success">₺ ' + m.buAy.toLocaleString('tr-TR') + '</td>' +
                    '<td class="text-muted">₺ ' + m.toplam.toLocaleString('tr-TR') + '</td>' +
                    '<td><span class="odeme-badge ' + durumClass + '"><i class="fas ' + durumIcon + ' me-1"></i>' + durumText + '</span></td>' +
                    '<td class="small">' + (m.sonOdeme || '—') + '</td>' +
                    '<td>' +
                        (m.durum === 'beklemede' ? 
                            '<button class="odeme-aksiyon-btn odeme-yap" data-id="' + m.id + '" data-magaza="' + m.magaza + '" data-tutar="' + m.buAy + '"><i class="fas fa-money-bill-wave me-1"></i>Öde</button>' : 
                            '<button class="odeme-aksiyon-btn detay" data-id="' + m.id + '" data-magaza="' + m.magaza + '"><i class="fas fa-info-circle me-1"></i>Detay</button>') +
                    '</td>' +
                '</tr>'
            );
        });
        
        $('#komisyonSayac').text(sayfaMagazalar.length + ' mağaza gösteriliyor (toplam ' + filtered.length + ')');
        komisyonPaginationRender(toplamSayfa);
    }

    // Pagination render
    function komisyonPaginationRender(toplamSayfa) {
        var $pagination = $('#komisyonPagination');
        $pagination.empty();
        
        if (toplamSayfa <= 1) return;
        
        // Previous butonu
        if (aktifSayfa > 1) {
            $pagination.append('<li class="page-item"><a class="page-link" href="#" data-page="prev"><i class="fas fa-chevron-left"></i></a></li>');
        }
        
        // Sayfa numaraları (maks 5 göster)
        var startPage = Math.max(1, aktifSayfa - 2);
        var endPage = Math.min(toplamSayfa, startPage + 4);
        if (endPage - startPage < 4 && startPage > 1) startPage = Math.max(1, endPage - 4);
        
        for (var i = startPage; i <= endPage; i++) {
            $pagination.append('<li class="page-item ' + (i === aktifSayfa ? 'active' : '') + '"><a class="page-link" href="#" data-page="' + i + '">' + i + '</a></li>');
        }
        
        // Next butonu
        if (aktifSayfa < toplamSayfa) {
            $pagination.append('<li class="page-item"><a class="page-link" href="#" data-page="next"><i class="fas fa-chevron-right"></i></a></li>');
        }
        
        $pagination.find('.page-link').on('click', function(e) {
            e.preventDefault();
            var page = $(this).data('page');
            if (page === 'prev' && aktifSayfa > 1) aktifSayfa--;
            else if (page === 'next' && aktifSayfa < toplamSayfa) aktifSayfa++;
            else if (!isNaN(parseInt(page))) aktifSayfa = parseInt(page);
            else return;
            komisyonTablosunuRender();
        });
    }

    // Filtreleme eventleri
    $('#magazaAra').on('keyup', function() {
        aramaKelimesi = $(this).val();
        aktifSayfa = 1;
        komisyonTablosunuRender();
    });
    
    $('#odemeDurumuFiltre').on('change', function() {
        aktifDurumFiltre = $(this).val();
        aktifSayfa = 1;
        komisyonTablosunuRender();
    });

    // Ödeme Modal
    var odemeModal;
    var secilenMagazaId = null;

    $(document).on('click', '.odeme-aksiyon-btn.odeme-yap', function() {
        var magazaId = $(this).data('id');
        var magazaAd = $(this).data('magaza');
        var tutar = $(this).data('tutar');
        
        secilenMagazaId = magazaId;
        $('#odemeMagazaAd').val(magazaAd);
        $('#odemeTutar').val(tutar);
        $('#odemeTarih').val(new Date().toISOString().split('T')[0]);
        $('#odemeAciklama').val('');
        
        odemeModal = new bootstrap.Modal(document.getElementById('odemeModal'));
        odemeModal.show();
    });

    $('#odemeOnaylaBtn').on('click', function() {
        var odemeTutar = parseFloat($('#odemeTutar').val());
        var odemeTarih = $('#odemeTarih').val();
        var aciklama = $('#odemeAciklama').val();
        
        if (!odemeTutar || odemeTutar <= 0) {
            abp.message.warning('Lütfen geçerli bir tutar girin.', 'Uyarı');
            return;
        }
        
        var magaza = komisyonVerileri.find(function(m) { return m.id === secilenMagazaId; });
        if (magaza) {
            magaza.durum = 'odendi';
            magaza.sonOdeme = odemeTarih;
            magaza.buAy = 0;
            
            kpiGuncelle();
            komisyonTablosunuRender();
            odemeModal.hide();
            abp.message.success(magaza.magaza + ' mağazasına ₺ ' + odemeTutar.toLocaleString('tr-TR') + ' ödeme yapıldı.', 'Başarılı');
        }
    });

    $(document).on('click', '.odeme-aksiyon-btn.detay', function() {
        var magazaId = $(this).data('id');
        var magaza = komisyonVerileri.find(function(m) { return m.id === magazaId; });
        if (magaza) {
            abp.message.info(
                '<strong>' + magaza.magaza + '</strong><br>' +
                'Paket: ' + magaza.paket + '<br>' +
                'Komisyon Oranı: %' + magaza.oran + '<br>' +
                'Toplam Komisyon: ₺ ' + magaza.toplam.toLocaleString('tr-TR'),
                'Mağaza Detayı'
            );
        }
    });

    // Rapor indirme
    $('#raporIndirBtn').on('click', function() {
        abp.message.info('Komisyon raporu Excel formatında hazırlanıyor...', 'Rapor');
    });

    $('#odemeYapBtn').on('click', function() {
        abp.message.info('Toplu ödeme sayfasına yönlendiriliyorsunuz.', 'Bilgi');
    });

    // Başlangıç
    komisyonTrendChartYukle('line');
    kpiGuncelle();
    komisyonTablosunuRender();

});