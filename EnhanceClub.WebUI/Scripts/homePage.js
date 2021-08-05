$(document).ready(function () {
    var btn = $('#back-to-top');
    $(window).scroll(function () {
        if ($(window).scrollTop() > 300) {
            btn.addClass('show');
        } else {
            btn.removeClass('show');
        }
    });
    btn.on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({ scrollTop: 0 }, '300');
    });

    // get the quantities for selected drop down
    var productId = $('#menu-generic option:selected').val();
    var strength = $('#menu-generic :selected').text();
    BindQuantityDropDown(true, productId, strength);

    // on change event bind quantities dropdown again
    $(".products-gallery--content").on('change', '#menu-generic', function () {
        productId = $("#menu-generic option:selected").val();
        strength = $('#menu-generic :selected').text();

        BindQuantityDropDown(true, productId, strength);
    });

    $(".products-gallery--content").on('change', '#menu-brand', function () {
        productId = $("#menu-brand option:selected").val();
        strength = $('#menu-brand :selected').text();
        BindQuantityDropDown(false, productId, strength);
    });

    $(".cart-discount").click(function (e) {
        e.preventDefault();
    });

    $(".product-page__popup-link").on("click", function (e) {
        $(".product-page__popup").toggleClass("active");
        e.preventDefault();
    })
    $(".product-page__popup__exit").on("click", function (e) {
        $(".product-page__popup").removeClass("active");
        e.preventDefault();
    })
});

function BindQuantityDropDown(isGeneric, productId, strength) {

    $.ajax({
        url: "/home/GetProductSizeQuantityList",
        type: "Get",
        contentType: "application/json",
        dataType: "json",
        async: false,
        data: { strength: strength, productFk: productId, productGeneric: isGeneric },
        success: function (response) {

            //$("#menu-quantity").empty();
            //$("#menu-quantity-web").empty();
            $('select[name="quantity-productsizeId"]').empty();

            $.each(response, function (_key, value) {
                $('select[name="quantity-productsizeId"]').append($("<option></option>").val(value.ProductSizeId).html(value.DisplayQuantity));
                //$("#menu-quantity").append($("<option></option>").val(value.ProductSizeId).html(value.DisplayQuantity));
                //if ($("#menu-quantity-web").length > 0) {
                //    $('select[name="quantity-productsizeId"]').append($("<option></option>").val(value.ProductSizeId).html(value.DisplayQuantity));
                //    //$("#menu-quantity-web").append($("<option></option>").val(value.ProductSizeId).html(value.DisplayQuantity));
                //}
            });

        },
        error: function (response) {
            console.log("error in binding product Size quantity")

        }

    });
}

//start: cart popup js
// open cart popup on clicking the cart in header
$('.cart-open').click(function () {
    showCartPopup();
});

$('#cart-popup').on('click', '.btn-cart-close', function () {
    $('.popup-cart').toggleClass('active');
});

$(".nav-link.cart-open").on("click", function () {
    $(".popup-cart-blocker").addClass("active");
});

$('#cart-popup').on('click', ".btn-cart-close", function () { 
    $(".popup-cart-blocker").removeClass("active");
});

$('#cart-popup').on('click', '.popup-cart-blocker', function () {
    $(this).removeClass("active");
    $(".popup-cart").removeClass("active");
});

$('#place-order').click(function () {

    $('.place-order, .popup-cart').toggleClass('active');


});

$('.close-popup').click(function () {
    $('.popup-box').toggleClass('active');
});

$('#cart-popup').on('click', '.increase-count', function () {
    var currentVal = $(this).closest('.count').children('.count-value').html();
    var updatedCount = parseInt(currentVal) + 1;
    var productSizeId = $(this).closest('.count').children('.prod-size-fk').val();

    $(this).closest('.count').children('.count-value').html(updatedCount);

    updatecart(productSizeId, updatedCount);

});

$('#cart-popup').on('click', '.decrease-count', function () {

    var currentVal = $(this).closest('.count').children('.count-value').html();
    var updatedCount = parseInt(currentVal) - 1;
    var productSizeId = $(this).closest('.count').children('.prod-size-fk').val();

    if (parseInt(currentVal) > 1) {
        $(this).closest('.count').children('.count-value').html(parseInt(currentVal) - 1);

        // to do update cart and subtotal
        updatecart(productSizeId, updatedCount);
    }
});

// update cart when cart item count is increased or decreased
function updatecart(productSizeId, updatedCount) {
    // update cart and subtotal
    $.ajax({
        url: "/cart/updatecartitem",
        contentType: "application/json",
        data: { productSizeFk: productSizeId, productSizeQuantity: updatedCount },
        type: 'Get',
        dataType: 'json',
        success: function (data) {
            // update cart summary
            //alert(data.cartItemsCount);
            showCartPopup();
        },
        error: function (response) {
            console.log("error in adding products to cart");
        }

    });
}
function removeCartItem(productSizeId) {
    $.post("/cart/removeFromCart", { productId: productSizeId }, function (data) {
        showCartPopup();
        $(".ld-module-trigger-count").html(data.cartItemsCount);
    });
}

