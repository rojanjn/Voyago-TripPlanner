async function initMap() {
    const itineraryId = window.__itineraryId;
    const locations = window.__locations || [];

    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 10,
        center: {lat: 43.6532, lng: -79.3832}
    });

    // Place markers
    locations.forEach(loc => {
        new google.maps.Marker({
            position: {lat: parseFloat(loc.lat), lng: parseFloat(loc.lng)},
            map: map,
            title: loc.name
        });
    });

    // Fit map to markers
    if (locations.length > 0) {
        const bounds = new google.maps.LatLngBounds();
        locations.forEach(loc => bounds.extend({lat: parseFloat(loc.lat), lng: parseFloat(loc.lng)}));
        map.fitBounds(bounds);
    }

    // Fetch and draw route if there are 2+ locations
    if (locations.length >= 2) {
        const response = await fetch(`/Itinerary/GetRoute?id=${itineraryId}`);
        if (response.ok) {
            const route = await response.json();

            const decodedPath = google.maps.geometry.encoding.decodePath(route.polyline);
            new google.maps.Polyline({
                path: decodedPath,
                geodesic: true,
                strokeColor: '#4A90D9',
                strokeOpacity: 1.0,
                strokeWeight: 4,
                map: map
            });

            route.legs.forEach((leg, i) => {
                const connector = document.querySelector(`.leg-connector[data-leg-index="${i}"]`);
                if (connector) {
                    connector.querySelector('.leg-distance').textContent = leg.distance;
                    connector.querySelector('.leg-duration').textContent = leg.duration;
                }
            });

            const summary = document.getElementById('routeSummary');
            if (summary) {
                document.getElementById('totalDistance').textContent = route.totalDistance;
                document.getElementById('totalDuration').textContent = route.totalDuration;
                summary.style.display = 'flex';
            }
        }
    }

    let selectedPlace = null;
    const searchInput = document.getElementById('attractionSearch');
    const btnAdd = document.getElementById('add-location');

    const {Autocomplete} = await google.maps.importLibrary("places");
    const autocomplete = new Autocomplete(searchInput, {
        fields: ['place_id', 'name', 'formatted_address', 'geometry']
    });

    autocomplete.addListener('place_changed', () => {
        const place = autocomplete.getPlace();
        if (!place.geometry) return;

        selectedPlace = {
            placeId: place.place_id,
            name: place.name,
            address: place.formatted_address,
            latitude: place.geometry.location.lat(),
            longitude: place.geometry.location.lng()
        };

        btnAdd.disabled = false;
    });

    btnAdd.addEventListener('click', async () => {
        if (!selectedPlace) return;

        await fetch(`/itineraries/${itineraryId}/items`, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({
                location: {
                    placeId: selectedPlace.placeId,
                    name: selectedPlace.name,
                    address: selectedPlace.address,
                    latitude: selectedPlace.latitude,
                    longitude: selectedPlace.longitude
                },
                note: null
            })
        });

        location.reload();
    });
}

window.initMap = initMap;