var ticketList = (function ($) {
    var domElement = {
        ShopSelector: "#ShopId",
        DateTimePicker: ".datetimepicker",
        FromDate: ".datetimepicker.from",
        ToDate: ".datetimepicker.to",
        FormFilter: "#filterForm",
        TicketTableSection: "#ticket-list-section",
        TicketTable: "#ticket-table",
        TicketStatusSelector: ".ticket-status",
        ConfirmPopup: "#confirm-modal",
        ConfirmButton: "#confirm-button",
        TicketDetailLink: "a.ticket-details",
        TicketDetaisModal: "#ticket-details-modal",
        TicketDetailsContent: "#ticket-details-content"
    }

    function setupFromDateToDate() {
        $(domElement.FromDate).on("dp.change",
            function (e) {
                $(domElement.ToDate).data("DateTimePicker").minDate(e.date);
            });

        $(domElement.ToDate).on("dp.change",
            function (e) {
                $(domElement.FromDate).data("DateTimePicker").maxDate(e.date);
            });
    }

    function initDateTimePicker() {
        $(domElement.DateTimePicker).datetimepicker({
            format: "MM-DD-YYYY",
            useCurrent: false,
        });

        setupFromDateToDate();
    }

    function initDataTable() {
        var ticketListTable = commonDataTableInit.Init();
        ticketListTable.order = [[1, "desc"]];
        ticketListTable.columns =
            [{
                width: 70,
                sortable: false
            },
            {
                width: 80,
                sortable: true
            },
            {
                width: null,
                sortable: true
            },
            {
                width: 150,
                sortable: true
            },
            {
                width: 150,
                sortable: true
            },
            {
                width: 180,
                sortable: true
            },
            {
                width: 150,
                sortable: false
            }];
        ticketListTable.drawCallback = function (settings) {
            sidebar.initSelect2();
        }
        $(domElement.TicketTable).DataTable(ticketListTable);
        floatThead.initFloatThead(domElement.TicketTable);
    }


    function getTicketList() {
        $.ajax({
            url: $(domElement.FormFilter).attr("action"),
            data: $(domElement.FormFilter).serialize(),
            success: function (data) {
                $(domElement.TicketTableSection).html(data);
                registerChangeStatusEvent();
                registerGetTicketDetailsEvent();
                initDataTable();
            }
        });
    }

    function registerChangeStatusEvent() {
        $(domElement.TicketStatusSelector).on("change", function () {
            var url = $(this).data("href");
            var status = $(this).val();
            var action = function () {
                $.ajax({
                    url: url,
                    method: "post",
                    data: {
                        status
                    },
                    success: function (response) {
                        $(domElement.ConfirmPopup).modal("hide");
                        if (response.isSuccessful) {
                            toastr.success(response.message);
                            getTicketList();
                        } else {
                            toastr.error(response.message)
                        }
                    }
                });
            }
            confirmPopup(action);
        })
    }

    function confirmPopup(action) {
        var $popup = $(domElement.ConfirmPopup);
        $popup.modal("show");
        var notTrigger = false;
        $(domElement.ConfirmPopup).one('click', domElement.ConfirmButton, function () {
            if (!notTrigger) {
                action();
            }
        });
        $(domElement.ConfirmPopup).one('click', '.btn-cancel', function () {
            notTrigger = true;
        });
    }

    function registerGetTicketDetailsEvent() {
        $(domElement.TicketDetailLink).on("click", function (e) {
            var href = $(this).attr("href");
            $.get(href, function (data) {
                $(domElement.TicketDetailsContent).html(data);
                $(domElement.TicketDetaisModal).modal("show");
            });
            e.preventDefault();
        });
    }

    function initModule() {
        initDateTimePicker();
        getTicketList();
        $(domElement.FormFilter).on("submit", function (e) {
            getTicketList();
            e.preventDefault();
        });
    }

    return {
        initModule
    }
})(jQuery);

$(document).ready(function () {
    ticketList.initModule();
});