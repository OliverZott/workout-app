# Copilot Instructions (workout-app)

## Project baseline
- Stack: **.NET 10 + .NET MAUI** (Android/iOS).
- Do not suggest Xamarin.Forms APIs; use MAUI equivalents.
- Keep changes minimal and non-breaking.

## Architecture conventions
- UI pages: `Pages/*.xaml`
- Reusable controls: `Pages/Controls/*`
- ViewModels: `PageModels/*` (CommunityToolkit.Mvvm)
- Data layer: `Services/DatabaseService.cs` with SQLite (`veolsaurus.db3`)
- Localization: `Resources/Languages/AppResources.resx` (default English) + `AppResources.de.resx`

## Data behavior
- DB path: `FileSystem.AppDataDirectory/veolsaurus.db3`
- `InitializeAsync()` creates schema and seeds only when tables are empty.
- Database backup/restore is available in Settings:
  - Export raw `.db3` via share
  - Import `.db3` via picker with explicit confirmation
- Use DatabaseSeed-based seeding for both debug and release (no bundled debug db flow).

## Localization rules
- All user-facing text should come from `AppResources` keys.
- Add/update keys in both EN and DE resource files.
- Keep `AppResources.Designer.cs` in sync if auto-generation does not update.

## UI/style preferences
- Compact numeric inputs and gray default text/placeholder styling.
- Keep chart max-range rendering readable (avoid dense zero-filled outputs).
- Keep export and import as separate sections in Settings.

## Validation
- Verify changed files compile (`get_errors`/build).
- Avoid unrelated refactors.