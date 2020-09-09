var OtherMachinesJobs = function ()
{
    var vmJobs;

    var init = function (data)
    {
        dataBindVueModel(data.vm_jobs);
    }

    var show = function () {
        if (vmJobs)
            vmJobs.showed = true;
    }

    var hide = function () {
        if (vmJobs)
            vmJobs.showed = false;
    }

    var dataBindVueModel = function (data)
    {
        vmJobs = new Vue({
            el: '#CardOtherMachinesJobs',
            data: {
                jobs: data.jobs,
                sorting: data.sorting,
                showed: true
            },
            computed: {
                colorOrders: function ()
                {
                    if (this.jobs == null)
                        return 'color-no-data';

                    var color = 'color-progress';

                    var inProgress = _.filter(this.jobs, function (job)
                    {
                        return job.perc < 100;
                    });

                    if (inProgress.length == 0)
                        color = 'color-green';

                    return color;
                },
                sortingCode: function ()
                {
                    if (this.sorting.code != null)
                        return this.sorting.code;
                },
                sortingProgress: function ()
                {
                    if (this.sorting.progress != null)
                        return this.sorting.progress;
                },
                sortingDay: function () {
                    if (this.sorting.day != null)
                        return this.sorting.day;
                },
                sortingQuantity: function () {
                    if (this.sorting.quantity != null)
                        return this.sorting.quantity;
                }
            },
            methods: {
                bgClass: function (perc)
                {
                    return {
                        'bg-progress': perc < 100,
                        'bg-green': perc == 100,
                    }
                },
                iconClass: function (perc)
                {
                    return {
                        'fa-cog color-progress': perc < 100,
                        'fa-check-circle color-green': perc == 100
                    }
                },
                refreshProgress: function ()
                {
                    $('.progress-job .progress-bar').css("width",
                          function () { return $(this).attr("aria-valuenow") + "%"; });
                },
                sortDuration: function ()
                {
                    this.sorting.quantity = null;
                    this.sorting.code = null;
                    this.sorting.progress = null;
                    this.sorting.day = null;

                    if (this.sorting.duration == 'desc')
                    {
                        this.jobs = _.sortBy(this.jobs, function (job) { return job.time.elapsed; });
                        this.sorting.duration = 'asc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }

                    if (this.sorting.duration == 'asc' || this.sorting.duration == null)
                    {
                        this.jobs = _.sortBy(this.jobs, function (job) { return job.time.elapsed; }).reverse();
                        this.sorting.duration = 'desc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }
                },
                sortDay: function () {
                    this.sorting.duration = null;
                    this.sorting.quantity = null;
                    this.sorting.code = null;
                    this.sorting.progress = null;

                    if (this.sorting.day == 'desc') {
                        this.jobs = _.sortBy(this.jobs, function (job) { return job.day; });
                        this.sorting.day = 'asc';

                        this.$nextTick(function () {
                            this.refreshProgress();
                        });

                        return;
                    }

                    if (this.sorting.day == 'asc' || this.sorting.day == null) {
                        this.jobs = _.sortBy(this.jobs, function (job) { return job.day; }).reverse();
                        this.sorting.day = 'desc';

                        this.$nextTick(function () {
                            this.refreshProgress();
                        });

                        return;
                    }
                },
                sortCode: function ()
                {
                    this.sorting.quantity = null;
                    this.sorting.duration = null;
                    this.sorting.progress = null;
                    this.sorting.day = null;

                    if (this.sorting.code == 'desc')
                    {
                        this.jobs = _.sortBy(this.jobs, 'code');
                        this.sorting.code = 'asc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }

                    if (this.sorting.code == 'asc' || this.sorting.code == null)
                    {
                        this.jobs = _.sortBy(this.jobs, 'code').reverse();
                        this.sorting.code = 'desc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }
                }
            }
        });
    }


    var update = function (data)
    {
        // update vue model
        var vm_jobs = data.vm_jobs;
        if (vmJobs) {
            vmJobs.jobs = vm_jobs.jobs;
            vmJobs.sorting = vm_jobs.sorting;
        }
    }


    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();