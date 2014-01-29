function openDialogModalTemp() {
    $("#dialog-modal-temp").dialog("open");
};

function closeDialogModalMessage(data) {
    debugger;
    var dialog = $("#dialog-modal-message");
    dialog.html(data);
    setTimeout(function () {
        dialog.dialog("close");
    }, 2000);
}

function openDialogModalMessage() {
    var dialog = $("#dialog-modal-message");
    dialog.dialog("open");
    dialog.html("Plese wait...");
}

function alertTempInit() {
    $.each($(".alarm-content"), function () {
        var sliderContent = $(this).find(".temp-range-slider-content");
        var slider = sliderContent.children(".slider-temp-range").first();
        var alarmCheckBox = $(this).find(".alarm-checkbox");
        var hiddenMinTempAlert = sliderContent.children(".min-temp-alert-value").first();
        var hiddenMaxTempAlert = sliderContent.children(".max-temp-alert-value").first();
        var rangeTempInput = sliderContent.children(".range-temp-input").first();
        slider.slider({
            orientation: "hotizontal",
            min: -20,
            max: 60,
            range: true,
            step: 0.5,
            slide: function (event, ui) {
                rangeTempInput.val(ui.values[0] + " - " + ui.values[1]);
                hiddenMinTempAlert.val(ui.values[0]);
                hiddenMaxTempAlert.val(ui.values[1]);
            }
        });
        var minTempAlert = hiddenMinTempAlert.val();
        var maxTempAlert = hiddenMaxTempAlert.val();
        if (minTempAlert === "")
            minTempAlert = 2;
        if (maxTempAlert === "")
            maxTempAlert = 30;
        slider.slider("values", [minTempAlert, maxTempAlert]);
        rangeTempInput.val(minTempAlert + " - " + maxTempAlert);

        alarmCheckBox.on('change', function () {
            // From the other examples
            if (this.checked == true) {
                slider.slider("enable");
            }
            else {
                slider.slider("disable");
            }
        });

        if (alarmCheckBox.is(':checked')) {
            slider.slider("enable");
        }
        else {
            slider.slider("disable");
        }

    })
}

function thermostatInit() {

    $.each($(".thermostat-content"), function () {
        var sliderContent = $(this).find(".target-temp-slider-content");
        var slider = sliderContent.children(".slider-target-temp").first();
        var thermostatStateCheckBox = $(this).find(".thermostat-state-checkbox");
        var thermostatBehavior = $(this).find(".thermostat-behavior");
        var targetTempInput = sliderContent.children(".target-temp-input").first();

        sliderContent.children(".slider-target-temp").first().slider({
            orientation: "hotizontal",
            min: -20,
            max: 60,
            step: 0.5,
            slide: function (event, ui) {
                targetTempInput.val(ui.value);
            }
        });

        var targetTemp = targetTempInput.val();
        slider.slider("value", targetTemp);

        thermostatStateCheckBox.on('change', function () {
            if (this.checked == true) {
                thermostatBehavior.removeClass("deactivate");
                slider.slider("enable");
                targetTempInput.removeAttr("readonly");
            }
            else {
                thermostatBehavior.addClass("deactivate");
                slider.slider("disable");
                targetTempInput.attr("readonly", "readonly");
            }
        });

        targetTempInput.change(function () {
            $(this).siblings(".slider-target-temp").slider("value", $(this).val());
        });


        if (thermostatStateCheckBox.is(':checked')) {
            thermostatBehavior.removeClass("deactivate");
            slider.slider("enable");
            targetTempInput.removeAttr("readonly");
        }
        else {
            thermostatBehavior.addClass("deactivate");
            slider.slider("disable");
            targetTempInput.attr("readonly", "readonly");
        }

    });
}

$(function () {
    $("#floors").accordion()
    heightStyle: "content";

    $('#dimension-switch').bootstrapSwitch('setSizeClass', 'switch-small');
    $(".floor-content").tabs().addClass("ui-tabs-vertical ui-helper-clearfix");
    $(".floor-content li").removeClass("ui-corner-top").addClass("ui-corner-left");

    $("#dialog-modal-temp").dialog({
        height: 'auto',
        width: 'auto',
        autoOpen: false,
        position: {
            my: "center",
            at: "center",
            of: $("body"),
            within: $("body")
        },
        modal: true,
        open: function (event, ui) {
            $("input").blur();
            $("#FromDate").datepicker("hide");
            $(this).parent().find('button').addClass('ui-button-icon-primary ui-icon ui-icon-closethick');

        },
        close: function (event, ui) {
            $('#temp-plot').highcharts().destroy();
        },
    });
    $("#dialog-modal-message").dialog({
        dialogClass: 'noTitleStuff',
        height: 50,
        width: 'auto',
        autoOpen: false,

        position: {
            my: "center",
            at: "center",
            of: $("body"),
            within: $("body")
        },
        modal: true,
        open: function (event, ui) {
            $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
        }
    });
    alertTempInit();
    thermostatInit();
});