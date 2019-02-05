var ChartManager = function ()
{
    Highcharts.setOptions({
        chart: {
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
                    fontSize: '9px'
                }
            }
        },
        tooltip: {
            shared: true,
            crosshairs: true
        },
        legend: {
            verticalAlign: 'top',
            itemMarginTop: 0,
            itemMarginBottom: 0,
            itemMarginRight: 0,
            itemDistance: 8,
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
            series: options.series
            //series: [{
            //    name: 'Produzione',
            //    color: '#A5CC48',
            //    data: [35, 33, 30, 25, 20, 21, 24],
            //}, {
            //    name: "Pausa",
            //    color: '#FFE941',
            //    lineWidth: 1.5,
            //    dashStyle: 'ShortDot',
            //    marker: {
            //        radius: 3
            //    },
            //    data: [22, 50, 10, 13, 15, 21, 20]
            //}, {
            //    name: 'Manuale',
            //    color: '#98C2ED',
            //    lineWidth: 1.5,
            //    dashStyle: 'ShortDot',
            //    marker: {
            //        radius: 3
            //    },
            //    data: [6, 5, 10, 9, 11, 10, 16]
            //}, {
            //    name: 'Guasto',
            //    color: '#ee0000',
            //    lineWidth: 1.5,
            //    dashStyle: 'ShortDot',
            //    marker: {
            //        radius: 3
            //    },
            //    data: [2, 1, 5, 3, 2, 5, 12]
            //}]
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);
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
            //    color: '#cc3333',
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
                        color: '#c0f73c',
                    },
                    x: 5
                },
                labels: {
                    style: {
                        color: '#c0f73c'
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
            //series: [{
            //    type: 'line',
            //    name: 'Efficienza',
            //    color: '#A5CC48',
            //    data: [70, 69, 95, 88, 84, 49, 89],
            //    zIndex: 3,
            //    tooltip: {
            //        valueSuffix: ' %'
            //    }
            //}, {
            //    type: null,
            //    name: 'Tempo Lordo',
            //    color: '#588FA4',
            //    //color: '#FEC8E0',
            //    yAxis: 1,
            //    data: [49.9, 71.5, 93, 91.2, 44.0, 76.0, 35.6],
            //    tooltip: {
            //        valueSuffix: ' p/h'
            //    }
            //},
            //{
            //    type: null,
            //    name: 'Tempo Netto',
            //    color: '#B1D3EA',
            //    color: '#AFEBDE',
            //    color: '#AEDFDE',
            //    yAxis: 1,
            //    data: [49.9, 71.5, 91, 91.2, 44.0, 76.0, 35.6],
            //    tooltip: {
            //        valueSuffix: ' p/h',
            //        useHTML: true
            //    }
            //}]
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);
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
                        color: '#c0f73c',
                    }
                },
                labels: {
                    style: {
                        color: '#c0f73c',
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
                        color: '#a2f3f0',
                    },
                    y: 15
                },
                labels: {
                    style: {
                        color: '#a2f3f0',
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
                }
            },
            series: getProductivitySeries(options.series, 'bar')
            //series: [{
            //    type: 'line',
            //    name: 'Efficienza',
            //    color: '#A5CC48',
            //    data: [70, 70, 70, 70],
            //    zIndex: 3
            //}, {
            //    name: 'Tempo Lordo',
            //    color: '#588FA4',
            //    yAxis: 1,
            //    data: [49.9, 71.5, 96.4, 91.2]
            //}, {
            //    name: 'Tempo Netto',
            //    color: '#AEDFDE',
            //    data: [7.0, 6.9, 9.5, 14.5]
            //}]
        }

        var chart = $('#' + chartID).highcharts();

        if (chart == null)
            Highcharts.chart(chartID, config);
        else
            chart.update(config);
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
        destroyChart: destroyChart
    }

}();