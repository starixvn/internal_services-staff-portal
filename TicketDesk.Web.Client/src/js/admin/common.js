var sidebar = {
    init: function () {
        sidebar.hovermenu();
        sidebar.initSelect2();
        $(".toggle-sidebar").on("click", function () {
            sidebar.toggleMenu();
            dataTableReflow.setIntervalToReinitFixedHeader();
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

    saveExpandedMenuState: function (isSidebarExpanded) {
        var url = "/NapUserSetting/SetIsSidebarExpanded?IsSidebarExpanded=" + isSidebarExpanded;
        $.get(url);
    },

    initSelect2: function () {
        $("select").select2({
            minimumResultsForSearch: Infinity
        });
    },

    toggleMenu: function () {
        if ($("body").hasClass("menu-collapsed")) {
            sidebar.expandSidebar();
        } else {
            sidebar.collapseSidebar();
        }
    },

    collapseSidebar: function () {
        $("body").addClass("menu-collapsed");
        $(".menu-item.has-sub").attr("href", "#");
        sidebar.saveExpandedMenuState(false);
    },

    expandSidebar: function () {
        $("body").removeClass("menu-collapsed");
        $(".menu-item.has-sub").attr("href", "#collapseExample");
        $("#collapseExample").css("height", "auto !important");
        sidebar.saveExpandedMenuState(true);
    },

    hovermenu: function () {
        $(".menu-section").hover(function () {
            $(this).addClass("hover-open");
        }, function () {
            $(this).removeClass("hover-open");
        });
    }
}

var dataTableReflow = {
    reInitFixedHeader: function () {
        if ($("table").length) {
            var $table = $("table").DataTable();
            $table.fixedHeader.disable();
            $table.fixedHeader.enable();
        }
    },

    setIntervalToReinitFixedHeader: function () {
        setTimeout(function () {
            dataTableReflow.reInitFixedHeader();
        }, 500);
    }
}

var bootstrapModalHack = {
    init: function () {
        $(document).on("show.bs.modal",
            ".modal",
            function () {
                var zIndex = 1040 + (10 * $(".modal:visible").length);
                $(this).css("z-index", zIndex);
                setTimeout(function () {
                    $(".modal-backdrop").not(".modal-stack").css("z-index", zIndex - 1).addClass("modal-stack");
                },
                    0);
            });
    }
}

var validation = {
    showHideValidationSummary: function (targetNode) {
        var callback = function () {
            if ($('.field-validation-error', targetNode).length > 0) {
                $(targetNode).removeClass('hidden');
            } else {
                $(targetNode).addClass('hidden');
            }
        };

        var config = { childList: true, subtree: true };
        var observer = new MutationObserver(callback);
        observer.observe(targetNode, config);
    }
}
$(document).ready(function () {
    sidebar.init();
    bootstrapModalHack.init();
    dataTableReflow.setIntervalToReinitFixedHeader();
});