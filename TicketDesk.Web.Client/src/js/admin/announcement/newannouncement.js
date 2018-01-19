var announcement = {
    init: function () {
        announcement.assignButton();
    },

    assignButton: function () {
        $("#announcement-modal").on("click",
            "#set-current-datetime-button",
            function() {
                var date = announcement.getCurrentDate();
                var time = announcement.getCurrentTime();

                announcement.setCurrentDateTime(date + " " + time);
            });
    },

    setCurrentDateTime: function(datetime) {
        $("#announcement-date").val(datetime);
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

        var tt = "AM";

        var hour = today.getHours();
        if (hour > 12) {
            hour = hour - 12;
            tt = "PM";
        }

        var hh = announcement.addMissingZero(hour);
        var mm = announcement.addMissingZero(today.getMinutes());

        return hh + ":" + mm + " " + tt;
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