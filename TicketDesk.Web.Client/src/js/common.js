
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
        if ($.fn.select2) {
            if (!$("select").hasClass("not-select2")) {

                $("select.select2-searchable").select2({
                    width: "100%"
                });

                $("select").not(".select2-searchable").select2({
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

