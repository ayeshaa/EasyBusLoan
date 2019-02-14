
function initAutocomplete() {
    autocomplete = new google.maps.places.Autocomplete(
        document.getElementById('autocomplete'), { types: ['geocode'] });
    autocomplete.addListener('place_changed', fillInAddress);
    autocomplete2 = new google.maps.places.Autocomplete(
        document.getElementById('Applicant_city'), { types: ['geocode'] });
    autocomplete2.addListener('place_changed', fillInAddress2);
    applicantGuarantorsAutoComplete = new google.maps.places.Autocomplete(
        document.getElementById('ApplicantGuarantors_city'), { types: ['geocode'] });
    applicantGuarantorsAutoComplete.addListener('place_changed', fillInAddress3);
    
}
function isEmpty(value) {
    return typeof value == 'string' && !value.trim() || typeof value == 'undefined' || value === null;
}
function fillInAddress() {
    var place = autocomplete.getPlace();
    var parsedResult = $(place.adr_address);
    var stateVal = parsedResult.filter('.region').text();
    var CityVal = parsedResult.filter('.locality').text();
    var country = parsedResult.filter('.country-name').text();
    console.log(stateVal);
    if (!isEmpty(stateVal)) {
        $("#ApplicantDetails_0__ApplicantVehicles_0__CityOrState").val(stateVal);
        $("#Vehicles_0__CityOrState").val(stateVal);
    }
  
}
function fillInAddress2() {
    var place = autocomplete2.getPlace();
    var parsedResult = $(place.adr_address);
    var stateVal = parsedResult.filter('.region').text();
    var CityVal = parsedResult.filter('.locality').text();
    var country = parsedResult.filter('.country-name').text();
    if (!isEmpty(stateVal)) {
        $("#Applicant_state").val(stateVal);
    }
    if (!isEmpty(country)) {
        $("#Applicant_country").val(country);
    }

}
function fillInAddress3() {
    var place = applicantGuarantorsAutoComplete.getPlace();
    var parsedResult = $(place.adr_address);
    var stateVal = parsedResult.filter('.region').text();
    var CityVal = parsedResult.filter('.locality').text();
    var country = parsedResult.filter('.country-name').text();
    if (!isEmpty(stateVal)) {
        $("#ApplicantGuarantors_state").val(stateVal);
    }

}
function geolocate() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var geolocation = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            var circle = new google.maps.Circle(
                { center: geolocation, radius: position.coords.accuracy });
            autocomplete.setBounds(circle.getBounds());
        });
    }
}
   