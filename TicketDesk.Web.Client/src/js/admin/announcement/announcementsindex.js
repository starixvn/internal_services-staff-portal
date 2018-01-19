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

                $(form).validate();

                $.ajax({
                    url: $(form).attr("action"),
                    method: $(form).attr("method"),
                    data: $(form).serialize(),
                    success: function(res, status, xhr) {
                        location.reload();
                    },
                    error: function(request, error) {
                        $("#notification-modal div.modal-header > h4").text(error);
                        $("#notification-modal div.modal-body > p").text("Error status code: " + request.status);
                        $("#notification-modal").modal();
                    }
                });
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

$(document).ready(function () {
    datetimepicker.init();
    modalhandler.init();
});