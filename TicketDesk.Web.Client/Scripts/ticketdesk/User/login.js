var userLogin = (function ($) {

    function registerEventLogin() {
        $('#btnLogin').on('click', function () {
            if (($.trim($("#Email").val()) === '') && ($.trim($("#Password").val()) === '')) {
                $("#Email").focus();
            }

            if ($.trim($("#Email").val()) === '') {
                $("#Email").effect("shake");
            }

            if ($.trim($("#Password").val()) === '') {
                $("#Password").effect("shake");
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

