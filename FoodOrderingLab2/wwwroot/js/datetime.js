(function($){
    $(function(){
        if (typeof flatpickr === 'undefined') return;

        $('.datetime-picker').each(function(){
            if ($(this).data('flatpickr-initialized')) return;
            flatpickr(this, {
                enableTime: true,
                dateFormat: 'd.m.Y H:i',
                time_24hr: true,
            });
            $(this).data('flatpickr-initialized', true);
        });
    });
})(jQuery);
