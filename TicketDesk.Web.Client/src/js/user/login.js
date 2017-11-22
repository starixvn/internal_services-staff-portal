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

    $('.field .form-control').focus(function () {
        $(this)[0].scrollIntoView();
    });

});