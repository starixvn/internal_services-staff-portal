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

    setupStartEnd: function () {
        $(".datepicker.start").datetimepicker({
            format: "MM-DD-YYYY"
        });

        $(".datepicker.end").datetimepicker({
            format: "MM-DD-YYYY",
            useCurrent: false
        });

        $(".datepicker.start").on("dp.change",
            function (e) {
                $(".datepicker.end").data("DateTimePicker").minDate(e.date);
            });

        $(".datepicker.end").on("dp.change",
            function (e) {
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
        $("#announcement-table").on("click", ".btn-publish", function () {
            var title = "Announcement Publish";
            var Ids = publishAnnouncement.getIdsToPublish();
            var message = "";
            if (Ids.length > 1) {
                message = "Are you sure you want to publish these announcements right away?"
            } else {
                message = "Are you sure you want to publish this announcement right away?"
            }
            var publish = modalhandler.publishAjax();
            modalhandler.confirmModal(title, message, publish);
            ;
        });
    },

    publishAjax: function () {
        var Ids = publishAnnouncement.getIdsToPublish();
        return function () {
            $.ajax({
                url: "/Admin/Announcements/Publish",
                method: "post",
                data: { Ids },
                success: function (response) {
                    if (response.success) {
                        $('#confirm-popup').modal("hide");
                        toastr.success(response.message);
                        setTimeout(function () { location.reload(true) }, 3000)
                    } else {
                        toastr.error("Error");
                    }
                },
            });
        };
    },

    confirmModal: function (title, message, action) {
        var popup = $('#confirm-popup');
        popup.find(".modal-title").text(title);
        popup.find('.message').text(message);
        popup.modal("show");
        var notTrigger = false;
        $(popup).one('click', '#confirm-btn', function () {
            if (!notTrigger) {
                action();
            }
        });
        $(popup).one('click', '.cancel-btn', function () {
            notTrigger = true;
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
                            $("#CreateAnnouncementModal").modal("hide");
                            toastr.success(result.message);
                        }
                        setTimeout(function () { location.reload(true); }, 3000)
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
        $("#announcement-table").on("click", ".edit",
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
        $("#announcement-table").on("click", ".delete",
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

var publishAnnouncement = {
    init: function () {
        $("#announcement-table").on("click", "input[type=checkbox]",
            function () {
                var listIdsToPublish = publishAnnouncement.getIdsToPublish();
                publishAnnouncement.handlePublishButton(listIdsToPublish.length);
            });
    },

    getIdsToPublish: function () {
        var listIdsToPublish = [];
        $(".announcement-list input[type=checkbox]:checked").each(function (i, e) {
            listIdsToPublish.push($(this).val());
        });
        return listIdsToPublish
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

var listAnnouncement = {
    initDataTable: function ($table) {
        $table.DataTable({
            pageLength: 20,
            paging: true,
            searching: true,
            order: [[0, "asc"]],
            dom: '<"top"f>rt<".pull-left" i><".pull-right" lp><"clear">',
            columns: [
                { width: "70px", sortable: false },
                { width: "70px", sortable: true },
                { width: "160px", sortable: true },
                { width: "120px", sortable: true },
                { width: "160px", sortable: true },
                { width: "auto", sortable: true },
                { width: "80px", sortable: true },
                { width: "70px", sortable: false },
                { width: "65px", sortable: false },
            ],
            fixedHeader: {
                headerOffset: 50,
                header: true,
                bScrollCollapse: true
            },
            lengthMenu: [20, 50, 100],
            info: true,
            language: {
                lengthMenu: 'Page size _MENU_',
                search: '_INPUT_',
                searchPlaceholder: 'Search',
                emptyTable: 'There is no data',
                paginate: {
                    first: 'First',
                    last: 'Last',
                    next: '<i class="icon-next"></i>',
                    previous: '<i class="icon-previous"></i>'
                }
            },
            autoWidth: false
        });
    },

    getListAnnouncement: function () {
        $.ajax({
            url: "/Admin/Announcements/AnnouncementList",
            method: "get",
            data: $("form.filter-row").serialize(),
            success: function (response) {
                $("#announcement-table").html(response);
                listAnnouncement.initDataTable($("table.announcement-list"));
            },
        });
    },
}

$(document).ready(function () {
    datetimepicker.init();
    modalhandler.init();
    publishAnnouncement.init();
    announcement.init();
    listAnnouncement.getListAnnouncement();
    $("form.filter-row").submit(function (e) {
        listAnnouncement.getListAnnouncement();
        e.preventDefault();
    });
});