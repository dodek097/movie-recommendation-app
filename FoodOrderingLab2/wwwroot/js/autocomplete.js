(function($){
    $(function(){
        function initSelect(el){
            var $el = $(el);
            var api = $el.data('api');
            if (!api) return;

            var placeholderText = $el.data('placeholder') || 'Type to search...';
            var dropdownClass = $el.hasClass('item-menu') ? 'item-menu-dropdown' : '';
            var selectOptions = {
                ajax: {
                    url: api,
                    dataType: 'json',
                    delay: 250,
                    data: function(params){ return { q: params.term || '' }; },
                    processResults: function(data){ return { results: data }; }
                },
                dropdownParent: $('body'),
                minimumInputLength: 0,
                width: '100%',
                placeholder: placeholderText,
                allowClear: true
            };
            if (dropdownClass) {
                selectOptions.dropdownCssClass = dropdownClass;
            }
            $el.select2(selectOptions);
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
