var customerSupport = (function ($) {
    function getSupportType() {
        var supportType = $("#select2-SupportTypeValue-container").attr("title");
        return supportType;
    }

    function setSubjectPrefix() {
        var prefix = getSupportType();
        $(".support-type-label").text(prefix);
    }

    function changeSupportTypeEvent() {
        $('#SupportType\\.Value').on("change", function () {
            setSubjectPrefix();
        });
    }

    function submitForm() {
        $(".ticket-form").on("submit", function (e) {
            e.preventDefault();
        });
    }

    function initModule() {
        setSubjectPrefix();
        changeSupportTypeEvent();
    }

    return {
        initModule: initModule
    }
})(jQuery);

$(document).ready(function () {
    customerSupport.initModule();
})