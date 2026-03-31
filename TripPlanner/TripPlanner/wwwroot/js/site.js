/* =============================================
   SCROLL LISTENER
   ============================================= */
window.addEventListener('scroll', function () {
    const navbar = document.querySelector('.navbar');
    const hero = document.querySelector('.hero');

    const isInnerPage = document.querySelector('.itinerary-page, .admin-dashboard, .page-container');
    if (isInnerPage) return;

    if (window.scrollY > 10) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }

    if (hero && !isInnerPage) {
        const heroHeight = hero.offsetHeight;
        if (window.scrollY > heroHeight - 100) {
            document.body.classList.add('past-hero');
        } else {
            document.body.classList.remove('past-hero');
        }
    }
});

/* =============================================
   TAB SWITCHER
   ============================================= */
function showTab(tabId) {
    let tabs = document.querySelectorAll(".tab-content");
    let buttons = document.querySelectorAll(".tab");
    tabs.forEach(t => t.classList.remove("active"));
    buttons.forEach(b => b.classList.remove("active"));
    document.getElementById(tabId).classList.add("active");
    event.target.classList.add("active");
}