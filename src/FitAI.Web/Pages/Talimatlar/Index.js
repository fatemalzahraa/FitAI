$(function () {

    // =============================================
    // Örnek Talimat Verileri
    // =============================================
    var talimatlar = [
        { id: 1, ad: 'Günlük NLP Analizi', aciklama: 'Gelen yorumları NLP ile analiz eder', tetikleyici: 'zamanli', cron: '0 2 * * *', durum: 'aktif', sonCalisma: '2024-05-07 02:15:23', prompt: 'Yorumları duygu analizi yaparak pozitif/negatif/nötr olarak sınıflandır.' },
        { id: 2, ad: 'Ürün Skor Güncelleme', aciklama: 'Ürünlerin AI skorlarını günceller', tetikleyici: 'zamanli', cron: '0 3 * * *', durum: 'aktif', sonCalisma: '2024-05-07 03:00:12', prompt: 'Her ürün için vücut uyum skorunu hesapla ve güncelle.' },
        { id: 3, ad: 'İade Riski Tespiti', aciklama: 'Yüksek iade riski olan siparişleri tespit eder', tetikleyici: 'olay', cron: '-', durum: 'aktif', sonCalisma: '2024-05-06 14:30:45', prompt: 'Sipariş verilerine göre iade riski yüksek olanları işaretle.' },
        { id: 4, ad: 'Stok Tahmini', aciklama: 'Gelecek ay için stok ihtiyacını tahmin eder', tetikleyici: 'zamanli', cron: '0 4 1 * *', durum: 'beklemede', sonCalisma: '-', prompt: 'Satış trendlerine göre stok tahmini yap.' },
        { id: 5, ad: 'Rapor Oluşturma', aciklama: 'Haftalık AI raporunu oluşturur', tetikleyici: 'manuel', cron: '-', durum: 'pasif', sonCalisma: '2024-05-01 09:00:00', prompt: 'Haftalık AI performans raporu hazırla.' }
    ];

    var sayfaBoyutu = 10;
    var aktifSayfa = 1;
    var aktifDurumFiltre = 'all';
    var aramaKelimesi = '';

    // KPI verilerini güncelle
    function kpiGuncelle() {
        var toplam = talimatlar.length;
        var aktif = talimatlar.filter(function(t) { return t.durum === 'aktif'; }).length;
        var beklemede = talimatlar.filter(function(t) { return t.durum === 'beklemede'; }).length;
        var bugun = new Date().toISOString().slice(0, 10);
        var bugunCalisan = talimatlar.filter(function(t) { 
            return t.sonCalisma && t.sonCalisma.startsWith(bugun);
        }).length;
        
        $('#toplamTalimat').text(toplam);
        $('#aktifTalimat').text(aktif);
        $('#bekleyenTalimat').text(beklemede);
        $('#bugunCalisan').text(bugunCalisan);
    }

    // Filtreleme
    function filtrelenmisTalimatlar() {
        var filtered = talimatlar.slice();
        
        if (aktifDurumFiltre !== 'all') {
            filtered = filtered.filter(function(t) { return t.durum === aktifDurumFiltre; });
        }
        
        if (aramaKelimesi.trim() !== '') {
            filtered = filtered.filter(function(t) {
                return t.ad.toLowerCase().includes(aramaKelimesi.toLowerCase()) ||
                       t.aciklama.toLowerCase().includes(aramaKelimesi.toLowerCase());
            });
        }
        
        return filtered;
    }

    // Tetikleyici metni
    function tetikleyiciText(tetikleyici) {
        if (tetikleyici === 'zamanli') return '⏰ Zamanlı';
        if (tetikleyici === 'olay') return '⚡ Olay';
        return '👆 Manuel';
    }

    // Durum metni
    function durumText(durum) {
        if (durum === 'aktif') return 'Aktif';
        if (durum === 'beklemede') return 'Beklemede';
        return 'Pasif';
    }

    // Tabloyu render et
    function talimatTablosunuRender() {
        var filtered = filtrelenmisTalimatlar();
        var toplamSayfa = Math.max(1, Math.ceil(filtered.length / sayfaBoyutu));
        
        if (aktifSayfa > toplamSayfa) aktifSayfa = 1;
        
        var start = (aktifSayfa - 1) * sayfaBoyutu;
        var sayfaTalimatlar = filtered.slice(start, start + sayfaBoyutu);
        
        var $tbody = $('#talimatListesiTablosu');
        $tbody.empty();
        
        if (sayfaTalimatlar.length === 0) {
            $tbody.html(
                '<tr class="empty-row">' +
                    '<td colspan="7" class="text-center py-5">' +
                        '<i class="fas fa-inbox fa-2x text-muted mb-2 d-block"></i>' +
                        '<span class="text-muted">Talimat bulunamadı</span>' +
                    '</td>' +
                '</tr>'
            );
            $('#talimatSayac').text('0 talimat gösteriliyor');
            return;
        }
        
        sayfaTalimatlar.forEach(function(t) {
            var durumClass = t.durum === 'aktif' ? 'durum-aktif' : (t.durum === 'beklemede' ? 'durum-beklemede' : 'durum-pasif');
            var durumIcon = t.durum === 'aktif' ? 'fa-check-circle' : (t.durum === 'beklemede' ? 'fa-clock' : 'fa-pause-circle');
            var sonCalismaText = t.sonCalisma !== '-' ? t.sonCalisma : '—';
            var aciklamaShort = t.aciklama.length > 35 ? t.aciklama.substring(0, 35) + '...' : t.aciklama;
            
            $tbody.append(
                '<tr data-id="' + t.id + '">' +
                    '<td><strong>' + escapeHtml(t.ad) + '</strong></td>' +
                    '<td class="text-muted small">' + escapeHtml(aciklamaShort) + '</td>' +
                    '<td>' + tetikleyiciText(t.tetikleyici) + '</td>' +
                    '<td class="font-monospace small">' + (t.cron !== '-' ? t.cron : '—') + '</td>' +
                    '<td><span class="durum-badge ' + durumClass + '"><i class="fas ' + durumIcon + ' me-1"></i>' + durumText(t.durum) + '</span></td>' +
                    '<td class="small">' + sonCalismaText + '</td>' +
                    '<td class="text-center">' +
                        '<div class="talimat-aksiyon">' +
                            '<button class="aksiyon-btn detay" data-id="' + t.id + '" data-aksiyon="detay" title="Detay"><i class="fas fa-eye"></i></button>' +
                            '<button class="aksiyon-btn calistir" data-id="' + t.id + '" data-aksiyon="calistir" title="Çalıştır"><i class="fas fa-play"></i></button>' +
                            '<button class="aksiyon-btn duzenle" data-id="' + t.id + '" data-aksiyon="duzenle" title="Düzenle"><i class="fas fa-edit"></i></button>' +
                            '<button class="aksiyon-btn sil" data-id="' + t.id + '" data-aksiyon="sil" title="Sil"><i class="fas fa-trash"></i></button>' +
                        '</div>' +
                    '</td>' +
                '</tr>'
            );
        });
        
        $('#talimatSayac').text(sayfaTalimatlar.length + ' talimat gösteriliyor (toplam ' + filtered.length + ')');
        paginationRender(toplamSayfa);
    }
    
    // XSS koruması için basit escape
    function escapeHtml(str) {
        if (!str) return '';
        return str.replace(/[&<>]/g, function(m) {
            if (m === '&') return '&amp;';
            if (m === '<') return '&lt;';
            if (m === '>') return '&gt;';
            return m;
        });
    }

    // Pagination render
    function paginationRender(toplamSayfa) {
        var $pagination = $('#talimatPagination');
        $pagination.empty();
        
        if (toplamSayfa <= 1) return;
        
        // Previous
        if (aktifSayfa > 1) {
            $pagination.append('<li class="page-item"><a class="page-link" href="#" data-page="prev"><i class="fas fa-chevron-left"></i></a></li>');
        }
        
        // Sayfa numaraları
        var startPage = Math.max(1, aktifSayfa - 2);
        var endPage = Math.min(toplamSayfa, startPage + 4);
        if (endPage - startPage < 4 && startPage > 1) startPage = Math.max(1, endPage - 4);
        
        for (var i = startPage; i <= endPage; i++) {
            $pagination.append('<li class="page-item ' + (i === aktifSayfa ? 'active' : '') + '"><a class="page-link" href="#" data-page="' + i + '">' + i + '</a></li>');
        }
        
        // Next
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
            talimatTablosunuRender();
        });
    }

    // Filtreleme eventleri
    $('#talimatAra').on('keyup', function() {
        aramaKelimesi = $(this).val();
        aktifSayfa = 1;
        talimatTablosunuRender();
    });
    
    $('#durumFiltre').on('change', function() {
        aktifDurumFiltre = $(this).val();
        aktifSayfa = 1;
        talimatTablosunuRender();
    });

    // Yeni Talimat Modal
    var yeniTalimatModal;
    $('#yeniTalimatBtn').on('click', function() {
        yeniTalimatModal = new bootstrap.Modal(document.getElementById('yeniTalimatModal'));
        $('#talimatForm')[0].reset();
        yeniTalimatModal.show();
    });

    // Kaydet
    $('#talimatKaydetBtn').on('click', function() {
        var ad = $('#talimatAd').val().trim();
        if (!ad) {
            abp.message.warning('Lütfen talimat adını girin.', 'Uyarı');
            return;
        }
        
        var yeniTalimat = {
            id: talimatlar.length + 1,
            ad: ad,
            aciklama: $('#talimatAciklama').val().trim() || '',
            tetikleyici: $('#talimatTetikleyici').val(),
            cron: $('#talimatCron').val().trim() || '-',
            durum: 'aktif',
            sonCalisma: '-',
            prompt: $('#talimatPrompt').val().trim() || ''
        };
        
        talimatlar.push(yeniTalimat);
        kpiGuncelle();
        talimatTablosunuRender();
        yeniTalimatModal.hide();
        abp.message.success('Yeni talimat başarıyla oluşturuldu.', 'Başarılı');
    });

    // Talimat aksiyonları
    $(document).on('click', '.aksiyon-btn', function() {
        var id = parseInt($(this).data('id'));
        var aksiyon = $(this).data('aksiyon');
        var talimat = talimatlar.find(function(t) { return t.id === id; });
        
        if (!talimat) return;
        
        if (aksiyon === 'detay') {
            $('#talimatDetayIcerik').html(
                '<div class="talimat-detay-bilgi">' +
                    '<p><strong>Talimat Adı:</strong> ' + escapeHtml(talimat.ad) + '</p>' +
                    '<p><strong>Açıklama:</strong> ' + escapeHtml(talimat.aciklama || '—') + '</p>' +
                    '<p><strong>Tetikleyici:</strong> ' + tetikleyiciText(talimat.tetikleyici) + '</p>' +
                    '<p><strong>Çalışma Zamanı:</strong> ' + (talimat.cron !== '-' ? talimat.cron : '—') + '</p>' +
                    '<p><strong>Durum:</strong> ' + durumText(talimat.durum) + '</p>' +
                    '<p><strong>Son Çalışma:</strong> ' + (talimat.sonCalisma !== '-' ? talimat.sonCalisma : '—') + '</p>' +
                    '<p><strong>AI Prompt:</strong></p>' +
                    '<pre>' + escapeHtml(talimat.prompt || '—') + '</pre>' +
                '</div>'
            );
            var detayModal = new bootstrap.Modal(document.getElementById('talimatDetayModal'));
            detayModal.show();
        }
        else if (aksiyon === 'calistir') {
            abp.message.info('"' + talimat.ad + '" talimatı çalıştırılıyor...', 'Talimat Çalıştırılıyor');
            setTimeout(function() {
                var now = new Date();
                var formatted = now.toISOString().slice(0, 10).replace('T', ' ') + ' ' + now.toTimeString().slice(0, 8);
                talimat.sonCalisma = formatted;
                talimatTablosunuRender();
                abp.message.success('"' + talimat.ad + '" talimatı başarıyla çalıştırıldı.', 'Başarılı');
                kpiGuncelle();
            }, 1500);
        }
        else if (aksiyon === 'duzenle') {
            abp.message.info('"' + talimat.ad + '" talimatı düzenleme özelliği yakında eklenecek.', 'Bilgi');
        }
        else if (aksiyon === 'sil') {
            abp.message.confirm('"' + talimat.ad + '" talimatını silmek istediğinize emin misiniz?', 'Onay').then(function(confirmed) {
                if (confirmed) {
                    var index = talimatlar.findIndex(function(t) { return t.id === id; });
                    if (index !== -1) {
                        talimatlar.splice(index, 1);
                        kpiGuncelle();
                        talimatTablosunuRender();
                        abp.message.success('Talimat başarıyla silindi.', 'Başarılı');
                    }
                }
            });
        }
    });

    // Test Et butonu (detay modalında)
    $('#talimatTestEtBtn').on('click', function() {
        var modal = bootstrap.Modal.getInstance(document.getElementById('talimatDetayModal'));
        if (modal) modal.hide();
        abp.message.info('Test çalıştırma özelliği yakında eklenecek.', 'Bilgi');
    });

    // Başlangıç
    kpiGuncelle();
    talimatTablosunuRender();

});