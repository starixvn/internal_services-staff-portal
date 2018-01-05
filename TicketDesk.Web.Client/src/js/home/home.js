var collapseTable = {
    init: function() {
        collapseTable.assignCollapseHeader();
    },

    assignCollapseHeader: function() {
        $(".header").on("click",
            function() {
                var $this = $(this);
                collapseTable.toggleCollapseExpand($this);
            });
    },

    toggleCollapseExpand: function($this) {
        var groupTicket = $this.attr("data-title");
        var childRows = $('[data-group="' + groupTicket + '"]');
        $this.toggleClass("expand");
        childRows.find("td").slideToggle("fast");
    }
};

$(document).ready(collapseTable.init);