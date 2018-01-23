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
    init: function() {
        // 250 characters are shown by default
        var showChar = 50;
        var dots = ".... ";
        
        $(".show-text").each(function() {
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

        $(".expand").on("click",
            function() {
                var element = $(this).parent().parent().find(".morecontent");
                var dots = $(this).parent().parent().find(".dots");
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

$(document).ready(function() {
    collapseTable.init();
    hiddenText.init();
});