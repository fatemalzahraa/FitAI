// --- 1. GENEL SİTE FONKSİYONLARI ---
function updateCartCount() {
    const cartBadge = document.querySelector('.cart-badge');
    if (!cartBadge) return;

    fetch('/api/cart/count')
        .then(res => {
            if (!res.ok) throw new Error("Network response was not ok");
            return res.json();
        })
        .then(data => {
            cartBadge.textContent = data.count || '0';
        })
        .catch(err => {
            console.warn('Sepet sayısı çekilemedi:', err);
        });
}

// --- 2. SAYFA YÜKLENDİĞİNDE ÇALIŞACAKLAR ---
document.addEventListener('DOMContentLoaded', function () {

    // Sepet sayısını güncelle
    updateCartCount();

    // Arama Çubuğu Kontrolü
    const searchToggle = document.querySelector('.nav-icon[href="#"]');
    const searchBarContainer = document.querySelector('.search-bar-container');
    const sidebarOverlay = document.querySelector('.sidebar-overlay');

    if (searchToggle && searchBarContainer) {
        searchToggle.addEventListener('click', function (e) {
            e.preventDefault();
            const isOpen = searchBarContainer.style.display === 'block';

            if (!isOpen) {
                searchBarContainer.style.display = 'block';
                if (sidebarOverlay) sidebarOverlay.classList.add('active');
                document.body.style.overflow = 'hidden';
            } else {
                searchBarContainer.style.display = 'none';
                if (sidebarOverlay) sidebarOverlay.classList.remove('active');
                document.body.style.overflow = '';
            }
        });
    }

    // Overlay'e tıklandığında aramayı kapat (Kullanıcı deneyimi için ekledim)
    if (sidebarOverlay) {
        sidebarOverlay.addEventListener('click', function () {
            if (searchBarContainer) searchBarContainer.style.display = 'none';
            sidebarOverlay.classList.remove('active');
            document.body.style.overflow = '';
        });
    }
});