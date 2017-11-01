function getFileExtension(fileName) {
    if (/[.]/.exec(fileName)) {
        return /[^.]+$/.exec(fileName)[0].toLowerCase();
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

$(document).ready(function() {
    $("input[type=file]").on("change",
        function () {
            console.log("changed");
            $(this).closest("form").valid();
        });
})