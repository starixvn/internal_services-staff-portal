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
                url: "/Admin/ShopManagement/Create",
                dataType: "html",
                success: function (data) {
                    $createModalContent.html(data);
                    $(domElement.CreateModal).modal('show');
                }
            })
        });
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