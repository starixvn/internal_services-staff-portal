var announcement = {
    init: function () {
        announcement.assignButton();
    },

    assignButton: function () {
        $("#announcement-modal").on("click",
            "#set-current-datetime-button",
            function() {
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
        var dd = announcement.addMissingZero(today.getDate());
        var mm = announcement.addMissingZero(today.getMonth() + 1); //January is 0!
        var yyyy = today.getFullYear();

        return mm + "-" + dd + "-" + yyyy;
    },

    getCurrentTime: function () {
        var today = new Date();
        var hh = announcement.addMissingZero(today.getHours());
        var mm = announcement.addMissingZero(today.getMinutes());

        return hh + ":" + mm;
    },

    addMissingZero: function(data) {
        if (data < 10) {
            data = "0" + data;
        }

        return data;
    }
};

$(document).ready(function () {
    announcement.init();
});