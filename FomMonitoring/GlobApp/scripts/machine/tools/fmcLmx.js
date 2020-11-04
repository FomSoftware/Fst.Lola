var ToolsFmcLmx = function ()
{
    var vmTools;

    var init = function (data)
    {
        initVueModel(data.vm_tools_fmc_lmx);
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
            el: '#CardToolsFmcLmx',
            data: {
                tools: data != null ? data.ToolsInfo : null,
                panel: data != null ? data.PanelId : null,
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
       
        var vm_tools = data.vm_tools_fmc_lmx;
        if (vm_tools != null) {
            vmTools.tools = vm_tools.ToolsInfo;
            vmTools.panel = vm_tools.PanelId;
        }
        
    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();