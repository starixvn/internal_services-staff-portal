function InitFormHandler(formclassname, allowRedirect) {
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
        function(e) {
            if ($(formSubmitSelector).hasClass("disabled")) {
                e.preventDefault();
                return false;
            }
            if ($(this).valid()) {
                if (CheckValidFilesIfDropzoneExisted()) {

                    $(formSubmitSelector).addClass("disabled");

                    if (!allowRedirect) {
                        e.preventDefault();
                        $.ajax({
                            url: $(this).attr('action'),
                            method: $(this).attr('method'),
                            data: $(this).serialize(),
                            success: function(res, status, xhr) {
                                $("#notification-modal").modal();
                            }
                        });
                    }

                } else {
                    SetErrorTextAndShowModal("Invalid or missing files");
                    e.preventDefault();
                    return false;
                }
            } else {
                SetErrorTextAndShowModal("Invalid data in field");
                e.preventDefault();
                return false;
            }

            return true;
        });

    CheckFormAndTurnSubmitButton(formclassname);
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
        // Enable the button
        $(inputSelector).attr("disabled", false);
    } else {
        // Disable the button
        $(inputSelector).attr("disabled", true);
    }

    $(selector).validate().submitted = formSummittedState;
}