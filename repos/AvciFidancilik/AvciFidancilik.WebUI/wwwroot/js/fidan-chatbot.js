$(document).ready(function () {
    const AsistanZekasi = {
        sessionId: localStorage.getItem('chat_session_id') || 'session_' + Math.random().toString(36).substr(2, 9),

        mesajGonder: async function (mesajMetni, aksiyonDegeri = null) {
            // 1. URL ve Link Kontrolü (Hemen yönlendir)
            if (aksiyonDegeri && (aksiyonDegeri.startsWith('/') || aksiyonDegeri.startsWith('http'))) {
                window.location.href = aksiyonDegeri;
                return;
            }

            // 2. Input ve Mesaj Hazırlığı
            const temizMesaj = mesajMetni ? mesajMetni.trim() : "";

            // Kullanıcı balonunu sadece gerçek bir mesaj yazıldığında ekle
            if (temizMesaj !== "" && aksiyonDegeri !== "SCENARIO_MAIN" && (!aksiyonDegeri || !aksiyonDegeri.startsWith("SCENARIO_"))) {
                this.mesajEkle(temizMesaj, 'user');
                $('#asistan-input').val('');
            }

            // 3. "Yükleniyor..." Göstergesi
            const loadingId = 'loading-' + Date.now();
            $('#asistan-mesajlar').append(`<div class="mesaj bot" id="${loadingId}">...</div>`);
            const mesajAlani = $('#asistan-mesajlar');
            mesajAlani.scrollTop(mesajAlani[0].scrollHeight);
            $('#asistan-secenekler').empty();

            try {
                const response = await fetch('/api/Chat/SendMessage', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        userId: 3,
                        Message: temizMesaj,
                        ActionValue: aksiyonDegeri
                    })
                });

                // Yanıt geldiğinde yükleniyor yazısını kaldır
                $(`#${loadingId}`).remove();

                if (!response.ok) throw new Error("Sunucu hatası");

                const data = await response.json();
                const botMesaj = data.message || data.Message || "";
                const butonMetinleri = data.buttons || data.Buttons || [];
                const butonAksiyonlari = data.buttonActions || data.ButtonActions || [];

                // 4. Bot Cevabını Yaz
                if (botMesaj) {
                    // Mesajı formatlayarak göster
                    let formatliMesaj = botMesaj
                        .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
                        .replace(/\n/g, '<br>');

                    this.mesajEkle(formatliMesaj, 'bot');
                }

                // 5. Butonları Getir
                this.butonlariOlustur(butonMetinleri, butonAksiyonlari);

            } catch (error) {
                $(`#${loadingId}`).remove();
                console.error("Chat Hatası:", error);
                this.mesajEkle("Bağlantı hatası oluştu. Lütfen tekrar deneyin. 🍃", 'bot');
            }
        },

        mesajEkle: function (metin, tip) {
            const mesajAlani = $('#asistan-mesajlar');
            if (!metin || mesajAlani.length === 0) return;

            mesajAlani.append(`<div class="mesaj ${tip}">${metin}</div>`);
            // Otomatik aşağı kaydır
            mesajAlani.stop().animate({ scrollTop: mesajAlani[0].scrollHeight }, 500);
        },

        butonlariOlustur: function (metinler, aksiyonlar) {
            const secenekAlani = $('#asistan-secenekler');
            secenekAlani.empty();

            if (metinler && metinler.length > 0) {
                metinler.forEach((metin, index) => {
                    const aksiyon = aksiyonlar[index] || metin;
                    const btn = $('<button class="opt-btn">' + metin + '</button>');
                    btn.on('click', (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        this.mesajGonder(metin, aksiyon);
                    });
                    secenekAlani.append(btn);
                });
            }
        }
    };

    // --- TETİKLEYİCİLER ---

    // 🟢 Pencere Açılış Kontrolü
    $(document).on('click', '#asistan-bar, .asistan-trigger', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const pencere = $('#asistan-pencere');
        const mesajAlani = $('#asistan-mesajlar');

        // Pencere zaten açık mı kontrol et
        if (pencere.hasClass('hidden')) {
            pencere.removeClass('hidden'); // Pencereyi aç

            // Eğer içerik boşsa menüyü zorla getir
            if (mesajAlani.find('.mesaj').length === 0) {
                AsistanZekasi.mesajGonder(null, "SCENARIO_MAIN");
            }

            // Input'a focus ol
            setTimeout(() => {
                $('#asistan-input').focus();
            }, 100);
        } else {
            pencere.addClass('hidden'); // Pencereyi kapat
        }
    });

    // Kapat butonu
    $(document).on('click', '#asistan-kapat', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#asistan-pencere').addClass('hidden');
    });

    // Pencere dışına tıklandığında kapat
    $(document).on('click', function (e) {
        const pencere = $('#asistan-pencere');
        const asistanBar = $('#asistan-bar, .asistan-trigger');

        if (!pencere.hasClass('hidden') &&
            !pencere.is(e.target) &&
            pencere.has(e.target).length === 0 &&
            !asistanBar.is(e.target) &&
            asistanBar.has(e.target).length === 0) {
            pencere.addClass('hidden');
        }
    });

    // Mesaj gönderme butonu
    $(document).on('click', '#asistan-gonder', function () {
        const metin = $('#asistan-input').val().trim();
        if (metin) {
            AsistanZekasi.mesajGonder(metin);
        }
    });

    // Enter tuşu ile gönderme
    $('#asistan-input').on('keypress', function (e) {
        if (e.key === 'Enter') {
            const metin = $(this).val().trim();
            if (metin) {
                AsistanZekasi.mesajGonder(metin);
            }
        }
    });

    // Input focus efekti
    $('#asistan-input').on('focus', function () {
        $(this).css({
            'border-color': '#28a745',
            'box-shadow': '0 0 0 0.2rem rgba(40, 167, 69, 0.25)'
        });
    }).on('blur', function () {
        $(this).css({
            'border-color': '#ddd',
            'box-shadow': 'none'
        });
    });

    // Sayfa yüklendiğinde bazı hazırlıklar
    $(function () {
        // Mesaj alanına smooth scroll ekle
        $('#asistan-mesajlar').addClass('smooth-scroll');

        // Asistan butonuna hover efekti
        $('#asistan-bar').hover(
            function () {
                $(this).css({
                    'transform': 'translateY(-3px)',
                    'box-shadow': '0 8px 25px rgba(40, 167, 69, 0.4)'
                });
            },
            function () {
                $(this).css({
                    'transform': 'translateY(0)',
                    'box-shadow': '0 4px 15px rgba(40, 167, 69, 0.4)'
                });
            }
        );

        // Pencere animasyonu için class ekle
        $('#asistan-pencere').addClass('animate__animated');
    });
});