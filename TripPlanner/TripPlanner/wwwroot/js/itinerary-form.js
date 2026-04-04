document.querySelector('.navbar')?.classList.add('scrolled');

const startDate = document.querySelector('input[name="StartDate"]');
const endDate = document.querySelector('input[name="EndDate"]');

if (startDate && endDate) {
    function validateDates() {
        if (startDate.value && endDate.value) {
            if (new Date(endDate.value) < new Date(startDate.value)) {
                alert('End date cannot be before start date');
                endDate.value = '';
            }
        }
    }
    startDate.addEventListener('change', validateDates);
    endDate.addEventListener('change', validateDates);
}