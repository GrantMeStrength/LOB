---
title: Add AI capabilities to a line-of-business WinUI app
description: How to integrate on-device AI (Phi Silica, ONNX Runtime) and cloud AI (Azure OpenAI) into a line-of-business WinUI 3 app.
ms.topic: how-to
ms.date: 07/27/2026
author: GrantMeStrength
ms.author: jken
---

# Add AI capabilities to a line-of-business WinUI app

AI can enhance LOB apps with summarization, data extraction, classification, and natural-language search. Windows provides multiple paths depending on connectivity, privacy, and compute requirements.

> [!NOTE]
> This article is a **first-draft stub** for SME review. Sections marked `> [!TODO]` require technical validation before publication.

## Prerequisites

- A WinUI 3 project built with the Windows App SDK
- Visual Studio with the Windows App SDK / WinUI workload
- For on-device Phi Silica: a Copilot+ PC with an NPU
- For Azure OpenAI: an Azure subscription and network connectivity

## Decision guide

| Scenario | Recommended approach | Requires |
|----------|---------------------|----------|
| Summarize text, extract fields, classify records | **Phi Silica** (on-device SLM) | Copilot+ PC with NPU |
| General-purpose chat, RAG, code gen | **Azure OpenAI** (cloud) | Azure subscription + network |
| Custom vision/NLP models | **ONNX Runtime** (on-device) | Model file + DirectML |
| OCR, speech-to-text, translation | **Windows platform APIs** | Windows 11 |

## On-device AI with Phi Silica

Phi Silica is a small language model (SLM) that runs locally on Copilot+ PCs via the NPU. It requires no network and keeps data on-device.

```csharp
using Microsoft.Windows.AI;         // AIFeatureReadyState
using Microsoft.Windows.AI.Text;    // LanguageModel

// Make sure the model is present and prepared (off the UI thread).
if (LanguageModel.GetReadyState() == AIFeatureReadyState.NotReady)
{
    await LanguageModel.EnsureReadyAsync();
}

using LanguageModel model = await LanguageModel.CreateAsync();
LanguageModelResponseResult result = await model.GenerateResponseAsync(
    "Summarize this customer complaint: " + complaintText);

if (result.Status == LanguageModelResponseStatus.Complete)
{
    string summary = result.Text;
}
```

> [!IMPORTANT]
> Phi Silica is only available on Copilot+ PCs (Snapdragon X, Intel Core Ultra, AMD Ryzen AI). Provide a graceful fallback for other hardware. Your package must declare the `systemAIModels` restricted capability.

> [!NOTE]
> On the **stable** Windows App SDK channel, the Phi Silica language model is a [Limited Access Feature](/uwp/api/windows.applicationmodel.limitedaccessfeatures) (`com.microsoft.windows.ai.languagemodel`). Third-party packages need a Microsoft-issued unlock token bound to their package identity, or `GenerateResponseAsync` fails with *"Access is denied."* For development and testing, the [experimental channel](/windows/apps/windows-app-sdk/experimental-channel) does **not** require a token. See the [API troubleshooting guide](/windows/ai/apis/troubleshooting) and the runnable [Sample 05 – LocalAI](https://github.com/GrantMeStrength/LOB/tree/main/WinUI-LOB-Samples/05-LocalAI), which demonstrates this pattern with graceful degradation when the gate blocks generation.

## Cloud AI with Azure OpenAI

For apps that need GPT-4o or other large models:

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;

var client = new AzureOpenAIClient(
    new Uri("https://your-resource.openai.azure.com/"),
    new DefaultAzureCredential());

var chatClient = client.GetChatClient("gpt-4o");
var response = await chatClient.CompleteChatAsync(
    new ChatMessage[] { new UserChatMessage(prompt) });
```

> [!TODO]
> Add guidance on token management, retry policies, and cost estimation for LOB scenarios (100–10K requests/day).

## On-device inference with ONNX Runtime

For custom models (classification, anomaly detection):

```csharp
using Microsoft.ML.OnnxRuntime;

var session = new InferenceSession("model.onnx");
var inputs = new List<NamedOnnxValue> { /* tensor inputs */ };
var results = session.Run(inputs);
```

Use DirectML for GPU acceleration on Windows.

## Windows platform AI APIs

- **OCR:** `Windows.Media.Ocr.OcrEngine`
- **Speech-to-text:** `Windows.Media.SpeechRecognition`
- **Text-to-speech:** `Windows.Media.SpeechSynthesis`
- **Translation:** (Requires Azure Translator or on-device model)

## Best practices for AI in LOB apps

1. **Run inference off the UI thread** — always use `async`/`await`.
2. **Provide feedback** — show a progress ring during inference.
3. **Handle hardware absence gracefully** — check `LanguageModel.GetReadyState()` returns `AIFeatureReadyState.Ready` before using Phi Silica, and handle the Limited Access Feature gate on the stable channel.
4. **Respect data privacy** — on-device models keep data local; cloud models send data to Azure (ensure compliance).
5. **Cache results** — don't re-run inference for identical inputs.

## Get the sample

The local AI sample is in the [LOB samples repo](https://github.com/GrantMeStrength/LOB) under the `05-LocalAI/` folder.

> [!NOTE]
> The sample repo URL may change if the repo is renamed or moved; this article will be updated if that happens.

## Related content

- [Windows AI APIs overview](/windows/ai/)
- [Phi Silica documentation](/windows/ai/apis/phi-silica)
- [Azure OpenAI Service](/azure/ai-services/openai/)
- [ONNX Runtime](https://onnxruntime.ai/)
