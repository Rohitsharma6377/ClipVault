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
            try
            {
                using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                        CREATE TABLE IF NOT EXISTS ClipboardItems (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Content TEXT,
                            ImageData BLOB,
                            ItemType TEXT NOT NULL DEFAULT 'Text',
                            Timestamp TEXT NOT NULL,
                            IsPinned INTEGER DEFAULT 0
                        );
                    ";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }

        public List<ClipboardItem> GetItems(bool isPremium)
        {
            var items = new List<ClipboardItem>();
            try
            {
                using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();

                    // Always show Pinned first, then newest
                    string limitClause = isPremium ? "" : "LIMIT 50"; // Increase limit a bit for user experience

                    command.CommandText = $"SELECT Id, Content, ImageData, ItemType, Timestamp, IsPinned FROM ClipboardItems ORDER BY IsPinned DESC, Timestamp DESC {limitClause}";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new ClipboardItem
                            {
                                Id = reader.GetInt32(0),
                                Content = reader.IsDBNull(1) ? null : reader.GetString(1),
                                ImageData = reader.IsDBNull(2) ? null : (byte[])reader.GetValue(2),
                                ItemType = reader.GetString(3),
                                Timestamp = DateTime.Parse(reader.GetString(4)),
                                IsPinned = reader.GetBoolean(5)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching items: {ex.Message}");
            }
            return items;
        }

        public void AddItem(string content, byte[] imageData = null, string type = "Text")
        {
            try
            {
                using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                {
                    connection.Open();

                    if (type == "Text" && !string.IsNullOrEmpty(content))
                    {
                        var checkCmd = connection.CreateCommand();
                        checkCmd.CommandText = "SELECT Content FROM ClipboardItems WHERE Content = $content AND ItemType = 'Text' ORDER BY Timestamp DESC LIMIT 1";
                        checkCmd.Parameters.AddWithValue("$content", content);
                        var existing = checkCmd.ExecuteScalar() as string;
                        if (existing == content) return;
                    }

                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                        INSERT INTO ClipboardItems (Content, ImageData, ItemType, Timestamp, IsPinned)
                        VALUES ($content, $imageData, $type, $timestamp, 0)
                    ";
                    command.Parameters.AddWithValue("$content", content ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("$imageData", imageData ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("$type", type);
                    command.Parameters.AddWithValue("$timestamp", DateTime.Now.ToString("o"));
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding item: {ex.Message}");
            }
        }

        public void DeleteItem(int id)
        {
            try
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
            catch { }
        }

        public void ClearAll(bool onlyPinned)
        {
            try
            {
                using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    if (onlyPinned)
                    {
                        command.CommandText = "DELETE FROM ClipboardItems WHERE IsPinned = 1";
                    }
                    else
                    {
                        // If user wants to clear "Clipboard", usually means history.
                        // Should we keep Pinned? Usually "Clear All" means clear non-pinned, or clear strictly what is shown.
                        // Let's assume clear everything EXCEPT pinned if viewing main list, to be safe?
                        // Or "Clear All" means wipe DB?
                        // User said "clean clip board".
                        // Let's clear non-pinned items.
                        command.CommandText = "DELETE FROM ClipboardItems WHERE IsPinned = 0";
                    }
                    command.ExecuteNonQuery();
                }
            }
            catch { }
        }

        public void TogglePin(int id, bool isPinned)
        {
            try
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
            catch { }
        }
    }
}
