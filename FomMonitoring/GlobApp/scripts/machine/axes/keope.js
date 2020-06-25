var AxesKeope = function () {
    var vmAxes;

    var init = function (data) {
        initVueModel(data.vm_axes_keope);
    }
    var show = function () {
        if (vmAxes != null)
            vmAxes.showed = true;
    }

    var hide = function () {
        if (vmAxes != null)
            vmAxes.showed = false;
    }

    var initVueModel = function (data) {
        vmAxes = new Vue({
            el: '#CardAxesKeope',
            data: {
                axes: data.axes,
                showed: true
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
        update: update,
        show: show,
        hide: hide
    }

}();