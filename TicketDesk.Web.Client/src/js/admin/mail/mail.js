var mail = {
    init: function () {
        mail.filterForm();
        mail.handleSave();
        mail.preventInValidMail();
    },

    preventInValidMail: function() {
        $("#data-list").on("beforeItemAdd",
            ".tag",
            function (event) {
                if (!mail.checkMailFormat(event.item)) {
                    event.cancel = true;
                }
            });
    },

    initTag: function() {
        $(".tag").tagsinput({
            confirmKeys: [13, 44, 32, 188],
            trimValue: true,
            delimiter: ";",
            delimiterRegex: null
        });
    },

    checkMailFormat: function(email) {
        var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(String(email).toLowerCase());
    },

    filterForm: function () {
        $(".filter-form form").on("change",
            "select",
            function() {
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