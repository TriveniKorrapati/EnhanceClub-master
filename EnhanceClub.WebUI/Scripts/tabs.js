//import $ from 'jquery';

const initTabs = () => {
  $('.tabs .tab-nav button').on('click', function onClick() {
    const id = $(this).data('href');
    const tabId = $(this).closest('.tabs').attr('id');
    if ($(id).length) {
      $(this).closest('.tabs').find('.tab-nav li').removeClass('active');
      $(this).closest('.tabs').find('.tab-content .tab').removeClass('active');
      $(this).parent('li').addClass('active');
      $(id).addClass('active');

      if (tabId) {
        $(document).trigger('tab_updated', tabId);
      }
    }
  });
};

//export default initTabs;
