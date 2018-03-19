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
        modalhandler.dismissAll();
        modalhandler.handleModalForms();
        modalhandler.assignButton();
    },

    assignButton: function () {
        $("#announcement-modal").on("click",
            "#set-current-datetime-button",
            function () {
                $("#announcement-date").val(moment().format("MM-DD-YYYY hh:mm A"));
            });
    },

    dismissAll: function () {
        $("#announcement-modal").on("click",
            ".dismiss-all",
            function () {
                $("#announcement-modal .modal").modal("hide");
            });
    },

    resetModalForm: function () {
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

    assignFunctions: function () {
        $("body").on("click",
            ".modal-form",
            function () {
                var href = $(this).data("href");
                modalhandler.getModalForm(href);
            });
    },

    getModalForm: function (href) {
        $.ajax({
            url: href,
            method: "get",
            success: function (result) {
                $("#announcement-modal").html(result);
                datetimepicker.setupDateTimePicker();
                $.validator.unobtrusive.parse($("#announcement-modal form"));
                $("#announcement-modal form").validate();
                $("#announcement-modal .modal").last().modal();
            }
        });
    },

    handleModalForms: function () {
        $("#announcement-modal").on("submit",
            "form",
            function (e) {
                e.preventDefault();
                if ($(this).valid()) {
                    // Check if announcement publish immediatly

                    var isPublishImmediately = $("#is-publish-immediatly").val();
                    if (isPublishImmediately === "False") {
                        $.ajax({
                            url: $("#is-publish-url").attr("href"),
                            method: $(this).attr("method"),
                            data: $(this).serialize(),
                            success: function(result) {
                                if (result === true) {
                                    // set value to true
                                    $("#is-publish-immediatly").val(true);

                                    // change action url
                                    var publishUrl = $("#publish-action-url").attr("href");
                                    $(this).attr("action", publishUrl);

                                    // render confirm modal
                                    $("#announcement-modal .modal").first().modal();
                                } else {
                                    modalhandler.submitModalForm($("#announcement-modal form"));
                                }
                            }
                        });
                    } else {
                        modalhandler.submitModalForm($(this));
                    }
                }
            });
    },

    submitModalForm: function(form) {
        $.ajax({
            url: $(form).attr("action"),
            method: $(form).attr("method"),
            data: $(form).serialize(),
            success: function (result) {
                // form return true/false
                if (result.success === true) {
                    $("#announcement-modal .modal").modal("hide");
                    toastr.success(result.message);
                    $(".filter-row").trigger("submit");
                }
            }
        });
    }
}

var quickPublish = {
    init: function () {
        quickPublish.handleCheckBoxes();
        quickPublish.handlePublishButton();
        quickPublish.handelQuickPublishForm();
    },

    handlePublishButton: function() {
        $("#announcement-table").on("click",
            ".quick-publish",
            function () {
                var listIdsToPublish = quickPublish.getIdsToPublish();
                var modalUrl = $(this).data("href");

                $.ajax({
                    url: modalUrl,
                    method: "post",
                    data: { ids: listIdsToPublish},
                    success: function (result) {
                        $("#quick-publish-modal").html(result);
                        $("#quick-publish-modal .modal").modal();
                    }
                });
            });
    },

    handelQuickPublishForm: function() {
        $("#quick-publish-modal").on("submit",
            "form",
            function(e) {
                e.preventDefault();
                var listIdsToPublish = quickPublish.getIdsToPublish();
                $.ajax({
                    url: $(this).attr("action"),
                    method: $(this).attr("method"),
                    data: { ids: listIdsToPublish },
                    success: function (result) {
                        // form return true/false
                        if (result.success === true) {
                            $("#quick-publish-modal .modal").modal("hide");
                            toastr.success(result.message);
                            $(".filter-row").trigger("submit");
                        }
                    }
                });
            });
    },

    handleCheckBoxes: function() {
        $("#announcement-table").on("click",
            "input[type=checkbox]",
            function () {
                var listIdsToPublish = quickPublish.getIdsToPublish();
                quickPublish.togglePublishButton(listIdsToPublish.length);
            });
    },

    getIdsToPublish: function () {
        var idsToPublish = [];

        $(".announcement-list input[type=checkbox]:checked").each(function () {
            var id = parseInt($(this).val());
            idsToPublish.push(id);
        });

        return idsToPublish;
    },

    togglePublishButton: function (count) {
        if (count > 0) {
            $(".btn-publish").attr("disabled", false);
        } else {
            $(".btn-publish").attr("disabled", true);
        }
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
                render: function (data, type, row) {
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
    quickPublish.init();
    listAnnouncement.init();
});