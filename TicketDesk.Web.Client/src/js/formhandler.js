// To validate hidden field
$.validator.setDefaults({
    ignore: []
    // any other default op1tions and/or rules
});

var formhandler = {
    init: function () {
        formhandler.countDownCharacter();
        formhandler.resetFormOnModalDismissed();
        formhandler.enableFormOnErrorModalDimissed();
        formhandler.checkFormOnInput();
        formhandler.setValueForRequiredRadioButton();
        formhandler.applyMaxLengh();
        formhandler.handelFormSubmit();
        formhandler.initDateTimePicker();
        formhandler.initTag();
    },

    initTag: function () {
        if ($.fn.tagEditor) {
            $(".tag").tagEditor({
                delimiter: ";, "
            });
        }
    },

    initDateTimePicker: function () {
        if ($.fn.datetimepicker) {
            $(".datetimepicker").datetimepicker({
                format: "DD/MM/YYYY hh:mm:A",
                useCurrent: false,
                maxDate: moment().endOf("d"),
                sideBySide: true
            });
        }
    },

    applyMaxLengh: function () {
        var textInput = $("input[type='text'], textarea");
        textInput.each(function (object) {
            if ($(this).attr("data-val-length-max") !== undefined && !$(this).hasClass("skip-limit")) {
                var maxChar = $(this).attr("data-val-length-max");
                $(this).attr("maxlength", maxChar);
            }
        });
    },

    setValueForRequiredRadioButton: function () {
        $(".required-radio").on("change",
            function () {
                $(".required-radio-value").val($(".required-radio:checked").val());
                $(".required-radio-value").valid();
            });
    },

    checkValidFilesIfDropzoneExisted: function () {
        if (typeof dropzoneobject !== "undefined") {
            return tdz.CheckValidFiles();
        } else {
            return true;
        }
    },

    checkFormAndTurnSubmitButton: function () {
        var validate = $(".content form").validate();
        var formSummittedState = validate.submitted;
        var isValid = validate.checkForm();
        if (isValid && formhandler.checkValidFilesIfDropzoneExisted()) { }

        $(".content form").validate().submitted = formSummittedState;
    },

    countDownCharacter: function () {
        $("form").on("keyup",
            ".description",
            function () {
                var maxLength = $(this).attr("maxLength");
                var remainLength = maxLength - $(this).val().length;
                var parent = $(this).closest(".item");
                if ($(".countdown-character").length) {
                    $(parent).find(".countdown-character").text(remainLength);
                }
            });
    },

    resetFormOnModalDismissed: function () {
        $("#notification-modal").on("hidden.bs.modal",
            function () {
                location.reload();
                $(".content form").validate().resetForm();
                $(".content form")[0].reset();
            });
    },

    enableFormOnErrorModalDimissed: function () {
        $("#error-modal").on("hidden.bs.modal",
            function () {
                $(".content form input[type=submit]").prop("disabled", false);
            });
    },

    checkFormOnInput: function () {
        $(".content form").on("input",
            "input[type='text'], textarea",
            function () {
                formhandler.checkFormAndTurnSubmitButton();
            });
    },

    handelFormSubmit: function () {
        $(".content form").on("submit",
            function (e) {
                if ($(this).valid()) {
                    if (formhandler.checkValidFilesIfDropzoneExisted()) {

                        $(".content form input[type=submit]").prop("disabled", true);

                        e.preventDefault();
                        $.ajax({
                            url: $(this).attr("action"),
                            method: $(this).attr("method"),
                            data: $(this).serialize(),
                            success: function (data) {
                                if (data.result === true) {
                                    $("#notification-modal").modal();
                                } else {
                                    if (data.errors !== undefined) {
                                        var validator = $(".content form").validate();
                                        var errors = {};

                                        data.errors.forEach(function (item, index) {
                                            errors[item.key] = item.errorMessage;
                                        });

                                        validator.showErrors(errors);

                                        $(".content form input[type=submit]").prop("disabled", false);
                                    }
                                }
                            },
                            error: function (request, error) {
                                $("#notification-modal div.modal-header > h4").text(error);
                                $("#notification-modal div.modal-body > p")
                                    .text("Error status code: " + request.status);
                                $("#notification-modal").modal();
                            }
                        });
                    } else {
                        // Errors in files
                        var englishText = tdz.BuildFileErrorText();
                        tdz.SetErrorTextAndShowModal(englishText);
                        e.preventDefault();
                        return false;
                    }
                }

                return true;
            });
    }
}

$(document).ready(function () {
    formhandler.init();
});