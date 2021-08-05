//Created by Rajiv S
$(document).ready(function () {
    
});

$("#mc-embedded-subscribe").click(function () {

    var btnText = $("#mc-embedded-subscribe").val();

    $("#mc-embedded-subscribe").val("Subscribing...");
    //alert($("#mce-EMAIL").length);
    //alert($("#mce-EMAIL").val());

    var subscriberEmail = $("#mce-EMAIL").val();
    var subscriberName = $("#mce-NAME").val();


    alert(subscriberEmail);
    alert(subscriberName);


    if (subscriberEmail == "") {
        alert("please enter email.");
    } else {
        $.ajax({
            url: '/marketing/SubscribeUser',
            contentType: "application/json",
            data: { name: subscriberName, email: subscriberEmail },
            type: 'Get',
            dataType: 'json',
            success: function (data) {
                if (data == "success") {
                    $("#mc_embed_signup_scroll").hide();
                    $(".subscribe-success").show();
                    $("#mc-embedded-subscribe").val(btnText);
                }
            },
            error: function (response) {
                console.log("error in subscribing");
            }
        });
    }
});