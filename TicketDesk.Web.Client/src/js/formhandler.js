function InitFormHandler(formclassname) {
    var selector = "." + formclassname;
    var formInputSelector = "." + formclassname + " input";
    var formSubmitSelector = "." + formclassname + " input[type=submit]";

    $(formInputSelector).on("input",
        function () {
            CheckFormAndTurnSubmitButton(formclassname);
        });

    $(".filechange").on("filechange",
        function () {
            CheckFormAndTurnSubmitButton(formclassname);
        });

    $(selector).on("submit",
        function (e) {
            e.preventDefault();
            if ($(formSubmitSelector).hasClass("disabled")) {
                return false;
            }
            if ($(this).valid()) {
                if (CheckValidFilesIfDropzoneExisted()) {
                    $(formSubmitSelector).addClass("disabled");
                    $.ajax({
                        url: $(this).attr('action'),
                        method: $(this).attr('method'),
                        data: $(this).serialize(),
                        success: function () {
                            $("#notification-modal").modal();
                        }
                    });
                } else {
                    SetErrorTextAndShowModal("Invalid or missing files");
                }
            } else {
                SetErrorTextAndShowModal("Invalid data in field");
            }
        });
}

function CheckValidFilesIfDropzoneExisted() {
    if (typeof ticketDeskDropzone !== "undefined") {
        return CheckValidFiles();
    } else {
        return true;
    }
}

function CheckFormAndTurnSubmitButton(formclassname) {
    var selector = "." + formclassname;
    var inputSelector = "." + formclassname + " input[type=submit]";

    var validate = $(selector).validate();
    var formSummittedState = validate.submitted;
    var isValid = validate.checkForm();
    if (isValid && CheckValidFilesIfDropzoneExisted()) {
        $(inputSelector).removeClass("disabled");
    } else {
        $(inputSelector).addClass("disabled");
    }

    $(selector).validate().submitted = formSummittedState;
}