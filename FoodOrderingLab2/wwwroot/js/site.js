(function($){
    $(function(){
        var canManage = document.body.dataset.canManage === 'true';
        var canDelete = document.body.dataset.canDelete === 'true';

        function formatCroatianPhone(value) {
            var digits = String(value || '').replace(/\D/g, '');
            if (digits.indexOf('385') === 0) digits = digits.substring(3);
            else if (digits.indexOf('0') === 0) digits = digits.substring(1);

            if (/^1\d{7}$/.test(digits)) {
                return '+385 ' + digits.substring(0, 1) + ' ' + digits.substring(1, 4) + ' ' + digits.substring(4);
            }
            if (/^[2-5]\d{7}$/.test(digits)) {
                return '+385 ' + digits.substring(0, 2) + ' ' + digits.substring(2, 5) + ' ' + digits.substring(5);
            }
            if (/^9\d{8}$/.test(digits)) {
                return '+385 ' + digits.substring(0, 2) + ' ' + digits.substring(2, 5) + ' ' + digits.substring(5);
            }
            return value;
        }

        $(document).on('blur', 'input[type="tel"]', function () {
            this.value = formatCroatianPhone(this.value);
            $(this).trigger('change');
        });

        function renderCards($container, template, items){
            $container.toggleClass('has-search-results', Array.isArray(items) && items.length > 0);

            if (!Array.isArray(items) || items.length === 0) {
                $container.html('<div class="search-empty">Nema rezultata za prikaz.</div>');
                return;
            }

            var html = items.map(function(item){
                switch (template) {
                    case 'customer':
                        return '<div class="card entity-card">'
                            + '<div class="card-header"><span class="card-kicker">Customer</span><h3 class="card-title">' + escapeHtml(item.fullName || item.text) + '</h3></div>'
                            + '<div class="card-body">'
                                + '<div class="card-stat-row"><span>Email</span><strong>' + escapeHtml(item.email || '-') + '</strong></div>'
                                + '<div class="card-stat-row"><span>Phone</span><strong>' + escapeHtml(item.phone || '-') + '</strong></div>'
                            + '</div>'
                            + '<div class="card-footer">'
                                + '<a class="btn btn-secondary" href="/kupci/' + item.id + '">View</a> '
                                + (canManage ? '<a class="btn btn-outline" href="/kupci/edit/' + item.id + '">Edit</a> ' : '')

                                + (canDelete ? '<form method="post" action="/kupci/delete/' + item.id + '" style="display:inline;" '
                                    + 'onsubmit="return confirm(&quot;Jeste li sigurni da želite obrisati ovog kupca?&quot;);">'

                                    + antiForgeryInput()

                                    + '<button type="submit" class="btn btn-danger">Delete</button>'
                                + '</form>' : '')

                            + '</div>'
                        + '</div>';
                    case 'restaurant':
                        return '<div class="card entity-card">'
                            + '<div class="card-header"><span class="card-kicker">Restaurant</span><h3 class="card-title">' + escapeHtml(item.name || item.text) + '</h3></div>'
                            + '<div class="card-body">'
                                + '<div class="card-stat-row"><span>Rating</span><strong>' + escapeHtml(item.rating != null ? item.rating + ' / 5' : '-') + '</strong></div>'
                                + '<div class="card-stat-row"><span>Address</span><strong>' + escapeHtml(item.address || '-') + '</strong></div>'
                            + '</div>'
                            + '<div class="card-footer">'
                                + '<a class="btn btn-secondary" href="/restorani/' + item.id + '">View</a> '
                                + (canManage ? '<a class="btn btn-outline" href="/restorani/edit/' + item.id + '">Edit</a> ' : '')

                                + (canDelete ? '<form method="post" action="/restorani/delete/' + item.id + '" style="display:inline;">'
                                    + antiForgeryInput()
                                    + '<button type="submit" class="btn btn-danger" onclick="return confirm(\'Jeste li sigurni da želite obrisati ovaj restoran?\');">'
                                    + 'Delete</button>'
                                + '</form>' : '')

                            + '</div>'
                        + '</div>';
                    case 'menuitem':
                        return '<div class="card entity-card">'
                            + '<div class="card-header"><span class="card-kicker">' + escapeHtml(item.category || 'Menu item') + '</span><h3 class="card-title">' + escapeHtml(item.name || item.text) + '</h3></div>'
                            + '<div class="card-body">'
                                + '<div class="card-stat-row"><span>Price</span><strong>' + escapeHtml(item.price ? item.price + ' EUR' : '-') + '</strong></div>'
                                + '<div class="card-stat-row"><span>Restaurant</span><strong>' + escapeHtml(item.restaurantName || '-') + '</strong></div>'
                            + '</div>'
                            + '<div class="card-footer">'
                                + '<a class="btn btn-secondary" href="/meni/' + item.id + '">View</a> '
                                + (canManage ? '<a class="btn btn-outline" href="/meni/edit/' + item.id + '">Edit</a> ' : '')

                                + (canDelete ? '<form method="post" action="/meni/delete/' + item.id + '" class="d-inline-block" '
                                    + 'onsubmit="return confirm(\'Jeste li sigurni da želite obrisati stavku menija?\');">'

                                    + antiForgeryInput()

                                    + '<button type="submit" class="btn btn-danger">'
                                    + 'Delete</button>'

                                + '</form>' : '')

                            + '</div>'
                        + '</div>';
                    case 'order':
                        return '<div class="order-card entity-card">'
                            + '<div class="order-card-heading"><div><span class="card-kicker">Order</span><h3>Narudžba #' + escapeHtml(item.id) + '</h3></div><span class="status-badge">' + escapeHtml(item.status || '-') + '</span></div>'
                            + '<div class="card-stat-row"><span>Kupac</span><strong>' + escapeHtml(item.customerName || '-') + '</strong></div>'
                            + '<div class="card-stat-row"><span>Restoran</span><strong>' + escapeHtml(item.restaurantName || '-') + '</strong></div>'
                            + '<div class="card-stat-row"><span>Cijena</span><strong class="price-emphasis">' + escapeHtml(item.totalPrice ? item.totalPrice + ' EUR' : '-') + '</strong></div>'
                            + '<div class="card-stat-row"><span>Datum</span><strong>' + escapeHtml(item.orderDateText || '-') + '</strong></div>'
                            + '<div class="btn-group-inline card-actions">'
                            + '<a class="btn btn-secondary" href="/narudzbe/' + item.id + '">View</a>'
                            + (canManage ? '<a class="btn btn-outline" href="/narudzbe/edit/' + item.id + '">Edit</a>' : '')
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

        function antiForgeryInput(){
            var token = $('input[name="__RequestVerificationToken"]').first().val() || '';
            return '<input type="hidden" name="__RequestVerificationToken" value="' + escapeHtml(token) + '" />';
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
                    $container.removeClass('has-search-results');
                    $container.html(originalMarkup);
                    return;
                }
                timerId = setTimeout(function(){ loadResults(query); }, 250);
            });
        });
    });
})(jQuery);
