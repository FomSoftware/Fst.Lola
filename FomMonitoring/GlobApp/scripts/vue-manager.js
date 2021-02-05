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

    var initTimeSpanManutenzioneComponent = function () {
        Vue.component('time-span-manu', {
            props: ['duration'],
            template: '#time-span-manu',
            computed: {
                showSeconds: function () {
                    if ((this.duration.days != null && this.duration.hours != null) ||
                        (this.duration.hours != null && this.duration.minutes != null))
                        return false;
                    else
                        return true;
                },
                showMinutes: function () {
                    if (this.duration.days != null && this.duration.hours != null)
                        return false;
                    else
                        return true;
                }
            }
        });
    }

    var initTimeSpanEfficiencyComponent = function () {
        Vue.component('time-span-efficiency', {
            props: ['duration'],
            template: '#time-span-efficiency',
            computed: {
                showSeconds: function () {
                    if ((this.duration.days != null && this.duration.hours != null) ||
                        (this.duration.hours != null && this.duration.minutes != null))
                        return false;
                    else
                        return true;
                },
                showMinutes: function () {
                    if (this.duration.days != null && this.duration.hours != null)
                        return false;
                    else
                        return true;
                }
            }
        });
    }

    return {
        initTimeSpanComponent: initTimeSpanComponent,
        initTimeSpanEfficiencyComponent: initTimeSpanEfficiencyComponent,
        initTimeSpanManutenzioneComponent: initTimeSpanManutenzioneComponent
}
}();