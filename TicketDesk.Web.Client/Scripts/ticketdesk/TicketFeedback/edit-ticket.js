var ticketFeedback = (function ($) {
    function registerEventSubmit() {
        $("#form-feedback").on("submit", function (event) {
            event.preventDefault();

            var inputCustomFields = $('.custom-field');

            var ticket = {
                TicketId: $('input[name="Ticket.TicketId"]').val(),
                IsHtml: $('input[name="Ticket.IsHtml"]').val()
            }
            var formData = {
                Ticket: ticket,
                CustomFields: []
            }

            inputCustomFields.each(function (index, element) {
                var customField = {
                    CustomFieldId: $(this).data('customfieldid'),
                    Value: $(this).val()
                }
                formData.CustomFields.push(customField);
                
            });

            $.ajax({
                url: "/ticket/feedback/new",
                type: "post",
                data: formData,
                success: function (d) {
                    alert(d);
                }
            });
        });
    }

    function registerEventChangeRadioButton() {
        var radioCustomFields = $('input:radio.custom-field-radio');

        radioCustomFields.change(function () {
            if ($(this).is(':checked')) {
                var inputName = $(this).attr("name");
                $('input.custom-field[name=' + inputName + ']').val($(this).val());
            }
        });
    }

    function initialize() {
        registerEventSubmit();
        registerEventChangeRadioButton();
    }

    return {
        Initialize: initialize
    };
})(jQuery);

$(document).ready(function () {
    ticketFeedback.Initialize();
});