# Price List API

A RESTful Web API for managing **products** and **sellers**, built with
ASP.NET Core 8 and SQL Server. Implements full CRUD over both resources
using ADO.NET (`Microsoft.Data.SqlClient`) for direct query control,
with Entity Framework Core wired in for migrations and the data model.

## Tech stack

- **.NET 8** / C#
- **ASP.NET Core** Web API
- **SQL Server** (LocalDB or full instance)
- **Microsoft.Data.SqlClient** — direct ADO.NET access in controllers
- **Entity Framework Core 9** — `DbContext` and DI registration
- **Swashbuckle (Swagger / OpenAPI)** — auto-generated API docs at `/swagger`
- **Docker** — `Dockerfile` for containerized runs

## Project layout

```
PriceListApi.sln              Solution file
PriceListApi.csproj           Project file (.NET 8, RootNamespace=PriceListApi)
PriceListApi.http             Sample HTTP requests for VS / REST Client
Program.cs                    App startup, DI, middleware pipeline, Swagger
appsettings.json              ConnectionStrings.DefaultConnection
Dockerfile                    Multi-stage build for containerized deployment
Controllers/
  ProductController.cs        GET/POST/PUT/DELETE /api/product
  SellerController.cs         GET/POST/PUT/DELETE /api/seller
data/
  AppDbContext.cs             EF Core DbContext (Products, Sellers DbSets)
  models/
    Product.cs                ProductID, ProductName, Price, Quantity, SellerID
    Seller.cs                 SellerID, SellerName, City
```

## API endpoints

### Products — `/api/product`

| Method | Route | Body | Returns |
|--------|-------|------|---------|
| `GET`    | `/api/product`       | —              | `Product[]` |
| `GET`    | `/api/product/{id}`  | —              | `Product` or `404` |
| `POST`   | `/api/product`       | `Product` JSON | `200 OK` |
| `PUT`    | `/api/product/{id}`  | `Product` JSON | `200 OK` or `404` |
| `DELETE` | `/api/product/{id}`  | —              | `200 OK` or `404` |

### Sellers — `/api/seller`

| Method | Route | Body | Returns |
|--------|-------|------|---------|
| `GET`    | `/api/seller`        | —             | `Seller[]` |
| `GET`    | `/api/seller/{id}`   | —             | `Seller` or `404` |
| `POST`   | `/api/seller`        | `Seller` JSON | `200 OK` |
| `PUT`    | `/api/seller/{id}`   | `Seller` JSON | `200 OK` or `404` |
| `DELETE` | `/api/seller/{id}`   | —             | `200 OK` or `404` |

Browse the live, auto-generated API docs at `http://localhost:5295/swagger`
once the app is running.

## Data model

```
Seller                       Product
------                       -------
SellerID    int (PK)         ProductID    int (PK)
SellerName  nvarchar         ProductName  nvarchar
City        nvarchar         Price        decimal
                             Quantity     int
                             SellerID     int  -> Seller.SellerID (FK)
```

## Running locally

> For a guided, copy-pasteable walkthrough — prerequisites, DB setup,
> verification, and troubleshooting — see **[PLAYBOOK.md](PLAYBOOK.md)**.

### Prerequisites
- .NET 8 SDK — https://dotnet.microsoft.com/download
- SQL Server LocalDB (ships with Visual Studio) **or** SQL Server 2019+

### Database setup
Create the database and tables (suggested DDL):

```sql
CREATE DATABASE PriceListApi;
GO
USE PriceListApi;
GO

CREATE TABLE Sellers (
    SellerID    INT          PRIMARY KEY,
    SellerName  NVARCHAR(100) NOT NULL,
    City        NVARCHAR(100) NOT NULL
);

CREATE TABLE Products (
    ProductID    INT          PRIMARY KEY,
    ProductName  NVARCHAR(100) NOT NULL,
    Price        DECIMAL(10,2) NOT NULL,
    Quantity     INT           NOT NULL,
    SellerID     INT           NOT NULL FOREIGN KEY REFERENCES Sellers(SellerID)
);
```

The default connection string in `appsettings.json` points at
`Server=(localdb)\\MSSQLLocalDB;Database=PriceListApi`. Override via the
`ConnectionStrings__DefaultConnection` environment variable if you use a
different server.

### Run the app

```bash
dotnet restore
dotnet run --launch-profile https
```

Open https://localhost:7147/swagger to interact with the API.

### Run with Docker

```bash
docker build -t pricelist-api .
docker run --rm -p 8080:8080 -p 8081:8081 \
  -e "ConnectionStrings__DefaultConnection=<your_conn_string>" \
  pricelist-api
```

## Manual testing

`PriceListApi.http` contains ready-to-fire GET/POST examples for both
resources. Open the file in Visual Studio, VS Code (with the
*REST Client* extension), or JetBrains Rider and click "Send request"
above each block.

## License

[MIT](LICENSE)
