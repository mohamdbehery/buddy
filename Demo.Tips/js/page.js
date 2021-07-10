$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/review-source.xml",
        dataType: "xml",
        success: function (xml) {
            debugger;
            if (xml.children[0] && xml.children[0].childNodes) {
                var filledItems;
                var itemTemplate = $('#item-template').html();
                for (let i = 0; i < xml.children[0].childNodes.length; i++) {
                    
                }
            }
        }
    });
});