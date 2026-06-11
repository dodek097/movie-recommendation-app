# Lab 5 - predajna checklista

## Pokretanje

SQL Server Docker container mora biti dostupan na `localhost:1433`. Aplikacija koristi novu bazu
`FoodOrderingLab5Db`, automatski primjenjuje migraciju i seeda domenske podatke, role i demo korisnike.

```powershell
docker start sqlserver
dotnet run
```

Demo računi:

| Rola | Email | Lozinka |
| --- | --- | --- |
| Admin | `admin@foodorder.local` | `Admin123!` |
| Manager | `manager@foodorder.local` | `Manager123!` |

Google provider koristi OAuth vjerodajnice spremljene kroz User Secrets:

```powershell
dotnet user-secrets set "Authentication:Google:ClientId" "<google-client-id>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<google-client-secret>"
```

Google Web application mora imati autorizirani redirect URI
`https://localhost:7096/signin-google` za lokalni HTTPS profil.

## API

CRUD + DTO API rute:

- `/api/customers`
- `/api/restaurants`
- `/api/menu-items`
- `/api/orders`
- `/api/orders/{orderId}/items`
- `/api/restaurant-attachments`

Liste podržavaju pretragu i smislene query filtre. API koristi ugniježđene DTO modele i statuse
`200`, `201`, `204`, `400`, `401`, `403`, `404` i `409`.

Autorizacija:

- liste i pretrage: javno
- detalji: prijavljeni korisnik
- create/edit: `Admin` ili `Manager`
- delete: `Admin`

## Upload i autentikacija

- Restoran ima privitke spremljene na disk i metapodatke u bazi.
- Edit restorana sadrži asinkroni HTML5 drag-and-drop upload.
- Popis i brisanje datoteka rade AJAX pozivima.
- Upload validira veličinu i ekstenziju te koristi anti-forgery zaštitu.
- ASP.NET Core Identity podržava lokalnu registraciju/prijavu i role.
- `AppUser` sadrži validirana polja `OIB` i `JMBG`.
- Prilagođene Register i ExternalLogin forme spremaju `OIB` i `JMBG`.
- Google authentication provider je konfiguriran bez hardkodiranih tajni.

## Customer self-service proširenje

- Lokalna i vanjska registracija automatski stvaraju povezani `Customer` zapis.
- Obični prijavljeni korisnik može kreirati narudžbu samo za vlastiti `Customer` profil.
- Obični korisnik vidi samo vlastite narudžbe i ne može otvoriti tuđe detalje.
- `Admin` i `Manager` zadržavaju pregled i upravljanje svim narudžbama.
- Upravljački gumbi za kupce, restorane i jela prikazuju se samo dopuštenim rolama.
- Lab5 API autorizacijska pravila ostaju nepromijenjena.
- Prijavljeni korisnik preko `/kupci/moj-profil` uređuje samo vlastito ime, prezime, telefon i adresu.
- Self-service profil ne prima `CustomerId`, pa nije moguće odabrati ili izmijeniti tuđi profil.
- Identity račun bez Customer zapisa može sigurno obnoviti vlastiti profil.

## Testovi

```powershell
dotnet build FoodOrderingLab2.sln --no-restore
dotnet test FoodOrderingLab2.sln --no-restore
```

Integracijski testovi koriste `WebApplicationFactory`, pravi HTTP sloj, testnu autentikaciju i
EF Core InMemory bazu. Pokrivaju CRUD, pretragu, validaciju, nepostojeće ID-eve i autorizaciju
za svih šest API površina. Dodatni test prolazi MVC upload, AJAX popis i brisanje privitka s
anti-forgery zaštitom.
