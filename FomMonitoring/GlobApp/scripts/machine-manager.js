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

        
    };

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
            Spindles.update(data.PanelParameter);
            XSpindles.update(data.XSpindles);
            ToolsBlitz.update(data.Tools);
            Tools.update(data.Tools);
            XTools.update(data.XTools);
            Jobs.update(data.Jobs);
            Messages.update(data.Messages);
            Maintenance.update(data.Maintenance);
            MotorAxesBlitz.update(data.PanelParameter);
            MotorKeope.update(data.PanelParameter);
            AxesKeope.update(data.PanelParameter);
            ElectroSpindle.update(data.PanelParameter);
            OtherData.update(data.PanelParameter);

            Vue.nextTick(function ()
            {
                initProgressBar();
                initFlipCard();
                initScrollBar();                          
            });
           
        });

        request.fail(function (jqXHR, textStatus, errorThrown)
        {
            console.debug(jqXHR);
            console.debug(textStatus);
            console.debug(errorThrown);
            location.reload();
        });
    };

    var initProgressBar = function ()
    {
        $('.progress .progress-bar').css("width",
           function () { return $(this).attr("aria-valuenow") + "%"; });


        $('.overfeed .overfeed-bar').css("height",
            function () { return $(this).attr("aria-valuenow") + "%"; });
    };

    var initScrollBar = function ()
    {
        $('.slimscroll').each(function () {

            var hh = 244;

            if ($(this).attr('id') == "maintenanceSlimscroll")
                hh = 525;

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
    };


    var initFlipCard = function ()
    {
        $(".card").flip({
            axis: "y",
            reverse: true,
            trigger: "click"
        });
    };


    var setGroupActive = function(group, itemActive) {
        $("[data-group='" + group + "']").removeClass("active");
        $(itemActive + "[data-group='" + group + "']").addClass("active");
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
            $(card_class + " .front").children().hide();
            $(card_class).flip(true);
            $(card_class + " .back").children().show();
        });

        $(".js-unflip").click(function (e)
        {
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');

            var card_class = '.card-' + $(this).data('group');


            $(card_class + " .back").children().hide();
            $(card_class).flip(false);
            $(card_class + " .front").children().show();
        });

        var swiper_efficiency = new Swiper('.swiper-container.efficiency', {
            direction: 'horizontal',
            loop: false
        });

        var swiper_productivity = new Swiper('.swiper-container.productivity', {
            direction: 'horizontal',
            loop: false
        });


        var swiper_message = new Swiper('.swiper-container.messages', {
            direction: 'horizontal',
            loop: false
        });

        $('.slide-dashboard').click(function (e) {
            e.preventDefault();
            var group = $(this).data('group');
            if (group == 'messages')
                swiper_message.slideTo(0, 300);
            if (group == 'efficiency')
                swiper_efficiency.slideTo(0, 300);
            if (group == 'productivity')
                swiper_productivity.slideTo(0, 300);

            setGroupActive(group, ".slide-dashboard");

        });

        $('.slide-summary').click(function (e) {
            e.preventDefault();
            var group = $(this).data('group');
            if (group == 'messages')
                swiper_message.slideTo(2, 300);

            setGroupActive(group, ".slide-summary");
        });


        $('.slide-history').click(function (e)
        {
            e.preventDefault();
            var group = $(this).data('group');

            if (group == 'efficiency')
                swiper_efficiency.slideTo(1, 300);

            if (group == 'productivity')
                swiper_productivity.slideTo(1, 300);

            if (group == 'messages')
                swiper_message.slideTo(1, 300);


            setGroupActive(group, ".slide-history");
        });

        $('.slide-operator').click(function (e)
        {
            e.preventDefault();
            var group = $(this).data('group');

            if (group == 'efficiency')
                swiper_efficiency.slideTo(2, 300);

            if (group == 'productivity')
                swiper_productivity.slideTo(2, 300);

            setGroupActive(group, ".slide-operator");
        });

        $('.slide-turni').click(function (e)
        {
            e.preventDefault();
            var group = $(this).data('group');

            if (group == 'efficiency')
                swiper_efficiency.slideTo(3, 300);

            if (group == 'productivity')
                swiper_productivity.slideTo(3, 300);

            setGroupActive(group, ".slide-turni");
        });

    };

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
    };

    var getColorKPI = function (color_name)
    {
        var color;

        switch (color_name)
        {
            case 'color-red':
                color = '#D32337';
                break;

            case 'color-yellow':
                color = '#F9ED4B';
                break;

            case 'color-green':
                color = '#8FBF36';
                break;
        }

        return color;
    };

    return {
        init: init,
        callAjaxMachineViewModelData: callAjaxMachineViewModelData,
        initVueComponents: initVueComponents,
        getColorKPI: getColorKPI
    };

}();