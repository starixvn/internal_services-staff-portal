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
                $("#dynamic-modals").html(result);
                var formResult = $("#is-successful").val();
                if (formResult) {
                    toastr.info("New item added");
                } else {
                    $("#dynamic-modals .modal").modal();
                    dataconfiguration.initValidation();
                }
            }
        });
    },

    initValidation: function() {
        $.validator.unobtrusive.parse($("#dynamic-modals form"));
    },

    initDatatable: function() {
        $("table").DataTable({
            pageLength: 20,
            paging: true,
            searching: true,
            lengthMenu: [20, 50, 100],
            dom: '<"top"f>rt<".pull-left" i><".pull-right" lp><"clear">',
            language: {
                lengthMenu: "Page size _MENU_",
                search: "_INPUT_",
                searchPlaceholder: "Search",
                emptyTable: "There is no data",
                paginate: {
                    first: "First",
                    last: "Last",
                    next: '<i class="icon-next"></i>',
                    previous: '<i class="icon-previous"></i>'
                }
            },
            columnDefs: [
                {
                    orderable: false,
                    target: [0]
                }]
        });
    }
}

$(document).ready(function() {
    dataconfiguration.init();
});