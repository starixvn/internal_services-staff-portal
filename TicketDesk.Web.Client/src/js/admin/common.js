var sidebar = {
    init: function () {
        sidebar.initSizeBarIsExpaned();
        sidebar.hovermenu();
        $(".toggle-sidebar").on("click", function() {
            sidebar.toggleMenu();
        });
        $("body").on("click",
            ".icon-calendar",
            function () {
                var datetimepickerElements = $(this).parent().find(".datetimepicker");
                if (datetimepickerElements.length > 0) {
                    $(datetimepickerElements).data("DateTimePicker").toggle();
                }

                var datepickerElements = $(this).parent().find(".datepicker");
                if (datepickerElements.length > 0) {
                    $(datepickerElements).data("DateTimePicker").toggle();
                }
            });
    },

    initSizeBarIsExpaned: function() {
        var isExpanded = localStorage.getItem("IsSidebarExpanded");
        if (isExpanded === "true") {
            sidebar.expandSidebar();
        } else {
            sidebar.collapseSidebar();
        }
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
        localStorage.setItem("IsSidebarExpanded", false);
    },

    expandSidebar: function() {
        $("body").removeClass("menu-collapsed");
        $(".menu-item.has-sub").attr("href", "#collapseExample");
        localStorage.setItem("IsSidebarExpanded", true);
    },

    hovermenu: function() {
        $(".menu-section").hover(function() {
            $(this).addClass("hover-open");
        }, function() {
            $(this).removeClass("hover-open");
        });
    }
}

var bootstrapModalHack = {
    init: function() {
        $(document).on("show.bs.modal",
            ".modal",
            function() {
                var zIndex = 1040 + (10 * $(".modal:visible").length);
                $(this).css("z-index", zIndex);
                setTimeout(function() {
                        $(".modal-backdrop").not(".modal-stack").css("z-index", zIndex - 1).addClass("modal-stack");
                    },
                    0);
            });
    }
}

$(document).ready(function() {
    sidebar.init();
    bootstrapModalHack.init();
});