var shopManagement = (function ($) {
    var domElement = {
        CreateShopForm: '#createShopForm',
        EditShopForm: '#editShopForm',
        CreateModal: '#createModal',
        CreateModalContent: '#createModalContent'
    };

    function getListShop() {
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
        eventClickCreateButtonHandler();
        initToastr();
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