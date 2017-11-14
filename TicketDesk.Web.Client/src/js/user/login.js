$(document).ready(function() {
    //$("#btnLogin").on("click",
    //    function() {
    //        if (($.trim($("#Email").val()) === "") && ($.trim($("#Password").val()) === "")) {
    //            $("#Email").focus();
    //        }

    //        if ($.trim($("#Email").val()) === "") {
    //            $("#Email").effect("shake");
    //        }

    //        if ($.trim($("#Password").val()) === "") {
    //            $("#Password").effect("shake");
    //        }
    //    });

    $("#user-login-form").on("submit",
        function() {
            if (($.trim($("#Email").val()) === "") && ($.trim($("#Password").val()) === "")) {
                $("#Email").focus();
            }

            if ($.trim($("#Email").val()) === "" || $("#Email").hasClass("input-validation-error")) {
                //$("#Email").effect("shake");
                $('#Email').startValidateEffect();
            }

            if ($.trim($("#Password").val()) === "" || $("#Password").hasClass("input-validation-error")) {
                //$("#Password").effect("shake");
                $("#Password").startValidateEffect();
            }
        });
    var $htmlOrBody = $('html, body'), // scrolling happens on body for some browsers, html for others
    scrollTopPadding = 80,
    scrollTopLast = 0;
    $('.field .form-control').focus(function () {
        // save the current scroll position for blur event
        scrollTopLast = $(window).scrollTop();
        // scroll to the textarea
        $htmlOrBody.scrollTop($(this).offset().top - scrollTopPadding);
    }).blur(function () {
        // scroll back to position before textarea focus
        $htmlOrBody.scrollTop(scrollTopLast);
    });
});