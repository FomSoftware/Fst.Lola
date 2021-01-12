function ImageManAxel() {
    ImageManager.call(this);
};

ImageManAxel.prototype = Object.create(ImageManager.prototype);
ImageManAxel.prototype.constructor = ImageManAxel;
ImageManAxel.prototype.checkModello = function (machineGroupSelected) {

    if (machineGroupSelected == 'AXEL_axes' ||
        machineGroupSelected == 'AXEL_axes' ||
        machineGroupSelected == 'AXEL_axes') {
            OtherData.show();
        } else {
            OtherData.hide();
        }

    if (machineGroupSelected == 'AXEL_spindles' ||
        machineGroupSelected == 'AXEL_spindles' ||
        machineGroupSelected == 'AXEL_spindles') {
            ElectroSpindle.show();
        } else {
            ElectroSpindle.hide();
        }

    if (machineGroupSelected == 'AXEL_tools' ||
        machineGroupSelected == 'AXEL_tools' ||
        machineGroupSelected == 'AXEL_tools') {
            ToolsFmcLmx.show();
        } else {
            ToolsFmcLmx.hide();
        }

};








