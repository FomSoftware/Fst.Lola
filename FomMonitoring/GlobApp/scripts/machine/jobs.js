﻿var Jobs = function ()
{
    var vmJobs;

    var init = function (data)
    {
        //var data = {
        //    vm_jobs: {
        //        jobs: [{
        //            code: 'A00JO3',
        //            perc: 72,
        //            time: {
        //                days: null,
        //                hours: '01',
        //                minutes: '23',
        //                seconds: null,
        //                elapsed: 3000
        //            },
        //            quantity: 1,
        //            pieces: 30
        //        }, {
        //            code: 'A00JO5',
        //            perc: 100,
        //            time: {
        //                days: null,
        //                hours: '04',
        //                minutes: '50',
        //                seconds: null,
        //                elapsed: 4000
        //            },
        //            quantity: 10,
        //            pieces: 33
        //        }],
        //        sorting: {
        //            duration: 'asc',
        //            quantity: null,
        //            code: null,
        //            progress: null
        //        }
        //    }
        //};

        //if (data.vm_jobs != null)
            dataBindVueModel(data.vm_jobs);
    }


    var dataBindVueModel = function (data)
    {
        vmJobs = new Vue({
            el: '#CardJobs',
            data: {
                jobs: data.jobs,
                sorting: data.sorting
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
                        return 'active ' + this.sorting.code;
                },
                sortingProgress: function ()
                {
                    if (this.sorting.progress != null)
                        return 'active ' + this.sorting.progress;
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
                sortQuantity: function ()
                {
                    this.sorting.duration = null;
                    this.sorting.code = null;
                    this.sorting.progress = null;
                    this.sorting.day = null;

                    if (this.sorting.quantity == 'desc')
                    {
                        this.jobs = _.sortBy(this.jobs, 'quantity');
                        this.sorting.quantity = 'asc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }

                    if (this.sorting.quantity == 'asc' || this.sorting.quantity == null)
                    {
                        this.jobs = _.sortBy(this.jobs, 'quantity').reverse();
                        this.sorting.quantity = 'desc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }
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
                },
                sortProgress: function ()
                {
                    this.sorting.quantity = null;
                    this.sorting.duration = null;
                    this.sorting.code = null;
                    this.sorting.day = null;

                    if (this.sorting.progress == 'desc')
                    {
                        this.jobs = _.sortBy(this.jobs, 'perc');
                        this.sorting.progress = 'asc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }

                    if (this.sorting.progress == 'asc' || this.sorting.progress == null)
                    {
                        this.jobs = _.sortBy(this.jobs, 'perc').reverse();
                        this.sorting.progress = 'desc';

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
        vmJobs.jobs = vm_jobs.jobs;
        vmJobs.sorting = vm_jobs.sorting;
    }


    return {
        init: init,
        update: update
    }

}();