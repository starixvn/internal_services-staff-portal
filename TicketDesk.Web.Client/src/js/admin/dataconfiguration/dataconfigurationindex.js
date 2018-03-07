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
        $("#dynamic-modals").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                dataconfiguration.ajaxForm($(this));
            });

        $("#dynamic-modals").on("click",
            ".submit-and-continue",
            function (e) {
                e.preventDefault();
                $("#dynamic-modals form").validate();
                var isFormValid = $("#dynamic-modals form").valid();
                if (isFormValid) {
                    $.ajax({
                        url: $(this).data("action"),
                        method: $("#dynamic-modals form").attr("method"),
                        data: $("#dynamic-modals form").serialize(),
                        success: function (result) {
                            toastr.success(dataconfiguration.itemCreatedMessage());
                            $("#dynamic-modals .modal").modal("hide");
                            $("body").removeClass("modal-open");
                            $(".modal-backdrop").remove();
                            $(".filter-form form").trigger("submit");
                            $("#dynamic-modals").html(result);
                            $("#dynamic-modals .modal").modal();
                            dataconfiguration.initValidation();
                        }
                    });
                }
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
                        $("#dynamic-modals").html(result);
                        $("#dynamic-modals .modal").modal();
                        dataconfiguration.initValidation();
                    }
                });
            });
    },

    ajaxForm: function (form) {
        var formType = $(form).find(":submit").data("type");
        var isEditForm = formType === "edit";
        $.ajax({
            url: $(form).attr("action"),
            method: $(form).attr("method"),
            data: $(form).serialize(),
            success: function (result) {
                if (result) {
                    if (isEditForm) {
                        toastr.success(dataconfiguration.itemUpdatedMessage);
                    } else {
                        toastr.success(dataconfiguration.itemCreatedMessage);
                    }
                    $("#dynamic-modals .modal").modal("hide");
                    $(".filter-form form").trigger("submit");
                }
            }
        });
    },

    initValidation: function () {
        $.validator.unobtrusive.parse($("#dynamic-modals form"));
    },

    initDatatable: function () {
        var datatableConfigs = commonDataTableInit.Init();
        datatableConfigs.columnDefs = [
            {
                orderable: false,
                searchable: false,
                targets: [0, -1]
            },
            {
                width: "70px",
                targets: [0, -1, -2]
            },
            {
                width: "400px",
                targets: [1, 2]
            }
        ];
        datatableConfigs.order = [[1, "asc"]];

        $("table").DataTable(datatableConfigs);
        commonDataTableInit.InitFixedHeaderDataTable();
    }
}

$(document).ready(function () {
    dataconfiguration.init();
});