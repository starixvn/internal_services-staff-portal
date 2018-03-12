function InitFormHandler(formclassname, allowRedirect) {
    var selector = "." + formclassname;
    var formSubmitSelector = "." + formclassname + " input[type=submit]";

    SetValueForRequiredRadioButton(formclassname);

    ApplyMaxLengh();

    $("body").on("input",
        "input[type='text'], textarea",
        function () {
            CheckFormAndTurnSubmitButton(formclassname);
        });

    $('#notification-modal').on('hidden.bs.modal',
        function () {
            location.reload();
            $(selector).validate().resetForm();
            $(selector)[0].reset();
        });

    $(selector).on("submit",
        function(e) {
            if ($(this).valid()) {
                if (CheckValidFilesIfDropzoneExisted()) {

                    $(formSubmitSelector).prop('disabled', true);

                    if (!allowRedirect) {
                        e.preventDefault();
                        $.ajax({
                            url: $(this).attr('action'),
                            method: $(this).attr('method'),
                            data: $(this).serialize(),
                            success: function(res, status, xhr) {
                                $("#notification-modal").modal();
                            },
                            error: function (request, error) {
                                $("#notification-modal div.modal-header > h4").text(error);
                                $("#notification-modal div.modal-body > p").text("Error status code: " + request.status);
                                $("#notification-modal").modal();
                            }
                        });
                    }

                } else {
                    // Errors in files
                    var englishText = BuildFileErrorText();
                    SetErrorTextAndShowModal(englishText);
                    e.preventDefault();
                    return false;
                }
            }

            return true;
        });
}

function ApplyMaxLengh() {
    var textInput = $("input[type='text'], textarea");
    textInput.each(function (object) {
        if ($(this).attr("data-val-length-max") !== undefined && !$(this).hasClass("skip-limit")) {
            var maxChar = $(this).attr("data-val-length-max");
            $(this).attr('maxlength', maxChar);
        }
    });
}

function SetValueForRequiredRadioButton(formclassname) {
    $(".required-radio").on("change",
        function () {
            $(".required-radio-value").val($(".required-radio:checked").val());
            $(".required-radio-value").valid();
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
    }

    $(selector).validate().submitted = formSummittedState;
}

function CountDownCharacter(element) {
    var maxLength = 5000;
    var remainLength = maxLength - element.val().length;
    var parent = $(element).closest(".item");
    if ($(".countdown-character").length) {
        $(parent).find(".countdown-character").text(remainLength);
    }
}

$("form").on("keyup",
    ".description",
    function() {
        CountDownCharacter($(this));
    });

$.validator.setDefaults({
    ignore: []
    // any other default options and/or rules
});