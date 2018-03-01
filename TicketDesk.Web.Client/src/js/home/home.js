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
            var html = "";
            if (content.length > showChar) {
                var cont = content.substr(0, showChar);
                var restOfTheText = content.substr(showChar, content.length - showChar);

                html = cont +
                    '<span class="dots">' +
                    dots +
                    '</span><span class="morecontent hide">' +
                    restOfTheText +
                    "</span>";

            } else {
                html = content +
                    '<span class="dots">' +
                    dots +
                    '</span>';
            }
            $(this).html(html);

            
        });

        function AddRemoveClassFromElement(element, classForElement) {
            if ($(element).hasClass(classForElement)) {
                $(element).removeClass(classForElement);
            } else {
                $(element).addClass(classForElement);
            }
        };

        $(".announcement-msg").on("click",
            function () {
                var element = $(this).find(".morecontent");
                var dots = $(this).find(".dots");
                var sign = $(this).find(".sign");

                AddRemoveClassFromElement(dots, "hide");

                AddRemoveClassFromElement(element, "hide");

                AddRemoveClassFromElement(sign, "expanded");
            });
    }
}

var readAnnouncement = {
    btnUnreadFilterHandler: function () {
        $(".btn-unread").on("click", function () {
            var btn = $(this);
            if (!$(btn).hasClass("unread-only")) {
                $(btn).addClass("unread-only");
                readAnnouncement.filterUnreadAnnouncements(true);
            } else {
                $(btn).removeClass("unread-only");
                readAnnouncement.filterUnreadAnnouncements(false);
            }
        });
    },

    filterUnreadAnnouncements: function (unreadOnly) {
        $(".announcement-msg").each(function (i, e) {
            if (!$(e).hasClass("unread")) {
                if (unreadOnly) {
                    $(e).css("display", "none");
                } else {
                    $(e).css("display", "table-row");
                }
            }
        });
    },

    setReadAnnouncement: function () {
        $(".unread").one("click", function () {
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
    readAnnouncement.btnUnreadFilterHandler();
});