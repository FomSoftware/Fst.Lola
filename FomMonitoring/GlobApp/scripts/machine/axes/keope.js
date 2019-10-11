var AxesKeope = function () {
    var vmAxes;

    var init = function (data) {
        initVueModel(data.vm_axes_keope);
    }

    var initVueModel = function (data) {
        vmAxes = new Vue({
            el: '#CardAxesKeope',
            data: {
                axes: data.axes,
            },
            computed: {
                colorKPI: function () {
                    if ( this.axes == null || this.axes.length == 0)
                        return 'color-no-data';

                    var color = 'color-darkgreen';

                    return color;
                },
            },
        });
    }

    var update = function (data) {
        // update vue model

        var vm_axes = data.vm_axes_keope;
        if (vm_axes != null) {
            vmAxes.axes = vm_axes.axes;
        }

    }

    return {
        init: init,
        update: update
    }

}();