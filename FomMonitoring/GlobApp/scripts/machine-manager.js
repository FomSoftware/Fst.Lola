var MachineManager = function ()
{
    var urlMachineAPI;

    var init = function (url)
    {
        initFlipAndSwipMenu();

        urlMachineAPI = url;

        // DOM updated
        Vue.nextTick(function ()
        {
            $('#MachineLevel').addClass('show');
            initFlipCard();
            initProgressBar();
            initScrollBar();             
        });

        
    }

    var callAjaxMachineViewModelData = function (filters)
    {
        var request = $.ajax({
            type: "POST",
            url: urlMachineAPI,
            contentType: 'application/json',
            data: JSON.stringify(filters),
            beforeSend: function ()
            {
                WaitmeManager.start('body');
            },
            complete: function ()
            {
                WaitmeManager.end('body');
            }
        });

        request.done(function (data)
        {
            $(".slimscroll").slimScroll({ destroy: true });

           
            SharedManager.updateLastUpdate(data.LastUpdate);

            Efficiency.update(data.Efficiency);
            Productivity.update(data.Productivity);
            //Alarms.update(data.Alarms);
            Spindles.update(data.Spindles);
            XSpindles.update(data.XSpindles);
            Tools.update(data.Tools);
            XTools.update(data.XTools);
            Jobs.update(data.Jobs);
            Messages.update(data.Messages);

            Vue.nextTick(function ()
            {
                initProgressBar();
                initFlipCard();
                initScrollBar();            
                initColumnHeader();
            });
           
        });

        request.fail(function (jqXHR, textStatus, errorThrown)
        {
            console.debug(jqXHR);
            console.debug(textStatus);
            console.debug(errorThrown);
            location.reload();
        });
    }

    var initProgressBar = function ()
    {
        $('.progress .progress-bar').css("width",
           function () { return $(this).attr("aria-valuenow") + "%"; });


        $('.overfeed .overfeed-bar').css("height",
            function () { return $(this).attr("aria-valuenow") + "%"; });
    }

    var initScrollBar = function ()
    {
        $('.slimscroll').each(function () {

            var hh = 244;
            // se sono in una tabella con il titolo fisso lo slimscroll contiene solo il tbody e non il theader
            var tableContainer = $(this).closest('.table-container');
            if (tableContainer.length > 0) {             
                hh = hh - tableContainer.first().children('.table-header').height();
            }
            
            $(this).slimScroll({
                size: '5px',
                height: hh + 'px',
                alwaysVisible: false,
                //wheelStep: 10,
                touchScrollStep: 35,
                color: '#999',
                allowPageScroll: true
            });
        });
    }

   
    var initFlipCard = function ()
    {
        $(".card").flip({
            axis: "y",
            reverse: true,
            trigger: "click"
        });
    }

    var initFlipAndSwipMenu = function ()
    {
        $(".card-portlet").flip({
            axis: "y",
            reverse: true,
            trigger: "manual"
        });

        $(".js-flip").click(function (e)
        {
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');

            var card_class = '.card-' + $(this).data('group');
            $(card_class).flip(true);
        });

        $(".js-unflip").click(function (e)
        {
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');

            var card_class = '.card-' + $(this).data('group');
            $(card_class).flip(false);
        });

        var swiper_efficiency = new Swiper('.swiper-container.efficiency', {
            direction: 'horizontal',
            loop: false
        });

        var swiper_productivity = new Swiper('.swiper-container.productivity', {
            direction: 'horizontal',
            loop: false
        })

        var swiper_alarm = new Swiper('.swiper-container.alarms', {
            direction: 'horizontal',
            loop: false
        })

        var swiper_message = new Swiper('.swiper-container.messages', {
            direction: 'horizontal',
            loop: false
        })

        $('.slide-summary').click(function (e) {
            e.preventDefault();
            var group = $(this).data('group');
            if (group == 'alarms')
                swiper_alarm.slideTo(1, 300);
            if (group == 'messages')
                swiper_message.slideTo(1, 300);

        });


        $('.slide-history').click(function (e)
        {
            e.preventDefault();
            var group = $(this).data('group');

            if (group == 'efficiency')
                swiper_efficiency.slideTo(0, 300);

            if (group == 'productivity')
                swiper_productivity.slideTo(0, 300);

            if (group == 'alarms')
                swiper_alarm.slideTo(0, 300);

            if (group == 'messages')
                swiper_message.slideTo(0, 300);
        });

        $('.slide-operator').click(function (e)
        {
            e.preventDefault();
            var group = $(this).data('group');

            if (group == 'efficiency')
                swiper_efficiency.slideTo(1, 300);

            if (group == 'productivity')
                swiper_productivity.slideTo(1, 300);
        });

        $('.slide-turni').click(function (e)
        {
            e.preventDefault();
            var group = $(this).data('group');

            if (group == 'efficiency')
                swiper_efficiency.slideTo(2, 300);

            if (group == 'productivity')
                swiper_productivity.slideTo(2, 300);
        });

    }

    var initVueComponents = function ()
    {
        Vue.component('no-data', {
            props: ['show'],
            template: '#no-data'
        });

        Vue.component('modal-tool', {
            props: ['tool'],
            template: '#modal-tool'
        });

        Vue.component('modal-spindle', {
            props: ['spindle'],
            template: '#modal-spindle'
        });
    }

    var getColorKPI = function (color_name)
    {
        var color;

        switch (color_name)
        {
            case 'color-red':
                color = '#cc3333';
                break;

            case 'color-yellow':
                color = '#ec0';
                break;

            case 'color-green':
                color = '#779933';
                break;
        }

        return color;
    }

    return {
        init: init,
        callAjaxMachineViewModelData: callAjaxMachineViewModelData,
        initVueComponents: initVueComponents,
        getColorKPI: getColorKPI
    }

}();