using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Diagnostics;
using Microsoft.UI.Dispatching; // WinUI 3 specific dispatcher
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;

namespace ClipVault.Services
{
    public class ClipboardService
    {
        public event EventHandler<string> ClipboardChanged;
        public event EventHandler<byte[]> ImageChanged;
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
                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            ClipboardChanged?.Invoke(this, text);
                        });
                    }
                }
                else if (dataPackageView.Contains(StandardDataFormats.Bitmap))
                {
                    var streamRef = await dataPackageView.GetBitmapAsync();
                    using (var stream = await streamRef.OpenReadAsync())
                    {
                        var decoder = await BitmapDecoder.CreateAsync(stream);
                        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                        
                        // Convert to byte array (PNG)
                        using (var ms = new InMemoryRandomAccessStream())
                        {
                            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms);
                            encoder.SetSoftwareBitmap(softwareBitmap);
                            await encoder.FlushAsync();
                            
                            byte[] bytes = new byte[ms.Size];
                            await ms.ReadAsync(bytes.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
                            
                            _dispatcherQueue.TryEnqueue(() =>
                            {
                                ImageChanged?.Invoke(this, bytes);
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
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
