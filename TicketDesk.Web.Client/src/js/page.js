//function appendSubMenu() {
//    $('.main-menu .menu-item').on('click', function(){
//       var text = $(this).children(".text").text();
//        var subMenu = $('.submenu-section');
//        var content = $(".content");
//        if(text == "References") {
//            subMenu.removeClass("hidden");
//            subMenu.empty().append("<ul class='submenu'>\
//                                            <li><a href='#' class=''><i class='icon-tv'></i><span class='text'>TV Schedules</span></a></li>\
//                                            <li><a href='#' class=''><i class='icon-feature'></i><span class='text'>Featured events</span></a></li>\
//                                            <li><a href='#' class=''><i class='icon-bet'></i><span class='text'>Bet of the day</span></a></li>\
//                                            <li><a href='#' class=''><i class='icon-trading'></i><span class='text'>Trading limits</span></a></li>\
//                                            <li><a href='#' class=''><i class='icon-newsletter'></i><span class='text'>Newsletters</span></a></li>\
//                                        </ul>");
//            content.addClass("menu-open");
            
            
//        }
//        else {
//            content.removeClass("menu-open");
//        }
//    });
//}

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

$(document).ready(function() {
    //appendSubMenu();
    $(".rippler").rippler({
        effectClass: 'rippler-effect'
       , effectSize: 16      // Default size (width & height)
       , addElement: 'div'   // e.g. 'svg'(feature)
       , duration: 400
    });
    $('input[type="text"]').focus(function () {
        $(this)[0].scrollIntoView();
    });
    $('textarea').focus(function () {
        $(this)[0].scrollIntoView();
    });
});