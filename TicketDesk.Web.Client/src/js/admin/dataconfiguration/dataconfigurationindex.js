var dataconfiguration = {
    init: function() {
        dataconfiguration.transformForm();
    },

    transformForm: function() {
        $("#dynamic-modals").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                dataconfiguration.ajaxForm($(this));
            });

        $(".table").on("click",
            ".create",
            function(e) {
                e.preventDefault();
                e.stopPropagation();
                var href = $(this).attr("href");
                $.ajax({
                    url: href,
                    method: "get",
                    success: function(result) {
                        $("#dynamic-modals").html(result);
                        $("#dynamic-modals .modal").modal();
                        dataconfiguration.initValidation();
                    }
                });
            });

        $(".filter-form").on("submit",
            "form",
            function(e) {
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
    },

    ajaxForm: function (form) {
        $.ajax({
            url: $(form).attr("action"),
            method: $(form).attr("method"),
            data: $(form).serialize(),
            success: function (result) {
                if (result) {
                    toastr.info("New item added");
                    $("#dynamic-modals .modal").modal("hide");
                    $(".filter-form form").trigger("submit");
                }
            }
        });
    },

    initValidation: function() {
        $.validator.unobtrusive.parse($("#dynamic-modals form"));
    },

    initDatatable: function () {
        var datatableConfigs = commonDataTableInit.Init();
        datatableConfigs.columnDefs = [
            {
                orderable: false,
                searchable: false,
                targets: [0, -1]
            }
        ];
        datatableConfigs.order = [[1, "asc"]];

        $("table").DataTable(datatableConfigs);
        commonDataTableInit.InitFixedHeaderDataTable();
    }
}

$(document).ready(function() {
    dataconfiguration.init();
});