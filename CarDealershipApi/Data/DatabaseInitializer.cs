using Dapper;
using Microsoft.Data.Sqlite;

namespace CarDealershipApi.Data
{
    public class DatabaseInitializer
    {
        public static void InitializeDatabase(string connectionString)
        {
            using var connection = new SqliteConnection(connectionString);

            connection.Execute("""
                    CREATE TABLE IF NOT EXISTS Dealerships (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DealershipId INTEGER NULL REFERENCES Dealerships(Id),
                        Username TEXT NOT NULL UNIQUE,
                        Email TEXT NOT NULL UNIQUE COLLATE NOCASE,
                        PasswordHash TEXT NOT NULL,
                        Role TEXT NOT NULL DEFAULT 'Staff',
                        CreatedAt TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS Cars (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DealershipId INTEGER NOT NULL REFERENCES Dealerships(Id),
                        Make TEXT NOT NULL,
                        Model TEXT NOT NULL,
                        Year INTEGER NOT NULL,
                        Color TEXT NOT NULL,
                        Price REAL NOT NULL,
                        Stock INTEGER NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT NOT NULL,
                        DeletedAt TEXT NULL
                    );

                    CREATE INDEX IF NOT EXISTS IDX_Cars_DealershipId ON Cars(DealershipId);
                """);
        }
    }
}