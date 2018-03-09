var announcementReadEvent = {
    registerUnreadClickEvent: function () {
        $(".unread").one("click", function () {
            var currentAnnouncement = $(this);
            var id = currentAnnouncement.data("id");
            var url = "/NapUserSetting/SaveUnreadAnnouncement/" + id;
            if (id > 0) {
                $.get(url, function (response) {
                    if (response) {
                        currentAnnouncement.removeClass("unread");
                        currentAnnouncement.attr("data-id", 0);
                        if ($(".unread-announcements").length) {
                            var unreadAnnouncement = parseInt($(".unread-announcements").text());
                            $(".unread-announcements").text(unreadAnnouncement - 1);
                        }
                    }
                });
            }
        });
    }
};

$(document).ready(function () {
    announcementReadEvent.registerUnreadClickEvent();
});