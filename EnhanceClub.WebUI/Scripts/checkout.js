$(window).bind('setup', function () {
    if (parseInt(($(".ld-module-trigger-count").html())) > 0) {
        showCartPopup();
    }
});

$(document).ready(function () {
    
    $(".navbar-main").addClass("navbar-main--white");
    if ($("#multiple-address").val() == "1") {
        $(".address-detail").attr('data-content', "Selected Address");

        // expand billing address by default
        $("#same-billing-address").slideDown();
    }
    else {
        $(".billing-address").attr('checked', true);
    }
   
    //code for pattern in credit card number
    $('.creditCardText').keypress(function () {
        if (event.which != 8 && isNaN(String.fromCharCode(event.which))) {
            return false;
        }
        else {
            var cardNum = $(this).val().split("-").join(""); // remove hyphens
            if (cardNum.length > 0) {
                cardNum = cardNum.match(new RegExp('.{1,4}', 'g')).join("-");
            }
            $(this).val(cardNum);
        }
    });
    //code for pattern in credit card number
    $('.creditCardText').blur(function () {
        var cardNum = $(this).val().split("-").join(""); // remove hyphens
        if (cardNum.length > 0) {
            cardNum = cardNum.match(new RegExp('.{1,4}', 'g')).join("-");
        }
        $(this).val(cardNum);
        
    });
    

    // if same billing address is checked when page loads, copy the same address details from shipping address
    if ($(".billing-address").prop('checked')) {
        $("#Customer_CustomerAddress").val($("#ShippingAddress").val());
        $("#Customer_CustomerCity").val($("#ShippingCity").val());
        $("#Customer_CustomerZipCode").val($("#ShippingZipCode").val());
        $("#Customer_CustomerProvinceId").val($("#ShippingProvinceId").val());
    }
    //if credit card error, then set focus on card number 
    if ($("#credit-card-message").html() != undefined) {
        $("#cardNumber").focus();
    }

    // if error on coupon applied then set focus on coupon text box
    if ($("#coupon-message").html() != undefined) {
        $("#couponCode").focus();
        $("#coupon-box").addClass("error");
    }

    // if error on credit applied then set focus on credit text box
    if ($("#credit-message").val() != undefined) {
        $("#creditApplied").focus();
        $("#credit-box").addClass("error");
    }
    $(".error-btn").on("click", function (e) {
        $('.CouponCodeClass').val('');
        $('.CrossClass').val('1');
        
        applyChanges();
    });
    $(".edit-address").on("click", function (e) {
        $(this).toggleClass("active");
        $(this).parent().parent().next().next(".address-form").toggle();
        // $(this).parent().parent().find(".input-wrapper input").focus();
        if ($(this).hasClass("active")) {
            $(this).text("Cancel");
        } else {
            $(this).text("Edit Address");
        }
        e.preventDefault();
    });

    $(".update-address").on("click", function (e) {
        $(this).parent().parent().parent().parent().parent().hide();
        $(this).parent().parent().parent().parent().parent().parent().find(".edit-address").removeClass("active").text("Edit Address");
        e.preventDefault();
    });

    $(".billing-address").on("click", function (e) {
        if ($(this).is(":not(:checked)")) {
            $(this).parent().parent().parent().parent().next(".address-form").slideDown();
        } else {
            $(this).parent().parent().parent().parent().next(".address-form").slideUp();
            
            if ($("#multiple-address").val() == "1") {
                $("#Customer_BillingFirstName").val($(".address-detail.active #CustomerFirstName").val());
                $("#Customer_BillingLastName").val($(".address-detail.active #CustomerLastName").val());
                $("#Customer_CustomerAddress").val($(".address-detail.active #ShippingAddress").val());
                $("#Customer_CustomerCity").val($(".address-detail.active #ShippingCity").val());
                $("#Customer_CustomerZipCode").val($(".address-detail.active #ShippingZipCode").val());
                $("#Customer_CustomerProvinceId").val($(".address-detail.active #ShippingProvinceId-hidden").val());
            }
            else {
                $("#Customer_CustomerAddress").val($("#ShippingAddress").val());
                $("#Customer_CustomerCity").val($("#ShippingCity").val());
                $("#Customer_CustomerZipCode").val($("#ShippingZipCode").val());
                $("#Customer_CustomerProvinceId").val($("#ShippingProvinceId").val());
            }
            
        }
    });

    $("#ShippingAddress").change(function () {
        if ($(".billing-address").prop('checked')) {
            $("#Customer_CustomerAddress").val($("#ShippingAddress").val());
        }
    });

    $("#ShippingCity").change(function () {
        if ($(".billing-address").prop('checked')) {
            $("#Customer_CustomerCity").val($("#ShippingCity").val());
        }
    });

    $("#ShippingZipCode").change(function () {
        if ($(".billing-address").prop('checked')) {
            $("#Customer_CustomerZipCode").val($("#ShippingZipCode").val());
        }
    });

    $("#ShippingProvinceId").change(function () {
        var shippingProvinceFk = $("#ShippingProvinceId").val();
        if ($(".billing-address").prop('checked')) {
            $("#Customer_CustomerProvinceId").val(shippingProvinceFk);
            $("#hiddenProvince").val(shippingProvinceFk);
        }

        //update tax details in cart and order summary section
        $.post("/order/updatecartprovincialtax", { customerProvinceFk: shippingProvinceFk }, function (data) {
            //alert(data);
            $("#order-summary").html(data);
            // $('.popup-cart').toggleClass('active');
        });
    });

    // show address fields before submitting form to create order,
    // to catch and show any validation error
    $("#submit-order").click(function () {
        $(".address-form").show();
    });

    $(".auto-refill").on("change", function () {    
        var name = $(this).attr("name");
        var checked = $(this).prop('checked');
        $(this).prop('checked', checked);      
        $('[name=\"' + name + '\"]:hidden').val(checked);
    });

    // multiple shipping address changes
    $(".address-section .edit-btn").on("click", function (e) {
        $(this).toggleClass("active");
        $(this).parent().parent().find(".input-value-section").toggle();
        $(this).parent().parent().find(".input-wrapper").toggle();
        $(this).parent().parent().find(".save-container").toggle();
        // $(this).parent().parent().find(".input-wrapper input").focus();
        if ($(this).hasClass("active")) {
            $(this).text("Cancel");
            $(this).find("svg").hide();
        } else {
            $(this).text("Edit");
            $(this).find("svg").show();
        }
        e.preventDefault();
    })

    $(".address-detail .address-checkbox").on("click", function (e) {
        if ($(this).is(":checked")) {
            //$(this).closest(".address-detail").attr('data-content', 'Selected Address');
            $(this).closest(".address-detail").addClass("active").closest(".col-md-6").siblings().find(".address-detail").removeClass("active");
        } else {
            $(this).closest(".address-detail").removeClass("active");
        }

        // update default shipping address in database
        var shippingAdressFk = $(this).closest(".address-detail").closest(".col-md-6").find("#shipping-address-fk").val();

        $("#selected-ship-address").val(shippingAdressFk);

        // check if same as shipping address is checked
        if ($(".billing-address").prop('checked')) {
            $("#Customer_BillingFirstName").val($(".address-detail.active #CustomerFirstName").val());
            $("#Customer_BillingLastName").val($(".address-detail.active #CustomerLastName").val());
            $("#Customer_CustomerAddress").val($(".address-detail.active #ShippingAddress").val());
            $("#Customer_CustomerCity").val($(".address-detail.active #ShippingCity").val());
            $("#Customer_CustomerZipCode").val($(".address-detail.active #ShippingZipCode").val());
            $("#Customer_CustomerProvinceId").val($(".address-detail.active #ShippingProvinceId").val());
        }

        //
        //$.ajax({
        //    url: "/customer/UpdateCustomerShippingAddressDefaultFlag",
        //    contentType: "application/json",
        //    data: { shippingAdressFk: shippingAdressFk },
        //    type: 'Get',
        //    dataType: 'json',
        //    success: function (data) {
        //        if (data.status) {

        //        }
        //        else {

        //        }
        //    },
        //    error: function (response) {

        //    }

        //});
    });

     

});

// submit form on apply coupon and apply credit button click
function applyChanges() {  

    var CartNetTotal = $("#CartNetTotal").val();
    var creditApplied = $("#creditApplied").val();
    var maxCreditAmount = $("#MaxCreditAmount").val();

    if (typeof CartNetTotal !== "undefined") {
        CartNetTotal = parseFloat(creditApplied);
    }

    if (typeof creditApplied !== "undefined") {
        creditApplied = parseFloat(creditApplied);
    }

    if (typeof CartNetTotal !== "undefined") {
        CartNetTotal = parseFloat(CartNetTotal);
    }

    if (creditApplied > parseFloat(maxCreditAmount)) {
        creditApplied = maxCreditAmount;
    }

    if (creditApplied > CartNetTotal) {
        creditApplied = creditApplied - (creditApplied - CartNetTotal), 2; 
    }

    if (creditApplied > 0) {
        var creditValue = parseFloat(creditApplied).toFixed(2);
        $("#creditApplied").val(creditValue);
    }
    else {
        $("#creditApplied").val(0);
    } 

    $("#frm_checkout").submit(); 
   
}
