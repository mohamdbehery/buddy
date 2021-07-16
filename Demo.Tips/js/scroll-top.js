$(document).ready(function () {
    ShowHideScrollTop();

    $(window).scroll(ShowHideScrollTop);

    $('#toTopBtn').click(function () {
        $("html, body").animate({
            scrollTop: 0
        }, 1000);
        return false;
    });

    function ShowHideScrollTop() {
        if ($(this).scrollTop() > 20) {
            $('#toTopBtn').fadeIn();
        } else {
            $('#toTopBtn').fadeOut();
        }
    }
});