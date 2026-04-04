document.querySelector('.navbar')?.classList.add('scrolled');

const startDate = document.querySelector('input[name="StartDate"]');
const endDate = document.querySelector('input[name="EndDate"]');

if (startDate && endDate) {
    function validateDates() {
        const start = new Date(startDate.value);
        const end = new Date(endDate.value);
        const startYear = start.getFullYear();
        const endYear = end.getFullYear();
        
        if (startDate.value && (startYear < 2000 || startYear > 3000)) {
            alert('Start date must be between year 2000 and 3000.');
            startDate.value = '';
            return;
        }

        if (endDate.value && (endYear < 2000 || endYear > 3000)) {
            alert('End date must be between year 2000 and 3000.');
            endDate.value = '';
            return;
        }

        if (startDate.value && endDate.value) {
            if (end <= start) {
                alert('End date cannot be before start date.');
                endDate.value = '';
            }
        }
        
        
    }
    startDate.addEventListener('change', validateDates);
    endDate.addEventListener('change', validateDates);
}