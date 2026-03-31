const CARDS_PER_PAGE = 4;
const pages = { current: 0, past: 0 };

function initPagination(tabId) {
    const grid = document.getElementById(tabId + '-grid');
    const cards = Array.from(grid.querySelectorAll('.itinerary-card'));
    const controls = document.getElementById(tabId + '-pagination');
    controls.style.display = cards.length <= CARDS_PER_PAGE ? 'none' : 'flex';
    renderPage(tabId);
}

function renderPage(tabId) {
    const grid = document.getElementById(tabId + '-grid');
    const cards = Array.from(grid.querySelectorAll('.itinerary-card'));
    const total = Math.ceil(cards.length / CARDS_PER_PAGE);
    const page = pages[tabId];

    cards.forEach((card, i) => {
        card.style.display = (i >= page * CARDS_PER_PAGE && i < (page + 1) * CARDS_PER_PAGE) ? '' : 'none';
    });

    document.getElementById(tabId + '-indicator').textContent = total > 0 ? `${page + 1} / ${total}` : '';
    document.getElementById(tabId + '-prev').disabled = page === 0;
    document.getElementById(tabId + '-next').disabled = page >= total - 1;
}

function changePage(tabId, dir) {
    const grid = document.getElementById(tabId + '-grid');
    const cards = grid.querySelectorAll('.itinerary-card').length;
    const total = Math.ceil(cards / CARDS_PER_PAGE);
    pages[tabId] = Math.max(0, Math.min(pages[tabId] + dir, total - 1));
    renderPage(tabId);
}

document.addEventListener('DOMContentLoaded', () => {
    initPagination('current');
    initPagination('past');
});