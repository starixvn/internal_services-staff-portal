Dropzone.autoDiscover = false;


var tdz = {
    isFileRequired: function () {
        return $("#dz-isfilerequired").text();
    },

    isMaxFilesViolated: false,

    maxNumber: function () {
        return $("#dz-maxnumberoffile").text();
    },

    isTotalSizeViolated: false,

    allowedBytes: function () {
        return $("#dz-maxbytes").text();
    },

    totalSize: function () {
        return tdz.allowedBytes() / 1048576;
    },

    isFileTypeViolated: false,

    fileType: function () {
        return $("#dz-acceptedextensions").text();
    },

    ResetError: function () {
        tdz.isMaxFilesViolated = false;
        tdz.isTotalSizeViolated = false;
        tdz.isFileTypeViolated = false;
    },

    CheckValidFiles: function () {
        if (dropzoneobject.ticketDeskDropzone.files.length > 0) {
            // have file(s)

            tdz.CheckTotalBytes();
            tdz.CheckTotalFiles(dropzoneobject.ticketDeskDropzone);
            tdz.CheckFileType(dropzoneobject.ticketDeskDropzone);

            var rejectedFiles = dropzoneobject.ticketDeskDropzone.getRejectedFiles();
            if (rejectedFiles.length > 0) {
                for (var i = 0; i < rejectedFiles.length; i++) {
                    if (rejectedFiles[i].status === "error" || rejectedFiles[i].status === "added") {
                        // have error files
                        return false;
                    }
                }
            }

            if (tdz.IsTotalBytesViolated()) {
                // Max size violated
                return false;
            }

            return true;
        }

        if (tdz.isFileRequired() === true) {
            // there is no files
            return false;
        } else {
            return true;
        }

    },

    CheckTotalFiles: function (dropzone) {
        var total = dropzone.files.length;
        if (total > tdz.maxNumber()) {
            tdz.isMaxFilesViolated = true;
        } else {
            tdz.isMaxFilesViolated = false;
        }
    },

    CheckFileType: function (dropzone) {
        var total = dropzone.files.length;
        tdz.isFileTypeViolated = false;
        for (var i = 0; i < total; i++) {
            var test = dropzone.files[i].name.split('.').pop();
            if (!tdz.fileType().includes(test)) {
                tdz.isFileTypeViolated = true;
                break;
            }
        }
    },

    BuildFileErrorText: function() {
        var text = "";

        var filerequired = $("#dz-error-filerequired").text();
        var maxfilesviolated = $("#dz-error-maxfilesviolated").text();
        var totalsizeviolated = $("#dz-error-totalsizeviolated").text();
        var filetypeviolated = $("#dz-error-filetypeviolated").text();
        var duplicatedfile = $("#dz-error-duplicatedfile").text();

        if (tdz.isFileRequired()) {
            text = filerequired;
        }
        if (tdz.isMaxFilesViolated) {
            text = text + maxfilesviolated;
        }
        if (tdz.isTotalSizeViolated) {
            text = text + totalsizeviolated;
        }
        if (tdz.isFileTypeViolated) {
            text = filetypeviolated;
        }

        return text;
    },

    SetErrorTextAndShowModal: function (text, vietnameseText) {
        if (text !== "") {

            var textToShow = text;

            var culture = getCookie("_culture");
            if (culture !== "en-US" && vietnameseText !== undefined) {
                textToShow = vietnameseText;
            }

            $(".dropzone-error-text").text(textToShow);
            if (($("element").data('bs.modal') || {}).isShown) {
                $('#attach-file-modal').modal('hide');
            }

            $("input").blur();
            $("#attach-file-modal").modal();
        }
    },

    ResumeQube: function (dropzone) {
        // Prevent removedfile event get called multiple time
        tdz.isRequeueing = true;

        var files = dropzone.getRejectedFiles();
        var copiedFiles = files.slice(0);
        for (var i = 0; i < copiedFiles.length; i++) {
            dropzone.removeFile(files[i]);
            dropzone.addFile(copiedFiles[i]);
        }

        tdz.isRequeueing = false;
    },

    GetTotalSize: function () {
        var totalBytes = 0;
        for (var i = 0; i < dropzoneobject.ticketDeskDropzone.files.length; i++) {
            if (dropzoneobject.ticketDeskDropzone.files[i] != undefined && dropzoneobject.ticketDeskDropzone.files[i].size != undefined) {
                var filesize = parseInt(dropzoneobject.ticketDeskDropzone.files[i].size);
                totalBytes = totalBytes + filesize;
            }
        }
        return totalBytes;
    },

    IsTotalBytesViolated: function () {
        var totalBytes = tdz.GetTotalSize();
        return totalBytes > tdz.allowedBytes();
    },

    CheckTotalBytes: function () {
        if (tdz.IsTotalBytesViolated()) {
            tdz.isTotalSizeViolated = true;
        } else {
            tdz.isTotalSizeViolated = false;
            tdz.SetErrorTextAndShowModal("");
        }
    },

    CheckDuplicateFile: function (dropzone, file) {
        if (dropzone.files.length) {
            var i, len;
            var isDuplicate = false;
            var errorText = $("#dz-error-duplicatedfile").text();

            for (i = 0, len = dropzone.files.length; i < len - 1; i++) {
                // -1 to exclude current file
                if (dropzone.files[i].name === file.name &&
                    dropzone.files[i].size === file.size &&
                    dropzone.files[i].lastModifiedDate.toString() === file.lastModifiedDate.toString()) {
                    dropzone.removeFile(file);
                    errorText += file.name + " | ";
                    isDuplicate = true;
                }
            }

            if (isDuplicate) {
                errorText = errorText.slice(0, -3);
                //tdz.SetErrorTextAndShowModal(errorText);
            } else {
                tdz.SetErrorTextAndShowModal("");
            }
        }
    },

    UpdateNumberOfSelectedFile: function (dropzone) {
        var numberOfSelectedFile = dropzone.files.length;
        $('.number-chosenfile .number').text(numberOfSelectedFile);
        if (numberOfSelectedFile > 1) {
            $('.btn-delete-all-file').show();
            $('.number-chosenfile').show();
            $('.number-chosenfile .text').text($("#dz-info-multiplefileselected").text());
        } else if (numberOfSelectedFile === 1) {
            $('.btn-delete-all-file').show();
            $('.number-chosenfile').show();
            $('.number-chosenfile .text').text($("#dz-info-singlefileselected").text());
        } else {
            $('.number-chosenfile').hide();
            $('.btn-delete-all-file').hide();
        }
    },

    isRequeueing: false,

    deleteAllFile: function () {
        $(".btn-delete-all-file").on("click",
            function () {
                var files = dropzoneobject.ticketDeskDropzone.files;
                for (var i = 0; i < files.length; i++) {
                    dropzoneobject.ticketDeskDropzone.removeFile(files[i]);
                }
            });
    },

    init: function () {
        tdz.deleteAllFile();
    }
}

