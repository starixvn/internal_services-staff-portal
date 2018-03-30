var ticketListFrontEnd = (function ($) {
    var domElement = {
        FormFiler: "form.filter",
        TicketListTable: "#ticket-list"
    }

    function getTicketList() {
        $.ajax({
            url: $(domElement.FormFiler).attr("action"),
            method: $(domElement.FormFiler).attr("method"),
            data: $(domElement.FormFiler).serialize(),
            success: function (data) {
                $(domElement.TicketListTable).html(data);
                initDataTable($("table"));
                nexcelSSPCommon.initSelect2();
            }
        })
    }

    function initDataTable($table) {
        var tableConfigs = commonDataTableInit.Init();
        tableConfigs.ordering = false;
        tableConfigs.searching = false;
        $table.DataTable(tableConfigs);
    }

    function initModule() {
        getTicketList();
        $(domElement.FormFiler).on("submit", function (e) {
            getTicketList();
            e.preventDefault();
        });
    }

    return {
        initModule
    }
})(jQuery);

$(document).ready(function () {
    ticketListFrontEnd.initModule();
})