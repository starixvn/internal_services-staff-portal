var mail = {
    init: function () {
        mail.filterForm();
        mail.handleSave();
    },

    initTag: function () {
        $(".tag").tagEditor({
            trimValue: true,
            delimiter: ";, "
        });
    },

    filterForm: function () {
        $(".filter-form form").on("change",
            "select",
            function () {
                $(".filter-form form").trigger("submit");
            });

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
                        mail.initTag();
                    }
                });
            });

        $(".filter-form form").trigger("submit");
    },

    handleSave: function () {
        $("#data-list").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                $.ajax({
                    url: $(this).attr("action"),
                    method: $(this).attr("method"),
                    data: $(this).serialize(),
                    success: function (result) {
                        $("#data-list").html(result);
                        mail.initTag();
                        var isSaveSuccess = $("#is-save-success").val();
                        if (isSaveSuccess) {
                            toastr.success("Email settings saved");
                        }
                    }
                });
            });
    }
}

$(document).ready(function () {
    mail.init();
})