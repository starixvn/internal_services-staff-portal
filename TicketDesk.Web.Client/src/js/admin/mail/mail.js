var mail = {
    init: function () {
        mail.filterForm();
        mail.handleSave();
    },

    initTag: function() {
        $(".tag").tagsinput();
    },

    checkMailFormat: function(params) {
        // Don't offset to create a tag if there is no @ symbol
        if (params.term.indexOf("@") === -1) {
            // Return null to disable tag creation
            return null;
        }

        return {
            id: params.term,
            text: params.term
        }
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