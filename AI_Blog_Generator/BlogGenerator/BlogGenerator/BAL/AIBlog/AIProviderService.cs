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
        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Gemini API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new InvalidOperationException(
                "Gemini model is not configured.");
        }

        var requestBody = new
        {
            contents = new[]
            {
            new
            {
                parts = new[]
                {
                    new
                    {
                        text = prompt
                    }
                }
            }
        }
        };

        var json = JsonSerializer.Serialize(requestBody);

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent";

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            url);

        request.Headers.Add("x-goog-api-key", apiKey);

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
                "Gemini API request failed. Status: {StatusCode}, Response: {Response}",
                response.StatusCode,
                responseContent);

            throw new HttpRequestException(
                $"Gemini API request failed: {response.StatusCode}. " +
                $"Details: {responseContent}");
        }

        using var document =
            JsonDocument.Parse(responseContent);

        var outputText = document.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(outputText))
        {
            throw new InvalidOperationException(
                "Gemini returned an empty response.");
        }

        return outputText.Trim();
    }

    private async Task<string> GenerateImageRequestAsync(string prompt)
    {
        var accountId = _configuration["Cloudflare:AccountId"];
        var apiToken = _configuration["Cloudflare:ApiToken"];

        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new InvalidOperationException(
                "Cloudflare Account ID is not configured.");
        }

        if (string.IsNullOrWhiteSpace(apiToken))
        {
            throw new InvalidOperationException(
                "Cloudflare API token is not configured.");
        }

        var endpoint =
            $"https://api.cloudflare.com/client/v4/accounts/" +
            $"{accountId}/ai/run/@cf/black-forest-labs/flux-1-schnell";

        var requestBody = new
        {
            prompt = prompt,
            steps = 4
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            endpoint);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                apiToken);

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
                "Cloudflare image generation failed. " +
                "Status: {StatusCode}, Response: {Response}",
                response.StatusCode,
                responseContent);

            throw new HttpRequestException(
                $"Cloudflare image generation failed: " +
                $"{response.StatusCode}. " +
                $"Details: {responseContent}");
        }

        using var document =
            JsonDocument.Parse(responseContent);

        var imageBase64 =
            document.RootElement
                .GetProperty("result")
                .GetProperty("image")
                .GetString();

        if (string.IsNullOrWhiteSpace(imageBase64))
        {
            throw new InvalidOperationException(
                "Cloudflare did not return an image.");
        }

        return imageBase64;
    }
}