
//initializa file upload
$('.evaImageSelector').fileupload({
    dataType: "json",
    url: "/api/UploadVideo",
    limitConcurrentUploads: 1,
    sequentialUploads: true,
    maxNumberOfFiles: 1,
});

//remove any previous handlers on the file upload
$('.evaImageSelector').off('fileuploadadd');
//create a new handler for the current object for the file upload plugin
//so from now on every time a file is uploaded this handles the events
$('.evaImageSelector').on('fileuploadadd', 
    function (e, data) {
        data.submit();
        var fileNameContainer = $('.evaSelectedFile').get(0);
        $(fileNameContainer).empty();
        data.context = $('<div/>').text(data.files[0].name + "...Cargando").appendTo(fileNameContainer);
    });
//manipulate the on upload action to set the current image URL
$('.evaImageSelector').off('fileuploadsubmit');
$('.evaImageSelector').on('fileuploadsubmit', 
    function (e, data) {
        //use this to send custom data to the controller
    });
//initialize the done handler
//remove any event handler to prevent nested calls
$('.evaImageSelector').off('fileuploaddone');
$('.evaImageSelector').on('fileuploaddone', 
    function (e, data) {
        //TODO: store the location information.
        $('.evaVideoUrl').text(data.result.location);
        //we need some feeback on the results
        var fileNameContainer = $('.evaSelectedFile').get(0);
        $(fileNameContainer).empty();
        data.context = $('<div/>').text(data.files[0].name + "...Subido").appendTo(fileNameContainer);
    });

//handle the progress bar.
$('.evaImageSelector').off('fileuploadprogressall');
$('.evaImageSelector').on('fileuploadprogressall',
    function (e, data) {
        //we need some feeback on the results
        var progress = parseInt(data.loaded / data.total * 100, 10);
        $('#progress .progress-bar').css(
                'width',
                progress + '%'
            );
    });

//make a special mark if the file upload fails.
$('.evaImageSelector').off('fileuploadfail');
$('.evaImageSelector').on('fileuploadfail', 
    function (e, data) {
        //we need some feeback on the results
        var fileNameContainer = $('.evaSelectedFile').get(0);
        $(fileNameContainer).empty();
        data.context = $('<div/>').text(data.files[0].name + "...Error").appendTo(fileNameContainer);
});