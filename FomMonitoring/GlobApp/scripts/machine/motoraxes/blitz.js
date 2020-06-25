var MotorAxesBlitz = function ()
{
    var vmMotorAxes;

    var init = function (data)
    {
        initVueModel(data.vm_motoraxes_blitz);
    }

    var show = function () {
        if (vmMotorAxes != null)
            vmMotorAxes.showed = true;
    }

    var hide = function () {
        if (vmMotorAxes != null)
            vmMotorAxes.showed = false;
    }

    var initVueModel = function (data)
    {
        vmMotorAxes = new Vue({
            el: '#CardSpindles',
            data: {
                motors: data ? data.motors : null,
                axes: data ? data.axes : null,
                showed: true
            },
            computed: {
                colorKPI: function ()
                {
                    if (this.motors == null && this.axes == null || (this.motors.length == 0 && this.axes.length == 0))
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
       
        var vm_motoraxes = data.vm_motoraxes_blitz;
        if (vm_motoraxes != null) {
            vmMotorAxes.motors = vm_motoraxes.motors;
            vmMotorAxes.axes = vm_motoraxes.axes;
        }
        
    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();