// Extend the animate.css
$.fn.extend({
    animateCss: function (animationName) {
        var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        this.addClass('animated ' + animationName).one(animationEnd, function () {
            $(this).removeClass('animated ' + animationName);
        });
        return this;
    }
});

$.fn.extend({
    startValidateEffect: function() {
        var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        var animationName = $(this).data("validateeffect");
        this.addClass('animated ' + animationName).one(animationEnd, function () {
            $(this).removeClass('animated ' + animationName);
        });
        return this;
    }
});

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

$(document).ready(function() {
    //appendSubMenu();
    $(".rippler").rippler({
        effectClass: 'rippler-effect'
       , effectSize: 16      // Default size (width & height)
       , addElement: 'div'   // e.g. 'svg'(feature)
       , duration: 400
    });
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $('input[type="text"]').focus(function () {
            $(this)[0].scrollIntoView();
        });
        $('textarea').focus(function () {
            $(this)[0].scrollIntoView();
        });
    }
    $("body").on("click",
        ".icon-calendar",
        function() {
            var datetimepickerElements = $(this).parent().find(".datetimepicker");
            $(datetimepickerElements).data("DateTimePicker").toggle();
        });
});