var shopManagement = (function ($) {
    var domElement = {
        CreateModal: '#createModal',
        CreateModalContent: '#createModalContent'
    };

    function eventClickCreateButtonHandler() {
        $(".btn-create").on("click", function () {
            var $createModalContent = $(domElement.CreateModalContent);
            $.ajax({
                type: "GET",
                url: "/Admin/Shops/Create",
                dataType: "html",
                success: function (data) {
                    $createModalContent.html(data);
                   // initValidationForm($("#createNewShop"));
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

    function initModule() {
        eventClickCreateButtonHandler();
    }

    return {
        initModule: initModule
    }
}(jQuery));

$(document).ready(function () {
    shopManagement.initModule();
});