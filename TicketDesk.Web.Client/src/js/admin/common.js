var sidebar = {
    init: function () {
        sidebar.hovermenu();
        sidebar.initSelect2();
        $(".toggle-sidebar").on("click",
            function () {
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
        if ($.fn.select2) {
            if ($("select").hasClass("select2-searchable")) {
                $("select").not(".not-select2").select2();
            } else {
                $("select").not(".not-select2").select2({
                    minimumResultsForSearch: Infinity
                });
            }
        }
    },

    toggleMenu: function () {
        if ($(".header, .sidebar").hasClass("menu-collapsed")) {
            sidebar.expandSidebar();
        } else {
            sidebar.collapseSidebar();
        }
    },

    collapseSidebar: function () {
        $(".header, .sidebar").addClass("menu-collapsed");
        $(".menu-item.has-sub").attr("href", "#");
        sidebar.saveExpandedMenuState(false);
    },

    expandSidebar: function () {
        $(".header, .sidebar").removeClass("menu-collapsed");
        $(".menu-item.has-sub").attr("href", "#collapseExample");
        $("#collapseExample").css("height", "auto !important");
        sidebar.saveExpandedMenuState(true);
    },

    hovermenu: function () {
        $(".menu-section").hover(function () {
            if ($(".sidebar").hasClass("menu-collapsed")) {
                $(this).addClass("hover-open");
                $(this).find(".collapse").prop("style", "height: auto;");
            }
        },
            function () {
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
        if ($(".data-table").length) {
            setTimeout(function () {
                dataTableReflow.reInitFixedHeader();
            }, 500);
        }
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

var toastrPopup = {
    initToastr: function () {
        toastr.options = {
            "closeButton": true,
            "positionClass": "toast-top-right",
            "preventDuplicates": true,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        return toastr;
    }
}

var ajaxSetup = {
    init: function () {
        $.ajaxSetup({
            error: function (x, status, error) {
                if (x.status === 403 || x.status === 302) {
                    window.location.reload();
                }
            }
        });
    }
}

var logoutConfirm = {
    init: function () {
        $('#confirmLogOut').on('click', function () {
            window.location.href = "/user/sign-out";
        });
    }
}
$(document).ready(function () {
    ajaxSetup.init();
    sidebar.init();
    bootstrapModalHack.init();
    toastrPopup.initToastr();
    logoutConfirm.init();
    dataTableReflow.setIntervalToReinitFixedHeader();
});