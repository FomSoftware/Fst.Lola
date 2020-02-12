var XToolsLmx = function ()
{
    var vmTools;

    var init = function (data)
    {
        initVueModel(data.vm_xtools_lmx);
    }


    var show = function () {
        vmTools.showed = true;
    }

    var hide = function () {
        vmTools.showed = false;
    }


    var initVueModel = function (data)
    {
        vmTools = new Vue({
            el: '#CardXToolsLmx',
            data: {
                tools: data.ToolsInfo,
                showed: true
            },
            computed: {
                colorKPI: function ()
                {
                    if (this.tools == null || this.tools.length == 0)
                        return 'color-no-data';

                    var color = 'color-darkgreen';                   

                    return color;
                },               
            },            
        });
    }

    var update = function (data)
    {
        // update vue model
       
        var vm_tools = data.vm_xtools_lmx;
        if (vm_tools != null) {
            vmTools.tools = vm_tools.ToolsInfo;
        }
        
    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();