var myOpenningTicket = {
    init: function() {
        $("#my-opening-tickets").on("click",
            "a.ajax",
            function(e) {
                e.preventDefault();
                e.stopPropagation();
                var action = $(this).attr("href");
                myOpenningTicket.getMyOpenningTickets(action);
            });

        myOpenningTicket.getMyOpenningTickets();
    },

    getMyOpenningTickets: function (action) {
        if (action === undefined) {
            action = $("#my-opening-tickets").data("action");
        }

        $.ajax({
            url: action,
            method: "get",
            success: function (result) {
                $("#my-opening-tickets").html(result);
                collapseTable.init();
            }
        });
    }
}

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
        childRows.find("td").slideToggle("fast");
        var $sign = $($this).find(".sign");
        utility.AddRemoveClassFromElement($this, "expand");
        utility.AddRemoveClassFromElement($sign, "expanded");
    }
};

var utility = {
    AddRemoveClassFromElement: function (element, classForElement) {
        if ($(element).hasClass(classForElement)) {
            $(element).removeClass(classForElement);
        } else {
            $(element).addClass(classForElement);
        }
    }
}

var hiddenText = {
    init: function () {
        var showChar = hiddenText.getShowCharForScreenResolution();
        var dots = " ...";

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
            }
        });

        $(".announcement-msg").on("click",
            function () {
                var element = $(this).find(".morecontent");
                var dots = $(this).find(".dots");
                var sign = $(this).find(".sign");
                utility.AddRemoveClassFromElement(sign, "expanded");
                if ($(element).hasClass("hide")) {
                    $(element).removeClass("hide");
                    $(dots).addClass("hide");
                } else {
                    $(element).addClass("hide");
                    $(dots).removeClass("hide");
                }
            });
    },

    getShowCharForScreenResolution: function () {
        var showChar = 85;
        var w = $(window).width();
        if (w >= 1920) showChar = 200;
        if (1280 <= w && w < 1366) showChar = 120;
        if (1366 <= w && w < 1920) showChar = 130;
        return showChar;
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
    myOpenningTicket.init();
    hiddenText.init();
    readAnnouncement.setReadAnnouncement();
    readAnnouncement.btnUnreadFilterHandler();
});