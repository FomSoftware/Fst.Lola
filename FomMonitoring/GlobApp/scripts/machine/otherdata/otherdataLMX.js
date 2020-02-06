var OtherDataLMX = function () {
    var vmOther;

    var init = function (data) {
        initVueModel(data.vm_other_data_lmx);
    }

    var show = function () {
        vmOther.showed = true;
    }

    var hide = function () {
        vmOther.showed = false;
    }

    var initVueModel = function (data) {
        vmOther = new Vue({
            el: '#CardOtherDataLMXMachine',
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
                noData: function () {
                    if (this.values.OreVitaMacchina == null ||
                        this.values.OreUltimoIngr == null ||
                        this.values.NumBarreCaricate == null ||
                        this.values.EtiMancanti == null ||
                        this.values.EtiPerse == null ||
                        this.values.EtiStampate == null ) {
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

        var vm_other = data.vm_other_data_lmx;
        if (vm_other != null) {
            vmOther.values = vm_other;
        }

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();