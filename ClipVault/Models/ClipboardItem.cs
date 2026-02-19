using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClipVault.Models
{
    public class ClipboardItem : INotifyPropertyChanged
    {
        private bool _isPinned;
        
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                if (_isPinned != value)
                {
                    _isPinned = value;
                    OnPropertyChanged();
                }
            }
        }

        // For display - use a shortened version
        // Requirement: not too long.
        public string TimestampDisplay => Timestamp.ToString("g");
        public string Preview => Content?.Length > 60 ? Content.Substring(0, 60) + "..." : Content;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
