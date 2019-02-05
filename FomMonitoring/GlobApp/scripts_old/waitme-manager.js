var WaitmeManager = function()
{
    var start = function (renderID)
    {
        $(renderID).waitMe({
            effect: 'timer',
            text: 'Please wait...',
            bg: 'rgba(42, 55, 59, 0.7)',
            color: '#fff',
            maxSize: '',
            textPos: 'vertical',
            fontSize: '',
            source: ''
        });
    }

    var end = function (renderID) {
        $(renderID).waitMe('hide');
    }

    return {
        start: start,
        end: end
    }

}();

