
var nexcelSSPCommon = (function ($) {

    function removeDateValidate() {
        // To remove date check
        jQuery.validator.methods["date"] = function (value, element) { return true; }
    }

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
        removeDateValidate();
    }

    return {
        Initialize: initialize
    };

})(jQuery);


$(document).ready(function () {
    nexcelSSPCommon.Initialize();
});

