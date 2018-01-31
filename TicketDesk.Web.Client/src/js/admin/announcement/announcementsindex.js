var datetimepicker = {
    init: function () {
        datetimepicker.setupDateTimePicker();
        datetimepicker.setupStartEnd();
    },

    setupDateTimePicker: function () {
        var dateString = $(".datetimepicker").val();
        if (dateString !== "" && dateString !== NaN) {
            $(".datetimepicker").datetimepicker({
                format: "MM-DD-YYYY hh:mm A",
                defaultDate: moment(dateString, "MM-DD-YYYY hh:mm A"),
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

    setupStartEnd: function() {
        $(".datepicker.start").datetimepicker({
            format: "MM-DD-YYYY"
        });

        $(".datepicker.end").datetimepicker({
            format: "MM-DD-YYYY",
            useCurrent: false
        });

        $(".datepicker.start").on("dp.change",
            function(e) {
                $(".datepicker.end").data("DateTimePicker").minDate(e.date);
            });

        $(".datepicker.end").on("dp.change",
            function(e) {
                $(".datepicker.start").data("DateTimePicker").maxDate(e.date);
            });
    }

    //setupDatePicker: function (useCurrent) {
    //    $(".datepicker").datetimepicker({
    //        format: "MM-DD-YYYY"
    //    });
    //}
}

var modalhandler = {
    init: function () {
        modalhandler.resetModalForm();
        modalhandler.assignFunctions();
        modalhandler.modalFormAjax();
        modalhandler.dismissAll();
        modalhandler.showMessageIfAny();
        modalhandler.quickPublish();
    },

    quickPublish: function () {
        $("section.table form").on("submit",
            function (e) {
                e.preventDefault();
                modalhandler.ajaxForm(this, "#table-modal .modal", "#table-modal .modal");
                return false;
            });
    },

    showMessageIfAny: function () {
        var message = localStorage.getItem("message");
        if (message !== null) {
            $(".alert-success").text(message);
            $(".alert-success").removeClass("hide");

            window.setTimeout(function () {
                $(".alert-success").fadeTo(500, 0).slideUp(500, function () {
                    $(".alert-success").addClass("hide");
                });
            }, 5000);

            localStorage.removeItem("message");
        }
    },

    dismissAll: function () {
        $("#announcement-modal").on("click",
            ".dismiss-all",
            function () {
                $("#announcement-modal .modal").modal("hide");
            });
    },

    resetModalForm: function () {
        // apply to future modal too
        $("#announcement-modal").on("hidden.bs.modal",
            ".modal",
            function () {
                $(this).remove();
            });

        $("section.table").on("hidden.bs.modal",
            ".modal",
            function () {
                $(this).remove();
            });
    },

    modalFormAjax: function () {
        // ONLY FOR PUBLISH
        $("#announcement-modal").on("submit",
            "form",
            function (e) {
                e.preventDefault();

                $.validator.unobtrusive.parse($(this));
                $(this).validate();

                if ($(this).valid()) {
                    modalhandler.ajaxForm(this, "#announcement-modal .modal", "#announcement-modal .modal");
                }

                return false;
            });
    },

    ajaxForm: function (form, selector, selectorWithModal) {
        $.ajax({
            url: $(form).attr("action"),
            method: $(form).attr("method"),
            data: $(form).serialize(),
            success: function (result) {
                if (result.success) {
                    if (result.html !== undefined) {
                        $(selector).before(result.html);
                        $(selectorWithModal).first().modal();
                    } else {
                        if (result.message !== undefined) {
                            localStorage.setItem("message", result.message);
                        }

                        location.reload();
                    }
                } else {
                    // ON VALIDATION ERROR
                    $(selectorWithModal).html(result);
                }
            }
        });
    },

    assignFunctions: function () {
        modalhandler.assignCreate();
        modalhandler.assignEdit();
        modalhandler.assignDelete();
        //modalhandler.assignPublish();
    },

    assignEdit: function () {
        $(".edit").on("click",
            function () {
                var href = $(this).data("href");
                modalhandler.ajaxModal(href);
            });
    },

    assignCreate: function () {
        $(".create").on("click",
            function () {
                var href = $(this).data("href");
                modalhandler.ajaxModal(href);
            });
    },

    assignDelete: function () {
        $(".delete").on("click",
            function () {
                var href = $(this).data("href");
                modalhandler.ajaxModal(href);
            });
    },

    ajaxModal: function (href, method) {
        if (method === undefined) {
            method = "get";
        }

        $.ajax({
            url: href,
            method: method,
            success: function (result) {
                $("#announcement-modal").html(result);
                datetimepicker.setupDateTimePicker();
                $("#announcement-modal .modal").modal();
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
                .text("Are you sure you want to publish these announcements right away?");
        } else {
            $("#publish-announcement-confirm-modal .modal-body p")
                .text("Are you sure you want to publish this announcement right away?");
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

        var hours = today.getHours();
        var minutes = today.getMinutes();
        var ampm = hours >= 12 ? "PM" : "AM";
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? "0" + minutes : minutes;
        var strTime = hours + ":" + minutes + " " + ampm;
        return strTime;
    },

    addMissingZero: function (data) {
        if (data < 10) {
            data = "0" + data;
        }

        return data;
    }
}

var announcementSort = {
    init: function () {
        announcementSort.assignSortIcon();

        $(".sort").on("click",
            function () {
                // Get column name
                var colName = $(this).data("col");

                // get list hidden sort direction input
                var input = $(".sort-direction." + colName);

                // toggle value
                if ($(input).val() === "True") {
                    $(input).val("False");
                } else {
                    $(input).val("True");
                }

                // Set input for sort-priority
                $(".sort-priority").val(1);
                $(".sort-priority." + colName).val(0);

                // submit form
                $(input).closest("form").trigger("submit");
            });
    },

    assignSortIcon: function () {
        $(".sort").each(function (index, obj) {
            var colName = $(obj).data("col");
            announcementSort.removeSortClass($(obj).parent());

            if ($(".sort-priority." + colName).val() === "0") {
                var input = $(".sort-direction." + colName);
                announcementSort.insertSortClass($(obj).parent(), $(input).val() === "True");
            }
        });
    },

    removeSortClass: function (element) {
        $(element).removeClass("increase");
        $(element).removeClass("decrease");
    },

    insertSortClass: function (element, isAsc) {
        if (isAsc) {
            $(element).addClass("increase");
        } else {
            $(element).addClass("decrease");
        }
    }
}

$(document).ready(function () {
    datetimepicker.init();
    modalhandler.init();
    publishannouncement.init();
    announcement.init();
    announcementSort.init();
});