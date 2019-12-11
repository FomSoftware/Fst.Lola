var NotificationManager = function() {

    var url;
    var urlSetRead;
    var interval;
    var notifications;
    var currentNotification;
    var context = this;

    context.buildDuration = function (expiredSpan) {
        var res = "";
        if (expiredSpan.days != null) {
            res = `${expiredSpan.days}<em>d</em>`;
        }
        if (expiredSpan.hours != null) {

            res = `${res} ${expiredSpan.hours}<em>h</em>`;
        }
        if (expiredSpan.minutes != null && context.showMinutes(expiredSpan)) {

            res = `${res} ${expiredSpan.minutes}<em>min</em>`;
        }
        if (expiredSpan.seconds != null && context.showMinutes(expiredSpan)) {

            res = `${res} ${expiredSpan.seconds}<em>s</em>`;
        }

        return res;
    }

    context.showSeconds = function (duration) {
        if ((duration.days != null && duration.hours != null) ||
            (duration.hours != null && duration.minutes != null))
            return false;
        else
            return true;
    }

    context.showMinutes = function (duration) {
        if (duration.days != null && duration.hours != null)
            return false;
        else
            return true;
    }

    context.buildToast = function (notification) {
        return toastr.info(`<div><h6>${notification.machineName}</h6><hr /><br /><p>${notification.description}</p> <hr /> <br />${context.buildDuration(notification.expiredSpan)}</div>`);
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