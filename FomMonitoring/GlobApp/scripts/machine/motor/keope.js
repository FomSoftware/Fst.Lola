var MotorKeope = function ()
{
    var vmMotor;

    var init = function (data)
    {
        initVueModel(data.vm_motor_keope);
    }
    var show = function () {
        if(vmMotor != null)
            vmMotor.showed = true;
    }

    var hide = function () {
        if (vmMotor != null)
            vmMotor.showed = false;
    }

    var initVueModel = function (data)
    {
        vmMotor = new Vue({
            el: '#CardMotorKeope',
            data: {
                fixedHead: data.fixedHead,
                mobileHead: data.mobileHead,
                showed: true
            },
            computed: {
                colorKPI: function () {
                    if ((this.fixedHead == null && this.mobileHead == null) || (this.fixedHead.length == 0 && this.mobileHead.length == 0))
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
       
        var vm_motor = data.vm_motor_keope;
        if (vm_motor != null) {
            vmMotor.fixedHead = vm_motor.fixedHead;
            vmMotor.mobileHead = vm_motor.mobileHead;
        }
        
    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();