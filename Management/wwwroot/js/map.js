function initMap() {
    // Rastgele bir konum belirliyoruz (Örnek: İstanbul)
    var location = { lat: 41.0082, lng: 28.9784 };

    // Harita objesini oluşturuyoruz
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 13,
        center: location
    });

    // Haritaya bir işaretçi ekliyoruz
    var marker = new google.maps.Marker({
        position: location,
        map: map
    });
}
