using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ClipVault.Models;
using ClipVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;

namespace ClipVault.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ClipboardService _clipboardService;
        private readonly DatabaseService _databaseService;
        private readonly StoreService _storeService;
        private readonly DispatcherQueue _dispatcherQueue;

        [ObservableProperty]
        private ObservableCollection<ClipboardItem> _items;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool _isPremium;

        [ObservableProperty]
        private bool _isPinnedFilter; // If true, only show pinned

        [ObservableProperty]
        private bool _isBusy;

        public MainViewModel()
        {
            // Simplified constructor for demo. Proper DI would be better.
            _clipboardService = new ClipboardService();
            _databaseService = new DatabaseService();
            _storeService = new StoreService();
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            Items = new ObservableCollection<ClipboardItem>();
            LoadItems();

            _clipboardService.ClipboardChanged += OnClipboardChanged;
            _storeService.PremiumStatusChanged += (s, isPremium) => IsPremium = isPremium;

            // Initial load check
            IsPremium = _storeService.IsPremium;
        }

        public void LoadItems()
        {
            IsBusy = true;
            try
            {
                // In a real app, pass filters to DB to be efficient
                var allItems = _databaseService.GetItems(IsPremium);

                var filtered = allItems;

                if (IsPinnedFilter)
                {
                    filtered = filtered.Where(x => x.IsPinned).ToList();
                }

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    filtered = filtered.Where(x => x.Content.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)).ToList();
                }

                Items.Clear();
                foreach (var item in filtered)
                {
                    Items.Add(item);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnClipboardChanged(object sender, string text)
        {
            if (Items.Count > 0 && Items[0].Content == text) return;

            _dispatcherQueue.TryEnqueue(() =>
           {
               _databaseService.AddItem(text);
               RefreshItemsCommand.Execute(null);
           });
        }

        [RelayCommand]
        private void RefreshItems()
        {
            LoadItems();
        }

        [RelayCommand]
        private void CopyItem(ClipboardItem item)
        {
            if (item == null) return;
            _clipboardService.SetContent(item.Content);
        }

        [RelayCommand]
        private void TogglePin(ClipboardItem item)
        {
            if (item == null) return;

            // Toggle
            bool newPinState = !item.IsPinned;
            _databaseService.TogglePin(item.Id, newPinState);

            // Refresh list
            RefreshItems();
        }

        [RelayCommand]
        private void DeleteItem(ClipboardItem item)
        {
            if (item == null) return;
            _databaseService.DeleteItem(item.Id);
            Items.Remove(item);
        }

        [RelayCommand]
        private void Search()
        {
            LoadItems();
        }

        [RelayCommand]
        private async Task PurchasePremium()
        {
            bool success = await _storeService.PurchasePremium();
            if (success)
            {
                IsPremium = true;
                RefreshItems();
            }
        }
    }
}
