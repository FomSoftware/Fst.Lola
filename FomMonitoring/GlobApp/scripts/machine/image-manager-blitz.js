function ImageManBlitz() {
    ImageManager.call(this);
};

ImageManBlitz.prototype = Object.create(ImageManager.prototype);
ImageManBlitz.prototype.constructor = ImageManFmc;
ImageManBlitz.prototype.checkModello = function (machineGroupSelected) {

    if (machineGroupSelected == 'SawM_Axes') {
        if (MotorAxesBlitz != null)
            MotorAxesBlitz.show();
        if (AxesKeope != null)
            AxesKeope.show();
    } else {
        if (AxesKeope != null)
            AxesKeope.hide();
        if (MotorAxesBlitz != null)
            MotorAxesBlitz.hide();
        }

    if (machineGroupSelected == 'SawM_DoubleHeads') {
        if (MotorKeope != null)
            MotorKeope.show();
        ToolsBlitz.show();
    } else {
        if (MotorKeope != null)
            MotorKeope.hide();
        ToolsBlitz.hide();
    }

};








