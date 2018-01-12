var reference = {

    InitDateTimePicker: function () {
        var nowDate = new Date();
        $(".datetimepicker").datetimepicker({
            format: "MM-DD-YYYY",
            minDate: new Date(),
            maxDate: nowDate.setFullYear(nowDate.getFullYear() + 1),
            disabledDates: reference.GetSelectedDates()
        });
    },

    GetSelectedDates: function () {
        var choosenDates = [];
        $(".datetimepicker").each(function (i, e) {
            var choosenDate = $(this).val();
            choosenDates.push(choosenDate);
        });
        return choosenDates;
    },

    UpdateDisabledDates: function () {
        $(".datetimepicker").each(function (i, e) {
            $(this).data("DateTimePicker").disabledDates(reference.GetSelectedDates());
        });
        
    },

    ConvertDate: function(oldDate) {
       var newDate = oldDate.split("-").reverse().join("-");
       return newDate;
    },

    UpdateReferenceIndex: function(index, element) {
        $(element).find(".datetimepicker").attr("name", "References[" + index + "].DateAppear");
        $(element).find(".image-url").attr("name", "References[" + index + "].ImageLink");
    },

    UpdateFormFieldsName: function () {
        reference.InitDateTimePicker();
        $(".reference-entry").each(function (i, e) {
            reference.UpdateReferenceIndex(i, e);
        });
    },

    CheckLimitedEntry: function(limitEntry) {
        var totalEntries = $(".reference-entry").length;
        if (totalEntries >= limitEntry) return false;
        return true;
    },

    AddReferenceEntry: function (referenceType, limitEntry) {
        var url = "/Admin/References/GetReferenceEntity?referenceType=" + referenceType
        $.get(url, function (html) {
            if (reference.CheckLimitedEntry(limitEntry)) {
                $("#ReferencesZone").append(html);
                reference.UpdateFormFieldsName();
                reference.UpdateDisabledDates();
            } else {
                alert("Limit of images is 31.");
            }
        });
    },

    RemoveEntry: function (element) {
        $(element).parent().parent().remove();
        reference.UpdateFormFieldsName();
        reference.UpdateDisabledDates();
    }
};

$(document).ready(reference.InitDateTimePicker());

$("#addReferenceEntry").click(function () {
    reference.AddReferenceEntry(1, 31);
});

$("#addNewsLetterReferenceEntry").click(function () {
    reference.AddReferenceEntry(0, 51);
});

$("form").on("click", ".remove-entry", function () {
    reference.RemoveEntry($(this));
});

$(document).on("dp.change", ".datetimepicker",function () {
    reference.UpdateDisabledDates();
});

$("#ImageZone").on("change", ".upload-image", function (e) {
    var uploadEntry = $(this).parent().parent();
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
                        console.log(result);
                        uploadEntry.find(".image-url").val(result.imageLink);
                    } else {
                        uploadEntry.find(".image-url").addClass("error");
                        uploadEntry.find(".image-url").val("");
                        uploadEntry.find(".upload-image").val("");
                        alert(result.message);
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