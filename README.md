# WinUI 3 Line-of-Business Sample Hub

Five self-contained WinUI 3 (Windows App SDK) desktop samples demonstrating common
line-of-business patterns. Each sample is an independent, buildable Visual Studio
solution. They are linked from the documentation at
[learn.microsoft.com/windows/apps/get-started/line-of-business/](https://learn.microsoft.com/windows/apps/get-started/line-of-business/).

## Samples

| # | Folder | Demonstrates |
|---|--------|--------------|
| 1 | [`WinUI-LOB-Samples/01-TabularData`](WinUI-LOB-Samples/01-TabularData) | **Recommended:** `ItemsView` + `DataTemplate` card layout as the first-party way to show collections. Also shows a columnar `ListView` "table" layout that stands in for the missing first-party data grid — a stopgap that illustrates the gap, not a peer recommendation |
| 2 | [`WinUI-LOB-Samples/02-ValidatedForm`](WinUI-LOB-Samples/02-ValidatedForm) | Input validation as a **known WinUI gap** — WinUI has no built-in input-validation control (unlike WPF), so this sample hand-rolls per-keystroke validation with `INotifyDataErrorInfo`, inline errors, and Save gated on `HasErrors` |
| 3 | [`WinUI-LOB-Samples/03-DatabaseAccess`](WinUI-LOB-Samples/03-DatabaseAccess) | EF Core + SQLite task tracker, all data access **async and off the UI thread** |
| 4 | [`WinUI-LOB-Samples/04-DesignShowcase`](WinUI-LOB-Samples/04-DesignShowcase) | `NavigationView`, Mica backdrop, light/dark theme toggle, summary card grid |
| 5 | [`WinUI-LOB-Samples/05-LocalAI`](WinUI-LOB-Samples/05-LocalAI) | **Local AI on a Copilot+ PC** — on-device support-ticket triage (summarize + categorize) with the Phi Silica language model via `Microsoft.Windows.AI.Text.LanguageModel`; no data leaves the device |

## Screenshots

| Sample | Screenshot |
|--------|-----------|
| 1 — TabularData (cards) | ![Cards / ItemsView](screenshots/01-TabularData-cards.png) |
| 2 — ValidatedForm (invalid email, Save disabled) | ![Validated form](screenshots/02-ValidatedForm.png) |
| 3 — DatabaseAccess (task tracker) | ![Task tracker](screenshots/03-DatabaseAccess.png) |
| 4 — DesignShowcase (dashboard + Mica) | ![Design showcase](screenshots/04-DesignShowcase.png) |
| 5 — LocalAI (on-device triage, experimental channel) | ![Local AI triage](screenshots/05-LocalAI-experimental.png) |
| 5 — LocalAI (graceful degradation when the LAF gate blocks generation on the stable channel) | ![Local AI blocked](screenshots/05-LocalAI-stable.png) |

## Shared conventions (all samples)

- **WinUI 3 desktop**, Windows App SDK **2.3.1** (stable channel), C# only, `net10.0-windows`.
- **MVVM** throughout — ViewModels derive from `ObservableObject` (`CommunityToolkit.Mvvm`).
- **Prefer `x:Bind`** for data binding; `Binding` still has valid uses (e.g. runtime-typed or late-bound scenarios).
- **Prefer `ItemsView`** for new collection UI; `ListView` remains valid (Sample 1's columnar table layout uses it).
- **`Microsoft.UI.Xaml.*`** only — no UWP `Windows.UI.Xaml.*`, `ApplicationView`, `CoreWindow`, or `CoreApplication`.
- **System theme brushes only** — no hardcoded colors.
- All data loading is **async**; EF Core queries and on-device model calls never run on the UI thread.
- No connection strings or credentials in source; the SQLite path is built at runtime from `LocalApplicationData`.

## Building & running

Each sample requires .NET SDK **10.0.302+**, Windows App SDK, Developer Mode enabled, and the
`winapp` CLI. From a sample's project folder:

```powershell
dotnet build
winapp run          # launch the packaged app (do not run the .exe directly)
```

Every sample builds with **zero errors and zero warnings** and has been launched and validated.

## Key findings (SME-relevant discoveries)

These are the "known unknowns" that were resolved while building the samples — the docs should
reflect these exact values:

1. **No first-party WinUI data grid yet (Sample 1).**
   WinUI 3 has no built-in data-grid control, and the old Community Toolkit
   `CommunityToolkit.WinUI.UI.Controls.DataGrid` is effectively unmaintained (no meaningful
   updates in years), so it is **not** recommended here. Until first-party grid support ships,
   use `ItemsView` + `DataTemplate` for collections (the recommended approach) and, where a
   columnar layout is genuinely needed, a first-party `ListView` with a columnar `DataTemplate`
   as a stopgap. Sample 1 shows both.

2. **No built-in input-validation control (Sample 2).**
   WinUI has no built-in validation UX control — a known gap versus WPF. Rather than scaffolding
   around a third-party abstraction, Sample 2 hand-rolls validation with `INotifyDataErrorInfo`
   (BCL), surfacing inline errors and gating Save on `HasErrors`.

3. **Compact density is deprecated (Sample 4).**
   Compact density has been **deprecated** and can break controls, so it is *not* recommended.
   There is no "contemporary" compact mode — the supported answer is simply the **default**
   density. Sample 4 ships default density only (theme + backdrop toggles remain).

4. **Phi Silica API namespace (Sample 5).**
   The current, verified local-AI API is `Microsoft.Windows.AI.Text.LanguageModel` (with the
   readiness enum `Microsoft.Windows.AI.AIFeatureReadyState`). The older
   `Microsoft.Windows.AI.Generative.*` namespace seen in some articles — and in this repo's own
   [`docs/ai-for-lob-apps.md`](docs/ai-for-lob-apps.md) — is **outdated**; that doc needs updating
   (see TODOs). Canonical call sequence:
   ```csharp
   var state = LanguageModel.GetReadyState();          // AIFeatureReadyState
   if (state == AIFeatureReadyState.NotReady) await LanguageModel.EnsureReadyAsync();
   var model = await LanguageModel.CreateAsync();
   var result = await model.GenerateResponseAsync(prompt);   // check result.Status == Complete
   ```
   Requires the restricted capability `systemAIModels` (with the `rescap`/`systemai` manifest
   namespace) — the `dotnet new winui` template already declares it.

5. **Phi Silica is a Limited Access Feature (LAF) on the stable channel (Sample 5).** ⚠️
   On **stable** Windows App SDK 2.3.1, `GenerateResponseAsync` throws *"Access is denied. Limited
   Access Feature is not available: com.microsoft.windows.ai.languagemodel. Status: 3"* for a
   locally dev-registered, unsigned package — even though `GetReadyState()` returns `Ready`, the
   `systemAIModels` capability is granted, and the privacy consent is allowed. `TryUnlockFeature`
   with the machine LAF token returns `Unavailable` because the token is bound to a
   Microsoft-issued **per-package identity**, which a sample can't obtain.
   The official [Windows AI troubleshooting guide](https://learn.microsoft.com/windows/ai/apis/troubleshooting)
   confirms this and recommends the **experimental channel**, which does *not* require a LAF token.
   Verified on this machine: switching to `Microsoft.WindowsAppSDK 2.2.2-experimental9` makes
   on-device generation succeed end-to-end (see the `05-LocalAI-experimental` screenshot). The
   sample ships on **stable** per the shared convention and **degrades gracefully** — it detects
   the gate, shows an explanatory `InfoBar`, and disables the triage button rather than surfacing a
   raw error.

## Open TODOs / SME follow-ups

- **Build toolchain:** `net10.0` requires **MSBuild 18+**. Visual Studio 2022's bundled MSBuild
  (17.14) cannot build these projects, so each sample pins the SDK via `global.json` (10.0.302)
  and builds through the `dotnet` CLI. Ensure the team's standard build host / CI uses a Visual
  Studio version with MSBuild 18, or standardize on `dotnet build`. The `global.json` files can be
  relaxed once that is confirmed.
- **Sample 3 – schema evolution:** uses `EnsureCreatedAsync()` (does not support schema changes).
  Switch to EF Core **migrations** for a production app.
- **Sample 3 – persistence layer:** no edit/delete-task UI yet (spec covered add + complete only).
- **Sample 2 – `Save()` is a stub** (shows an InfoBar and resets the form); wire it to a real
  service layer for an actual LOB app.
- **Sample 3 – supply chain:** the transitive `SQLitePCLRaw.lib.e_sqlite3 2.1.11` carried
  CVE-2025-6965; pinned `SQLitePCLRaw.bundle_e_sqlite3 2.1.12` to stay clean. Bump when EF Core
  ships a newer transitive default.
- **Sample 5 – local AI runtime gate (needs SME/environment decision).** On-device Phi Silica
  generation is blocked on the **stable** channel by the Limited Access Feature gate (see Key
  Finding 4). Options for the docs: (a) ship on stable with graceful degradation as done here and
  document that end-to-end generation requires either a Microsoft-issued LAF token or the
  experimental channel; or (b) target the experimental channel for the live-AI walkthrough
  (note: `ItemsView`/`ItemContainer` are marked evaluation-only there and emit `CS8305`, so it
  can't hit zero warnings without `<NoWarn>`). SME to confirm the intended guidance for readers.
- **Sample 5 – SDK pin divergence.** This machine only has .NET SDK **10.0.300-preview.0.26177.108**
  installed (not 10.0.302), so `05-LocalAI/.../global.json` pins that preview to build and run
  locally. Normalize to **10.0.302** (matching Samples 1–4) on a host that has it installed.
- **Sample 5 – `docs/ai-for-lob-apps.md` updated.** The Phi Silica section now uses the correct
  `Microsoft.Windows.AI.Text.LanguageModel` API (was the outdated `Microsoft.Windows.AI.Generative`)
  and documents the LAF gate / channel guidance. SME to review the wording.
