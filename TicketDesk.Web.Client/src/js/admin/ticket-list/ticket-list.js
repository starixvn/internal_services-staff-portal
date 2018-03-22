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
        ConfirmPopup: "#confirm-popup",
        ConfirmButton: "#confirm-button"
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
        var nowDate = new Date();
        $(domElement.DateTimePicker).datetimepicker({
            format: "MM-DD-YYYY",
            useCurrent: false,
        });

        setupFromDateToDate();
    }

    function initDataTable() {
        var ticketListTable = commonDataTableInit.Init();
        ticketListTable.order = [[3, "asc"]];
        ticketListTable.columns =
            [{
                width: 70,
                sortable: false
            },
            {
                width: 300,
                sortable: true
            },
            {
                width: 200,
                sortable: true
            },
            {
                width: 250,
                sortable: true
            },
            {
                width: 150,
                sortable: true
            },
            {
                width: 100,
                sortable: true
            }];
        ticketListTable.bAutoWidth = false;
        $(domElement.TicketTable).DataTable(ticketListTable);

        commonDataTableInit.InitFixedHeaderDataTable();
    }

    function getTicketList() {
        $.ajax({
            url: $(domElement.FormFilter).attr("action"),
            data: $(domElement.FormFilter).serialize(),
            success: function (data) {
                $(domElement.TicketTableSection).html(data);
                registerChangeStatusEvent();
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
        $('#confirm-popup').one('click', domElement.ConfirmButton, function () {
            if (!notTrigger) {
                action();
            }
        });
        $('#confirm-popup').one('click', '.btn-cancel', function () {
            notTrigger = true;
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