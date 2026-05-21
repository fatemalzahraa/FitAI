$(function () {

    // =============================================
    // ABP Servis Tanımlaması (Rota Çözücü Güvenlik Duvarı)
    // =============================================
    var _kullaniciService = {
        getList: function (params) {
            // Eğer standart proxy yolları çalışmıyorsa direkt ABP Ajax ile rotayı zorla
            var customUrl = '/api/app/kullanici'; 
            
            // Alternatif ABP rotalarını kontrol et ve eşleştir
            if (window.fitAI && fitAI.domain && fitAI.domain.users && fitAI.domain.users.kullanici) {
                return fitAI.domain.users.kullanici.getList(params);
            } else if (window.fitAI && fitAI.users && fitAI.users.kullanici) {
                return fitAI.users.kullanici.getList(params);
            }
            
            // Eğer hiçbir proxy nesnesi uymadıysa doğrudan API endpoint'ine istek at
            return abp.ajax({
                type: 'GET',
                url: customUrl,
                data: params
            });
        }
    };
});