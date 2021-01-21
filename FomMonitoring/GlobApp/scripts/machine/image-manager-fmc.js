function ImageManFmc() {
    ImageManager.call(this);
};

ImageManFmc.prototype = Object.create(ImageManager.prototype);
ImageManFmc.prototype.constructor = ImageManFmc;
ImageManFmc.prototype.checkModello = function (machineGroupSelected) {

    if (machineGroupSelected == 'FMC3-4_axes' ||
        machineGroupSelected == 'FMC2_axes' ||
        machineGroupSelected == 'FMC1_axes' ||
        machineGroupSelected == 'FMC650_axes') {
            OtherData.show();
        } else {
            OtherData.hide();
        }

    if (machineGroupSelected == 'FMC3-4_spindles' ||
        machineGroupSelected == 'FMC2_spindles' ||
        machineGroupSelected == 'FMC1_spindles' ||
        machineGroupSelected == 'FMC650_spindle') {
            ElectroSpindle.show();
        } else {
            ElectroSpindle.hide();
        }

    if (machineGroupSelected == 'FMC3-4_tools' ||
        machineGroupSelected == 'FMC2_tools' ||
        machineGroupSelected == 'FMC1_tools' ||
        machineGroupSelected == 'FMC650_tools') {
            ToolsFmcLmx.show();
        } else {
            ToolsFmcLmx.hide();
    }
    if (machineGroupSelected == 'FMC650_Motor_Blade') {
        //ToolsFmcLmx.show();
    } else {
        //ToolsFmcLmx.hide();
    }

};








