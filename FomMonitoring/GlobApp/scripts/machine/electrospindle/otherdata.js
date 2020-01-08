var OtherData = function () {
    var vmOther;

    var init = function (data) {
        initVueModel(data.vm_other_data);
    }

    var initVueModel = function (data) {
        vmOther = new Vue({
            el: '#CardOtherDataMachine',
            data: {
                values: data
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
                    if (this.values.CycleCountXFlow == null ||
                        this.values.OreVitaMacchina == null ||
                        this.values.OreParzialiDaIngrassaggio == null ||
                        this.values.AsseXKm == null ||
                        this.values.AsseYKm == null ||
                        this.values.AsseZKm == null ||
                        this.values.CountRotationA == null) {
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

        var vm_other = data.vm_other_data;
        if (vm_other != null) {
            vmOther.values = vm_other;
        }

    }

    return {
        init: init,
        update: update
    }

}();