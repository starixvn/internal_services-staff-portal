$("#AddImage").click(function () {
    $.get('/Admin/References/ReferenceEntity?referenceType=0', function (html) {
        if (checkLimitedImage(31)) {
            $("#AddImageZone").append(html);
        } else {
            alert("Limit of images is 31.");
        }
    });
});

$("#AddNewsLetterImage").click(function () {
    $.get('/Admin/References/ReferenceEntity?referenceType=4', function (html) {
        if (checkLimitedImage(50)) {
            $("#AddImageZone").append(html);
        } else {
            alert("Limit of images is 50.");
        }
    });
});

function checkLimitedImage(limit) {
    var totalEntries = $(".reference-entry").length;
    if (totalEntries >= limit) return false;
    return true;
}

$("#AddImageZone").on("change", ".upload-image", function (e) {
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
                    console.log(result);
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                }
            });
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
        }
    }
});