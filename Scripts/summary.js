(function ($) {
    var summaryVM = {
        itemCount: ko.observable(0),
        totalAmount: ko.observable(0.00)
    }

    if ($("article.summary").length > 0) {
        ko.applyBindings(summaryVM, $("article.summary")[0]);
    }

    function getUpdate(items) {
        $.ajaxSetup({ cache: false });
        var dataUrl = $("article.summary").data("load-shoppingcart-url");
        $.getJSON(dataUrl, function (data) {
            summaryVM.itemCount(data.itemCount);
            summaryVM.totalAmount(data.total);
        });
    }

    $("body").bind("updateOrchard", function(event) {
        getUpdate(event.data);
    });

    getUpdate();

})(jQuery);


