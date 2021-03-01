var AxesAxel = function () {
    var vmAxes;

    var init = function (data) {
        initVueModel(data.vm_axes_axel);
    }

    var show = function () {
        vmAxes.showed = true;
    }

    var hide = function () {
        vmAxes.showed = false;
    }

    var initVueModel = function (data) {
        vmAxes = new Vue({
            el: '#CardAxesAxel',
            data: {
                axes: data,
                showed: true
            },
            computed: {
                colorKPI: function () {
                    if (this.axes == null)
                        return 'color-no-data';

                    var color = 'color-darkgreen';

                    return color;
                }
            },
        });
    }

    var update = function (data) {
        // update vue model

        var vmAxel = data.vm_axes_axel;
        if (vmAxel != null) {
            vmAxes.axes = vmAxel;
        }

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();