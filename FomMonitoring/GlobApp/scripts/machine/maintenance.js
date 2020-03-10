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
                show: {
                    historical: false
                },
                showed: true
            },
            computed: {                
            },
            methods: {       
                convert_timestamp: function (timestamp, utc) {
                    return moment.utc(timestamp).add(utc, 'hour').format('L LTS');
                },
                sortTimestamp: function () {

                    if (this.sorting.timestamp == 'desc') {
                        this.$data.details = _.sortBy(this.$data.details, function (message) { return message.timestamp; });
                        this.sorting.timestamp = 'asc';
                        return;
                    }

                    if (this.sorting.timestamp == 'asc' || this.sorting.timestamp == null) {
                        this.$data.details = _.sortBy(this.$data.details, function (message) { return message.timestamp; }).reverse();
                        this.sorting.timestamp = 'desc';
                        return;
                    }
                },

                ignoreMessage: function ignoreMessage(messageId, event) {
                    console.log(urlIgnoreMessageAPI);
                    callAjaxIgnoreMessage(messageId);
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
        /*request.done(function (data) {
            Maintenance.update(data);             

        });*/

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
        if (vm_messages) {
            vmMessages.messages = vm_messages.messages;
            vmMessages.sorting = vm_messages.sorting;
        }

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();