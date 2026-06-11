(function($){
    $(function(){
        if (typeof flatpickr === 'undefined') return;

        $('.datetime-picker').each(function(){
            if ($(this).data('flatpickr-initialized')) return;
            flatpickr(this, {
                enableTime: true,
                dateFormat: $(this).data('date-format'),
                time_24hr: $(this).data('time-24hr') === true,
            });
            $(this).data('flatpickr-initialized', true);
        });
    });
})(jQuery);
