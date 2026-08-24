using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BlogGenerator.Interfaces;

namespace BlogGenerator.BAL;

public class AIProviderService : IAIProviderService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AIProviderService> _logger;

    public AIProviderService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AIProviderService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GenerateBlogAsync(string prompt)
    {
        return await SendAIRequestAsync(
            $"Generate a high-quality blog post based on the following prompt:\n\n{prompt}");
    }

    public async Task<string> RewriteBlogAsync(
        string content,
        string instructions)
    {
        return await SendAIRequestAsync(
            $"Rewrite the following blog according to these instructions:\n\n" +
            $"Instructions: {instructions}\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> ExpandBlogAsync(
        string content,
        string? instructions)
    {
        var additionalInstructions =
            string.IsNullOrWhiteSpace(instructions)
                ? "Expand the blog with more useful details while maintaining its original tone and meaning."
                : instructions;

        return await SendAIRequestAsync(
            $"Expand the following blog.\n\n" +
            $"Instructions: {additionalInstructions}\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> ShortenBlogAsync(
        string content,
        string? instructions)
    {
        var additionalInstructions =
            string.IsNullOrWhiteSpace(instructions)
                ? "Shorten the blog while preserving its key ideas and important information."
                : instructions;

        return await SendAIRequestAsync(
            $"Shorten the following blog.\n\n" +
            $"Instructions: {additionalInstructions}\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> TranslateBlogAsync(
        string content,
        string language)
    {
        return await SendAIRequestAsync(
            $"Translate the following blog into {language}. " +
            $"Preserve the original meaning, tone, formatting and structure.\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> GenerateTagsAsync(string content)
    {
        return await SendAIRequestAsync(
            $"Generate 5 to 10 relevant SEO tags/keywords for the following blog. " +
            $"Return only the tags separated by commas.\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> GenerateSummaryAsync(string content)
    {
        return await SendAIRequestAsync(
            $"Generate a concise summary/excerpt for the following blog. " +
            $"Return only the summary.\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> GenerateImageAsync(string prompt)
    {
        return await GenerateImageRequestAsync(prompt);
    }

    private async Task<string> SendAIRequestAsync(string prompt)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        var model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is not configured.");
        }

        var requestBody = new
        {
            model = model,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content =
                        "You are a professional AI blog writing assistant."
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            temperature = 0.7
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.openai.com/v1/chat/completions");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "OpenAI API request failed. Status: {StatusCode}, Response: {Response}",
                response.StatusCode,
                responseContent);

            throw new HttpRequestException(
                $"AI provider request failed: {response.StatusCode}");
        }

        using var document =
            JsonDocument.Parse(responseContent);

        var result =
            document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

        if (string.IsNullOrWhiteSpace(result))
        {
            throw new InvalidOperationException(
                "AI provider returned an empty response.");
        }

        return result.Trim();
    }

    private async Task<string> GenerateImageRequestAsync(
        string prompt)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is not configured.");
        }

        var requestBody = new
        {
            model = "gpt-image-1",
            prompt = prompt,
            size = "1024x1024"
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.openai.com/v1/images/generations");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(request);

        var responseContent =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "OpenAI image generation failed. Status: {StatusCode}, Response: {Response}",
                response.StatusCode,
                responseContent);

            throw new HttpRequestException(
                $"AI image generation failed: {response.StatusCode}");
        }

        using var document =
            JsonDocument.Parse(responseContent);

        var imageUrl =
            document.RootElement
                .GetProperty("data")[0]
                .GetProperty("url")
                .GetString();

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new InvalidOperationException(
                "AI provider did not return an image URL.");
        }

        return imageUrl;
    }
}