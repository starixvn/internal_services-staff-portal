ReInitDateTimePicker();

$("#AddImage").click(function () {
    $.get('/Admin/References/GetReferenceEntity?referenceType=0', function (html) {
        if (checkLimitedImage(31)) {
            $("#ImageZone").append(html);
            UpdateFormFieldsName();
            ReInitDateTimePicker();
        } else {
            alert("Limit of images is 31.");
        }
    });
});

$("#AddImageNewsLetter").click(function () {
    $.get('/Admin/References/GetReferenceEntity?referenceType=4', function (html) {
        if (checkLimitedImage(50)) {
            $("#ImageZone").append(html);
            UpdateFormFieldsName();
        } else {
            alert("Limit of images is 50.");
        }
    });
});

$("form").on("click", ".remove-entry", function () {
    $(this).parent().parent().remove();
    UpdateFormFieldsName();
    ReInitDateTimePicker();
});

$("form").on("dp.hide", ".datetimepicker", function (e) {
    ReInitDateTimePicker();
})

$("form").on("dp.show", ".datetimepicker", function (e) {
    ReInitDateTimePicker();
})

$("form").on("dp.change", ".datetimepicker", function (e) {
    ReInitDateTimePicker();
})

function ReInitDateTimePicker() {
    $(".datetimepicker").each(function () {
        InitDateTimePicker();
    });
}

function InitDateTimePicker() {
    var nowDate = new Date();
    $(".datetimepicker").datetimepicker({
        format: "MM-DD-YYYY",
        minDate: nowDate.setDate(nowDate.getDate() - 1),
        maxDate: nowDate.setFullYear(nowDate.getFullYear() + 1),
        disabledDates: GetSelectedDates()
    });
}

function GetSelectedDates() {
    var selectedDates = [];
    $(".datetimepicker").each(function (i, e) {
        var selectedDate = $(this).val();
        selectedDates.push(ConvertDate(selectedDate));
    });
    return selectedDates;
}

function checkLimitedImage(limit) {
    var totalEntries = $(".reference-entry").length;
    if (totalEntries >= limit) return false;
    return true;
}

function UpdateFormFieldsName() {
    $(".reference-entry").each(function (i, e) {
        UpdateReferencesIndex(i, e);
    });
}

function UpdateReferencesIndex(index, element) {
    $(element).find(".datetimepicker").attr("name", "References[" + index + "].DateAppear");
    $(element).find(".image-url").attr("name", "References[" + index + "].ImageLink");
}

function ConvertDate(date) {
    var newdate = date.split("-").reverse().join("-");
    return newdate;
}

$("#ImageZone").on("change", ".upload-image", function (e) {
    var uploadEntry = $(this).parent();
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
                        uploadEntry.find(".image-url").val(result.filePath);
                    } else {
                        uploadEntry.find(".image-url").addClass("error");
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