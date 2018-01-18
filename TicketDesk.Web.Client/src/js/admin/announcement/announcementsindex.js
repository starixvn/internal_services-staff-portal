var datetimepicker = {
    init: function() {
        $(".datetimepicker").datetimepicker({
            format: "MM-DD-YYYY"
        });
    }
}

$(document).ready(function() {
    datetimepicker.init();
});