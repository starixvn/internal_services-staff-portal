
var nexcelSSPCommon = (function ($) {

    function removeDateValidate() {
        // To remove date check
        jQuery.validator.methods["date"] = function (value, element) { return true; }
    }

    function registerEventLogout() {
        $('#confirmLogOut').on('click', function () {
            console.log("Clicked!");
            window.location.href = "/user/sign-out";
        });
    }

    function initSelect2() {
        if ($.fn.select2) {
            if ($("select").hasClass("select2-searchable")) {
                $("select").not(".not-select2").select2();
            } else {
                $("select").not(".not-select2").select2({
                    minimumResultsForSearch: Infinity
                });
            }
        }
    }

    function initialize() {
        registerEventLogout();
        initSelect2();
        removeDateValidate();
    }

    return {
        Initialize: initialize,
        initSelect2: initSelect2
    };

})(jQuery);


$(document).ready(function () {
    nexcelSSPCommon.Initialize();
});

