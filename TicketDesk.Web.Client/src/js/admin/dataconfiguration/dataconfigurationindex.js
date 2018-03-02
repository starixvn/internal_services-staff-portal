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

        $(".create").on("click",
            function(e) {
                e.preventDefault();
                e.stopPropagation();
                var href = $(this).attr("href");
                $.ajax({
                    url: href,
                    method: "get",
                    success: function (result) {
                        $("#dynamic-modals").html(result);
                        $("#dynamic-modals .modal").modal();
                    }
                });
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

$(document).ready(function() {
    dataconfiguration.init();
});