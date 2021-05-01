"use strict";

// Class definition
var KTIntro = function () {
    // Private functions
    var initIntro1 = function() {
        var element = document.querySelector('#kt_explore_toggle_label');

        if (!element) {
            return;
        }

        var popover = new bootstrap.Popover(element, {
            customClass: 'popover-dark',
            trigger: 'hover',
            container: 'body',
            boundary: 'window',
            placement: 'right',
            html: true,
            title: 'Explore Metronic',
            content: 'Looking for more layouts? Check out Metronic\'s other amazingly unique demos to better suit your project needs.'
        });

        popover.show();

        // Remove
        setTimeout(function(params) {
            if (popover) {
                popover.dispose();
            } 
        }, 1000 * 10);  // hide in 5 seconds
    }

    var initIntro2 = function() {
        var element = document.querySelector('#kt_header_search_toggle');

        if (!element) {
            return;
        }

        var popover = new bootstrap.Popover(element, {
            customClass: 'popover-dark',
            container: 'body',
            trigger: 'hover',
            boundary: 'window',
            placement: 'left',
            html: true,
            title: 'Quick Search',
            content: 'Click here to check out our brand new, frontend ready Quick Search feature.'
        });

        popover.show();

        // Remove
        setTimeout(function(params) {
            if (popover) {
                popover.dispose();
            } 
        }, 1000 * 10);
    }

    var initIntro3 = function() {
        var element = document.querySelector('#kt_toolbar_primary_button');

        if (!element) {
            return;
        }

        var popover = new bootstrap.Popover(element, {
            customClass: 'popover-dark',
            container: 'body',
            trigger: 'hover',
            boundary: 'window',
            placement: 'left',
            html: true,
            title: 'Modal Form Wizard',
            content: 'Wizard Modals provides an exceptional solution for any ad-hoc form requirement.'
        });

        popover.show();

        // Remove
        setTimeout(function(params) {
            if (popover) {
                popover.dispose();
            } 
        }, 1000 * 10);
    }

    var initIntro4 = function() {
        var element = document.querySelector('#kt_header_user_menu_toggle');

        if (!element) {
            return;
        }

        var popover = new bootstrap.Popover(element, {
            customClass: 'popover-dark',
            container: 'body',
            trigger: 'hover',
            boundary: 'window',
            placement: 'left',
            html: true,
            title: 'Advanced User Menu',
            content: 'Explore our updated User Menus that fits perfectly within any project.'
        });

        popover.show();

        // Remove
        setTimeout(function(params) {
            if (popover) {
                popover.dispose();
            }            
        }, 1000 * 10);
    }

    var handleDisplay = function(params) {
        var date = new Date();
        var expires = 1000 * 60 * 60 * 24 * 10; // 1 day
             
        if (!KTCookie.get('show_intro_2')) {
            setTimeout(function() {
                initIntro2();
            }, 1000 * 5);            

            KTCookie.set("show_intro_2", 1, {expires: new Date(date.getTime() + expires)});
        } else if (!KTCookie.get('show_intro_3')) {
            setTimeout(function() {
                initIntro3();
            }, 1000 * 5);

            KTCookie.set("show_intro_3", 1, {expires: new Date(date.getTime() + expires)});
        } else if (!KTCookie.get('show_intro_4')) {
            setTimeout(function() {
                initIntro4();
            }, 1000 * 5);

            KTCookie.set("show_intro_4", 1, {expires: new Date(date.getTime() + expires)});
        }   
    }

    // Public methods
    return {
        init: function () {
            if (KTUtil.inIframe() === false) {
                handleDisplay();
            } 
        }   
    }
}();

// Webpack support
if (typeof module !== 'undefined') {
    module.exports = KTIntro;
}

// On document ready
KTUtil.onDOMContentLoaded(function() {
    KTIntro.init();
});
