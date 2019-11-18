var ToolsFmcLmx = function ()
{
    var vmTools;

    var init = function (data)
    {
        initVueModel(data.vm_tools_fmc_lmx);
    }

    var initVueModel = function (data)
    {
        vmTools = new Vue({
            el: '#CardToolsFmcLmx',
            data: {
                tools: data.ToolsInfo
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
        }
        
    }

    return {
        init: init,
        update: update
    }

}();