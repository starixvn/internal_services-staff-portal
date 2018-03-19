var announcementReadEvent = {
    registerUnreadClickEvent: function () {
        $(".unread").on("click", function (e) {
            var currentAnnouncement = $(this);
            var id = currentAnnouncement.data("id");
            var url = $(currentAnnouncement).data("href");
            if (id > 0) {
                $.get(url, function (response) {
                    if (response.result) {
                        currentAnnouncement.removeClass("unread");
                        currentAnnouncement.attr("data-id", 0);
                        if ($(".unread-announcements").length) {
                            $(".unread-announcements").text(response.unreadQuantity);
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