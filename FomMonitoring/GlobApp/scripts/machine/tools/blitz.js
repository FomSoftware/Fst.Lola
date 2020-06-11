var ToolsBlitz = function ()
{
    var vmTools;

    var init = function (data)
    {
        initVueModel(data.vm_tools_blitz);
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
            el: '#CardTools',
            data: {
                toolsTm: data.toolsTm,
                toolsTf: data.toolsTf,
                showed: true
            },
            computed: {
                colorKPI: function ()
                {
                    if (this.toolsTm == null && this.toolsTf == null || (this.toolsTm.length == 0 && this.toolsTf.length == 0))
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
       
        var vm_tools = data.vm_tools_blitz;
        if (vm_tools != null) {
            vmTools.toolsTm = vm_tools.toolsTm;
            vmTools.toolsTf = vm_tools.toolsTf;
        }
        
    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();