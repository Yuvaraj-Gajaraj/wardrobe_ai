# WardrobeAI

A .NET MAUI sample that showcases how to build an AI-driven wardrobe
assistant with **Syncfusion's .NET MAUI controls**. The app lets a user
catalog their closet, plan an outfit for a given occasion and weather,
and discover missing essentials to shop for — all driven by Azure
OpenAI (with a deterministic offline fallback when no model is wired up).

## Sample Overview

The app boots directly into a five-tab Syncfusion `SfTabView` shell:

- **Dashboard** — today's AI-curated outfit suggestion, live weather
  chip, quick actions, upcoming plans.
- **Closet** — gallery of clothing items with category filter chips
  and an "Add Clothing" popup (camera/gallery capture + 7 attribute
  chip groups).
- **Plan** — occasion chips (Work / Meeting / Casual / Party), live
  weather, AI-generated outfit hero with four radial score gauges,
  alternatives, "Why this works" blurb, and a Plan-an-outfit popup.
- **Shop** — wardrobe completion score, budget widget, AI-curated
  missing-essentials cards with deep links to real retailers
  (Amazon / Flipkart / Myntra / Nordstrom / ASOS).
- **Profile** — style preferences, palette swatches, achievements,
  monthly budget slider, settings list.

On desktop (Windows / Mac Catalyst), the same five views are arranged
in a persistent split-view shell with a side rail toggling between
the Dashboard+Shopping and Closet+Planner panes.

## Syncfusion Controls Demonstrated

| Control | Used in |
| --- | --- |
| `SfTabView` | Main tabbed shell (`MainTabHostPage`) |
| `SfAvatarView` | Dashboard greeting, desktop side rail, Profile header |
| `SfRotator` | Today's Suggestion hero, Outfit Planner hero, Shopping products |
| `SfRadialGauge` | Today's Match score, four per-axis scores on the planner |
| `SfBusyIndicator` | AI-generation loading state |
| `SfChipGroup` | Closet filter, Add-Clothing attribute pickers, Planner occasion chips, Profile style chips |
| `SfComboBox` | Add-Clothing attribute pickers |
| `SfButton` | Quick actions, FABs, save buttons, "Shop Now" CTA |
| `SfPopup` | Add-Clothing popup, Plan-an-outfit popup, Add-Style popup |
| `SfDatePicker` | Plan-an-outfit date picker |
| `SfAccordion` | Shopping "How we picked these" disclosure |

## Prerequisites

- .NET 10 SDK
- MAUI workload:
  ```bash
  dotnet workload install maui
  ```
- Azure OpenAI resource (optional — the app falls back to a
  deterministic outfit / shopping picker when not configured)

## Setup

1. Clone the repository.
2. Restore the .NET workloads and packages:
   ```bash
   dotnet workload restore
   dotnet restore WardrobeAI/WardrobeAI.csproj
   ```
3. (Optional) Wire up Azure OpenAI by editing
   `WardrobeAI/AI/AzureOpenAiOptions.cs` or by overriding the bound
   configuration in your `MauiProgram.cs`. The default values
   (`"END_POINT"`, `"API_KEY"`, `"MODEL"`, `"API_VERSION"`) are
   placeholders — the app detects an unconfigured state and uses the
   deterministic fallback path, so the sample is fully runnable
   without an Azure subscription.
4. Register your Syncfusion license key. The license is **not**
   hard-coded in the repo. Set it as an environment variable before
   running the app:
   ```bash
   # Windows (PowerShell)
   $env:SYNCFUSION_LICENSE_KEY = "your-key-here"
   # macOS / Linux
   export SYNCFUSION_LICENSE_KEY="your-key-here"
   ```
   `MauiProgram.cs` reads `SYNCFUSION_LICENSE_KEY` and calls
   `SyncfusionLicenseProvider.RegisterLicense(...)` at startup when
   the variable is set. When the variable is empty the controls
   render in trial mode (watermarked).

## AI Setup Steps

The sample ships with a deterministic offline path so the AI
endpoints are runnable without any Azure wiring. To enable the live
LLM path:

1. Create an Azure OpenAI resource and a chat-completion deployment
   (for example `gpt-4o`).
2. Populate `AzureOpenAiOptions` at runtime (the sample's default
   binding reads from `appsettings.json` / `Preferences`, but you can
   wire any `IConfiguration` source — environment variables, user
   secrets, etc.). The four fields are:
   - `Endpoint` — `https://<your-resource>.openai.azure.com/` (trailing
     slash required)
   - `ApiKey` — Azure OpenAI key
   - `Deployment` — deployment name (e.g. `gpt-4o`)
   - `ApiVersion` — e.g. `2024-08-01-preview`
3. Restart the app. Once `IsConfigured` returns `true` the
   `AzureOpenAiOutfitRecommendationService` and
   `AzureOpenAiSmartShoppingService` will start calling Chat
   Completions with JSON-schema structured outputs (the deterministic
   fallback is no longer hit).
4. Weather and geolocation use the free, key-less
   [Open-Meteo](https://open-meteo.com/) APIs — no setup required.

## Run Instructions

Pick the target platform from the project file and run:

```bash
# Windows
dotnet build WardrobeAI/WardrobeAI.csproj -f net10.0-windows10.0.19041.0
dotnet run  --project WardrobeAI/WardrobeAI.csproj -f net10.0-windows10.0.19041.0

# Android
dotnet build WardrobeAI/WardrobeAI.csproj -f net10.0-android
dotnet run  --project WardrobeAI/WardrobeAI.csproj -f net10.0-android

# iOS / Mac Catalyst (macOS host required)
dotnet build WardrobeAI/WardrobeAI.csproj -f net10.0-maccatalyst
dotnet run  --project WardrobeAI/WardrobeAI.csproj -f net10.0-maccatalyst
```

The first launch seeds a default user (Alex Rivera) into the local
SQLite database at `FileSystem.AppDataDirectory/wardrobe.db` and
lands on the Dashboard tab.

## Project Structure

```
WardrobeAI/
├── AI/                  Azure OpenAI service implementations + scoring
├── Constants/           Closet attribute catalogs (ClothingOptions)
├── Contracts/           Cross-cutting DTOs (weather, etc.)
├── Converters/          XAML value converters
├── Data/                EF Core DbContext
├── Helpers/             Result<T> + small utilities
├── Models/              Persistence entities
├── Platforms/           Per-platform code (Android, iOS, Windows, Mac)
├── Repositories/        EF Core repositories
├── Resources/           Images, fonts, raw assets (wardrobe.json), styles
├── Services/            Cross-cutting services (weather, user, analysis, …)
├── ViewModels/          MVVM view models (one per page + tab host)
└── Views/               XAML pages (Dashboard, Closet, OutfitPlanner, Shopping, Profile)
```

## Notes for Customers

- The sample was built to demonstrate control patterns (chip groups,
  popups, gauges, rotator), MVVM with `CommunityToolkit.Mvvm`, and
  the end-to-end Azure OpenAI flow with structured outputs.
- The two `MauiXaml Update="…" Inflator="Runtime"` lines in
  `WardrobeAI.csproj` are intentional workarounds for MAUIG1001
  parse failures on `SfTabView` and `SfChip` XAML under the source
  generator. They keep the rest of the app on the faster source-gen
  path.
- The `MauiMaterialAssets.ttf` font registration in `MauiProgram.cs`
  points at the .ttf file that ships inside the
  `Syncfusion.Maui.Core` NuGet package — no need to bundle a copy
  under `Resources/Fonts/`.
