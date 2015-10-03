//add the product id to the shopping cart on the server (JSON POST with Antiforgery)
var addToCart = function (productId) {
    var tokenName = '__RequestVerificationToken';
    var token = $("input[type='hidden'][name='" + tokenName + "']").val(); // getAntiForgeryToken();
    var url = $(".addtocartbuttons").data("addtocart-url");
    var headers = {};
    headers[tokenName] = token;
    var data = { productId: productId, __RequestVerificationToken: token};
    var config = {
        url: url,
        type: "POST",
        headers: headers,
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function(data, textStatus, jqXHR) {
             $("body").trigger("updateOrchard", data);
        }
    };
    $.ajax(config);

};
