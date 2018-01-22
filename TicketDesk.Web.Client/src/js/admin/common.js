var sidebar = {
    init: function () {
        sidebar.hovermenu();
        $(".toggle-sidebar").on("click", function() {
            sidebar.toggleMenu();
        });
    },

    toggleMenu: function() {
        if ($("body").hasClass("menu-collapsed")) {
            $("body").removeClass("menu-collapsed");
        } else {
            $("body").addClass("menu-collapsed");
        }
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