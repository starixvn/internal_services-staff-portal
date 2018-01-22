var sidebar = {
    init: function () {
        sidebar.hovermenu();
        $(".toggle-sidebar").on("click", function() {
            sidebar.toggleMenu();
        });
    },

    toggleMenu: function() {
        if ($("body").hasClass("menu-collapsed")) {
            sidebar.expandSidebar();
        } else {
            sidebar.collapseSidebar();
        }
    },

    collapseSidebar: function() {
        $("body").addClass("menu-collapsed");
        $(".menu-item.has-sub").attr("href", "#");
    },

    expandSidebar: function() {
        $("body").removeClass("menu-collapsed");
        $(".menu-item.has-sub").attr("href", "#collapseExample");
    },

    hovermenu: function() {
        $(".menu-section").hover(function() {
            $(this).addClass("hover-open");
        }, function() {
            $(this).removeClass("hover-open");
        });
    }
}

$(document).ready(function() {
    sidebar.init();
});