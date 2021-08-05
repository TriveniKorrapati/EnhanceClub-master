$(document).ready(function () {
    $('.btnEnabledId').hide();
  
    $('.btnDisabledId').show();
    $('.nextIdBtn').click(function (e) {
        
        $("#img-uploadedStep2").attr("src", "/content/images/placeholder_350.png");
        $("#rxUploadLink").removeClass("active");
        $('#idUploadLink').addClass('active');
       
    });
});



// change label text with file name selected to upload pres
$(".custom-uploadpres").change(function (e) {
   
    //$('#img-uploadedpres').attr('src', e.target.result);
    var fileUpload = $("#prescriptionImage").get(0);
    var files = fileUpload.files;


    // Create FormData object  
    var fileData = new FormData();

    // Looping over all files and add it to FormData object  
    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }

    // Adding one more key to FormData object  
    // fileData.append('returnUrl', $("#returnUrl").val());
    fileData.append('orderInvoiceId', $("#orderInvoiceId").val());

    $.ajax({
       // url: '/order/loadPrescription',
       url: '/order/loadPrescriptionS3',
        type: "POST",
        contentType: false, // Not to set any content header  
        processData: false, // Not to process data  
        data: fileData,
        success: function (result) {
            if (result.success) {
                $("#statusPres").html(result.message);
                $("#statusPresErr").html("");
            }
            else {
                $("#statusPres").html("");
                $("#statusPresErr").html(result.message);
            }
            if (!result.status) {
                $("#submit-documents").removeClass("d-none");
                //$("#button-next").removeClass("d-none");
            }
        },
        error: function (err) {
            $("#statusPres").html("");
            $("#statusPresErr").html("errr occured while uploading the document. Please try again.")
        }
    });
});

$(".uploader.custom-uploadpres").change(function (event) {
    readURL(this, '#img-uploadedpres');
});

function readURL(input, imgControlId) {
   // alert(imgControlId);
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $(imgControlId).attr('src', e.target.result);
            $(imgControlId).addClass('image-uploaded-success');
        }

        reader.readAsDataURL(input.files[0]); // convert to base64 string
    }
}
$("input[type='file']").on('change', function (event) {
    var id = $(this).attr('id');
    var url = '/order/loadIdS3';
    var updateButtonText = "Replace ID";
    var reloadImageId = '#img-replaceId';
    if (id == "imgReplace") {
        url = '/order/loadIdS3';
        reloadImageId = '#img-replaceId';
        updateButtonText = "Replace ID";
      
    }
    else if (id == "imgReplaceBack") {
        url = '/order/LoadIdS3BackImage';
        reloadImageId = '#img-replaceIdStep2';
        updateButtonText = "Replace back side";
        
    }
    else if (id == "imgInpStep2") {
        url = '/order/LoadIdS3BackImage';
        reloadImageId = '#img-uploadedStep2'; 
        updateButtonText = "Replace back side";
    }
    else if (id == "imgInp") {
        url = '/order/LoadIdS3';
        reloadImageId = '#img-uploaded';
        updateButtonText = "Replace ID";
       
    }

   
    var fileUpload = $(this).get(0);
    var files = fileUpload.files;
    if (files[0].size / 1024 < 4096) {



        // Create FormData object  
        var fileData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        var mobileDevice = $("#mobile-device").val();;
        var module = $("#module").val();

        fileData.append("mobileDevice", mobileDevice);
        fileData.append("module", module);

        // Adding one more key to FormData object  
        // fileData.append('returnUrl', $("#returnUrl").val());

        $.ajax({

            url: url,
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: fileData,

            success: function (result) {
                if (result.success) {
                    $(event.currentTarget).closest('li').find('.btnEnabledId').show();
                    $(event.currentTarget).closest('li').find('.btnDisabledId').hide();
                    $(event.currentTarget).closest('li').find('.upload-btn').html(updateButtonText);

                    $(".upload-documents.fs-current #status").html(result.message);
                    $(".upload-documents.fs-current #statusErr").html("");
                    $("#notify-message").removeClass("d-none").removeClass("error").addClass("success").html(result.message);

                    /* reload requirePrescription page from the server on successful upload of user id. */
                    //if (id == "imgInp") {
                    //    location.reload(true); 
                    //} 
                }
                else {
                    $(".upload-documents.fs-current #status").html("");
                    $(".upload-documents.fs-current #statusErr").html(result.message);
                    $("#notify-message").removeClass("d-none").removeClass("success").addClass("error").html(result.message);
                }
                if (!result.status) {
                    $("#enter-next").removeClass("d-none");
                    $("#button-next").removeClass("d-none");
                }
            },
            error: function (err) {
                $(".upload-documents.fs-current #status").html("");
                $(".upload-documents.fs-current #statusErr").html("error occured while uploading the document. Please try again.");
                $("#notify-message").removeClass("d-none").removeClass("success").addClass("error").html("Please upload jpeg/jpg/png/bmp only");
            },

        });
    }
    else {
        $(".upload-documents.fs-current #status").html("");
        $(".upload-documents.fs-current #statusErr").html("Please upload file size less than 4MB");
        $("#notify-message").removeClass("d-none").removeClass("success").addClass("error").html("Please upload file size less than 4MB");

    }
    readURL(this, reloadImageId);
});
