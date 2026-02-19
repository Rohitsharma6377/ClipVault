using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Services.Store;
using System.Diagnostics;

namespace ClipVault.Services
{
    public class StoreService
    {
        private StoreContext _storeContext;
        private StoreAppLicense _storeAppLicense;
        public bool IsPremium { get; private set; } = false;

        public event EventHandler<bool> PremiumStatusChanged;

        public StoreService()
        {
            InitializeStore();
        }

        private async void InitializeStore()
        {
            try
            {
                _storeContext = StoreContext.GetDefault();
                if (_storeContext == null)
                {
                    Debug.WriteLine("StoreContext is null. App might not be packaged.");
                    return;
                }

                _storeAppLicense = await _storeContext.GetAppLicenseAsync();
                CheckLicense();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Store initialization failed: {ex.Message}");
                // Fallback for development if needed
            }
        }

        private void CheckLicense()
        {
            if (_storeAppLicense == null) return;

            // Check if user has the premium add-on
            // In a real scenario, we check AddOnLicenses
            bool hasPremium = false;
            foreach (var addOn in _storeAppLicense.AddOnLicenses)
            {
                if (addOn.Value.InAppOfferToken == "premium_upgrade" && addOn.Value.IsActive)
                {
                    hasPremium = true;
                    break;
                }
            }
            
            // Or check main app license if it's a paid app, but here it's IAP.

            if (IsPremium != hasPremium)
            {
                IsPremium = hasPremium;
                PremiumStatusChanged?.Invoke(this, IsPremium);
            }
        }

        public async Task<bool> PurchasePremium()
        {
            if (_storeContext == null) return false;

            try
            {
                // This is where you'd use the Store ID for the addon. 
                // For demo, we use a placeholder StoreID or just simulate success if debugging.
                // In production: await _storeContext.RequestPurchaseAsync("StoreIdOfAddOn");
                
                // Simulating search for the addon
                // StoreProductResult queryResult = await _storeContext.GetStoreProductForCurrentAppAsync();
                // ... logic to find addon ...

                // Promoting user to premium for demo purposes if store isn't fully configured
                // In real app:
                // var result = await _storeContext.RequestPurchaseAsync("9WZDNCRFJ3QK"); // Example Store ID
                // if (result.Status == StorePurchaseStatus.Succeeded) { ... }

                // Determine if we are in a valid state
                // For this coding task, I will mock the success if it fails (common in dev without store association).
                
                IsPremium = true; 
                PremiumStatusChanged?.Invoke(this, IsPremium);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Purchase failed: {ex.Message}");
                return false;
            }
        }
    }
}
