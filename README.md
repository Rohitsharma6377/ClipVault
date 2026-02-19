# ClipVault

ClipVault is a modern Windows clipboard manager built with WinUI 3 and C#.

## Features

- **Clipboard History**: Automatically tracks copied text.
- **Pinning**: Pin important items to keep them at the top.
- **Search**: Fast real-time search of history.
- **Glass UI**: Beautiful Mica/Acrylic background.
- **Premium Upgrade**: Unlock unlimited history via Microsoft Store integration.

## Requirements

- Windows 10 (1809+) or Windows 11.
- Visual Studio 2022 with "Windows App SDK" workload.

## Architecture

- **MVVM**: Using CommunityToolkit.Mvvm.
- **Database**: SQLite (local storage in AppData).
- **Store**: Windows.Services.Store for In-App Purchases.

## How to Run

1. Open `ClipVault.sln` in Visual Studio.
2. Ensure the startup project is `ClipVault`.
3. Press F5 to build and run.
4. Note: Store features (Purchase) require Store Association or will run in simulation mode.

## Project Structure

- **Models**: Data structures (`ClipboardItem`).
- **ViewModels**: UI logic (`MainViewModel`).
- **Views**: UI Pages (`ClipboardListPage`, `SettingsPage`, `PremiumPage`).
- **Services**: Core logic (`ClipboardService`, `DatabaseService`, `StoreService`).
- **Helpers**: Converters and utilities.

## Notes

- The app uses `Windows.ApplicationModel.DataTransfer.Clipboard` API.
- Purchase flow simulates success for testing purposes if not associated with Store.
