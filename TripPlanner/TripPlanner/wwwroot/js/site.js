/* =============================================
   SCROLL LISTENER
   ============================================= */
window.addEventListener('scroll', function () {
    const navbar = document.querySelector('.navbar');
    const hero = document.querySelector('.hero');

    if (window.scrollY > 10) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }

    if (hero) {
        const heroHeight = hero.offsetHeight;
        if (window.scrollY > heroHeight - 100) {
            document.body.classList.add('past-hero');
        } else {
            document.body.classList.remove('past-hero');
        }
    }
});