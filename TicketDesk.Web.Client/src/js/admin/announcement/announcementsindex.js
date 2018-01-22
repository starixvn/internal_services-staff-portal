var datetimepicker = {
    init: function () {
        datetimepicker.setupDateTimePicker();
        datetimepicker.setupDatePicker();
    },

    setupDateTimePicker: function () {
        var dateString = $(".datetimepicker").val();
        if (dateString !== "") {
            $(".datetimepicker").datetimepicker({
                format: "MM-DD-YYYY hh:mm A",
                defaultDate: dateString,
                minDate: moment()
            });
        } else {
            $(".datetimepicker").datetimepicker({
                format: "MM-DD-YYYY hh:mm A",
                minDate: moment()
            });
        }
    },

    setupDatePicker: function() {
        $(".datepicker").datetimepicker({
            format: "MM-DD-YYYY"
        });
    }
}

var modalhandler = {
    init: function () {
        modalhandler.resetModalForm();
        modalhandler.manualModalHandler();
        modalhandler.assignAnnouncementDelete();
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
        $(form).on("submit",
            function(e) {
                e.preventDefault();
                
                $.validator.unobtrusive.parse($(form));
                $(form).validate();

                if ($(form).valid()) {
                    $.ajax({
                        url: $(form).attr("action"),
                        method: $(form).attr("method"),
                        data: $(form).serialize(),
                        success: function (res, status, xhr) {
                            location.reload();
                        },
                        error: function (request, error) {
                            $("#notification-modal div.modal-header > h4").text(error);
                            $("#notification-modal div.modal-body > p").text("Error status code: " + request.status);
                            $("#notification-modal").modal();
                        }
                    });
                }

                return false;
            });
    },

    assignAnnouncementDelete: function() {
        $(".delete").on("click",
            function() {
                var href = $(this).data("href");
                $(".announcement-delete").attr("href", href);
                $("#delete-announcement-confirm-modal").modal();
            });
    }
}

var publishannouncement = {
    init: function() {
        $(".checkbox").on("click",
            function() {
                if ($(this).find("input[type='checkbox']").attr("name").indexOf("IsWaitingToPublish") > 0) {
                    var values = $(".table > form").serialize().split("&");
                    var count = 0;
                    for (var i = 0; i < values.length; i++) {
                        if (values[i].indexOf("IsWaitingToPublish") > 0 && values[i].split("=")[1] === "true") {
                            count++;
                        }
                    }

                    publishannouncement.updatetext(count);
                }
            });
    },

    updatetext: function(count) {
        if (count > 1) {
            $("#publish-announcement-confirm-modal .modal-body p")
                .text("Are you sure you want to public these announcements right away?");
        } else {
            $("#publish-announcement-confirm-modal .modal-body p")
                .text("Are you sure you want to public this announcement right away?");
        }
    }
}

$(document).ready(function () {
    datetimepicker.init();
    modalhandler.init();
    publishannouncement.init();
});