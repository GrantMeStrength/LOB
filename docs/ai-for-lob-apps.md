---
title: Add AI capabilities to a line-of-business WinUI app
description: How to integrate on-device AI (Phi Silica, ONNX Runtime) and cloud AI (Azure OpenAI) into a WinUI 3 LOB app.
ms.topic: how-to
ms.date: 07/20/2026
author: GrantMeStrength
ms.author: jken
---

# Add AI capabilities to a line-of-business WinUI app

AI can enhance LOB apps with summarization, data extraction, classification, and natural-language search. Windows provides multiple paths depending on connectivity, privacy, and compute requirements.

---

## Decision guide

| Scenario | Recommended approach | Requires |
|----------|---------------------|----------|
| Summarize text, extract fields, classify records | **Phi Silica** (on-device SLM) | Copilot+ PC with NPU |
| General-purpose chat, RAG, code gen | **Azure OpenAI** (cloud) | Azure subscription + network |
| Custom vision/NLP models | **ONNX Runtime** (on-device) | Model file + DirectML |
| OCR, speech-to-text, translation | **Windows platform APIs** | Windows 11 |

---

## On-device AI with Phi Silica

Phi Silica is a small language model (SLM) that runs locally on Copilot+ PCs via the NPU. It requires no network and keeps data on-device.

```csharp
using Microsoft.Windows.AI.Generative;

var session = await LanguageModel.CreateAsync();
var result = await session.GenerateResponseAsync(
    "Summarize this customer complaint: " + complaintText);
string summary = result.Response;
```

> [!IMPORTANT]
> Phi Silica is only available on Copilot+ PCs (Snapdragon X, Intel Core Ultra, AMD Ryzen AI). Provide a graceful fallback for other hardware.

---

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

---

## On-device inference with ONNX Runtime

For custom models (classification, anomaly detection):

```csharp
using Microsoft.ML.OnnxRuntime;

var session = new InferenceSession("model.onnx");
var inputs = new List<NamedOnnxValue> { /* tensor inputs */ };
var results = session.Run(inputs);
```

Use DirectML for GPU acceleration on Windows.

---

## Windows platform AI APIs

- **OCR:** `Windows.Media.Ocr.OcrEngine`
- **Speech-to-text:** `Windows.Media.SpeechRecognition`
- **Text-to-speech:** `Windows.Media.SpeechSynthesis`
- **Translation:** (Requires Azure Translator or on-device model)

---

## Best practices for AI in LOB apps

1. **Run inference off the UI thread** — always use `async`/`await`.
2. **Provide feedback** — show a progress ring during inference.
3. **Handle hardware absence gracefully** — check `LanguageModel.IsAvailable()` before using Phi Silica.
4. **Respect data privacy** — on-device models keep data local; cloud models send data to Azure (ensure compliance).
5. **Cache results** — don't re-run inference for identical inputs.

---

## Related content

- [Windows AI APIs overview](https://learn.microsoft.com/windows/ai/)
- [Phi Silica documentation](https://learn.microsoft.com/windows/ai/apis/phi-silica)
- [Azure OpenAI Service](https://learn.microsoft.com/azure/ai-services/openai/)
- [ONNX Runtime](https://onnxruntime.ai/)
