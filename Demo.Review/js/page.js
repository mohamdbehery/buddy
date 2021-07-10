$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/review-source.xml",
        dataType: "xml",
        success: function (xml) {
            console.log('>>>>>>>>>>>>' + xml);
        }
    });
});