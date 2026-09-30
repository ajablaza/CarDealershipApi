using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CarDealershipApi.Data;

// Development-only test data, so you can sign in straight away without
// going through sign-up / create-dealership / add-staff by hand.
//
// Runs on startup after the tables are created. Does nothing if any
// dealership already exists, so it's safe to run every time.
// To reset: stop the app, delete the .db file, start again.
//
// Every seeded account uses the same password: Password123!
public static class DatabaseSeeder
{
    public const string SeedPassword = "Password123!";

    public static void Seed(string connectionString)
    {
        using var conn = new SqliteConnection(connectionString);
        conn.Open();

        var alreadySeeded = conn.ExecuteScalar<long>("SELECT COUNT(*) FROM Dealerships;") > 0;
        if (alreadySeeded)
        {
            return;
        }

        using var tx = conn.BeginTransaction();
        var now = DateTime.UtcNow.ToString("O");

        // --- Dealerships -------------------------------------------------
        var sydney = InsertDealership(conn, tx, "Sydney Motors", now);
        var melbourne = InsertDealership(conn, tx, "Melbourne Autos", now);

        // --- Users -------------------------------------------------------
        // Sydney Motors: one Admin, one Staff
        InsertUser(conn, tx, sydney, "sydney.admin", "admin@sydneymotors.test", "Admin", now);
        InsertUser(conn, tx, sydney, "sydney.staff", "staff@sydneymotors.test", "Staff", now);

        // Melbourne Autos: one Admin, to check dealerships can't see each other's cars
        InsertUser(conn, tx, melbourne, "melbourne.admin", "admin@melbourneautos.test", "Admin", now);

        // No dealership yet: to test the 403 on car routes and POST /dealerships
        InsertUser(conn, tx, null, "no.dealership", "nodealership@test.test", "Staff", now);

        // --- Cars (each row is a stock line of identical new vehicles) ----
        //                          make       model      year  color     price    stock
        InsertCar(conn, tx, sydney, "Toyota", "Corolla", 2025, "White", 32990m, 4, now);
        InsertCar(conn, tx, sydney, "Toyota", "Camry", 2025, "Silver", 41990m, 2, now);
        InsertCar(conn, tx, sydney, "Mazda", "CX-5", 2025, "Red", 38990m, 1, now);
        InsertCar(conn, tx, sydney, "Ford", "Ranger", 2025, "Black", 56990m, 0, now); // out of stock

        // Soft-deleted: should NOT appear in GET /cars, but the row stays in the table
        InsertCar(conn, tx, sydney, "Honda", "Civic", 2025, "Blue", 36990m, 3, now, deletedAt: now);

        InsertCar(conn, tx, melbourne, "Hyundai", "i30", 2025, "Grey", 29990m, 5, now);
        InsertCar(conn, tx, melbourne, "Kia", "Sportage", 2025, "White", 42990m, 2, now);

        tx.Commit();
    }

    private static long InsertDealership(IDbConnection conn, IDbTransaction tx, string name, string now) =>
        conn.ExecuteScalar<long>("""
            INSERT INTO Dealerships (Name, CreatedAt)
            VALUES (@Name, @CreatedAt)
            RETURNING Id;
            """,
            new { Name = name, CreatedAt = now },
            tx);

    private static void InsertUser(
        IDbConnection conn, IDbTransaction tx,
        long? dealershipId, string username, string email, string role, string now) =>
        conn.Execute("""
            INSERT INTO Users (DealershipId, Username, Email, PasswordHash, Role, CreatedAt)
            VALUES (@DealershipId, @Username, @Email, @PasswordHash, @Role, @CreatedAt);
            """,
            new
            {
                DealershipId = dealershipId,
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(SeedPassword),
                Role = role,
                CreatedAt = now
            },
            tx);

    private static void InsertCar(
        IDbConnection conn, IDbTransaction tx,
        long dealershipId, string make, string model, int year, string color,
        decimal price, int stock, string now, string? deletedAt = null) =>
        conn.Execute("""
            INSERT INTO Cars (DealershipId, Make, Model, Year, Color, Price, Stock, CreatedAt, UpdatedAt, DeletedAt)
            VALUES (@DealershipId, @Make, @Model, @Year, @Color, @Price, @Stock, @Now, @Now, @DeletedAt);
            """,
            new
            {
                DealershipId = dealershipId,
                Make = make,
                Model = model,
                Year = year,
                Color = color,
                Price = price,
                Stock = stock,
                Now = now,
                DeletedAt = deletedAt
            },
            tx);
}