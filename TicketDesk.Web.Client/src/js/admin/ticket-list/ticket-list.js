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

        ticketListTable.columnDefs =
            [
                {
                    orderable: false,
                    searchable: false,
                    targets: [0,]
                },
            {
                width: "70px",
                targets: [0]
            },
            {
                width: "200px",
                targets: [2, 3]
            },
            {
                width: "250px",
                targets: [4]
            },
            {
                width: "150px",
                targets: [5]
            }
            ];
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