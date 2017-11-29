function InitFormHandler(formclassname, allowRedirect) {
    var selector = "." + formclassname;
    var formSubmitSelector = "." + formclassname + " input[type=submit]";

    SetValueForRequiredRadioButton(formclassname);

    $("body").on("input",
        "input[type='text'], textarea",
        function() {
            if ($(this).attr("data-val-length-max") !== undefined) {
                var maxChar = $(this).attr("data-val-length-max");
                if ($(this).val().length > maxChar) {
                    var textRemain = $(this).val().substring(0, maxChar);
                    $(this).val(textRemain);
                }
            }
        });

    $('#notification-modal').on('hidden.bs.modal',
        function () {
            location.reload();
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

                    $(formSubmitSelector).disable();

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
                    SetErrorTextAndShowModal("Invalid or missing files", "Files không hợp lệ hoặc không có file nào");
                    e.preventDefault();
                    return false;
                }
            } else {
                SetErrorTextAndShowModal("Invalid data in fields", "Không có dữ liệu trong các ô");
                e.preventDefault();
                return false;
            }

            return true;
        });

    //CheckFormAndTurnSubmitButton(formclassname);
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
    //var selector = "." + formclassname;
    //var inputSelector = "." + formclassname + " input[type=submit]";

    //var validate = $(selector).validate();
    //var formSummittedState = validate.submitted;
    //var isValid = validate.checkForm();
    //if (isValid && CheckValidFilesIfDropzoneExisted()) {
    //    // Enable the button
    //    $(inputSelector).attr("disabled", false);
    //} else {
    //    // Disable the button
    //    $(inputSelector).attr("disabled", true);
    //}

    //$(selector).validate().submitted = formSummittedState;
}

$.validator.setDefaults({
    ignore: [],
    // any other default options and/or rules
});