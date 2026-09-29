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

        // --- Cars --------------------------------------------------------
        InsertCar(conn, tx, sydney, "Toyota", "Corolla", 2020, "White", 22990m, 45000, "Available", now);
        InsertCar(conn, tx, sydney, "Toyota", "Camry", 2021, "Silver", 31500m, 30000, "Available", now);
        InsertCar(conn, tx, sydney, "Mazda", "CX-5", 2019, "Red", 27990m, 62000, "Available", now);
        InsertCar(conn, tx, sydney, "Ford", "Ranger", 2022, "Black", 48990m, 15000, "Sold", now);

        // Soft-deleted: should NOT appear in GET /cars, but the row stays in the table
        InsertCar(conn, tx, sydney, "Honda", "Civic", 2018, "Blue", 18990m, 80000, "Available", now, deletedAt: now);

        InsertCar(conn, tx, melbourne, "Hyundai", "i30", 2021, "Grey", 21990m, 35000, "Available", now);
        InsertCar(conn, tx, melbourne, "Kia", "Sportage", 2023, "White", 36990m, 8000, "Available", now);

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
        decimal price, int mileage, string status, string now, string? deletedAt = null) =>
        conn.Execute("""
            INSERT INTO Cars (DealershipId, Make, Model, Year, Color, Price, Mileage, Status, CreatedAt, UpdatedAt, DeletedAt)
            VALUES (@DealershipId, @Make, @Model, @Year, @Color, @Price, @Mileage, @Status, @Now, @Now, @DeletedAt);
            """,
            new
            {
                DealershipId = dealershipId,
                Make = make,
                Model = model,
                Year = year,
                Color = color,
                Price = price,
                Mileage = mileage,
                Status = status,
                Now = now,
                DeletedAt = deletedAt
            },
            tx);
}