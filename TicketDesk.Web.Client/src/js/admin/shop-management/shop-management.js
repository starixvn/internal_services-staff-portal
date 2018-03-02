var shopManagement = (function ($) {
    var domElement = {
        FilterForm: '#filterForm',
        TableArea: '#shopList',
        CreateShopForm: '#createShopForm',
        EditShopForm: '#editShopForm',
        CreateModal: '#createModal',
        CreateModalContent: '#createModalContent'
    };

    function getListShop() {
        var data = $(domElement.FilterForm).serialize();
        $.ajax({
            method: "post",
            url: "/Admin/Shops/GetListShop",
            data: data,
            success: function (data) {
                $(domElement.TableArea).html(data);
                initDataTable();
            },
        })
    }

    function initDataTable() {
        $("table").DataTable({
            pageLength: 20,
            paging: true,
            searching: true,
            dom: '<"top"f>rt<".pull-left" i><".pull-right" lp><"clear">',
            columns: [
                { width: "65px", sortable: false },
                { width: "140px", sortable: true },
                { width: "140px", sortable: true },
                { width: "100px", sortable: true },
                { width: "140px", sortable: true },
                { width: "auto", sortable: true },
                { width: "70px", sortable: true },
                { width: "70px", sortable: false },
            ],
            fixedHeader: {
                header: true,
                headerOffset: 50
            },
            lengthMenu: [20, 50, 100],
            info: true,
            language: {
                lengthMenu: 'Page size _MENU_',
                search: '_INPUT_',
                searchPlaceholder: 'Search',
                emptyTable: 'There is no data',
                paginate: {
                    first: 'First',
                    last: 'Last',
                    next: '<i class="icon-next"></i>',
                    previous: '<i class="icon-previous"></i>'
                }
            },
            autoWidth: false
        });

        $(window).scroll(function () {
            if ($(this).scrollTop() > 130 && $('table').length > 2) {
                $('table:nth(1)').remove();
            }
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

    function onAjaxSuccessful(response, $form) {
        if (response.IsSuccess) {
            toastr.success(response.message);
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
        onAjaxSuccessful(response, $form);
    }

    function onUpdateSuccessful(response) {
        var $form = $(domElement.EditShopForm);
        onAjaxSuccessful(response, $form);
    }

    function initToastr() {
        toastr.options = {
            "closeButton": true,
            "positionClass": "toast-top-right",
            "preventDuplicates": true,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        return toastr;
    }

    function initModule() {
        getListShop();
        eventClickCreateButtonHandler();
        initToastr();
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