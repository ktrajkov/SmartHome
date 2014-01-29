
jQuery.validator.addMethod('requirediftrue', function (value, element, params) {
   
    var checkboxId = $(element).attr('data-val-requirediftrue-boolprop');
    var checkboxValue = $('#' + checkboxId).first().is(":checked");
    return !checkboxValue || value;
}, '');

jQuery.validator.unobtrusive.adapters.add('requirediftrue', {}, function (options) {
    options.rules['requirediftrue'] = true;
    options.messages['requirediftrue'] = options.message;
});


jQuery.validator.addMethod('maxtemprange', function (value, element, params) {
 
    var maxTempAlert = parseFloat(params.maxtemp);   
    var minTempAlert = parseFloat($('#' + params.mintempproperty).first().val());
    var currrentMaxTemp = parseFloat(value);
    if (isNaN(currrentMaxTemp) || isNaN(minTempAlert)) {
        return true;
    }
    return currrentMaxTemp < maxTempAlert && currrentMaxTemp >minTempAlert;
}, '');

jQuery.validator.unobtrusive.adapters.add('maxtemprange', ["maxtemp", "mintempproperty"], function (options) {
   
    options.rules['maxtemprange'] = options.params;
    options.messages['maxtemprange'] = options.message;
});

jQuery.validator.addMethod('mintemprange', function (value, element, params) {
  
    var minTempAlert = parseFloat(params.mintemp);
    var maxTempAlert = parseFloat($('#' + params.maxtempproperty).first().val());
    var currrentMinTemp = parseFloat(value);
    if (isNaN(currrentMinTemp) || isNaN(maxTempAlert))
    {
        return true;
    }
    return currrentMinTemp < maxTempAlert && currrentMinTemp > minTempAlert;
}, '');

jQuery.validator.unobtrusive.adapters.add('mintemprange', ["mintemp", "maxtempproperty"], function (options) {
   
    options.rules['mintemprange'] = options.params;
    options.messages['mintemprange'] = options.message;
});




