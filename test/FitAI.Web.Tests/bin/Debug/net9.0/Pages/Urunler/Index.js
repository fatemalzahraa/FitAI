// jQuery yüklendiğinde çalışacak fonksiyon
(function() {
    // jQuery'nin yüklenmesini bekle
    if (typeof $ === 'undefined') {
        console.log("jQuery henüz yüklenmedi, bekleniyor...");
        // 100ms aralıklarla jQuery kontrol et
        var checkInterval = setInterval(function() {
            if (typeof $ !== 'undefined') {
                console.log("jQuery yüklendi, başlatılıyor...");
                clearInterval(checkInterval);
                baslat();
            }
        }, 100);
        
        // 5 saniye timeout (sonsuz döngüye girmemek için)
        setTimeout(function() {
            clearInterval(checkInterval);
            if (typeof $ === 'undefined') {
                console.error("jQuery yüklenemedi!");
            }
        }, 5000);
    } else {
        baslat();
    }
    
    function baslat() {
        $(function() {
            console.log("Index.js başladı (jQuery hazır)");
            
            var tumUrunler = [];
            var duzenlenecekId = null;
            
            // =============================================
            // Yardımcı fonksiyonlar
            // =============================================
            function kesimBadge(kesim) {
                if (!kesim) return '<span class="text-muted">—</span>';
                return '<span class="kesim-badge">' + kesim + '</span>';
            }
            
            function kumasBadge(esnek) {
                if (esnek === null || esnek === undefined) return '<span class="text-muted">—</span>';
                return esnek
                    ? '<span class="kumas-esnek"><i class="fas fa-expand-arrows-alt me-1"></i>Esnek</span>'
                    : '<span class="kumas-sert"><i class="fas fa-minus me-1"></i>Standart</span>';
            }
            
            function kisalt(metin, limit) {
                if (!metin) return '<span class="text-muted">—</span>';
                return metin.length > limit
                    ? '<span title="' + metin + '">' + metin.substring(0, limit) + '...</span>'
                    : metin;
            }
            
            // =============================================
            // KPI Güncelleme
            // =============================================
            function kpiGuncelle() {
                var esnek = tumUrunler.filter(function(u) { return u.kumasEsnek === true; }).length;
                var kesimler = [...new Set(tumUrunler.filter(function(u) { return u.kesimTuru; }).map(function(u) { return u.kesimTuru; }))].length;
                
                $('#toplamUrun').text(tumUrunler.length);
                $('#aktifUrun').text(tumUrunler.length);
                $('#eskekUrun').text(esnek);
                $('#kesimSayisi').text(kesimler);
            }
            
            // =============================================
            // Tablo Render
            // =============================================
            function tabloYenile() {
                var arama = $('#aramaInput').val().toLowerCase();
                var kesim = $('#kesimFiltre').val();
                var kumas = $('#kumasFiltre').val();
                
                var filtre = tumUrunler.filter(function(u) {
                    var aramaUyumu = !arama || u.ad.toLowerCase().includes(arama);
                    var kesimUyumu = !kesim || u.kesimTuru === kesim;
                    var kumasUyumu = !kumas ||
                        (kumas === 'esnek' && u.kumasEsnek === true) ||
                        (kumas === 'esnek_degil' && u.kumasEsnek === false);
                    return aramaUyumu && kesimUyumu && kumasUyumu;
                });
                
                var $tbody = $('#urunTablosu');
                $tbody.empty();
                
                if (filtre.length === 0) {
                    $tbody.html('<tr><td colspan="8" class="text-center py-4 text-muted">Sonuç bulunamadı.</td></tr>');
                    $('#toplamKayitYazi').text('0 sonuç');
                    return;
                }
                
                for (var i = 0; i < filtre.length; i++) {
                    var u = filtre[i];
                    $tbody.append(
                        '<tr>' +
                            '<td class="text-muted small">' + u.id + '</td>' +
                            '<td>' +
                                '<div class="urun-bilgi">' +
                                    '<div class="urun-ikon"><i class="fas fa-tshirt"></i></div>' +
                                    '<span class="urun-ad">' + (u.ad || '—') + '</span>' +
                                '</div>' +
                            '</td>' +
                            '<td class="text-muted small">' + (u.magazaAdi || '—') + '</td>' +
                            '<td>' + kesimBadge(u.kesimTuru) + '</td>' +
                            '<td>' + kumasBadge(u.kumasEsnek) + '</td>' +
                            '<td class="text-muted small">' + kisalt(u.aciklama, 40) + '</td>' +
                            '<td><span class="durum-aktif"><i class="fas fa-check me-1"></i>Aktif</span></td>' +
                            '<td class="text-center">' +
                                '<div class="d-flex gap-1 justify-content-center">' +
                                    '<button class="btn-islem btn-duzenle btn-duzenle-ac" data-id="' + u.id + '" title="Düzenle">' +
                                        '<i class="fas fa-pencil-alt"></i>' +
                                    '</button>' +
                                    '<button class="btn-islem btn-sil btn-sil-ac" data-id="' + u.id + '" data-ad="' + (u.ad || '') + '" title="Sil">' +
                                        '<i class="fas fa-trash"></i>' +
                                    '</button>' +
                                '</div>' +
                            '</td>' +
                        '</tr>'
                    );
                }
                
                $('#toplamKayitYazi').text(filtre.length + ' / ' + tumUrunler.length + ' ürün gösteriliyor');
            }
            
            // =============================================
            // VERİ ÇEKME
            // =============================================
            function yukleUrunler() {
                console.log("Ürünler yükleniyor...");
                
                fetch('/api/app/urun?maxResultCount=1000&skipCount=0', {
                    method: 'GET',
                   headers: {
    'Content-Type': 'application/json',
    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
}
                })
                .then(function(response) {
                    console.log("API Yanıt Kodu:", response.status);
                    if (!response.ok) {
                        throw new Error('HTTP ' + response.status);
                    }
                    return response.json();
                })
                .then(function(result) {
                    console.log("API'den gelen veri:", result);
                    
                    if (result && result.items) {
                        tumUrunler = result.items;
                    } else if (Array.isArray(result)) {
                        tumUrunler = result;
                    } else {
                        tumUrunler = [];
                    }
                    
                    console.log("Toplam ürün sayısı:", tumUrunler.length);
                    
                    // Yükleniyor mesajını kaldır
                    var $spinner = $('#urunTablosu .spinner-border');
                    if ($spinner.length) {
                        $spinner.parent().parent().remove();
                    }
                    
                    tabloYenile();
                    kpiGuncelle();
                })
                .catch(function(err) {
                    console.error("API Hatası:", err);
                    $('#urunTablosu').html(
                        '<tr><td colspan="8" class="text-center py-4 text-danger">' +
                        '<i class="fas fa-exclamation-circle fa-2x mb-2 d-block"></i>' +
                        'API bağlantı hatası!<br>' +
                        '<span class="small text-muted">' + err.message + '</span>' +
                        '</td></tr>'
                    );
                });
            }
            
            // =============================================
            // Modal ve Form İşlemleri
            // =============================================
            function formTemizle() {
                $('#f_ad, #f_aciklama').val('');
                $('#f_magazaId, #f_kesimTuru').val('');
                $('#f_kumasEsnek').prop('checked', false);
                $('#aciklamaKarakter').text('0 karakter');
                $('#kesimBilgiKarti').hide();
                $('.is-invalid').removeClass('is-invalid');
            }
            
            // Eventler
            $('#yeniUrunBtn').on('click', function() {
                duzenlenecekId = null;
                formTemizle();
                $('#urunModalBaslik').html('<i class="fas fa-tshirt me-2 text-primary"></i>Yeni Ürün Ekle');
                new bootstrap.Modal(document.getElementById('urunModal')).show();
            });
            
            $(document).on('click', '.btn-duzenle-ac', function() {
                var id = $(this).data('id');
                var u = tumUrunler.find(function(x) { return x.id === id; });
                if (!u) return;
                
                duzenlenecekId = id;
                formTemizle();
                $('#urunModalBaslik').html('<i class="fas fa-tshirt me-2 text-warning"></i>Ürünü Düzenle');
                
                $('#f_ad').val(u.ad);
                $('#f_magazaId').val(u.magazaId);
                $('#f_kesimTuru').val(u.kesimTuru || '');
                $('#f_kumasEsnek').prop('checked', u.kumasEsnek === true);
                $('#f_aciklama').val(u.aciklama || '');
                $('#aciklamaKarakter').text((u.aciklama || '').length + ' karakter');
                
                new bootstrap.Modal(document.getElementById('urunModal')).show();
            });
            
            $('#urunKaydetBtn').on('click', function() {
                if (!$('#f_ad').val().trim()) {
                    abp.notify.warn('Ürün adı giriniz.');
                    return;
                }
                if (!$('#f_magazaId').val()) {
                    abp.notify.warn('Mağaza seçiniz.');
                    return;
                }
                
                var dto = {
                    magazaId: parseInt($('#f_magazaId').val()),
                    ad: $('#f_ad').val().trim(),
                    aciklama: $('#f_aciklama').val().trim() || null,
                    kesimTuru: $('#f_kesimTuru').val() || null,
                    kumasEsnek: $('#f_kumasEsnek').is(':checked')
                };
                
                var url = duzenlenecekId ? '/api/app/urun/' + duzenlenecekId : '/api/app/urun';
                var method = duzenlenecekId ? 'PUT' : 'POST';
                
                fetch(url, {
                    method: method,
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(dto)
                })
                .then(function(response) {
                    if (!response.ok) throw new Error('HTTP ' + response.status);
                    return response.json();
                })
                .then(function() {
                    abp.notify.success(duzenlenecekId ? 'Ürün güncellendi.' : 'Ürün eklendi.', 'Başarılı');
                    bootstrap.Modal.getInstance(document.getElementById('urunModal')).hide();
                    yukleUrunler();
                })
                .catch(function(err) {
                    console.error("Kayıt hatası:", err);
                    abp.notify.error('İşlem sırasında hata oluştu.');
                });
            });
            
            var silinecekId = null;
            
            $(document).on('click', '.btn-sil-ac', function() {
                silinecekId = $(this).data('id');
                $('#silUrunAdi').text($(this).data('ad'));
                new bootstrap.Modal(document.getElementById('silOnayModal')).show();
            });
            
            $('#silOnayBtn').on('click', function() {
                if (!silinecekId) return;
                
                fetch('/api/app/urun/' + silinecekId, {
                    method: 'DELETE'
                })
                .then(function(response) {
                    if (!response.ok) throw new Error('HTTP ' + response.status);
                    abp.notify.warn('Ürün silindi.', 'Silindi');
                    bootstrap.Modal.getInstance(document.getElementById('silOnayModal')).hide();
                    yukleUrunler();
                    silinecekId = null;
                })
                .catch(function(err) {
                    console.error("Silme hatası:", err);
                    abp.notify.error('Ürün silinirken hata oluştu.');
                });
            });
            
            // Filtreler
            $('#aramaInput').on('input', tabloYenile);
            $('#kesimFiltre, #kumasFiltre').on('change', tabloYenile);
            $('#filtreTemizle').on('click', function() {
                $('#aramaInput').val('');
                $('#kesimFiltre, #kumasFiltre').val('');
                tabloYenile();
            });
            
            // Sayfa yüklendiğinde verileri çek
            yukleUrunler();
        });
    }
})();