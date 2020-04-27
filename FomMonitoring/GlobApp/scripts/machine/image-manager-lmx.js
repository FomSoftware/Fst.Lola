function ImageManLmx() {
    ImageManager.call(this);
};

ImageManLmx.prototype = Object.create(ImageManager.prototype);
ImageManLmx.prototype.constructor = ImageManLmx;
ImageManLmx.prototype.checkModello = function (machineGroupSelected) {

    if (machineGroupSelected == 'LMX650_Motor_Blade') {
        MotorBladeLMX.show();
    } else {
        MotorBladeLMX.hide();
    }
    if (machineGroupSelected == 'LMX650_XMU') {
        ElectroSpindle.show();
        SensorSpindles.show();
        RotaryAxes.show();
        XToolsLmx.show();
    } else {
        ElectroSpindle.hide();
        SensorSpindles.hide();
        RotaryAxes.hide();
        XToolsLmx.hide();
    }
    if (machineGroupSelected == 'LMX650_StepIn_Out') {
        AxesLmx650.show();
        OtherDataLMX.show();

    } else {
        AxesLmx650.hide();
        OtherDataLMX.hide();
    }
    if (machineGroupSelected == 'LMX650_MM') {
        MultiSpindles.show();
        TiltingAxes.show();
        ToolsFmcLmx.show();
    } else {
        MultiSpindles.hide();
        TiltingAxes.hide();
        ToolsFmcLmx.hide();
    }

};








