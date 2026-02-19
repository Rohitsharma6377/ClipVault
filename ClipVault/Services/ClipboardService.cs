using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Diagnostics;
using Microsoft.UI.Dispatching; // WinUI 3 specific dispatcher

namespace ClipVault.Services
{
    public class ClipboardService
    {
        public event EventHandler<string> ClipboardChanged;
        private DispatcherQueue _dispatcherQueue;

        public ClipboardService()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            try
            {
                Clipboard.ContentChanged += OnClipboardContentChanged;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error subscribing to clipboard: {ex.Message}");
            }
        }

        private async void OnClipboardContentChanged(object sender, object e)
        {
            try
            {
                DataPackageView dataPackageView = Clipboard.GetContent();
                if (dataPackageView.Contains(StandardDataFormats.Text))
                {
                    string text = await dataPackageView.GetTextAsync();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        // Dispatch to avoid threading issues if accessed from bg thread
                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            ClipboardChanged?.Invoke(this, text);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Clipboard might be locked by another process
                Debug.WriteLine($"Clipboard read error: {ex.Message}");
            }
        }

        public void SetContent(string text)
        {
            var package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContent(package);
        }
    }
}
