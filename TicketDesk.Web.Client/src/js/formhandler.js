function InitFormHandler(formclassname, allowRedirect) {
    var selector = "." + formclassname;
    var formSubmitSelector = "." + formclassname + " input[type=submit]";

    SetValueForRequiredRadioButton(formclassname);

    ApplyMaxLengh();
    
    $("body").on("input",
        "input[type='text'], textarea",
        function () {
            CheckFormAndTurnSubmitButton(formclassname);
            //if ($(this).attr("data-val-length-max") !== undefined && !$(this).hasClass("skip-limit")) {
                //var maxChar = $(this).attr("data-val-length-max");
                //if ($(this).val().length > maxChar) {
                //    var textRemain = $(this).val().substring(0, maxChar);
                //    $(this).val(textRemain);
                //}
            //}
        });

    $('#notification-modal').on('hidden.bs.modal',
        function () {
            location.reload();
            $(selector).validate().resetForm();
            $(selector)[0].reset();
        });
    
    //$(selector).on("input",
    //    "input",
    //    function() {
    //        console.log("on input");
    //        CheckFormAndTurnSubmitButton(formclassname);
    //    });

    //$(selector).on("input",
    //    "textarea",
    //    function () {
    //        console.log("on input");
    //        CheckFormAndTurnSubmitButton(formclassname);
    //    });

    //$(".filechange").on("filechange",
    //    function () {
    //        CheckFormAndTurnSubmitButton(formclassname);
    //    });

    $(selector).on("submit",
        function(e) {
            //if ($(formSubmitSelector).hasClass("disabled")) {
            //    e.preventDefault();
            //    return false;
            //}
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
                    //SetErrorTextAndShowModal("Invalid or missing files or total file size exceeded max number", "Files không hợp lệ hoặc không có file nào hoặc tổng dung lượng file vượt quá giới hạn cho phép");
                    SetErrorTextAndShowModal(englishText);
                    e.preventDefault();
                    return false;
                }
            }
            //else {
            //    SetErrorTextAndShowModal("Invalid data in fields", "Không có dữ liệu trong các ô");
            //    e.preventDefault();
            //    return false;
            //}

            return true;
        });

    //CheckFormAndTurnSubmitButton(formclassname);
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
            //CheckFormAndTurnSubmitButton(formclassname);
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
        // Enable the button
        //$(inputSelector).attr("disabled", false);
    } else {
        // Disable the button
        //$(inputSelector).attr("disabled", true);
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

$("form").on("keyup", ".description", function () {
    CountDownCharacter($(this));
})

$.validator.setDefaults({
    ignore: [],
    // any other default options and/or rules
});