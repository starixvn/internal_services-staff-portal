$(document).ready(function () {
    document.forms[0].onsubmit = function () {
        var isValid = true;
        var username = $('#Username').val(),
            password = $('#txtPassword').val();

        if (username.length === 0) {
            isValid = false;
        }

        if (password.length === 0) {
            isValid = false;
        } else {
            var encryptedPwd = CryptoJS.CfsEnCode(password),
                hashPwd = CryptoJS.SHA256(encryptedPwd);

            $('#hashedPassword').val(hashPwd);
        }

        return isValid;
    };
});