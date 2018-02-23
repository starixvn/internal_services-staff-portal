var reference = {
    initReference: function () {
        reference.initDateTimePicker();
        reference.setSubmitButtonState();
        reference.initToastr();
    },

    initToastr: function () {
        toastr.options = {
            "closeButton": true,
            "positionClass": "toast-top-right",
            "preventDuplicates": true,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        return toastr;
    },

    initDateTimePicker: function () {
        var nowDate = new Date();
        $(".datetimepicker").datetimepicker({
            format: "MM-DD-YYYY",
            useCurrent: false,
            minDate: nowDate.toDateString(),
            maxDate: nowDate.setFullYear(nowDate.getFullYear() + 1),
            disabledDates: reference.getSelectedDates()
        });
    },

    getSelectedDates: function () {
        var choosenDates = [];
        $(".datetimepicker").each(function (i, e) {
            var choosenDate = $(this).val();
            choosenDates.push(choosenDate);
        });
        return choosenDates;
    },

    updateDisabledDates: function () {
        $(".datetimepicker").each(function (i, e) {
            $(this).data("DateTimePicker").disabledDates(reference.getSelectedDates());
        });
    },

    updateReferenceState: function () {
        reference.updateFormFieldsName();
        reference.updateDisabledDates();
        reference.setSubmitButtonState();
    },

    updateReferenceIndex: function (index, element) {
        $(element).find(".datetimepicker").attr("name", "References[" + index + "].DateAppear");
        $(element).find(".image-url").attr("name", "References[" + index + "].ImageLink");
        $(element).find(".reference-id").attr("name", "References[" + index + "].Id");
        $(element).find(".date span").attr("data-valmsg-for", "References[" + index + "].DateAppear");
        $(element).find(".url span").attr("data-valmsg-for", "References[" + index + "].ImageLink");
    },

    updateFormFieldsName: function () {
        reference.initDateTimePicker();
        $(".reference-entry").each(function (i, e) {
            reference.updateReferenceIndex(i, e);
        });
    },

    checkLimitedEntry: function (limitEntry) {
        var totalEntries = $(".reference-entry").length;
        if (totalEntries >= limitEntry) return false;
        return true;
    },

    addReferenceEntry: function (isNewsletters, limitEntry) {
        var url = "/Admin/References/GetReferenceEntity?isNewsletters=" + isNewsletters
        $.get(url, function (html) {
            if (reference.checkLimitedEntry(limitEntry)) {
                $("#ReferencesZone").append(html);
                reference.updateReferenceState();
            } else {
                var message = "You only can upload " + limitEntry + " files of attachment";
                $("#limitEntry").text(message);
            }
        });
    },

    isEmptyReference: function () {
        if (!$(".reference-entry").length) {
            return true;
        }
        return false;
    },

    setSubmitButtonState: function () {
        $("#submit").prop("disabled", reference.isEmptyReference());
    },

    removeEntry: function (element) {
        $(element).parent().parent().remove();
        reference.updateFormFieldsName();
        reference.updateDisabledDates();
        if (originReference == 0) {
            reference.setSubmitButtonState();
        }
    }
};

var originForm = $("form").serialize();
var userSubmit = 0;
var originReference = $(".reference-entry").length;
reference.initReference();

$("#addReferenceEntry").click(function () {
    var type = $(this).attr("data-type");
    var isNewsletters = type == "newsLetters";
    var limitEntry = isNewsletters ? 50 : 31
    reference.addReferenceEntry(isNewsletters, limitEntry);
});

$("form").on("click", ".remove-entry", function () {
    reference.removeEntry($(this));
});

$(document).on("dp.change", ".datetimepicker", function () {
    reference.updateDisabledDates();
});

$("#ReferencesZone").on("change", ".upload-image", function (e) {
    var uploadEntry = $(this).closest(".reference-entry");
    var files = e.target.files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            var data = new FormData();
            data.append("file", files[0]);
            $.ajax({
                type: "POST",
                url: "/Admin/References/UploadImage",
                contentType: false,
                processData: false,
                data: data,
                success: function (result) {
                    if (result.isSuccessful) {
                        uploadEntry.find(".image-url").val(result.imageDirectory);
                        uploadEntry.find(".image-url").removeClass("input-validation-error");
                        uploadEntry.find(".field-validation-error").addClass("hidden");
                        uploadEntry.find(".upload-image-validation").text("");
                    } else {
                        uploadEntry.find(".image-url").addClass("input-validation-error");
                        uploadEntry.find(".upload-image-validation").text(result.message);
                        uploadEntry.find(".image-url").val("");
                        uploadEntry.find(".upload-image").val("");
                    }
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] === "{")
                        err = JSON.parse(xhr.responseText).Message;
                    alert(err);
                }
            });
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
        }
    }
});

$("#submit").click(function (e) {
    $("form").submit();
    userSubmit = 1;
    $(".reference-entry").find(".upload-image-validation").text("");
});

$(window).on('beforeunload', function () {
    var formAfterChange = $("form").serialize();
    if (originForm != formAfterChange && userSubmit == 0) {
        return true;
    }
});

var ajaxSubmitReference = {
    renderErrorsMessage: function (errors) {
        errors.forEach(function (error) {
            $("input[name='" + error.key + "']").addClass("input-validation-error");
            $('[data-valmsg-for="' + error.key + '"]:last').text(error.errorMessage);
        });
    },

    onSuccess: function (response) {
        userSubmit = 0;
        if (response.isSuccessful) {
            toastr.success(response.message);
            $(".reference-entry").each(function (i, e) {
                $(this).find(".image-url").removeClass("input-validation-error");
                $(this).find(".field-validation-valid").text("");
            });
            originForm = $("form").serialize();
            setTimeout(function () { location.reload(true); }, 3000);
        } else {
            if (response.errors) {
                ajaxSubmitReference.renderErrorsMessage(response.errors);
            }

            if (response.message) {
                toastr.error(response.message);
            }
        }
    },

    onFailure: function (response) {
        toastr.error("Error");
    }
};