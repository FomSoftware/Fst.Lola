var NotificationManager = function() {

    var url;
    var urlSetRead;
    var interval;
    var notifications;
    var currentNotification;
    var context = this;



    context.buildToast = function (notification) {
        return toastr.info('<div><h4>${notification.panelName}</h4><h6>(${notification.machineSerial}) - ${notification.machineName}</h6><hr /><br /><p>${notification.description}</p></div>');
    }

    context.setNotificationRead = function () {
        var request = $.ajax({
            type: "POST",
            url: context.urlSetRead,
            data: JSON.stringify(context.currentNotification.id),
            contentType: 'application/json'
        });

        request.done(function () {

            context.notifications.shift();
            if (context.notifications.length > 0) {
                context.currentNotification = context.notifications[0];
                buildToast(context.notifications[0]);
            }
        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            console.error("jqXHR: " + jqXHR + " textStatus: " + textStatus + " errorThrown: " + errorThrown);
        });
    }

    context.init = function (url, urlSetRead) {
        context.url = url;
        context.urlSetRead = urlSetRead;

        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-bottom-right",
            "preventDuplicates": false,
            "showDuration": "0",
            "hideDuration": "0",
            "timeOut": "0",
            "extendedTimeOut": "0",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        }

        toastr.options.onHidden = function () {
            context.setNotificationRead();

        }

        context.update();
        context.interval = setInterval(function() {
            context.update();
        }, 10000);
    }



    context.update = function () {

        var request = $.ajax({
            type: "POST",
            url: this.url,
            contentType: 'application/json'
        });

        request.done(function (data) {
            context.notifications = data;
            if (context.currentNotification == null && context.notifications.length > 0) {
                toastr.clear();
                context.currentNotification = context.notifications[0];
                buildToast(context.notifications[0]);
            }
        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            console.error("jqXHR: " + jqXHR + " textStatus: " + textStatus + " errorThrown: " + errorThrown);
        });
    }


    return {
        init: context.init,
        update: context.update
    }
}()