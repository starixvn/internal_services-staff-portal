var collapseTable = {
    init: function () {
        collapseTable.assignCollapseHeader();
    },

    assignCollapseHeader: function () {
        $(".header").on("click",
            function () {
                var $this = $(this);
                collapseTable.toggleCollapseExpand($this);
            });
    },

    toggleCollapseExpand: function ($this) {
        var groupTicket = $this.attr("data-title");
        var childRows = $('[data-group="' + groupTicket + '"]');
        $this.toggleClass("expand");
        childRows.find("td").slideToggle("fast");
    }
};

var hiddenText = {
    init: function () {
        // 250 characters are shown by default
        var showChar = 50;
        var dots = ".... ";

        $(".show-text").each(function () {
            var content = $(this).html();

            if (content.length > showChar) {
                var cont = content.substr(0, showChar);
                var restOfTheText = content.substr(showChar, content.length - showChar);

                var html = cont +
                    '<span class="dots">' +
                    dots +
                    '</span><span class="morecontent hide">' +
                    restOfTheText +
                    "</span>";

                $(this).html(html);

                $(this).parent().find(".sign").addClass("expand");
            } else {
                $(this).parent().find(".sign").addClass("disabled");
            }
        });

        $(".announcement-detail").on("click",
            function () {
                var element = $(this).find(".morecontent");
                var dots = $(this).find(".dots");
                if ($(element).hasClass("hide")) {
                    $(element).removeClass("hide");
                    $(dots).addClass("hide");
                    $(this).addClass("expanded");
                } else {
                    $(element).addClass("hide");
                    $(dots).removeClass("hide");
                    $(this).removeClass("expanded");
                }
            });
    }
}

var readAnnouncement = {
    setReadAnnouncement: function () {
        $(".unread").on("click", function () {
            var currentAnnouncement = $(this);
            var id = currentAnnouncement.data("id");
            var url = "/NapUserSetting/SaveUnreadAnnouncement/" + id;
            if (id > 0) {
                $.get(url, function (response) {
                    if (response) {
                        currentAnnouncement.removeClass("unread");
                        currentAnnouncement.attr("data-id", 0);
                        var unreadAnnouncement = parseInt($(".unread-announcements").text());
                        $(".unread-announcements").text(unreadAnnouncement - 1);
                    }
                });
            }
        });
    }
};

$(document).ready(function () {
    collapseTable.init();
    hiddenText.init();
    readAnnouncement.setReadAnnouncement();
});