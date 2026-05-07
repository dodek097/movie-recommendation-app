# Semantic Model - FoodOrderingLab2

## Pregled modela
Aplikacija koristi Entity Framework Core s `ApplicationDbContext` i SQL Server vezom. Model reprezentira narudžbe hrane s entitetima: `Customer`, `Restaurant`, `MenuItem`, `Order` i `OrderItem`.

## Entiteti i tablice

### Customer
- `CustomerId` (PK, int, ValueGeneratedNever)
- `FirstName` (string)
- `LastName` (string)
- `Email` (string)
- `Phone` (string)
- `Address` (string)
- `RegisterDate` (DateTime)
- `Orders` (ICollection<Order>)

Veza:
- 1 customer može imati više `Orders` (1:n)

### Restaurant
- `RestaurantId` (PK, int, ValueGeneratedNever)
- `Name` (string)
- `Address` (string)
- `Phone` (string)
- `Email` (string)
- `Rating` (decimal(3,2))
- `OpeningHours` (string)
- `MenuItems` (ICollection<MenuItem>)
- `Orders` (ICollection<Order>)

Veze:
- 1 restaurant može imati više `MenuItems` (1:n)
- 1 restaurant može imati više `Orders` (1:n)

### MenuItem
- `MenuItemId` (PK, int, ValueGeneratedNever)
- `Name` (string)
- `Description` (string)
- `Price` (decimal(18,2))
- `Category` (enum `FoodCategory`)
- `Calories` (int)
- `IsAvailable` (bool)
- `RestaurantId` (FK, int)
- `Restaurant` (virtual Restaurant)
- `OrderItems` (ICollection<OrderItem>)

Veze:
- svaki `MenuItem` pripada jednom `Restaurant` (n:1)
- jedan `MenuItem` može se pojaviti u više `OrderItems` (1:n)

### Order
- `OrderId` (PK, int, ValueGeneratedNever)
- `CustomerId` (FK, int)
- `RestaurantId` (FK, int)
- `OrderDate` (DateTime)
- `TotalPrice` (decimal(18,2))
- `Status` (enum `OrderStatus`)
- `Customer` (virtual Customer)
- `Restaurant` (virtual Restaurant)
- `OrderItems` (ICollection<OrderItem>)

Veze:
- svaki `Order` pripada jednom `Customer` (n:1)
- svaki `Order` pripada jednom `Restaurant` (n:1)
- jedan `Order` može imati više `OrderItems` (1:n)

### OrderItem
- `OrderItemId` (PK, int, ValueGeneratedNever)
- `OrderId` (FK, int)
- `MenuItemId` (FK, int)
- `Quantity` (int)
- `UnitPrice` (decimal(18,2))
- `SpecialRequests` (string?)
- `Order` (virtual Order)
- `MenuItem` (virtual MenuItem)

Veze:
- svaki `OrderItem` pripada jednom `Order` (n:1)
- svaki `OrderItem` pripada jednom `MenuItem` (n:1)

## Posebnosti implementacije
- Svi ID-jevi su konfigurirani s `ValueGeneratedNever` kako bi se koristile unaprijed definirane vrijednosti u seed podacima.
- Decimalna polja imaju eksplicitnu preciznost: `Price`, `TotalPrice`, `UnitPrice` 18,2 i `Rating` 3,2.
- `OrderItem` FK veze koriste `DeleteBehavior.Restrict` kako bi se izbjegli problemi s višestrukim kaskadnim putanjama.
- `ApplicationDbContext` definira `DbSet<T>` za sve entitete: `Customers`, `Restaurants`, `MenuItems`, `Orders`, `OrderItems`.

## View modeli

### RestaurantDetailViewModel
- `Restaurant` (Restaurant)
- `MenuItems` (List<MenuItem>)

### MenuItemDetailViewModel
- `MenuItem` (MenuItem)
- `Restaurant` (Restaurant)

### OrderDetailViewModel
- `Order` (Order)
- `Customer` (Customer)
- `Restaurant` (Restaurant)
- `OrderItems` (List<OrderItem>)
