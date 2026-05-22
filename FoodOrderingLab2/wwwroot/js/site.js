(function($){
    $(function(){
        function renderCards($container, template, items){
            if (!Array.isArray(items) || items.length === 0) {
                $container.html('<div class="search-empty">Nema rezultata za prikaz.</div>');
                return;
            }

            var html = items.map(function(item){
                switch (template) {
                    case 'customer':
                        return '<div class="card">'
                            + '<div class="card-header"><h3 class="card-title">' + escapeHtml(item.fullName || item.text) + '</h3></div>'
                            + '<div class="card-body"><p class="card-description"><strong>Email:</strong> ' + escapeHtml(item.email || '-') + '</p>'
                            + '<p class="card-description"><strong>Phone:</strong> ' + escapeHtml(item.phone || '-') + '</p></div>'
                            + '<div class="card-footer"><a class="btn btn-secondary" href="/kupci/' + item.id + '">View</a>'
                            + ' <a class="btn btn-secondary" href="/kupci/edit/' + item.id + '">Edit</a></div>'
                            + '</div>';
                    case 'restaurant':
                        return '<div class="card">'
                            + '<div class="card-header"><h3 class="card-title">' + escapeHtml(item.name || item.text) + '</h3></div>'
                            + '<div class="card-body"><p class="card-description"><strong>Rating:</strong> ' + escapeHtml(item.rating != null ? item.rating : '-') + '</p>'
                            + '<p class="card-description">' + escapeHtml(item.address || '-') + '</p></div>'
                            + '<div class="card-footer"><a class="btn btn-secondary" href="/restorani/' + item.id + '">View</a>'
                            + ' <a class="btn btn-secondary" href="/restorani/edit/' + item.id + '">Edit</a></div>'
                            + '</div>';
                    case 'menuitem':
                        return '<div class="card">'
                            + '<div class="card-header"><h3 class="card-title">' + escapeHtml(item.name || item.text) + '</h3></div>'
                            + '<div class="card-body"><p class="card-description"><strong>Category:</strong> ' + escapeHtml(item.category || '-') + '</p>'
                            + '<p class="card-description"><strong>Price:</strong> ' + escapeHtml(item.price ? item.price + ' EUR' : '-') + '</p></div>'
                            + '<div class="card-footer"><a class="btn btn-secondary" href="/meni/' + item.id + '">Details</a>'
                            + ' <a class="btn btn-secondary" href="/meni/edit/' + item.id + '">Edit</a>'
                            + '</div>'
                            + '</div>';
                    case 'order':
                        return '<div class="order-card">'
                            + '<h3>Narudžba #' + escapeHtml(item.id) + '</h3>'
                            + '<p><strong>Kupac:</strong> ' + escapeHtml(item.customerName || '-') + '</p>'
                            + '<p><strong>Restoran:</strong> ' + escapeHtml(item.restaurantName || '-') + '</p>'
                            + '<p><strong>Status:</strong> ' + escapeHtml(item.status || '-') + '</p>'
                            + '<p><strong>Cijena:</strong> ' + escapeHtml(item.totalPrice ? item.totalPrice + ' EUR' : '-') + '</p>'
                            + '<p><strong>Datum:</strong> ' + escapeHtml(item.orderDateText || '-') + '</p>'
                            + '<div class="btn-group-inline" style="margin-top: 10px;">'
                            + '<a class="btn btn-secondary" href="/narudzbe/' + item.id + '">Pregled</a>'
                            + '<a class="btn btn-secondary" href="/narudzbe/edit/' + item.id + '">Uredi</a>'
                            + '</div>'
                            + '</div>';
                    default:
                        return '<div class="search-empty">Nepoznati template za prikaz rezultata.</div>';
                }
            }).join('');

            $container.html(html);
        }

        function escapeHtml(value){
            return String(value || '').replace(/[&<>"]+/g, function(match){
                return {
                    '&':'&amp;',
                    '<':'&lt;',
                    '>':'&gt;',
                    '"':'&quot;'
                }[match];
            });
        }

        $('[data-search-api]').each(function(){
            var $input = $(this);
            var api = $input.data('search-api');
            var target = $input.data('search-target');
            var template = $input.data('search-template');
            if (!api || !target || !template) return;

            var $container = $(target);
            if (!$container.length) return;
            var originalMarkup = $container.html();
            var timerId;

            function loadResults(query){
                $.getJSON(api, { q: query }, function(data){
                    renderCards($container, template, data);
                });
            }

            $input.on('input', function(){
                var query = $input.val().trim();
                clearTimeout(timerId);
                if (query === '') {
                    $container.html(originalMarkup);
                    return;
                }
                timerId = setTimeout(function(){ loadResults(query); }, 250);
            });
        });
    });
})(jQuery);
