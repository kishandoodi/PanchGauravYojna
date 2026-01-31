window.toast = {
    showToast: function (heading, message, icon) {
        $.toast({
            heading: heading,
            text: message,
            icon: icon,
            position: 'bottom-right',
            loaderBg: '#000',
            hideAfter: 4000
        });
    }
};
