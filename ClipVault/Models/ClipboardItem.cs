using System;

namespace ClipVault.Models
{
    public class ClipboardItem
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsPinned { get; set; }

        // For display
        public string TimestampDisplay => Timestamp.ToString("g");
        public string Preview => Content?.Length > 50 ? Content.Substring(0, 50) + "..." : Content;
    }
}
