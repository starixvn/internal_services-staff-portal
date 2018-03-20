var mail = {
    init: function () {
        mail.filterForm();
        mail.handleSave();
    },

    filterForm: function() {
        $(".filter-form").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                $.ajax({
                    url: $(this).attr("action"),
                    method: $(this).attr("method"),
                    data: $(this).serialize(),
                    success: function (result) {
                        $("#data-list").html(result);
                    }
                });
            });
    },

    handleSave: function () {
        $("#mail-settings").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                $.ajax({
                    url: $(this).attr("action"),
                    method: $(this).attr("method"),
                    data: $(this).serialize(),
                    success: function () {
                        toastr.success("Mail Settings Saved");
                        $(".filter-form form").trigger("submit");
                    }
                });
            });
    }
}

$(document).ready(function () {
    mail.init();
})