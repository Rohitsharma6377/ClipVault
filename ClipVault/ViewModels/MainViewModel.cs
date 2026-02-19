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
using Microsoft.UI.Xaml; // For ElementSoundPlayer

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
        private bool _isPinnedFilter;

        [ObservableProperty]
        private bool _isBusy;

        private bool _handleUpdates = true;

        public MainViewModel()
        {
            _clipboardService = new ClipboardService();
            _databaseService = new DatabaseService();
            _storeService = new StoreService();
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            Items = new ObservableCollection<ClipboardItem>();
            LoadItemsAsync();

            _clipboardService.ClipboardChanged += OnClipboardChanged;
            _storeService.PremiumStatusChanged += (s, isPremium) => IsPremium = isPremium;
            IsPremium = _storeService.IsPremium;

            // Enable global sounds
            ElementSoundPlayer.State = ElementSoundPlayerState.On;
        }

        public async void LoadItemsAsync(bool forceReload = false)
        {
            if (IsBusy && !forceReload) return;

            IsBusy = true;
            try
            {
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
            if (Items.Count > 0 && Items[0].Content == text) return;

            Task.Run(() =>
            {
                _databaseService.AddItem(text);
                _dispatcherQueue.TryEnqueue(() =>
                {
                    if (string.IsNullOrEmpty(SearchText) && !IsPinnedFilter)
                    {
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
            _handleUpdates = false;
            try
            {
                _clipboardService.SetContent(item.Content);
                PlaySound(ElementSoundKind.Invoke);
            }
            finally
            {
                Task.Delay(500).ContinueWith(_ => _handleUpdates = true);
            }
        }

        [RelayCommand]
        private async Task ClearAll()
        {
            // Simple logic: Clear based on current view
            IsBusy = true;
            await Task.Run(() =>
            {
                _databaseService.ClearAll(IsPinnedFilter);
            });

            // Refresh UI
            LoadItemsAsync(true);

            IsBusy = false;
            PlaySound(ElementSoundKind.Hide);
        }

        [RelayCommand]
        private void TogglePin(ClipboardItem item)
        {
            if (item == null) return;

            bool newPinState = !item.IsPinned;
            item.IsPinned = newPinState;

            Task.Run(() => _databaseService.TogglePin(item.Id, newPinState));

            if (IsPinnedFilter && !newPinState)
            {
                Items.Remove(item);
            }

            PlaySound(newPinState ? ElementSoundKind.Show : ElementSoundKind.Hide);
        }

        [RelayCommand]
        private void DeleteItem(ClipboardItem item)
        {
            if (item == null) return;

            Task.Run(() => _databaseService.DeleteItem(item.Id));
            Items.Remove(item);
            PlaySound(ElementSoundKind.Hide);
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
                PlaySound(ElementSoundKind.Show);
            }
        }

        private void PlaySound(ElementSoundKind sound)
        {
            try
            {
                ElementSoundPlayer.Play(sound);
            }
            catch { }
        }
    }
}
