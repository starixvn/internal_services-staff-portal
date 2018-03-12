var myOpenningTicket = {
    init: function () {
        $("#my-opening-tickets").on("click",
            "a.ajax",
            function (e) {
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
                console.log("Click");
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

var announcementUnreadBtn = {
    registerUnreadBtnClickEvent: function () {
        $(".btn-unread").on("click", function () {
            var btn = $(this);
            if ($(btn).hasClass("unread-only")) {
                $(btn).removeClass("unread-only");
            } else {
                $(btn).addClass("unread-only");
            }
            announcementUnreadBtn.getListAnnouncement();
        });
    },

    getListAnnouncement: function () {
        var isUnreadOnly = $(".btn-unread").hasClass("unread-only");
        var url = "/Home/GetListAnnouncementByReadState?isUnreadOnly=" + isUnreadOnly;
        $.get(url, function (response) {
            $("#announcement-list").html(response);
            hiddenText.init();
            announcementReadEvent.registerUnreadClickEvent();
        });
    },
}

$(document).ready(function () {
    myOpenningTicket.init();
    announcementUnreadBtn.getListAnnouncement();
    announcementUnreadBtn.registerUnreadBtnClickEvent();
});