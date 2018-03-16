var shopManagement = (function ($) {
    var domElement = {
        FilterForm: '#filterForm',
        FilterShopName: '#filterForm #ShopName',
        FilterShopId: '#filterForm #ShopId',
        TableArea: '#shopList',
        CreateShopForm: '#createShopForm',
        CreateModal: '#createModal',
        CreateModalContent: '#createModalContent',
        EditShopForm: '#editShopForm',
        EditModal: '#editModal',
        EditModalContent: '#editModalContent'
    };

    function searchShopSuggest() {
        $(domElement.FilterShopName).autocomplete({
            source: function (request, response) {
                var term = request.term;
                $.ajax({
                    url: '/Admin/Shop/GetShopSuggestion',
                    dataType: 'json',
                    data: {
                        shopName: term
                    },
                    success: function (data) {
                        if (data && data.length > 0) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.shopName,
                                    value: item.shopId
                                };
                            }));
                        }
                    }
                });
            },
            minLength: 2,
            focus: function (event, ui) {
                event.preventDefault();
                $(domElement.FilterShopName).val(ui.item.label);
                return false;
            },
            select: function (event, ui) {
                event.preventDefault();
                $(domElement.FilterShopName).val(ui.item.label);
                $(domElement.FilterShopId).val(ui.item.value);
                return false;
            },
            change: function (event, ui) {
                var shopName = $(this).val();
                if (!shopName) $(domElement.FilterShopId).val("");
            }
        });
    }

    function getListShop($form) {
        var data = $(domElement.FilterForm).serialize();
        $.ajax({
            method: "post",
            url: $($form).attr("action"),
            data: data,
            success: function(data) {
                $(domElement.TableArea).html(data);
                initDataTable();
                eventClickEditIconHandler();
            }
        });
    }

    function registerFormSubmitEvent($form, $modal) {
        $($form).on("submit", function (e) {
            e.preventDefault();
            $form = $(this);
            $.ajax({
                url: $($form).attr("action"),
                method: "post",
                data: $($form).serialize(),
                success: function (response) {
                    onAjaxSuccessful(response, $form, $modal);
                },
                error: function (response) {
                    toastr.error(response);
                }
            });
        });
    }

    function initDataTable() {
        var shopDataTable = commonDataTableInit.Init();
        shopDataTable.order = [[2, "asc"]];
        shopDataTable.columnDefs =
            [
                {
                    orderable: false,
                    searchable: false,
                    targets: [0, -1, -2]
                },
            {
                width: "70px",
                targets: [0, -1, -2]
            },
            {
                width: "200px",
                targets: [1, 3]
            },
            {
                width: "100px",
                targets: [2]
            },
            {
                width: "140px",
                targets: [4]
            }
            ];

        $("table").DataTable(shopDataTable);

        commonDataTableInit.InitFixedHeaderDataTable();
    }

    function eventClickEditIconHandler() {
        $(".icon-edit").on("click", function () {
            var id = $(this).data("id");
            var editModalContent = $(domElement.EditModalContent);
            $(editModalContent).html('');
            $.ajax({
                type: "GET",
                url: "/Admin/Shop/Edit/" + id,
                dataType: "html",
                success: function (data) {
                    $(editModalContent).html(data);
                    initValidationForm($(editModalContent));
                    registerFormSubmitEvent(domElement.EditShopForm, domElement.EditModal);
                    $(domElement.EditModal).modal('show');
                }
            })
        });
    }

    function eventClickCreateButtonHandler() {
        $(".btn-create").on("click", function () {
            var $createModalContent = $(domElement.CreateModalContent);
            $.ajax({
                type: "GET",
                url: "/Admin/Shop/Create",
                dataType: "html",
                success: function(data) {
                    $createModalContent.html(data);
                    initValidationForm($(domElement.CreateShopForm));
                    registerFormSubmitEvent(domElement.CreateShopForm, domElement.CreateModal);
                    $(domElement.CreateModal).modal('show');
                }
            });
        });
    }

    function initValidationForm($form) {
        $.validator.unobtrusive.parse($form);

        var $validator = $form.validate();
        $validator.settings.highlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.addClass(errorClass).removeClass(validClass);
        };

        $validator.settings.unhighlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass(errorClass).addClass(validClass);
        };
    }

    function showErrorMessage($form, errors) {
        errors.forEach(function (error) {
            $($form).find("input[name='" + error.key + "']").addClass("input-validation-error");
            $($form).find('[data-valmsg-for="' + error.key + '"]:last').text(error.errorMessage);
        });
    }

    function onAjaxSuccessful(response, $form, $modal) {
        if (response.isSuccessful) {
            $($modal).modal("hide");
            toastr.success(response.message);
            setTimeout(function () { location.reload(true); }, 3000)
        } else {
            if (response.errors) {
                showErrorMessage($form, response.errors);
            } else {
                toastr.error(response.message);
            }
        }
    }

    function initModule() {
        getListShop(domElement.FilterForm);
        eventClickCreateButtonHandler();
        eventClickEditIconHandler();
        searchShopSuggest();

        $(domElement.FilterForm).on("submit",
            function(e) {
                getListShop($(this));
                e.preventDefault();
            });
    }

    return {
        initModule: initModule
    }
}(jQuery));

$(document).ready(function () {
    shopManagement.initModule();
});