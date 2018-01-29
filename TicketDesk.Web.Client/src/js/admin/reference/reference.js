var reference = {
    initReference: function () {
        reference.initDateTimePicker();
        reference.setSubmitButtonState();

        if ($("#successMessage").length) {
            setTimeout(function () {
                $("#successMessage").hide();
            }, 5000);
        }
    },

    initDateTimePicker: function () {
        var nowDate = new Date();
        $(".datetimepicker").datetimepicker({
            format: "MM-DD-YYYY",
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

    addReferenceEntry: function (isNewsLetter, limitEntry) {
        var url = "/Admin/References/GetReferenceEntity?isNewsLetter=" + isNewsLetter
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
        reference.updateReferenceState();
    }
};

var originForm = JSON.stringify($("form").serializeArray());
var userSubmit = 0;
reference.initReference();

$("#addReferenceEntry").click(function () {
    var type = $(this).attr("data-type");
    var isNewLetter = type == "newsLetter" ? true : false;
    var limitEntry = isNewLetter ? 50 : 31
    reference.addReferenceEntry(isNewLetter, limitEntry);
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

$("form").submit(function () {
    userSubmit = 1;
});

$(window).on('beforeunload', function (e) {
    var formAfterChange = JSON.stringify($("form").serializeArray());
    if (originForm != formAfterChange && userSubmit == 0) {
        return true;
    }
});