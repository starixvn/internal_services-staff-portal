var customfield = {
    init: function () {
        customfield.transformForm();
    },

    itemCreatedMessage: function () {
        return $("#item-created-message").text();
    },

    itemUpdatedMessage: function () {
        return $("#item-updated-message").text();
    },

    transformForm: function () {
        $("#dynamic-modals").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                customfield.ajaxForm($(this));
            });

        $(".table").on("click",
            ".create",
            function (e) {
                e.preventDefault();
                e.stopPropagation();
                var href = $(this).attr("href");
                $.ajax({
                    url: href,
                    method: "get",
                    success: function (result) {
                        $("#dynamic-modals").html(result);
                        $("#dynamic-modals .modal").modal();
                        customfield.initValidation();
                    }
                });
            });

        $(".filter-form").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                var ticketTypeId = $("#SelectedTicketCategoryType").val();
                $.ajax({
                    url: $(this).attr("action"),
                    method: $(this).attr("method"),
                    data: { ticketTypeId: ticketTypeId },
                    success: function (result) {
                        $("#data-list").html(result);
                        customfield.initDatatable();
                        sidebar.initSelect2();
                    }
                });
            });

        $(".table").on("click",
            ".edit",
            function () {
                var href = $(this).data("href");
                $.ajax({
                    url: href,
                    success: function (result) {
                        $("#dynamic-modals").html("");
                        $("#dynamic-modals").html(result);
                        $("#dynamic-modals .modal").modal();
                        customfield.initValidation();
                    }
                });
            });
    },

    ajaxForm: function (form) {
        var formType = $(form).data("type");
        var isEditForm = formType === "edit";
        $.ajax({
            url: $(form).attr("action"),
            method: $(form).attr("method"),
            data: $(form).serialize(),
            success: function (result) {
                if (isEditForm) {
                    toastr.success(customfield.itemUpdatedMessage);
                } else {
                    toastr.success(customfield.itemCreatedMessage);
                }

                $("#dynamic-modals .modal").modal("hide");
                $(".modal-backdrop").remove();
                $("body").removeClass("modal-open");
                $(".filter-form form").trigger("submit");
            }
        });
    },

    initValidation: function () {
        $.validator.unobtrusive.parse($("#dynamic-modals form"));
    },

    initDatatable: function () {
        var datatableConfigs = commonDataTableInit.Init();
        datatableConfigs.ordering = false;
        datatableConfigs.order = [[1, "asc"]];

        $("table").DataTable(datatableConfigs);
        floatThead.initFloatThead("table");
    }
}

$(document).ready(function () {
    customfield.init();
});