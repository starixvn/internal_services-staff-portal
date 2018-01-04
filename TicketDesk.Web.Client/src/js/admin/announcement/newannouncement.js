var announcement = {
    init: function () {
        announcement.assignButton();
    },

    assignButton: function () {
        $("#set-current-datetime-button").on("click",
            function () {
                var date = announcement.getCurrentDate();
                announcement.setCurrentDate(date);
                var time = announcement.getCurrentTime();
                announcement.setCurrentTime(time);
            });
    },

    setCurrentDate: function (date) {
        $("#announcement-date").val(date);
    },

    setCurrentTime: function (time) {
        $("#announcement-time").val(time);
    },

    getCurrentDate: function () {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = '0' + dd;
        }

        if (mm < 10) {
            mm = '0' + mm;
        }

        return yyyy + '-' + mm + '-' + dd;
    },

    getCurrentTime: function () {
        var today = new Date();
        var hh = today.getHours();
        var mm = today.getMinutes();

        return hh + ':' + mm;
    }
};

$(document).ready(function () {
    announcement.init();
});