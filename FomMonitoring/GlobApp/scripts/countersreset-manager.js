var CountersReset = function() {
    var vmCounters;
    var urlSearch;

    var init = function (data, urlReset) {
        initVueModel(data);
        urlSearch = urlReset;
        Vue.nextTick(function () {
            initScrollBar();
        });
    }

    var initVueComponents = function () {
        Vue.component('no-data', {
            props: ['show'],
            template: '#no-data'
        });

    };

    var initScrollBar = function () {
        $(".slimscrollTable").slimScroll({ destroy: true });
        $('.slimscrollTable').slimScroll({
                size: '5px',
                height: '100%',
                alwaysVisible: true,
                //wheelStep: 10,
                touchScrollStep: 35,
                color: '#999',
                allowPageScroll: true
            });

    }

    var search = function () {

        var request = $.ajax({
            type: "POST",
            url: urlSearch,
            contentType: 'application/json',
            data:  JSON.stringify($("#SearchvariableInput").val()),
            beforeSend: function () {
                WaitmeManager.start('body');
            },
            complete: function () {
                WaitmeManager.end('body');
            }
        });

        //non serve perchè il servizio ignoreMessage restituisce la nuova lista 
        request.done(function (data) {
            $(".slimscroll").slimScroll({ destroy: true });
            vmCounters.counters = data;
            Vue.nextTick(function () {
                initScrollBar();
            });
        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            console.debug(jqXHR);
            console.debug(textStatus);
            console.debug(errorThrown);
            location.reload();
        });
    }





    var initVueModel = function (data) {
        vmCounters = new Vue({
            el: '#CardCountersReset',
            data: {
                counters: data.lista
            },
            methods: {
                convert_timestamp: function (timestamp, utc) {
                    var m = moment.utc(timestamp);
                    if (this.timeZone && this.timeZone !== "")
                        m = m.tz(this.timeZone);
                    else
                        m = m.add(utc, 'hour');
                    var str = m.format('L') + " " + m.format('HH:mm:ss');
                    return str;
                },
                convert_time: function (timestamp, utc) {
                    var m = moment.utc(timestamp);
                    if (this.timeZone && this.timeZone !== "")
                        m = m.tz(this.timeZone);
                    else
                        m = m.add(utc, 'hour');
                    var str = m.format('HH:mm:ss');
                    return str;
                },
                convert_date: function (timestamp, utc) {
                    var m = moment.utc(timestamp);
                    if (this.timeZone && this.timeZone !== "")
                        m = m.tz(this.timeZone);
                    else
                        m = m.add(utc, 'hour');
                    var str = m.format('L');
                    return str;
                }

            }
        });
    }

    
    return {
        init: init,
        initScrollBar: initScrollBar,
        search: search,
        initVueComponents: initVueComponents
    }

}();
