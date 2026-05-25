# Playbook — Price List API

Step-by-step guide to get the API running locally, hit the endpoints, and
verify it works end-to-end. If you've never run a .NET project before,
this is the file to follow.

## 1. Prerequisites

Install the following once per machine. Pinned versions are what the
project was developed against — newer minor versions also work.

| Tool | Version | Where to get it |
|---|---|---|
| .NET SDK            | 8.0.x  | https://dotnet.microsoft.com/download/dotnet/8.0  |
| SQL Server LocalDB  | 2019+  | Installs with Visual Studio, or as a standalone   |
| Visual Studio       | 2022+  | (optional) https://visualstudio.microsoft.com/    |
| Git                 | any    | https://git-scm.com/downloads                     |

Verify .NET is installed:

```bash
dotnet --list-sdks
# should print at least one line starting with "8."
```

Verify LocalDB is installed (Windows):

```powershell
sqllocaldb info
# should print "MSSQLLocalDB" (and maybe others)
```

## 2. Clone the repo

```bash
git clone https://github.com/salmaamr129/price-list-api.git
cd price-list-api
```

## 3. Create the database

Connect to your local SQL Server / LocalDB with **SQL Server Management
Studio (SSMS)**, **Azure Data Studio**, or `sqlcmd`. Run:

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

-- Seed one row so GET /api/seller returns something useful immediately
INSERT INTO Sellers (SellerID, SellerName, City) VALUES (1, 'Acme Corp', 'Cairo');
INSERT INTO Products (ProductID, ProductName, Price, Quantity, SellerID)
VALUES (1, 'Widget', 9.99, 100, 1);
```

The default connection string assumes `(localdb)\MSSQLLocalDB`. If you
use a different SQL Server instance, edit
`appsettings.json` → `ConnectionStrings.DefaultConnection`, or set the
environment variable `ConnectionStrings__DefaultConnection`.

## 4. Restore dependencies and run

```bash
dotnet restore
dotnet run --launch-profile https
```

You should see output ending in:

```
Now listening on: https://localhost:7147
Now listening on: http://localhost:5295
Application started.
```

## 5. Verify it works

Open **https://localhost:7147/swagger** in a browser. Swagger UI loads
the auto-generated API docs for `Product` and `Seller`. Try each:

- **`GET /api/seller`** → expects `200 OK` with `[{"sellerID":1,"sellerName":"Acme Corp","city":"Cairo"}]`
- **`GET /api/product`** → expects `200 OK` with the seeded `Widget` row
- **`POST /api/product`** → click *Try it out*, paste:
  ```json
  { "productID": 2, "productName": "Gadget", "price": 19.99, "quantity": 50, "sellerID": 1 }
  ```
  Send. Expect `200 OK` with `"Product added successfully!"`
- **`GET /api/product/2`** → expects to see the row you just added
- **`DELETE /api/product/2`** → cleans up

If any of these fail with `500`, check the terminal where `dotnet run` is
running — exceptions are logged there.

## 6. Alternative: run via the `.http` file

If you use VS / VS Code with the **REST Client** extension, just open
`PriceListApi.http`, click *Send request* above any block. The file
has prepared GET/POST requests for both resources.

## 7. Alternative: run in Docker

```bash
docker build -t pricelist-api .
docker run --rm -p 8080:8080 -p 8081:8081 \
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=PriceListApi;User Id=sa;Password=YourPwd;TrustServerCertificate=True;" \
  pricelist-api
```

`host.docker.internal` is how a container reaches a SQL Server running on
the host machine on macOS / Windows. On Linux, use the host's IP address
or pass `--add-host=host.docker.internal:host-gateway`.

## 8. Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| `Cannot open server 'SALMA'` | Old connection string from a previous version | Pull latest `main`; verify `appsettings.json` says `(localdb)\MSSQLLocalDB` |
| `Login failed for user '...'` | LocalDB isn't running, or the DB doesn't exist | Run `sqllocaldb start MSSQLLocalDB` and re-create the schema (step 3) |
| `SSL connection error` on connect | TrustServerCertificate not set | Already enabled in the default connection string; if you changed it, add `TrustServerCertificate=True;` back |
| Swagger UI 404 | App is running but not at the expected port | Look at the actual port in the terminal output ("Now listening on: …") |
| `dotnet` not found | .NET SDK not in PATH | Reinstall .NET 8 SDK and reopen your terminal |

## 9. Stop the app

`Ctrl+C` in the terminal running `dotnet run`. Docker users: `Ctrl+C` or
`docker stop <container_id>`.
