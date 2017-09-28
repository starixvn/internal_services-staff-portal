
var nexcelSSPCommon = (function ($) {

    function registerEventLogout() {
        $('#confirmLogOut').on('click', function () {
            window.location.href = "/user/sign-out";
        });
    }

    function initialize() {
        registerEventLogout();
    }

    return {
        Initialize: initialize
    };

})(jQuery);


$(document).ready(function () {
    nexcelSSPCommon.Initialize();
});

