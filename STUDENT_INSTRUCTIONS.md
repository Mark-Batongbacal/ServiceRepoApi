# Lab: Fix the Service–Repository Web API

This ASP.NET Core Web API is supposed to follow the **Service–Repository pattern**,
but it has been broken. Your job is to find and fix every bug.

There are **29 bugs**. The project **compiles**, so the compiler won't find them for you.
Every bug is a runtime/logic bug, a wrong HTTP contract (route, verb, status code), or a
**design violation**: code sitting in the wrong layer. Some bugs only show up after you
fix another one, so re-test after every change.

## Do NOT change these (they are correct)

- `Models/Product.cs`
- `Data/AppDbContext.cs`
- `Program.cs`
- `appsettings.json` (except your own connection string)
- `ServiceRepoApi.csproj`
- `ServiceRepoApi.http` (the test requests; see below)

## Setup

1. Set `ConnectionStrings:DefaultConnection` in `appsettings.json`.
2. Run:

       dotnet ef migrations add InitialCreate
       dotnet ef database update
       dotnet run

3. Test with Swagger (`https://localhost:7081/swagger`) or with the requests in
   `ServiceRepoApi.http`. Each request is labelled with the result it should return
   once the API is fixed. Run them top to bottom on a fresh database.

## Architecture rules

    Controller  ->  IProductService  ->  IProductRepository  ->  AppDbContext

- **Controllers** depend only on `IProductService`. They must not use `AppDbContext` or
  contain business rules. They handle HTTP only: routes, verbs, binding, status codes.
- **Services** contain all business rules and report failures with `ServiceResult`.
- **Repositories** contain data access only (EF Core queries, add/update/remove, save).

## Required API contract

Base route: `/api/products`. Error responses have a JSON body `{ "error": "..." }`
(automatic validation errors use the standard ProblemDetails format).

| Request | Success | Failure |
|---|---|---|
| `GET /api/products` | 200, **all** products sorted by name A–Z | |
| `GET /api/products/{id}` | 200, the product | 404 if it doesn't exist |
| `POST /api/products` | 201 with a `Location` header and the created product | 400 invalid body, 409 duplicate name |
| `PUT /api/products/{id}` | 204 | 400 invalid body or route id ≠ body id, 404 not found, 409 duplicate name |
| `DELETE /api/products/{id}` | 204 | 404 not found, 409 if stock > 0 |

## Business rules

1. Leading and trailing spaces are removed from names on create **and** update.
2. Names must be **unique** (after trimming) on create **and** update.
3. The server assigns `Id`. An `id` sent in a POST body is ignored.
4. `CreatedAt` is set once, by the **service**, in **UTC**. A `createdAt` sent by the
   client (POST or PUT) is ignored, and updates never change it.
5. PUT saves every editable field: name, description, price, stock.
6. A product can only be deleted when its stock is exactly **0**.
7. Unexpected exceptions (500) must never be the answer to a normal request,
   such as asking for an id that doesn't exist.

## Tips

- Watch the status code, the headers and the body of every response, not just whether it "worked".
- Check the database directly (e.g. SSMS) to see what was really saved.
- Read the app's console log when you get a 500.
- For each bug you fix, write down the file, what was wrong, the symptom, and your fix.
