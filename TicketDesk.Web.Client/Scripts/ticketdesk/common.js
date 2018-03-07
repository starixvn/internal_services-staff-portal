
var nexcelSSPCommon = (function ($) {

    function registerEventLogout() {
        $('#confirmLogOut').on('click', function () {
            window.location.href = "/user/sign-out";
        });
    }

    function initSelect2() {
        $("select").select2({
            minimumResultsForSearch: Infinity
        });
    }
    function initialize() {
        registerEventLogout();
        initSelect2();
    }

    return {
        Initialize: initialize
    };

})(jQuery);


$(document).ready(function () {
    nexcelSSPCommon.Initialize();
});

