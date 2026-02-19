using System;
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

        // Use this flag to stop listening temporarily if needed
        private bool _handleUpdates = true;

        public MainViewModel()
        {
            _clipboardService = new ClipboardService();
            _databaseService = new DatabaseService();
            _storeService = new StoreService();
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            Items = new ObservableCollection<ClipboardItem>();
            LoadItemsAsync(); // Fire and forget (it handles failures)

            _clipboardService.ClipboardChanged += OnClipboardChanged;
            _storeService.PremiumStatusChanged += (s, isPremium) => IsPremium = isPremium;

            IsPremium = _storeService.IsPremium;
        }

        public async void LoadItemsAsync(bool forceReload = false)
        {
            if (IsBusy && !forceReload) return;

            IsBusy = true;
            try
            {
                // Run DB query on background thread
                var loadedItems = await Task.Run(() =>
                {
                    var allItems = _databaseService.GetItems(IsPremium);
                    var filtered = allItems;

                    if (IsPinnedFilter)
                    {
                        filtered = filtered.Where(x => x.IsPinned).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(SearchText))
                    {
                        filtered = filtered.Where(x => x.Content.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    return filtered;
                });

                // Update UI thread
                Items.Clear();
                foreach (var item in loadedItems)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading items: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnClipboardChanged(object sender, string text)
        {
            if (!_handleUpdates) return;

            // Avoid loops - check most recent
            if (Items.Count > 0 && Items[0].Content == text) return;

            // Run logic on background thread then post to UI
            Task.Run(() =>
            {
                _databaseService.AddItem(text); // Add to DB

                _dispatcherQueue.TryEnqueue(() =>
                {
                    // Only reload if we are not filtering/searching
                    // or if the new item matches current filter
                    if (string.IsNullOrEmpty(SearchText) && !IsPinnedFilter)
                    {
                        // Optimization: Instead of reloading everything, just validly insert at top
                        // But getting ID is tricky without reloading unless AddItem returns ID.
                        // For simplicity and correctness, reload list but quickly.
                        LoadItemsAsync(true);
                    }
                });
            });
        }

        [RelayCommand]
        private void RefreshItems()
        {
            LoadItemsAsync(true);
        }

        [RelayCommand]
        private void CopyItem(ClipboardItem item)
        {
            if (item == null) return;
            _handleUpdates = false; // Don't trigger recursive loop
            try
            {
                _clipboardService.SetContent(item.Content);
            }
            finally
            {
                // Re-enable after short delay to skip own event
                Task.Delay(500).ContinueWith(_ => _handleUpdates = true);
            }
        }

        [RelayCommand]
        private void TogglePin(ClipboardItem item)
        {
            if (item == null) return;

            // Toggle local object state for immediate feedback
            bool newPinState = !item.IsPinned;
            item.IsPinned = newPinState; // UI updates via INotifyPropertyChanged

            // Update DB
            Task.Run(() => _databaseService.TogglePin(item.Id, newPinState));

            // If we are showing only Pinned items, we might want to remove it from view if unpinned
            if (IsPinnedFilter && !newPinState)
            {
                Items.Remove(item);
            }
        }

        [RelayCommand]
        private void DeleteItem(ClipboardItem item)
        {
            if (item == null) return;

            Task.Run(() => _databaseService.DeleteItem(item.Id));
            Items.Remove(item);
        }

        [RelayCommand]
        private void Search()
        {
            LoadItemsAsync(true);
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
