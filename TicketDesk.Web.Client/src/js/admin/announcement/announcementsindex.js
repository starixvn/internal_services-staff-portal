var datetimepicker = {
    init: function () {
        datetimepicker.setupDateTimePicker();
    },

    setupDateTimePicker: function () {
        var dateString = $(".datetimepicker").val();
        if (dateString !== "") {
            $(".datetimepicker").datetimepicker({
                format: "MM-DD-YYYY",
                defaultDate: dateString,
                minDate: moment()
            });
        } else {
            $(".datetimepicker").datetimepicker({
                format: "MM-DD-YYYY",
                minDate: moment()
            });
        }

        $(".timepicker").datetimepicker({
            format: "LT"
        });
    }
}

var modalhandler = {
    init: function () {
        modalhandler.resetModalForm();
        modalhandler.manualModalHandler();
    },

    resetModalForm: function () {
        // apply to future modal too
        $("#announcement-modal").on("hidden.bs.modal",
            ".modal",
            function () {
                $(this).find("form")[0].reset();
                $(this).find(".modal").remove();
            });
    },

    manualModalHandler: function () {
        $(".manual-modal-handler").on("click",
            function () {
                var action = $(this).data("action");
                var target = $(this).data("modalid");

                $("#announcement-modal").load(action, function () {
                    datetimepicker.setupDateTimePicker();
                    var form = $("#announcement-modal").find("form");
                    modalhandler.manualFormHandle(form);
                    $(target).modal();
                });
            });
    },

    manualFormHandle: function (form) {
        //$(form).on("submit",
        //    function() {
        //        e.preventDefault();
        //        var returnurl = $(form).data("return");

        //        $(form).validate();

        //        $.ajax({
        //            url: $(form).attr("action"),
        //            method: $(form).attr("method"),
        //            data: $(form).serialize(),
        //            success: function(res, status, xhr) {
        //                window.location.href = returnurl;
        //            },
        //            error: function(request, error) {
        //                $("#notification-modal div.modal-header > h4").text(error);
        //                $("#notification-modal div.modal-body > p").text("Error status code: " + request.status);
        //                $("#notification-modal").modal();
        //            }
        //        });
        //    });
        $(form).validate({
            submitHandler: function (form) {
                $(form).ajaxSubmit();
            }
        });
    }
}

$(document).ready(function () {
    datetimepicker.init();
    modalhandler.init();
});