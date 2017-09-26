
var userLogin = (function ($) {

    function registerEventLogin() {
        $('#btnLogin').on('click', function () {
            if (($.trim($("#Email").val()) === '') && ($.trim($("#Password").val()) === '')) {
                $("#Email").focus();
            }
        });
    }

    function initialize() {
        registerEventLogin();
    }

    return {
        Initialize: initialize
    };

})(jQuery);


$(document).ready(function () {
    userLogin.Initialize();
});

