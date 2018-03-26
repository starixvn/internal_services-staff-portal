var moreAnnouncement = (function ($) {
    var domElement = {
        FormFiler: "form.filter",
        FromDate: "#from-date",
        ToDate: "#to-date",
        AnnouncementTable: "#announcement-list"
    }

    function getAnnouncementList() {
        $.ajax({
            url: $(domElement.FormFiler).attr("action"),
            method: $(domElement.FormFiler).attr("method"),
            data: $(domElement.FormFiler).serialize(),
            success: function (data) {
                $(domElement.AnnouncementTable).html(data);
                initDataTable($("table"));
                nexcelSSPCommon.initSelect2();
            }
        })
    }

    function initDataTable($table) {
        var tableConfigs = commonDataTableInit.Init();
        tableConfigs.ordering = false;
        tableConfigs.searching = false;
        tableConfigs.info = true;
        tableConfigs.infoCallback = function (settings, start, end, max, total, pre) {
            return "Displaying " + start + " to " + end + " of " + max + " item(s)";
        }
        $table.DataTable(tableConfigs);
    }

    function initDateTimePicker() {
        $(domElement.FromDate).datetimepicker({
            format: 'MM-DD-YYYY'
        });

        $(domElement.ToDate).datetimepicker({
            format: 'MM-DD-YYYY',
            useCurrent: false
        });

        $(domElement.FromDate).on("dp.change", function (e) {
            $(domElement.ToDate).data("DateTimePicker").minDate(e.date);
        });

        $(domElement.ToDate).on("dp.change", function (e) {
            $(domElement.FromDate).data("DateTimePicker").maxDate(e.date);
        });
    }

    function initModule() {
        getAnnouncementList();
        initDateTimePicker();
        $(domElement.FormFiler).on("submit", function (e) {
            getAnnouncementList();
            e.preventDefault();
        });
    }

    return {
        initModule
    }
})(jQuery);

$(document).ready(function () {
    moreAnnouncement.initModule();
});