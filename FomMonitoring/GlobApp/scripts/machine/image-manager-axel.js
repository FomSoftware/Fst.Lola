function ImageManAxel() {
    ImageManager.call(this);
};

ImageManAxel.prototype = Object.create(ImageManager.prototype);
ImageManAxel.prototype.constructor = ImageManAxel;
ImageManAxel.prototype.checkModello = function (machineGroupSelected) {

    if (machineGroupSelected == 'AXEL_axes' ||
        machineGroupSelected == 'AXEL_axes' ||
        machineGroupSelected == 'AXEL_axes') {
        OtherDataAxel.show();
        AxesAxel.show();
    } else {
        OtherDataAxel.hide();
        AxesAxel.hide();
    }

    if (machineGroupSelected == 'AXEL_spindles' ||
        machineGroupSelected == 'AXEL_spindles' ||
        machineGroupSelected == 'AXEL_spindles') {
        ElectroSpindleAxel.show();
        SensorsSpindleAxel.show();
    } else {
        ElectroSpindleAxel.hide();
        SensorsSpindleAxel.hide();
    }

    if (machineGroupSelected == 'AXEL_tools' ||
        machineGroupSelected == 'AXEL_tools' ||
        machineGroupSelected == 'AXEL_tools') {
        ToolsFmcLmx.show();
        ToolsWarehouseAxel.show();
    } else {
        ToolsFmcLmx.hide();
        ToolsWarehouseAxel.hide();
        }

};








