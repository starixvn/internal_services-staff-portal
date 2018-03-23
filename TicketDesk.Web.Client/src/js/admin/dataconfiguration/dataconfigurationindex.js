var dataconfiguration = {
    init: function () {
        dataconfiguration.transformForm();
    },

    itemCreatedMessage: function () {
        return $("#item-created-message").text();
    },

    itemUpdatedMessage: function () {
        return $("#item-updated-message").text();
    },

    transformForm: function () {
        $("#dynamic-modals").on("click",
            ".submit",
            function (e) {
                e.preventDefault();
                var action = $(this).data("action");
                if (action === undefined) {
                    $(this).closest("form").trigger("submit");
                } else {
                    $(this).closest("form").prop("action", action);
                    $(this).closest("form").data("type", "create-and-continue");

                    $(this).closest("form").trigger("submit");
                }
            });

        $("#dynamic-modals").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                dataconfiguration.ajaxForm($(this));
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
                        dataconfiguration.initValidation();
                    }
                });
            });

        $(".filter-form").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                var selectedFieldId = $("#SelectedFieldId").val();
                var modalTitle = $("#SelectedFieldId option:selected").text();
                $.ajax({
                    url: $(this).attr("action"),
                    method: $(this).attr("method"),
                    data:
                        {
                            selectedFieldId: selectedFieldId,
                            modalTitle: modalTitle
                        },
                    success: function (result) {
                        $("#data-list").html(result);
                        dataconfiguration.initDatatable();
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
                        dataconfiguration.initValidation();
                    }
                });
            });
    },

    ajaxForm: function (form) {
        var formType = $(form).data("type");
        var isEditForm = formType === "edit";
        var isCreateAndContinue = formType === "create-and-continue";
        $.ajax({
            url: $(form).attr("action"),
            method: $(form).attr("method"),
            data: $(form).serialize(),
            success: function (result) {
                if (isEditForm) {
                    toastr.success(dataconfiguration.itemUpdatedMessage);
                } else {
                    toastr.success(dataconfiguration.itemCreatedMessage);
                }

                $("#dynamic-modals .modal").modal("hide");
                $(".modal-backdrop").remove();
                $("body").removeClass("modal-open");
                $(".filter-form form").trigger("submit");

                if (isCreateAndContinue) {
                    $("#dynamic-modals").html("");
                    $("#dynamic-modals").html(result);
                    $("#dynamic-modals .modal").modal();
                    dataconfiguration.initValidation();
                }
            }
        });
    },

    initValidation: function () {
        $.validator.unobtrusive.parse($("#dynamic-modals form"));
    },

    initDatatable: function () {
        var datatableConfigs = commonDataTableInit.Init();
        datatableConfigs.ordering = false;
        datatableConfigs.columnDefs = [
            {
                searchable: false,
                targets: [0, -1]
            },
            {
                width: "70px",
                targets: [0, -1, -2]
            },
            {
                width: "400px",
                targets: [2, 3]
            },
            {
                visible: false,
                targets: [1]
            }
        ];
        datatableConfigs.order = [[1, "asc"]];

        $("table").DataTable(datatableConfigs);
        floatThead.initFloatThead("table");
    }
}

$(document).ready(function () {
    dataconfiguration.init();
});