document.querySelector('.navbar')?.classList.add('scrolled');

const form = document.querySelector('form');
const startDate = document.querySelector('input[name="StartDate"]');
const endDate = document.querySelector('input[name="EndDate"]');
const validationSummary = document.querySelector('.validation-summary-errors, [data-valmsg-summary]');

if (form && startDate && endDate) {
    form.addEventListener('submit', function (e) {
        const errors = [];
        const start = new Date(startDate.value);
        const end = new Date(endDate.value);

        if (startDate.value && (start.getFullYear() < 2000 || start.getFullYear() > 3000))
            errors.push('Start date must be between year 2000 and 3000.');

        if (endDate.value && (end.getFullYear() < 2000 || end.getFullYear() > 3000))
            errors.push('End date must be between year 2000 and 3000.');

        if (startDate.value && endDate.value && end <= start)
            errors.push('End date cannot be before start date.');

        if (errors.length > 0) {
            e.preventDefault();
            if (validationSummary) {
                validationSummary.innerHTML = '<ul>' + errors.map(err => `<li>${err}</li>`).join('') + '</ul>';
                validationSummary.style.display = 'block';
            }
        }
    });
}