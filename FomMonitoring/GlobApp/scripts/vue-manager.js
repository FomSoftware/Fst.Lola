var VueManager = function()
{
    var initTimeSpanComponent = function()
    {
        Vue.component('time-span', {
            props: ['duration'],
            template: '#time-span',
            computed: {
                showSeconds: function()
                {
                    if ((this.duration.days != null && this.duration.hours != null) || 
                        (this.duration.hours != null && this.duration.minutes != null))
                        return false;
                    else
                        return true;
                },
                showMinutes: function ()
                {
                    if (this.duration.days != null && this.duration.hours != null)
                        return false;
                    else
                        return true;
                }
            }
        });
    }

    return {
        initTimeSpanComponent: initTimeSpanComponent
    }
}();