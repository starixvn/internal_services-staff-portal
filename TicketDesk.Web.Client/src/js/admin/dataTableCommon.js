var commonDataTableInit = {
    Init: function () {
        return {
            pageLength: 20,
            paging: true,
            searching: true,
            dom: '<"top"f>rt<".pull-left" i><".pull-right" lp><"clear">',
            lengthMenu: [20, 50, 100],
            info: false,
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
    }
}