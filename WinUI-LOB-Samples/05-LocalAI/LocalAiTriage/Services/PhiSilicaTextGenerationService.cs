using System;
using System.Threading.Tasks;
using Microsoft.Windows.AI;
using Microsoft.Windows.AI.Text;

namespace LocalAiTriage.Services;

/// <summary>
/// On-device text generation backed by the Phi Silica small language model in
/// the Windows App SDK. All work runs locally on the NPU of a Copilot+ PC — no
/// data leaves the device and no network connection is required.
/// </summary>
public sealed class PhiSilicaTextGenerationService : ITextGenerationService, IDisposable
{
    private LanguageModel? _model;

    public async Task<AiStatus> EnsureReadyAsync()
    {
        AIFeatureReadyState state;
        try
        {
            state = LanguageModel.GetReadyState();
        }
        catch (Exception ex)
        {
            // The AI runtime is not present (for example, on a non-Copilot+ PC).
            return new AiStatus(false, $"Local AI is unavailable on this device. {ex.Message}");
        }

        switch (state)
        {
            case AIFeatureReadyState.Ready:
                break;

            case AIFeatureReadyState.NotReady:
                // The model needs to be downloaded/prepared. This can take a while
                // on first run, so it is awaited off the UI thread by the caller.
                await LanguageModel.EnsureReadyAsync();
                if (LanguageModel.GetReadyState() != AIFeatureReadyState.Ready)
                {
                    return new AiStatus(false, "The local Phi Silica model could not be prepared on this device.");
                }

                break;

            case AIFeatureReadyState.DisabledByUser:
                return new AiStatus(false, "Local AI is turned off in Windows settings.");

            case AIFeatureReadyState.NotSupportedOnCurrentSystem:
            case AIFeatureReadyState.NotCompatibleWithSystemHardware:
                return new AiStatus(false, "Local AI (Phi Silica) requires a Copilot+ PC with a compatible NPU.");

            case AIFeatureReadyState.OSUpdateNeeded:
                return new AiStatus(false, "A Windows update is required before local AI can be used.");

            case AIFeatureReadyState.CapabilityMissing:
                return new AiStatus(false, "The Windows AI components required for local AI are not installed.");

            default:
                return new AiStatus(false, "Local AI is not available on this device.");
        }

        try
        {
            _model ??= await LanguageModel.CreateAsync();
        }
        catch (Exception ex) when (IsAccessGatedError(ex))
        {
            return new AiStatus(false, LimitedAccessMessage);
        }

        return new AiStatus(true, "Local Phi Silica model ready — running on-device.");
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        if (_model is null)
        {
            throw new InvalidOperationException("Call EnsureReadyAsync before generating a response.");
        }

        LanguageModelResponseResult result;
        try
        {
            result = await _model.GenerateResponseAsync(prompt);
        }
        catch (Exception ex) when (IsAccessGatedError(ex))
        {
            throw new LocalAiUnavailableException(LimitedAccessMessage, ex);
        }

        if (result.Status != LanguageModelResponseStatus.Complete)
        {
            throw new LocalAiUnavailableException(
                $"The local model did not complete the request ({result.Status}).", result.ExtendedError);
        }

        return result.Text?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Message shown when on-device generation is blocked by the Limited Access
    /// Feature (LAF) gate on the stable Windows App SDK channel. See the sample
    /// README for the LAF-token requirement and the experimental-channel option.
    /// </summary>
    private const string LimitedAccessMessage =
        "On-device generation is blocked on this build: the Phi Silica language model is a " +
        "Limited Access Feature that requires a Microsoft-issued unlock token for this app's " +
        "identity on the stable Windows App SDK channel. See the sample README for details.";

    /// <summary>
    /// Detects the Limited Access Feature / access-denied gate that the stable
    /// Windows App SDK enforces for the Phi Silica language model.
    /// </summary>
    private static bool IsAccessGatedError(Exception ex)
    {
        const int E_ACCESSDENIED = unchecked((int)0x80070005);
        return ex is UnauthorizedAccessException
            || ex.HResult == E_ACCESSDENIED
            || (ex.Message?.Contains("Limited Access Feature", StringComparison.OrdinalIgnoreCase) ?? false);
    }

    public void Dispose() => _model?.Dispose();
}

/// <summary>
/// Raised when the local Phi Silica model is present but on-device generation
/// cannot run (for example, because of the Limited Access Feature gate).
/// </summary>
public sealed class LocalAiUnavailableException : Exception
{
    public LocalAiUnavailableException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
