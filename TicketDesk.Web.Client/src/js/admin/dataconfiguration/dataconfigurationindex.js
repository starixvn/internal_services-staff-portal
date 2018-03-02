var dataconfiguration = {
    init: function() {
        dataconfiguration.transformForm();
    },

    transformForm: function() {
        $("#dynamic-modals").on("submit",
            "form",
            function() {
                dataconfiguration.ajaxForm($(this));
            });
    },

    ajaxForm: function (form) {
        $.ajax({
            url: $(form).attr("action"),
            method: $(form).attr("method"),
            data: $(form).serialize(),
            success: function (result) {
                $("#dynamic-modals").html(result);
            }
        });
    }
}