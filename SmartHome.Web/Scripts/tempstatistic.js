$(function () {
    $.ajax({
        dataType: "json",
        type: "POST",
        url: "/TempStatistics/GetTempStatistics",
        data: $("#temp-range-form").serialize(),
    })
    .done(function (data) {
        tempPlotRender(data, "#temp-plot")
    });

    $("#FromDate").datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        changeMonth: true,
        beforeShow: function () {
            var maxDate = $("#ToDate").datepicker("getDate");
            $("#FromDate").datepicker("option", "maxDate", maxDate);
        },
        onClose: function (selectedDate) {
            var maxDate = $("#ToDate").datepicker("getDate");
            var currentDate = $("#FromDate").datepicker("getDate");
            if (currentDate > maxDate) {
                $("#FromDate").datepicker("option", "maxDate", maxDate);
            }
            else {
                $("#ToDate").datepicker("option", "minDate", selectedDate);
            }
            setTimeout(function () {
                $("#ToDate").focus();
            }, 200);
        }
    });
    $("#ToDate").datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        changeMonth: true,
        maxDate: 0,
        onClose: function (selectedDate) {
            $("#ToDate").datepicker("option", "maxDate", 0);
            $("#FromDate").datepicker("option", "maxDate", selectedDate);

        }
    });
});

function updateTempPlot(data, selector) {
    if (data.message != null) {
        $(selector).highcharts().series[0].hide();
        $("#dialog-modal-message").html(data.message);
        openDialogModalMessage();
    }
    else {
        $(selector).highcharts().series[0].setData(data);
        $(selector).highcharts().series[0].show();
    }
}

function tempPlotRender(data, selector) {

    $(selector).highcharts('StockChart',
         {
             rangeSelector: {
                 buttons: [
                  {
                      type: 'day',
                      count: 1,
                      text: '1d'
                  }, {
                      type: 'day',
                      count: 3,
                      text: '3d'
                  }, {
                      type: 'week',
                      count: 1,
                      text: '1w'
                  }, {
                      type: 'month',
                      count: 1,
                      text: '1m'
                  }, {
                      type: 'month',
                      count: 6,
                      text: '6m'
                  }, {
                      type: 'year',
                      count: 1,
                      text: '1y'
                  }, {
                      type: 'all',
                      text: 'All'
                  }
                 ],
                 selected: 3,
                 inputEnabled: false,
             },

             title: {
                 text: ''
             },
             xAxis: {
                 type: 'datetime',
                 labels: {
                     format: '{value:%d:%m:%Y}',
                 }
             },
             series: [
                 {
                     name: 'Temperature',
                     data: data,
                     type: 'areaspline',
                     threshold: null,
                     tooltip: {
                         valueDecimals: 1,
                         valueSuffix: '°C',
                         dateTimeLabelFormats: null,
                     },
                     fillColor: {
                         linearGradient: {
                             x1: 0,
                             y1: 0,
                             x2: 0,
                             y2: 1
                         },
                         stops: [[0, Highcharts.getOptions().colors[0]], [1, 'rgba(0,0,0,0)']]
                     }
                 }
             ]
         });
}