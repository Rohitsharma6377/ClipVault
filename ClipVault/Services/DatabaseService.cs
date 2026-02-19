using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using ClipVault.Models;

namespace ClipVault.Services
{
    public class DatabaseService
    {
        private string _dbPath;

        public DatabaseService()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClipVault");
            Directory.CreateDirectory(folder);
            _dbPath = Path.Combine(folder, "clipvault.db");
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                    CREATE TABLE IF NOT EXISTS ClipboardItems (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Content TEXT NOT NULL,
                        Timestamp TEXT NOT NULL,
                        IsPinned INTEGER DEFAULT 0
                    );
                ";
                command.ExecuteNonQuery();
            }
        }

        public List<ClipboardItem> GetItems(bool isPremium)
        {
            var items = new List<ClipboardItem>();
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                
                // If premium, get all. If not, get pinned + recent 10 non-pinned? 
                // Wait, requirement: "Limit free version to last 10 items". Pinned items should probably persist.
                // Let's just limit query if not premium.
                // It's safer to just fetch top N by date descending.
                
                string limitClause = isPremium ? "" : "LIMIT 10";
                
                command.CommandText = $"SELECT Id, Content, Timestamp, IsPinned FROM ClipboardItems ORDER BY IsPinned DESC, Timestamp DESC {limitClause}";
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new ClipboardItem
                        {
                            Id = reader.GetInt32(0),
                            Content = reader.GetString(1),
                            Timestamp = DateTime.Parse(reader.GetString(2)),
                            IsPinned = reader.GetBoolean(3)
                        });
                    }
                }
            }
            return items;
        }

        public void AddItem(string content)
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                // Check for duplicate recent content to avoid spam
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT Content FROM ClipboardItems WHERE Content = $content ORDER BY Timestamp DESC LIMIT 1";
                checkCmd.Parameters.AddWithValue("$content", content);
                var existing = checkCmd.ExecuteScalar() as string;
                if (existing == content) return;

                var command = connection.CreateCommand();
                command.CommandText = 
                @"
                    INSERT INTO ClipboardItems (Content, Timestamp, IsPinned)
                    VALUES ($content, $timestamp, 0)
                ";
                command.Parameters.AddWithValue("$content", content);
                command.Parameters.AddWithValue("$timestamp", DateTime.Now.ToString("o"));
                command.ExecuteNonQuery();
            }
        }

        public void DeleteItem(int id)
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM ClipboardItems WHERE Id = $id";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
        }

        public void TogglePin(int id, bool isPinned)
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE ClipboardItems SET IsPinned = $pinned WHERE Id = $id";
                command.Parameters.AddWithValue("$pinned", isPinned ? 1 : 0);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
        }
        
        public void ClearHistory(bool keepPinned = true)
        {
             using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                if (keepPinned)
                    command.CommandText = "DELETE FROM ClipboardItems WHERE IsPinned = 0";
                else
                    command.CommandText = "DELETE FROM ClipboardItems";
                command.ExecuteNonQuery();
            }
        }
    }
}
