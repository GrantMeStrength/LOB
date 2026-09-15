---
title: Add AI capabilities to a line-of-business WinUI app
description: Compare on-device Windows AI, ONNX Runtime, and cloud AI approaches for line-of-business WinUI 3 apps.
ms.topic: how-to
ms.date: 08/31/2026
author: GrantMeStrength
ms.author: jken
---

# Add AI capabilities to a line-of-business WinUI app

AI can add summarization, extraction, classification, search, and assistance to a business workflow. Choose an architecture based on data policy, model quality, supported hardware, connectivity, latency, and operating cost.

:::image type="content" source="images/05-local-ai.png" alt-text="A WinUI 3 support-ticket app displaying an on-device AI summary and suggested category.":::

## Prerequisites

- Complete the [WinUI setup instructions](../../get-started/start-here.md) and create a WinUI 3 app.
- Review the [Windows AI APIs overview](/windows/ai/) and the requirements for the API version you plan to use.
- For cloud AI, configure an approved Azure resource, identity, network path, and data-governance policy.

WinUI 3 is delivered as part of the Windows App SDK; it isn't a separate UI framework that you add to a WinUI 3 project afterward.

## Choose an approach

| Requirement | Approach |
|---|---|
| Supported built-in on-device language capability | Windows AI language-model APIs |
| A custom model that must run locally | ONNX Runtime with an applicable execution provider |
| A larger general-purpose model, retrieval, or centralized governance | Azure AI service |
| OCR, speech, or other Windows capability | The applicable Windows platform API |

Don't select an on-device model from the processor name alone. Verify the current API's OS, Windows App SDK, model, and hardware requirements. Supported APIs can use different NPU or GPU configurations.

## Use the Windows AI language model

The Windows AI language-model APIs expose readiness states so that an app can detect whether a model is available and prepare it before generation.

```csharp
using Microsoft.Windows.AI;
using Microsoft.Windows.AI.Text;

if (LanguageModel.GetReadyState() == AIFeatureReadyState.NotReady)
{
    await LanguageModel.EnsureReadyAsync();
}

if (LanguageModel.GetReadyState() == AIFeatureReadyState.Ready)
{
    using LanguageModel model = await LanguageModel.CreateAsync();
    LanguageModelResponseResult result =
        await model.GenerateResponseAsync(prompt);

    if (result.Status == LanguageModelResponseStatus.Complete)
    {
        string response = result.Text;
    }
}
```

Handle every non-ready and incomplete state that the version you target can return. Show a clear status, disable unavailable commands, and provide a non-AI or approved cloud fallback where the workflow requires one.

> [!IMPORTANT]
> Windows is transitioning its built-in language model from Phi Silica to Aion Instruct. Phi Silica is a Limited Access Feature on stable releases, while Aion Instruct doesn't require a Limited Access Feature token. Review [Get started with Phi Silica](/windows/ai/apis/phi-silica) and the current Windows AI release guidance before choosing package capabilities or shipping a dependency on either model.

The sample associated with this article targets the Phi Silica API and demonstrates readiness checks and graceful failure. Treat it as a transition sample, not a promise that the same model is present on every supported Windows device.

## Use a cloud AI service

For a cloud model:

- Authenticate with an identity flow approved for your organization.
- Keep service credentials out of the client app.
- Apply timeouts, cancellation, bounded retries, and rate-limit handling.
- Tell users when data leaves the device.
- Apply the organization's retention, regional-processing, and content-safety requirements.
- Monitor quality and cost using production-like request sizes.

Prefer a service owned by your organization when the desktop client would otherwise need a privileged secret.

## Run a custom model with ONNX Runtime

ONNX Runtime can run a packaged model on the device:

```csharp
using var session = new InferenceSession("model.onnx");
using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results =
    session.Run(inputs);
```

Choose and test an execution provider for the target hardware. Include model files in your servicing, licensing, integrity, and update plans. Dispose sessions and result objects.

## Design the user experience

1. Keep the UI responsive and support cancellation for longer operations.
2. Show progress and identify AI-generated output.
3. Let users review consequential suggestions before applying them.
4. Preserve source data and make corrections possible.
5. Record enough context for diagnostics without logging sensitive prompts or output.
6. Evaluate the feature with representative business data and failure cases.

## Get the sample

The [local AI sample](https://github.com/GrantMeStrength/LOB/tree/main/WinUI-LOB-Samples/05-LocalAI) performs support-ticket summarization and classification and surfaces model availability in the UI.

:::image type="content" source="images/05-local-ai-dark.png" alt-text="The support-ticket AI sample running in the dark theme.":::

## Related content

- [Windows AI APIs overview](/windows/ai/)
- [Phi Silica documentation](/windows/ai/apis/phi-silica)
- [Windows AI troubleshooting](/windows/ai/apis/troubleshooting)
- [Azure OpenAI Service](/azure/ai-services/openai/)
- [ONNX Runtime](https://onnxruntime.ai/)
