var commonDataTableInit = {
    Init: function () {
        return {
            pageLength: 20,
            paging: true,
            searching: true,
            dom: '<"top"f>rt<".pull-left" i><".pull-right" lp><"clear">',
            fixedHeader: {
                header: true,
                headerOffset: 50
            },
            lengthMenu: [20, 50, 100],
            info: true,
            language: {
                lengthMenu: "Page size _MENU_",
                search: "_INPUT_",
                searchPlaceholder: "Search",
                emptyTable: "There is no data",
                paginate: {
                    first: "First",
                    last: "Last",
                    next: '<i class="icon-next"></i>',
                    previous: '<i class="icon-previous"></i>'
                }
            },
            autoWidth: false
        }
    },

    InitFixedHeaderDataTable: function () {
        $(window).scroll(function () {
            if ($(this).scrollTop() > 130 && $("table").length > 2) {
                $("table:nth(1)").remove();
            }
        });
    }
}

function InitFixedHeaderDataTable() {
}