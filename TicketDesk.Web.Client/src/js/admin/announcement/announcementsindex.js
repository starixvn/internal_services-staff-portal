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
                minDate: moment(),
                maxDate: moment().add(1, "year")
            });
        } else {
            $(".datetimepicker").datetimepicker({
                format: "MM-DD-YYYY hh:mm A",
                minDate: moment(),
                maxDate: moment().add(1, "year")
            });
        }
    },

    setupDatePicker: function () {
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
            function (e) {
                e.preventDefault();

                $.validator.unobtrusive.parse($(form));
                $(form).validate();

                if ($(form).valid()) {
                    modalhandler.sendAjax("#modal-form");
                    ////$.ajax({
                    ////    url: $(form).attr("action"),
                    ////    method: $(form).attr("method"),
                    ////    data: $(form).serialize(),
                    ////    success: function (result) {
                    ////        if (result.success) {
                    ////            location.reload();
                    ////        } else {
                    ////            $("#modal-form").html(result);
                    ////        }
                    ////    }
                    ////});
                }

                return false;
            });
    },

    assignAnnouncementDelete: function () {
        $(".delete").on("click",
            function() {
                var href = $(this).data("href");
                ////$(".announcement-delete").attr("href", href);
                ////$("#delete-announcement-confirm-modal").modal();
                $.ajax({
                    url: href,
                    method: "get",
                    success: function(result) {
                        $("#announcement-modal").html(result);
                    }
                });
            });
    },

    sendAjax: function(elementId) {
        $.ajax({
            url: $(form).attr("action"),
            method: $(form).attr("method"),
            data: $(form).serialize(),
            success: function (result) {
                if (result.success) {
                    location.reload();
                } else {
                    $(elementId).html(result);
                }
            }
        });
    }
}

var publishannouncement = {
    init: function () {
        $(".checkbox").on("click",
            function () {
                if ($(this).find("input[type='checkbox']").attr("name").indexOf("IsWaitingToPublish") > 0) {
                    var values = $(".table > form").serialize().split("&");
                    var count = 0;
                    for (var i = 0; i < values.length; i++) {
                        if (values[i].indexOf("IsWaitingToPublish") > 0 && values[i].split("=")[1] === "true") {
                            count++;
                        }
                    }

                    publishannouncement.updatetext(count);
                    publishannouncement.handlePublishButton(count);
                }
            });
    },

    updatetext: function (count) {
        if (count > 1) {
            $("#publish-announcement-confirm-modal .modal-body p")
                .text("Are you sure you want to public these announcements right away?");
        } else {
            $("#publish-announcement-confirm-modal .modal-body p")
                .text("Are you sure you want to public this announcement right away?");
        }
    },

    handlePublishButton: function (count) {
        if (count > 0) {
            $(".btn-publish").attr("disabled", false);
        } else {
            $(".btn-publish").attr("disabled", true);
        }
    }
}

var announcement = {
    init: function () {
        announcement.assignButton();
    },

    assignButton: function () {
        $("#announcement-modal").on("click",
            "#set-current-datetime-button",
            function () {
                var date = announcement.getCurrentDate();
                var time = announcement.getCurrentTime();

                announcement.setCurrentDateTime(date + " " + time);
            });
    },

    setCurrentDateTime: function (datetime) {
        $("#announcement-date").val(datetime);
    },

    getCurrentDate: function () {
        var today = new Date();
        var dd = announcement.addMissingZero(today.getDate());
        var mm = announcement.addMissingZero(today.getMonth() + 1); //January is 0!
        var yyyy = today.getFullYear();

        return mm + "-" + dd + "-" + yyyy;
    },

    getCurrentTime: function () {
        var today = new Date();

        var tt = "AM";

        var hour = today.getHours();
        if (hour > 12) {
            hour = hour - 12;
            tt = "PM";
        }

        var hh = announcement.addMissingZero(hour);
        var mm = announcement.addMissingZero(today.getMinutes());

        return hh + ":" + mm + " " + tt;
    },

    addMissingZero: function (data) {
        if (data < 10) {
            data = "0" + data;
        }

        return data;
    }
}

$(document).ready(function () {
    datetimepicker.init();
    modalhandler.init();
    publishannouncement.init();
    announcement.init();
});