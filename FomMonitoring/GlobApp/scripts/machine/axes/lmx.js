var AxesLmx650 = function () {
    var vmAxes;

    var init = function (data) {
        initVueModel(data.vm_axes_lmx);
    }

    var initVueModel = function (data) {
        vmAxes = new Vue({
            el: '#CardAxesLmx',
            data: {
                axes: data.AxesLmx
            },
            computed: {
                colorKPI: function () {
                    if (this.axes == null || this.axes.length == 0)
                        return 'color-no-data';

                    var color = 'color-darkgreen';

                    return color;
                },
                firstRow: function() {
                    return this.axes.slice(0, 3);
                },
                secondRow: function () {
                    return this.axes.slice(3, 6);
                },
                thirtRow: function () {
                    return this.axes.slice(6, 9);
                },
                fourthRow: function () {
                    return this.axes.slice(9, 12);
                },
                fifthRow: function () {
                    return this.axes.slice(12, 14);
                }
            },
        });
    }

    var update = function (data) {
        // update vue model

        var vmLmx = data.vm_axes_lmx;
        if (vmLmx != null) {
            vmAxes.AxesLmx = vmLmx.AxesLmx;
        }

    }

    return {
        init: init,
        update: update
    }

}();