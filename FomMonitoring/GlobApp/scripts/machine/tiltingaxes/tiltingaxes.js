var TiltingAxes = function () {
    var vmTiltingAxes;

    var init = function (data) {
        initVueModel(data.vm_tilting_axes);
    }

    var show = function () {
        vmTiltingAxes.showed = true;
    }

    var hide = function () {
        vmTiltingAxes.showed = false;
    }

    var initVueModel = function (data) {
        vmTiltingAxes = new Vue({
            el: '#CardTiltingAxes',
            data: {
                values: data,
                showed: true
            },
            computed: {
                colorKPI: function () {
                    if (this == null)
                        return 'color-no-data';

                    var color = 'color-darkgreen';

                    return color;
                },
            },
            methods: {
                noData: function() {
                    if (this.values.NrotazioniAsse2A1 == null &&
                        this.values.NrotazioniAsse2A2 == null &&
                        this.values.NrotazioniAsse2A3 == null &&
                        this.values.NrotazioniAsse2A4 == null &&
                        this.values.NrotazioniAsse2A5 == null &&
                        this.values.NrotazioniAsse2A6 == null) {
                        return true;
                    } else {
                        return false;
                    }
                }
            }
        });
    }

    var update = function (data) {
        // update vue model

        var vm_tilting_axes = data.vm_tilting_axes;
        vmTiltingAxes.values = vm_tilting_axes;
        

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();