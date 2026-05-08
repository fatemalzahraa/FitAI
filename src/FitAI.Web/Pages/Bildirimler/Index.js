$(function () {
    // Mock Bildirim Verileri (entity Bildirim'e uygun)
    let tumBildirimler = [
        { id: 1, tip: 'NLP Uyarısı', baslik: 'Yorum Analizi Tamamlandı', icerik: 'SportZone TR ürünleri için NLP analizi bitti, 12 olumlu, 3 olumsuz yorum tespit edildi.', tarih: '2025-05-07T10:30:00', okunduMu: false },
        { id: 2, tip: 'Sync Hatası', baslik: 'Trendyol Senkronizasyon Hatası', icerik: 'Ürün güncellemesi başarısız, API limit aşımı.', tarih: '2025-05-06T15:45:00', okunduMu: false },
        { id: 3, tip: 'Yüksek İade Riski', baslik: 'Slim Fit Tayt için İade Riski', icerik: 'Üründe iade oranı %32, aksiyon alın.', tarih: '2025-05-05T09:00:00', okunduMu: true },
        { id: 4, tip: 'AI Etki Raporu', baslik: 'Haftalık AI Raporu', icerik: 'AI attribution %18 arttı, toplam komisyon ₺12.400.', tarih: '2025-05-04T12:00:00', okunduMu: false }
    ];

    let seciliIds = [];
    let aktifSayfa = 1;
    let sayfaBoyutu = 10;
    let aktifTip = '';
    let aktifDurum = '';
    let aramaKelime = '';

    // KPI hesapla
    function kpiGuncelle() {
        let toplam = tumBildirimler.length;
        let okunmamis = tumBildirimler.filter(b => !b.okunduMu).length;
        let buHafta = tumBildirimler.filter(b => {
            let tarih = new Date(b.tarih);
            let bugun = new Date();
            let haftaBas = new Date(bugun.setDate(bugun.getDate() - bugun.getDay()));
            return tarih >= haftaBas;
        }).length;
        let kritik = tumBildirimler.filter(b => b.tip === 'Yüksek İade Riski' || b.tip === 'Sync Hatası').length;

        $('#toplamBildirim').text(toplam);
        $('#okunmamisBildirim').text(okunmamis);
        $('#buHaftaBildirim').text(buHafta);
        $('#kritikBildirim').text(kritik);
    }

    // Tip badge CSS sınıfı
    function tipBadge(tip) {
        let cls = '';
        if (tip === 'NLP Uyarısı') cls = 'tip-nlp';
        else if (tip === 'Sync Hatası') cls = 'tip-sync';
        else if (tip === 'Yüksek İade Riski') cls = 'tip-ipade';
        else cls = 'tip-ai';
        return `<span class="tip-badge ${cls}">${tip}</span>`;
    }

    // Durum badge
    function durumBadge(okundu) {
        return okundu ? '<span class="durum-badge durum-okundu"><i class="fas fa-check-circle me-1"></i>Okundu</span>' 
                      : '<span class="durum-badge durum-okunmadi"><i class="fas fa-clock me-1"></i>Okunmadı</span>';
    }

    // Tarih formatlama
    function formatTarih(iso) {
        let d = new Date(iso);
        return d.toLocaleString('tr-TR');
    }

    // Filtreleme ve sıralama
    function filtrelenmisBildirimler() {
        let filtered = tumBildirimler.filter(b => {
            if (aktifTip && b.tip !== aktifTip) return false;
            if (aktifDurum === 'okundu' && !b.okunduMu) return false;
            if (aktifDurum === 'okunmadi' && b.okunduMu) return false;
            if (aramaKelime && !b.baslik.toLowerCase().includes(aramaKelime) && !b.icerik.toLowerCase().includes(aramaKelime)) return false;
            return true;
        });
        return filtered.sort((a,b) => new Date(b.tarih) - new Date(a.tarih));
    }

    // Tablo render
    function tabloRender() {
        let filtered = filtrelenmisBildirimler();
        let toplamSayfa = Math.ceil(filtered.length / sayfaBoyutu);
        let start = (aktifSayfa - 1) * sayfaBoyutu;
        let sayfaBildirimler = filtered.slice(start, start + sayfaBoyutu);
        let $tbody = $('#bildirimTablosu');
        $tbody.empty();

        if (sayfaBildirimler.length === 0) {
            $tbody.html('<tr><td colspan="7" class="text-center py-4 text-muted">Bildirim bulunamadı.</td></tr>');
            $('#bildirimSayac').text('0 bildirim');
            return;
        }

        sayfaBildirimler.forEach(b => {
            let satir = $('<tr>');
            satir.data('id', b.id);
            satir.append(`<td><div class="form-check"><input class="form-check-input bildirim-check" type="checkbox" value="${b.id}"></div></td>`);
            satir.append(`<td>${tipBadge(b.tip)}</td>`);
            satir.append(`<td><strong>${b.baslik}</strong></td>`);
            satir.append(`<td>${b.icerik.length > 60 ? b.icerik.substring(0,60)+'...' : b.icerik}</td>`);
            satir.append(`<td class="small">${formatTarih(b.tarih)}</td>`);
            satir.append(`<td>${durumBadge(b.okunduMu)}</td>`);
            satir.append(`<td><button class="btn btn-sm btn-outline-secondary bildirim-detay" data-id="${b.id}"><i class="fas fa-eye"></i></button> ${!b.okunduMu ? '<button class="btn btn-sm a-okundu okundu-isaretle" data-id="'+b.id+'"><i class="fas fa-check"></i></button>' : ''}</td>`);
            satir.click(function(e) {
                if (!$(e.target).closest('.form-check, button').length) {
                    $('.bildirim-detay[data-id="'+b.id+'"]').click();
                }
            });
            $tbody.append(satir);
        });
        $('#bildirimSayac').text(`${filtered.length} bildirim gösteriliyor`);
        paginationRender(toplamSayfa);
    }

    function paginationRender(toplamSayfa) {
        let $pagination = $('#bildirimPagination');
        $pagination.empty();
        if (toplamSayfa <= 1) return;
        for (let i = 1; i <= Math.min(toplamSayfa, 5); i++) {
            $pagination.append(`<li class="page-item ${i === aktifSayfa ? 'active' : ''}"><a class="page-link" href="#" data-page="${i}">${i}</a></li>`);
        }
        if (toplamSayfa > 5) $pagination.append('<li class="page-item disabled"><span class="page-link">...</span></li>').append(`<li class="page-item"><a class="page-link" href="#" data-page="${toplamSayfa}">${toplamSayfa}</a></li>`);
        $pagination.find('.page-link').on('click', function(e) {
            e.preventDefault();
            let page = $(this).data('page');
            if (page) { aktifSayfa = page; tabloRender(); }
        });
    }

    // Detay modal
    $(document).on('click', '.bildirim-detay', function() {
        let id = $(this).data('id');
        let bildirim = tumBildirimler.find(b => b.id === id);
        if (!bildirim) return;
        $('#bildirimDetayIcerik').html(`
            <div class="mb-2"><strong>Tip:</strong> ${tipBadge(bildirim.tip)}</div>
            <div class="mb-2"><strong>Başlık:</strong> ${bildirim.baslik}</div>
            <div class="mb-2"><strong>İçerik:</strong><br>${bildirim.icerik}</div>
            <div class="mb-2"><strong>Gönderim Tarihi:</strong> ${formatTarih(bildirim.tarih)}</div>
            <div><strong>Durum:</strong> ${durumBadge(bildirim.okunduMu)}</div>
        `);
        $('#modalOkunduBtn').data('id', id);
        let modal = new bootstrap.Modal(document.getElementById('bildirimDetayModal'));
        modal.show();
        if (!bildirim.okunduMu) {
            $('#modalOkunduBtn').show();
        } else {
            $('#modalOkunduBtn').hide();
        }
    });

    // Okundu işaretle (detaydan)
    $('#modalOkunduBtn').on('click', function() {
        let id = $(this).data('id');
        let idx = tumBildirimler.findIndex(b => b.id === id);
        if (idx !== -1 && !tumBildirimler[idx].okunduMu) {
            tumBildirimler[idx].okunduMu = true;
            kpiGuncelle();
            tabloRender();
            bootstrap.Modal.getInstance(document.getElementById('bildirimDetayModal')).hide();
            abp.message.success('Bildirim okundu olarak işaretlendi.', 'Başarılı');
        }
    });

    // Tümünü Okundu İşaretle
    $('#markAllReadBtn').on('click', function() {
        let okunmamisIds = tumBildirimler.filter(b => !b.okunduMu).map(b => b.id);
        if (okunmamisIds.length === 0) { abp.message.warning('Zaten tüm bildirimler okunmuş.'); return; }
        okunmamisIds.forEach(id => {
            let idx = tumBildirimler.findIndex(b => b.id === id);
            if (idx !== -1) tumBildirimler[idx].okunduMu = true;
        });
        kpiGuncelle();
        tabloRender();
        abp.message.success(`${okunmamisIds.length} bildirim okundu işaretlendi.`, 'Başarılı');
    });

    // Toplu seçim
    $('#selectAll').on('change', function() {
        let isChecked = $(this).prop('checked');
        $('.bildirim-check').prop('checked', isChecked);
    });

    // Filtreler
    $('#tipFiltre, #durumFiltre, #aramaInput').on('change keyup', function() {
        aktifTip = $('#tipFiltre').val();
        aktifDurum = $('#durumFiltre').val();
        aramaKelime = $('#aramaInput').val().toLowerCase();
        aktifSayfa = 1;
        tabloRender();
    });
    $('#filtreTemizle').on('click', function() {
        $('#tipFiltre, #durumFiltre').val('');
        $('#aramaInput').val('');
        aktifTip = ''; aktifDurum = ''; aramaKelime = '';
        aktifSayfa = 1;
        tabloRender();
    });
    $('#yenileBtn').on('click', function() {
        // simülasyon - gerçekte API'dan yeniden çekilir
        kpiGuncelle();
        tabloRender();
    });

    // Başlat
    kpiGuncelle();
    tabloRender();
});