function showCartPopup() {
    $.post("/cart/CartPopup", function (data) {
        // alert(data);
        $("#cart-popup").html(data);
        $('.popup-cart').toggleClass('active');
        $(".popup-cart-blocker").addClass("active");
    });
}

function replaceCartItem(currentProductSizeFk, updatedProductSizeFk, frontendVisibleProductSizeFk) {
    var path = window.location.pathname;

    $.post("/cart/ReplaceCartItem", { currentProductSizeFk, updatedProductSizeFk, frontendVisibleProductSizeFk }, function (data) {
        // alert(data);        

        if (path.toLowerCase() == "/order/checkout") {
            window.location.reload();
        } else {
            $("#cart-popup").html(data);
            $('.popup-cart').toggleClass('active');
        }
    });
    return false;
}

//end: cart popup js

// called from product detail page mobile on click of get it now button  
$("#get-product-brand-mobile").click(function () {
    if (($("#menu-brand-mobile").val() != 0) && ($("#menu-selection-mobile").val() != 0)) {
        var productSizeId = $("#menu-brand-mobile").val();

        // check if quantity is shown in separate dropdown
        if ($("#menu-quantity").length > 0) {
            productSizeId = $('[name=quantity-productsizeId]').val();
        }
        var productId = $("#product-id").val();

        addProductToCart(productSizeId, productId);
        //return true;
    }
    else {
        alert('Please choose a valid product & strength.');
        //return false;
    }
});
$("#get-product-generic-mobile").click(function () {
    if (($("#menu-generic-mobile").val() != 0) && ($("#menu-selection-mobile").val() != 0)) {
        var productSizeId = $("#menu-generic-mobile").val();

        // check if quantity is shown in separate dropdown
        if ($("#menu-quantity").length > 0) {
            productSizeId = $('[name=quantity-productsizeId]').val();
        }
        var productId = $("#product-id").val();
        addProductToCart(productSizeId, productId);
        //return true;
    }
    else {
        alert('Please choose a valid product & strength.');
        //return false;
    }
});

// called from product detail page web on click of get it now button menu-selection-web
$("#get-product-brand-web").click(function () {
    if (($("#menu-brand-web").val() != 0) && ($("#menu-selection-web").val() != 0)) {
        var productSizeId = $("#menu-brand-web").val();

        // check if quantity is shown in separate dropdown
        if ($("#menu-quantity-web").length > 0) {
            productSizeId = $('[name=quantity-productsizeId]').val();
        }
        var productId = $("#product-id").val();
        addProductToCart(productSizeId, productId);
        //return true;
    }
    else {
        alert('Please choose a valid product & strength.');
        //return false;
    }
});
$("#get-product-generic-web").click(function () {
    if (($("#menu-generic-web").val() != 0) && ($("#menu-selection-web").val() != 0)) {

        var productSizeId = $("#menu-generic-web").val();

        // check if quantity is shown in separate dropdown
        if ($("#menu-quantity-web").length > 0) {
            productSizeId = $('[name=quantity-productsizeId]').val();
        }
        var productId = $("#product-id").val();
        addProductToCart(productSizeId, productId);
        //return true;
    }
    else {
        alert('Please choose a valid product & strength.');
        //return false;
    }
});
function addProductToCart(productSizeId, productId) {

    $.ajax({
        url: "/cart/addtocart",
        contentType: "application/json",
        data: { productsizeId: productSizeId, productFk: productId },
        type: 'Get',
        dataType: 'json',
        success: function (data) {
            // update cart summary
            //alert(data.cartItemsCount);
            if (data.cartItemAdded) {
                showCartPopup();
                $(".ld-module-trigger-count").html(data.cartItemsCount);
            }
            else {
                showCartPopup();
            }

        },
        error: function (response) {
            console.log("error in adding products to cart");
        }

    });
}
//start: home page js for products

//end: home page js for products

// start js to check for internal/external links
$('a').click(function () {

    var linkUrl = $(this).attr("href");
    //alert(linkUrl);

    if (linkUrl.indexOf("staging.enhanceclub") > 0 || linkUrl.indexOf("/") == 0 || linkUrl.indexOf("enhanceclub.com") > 0) {

        // internal link
        //alert("internal");


    } else {

        // external link
        // alert("external");
        $(this).attr('target', '_blank');

    }

    return true; // return true so that the browser will navigate to the clicked a's href
});
