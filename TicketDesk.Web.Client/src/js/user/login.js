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

            if ($.trim($("#Email").val()) === "") {
                //$("#Email").effect("shake");
                $('#Email').animateCss("shake");
            }

            if ($.trim($("#Password").val()) === "") {
                //$("#Password").effect("shake");
                $("#Password").animateCss("shake");
            }
        });
});