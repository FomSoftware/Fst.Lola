var Maintenance = function ()
{
    var vmMessages;
    var urlIgnoreMessageAPI;

    var init = function (data, url)
    {
        initVueModel(data);
        urlIgnoreMessageAPI = url;      
    }

    var initVueModel = function (data)
    {
        vmMessages = new Vue({
            el: '#CardMaintenance',
            data: {
                messages: data.vm_messages.messages,
                sorting: data.vm_messages.sorting,
                sortingIgnoredMessages: data.ignored_messages.sorting,
                ignoredMessages: data.ignored_messages.messages,
                show: {
                    historical: (data.ignored_messages.messages != null)
                },
                timeZone: data.timeZone,
                showed: true
            },
            computed: {                
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
                sortTimestamp: function () {
                    console.log(this.$data);
                    if (this.sorting.timestamp == 'desc') {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.timestamp; });
                        this.sorting.timestamp = 'asc';
                        return;
                    }

                    if (this.sorting.timestamp == 'asc' || this.sorting.timestamp == null) {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.timestamp; }).reverse();
                        this.sorting.timestamp = 'desc';
                        return;
                    }
                },
                sortIgnoredMessagesTimestamp: function () {

                    console.log(this.$data);
                    if (this.sortingIgnoredMessages.timestamp == 'desc') {
                        this.$data.ignoredMessages = _.sortBy(this.$data.ignoredMessages, function (message) { return message.timestamp; });
                        this.sortingIgnoredMessages.timestamp = 'asc';
                        return;
                    }

                    if (this.sortingIgnoredMessages.timestamp == 'asc' || this.sortingIgnoredMessages.timestamp == null) {
                        this.$data.ignoredMessages = _.sortBy(this.$data.ignoredMessages, function (message) { return message.timestamp; }).reverse();
                        this.sortingIgnoredMessages.timestamp = 'desc';
                        return;
                    }
                },
                sortIgnoredMessagesUser: function () {

                    console.log(this.$data);
                    if (this.sortingIgnoredMessages.user == 'desc') {
                        this.$data.ignoredMessages = _.sortBy(this.$data.ignoredMessages, function (message) { return message.user; });
                        this.sortingIgnoredMessages.user = 'asc';
                        return;
                    }

                    if (this.sortingIgnoredMessages.user == 'asc' || this.sortingIgnoredMessages.user == null) {
                        this.$data.ignoredMessages = _.sortBy(this.$data.ignoredMessages, function (message) { return message.user; }).reverse();
                        this.sortingIgnoredMessages.user = 'desc';
                        return;
                    }
                },

                ignoreMessage: function ignoreMessage(messageId, event) {
                    var icon = event.currentTarget.getElementsByClassName('red-square-icon')[0];
                    if (icon) {
                        icon.className = icon.className.replace(className, "green-square-icon");
                    }

                    setTimeout(function() {
                        callAjaxIgnoreMessage(messageId);
                    }, 1000);

                },
                changeIconColor: function changeIconColor(className, event) {
                    var icon = event.currentTarget.getElementsByClassName(className)[0];
                    if (icon) {
                        if (className == 'red-square-icon')
                            icon.className = icon.className.replace(className, "green-square-icon");
                        else
                            icon.className = icon.className.replace(className, 'red-square-icon');
                    }
                }

            }
        });
    }

    var show = function() {
        vmMessages.showed = true;
    }

    var hide = function () {
        vmMessages.showed = false;
    }

    var callAjaxIgnoreMessage = function (messageId) {
       
        var request = $.ajax({
            type: "POST",
            url: urlIgnoreMessageAPI + "/" + messageId,
            contentType: 'application/json',
            //data: JSON.stringify(messageId),
            beforeSend: function () {
                WaitmeManager.start('body');
            },
            complete: function () {
                WaitmeManager.end('body');
            }
        });

        //non serve perchè il servizio ignoreMessage restituisce la nuova lista 
        request.done(function (data) {
            Maintenance.update(data);             

        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            console.debug(jqXHR);
            console.debug(textStatus);
            console.debug(errorThrown);
            location.reload();
        });
    }    

    var update = function (data)
    {
        // update vue model
        var vm_messages = data.vm_messages;
        var vm_ignored_messages = data.ignored_messages;
        vmMessages.timeZone = data.timeZone;
        if (vm_messages) {
            vmMessages.messages = vm_messages.messages;
            vmMessages.sorting = vm_messages.sorting;
        }
        if (vm_ignored_messages) {
            vmMessages.ignoredMessages = vm_ignored_messages.messages;
            vmMessages.sortingIgnoredMessages = vm_ignored_messages.sorting;
            vmMessages.show.historical = true;
        }
        else
            vmMessages.show.historical = false;
    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();