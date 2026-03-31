document.addEventListener('DOMContentLoaded', () => {
    const itineraryId = window.__itineraryId;

    document.querySelectorAll('.btn-edit').forEach(btn => {
        btn.addEventListener('click', () => {
            const card = btn.closest('.location-item');
            card.querySelector('.view-mode').style.display = 'none';
            card.querySelector('.edit-mode').style.display = 'block';
        });
    });

    document.querySelectorAll('.btn-cancel-edit').forEach(btn => {
        btn.addEventListener('click', () => {
            const card = btn.closest('.location-item');
            card.querySelector('.view-mode').style.display = 'block';
            card.querySelector('.edit-mode').style.display = 'none';
        });
    });

    document.querySelectorAll('.btn-save-edit').forEach(btn => {
        btn.addEventListener('click', async () => {
            const card = btn.closest('.location-item');
            const itineraryItemId = parseInt(card.dataset.itemId);
            const stopOrder = parseInt(card.dataset.stopOrder);
            const startDateTime = card.querySelector('.input-start').value;
            const endDateTime = card.querySelector('.input-end').value;

            const response = await fetch(`/itineraries/${itineraryId}/items/${itineraryItemId}`, {
                method: 'PUT',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({
                    startDateTime,
                    endDateTime,
                    stopOrder,
                    note: null
                })
            });

            if (response.ok) location.reload();
        });
    });

    document.querySelectorAll('.btn-remove').forEach(btn => {
        btn.addEventListener('click', async () => {
            const card = btn.closest('.location-item');
            const itineraryItemId = card.dataset.itemId;

            await fetch(`/itineraries/${itineraryId}/items/${itineraryItemId}`, {
                method: 'DELETE'
            });

            location.reload();
        });
    });

    const attractionsList = document.querySelector('.locations-list');
    if (!attractionsList) return;

    let draggedCard = null;
    const placeholder = document.createElement('div');
    placeholder.className = 'drag-placeholder';

    document.querySelectorAll('.location-item').forEach(card => {
        card.querySelector('.drag-handle').addEventListener('mousedown', (e) => {
            // To prevent the broswer select any words
            e.preventDefault();
            draggedCard = card;
            const rect = card.getBoundingClientRect();

            card.classList.add('dragging');
            card.style.width = rect.width + 'px';
            card.style.position = 'fixed';
            card.style.top = rect.top + 'px';
            card.style.left = rect.left + 'px';
            card.style.zIndex = '1000';
            card.style.opacity = '0.8';
            card.style.pointerEvents = 'none';

            placeholder.style.height = rect.height + 'px';
            card.parentNode.insertBefore(placeholder, card);

            document.addEventListener('mousemove', onMouseMove);
            document.addEventListener('mouseup', onMouseUp);
        });
    });

    function onMouseMove(e) {
        if (!draggedCard) return;
        draggedCard.style.top = e.clientY - draggedCard.offsetHeight / 2 + 'px';

        const cards = [...attractionsList.querySelectorAll('.location-item:not(.dragging)')];
        const closest = cards.reduce((closest, child) => {
            const box = child.getBoundingClientRect();
            const offset = e.clientY - box.top - box.height / 2;
            if (offset < 0 && offset > closest.offset) return {offset, element: child};
            return closest;
        }, {offset: Number.NEGATIVE_INFINITY}).element;

        if (closest) attractionsList.insertBefore(placeholder, closest);
        else attractionsList.appendChild(placeholder);
    }

    async function onMouseUp() {
        if (!draggedCard) return;

        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);

        draggedCard.style.cssText = '';
        draggedCard.classList.remove('dragging');
        attractionsList.insertBefore(draggedCard, placeholder);
        placeholder.remove();

        const itemIds = [...attractionsList.querySelectorAll('.location-item')]
            .map(c => parseInt(c.dataset.itemId));

        await fetch(`/itineraries/${itineraryId}/items/reorder`, {
            method: 'PUT',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(itemIds)
        });

        draggedCard = null;
        location.reload();
    }
});