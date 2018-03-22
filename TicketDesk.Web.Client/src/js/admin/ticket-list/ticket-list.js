var ticketList = (function ($) {
    var domElement = {
        ShopSelector: "#ShopId",
        DateTimePicker: ".datetimepicker",
        FromDate: ".datetimepicker.from",
        ToDate: ".datetimepicker.to",
        FormFilter: "#filterForm",
        TicketTableSection: "#ticket-list-section",
        TicketTable: "#ticket-table"
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
                initDataTable();
            }
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