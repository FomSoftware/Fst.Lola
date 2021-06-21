function ImageManLmc() {
    ImageManager.call(this);
};

ImageManLmc.prototype = Object.create(ImageManager.prototype);
ImageManLmc.prototype.constructor = ImageManLmc;
ImageManLmc.prototype.checkModello = function (machineGroupSelected) {

    if (machineGroupSelected == 'LMC650_Motor_Blade') {
        //MotorBladeLMX.show();
    } else {
        //MotorBladeLMX.hide();
    }
    if (machineGroupSelected == 'LMC650_StepIn_Out') {
       // AxesLmx650.show();
       // OtherDataLMX.show();
    } else {
       // AxesLmx650.hide();
       // OtherDataLMX.hide();
    }
    if (machineGroupSelected == 'LMC650_MM') {
       // MultiSpindles.show();
       // TiltingAxes.show();
       // ToolsFmcLmx.show();
    } else {
      //  MultiSpindles.hide();
      //  TiltingAxes.hide();
      //  ToolsFmcLmx.hide();
    }

};