var dropzoneobject = {
    ticketDeskDropzone: new Dropzone("#add-file-btn", {
        dictDefaultMessage: $("#dz-dict-dictDefaultMessage").text(),
        dictFallbackMessage: $("#dz-dict-dictFallbackMessage").text(),
        dictFallbackText: $("#dz-dict-dictFallbackText").text(),
        dictFileTooBig: $("#dz-dict-dictFileTooBig").text(),
        dictInvalidFileType: $("#dz-dict-dictInvalidFileType").text(),
        dictResponseError: $("#dz-dict-dictResponseError").text(),
        dictCancelUpload: $("#dz-dict-dictCancelUpload").text(),
        dictCancelUploadConfirmation: $("#dz-dict-dictCancelUploadConfirmation").text(),
        dictRemoveFile: $("#dz-dict-dictRemoveFile").text(),
        dictMaxFilesExceeded: $("#dz-dict-dictMaxFilesExceeded").text(),
        maxFilesize: tdz.totalSize(),
        url: $("#dz-action-upload").prop("href"),
        uploadMultiple: true,
        parallelUploads: 1,
        maxFiles: tdz.maxNumber(),
        createImageThumbnails: false,
        acceptedFiles: tdz.fileType(),
        addRemoveLinks: true,
        previewsContainer: ".dropzone-previews",
        clickable: "#add-file-btn",
        init: function () {
            this.on("removedfile",
                function (file) {
                    var deleteUrl = $("#dz-action-delete").prop("href") + "&fileName=" + encodeURIComponent(file.name);

                    if (file.status === "success") {
                        $.ajax({
                            url: deleteUrl,
                            method: "POST"
                        });
                        tdz.SetErrorTextAndShowModal("");
                        tdz.ResetError();
                        if (tdz.isRequeueing === false) {
                            tdz.ResumeQube(this);
                        }
                    }

                    tdz.UpdateNumberOfSelectedFile(this);
                    $(".filechange").trigger("filechange");
                });
            this.on("addedfile",
                function (file) {
                    $(".dropzone-previews").addClass("dropzone");

                    tdz.CheckTotalBytes();
                    tdz.CheckDuplicateFile(this, file);
                    tdz.UpdateNumberOfSelectedFile(this);
                    $(".filechange").trigger("filechange");
                });
            this.on("reset",
                function () {
                    $(".dropzone-previews").removeClass("dropzone");
                    tdz.SetErrorTextAndShowModal("");
                    $(".filechange").trigger("filechange");
                });
            this.on("maxfilesexceeded",
                function () {
                    tdz.isMaxFilesViolated = true;
                });
            this.on("error",
                function (file, errorMessage) {
                    //tdz.SetErrorTextAndShowModal(errorMessage);
                    var test = file.name.split('.').pop();
                    if (!tdz.fileType().includes(test)) {
                        tdz.isFileTypeViolated = true;
                    }
                });
            this.on("processing",
                function () {
                    $(".filechange").trigger("filechange");
                });
        }
    }),
}

$(document).ready(function () {
    tdz.init();
});