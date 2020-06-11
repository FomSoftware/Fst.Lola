function ImageManBlitz() {
    ImageManager.call(this);
};

ImageManBlitz.prototype = Object.create(ImageManager.prototype);
ImageManBlitz.prototype.constructor = ImageManFmc;
ImageManBlitz.prototype.checkModello = function (machineGroupSelected) {

    if (machineGroupSelected == 'SawM_Axes') {
        MotorAxesBlitz.show();
        } else {
            MotorAxesBlitz.hide();
        }

    if (machineGroupSelected == 'SawM_DoubleHeads') {
        ToolsBlitz.show();
        } else {
        ToolsBlitz.hide();
        }

};








