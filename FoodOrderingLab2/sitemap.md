# Sitemap - FoodOrderingLab2

## Routing overview
Aplikacija koristi mješavinu atributnog routanja i standardne default rute. Default ruta u `Program.cs` je:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

To znači da je URL `/` ekvivalentan `HomeController.Index()`.

## Dostupni URL-ovi

| URL | Controller | Akcija | View |
| --- | --- | --- | --- |
| `/` | `HomeController` | `Index()` | `Views/Home/Index.cshtml` |
| `/Home/Index` | `HomeController` | `Index()` | `Views/Home/Index.cshtml` |
| `/restorani` | `RestaurantController` | `Index()` | `Views/Restaurant/Index.cshtml` |
| `/restorani/{id:int}` | `RestaurantController` | `Details(int id)` | `Views/Restaurant/Details.cshtml` |
| `/meni` | `MenuItemController` | `Index(int restaurantId = 0)` | `Views/MenuItem/Index.cshtml` |
| `/meni/restoran/{restaurantId:int}` | `MenuItemController` | `Index(int restaurantId)` | `Views/MenuItem/Index.cshtml` |
| `/meni/{id:int}` | `MenuItemController` | `Details(int id)` | `Views/MenuItem/Details.cshtml` |
| `/narudzbe` | `OrderController` | `Index()` | `Views/Order/Index.cshtml` |
| `/narudzbe/{id:int}` | `OrderController` | `Details(int id)` | `Views/Order/Details.cshtml` |
| `/kupci` | `CustomerController` | `Index()` | `Views/Customer/Index.cshtml` |
| `/kupci/{id:int}` | `CustomerController` | `Details(int id)` | `Views/Customer/Details.cshtml` |

## Atributno routing

- `RestaurantController` koristi `[Route("restorani")]` na controlleru.
- `MenuItemController` koristi `[Route("meni")]` i dodatne rute:
  - `[Route("")]`
  - `[Route("restoran/{restaurantId:int}")]`
  - `[Route("{id:int}")]`
- `OrderController` koristi `[Route("narudzbe")]` s `Index` i `Details` rutama.
- `CustomerController` koristi `[Route("kupci")]` s `Index` i `Details` rutama.

## Napomena
`HomeController` još uvijek koristi default routing; akcija `Index()` je dostupna kroz `/` i `/Home/Index`.
