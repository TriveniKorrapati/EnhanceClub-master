
  $(window).scroll(function () {
    if ($(this).scrollTop() > window.innerHeight) {
      $('.navbar').addClass('active');
    } else {
      $('.navbar').removeClass('active');
    }
    /*
    if ($(this).scrollTop() > 0) {
      $('.home-banner').addClass('scroll-in');
    } else {
      $('.home-banner').removeClass('scroll-in');
    }
    */
  });
