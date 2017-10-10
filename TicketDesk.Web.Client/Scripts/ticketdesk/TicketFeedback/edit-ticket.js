var ticketFeedback = (function ($) {
    function registerEventSubmit() {
        $("#form-feedback").on("submit", function (event) {
            event.preventDefault();

            var inputCustomFields = $('input.custom-field');

            for (var i = 0; i < inputCustomFields.length; i++) {
            }
        });
    }

    function initialize() {
        registerEventLogin();
    }

    return {
        Initialize: initialize
    };
})(jQuery);

$(document).ready(function () {
    ticketFeedback.Initialize();
});