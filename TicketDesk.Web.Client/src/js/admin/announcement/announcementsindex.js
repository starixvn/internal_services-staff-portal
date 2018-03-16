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
            var message;
            if (Ids.length > 1) {
                message = "Are you sure you want to publish these announcements right away?";
            } else {
                message = "Are you sure you want to publish this announcement right away?";
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
                url: $(".btn-publish").data("href"),
                method: "post",
                data: { Ids },
                success: function (response) {
                    if (response.success) {
                        $("#confirm-popup").modal("hide");
                        toastr.success(response.message);
                        setTimeout(function () { location.reload(true) }, 3000);
                    } else {
                        toastr.error("Error");
                    }
                }
            });
        };
    },

    confirmModal: function (title, message, action) {
        var popup = $("#confirm-popup");
        popup.find(".modal-title").text(title);
        popup.find(".message").text(message);
        popup.modal("show");
        var notTrigger = false;
        $(popup).one("click", "#confirm-btn", function () {
            if (!notTrigger) {
                action();
            }
        });
        $(popup).one("click", ".cancel-btn", function () {
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
                            $(".modal").modal("hide");
                            toastr.success(result.message);
                            //setTimeout(function() {
                            //        location.reload();
                            //    },
                            //    3000);
                            $(".filter-row").trigger("submit");
                        }
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

        $(".announcement-list input[type=checkbox]:checked").each(function () {
            listIdsToPublish.push($(this).val());
        });

        return listIdsToPublish;
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
                $("#announcement-date").val(moment().format("MM-DD-YYYY hh:mm A"));
            });
    }
}

var listAnnouncement = {
    init: function () {
        listAnnouncement.filterForm();
        $(".filter-row").trigger("submit");
    },

    dataTableDateTimeRenderer: $.fn.dataTable.render.moment("M/D/YYYY h:m A", "M/D/YYYY h:m A"),

    initDataTable: function ($table) {
        var tableConfigs = commonDataTableInit.Init();
        tableConfigs.order = [[2, "desc"]];
        tableConfigs.columnDefs = [
            {
                sortable: false,
                targets: [0, -1, -2]
            },
            {
                width: "55px",
                targets: [0, 1, -1]
            },
            {
                width: "140px",
                targets: [2, 3, 4]
            },
            {
                width: "70px",
                targets: [-2, -3]
            },
            {
                render: function(data, type, row) {
                    if (!data) {
                        return data;
                    } else {
                        return listAnnouncement.dataTableDateTimeRenderer(data, type, row);
                    }
                },

                targets: [2, 4]
            }
        ];

        $table.DataTable(tableConfigs);
        commonDataTableInit.InitFixedHeaderDataTable();
    },

    filterForm: function () {
        $(".filter-form").on("submit",
            ".filter-row",
            function (e) {
                e.preventDefault();
                $.ajax({
                    url: $(this).attr("action"),
                    method: $(this).attr("method"),
                    data: $(this).serialize(),
                    success: function (response) {
                        $("#announcement-table").html("");
                        $("#announcement-table").html(response);
                        listAnnouncement.initDataTable($("table.announcement-list"));
                        sidebar.initSelect2();
                    }
                });
            });
    }
}

$(document).ready(function () {
    datetimepicker.init();
    modalhandler.init();
    publishAnnouncement.init();
    announcement.init();
    listAnnouncement.init();
});