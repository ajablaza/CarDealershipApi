# Car Dealership API

A REST API for car dealerships to manage their vehicle stock. Dealership users sign in, then list, search, add, update and remove the cars their dealership holds, including each car's stock level. Every dealership only ever sees and changes its own cars.

## Tech stack

| Technology | Used for |
| --- | --- |
| .NET 10 / ASP.NET Core | Web API host |
| [FastEndpoints](https://fast-endpoints.com) | Endpoints, following the REPR pattern (see below) |
| FastEndpoints.Security | JWT creation and validation |
| FastEndpoints.Swagger | Swagger UI for exploring the API |
| FluentValidation | Request validation (bundled with FastEndpoints) |
| Dapper | Maps hand-written SQL queries to C# objects |
| SQLite (Microsoft.Data.Sqlite) | File-based database, no server to install |
| BCrypt.Net-Next | Password hashing |

---

## Running the API

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Optional: Visual Studio 2022+, Postman, or [DB Browser for SQLite](https://sqlitebrowser.org) to inspect the database

### Start it

**Visual Studio:** open the solution and press **F5**.

**Command line:**

```bash
cd CarDealershipApi
dotnet run
```

The console shows the URLs, for example `http://localhost:5272`. Swagger UI is at `/swagger`, and `GET /health` is a quick check that the API is up.

On startup the API:

1. **Creates the database.** `dealership.db` is created in the project folder, and the tables are created if they don't exist (`Data/DatabaseInitializer.cs`).
2. **Seeds test data** when running in the Development environment (the default in Visual Studio and `dotnet run`).

### Seeded test data (Development only)

So the API can be tested straight away, without registering users or creating dealerships by hand, `Data/DatabaseSeeder.cs` fills an empty database with two dealerships, four users and some stock. It only runs in Development and skips itself if any dealership already exists, so it never overwrites data.

**Every seeded account uses the password `Password123!`**

| Email | Dealership | Role | Useful for testing |
| --- | --- | --- | --- |
| `admin@sydneymotors.test` | Sydney Motors | Admin | Everything, including deleting cars and adding staff |
| `staff@sydneymotors.test` | Sydney Motors | Staff | Admin-only routes return 403 |
| `admin@melbourneautos.test` | Melbourne Autos | Admin | Can't see Sydney's cars (404) |
| `nodealership@test.test` | none | Staff | Car routes return 403; can create a dealership or be added as staff |

Sydney Motors has four stock lines (one with stock 0) plus one soft-deleted car that never appears in results. Melbourne Autos has two stock lines.

**To reset:** stop the app, delete `dealership.db`, and start it again.
___

### Testing with Postman

A Postman collection covering every route is included at the repository root: `cardealershipapi.postman_collection.json`.

1. In Postman, click **Import** and select the file.
2. Open the collection's **Variables** tab and check `baseUrl` matches the URL the API printed on startup (e.g. `http://localhost:5272`).
3. Run **Sign in** first. A script on that request saves the returned token to the `token` collection variable, and every other request sends it automatically as a Bearer token. **Create dealership** does the same, so the new token is picked up after creating one.

To test as a different user, change the email in **Sign in** to one of the seeded accounts above and send it again.
___
### Configuration

`appsettings.json` holds the connection string and the JWT settings. The signing key there is a development placeholder so the project runs with no setup. In a real deployment it would come from user secrets or environment variables, never source control.

---

## Authentication

1. `POST /auth/signin` with an email and password returns a JWT.
2. Send it on every other request: `Authorization: Bearer <token>`.
3. The token carries the user's `UserId`, `role` and, once they belong to one, their `DealershipId`. Car routes read the dealership from the token, never from the URL or body, so a user can't access another dealership's stock.

A new user signs up without a dealership. They either create one (and become its Admin) or are added to an existing one by that dealership's Admin. Both change what the token should contain, so the user then needs a new token: create-dealership returns one, and added staff sign in again.

---

## Routes

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| GET | `/health` | Public | Health check |
| POST | `/auth/signup` | Public | Register a new user (no dealership yet, role Staff) |
| POST | `/auth/signin` | Public | Sign in and receive a JWT |
| GET | `/auth/me` | Signed in | The current user's details |
| POST | `/dealerships` | Signed in, no dealership | Create a dealership; the creator becomes its Admin and gets a new token |
| POST | `/dealerships/staff` | Admin | Add an existing user (by email) to the Admin's dealership as Staff |
| GET | `/cars?make=&model=` | Dealership member | List the dealership's cars and stock levels; optional `make`/`model` filters are case-insensitive "contains" searches |
| GET | `/cars/{id}` | Dealership member | Get a single car |
| POST | `/cars` | Dealership member | Add a car to the dealership's stock |
| PATCH | `/cars/{id}` | Dealership member | Update any car details, including stock; only the fields sent are changed |
| DELETE | `/cars/{id}` | Admin | Soft-delete a car |

### Status codes

| Code | Meaning |
| --- | --- |
| 200 / 201 / 204 | Success / created / deleted |
| 400 | Validation failed; the response lists which fields and why |
| 401 | Missing or invalid token, or wrong email/password |
| 403 | Signed in but not allowed (no dealership, or not an Admin) |
| 404 | Not found, **including another dealership's car**, so the API doesn't reveal that it exists |
| 409 | Conflict: email/username taken, or user already in a dealership |

---

## Entities

### Users

A person who signs in. A user belongs to at most one dealership.

| Column | Type | Description |
| --- | --- | --- |
| `Id` | INTEGER | Primary key |
| `DealershipId` | INTEGER, nullable | The dealership the user belongs to; `NULL` until they create or join one |
| `Username` | TEXT, unique | Display/login name |
| `Email` | TEXT, unique | Used to sign in; uniqueness and matching ignore capitalisation |
| `PasswordHash` | TEXT | BCrypt hash of the password; the plain password is never stored or returned |
| `Role` | TEXT | `Admin` (can delete cars and add staff) or `Staff` |
| `CreatedAt` | TEXT (ISO 8601) | When the account was created (UTC) |

### Dealerships

A business whose stock is managed through the API.

| Column | Type | Description |
| --- | --- | --- |
| `Id` | INTEGER | Primary key |
| `Name` | TEXT | Dealership name |
| `CreatedAt` | TEXT (ISO 8601) | When the dealership was created (UTC) |

### Cars

Each row is a **stock line**: identical new vehicles of the same make, model, year and colour. `Stock` is how many units the dealership has.

| Column | Type | Description |
| --- | --- | --- |
| `Id` | INTEGER | Primary key |
| `DealershipId` | INTEGER | The dealership that owns this stock line |
| `Make` | TEXT | Manufacturer, e.g. Toyota |
| `Model` | TEXT | Model name, e.g. Corolla |
| `Year` | INTEGER | Model year |
| `Color` | TEXT | Exterior colour |
| `Price` | REAL | Price per unit |
| `Stock` | INTEGER | Units on hand; `0` means none currently available |
| `CreatedAt` | TEXT (ISO 8601) | When the line was added (UTC) |
| `UpdatedAt` | TEXT (ISO 8601) | When it was last changed (UTC) |
| `DeletedAt` | TEXT, nullable | Set when the car is deleted; `NULL` means active |

**Soft delete:** deleting a car sets `DeletedAt` instead of removing the row, so history isn't lost. Every query filters on `DeletedAt IS NULL`, so deleted cars never appear in the API.

SQLite has no date or decimal types, so dates are stored as ISO 8601 text and prices as `REAL`.

---

## Code structure: REPR

The API follows the **REPR pattern** (Request–Endpoint–Response), which FastEndpoints is built around. Instead of controllers grouping many routes, **each route is its own class with its own folder**, containing only what that route needs:

| File | Role |
| --- | --- |
| `Request.cs` | What the client sends: route values, query string and/or JSON body |
| `Validator.cs` | Rules the request must pass; failures return 400 before the endpoint runs |
| `Endpoint.cs` | Declares the route and access rules (`Configure`) and handles the request (`HandleAsync`) |
| `Response.cs` | What the endpoint returns |

A folder only contains the files its route needs. For example, `Me` has no `Request.cs` because everything comes from the token, and `DeleteCar` has no response because it returns 204 with an empty body. Endpoints that return the same shape share a single response class (`CarResponse`, `UserResponse`) rather than keeping identical copies.

```
CarDealershipApi/
├── Program.cs                    Service registration, JWT, FastEndpoints, Swagger, DB init + seed
├── appsettings.json              Connection string and JWT settings
├── Domain/                       Entities that mirror the database tables
│   ├── User.cs
│   ├── Dealership.cs
│   ├── Car.cs
│   └── Roles.cs                  "Admin" / "Staff" constants
├── Data/
│   ├── DbConnectionFactory.cs    Creates SQLite connections
│   ├── DatabaseInitializer.cs    CREATE TABLE scripts, run at startup
│   ├── DatabaseSeeder.cs         Development test data
│   └── Repositories/             All SQL lives here (Dapper)
│       ├── UserRepository.cs
│       ├── DealershipRepository.cs
│       └── CarRepository.cs
├── Common/
│   ├── TokenService.cs           Builds JWTs
│   └── ClaimsPrincipalExtensions.cs   Reads UserId / DealershipId from the token
└── Features/
    ├── Health/                   Endpoint
    ├── Auth/
    │   ├── UserResponse.cs       Shared by SignUp, Me and AddStaff
    │   ├── SignUp/               Request, Validator, Endpoint
    │   ├── SignIn/               Request, Response, Validator, Endpoint
    │   └── Me/                   Endpoint
    ├── Dealerships/
    │   ├── CreateDealership/     Request, Response, Validator, Endpoint
    │   └── AddStaff/             Request, Validator, Endpoint
    └── Cars/
        ├── CarResponse.cs        Shared by every car endpoint that returns a car
        ├── ListCars/             Request, Endpoint
        ├── GetCar/               Request, Endpoint
        ├── CreateCar/            Request, Validator, Endpoint
        ├── UpdateCar/            Request, Validator, Endpoint
        ├── UpdateStock/          Request, Validator, Endpoint
        └── DeleteCar/            Request, Endpoint
```

### How a request flows

Taking `POST /cars` as an example:

1. FastEndpoints binds the JSON body to `CreateCar/Request.cs`.
2. `CreateCar/Validator.cs` checks it (make and model required, price > 0, stock ≥ 0, ...). If it fails, a 400 is returned and nothing else runs.
3. `Configure()` in `CreateCar/Endpoint.cs` requires a token with a `DealershipId` claim, otherwise 401 or 403.
4. `HandleAsync` reads the dealership from the token and calls `CarRepository.CreateAsync`, which runs a parameterised SQL `INSERT` through Dapper.
5. The saved car is returned as a `CarResponse` with status 201.

Endpoints hold no SQL. They handle HTTP concerns and delegate data access to the repositories, which are injected through ASP.NET Core dependency injection behind interfaces (`ICarRepository` etc.). All SQL uses parameters (`@Make`, `@Id`, ...), never string concatenation, to prevent SQL injection.

---