using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextGeneration;
using NetTopologySuite.Utilities;
using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Http;

namespace AutoDLL;

public static class ChatService
{
    public static async Task<string> GetChatDocumentFiLe(ChatInfo info, string userSpeak)
    {
        var openAIEmbeddingConfig = new OpenAIConfig
        {
            Endpoint = info.ApiUrl,
            EmbeddingModel = info.Embedding,
            EmbeddingModelMaxTokenTotal = 8191,
            APIKey = info.ApiKey
        };
        var lmStudioConfig = new OpenAIConfig
        {
            Endpoint = info.ApiUrl,
            TextModel = info.Model,
            TextModelMaxTokenTotal = 8191,
            APIKey = info.ApiKey
        };
        var memory = new KernelMemoryBuilder()
            .WithOpenAITextEmbeddingGeneration(openAIEmbeddingConfig)
            .WithOpenAITextGeneration(lmStudioConfig)
            .Build();

        // Import system message if it exists
        if (!string.IsNullOrWhiteSpace(info.System))
        {
            await memory.ImportTextAsync(info.System);
        }

        // Import document if it exists
        if (!string.IsNullOrWhiteSpace(info.Document))
        {
            await ImportDocument(memory, info.Document);
        }

        // Generate an answer
        var answer = await memory.AskAsync(userSpeak);
        return answer.Result;
    }

    private static async Task ImportDocument(IKernelMemory memory, string documentPath)
    {
        try
        {
            if (Uri.TryCreate(documentPath, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                // If it's a valid URL, import as a web page
                await memory.ImportWebPageAsync(documentPath);
            }
            else
            {
                // If it's a file, determine the type based on extension
                string extension = Path.GetExtension(documentPath).ToLower();
                switch (extension)
                {
                    case ".txt":
                    case ".md":
                    case ".js":
                    case ".json":
                    case ".cs":
                    case ".cpp":
                    case ".c":
                    case ".py":
                    case ".html":
                    case ".css":
                    case ".xml":
                    case ".yaml":
                    case ".yml":
                    case ".ini":
                    case ".config":
                    case ".log":
                        await memory.ImportTextAsync(File.ReadAllText(documentPath));
                        break;
                    case ".xlsx":
                    case ".xls":
                    case ".pdf":
                    case ".docx":
                        await memory.ImportDocumentAsync(new Document().AddFile(documentPath));
                        break;
                    default:
                        throw new ArgumentException($"Unsupported file type: {extension}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing document: {ex.Message}");
            // 可以选择将错误记录到日志文件或其他日志系统
        }
    }

    public static async Task<string> GetChatDocumentText(ChatInfo info, string userSpeak)
    {
        var openAIEmbeddingConfig = new OpenAIConfig
        {
            Endpoint = info.ApiUrl,
            EmbeddingModel = info.Embedding,
            EmbeddingModelMaxTokenTotal = 8191,
            APIKey = info.ApiKey
        };

        var lmStudioConfig = new OpenAIConfig
        {
            Endpoint = info.ApiUrl,
            TextModel = info.Model,
            TextModelMaxTokenTotal = 8191,
            APIKey = info.ApiKey
        };

        var memory = new KernelMemoryBuilder()
            .WithOpenAITextEmbeddingGeneration(openAIEmbeddingConfig)
            .WithOpenAITextGeneration(lmStudioConfig)
            .Build();

        // Import system message if it exists
        if (!string.IsNullOrWhiteSpace(info.System))
        {
            await memory.ImportTextAsync(info.System);
        }

        // Import document if it exists
        if (!string.IsNullOrWhiteSpace(info.Document))
        {
            await memory.ImportTextAsync(info.Document);
        }

        // Generate an answer
        var answer = await memory.AskAsync(userSpeak);

        return answer.Result;
    }

    public static async Task<string> GetChatWebPage(ChatInfo info, string userSpeak)
    {
        var openAIEmbeddingConfig = new OpenAIConfig
        {
            Endpoint = info.ApiUrl,
            EmbeddingModel = info.Embedding,
            EmbeddingModelMaxTokenTotal = 8191,
            APIKey = info.ApiKey
        };
        var lmStudioConfig = new OpenAIConfig
        {
            Endpoint = info.ApiUrl,
            TextModel = info.Model,
            TextModelMaxTokenTotal = 8191,
            APIKey = info.ApiKey
        };
        var memory = new KernelMemoryBuilder()
            .WithOpenAITextEmbeddingGeneration(openAIEmbeddingConfig)
            .WithOpenAITextGeneration(lmStudioConfig)
            .Build();

        // Import system message if it exists
        if (!string.IsNullOrWhiteSpace(info.System))
        {
            await memory.ImportTextAsync(info.System);
        }

        // Import web page
        await ImportWebPage(memory, info.WebPageUrl);

        // Generate an answer
        var answer = await memory.AskAsync(userSpeak);
        return answer.Result;
    }

    private static async Task ImportWebPage(IKernelMemory memory, string webPageUrl)
    {
        try
        {
            if (Uri.TryCreate(webPageUrl, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                await memory.ImportWebPageAsync(webPageUrl);
            }
            else
            {
                throw new ArgumentException($"Invalid URL: {webPageUrl}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing web page: {ex.Message}");
        }
    }
}
