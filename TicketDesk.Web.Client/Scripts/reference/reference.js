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