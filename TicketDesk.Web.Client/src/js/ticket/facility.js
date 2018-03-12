var facility = {
    init: function() {
        facility.assignRemoveButton();
        facility.assignAddMoreButton();
        facility.changeTitleOnFormChange();
    },

    assignRemoveButton: function() {
        facility.displayDeleteButton();
        $("body").on("click",
            ".btn-delete-item",
            function() {
                $(this).parent().parent().remove();
                facility.resetFormValidator();
                facility.displayEvenItems();

                $(".ticket-form").trigger("change");
            });
    },

    displayDeleteButton: function() {
        $(".btn-delete-item").show();
        $(".btn-delete-item:first").hide();
    },

    initSelect2: function() {
        if ($("select").length) {
            $("select").select2({
                minimumResultsForSearch: Infinity
            });
        }
    },

    displayEvenItems: function () {
        var items = $("#item-list .item");
        $(items).removeClass("even");
        $(items).each(function(index) {
            if (index % 2 === 1) {
                $(this).addClass("even");
            }
        });
    },

    cloneFacilityItem: function(id) {
        $("select").select2("destroy");
        var clone = $(id).children().last().clone();
        clone.find(".countdown-character").text(5000);
        var guid = clone.children().first().val();
        var regex = new RegExp(guid, "g");
        var newHtml = clone.html(function (i, oldHtml) {
            return oldHtml.replace(regex, facility.generateGuid());
        });

        $(id).append(newHtml);
    },

    assignAddMoreButton: function() {
        $(".btn-add").click(function (event) {
            event.preventDefault();

            var id = "#" + $(this).data("class");

            facility.cloneFacilityItem(id);

            facility.resetFormValidator();

            facility.displayDeleteButton();

            $(".ticket-form").trigger("change");

            facility.displayEvenItems();

            facility.initSelect2();
        });
    },

    resetFormValidator: function() {
        $(".ticket-form").removeData("validator");
        $(".ticket-form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(".ticket-form");
        formhandler.checkFormAndTurnSubmitButton("ticket-form");
    },

    generateGuid: function() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }

        return s4() + s4() + "-" + s4() + "-" + s4() + "-" + s4() + "-" + s4() + s4() + s4();
    },

    getTitle: function(baseTitle) {
        var values = $(".ticket-form").serializeArray();

        var requestTypeValue = "";
        for (var i = 0; i < values.length; i++) {
            if (values[i].name === "RequestType.Value") {
                requestTypeValue = values[i].value;
                break;
            }
        }

        var itemNames = "";
        for (var j = 0; j < values.length; j++) {
            if (values[j].name.indexOf("ItemName.Value") !== -1) {
                if (itemNames === "") {
                    itemNames = values[j].value;
                } else {
                    itemNames = itemNames + "...";
                    break;
                }
            }
        }

        return "[" + baseTitle + "] - " + requestTypeValue + " " + itemNames;
    },

    toTitleCase: function(str) {
        return str.toLowerCase().split(" ").map(function (word) {
            return word.replace(word[0], word[0].toUpperCase());
        }).join(" ");
    },

    changeTitleOnFormChange: function() {
        var title = facility.toTitleCase($("#form-title").text());

        // Default title for a ticket
        $(".ticket-title").val("[" + title + "]");

        $(".ticket-form").on("change",
            function () {
                $(".ticket-title").val(facility.getTitle(title));
            });

        $('.subject input[type="text"]').keypress(function () {
            var value = ($(this).val().length + 1) * 8;
            $(this).css("width", value + "px");
        });
    }
}

$(document).ready(function () {
    facility.init();
});