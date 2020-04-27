function ImageManFmc() {
    ImageManager.call(this);
};

ImageManFmc.prototype = Object.create(ImageManager.prototype);
ImageManFmc.prototype.constructor = ImageManFmc;
ImageManFmc.prototype.checkModello = function (machineGroupSelected) {

        if (machineGroupSelected == 'FMC3-4_axes') {
            OtherData.show();
        } else {
            OtherData.hide();
        }

        if (machineGroupSelected == 'FMC3-4_spindles') {
            ElectroSpindle.show();
        } else {
            ElectroSpindle.hide();
        }

        if (machineGroupSelected == 'FMC3-4_tools') {
            ToolsFmcLmx.show();
        } else {
            ToolsFmcLmx.hide();
        }

};








