function BusyIndicatorUp() {
    $(".loading-wrapper").fadeIn('slow');
}
function BusyIndicatorDown() {
    $(".loading-wrapper").fadeOut('slow');
}
$(document).ajaxSend(function () {
    $(".loading-wrapper").fadeIn('slow');
}).ajaxComplete(function () {
    $(".loading-wrapper").fadeOut('slow');
});

$('[data-toggle="tooltip"]').tooltip(); 