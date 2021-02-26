var ToolsWarehouseAxel = function () {
    var toolsWarehouse;

    var init = function (data) {
        initVueModel(data.vm_toolsW_axel);
    }


    var show = function () {
        toolsWarehouse.showed = true;
    }

    var hide = function () {
        toolsWarehouse.showed = false;
    }


    var initVueModel = function (data) {
        toolsWarehouse = new Vue({
            el: '#CardToolsWarehouse',
            data: {
                tools: data != null ? data.toolsWharehouse : null,
                showed: true
            },
            computed: {
                colorKPI: function () {
                    if (this.tools == null || this.tools.length == 0)
                        return 'color-no-data';

                    var color = 'color-darkgreen';

                    return color;
                },
            },
        });
    }

    var update = function (data) {
        // update vue model

        var vm_tools = data.vm_toolsW_axel;
        if (vm_tools != null) {
            toolsWarehouse.tools = vm_tools.toolsWharehouse;
        }

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();