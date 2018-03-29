var backBtn = {
    registerBackBtnEvent: function () {
        $(".btn-back").on("click", function () {
            if (window.history.length <= 2) {
                window.location.href = "/TicketList";
                console.log("ĐMM");
            } else {
                window.history.back();
            }
        });
    }
}

$(document).ready(function () {
    backBtn.registerBackBtnEvent();
})