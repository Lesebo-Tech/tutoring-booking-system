$(document).ready(function () {
    // Confirm before cancelling a booking
    $('form[action*="Cancelled"]').on('submit', function (e) {
        if (!confirm('Are you sure you want to cancel this booking?')) {
            e.preventDefault();
        }
    });
});