(function($){
    $(function(){
        function initSelect(el){
            var $el = $(el);
            var api = $el.data('api');
            if (!api) return;

            $el.select2({
                ajax: {
                    url: api,
                    dataType: 'json',
                    delay: 250,
                    data: function(params){ return { q: params.term }; },
                    processResults: function(data){ return { results: data }; }
                },
                minimumInputLength: 1,
                width: '100%',
                placeholder: 'Type to search...'
            });
        }

        $('.autocomplete-select').each(function(){ initSelect(this); });

        // initialize when control is focused (for dynamically injected content)
        $(document).on('focus', '.autocomplete-select', function(){
            var $s = $(this);
            if (!$s.hasClass('select2-hidden-accessible')){
                initSelect(this);
            }
        });
    });
})(jQuery);
