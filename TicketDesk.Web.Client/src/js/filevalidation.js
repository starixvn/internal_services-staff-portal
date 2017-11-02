var files = [];

function getFileExtension(fileName) {
    if (/[.]/.exec(fileName)) {
        return "." + /[^.]+$/.exec(fileName)[0].toLowerCase();
    }
    return null;
}

$.validator.unobtrusive.adapters.add('filetype',
    ['validtypes'],
    function (options) {
        options.rules['filetype'] = { validtypes: options.params.validtypes.split(',') };
        options.messages['filetype'] = options.message;
    });

$.validator.addMethod("filetype",
    function (value, element, param) {
        for (var i = 0; i < element.files.length; i++) {
            var extension = getFileExtension(element.files[i].name);
            if ($.inArray(extension, param.validtypes) === -1) {
                return false;
            }
        }
        return true;
    });

$.validator.unobtrusive.adapters.add('numberoffile',
    ['max'],
    function (options) {
        options.rules['numberoffile'] = { max: options.params.max };
        options.messages['numberoffile'] = options.message;
    });

$.validator.addMethod("numberoffile",
    function (value, element, params) {
        if (element.files.length > params.max) {
            return false;
        }
        return true;
    });

$.validator.unobtrusive.adapters.add('filesize',
    ['maxbytes'],
    function (options) {
        // Set up test parameters
        var params = {
            maxbytes: options.params.maxbytes
        };

        // Match parameters to the method to execute
        options.rules['filesize'] = params;
        if (options.message) {
            // If there is a message, set it for the rule
            options.messages['filesize'] = options.message;
        }
    });

$.validator.addMethod("filesize",
    function (value, element, param) {
        if (value === "") {
            // no file supplied
            return true;
        }

        var maxBytes = parseInt(param.maxbytes);

        // use HTML5 File API to check selected file size
        // https://developer.mozilla.org/en-US/docs/Using_files_from_web_applications
        // http://caniuse.com/#feat=fileapi
        if (element.files !== undefined) {
            var totalBytes = 0;
            var numberOfFiles = element.files.length;

            for (var i = 0; i < numberOfFiles; i++) {
                if (element.files[i] != undefined && element.files[i].size != undefined) {
                    var filesize = parseInt(element.files[0].size);
                    totalBytes = totalBytes + filesize;
                }
            }

            if (totalBytes > maxBytes) {
                return false;
            }
        }

        // if the browser doesn't support the HTML5 file API, just return true
        // since returning false would prevent submitting the form 
        return true;
    });

function pushFilesToArray(x) {
    for (var i = 0; i < x.files.length; i++) {
        var file = x.files[i];
        if ($.inArray(file, files) === -1) {
            //file not existed in array
            files.push(file);
        }
    }
}

function displaySelectedFiles() {
    $("#file-list").html("");

    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        if ('name' in file) {
            var fileElement = $("<div>");
            //fileElement.data("fileData", file);
            fileElement.text(file.name);

            var deleteButton = $("<button>", { "class": "delete-file" });
            deleteButton.text("Delete");
            deleteButton.data("fileData", file);

            deleteButton.on("click",
                function() {
                    var indexToRemove = files.indexOf($(this).data("fileData"));
                    $(this).parent().remove();
                    files.splice(indexToRemove, 1);
                });

            fileElement.append(deleteButton);

            $("#file-list").append(fileElement);
        }
    }
}

function handleFileSelect(x) {
    pushFilesToArray(x);
    displaySelectedFiles();
}

Dropzone.options.ticketDeskDropzone = {
    url: '@Url.Action("Upload", "File", new { tempId = ViewBag.TempId })',
    uploadMultiple: true,
    maxFilesize: 4,
    maxFiles: 3,
    createImageThumbnails: false,
    acceptedFiles: ".jpg, .jpeg, .png, .pdf",
    addRemoveLinks: true,
    init: function () {
        this.on("removedfile",
            function (file) {
                console.log(file);
                var deleteUrl = '@Url.Action("Delete", "File", new {id = ViewBag.TempId})' + "&fileName=" + file.name;
                $.ajax({
                    url: deleteUrl,
                    method: "POST"
                });
            });
    }
}

//$(document).ready(function () {
//    $("#file-list").on("click",
//        ".delete-file",
//        function() {

//        });

//    $("input[type=file]").on("change",
//        function (x) {
//            $(this).valid();
//            handleFileSelect(x.target);
//        });

//    $("input[type=file]").closest("form").on("submit",
//        function (e) {
//            e.preventDefault();
//            var formData = new FormData($(this));

//            // Append files
//            for (var i = 0; i < files.length; i++) {
//                formData.append("file", files[i]);
//            }

//            $.ajax({
//                url: $(this).action,
//                data: formData,
//                type: "POST",
//                processData: false,
//                contentType: false,
//                success: function (data) {
//                    console.log("Success");
//                },
//                error: function (data) {
//                    console.log("Error");
//                }
//            });
//        });
//})