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
        var bugunCalisan = talimatlar.filter(function(t) { return t.sonCalisma && t.sonCalisma.startsWith('2024-05-07'); }).length;
        
        $('#toplamTalimat').text(toplam);
        $('#aktifTalimat').text(aktif);
        $('#bekleyenTalimat').text(beklemede);
        $('#bugunCalisan').text(bugunCalisan);
    }

    // Filtreleme
    function filtrelenmisTalimatlar() {
        var filtered = talimatlar;
        
        if (aktifDurumFiltre !== 'all') {
            filtered = filtered.filter(function(t) { return t.durum === aktifDurumFiltre; });
        }
        
        if (aramaKelimesi !== '') {
            filtered = filtered.filter(function(t) {
                return t.ad.toLowerCase().includes(aramaKelimesi.toLowerCase()) ||
                       t.aciklama.toLowerCase().includes(aramaKelimesi.toLowerCase());
            });
        }
        
        return filtered;
    }

    // Tabloyu render et
    function talimatTablosunuRender() {
        var filtered = filtrelenmisTalimatlar();
        var toplamSayfa = Math.ceil(filtered.length / sayfaBoyutu);
        var start = (aktifSayfa - 1) * sayfaBoyutu;
        var sayfaTalimatlar = filtered.slice(start, start + sayfaBoyutu);
        
        var $tbody = $('#talimatListesiTablosu');
        $tbody.empty();
        
        if (sayfaTalimatlar.length === 0) {
            $tbody.append('<tr><td colspan="7" class="text-center py-4"><i class="fas fa-inbox me-2"></i>Talimat bulunamadı</td></tr>');
            $('#talimatSayac').text('0 talimat gösteriliyor');
            return;
        }
        
        sayfaTalimatlar.forEach(function(t) {
            var durumClass = t.durum === 'aktif' ? 'durum-aktif' : (t.durum === 'beklemede' ? 'durum-beklemede' : 'durum-pasif');
            var durumIcon = t.durum === 'aktif' ? 'fa-check-circle' : (t.durum === 'beklemede' ? 'fa-clock' : 'fa-pause-circle');
            var tetikleyiciText = t.tetikleyici === 'zamanli' ? '⏰ Zamanlı' : (t.tetikleyici === 'olay' ? '⚡ Olay' : '👆 Manuel');
            var sonCalismaText = t.sonCalisma !== '-' ? t.sonCalisma : '—';
            
            $tbody.append(
                '<tr data-id="' + t.id + '">' +
                    '<td><strong>' + t.ad + '</strong></td>' +
                    '<td class="text-muted small">' + (t.aciklama.length > 40 ? t.aciklama.substring(0,40) + '...' : t.aciklama) + '</td>' +
                    '<td>' + tetikleyiciText + '</td>' +
                    '<td class="font-monospace small">' + (t.cron !== '-' ? t.cron : '—') + '</td>' +
                    '<td><span class="durum-badge ' + durumClass + '"><i class="fas ' + durumIcon + ' me-1"></i>' + (t.durum === 'aktif' ? 'Aktif' : (t.durum === 'beklemede' ? 'Beklemede' : 'Pasif')) + '</span></td>' +
                    '<td class="small">' + sonCalismaText + '</td>' +
                    '<td>' +
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

    // Pagination render
    function paginationRender(toplamSayfa) {
        var $pagination = $('#talimatPagination');
        $pagination.empty();
        
        if (toplamSayfa <= 1) return;
        
        for (var i = 1; i <= Math.min(toplamSayfa, 5); i++) {
            $pagination.append('<li class="page-item ' + (i === aktifSayfa ? 'active' : '') + '"><a class="page-link" href="#" data-page="' + i + '">' + i + '</a></li>');
        }
        
        if (toplamSayfa > 5) {
            $pagination.append('<li class="page-item disabled"><span class="page-link">...</span></li>');
            $pagination.append('<li class="page-item"><a class="page-link" href="#" data-page="' + toplamSayfa + '">' + toplamSayfa + '</a></li>');
        }
        
        $pagination.find('.page-link').on('click', function(e) {
            e.preventDefault();
            var page = $(this).data('page');
            if (page) {
                aktifSayfa = page;
                talimatTablosunuRender();
            }
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

    $('#talimatKaydetBtn').on('click', function() {
        var yeniTalimat = {
            id: talimatlar.length + 1,
            ad: $('#talimatAd').val(),
            aciklama: $('#talimatAciklama').val(),
            tetikleyici: $('#talimatTetikleyici').val(),
            cron: $('#talimatCron').val() || '-',
            durum: 'aktif',
            sonCalisma: '-',
            prompt: $('#talimatPrompt').val()
        };
        
        if (!yeniTalimat.ad) {
            abp.message.warning('Lütfen talimat adını girin.', 'Uyarı');
            return;
        }
        
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
                    '<p><strong>Talimat Adı:</strong> ' + talimat.ad + '</p>' +
                    '<p><strong>Açıklama:</strong> ' + talimat.aciklama + '</p>' +
                    '<p><strong>Tetikleyici:</strong> ' + (talimat.tetikleyici === 'zamanli' ? 'Zamanlı' : (talimat.tetikleyici === 'olay' ? 'Olay Tabanlı' : 'Manuel')) + '</p>' +
                    '<p><strong>Çalışma Zamanı:</strong> ' + (talimat.cron !== '-' ? talimat.cron : '—') + '</p>' +
                    '<p><strong>Durum:</strong> ' + (talimat.durum === 'aktif' ? 'Aktif' : (talimat.durum === 'beklemede' ? 'Beklemede' : 'Pasif')) + '</p>' +
                    '<p><strong>Son Çalışma:</strong> ' + talimat.sonCalisma + '</p>' +
                    '<p><strong>AI Prompt:</strong></p>' +
                    '<pre class="small bg-light p-2 rounded">' + talimat.prompt + '</pre>' +
                '</div>'
            );
            var detayModal = new bootstrap.Modal(document.getElementById('talimatDetayModal'));
            detayModal.show();
        }
        else if (aksiyon === 'calistir') {
            abp.message.info('"' + talimat.ad + '" talimatı çalıştırılıyor...', 'Talimat Çalıştırılıyor');
            setTimeout(function() {
                talimat.sonCalisma = new Date().toISOString().slice(0,10).replace('T',' ') + ' ' + new Date().toTimeString().slice(0,8);
                talimatTablosunuRender();
                abp.message.success('"' + talimat.ad + '" talimatı başarıyla çalıştırıldı.', 'Başarılı');
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

    // Başlangıç
    kpiGuncelle();
    talimatTablosunuRender();

}); 