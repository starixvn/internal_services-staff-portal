var reference = (function ($) {
    var originForm = $("form").serialize();

    var userSubmit = 0;

    var originReference = $(".reference-entry").length;

    var domElement = {
        DateTimePicker: ".datetimepicker",
        ImageUrl: ".image-url",
        ReferenceEntry: ".reference-entry",
        ReferenceZone: "#ReferencesZone",
        LimitMessagePlace: "#limitEntry",
        AddReferenceButton: "#addReferenceEntry",
        RemoveReferenceButton: ".remove-entry",
        InputValidationError: "input-validation-error",
        UploadImageValidation: ".upload-image-validation",
        FieldValidationError: ".field-validation-error",
        SubmitButtion: "#submit",
        UploadImageButton: ".upload-image",
        Form: "form"
    }

    function initDateTimePicker() {
        var nowDate = new Date();
        $(domElement.DateTimePicker).datetimepicker({
            format: "MM-DD-YYYY",
            useCurrent: false,
            minDate: nowDate.toDateString(),
            maxDate: nowDate.setFullYear(nowDate.getFullYear() + 1),
            disabledDates: getSelectedDates()
        });
    }

    function getSelectedDates() {
        var choosenDates = [];
        $(domElement.DateTimePicker).each(function (i, e) {
            var choosenDate = $(this).val();
            choosenDates.push(choosenDate);
        });
        return choosenDates;
    }

    function updateDisabledDates() {
        $(domElement.DateTimePicker).each(function (i, e) {
            $(this).data("DateTimePicker").disabledDates(getSelectedDates());
        });
    }

    function registerChangeDateEvent() {
        $(domElement.DateTimePicker).on("dp.change", function () {
            updateDisabledDates();
        });
    }
    
    function removeLimitWarning() {
        var limit = $(domElement.AddReferenceButton).data("limit");
        var length = $(domElement.ReferenceEntry).length;
        if (length <= limit) {
            $(domElement.LimitMessagePlace).text("");
        }
    }

    function registerUploadImageButtonEvent() {
        $(domElement.UploadImageButton).on("change", function (e) {
            var btn = $(this);
            var parent = $(btn).closest(domElement.ReferenceEntry);
            var files = e.target.files;
            if (files.length > 0) {
                if (window.FormData !== undefined) {
                    var data = new FormData();
                    data.append("file", files[0]);
                    $.ajax({
                        type: "POST",
                        url: $(btn).data("href"),
                        contentType: false,
                        processData: false,
                        data: data,
                        success: function (result) {
                            if (result.isSuccessful) {
                                parent.find(domElement.ImageUrl).val(result.imageDirectory);
                                parent.find(domElement.ImageUrl).removeClass(domElement.InputValidationError);
                                parent.find(domElement.FieldValidationError).addClass("hidden");
                                parent.find(domElement.UploadImageValidation).text("");
                            } else {
                                parent.find(domElement.ImageUrl).addClass(domElement.InputValidationError);
                                parent.find(domElement.UploadImageValidation).text(result.message);
                                parent.find(domElement.ImageUrl).val("");
                                parent.find(domElement.UploadImageButton).val("");
                            }
                        },
                        error: function (xhr, status, p3, p4) {
                            var err = "Error " + " " + status + " " + p3 + " " + p4;
                            if (xhr.responseText && xhr.responseText[0] === "{")
                                err = JSON.parse(xhr.responseText).Message;
                            toastr.error(err);
                        }
                    });
                } else {
                    alert("This browser doesn't support HTML5 file uploads!");
                }
            }
        })
    }

    function updateFormFieldsName() {
        initDateTimePicker();
        $(domElement.ReferenceEntry).each(function (index, element) {
            $(element).find(domElement.DateTimePicker).attr("name", "References[" + index + "].DateAppear");
            $(element).find(domElement.ImageUrl).attr("name", "References[" + index + "].ImageLink");
            $(element).find(".reference-id").attr("name", "References[" + index + "].Id");
            $(element).find(".date span").attr("data-valmsg-for", "References[" + index + "].DateAppear");
            $(element).find(".url span").attr("data-valmsg-for", "References[" + index + "].ImageLink");
        });
    }

    function setSubmitButtonState() {
        var isEmptyReference = $(domElement.ReferenceEntry).length == 0;
        $(domElement.SubmitButtion).prop("disabled", isEmptyReference);
    }

    function isLimitedReference(limitNumber) {
        var totalEntries = $(domElement.ReferenceEntry).length;
        return totalEntries >= limitNumber;
    }

    function eventClickAddNewReference() {
        $(domElement.AddReferenceButton).click(function () {
            var addBtn = $(this);
            var limitEntry = $(addBtn).data("limit");
            var url = $(addBtn).data("href");
            $.get(url, function (html) {
                if (!isLimitedReference(limitEntry)) {
                    $(domElement.ReferenceZone).append(html);
                    onAddNewReferenceSuccessful();
                } else {
                    var message = "You only can upload " + limitEntry + " files of attachment";
                    $(domElement.LimitMessagePlace).text(message);
                }
            });
        });
    }

    function onAddNewReferenceSuccessful() {
        updateFormFieldsName();
        registerRemoveReferenceButtonEvent();
        registerUploadImageButtonEvent();
        updateDisabledDates();
        registerChangeDateEvent();
        setSubmitButtonState();
        initValidationForm(domElement.Form);
    }

    function registerRemoveReferenceButtonEvent() {
        $(domElement.RemoveReferenceButton).on("click", function () {
            $(this).parent().parent().remove();
            updateFormFieldsName();
            updateDisabledDates();
            removeLimitWarning();
            if (originReference == 0) {
                setSubmitButtonState();
            }
        });
    }

    function eventClickSubmitButton() {
        $(domElement.Form).on("submit", function (e) {
            userSubmit++;
            $form = $(this);
            e.preventDefault();
            $.ajax({
                url: $($form).attr("action"),
                data: $($form).serialize(),
                method: "post",
                success: function (response) {
                    onSuccess(response);
                }
            });
        });
    }

    function initValidationForm($form) {
        $($form).removeData("validator");
        $($form).removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($form);
    }

    function showErrorMessage($form, errors) {
        errors.forEach(function (error) {
            $($form).find("input[name='" + error.key + "']").addClass("input-validation-error");
            $($form).find('[data-valmsg-for="' + error.key + '"]:last').text(error.errorMessage);
        });
    }

    function eventLeavePage() {
        $(window).on("beforeunload", function () {
            var formAfterChange = $(domElement.Form).serialize();
            if (originForm != formAfterChange && userSubmit == 0) {
                return true;
            }
        });
    }

    function onSuccess(response) {
        if (response.isSuccessful) {
            toastr.success(response.message);
            $(".reference-entry").each(function (i, e) {
                $(this).find(".image-url").removeClass("input-validation-error");
                $(this).find(".field-validation-valid").text("");
            });
            setTimeout(function () { location.reload(true); }, 2000);
        } else {
            userSubmit = 0;

            if (response.errors) {
                showErrorMessage(domElement.Form, response.errors);
            }

            if (response.message) {
                toastr.error(response.message);
            }
        }
    }

    function onFailure(response) {
        userSubmit = 0;
        toastr.error("500 Internal Server Error");
    }

    function initModule() {
        initDateTimePicker();
        eventClickAddNewReference();
        registerRemoveReferenceButtonEvent();
        registerUploadImageButtonEvent();
        eventClickSubmitButton();
        eventLeavePage();
        setSubmitButtonState();
    }

    return {
        initModule: initModule
    }
})(jQuery);

$(document).ready(function () {
    reference.initModule();
})