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
                    url: '/Admin/Shops/GetShopSuggestion',
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

    function getListShop() {
        var data = $(domElement.FilterForm).serialize();
        $.ajax({
            method: "post",
            url: "/Admin/Shops/GetListShop",
            data: data,
            success: function (data) {
                $(domElement.TableArea).html(data);
                initDataTable();
                eventClickEditIconHandler();
            },
        })
    }

    function initDataTable() {
        var commonDataTable = commonDataTableInit;
        commonDataTable.columns = 
            [
                { width: "70px", sortable: false },
                { width: "200px", sortable: true },
                { width: "100px", sortable: true },
                { width: "170px", sortable: true },
                { width: "140px", sortable: true },
                { width: "auto", sortable: true },
                { width: "70px", sortable: true },
                { width: "70px", sortable: false }
            ];
        $("table").DataTable(commonDataTable);

        InitFixedHeaderDataTable();
    }

    function eventClickEditIconHandler() {
        $(".icon-edit").on("click", function () {
            var id = $(this).data("id");
            var editModalContent = $(domElement.EditModalContent);
            $(editModalContent).html('');
            $.ajax({
                type: "GET",
                url: "/Admin/Shops/Edit/" + id,
                dataType: "html",
                success: function (data) {
                    $(editModalContent).html(data);
                    initValidationForm($(editModalContent));
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
                url: "/Admin/Shops/Create",
                dataType: "html",
                success: function (data) {
                    $createModalContent.html(data);
                    initValidationForm($(domElement.CreateShopForm));
                    $(domElement.CreateModal).modal('show');
                }
            })
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
            setTimeout(function () {
                location.reload();
            }, 3000);
        } else {
            if (response.errors) {
                showErrorMessage($form, response.errors);
            } else {
                toastr.error(response.message);
            }
        }
    }

    function onCreateSuccessful(response) {
        var $form = $(domElement.CreateShopForm);
        onAjaxSuccessful(response, $form, domElement.CreateModal);
    }

    function onUpdateSuccessful(response) {
        var $form = $(domElement.EditShopForm);
        onAjaxSuccessful(response, $form, domElement.EditModal);
    }

    function initModule() {
        getListShop();
        eventClickCreateButtonHandler();
        eventClickEditIconHandler();
        searchShopSuggest();

        $(domElement.FilterForm).on("submit", function (e) {
            getListShop();
            e.preventDefault();
        })
    }

    return {
        initModule: initModule,
        onCreateSuccessful: onCreateSuccessful,
        onUpdateSuccessful: onUpdateSuccessful
    }
}(jQuery));

$(document).ready(function () {
    shopManagement.initModule();
});