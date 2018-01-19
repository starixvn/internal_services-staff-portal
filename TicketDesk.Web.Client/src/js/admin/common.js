var sidebar = {
    init: function() {
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
    }
}

$(document).ready(function() {
    sidebar.init();
});