using System.Threading.Tasks;

namespace LocalAiTriage.Services;

/// <summary>
/// The readiness of the on-device language model, together with a
/// human-readable message suitable for display in the UI.
/// </summary>
/// <param name="IsReady">Whether the model is ready to generate responses.</param>
/// <param name="Message">A short status message describing the current state.</param>
public readonly record struct AiStatus(bool IsReady, string Message);

/// <summary>
/// Abstracts on-device text generation so the UI layer depends on an interface
/// rather than a specific model. The concrete implementation uses the local
/// Phi Silica model shipped with the Windows App SDK.
/// </summary>
public interface ITextGenerationService
{
    /// <summary>
    /// Ensures the model is present and initialized. May download the model on
    /// first use, so it must be awaited off the UI thread.
    /// </summary>
    Task<AiStatus> EnsureReadyAsync();

    /// <summary>Generates a response for the supplied prompt on-device.</summary>
    Task<string> GenerateAsync(string prompt);
}
