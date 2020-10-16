var ChartManager = function ()
{
    Highcharts.setOptions({
        chart: {
            animation: false,
            style: {
                fontFamily: 'Roboto Mono'
            },
            height: 250,
            backgroundColor: 'transparent'
        },
        title: {
            text: null
        },
        subtitle: {
            text: null
        },
        credits: {
            enabled: false
        },
        yAxis: {
            title: {
                style: {
                    fontSize: '10px',
                    fontWeight: '400'
                }
            },
            labels: {
                style: {
                    fontSize: '10px'
                }
            }
        },
        tooltip: {
            shared: true,
            crosshairs: true
        },
        legend: {
            verticalAlign: 'bottom',
            itemMarginTop: 0,
            itemMarginBottom: 0,
            itemMarginRight: 1,
            itemDistance: 10,
            align: 'center',
            itemStyle: {
                fontWeight: 'normal',
                fontSize: '10px',
                lineHeight: '10px'
            }
        },
        plotOptions: {
            column: {
                dataLabels: {
                    color: 'red'
                }
            },
            series: {
                marker: {
                    symbol: 'circle',
                    fillColor: '#1F282B',
                    fillColor: '#eee',
                    lineWidth: 2,
                    lineColor: null // inherit from series
                }
            }
        }
    });

    var lineChart = function (chartID, options)
    {
        var config = {
            chart: {
                type: 'line',
                marginLeft: 40,
                marginRight: 5,
                marginTop: 30,
                spacingTop: 0
            },
            xAxis: {
                //categories: ['05/09', '06/09', '07/09', '08/09', '09/09', '10/09', '11/09'],
                categories: options.categories,
                tickWidth: 0,
                gridLineWidth: 1,
                labels: {
                    y: 15
                }
            },
            yAxis: {
                min: 0,
                tickInterval: 10,
                tickWidth: 0,
                gridLineWidth: 1,
                title: {
                    //text: 'Durata (min)'
                    text: options.yTitle
                },
                labels: {
                    x: -5
                }
            },
            tooltip: {
                valueSuffix: options.valueSuffix
            },
            series: options.series,

        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);

        //setInterval(function () {
        //    if ($('#' + chartID).highcharts())
        //        $('#' + chartID).highcharts().setSize($($('#' + chartID).closest(".card-portlet")[0]).width() - 30, $($('#' + chartID).closest(".card-portlet")[0]).height() - 30);
        //}, 250);
    }

    var columnChart = function (chartID, options)
    {
        var config = {
            chart: {
                type: 'column',
                height: 250,
                //plotBorderWidth: 1,
                marginTop: 10,
                marginLeft: 40,
            },
            xAxis: {
                categories: options.categories,
                title: {
                    //text: 'Fasce di Velocità (K giri/min)',
                    text: options.xTitle,
                    style: {
                        fontSize: '10px',
                        fontWeight: '400'
                    }
                },
                label: {
                    style: {
                        fontSize: '9px'
                    }
                }
            },
            yAxis: {
                min: 0,
                title: {
                    //text: 'Durata (min)',
                    text: options.yTitle,
                },
                labels: {
                    x: -5
                },
                tickWidth: 0,
                maxPadding: 0.1,
            },
            legend: {
                enabled: false
            },
            tooltip: {
                valueSuffix: options.valueSuffix
            },
            plotOptions: {
                series: {
                    dataLabels: {
                        enabled: true,
                        inside: false,
                        color: '#fff',
                        color: '#676767',
                        style: {
                            fontSize: '10px',
                            textOutline: '0px contrast'
                        }
                    }
                }
            },
            series: options.series
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);
    }

    var stackedColumnChart = function (chartID, options)
    {
        var config = {
            chart: {
                type: 'column'
            },
            xAxis: {
                categories: options.categories
                //categories: ['05/09', '06/09', '07/09', '08/09', '09/09', '10/09', '11/09']
            },
            yAxis: {
                min: 0,
                title: {
                    text: options.yTitle
                    //text: 'Quantity (n)',
                }
            },
            plotOptions: {
                column: {
                    stacking: 'normal'
                },
                series: {
                    dataLabels: {
                        enabled: true,
                        inside: true,
                        color: '#fff',
                        style: {
                            color: '#fff',
                            fontSize: '10px',
                            fontWeight: 'bold',
                            textOutline: '0px contrast'
                        }
                    }
                }
            },
            series: options.series
            //series: [{
            //    name: 'Error',
            //    color: '#D32337',
            //    index: 4,
            //    legendIndex: 0,
            //    data: [5, 3, 4, 5, 7, 4, 9]
            //}, {
            //    name: 'Operator',
            //    color: '#ec0',
            //    index: 3,
            //    legendIndex: 1,
            //    data: [1, 2, 3, 4, 5, 7, 4]
            //}]
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);
    }

    var stackedBarChart = function (chartID, options)
    {
        var config = {
            chart: {
                type: 'bar'
            },
            xAxis: {
                categories: options.categories
                //categories: ['Operator 1', 'Operator 2', 'Operator 3']
            },
            yAxis: {
                min: 0,
                max: 100,
                title: {
                    text: options.yTitle,
                    //text: 'Total Time ON (%)',
                },
                labels: {
                    format: '{value}%'
                }
            },
            tooltip: {
                valueSuffix: '%'
            },
            plotOptions: {
                bar: {
                    stacking: 'normal',
                }
            },
            legend: {
                reversed: true
            },
            series: options.series
            //series: [{
            //    name: 'Produzione',
            //    color: '#A5CC48',
            //    data: [50, 60, 70]
            //}, {
            //    name: 'Pausa',
            //    color: '#FFE941',
            //    data: [20, 20, 10]
            //}, {
            //    name: 'Manuale',
            //    color: '#98C2ED',
            //    data: [15, 10, 10]
            //}, {
            //    name: 'Guasto',
            //    color: '#cc3344',
            //    data: [15, 10, 10]
            //}]
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);


        //setInterval(function () {
        //    $('#' + chartID).highcharts().setSize($($('#' + chartID).closest(".card-portlet")[0]).width() - 30, $($('#' + chartID).closest(".card-portlet")[0]).height() - 30);
        //}, 250);
            
        
    }

    var dualAxesColumnChart = function (chartID, options)
    {
        var config = {
            chart: {
                type: 'column',
                marginTop: 30,
                spacingTop: 0,
                marginLeft: 40,
                marginRight: 40
            },
            xAxis: [{
                categories: options.categories,
                //categories: ['05/09', '06/09', '07/09', '08/09', '09/09', '10/09', '11/09'],
                tickWidth: 0.5,
                gridLineWidth: 1,
                labels: {
                    y: 15,
                }
            }],
            yAxis: [{
                title: {
                    //text: 'Efficienza (%)',
                    text: options.yTitle,
                    style: {
                        color: '#8EBF36',
                    },
                    x: 5
                },
                labels: {
                    style: {
                        color: '#8EBF36'
                    },
                    x: -3,
                    y: 4
                },
                tooltipValueFormat: '{value}%',
                max: 100,
                min: 0,
                tickInterval: 20,
                tickAmount: 6
            }, {
                title: {
                    //text: 'Produttività (p/h)',
                    text: options.yTitle2,
                    style: {
                        color: '#a2f3f0',
                    },
                    x: -5
                },
                labels: {
                    style: {
                        color: '#a2f3f0',
                    },
                    x: 3,
                    y: 4
                },
                tooltipValueFormat: '{value}pz/h',
                opposite: true,
                //max: 100,
                tickInterval: 20,
                tickAmount: 6
            }],
            legend: {
                enabled: true
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2
                }
            },
            series: getProductivitySeries(options.series, 'column')
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);

        //setInterval(function () {
        //    $('#' + chartID).highcharts().setSize($($('#' + chartID).closest(".card-portlet")[0]).width() - 30, $($('#' + chartID).closest(".card-portlet")[0]).height() - 30);
        //}, 250);
    }

    var dualAxesBarChart = function (chartID, options)
    {
        var config = {
            chart: {
                type: 'bar',
                spacingTop: 0,
                //marginLeft: 90,
                //spacingLeft: 0,
                marginTop: 60
            },
            xAxis: [{
                categories: options.categories,
                //categories: ['Operator 1', 'Operator 2', 'Operator 3', 'Operator 4'],
                tickWidth: 0.5,
                gridLineWidth: 1,
                labels: {
                    x: -7
                },
                crosshair: true
            }],
            yAxis: [{
                title: {
                    //text: 'Efficienza (%)',
                    text: options.yTitle,
                    style: {
                        color: '#8EBF36',
                    }
                },
                labels: {
                    style: {
                        color: '#8EBF36',
                    },
                    x: 0,
                    y: 11
                },
                min: 0,
                max: 100,
                tickInterval: 20,
                tickAmount: 6
            }, {
                title: {
                    //text: 'Produttività (p/h)',
                    text: options.yTitle2,
                    style: {
                        color: '#588FA4',
                    },
                    y: 15
                },
                labels: {
                    style: {
                        color: '#588FA4',
                    },
                    x: 0,
                    y: -5
                },
                opposite: true,
                min: 0,
                //max: 100,
                tickInterval: 20,
                tickAmount: 6
            }],
            legend: {
                enabled: true
            },
            plotOptions: {
                bar: {
                    pointPadding: 0.2
                },
                series: {
                    states: {
                        hover: {
                            enabled: false,
                            lineWidth: 0
                        }
                    },
                    lineWidth: 0
                }
            },
            series: getProductivitySeries(options.series, 'bar')
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);

        //setInterval(function () {
        //    $('#' + chartID).highcharts().setSize($($('#' + chartID).closest(".card-portlet")[0]).width() - 30, $($('#' + chartID).closest(".card-portlet")[0]).height() - 30);
        //}, 250);
    }

    var stateMachinePieChart = function (chartID, options) {
        var config = {

            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie',
                height: 140,
                spacingBottom: 0,
                spacingTop: 0,
                spacingLeft: 0,
                spacingRight: 0,
                marginBottom: 0,
                marginTop: 0,
                marginLeft: 0,
                marginRight: 0

            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false,
                        format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                        style: {
                            color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                        }
                    }
                }
            },
            series: [{
                name: 'States',
                colorByPoint: true,
                data: options.series
            }],

            
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);


        var myElement = document;
        var mc = new Hammer.Manager(myElement);
        // create a pinch and rotate recognizer
        // these require 2 pointers
        var rotate = new Hammer.Rotate();
        // add to the Manager
        mc.add([rotate]);
        mc.on("rotate", function (ev) {
            var chart = $('#' + chartID).highcharts();
            chart.update(config);
        });

        setInterval(function () {
            var chart = $('#' + chartID).highcharts();
            var containerWidth = $('#' + chartID).parent().width();
            var containerHeight = $('#' + chartID).parent().height();
            if (containerHeight > 0 && containerWidth > 0) {
                chart.setSize(containerWidth + 10, Math.max(containerHeight - 15, 120));
            }
        }, 250);
    }

    var productivityMachineSolidGaugeChart = function (chartID, options, localizations) {
        var config = {
            yAxis: {
                min: 0,
                max: 100,
                stops: [
                    [0.01, '#D32337'], // red
                    [options.series[0].stateProductivityYellowThreshold / 100 - 0.01, '#D32337'], // red
                    [options.series[0].stateProductivityYellowThreshold / 100, '#F9ED4B'], //yellow
                    [options.series[0].stateProductivityGreenThreshold / 100 - 0.01, '#F9ED4B'], //yellow
                    [options.series[0].stateProductivityGreenThreshold / 100, '#8EBF36'], //green
                ],
                lineWidth: 0,
                labels: {
                    y: 16,
                    x: 0
                },
                lineWidth: 0,
                minorTickInterval: null,
                tickAmount: 2,
                title: {
                    style: {
                        fontSize: '1px',
                        fontWeight: '400'
                    }
                },
                labels: {
                    style: {
                        fontSize: '1px'
                    }
                }
            },
            chart: {
                height: 140,
                type: 'solidgauge'
            },
            //tooltip: {
            //    pointFormat: '{series.name}: <b>{point.y:.1f} %</b>'
            //},
            series: [{
                name: localizations.efficiency.trim(),
                data: _.map(options.series, function (opt) { return Math.round(opt.y); }),

                dataLabels: {
                    format: '<div style="text-align:center"><span style="font-size:8px;color:' +
                        ((Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black') + '">' + localizations.efficiency + '</span><br /><span style="font-size:8px;color:' +
                        ((Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black') + '">{y}</span>' +
                        '<span style="font-size:8px;">%</span></div>'
                },
                tooltip: {
                    valueSuffix: ' %'
                },
                enableMouseTracking: false
            }],
            pane: {
                center: ['50%', '100%'],
                size: '150%',
                startAngle: -90,
                endAngle: 90,
                background: {
                    backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || '#EEE',
                    innerRadius: '60%',
                    outerRadius: '100%',
                    shape: 'arc'
                }
            },
            plotOptions: {
                solidgauge: {
                    dataLabels: {
                        y: 0,
                        x: -10,
                        borderWidth: 0,
                        useHTML: true
                    }
                }
            }
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);

        var myElement = document.getElementById(chartID);
        var mc = new Hammer.Manager(myElement);
        var rotate = new Hammer.Rotate();
        mc.add([rotate]);
        mc.on("rotate", function (ev) {
            var chart = $('#' + chartID).highcharts();
            chart.update(config);
        });

        setInterval(function () {
            var chart = $('#' + chartID).highcharts();
            var containerWidth = $('#' + chartID).parent().width();
            var containerHeight = $('#' + chartID).parent().height();
            if (containerHeight > 0 && containerWidth > 0) {
                chart.setSize(containerWidth + 30, Math.max(containerHeight - 15, 120));
            }
        }, 250);
    }
    
    var getProductivitySeries = function (series, type)
    {
        var enSerieType = {
            Efficiency: 0,
            GrossTime: 1,
            NetTime: 2
        };

        $.each(series, function (i, serie)
        {
            switch (serie.type)
            {
                case enSerieType.Efficiency:
                    var options = {
                        type: 'line',
                        zIndex: 1,
                        tooltip: {
                            valueSuffix: ' %'
                        }
                    }
                    $.extend(true, serie, options);
                    break;

                case enSerieType.GrossTime:
                case enSerieType.NetTime:
                    var options = {
                        type: type,
                        yAxis: 1,
                        tooltip: {
                            valueSuffix: ' pz/h'
                        }
                    }
                    $.extend(true, serie, options);
                    break;
            }
        });

        return series;
    }

    var destroyChart = function(chartID)
    {
        var chart = $('#' + chartID).highcharts();

        if (chart != null)
            chart.destroy();
    }


    return {
        lineChart: lineChart,
        columnChart: columnChart,
        stackedColumnChart: stackedColumnChart,
        stackedBarChart: stackedBarChart,
        dualAxesColumnChart: dualAxesColumnChart,
        dualAxesBarChart: dualAxesBarChart,
        destroyChart: destroyChart,
        stateMachinePieChart: stateMachinePieChart,
        productivityMachineSolidGaugeChart: productivityMachineSolidGaugeChart
    }

}();