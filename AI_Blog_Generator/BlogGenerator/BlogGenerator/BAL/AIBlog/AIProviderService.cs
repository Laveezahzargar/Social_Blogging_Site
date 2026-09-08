using System.Diagnostics;
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

        _logger.LogInformation(
            "AIProviderService initialized. BaseAddress: {BaseAddress}",
            _httpClient.BaseAddress);
    }

    public async Task<string> GenerateBlogAsync(string prompt)
    {
        _logger.LogInformation(
            "GenerateBlogAsync started. PromptLength: {PromptLength}",
            prompt?.Length ?? 0);

        try
        {
            var result = await SendAIRequestAsync(
                $"Generate a high-quality blog post based on the following prompt:\n\n{prompt}");

            _logger.LogInformation(
                "GenerateBlogAsync completed successfully. ResultLength: {ResultLength}",
                result?.Length ?? 0);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "GenerateBlogAsync failed.");

            throw;
        }
    }

    public async Task<string> RewriteBlogAsync(
        string content,
        string instructions)
    {
        _logger.LogInformation(
            "RewriteBlogAsync started. ContentLength: {ContentLength}, InstructionsLength: {InstructionsLength}",
            content?.Length ?? 0,
            instructions?.Length ?? 0);

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

        _logger.LogInformation(
            "ExpandBlogAsync started. ContentLength: {ContentLength}, InstructionsLength: {InstructionsLength}",
            content?.Length ?? 0,
            additionalInstructions.Length);

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

        _logger.LogInformation(
            "ShortenBlogAsync started. ContentLength: {ContentLength}, InstructionsLength: {InstructionsLength}",
            content?.Length ?? 0,
            additionalInstructions.Length);

        return await SendAIRequestAsync(
            $"Shorten the following blog.\n\n" +
            $"Instructions: {additionalInstructions}\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> TranslateBlogAsync(
        string content,
        string language)
    {
        _logger.LogInformation(
            "TranslateBlogAsync started. ContentLength: {ContentLength}, Language: {Language}",
            content?.Length ?? 0,
            language);

        return await SendAIRequestAsync(
            $"Translate the following blog into {language}. " +
            $"Preserve the original meaning, tone, formatting and structure.\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> GenerateTagsAsync(string content)
    {
        _logger.LogInformation(
            "GenerateTagsAsync started. ContentLength: {ContentLength}",
            content?.Length ?? 0);

        return await SendAIRequestAsync(
            $"Generate 5 to 10 relevant SEO tags/keywords for the following blog. " +
            $"Return only the tags separated by commas.\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> GenerateSummaryAsync(string content)
    {
        _logger.LogInformation(
            "GenerateSummaryAsync started. ContentLength: {ContentLength}",
            content?.Length ?? 0);

        return await SendAIRequestAsync(
            $"Generate a concise summary/excerpt for the following blog. " +
            $"Return only the summary.\n\n" +
            $"Blog:\n{content}");
    }

    public async Task<string> GenerateImageAsync(string prompt)
    {
        _logger.LogInformation(
            "GenerateImageAsync started. PromptLength: {PromptLength}",
            prompt?.Length ?? 0);

        return await GenerateImageRequestAsync(prompt);
    }

    private async Task<string> SendAIRequestAsync(string prompt)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "========== GEMINI REQUEST START ==========");

        _logger.LogInformation(
            "Gemini prompt length: {PromptLength}",
            prompt?.Length ?? 0);

        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"];

        _logger.LogInformation(
            "Gemini configuration loaded. ApiKeyConfigured: {ApiKeyConfigured}, Model: {Model}",
            !string.IsNullOrWhiteSpace(apiKey),
            model);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogError(
                "Gemini API key is missing.");

            throw new InvalidOperationException(
                "Gemini API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            _logger.LogError(
                "Gemini model is missing.");

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

        _logger.LogInformation(
            "Gemini request body serialized. JsonLength: {JsonLength}",
            json.Length);

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent";

        _logger.LogInformation(
            "Gemini endpoint: {Endpoint}",
            url);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            url);

        request.Headers.Add(
            "x-goog-api-key",
            apiKey);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        _logger.LogInformation(
            "Sending HTTP request to Gemini...");

        try
        {
            var response = await _httpClient.SendAsync(request);

            stopwatch.Stop();

            _logger.LogInformation(
                "Gemini HTTP request completed. " +
                "StatusCode: {StatusCode}, " +
                "Success: {Success}, " +
                "ElapsedMs: {ElapsedMs}",
                (int)response.StatusCode,
                response.IsSuccessStatusCode,
                stopwatch.ElapsedMilliseconds);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            _logger.LogInformation(
                "Gemini response received. ResponseLength: {ResponseLength}",
                responseContent.Length);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Gemini API request FAILED. " +
                    "StatusCode: {StatusCode}, " +
                    "Reason: {ReasonPhrase}, " +
                    "Response: {Response}",
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    responseContent);

                throw new HttpRequestException(
                    $"Gemini API request failed: {response.StatusCode}. " +
                    $"Details: {responseContent}");
            }

            _logger.LogInformation(
                "Parsing Gemini JSON response...");

            using var document =
                JsonDocument.Parse(responseContent);

            var candidates =
                document.RootElement.GetProperty("candidates");

            _logger.LogInformation(
                "Gemini returned {CandidateCount} candidate(s).",
                candidates.GetArrayLength());

            var outputText = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(outputText))
            {
                _logger.LogError(
                    "Gemini returned an empty response.");

                throw new InvalidOperationException(
                    "Gemini returned an empty response.");
            }

            _logger.LogInformation(
                "Gemini response parsed successfully. " +
                "OutputLength: {OutputLength}, " +
                "TotalElapsedMs: {ElapsedMs}",
                outputText.Length,
                stopwatch.ElapsedMilliseconds);

            _logger.LogInformation(
                "========== GEMINI REQUEST SUCCESS ==========");

            return outputText.Trim();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "========== GEMINI REQUEST EXCEPTION ==========" +
                " ElapsedMs: {ElapsedMs}",
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }

    private async Task<string> GenerateImageRequestAsync(string prompt)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "========== CLOUDFLARE IMAGE REQUEST START ==========");

        var accountId = _configuration["Cloudflare:AccountId"];
        var apiToken = _configuration["Cloudflare:ApiToken"];

        _logger.LogInformation(
            "Cloudflare configuration loaded. " +
            "AccountIdConfigured: {AccountIdConfigured}, " +
            "ApiTokenConfigured: {ApiTokenConfigured}",
            !string.IsNullOrWhiteSpace(accountId),
            !string.IsNullOrWhiteSpace(apiToken));

        if (string.IsNullOrWhiteSpace(accountId))
        {
            _logger.LogError(
                "Cloudflare Account ID is missing.");

            throw new InvalidOperationException(
                "Cloudflare Account ID is not configured.");
        }

        if (string.IsNullOrWhiteSpace(apiToken))
        {
            _logger.LogError(
                "Cloudflare API token is missing.");

            throw new InvalidOperationException(
                "Cloudflare API token is not configured.");
        }

        var endpoint =
            $"https://api.cloudflare.com/client/v4/accounts/" +
            $"{accountId}/ai/run/@cf/black-forest-labs/flux-1-schnell";

        _logger.LogInformation(
            "Cloudflare endpoint: {Endpoint}",
            endpoint);

        var requestBody = new
        {
            prompt = prompt,
            steps = 4
        };

        var json = JsonSerializer.Serialize(requestBody);

        _logger.LogInformation(
            "Cloudflare request serialized. JsonLength: {JsonLength}",
            json.Length);

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

        _logger.LogInformation(
            "Sending HTTP request to Cloudflare...");

        try
        {
            var response = await _httpClient.SendAsync(request);

            stopwatch.Stop();

            _logger.LogInformation(
                "Cloudflare HTTP request completed. " +
                "StatusCode: {StatusCode}, " +
                "Success: {Success}, " +
                "ElapsedMs: {ElapsedMs}",
                (int)response.StatusCode,
                response.IsSuccessStatusCode,
                stopwatch.ElapsedMilliseconds);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            _logger.LogInformation(
                "Cloudflare response received. ResponseLength: {ResponseLength}",
                responseContent.Length);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Cloudflare image generation FAILED. " +
                    "StatusCode: {StatusCode}, " +
                    "Reason: {ReasonPhrase}, " +
                    "Response: {Response}",
                    (int)response.StatusCode,
                    response.ReasonPhrase,
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
                _logger.LogError(
                    "Cloudflare did not return an image.");

                throw new InvalidOperationException(
                    "Cloudflare did not return an image.");
            }

            _logger.LogInformation(
                "Cloudflare image generated successfully. " +
                "Base64Length: {Base64Length}, " +
                "ElapsedMs: {ElapsedMs}",
                imageBase64.Length,
                stopwatch.ElapsedMilliseconds);

            _logger.LogInformation(
                "========== CLOUDFLARE IMAGE REQUEST SUCCESS ==========");

            return imageBase64;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "========== CLOUDFLARE REQUEST EXCEPTION ==========" +
                " ElapsedMs: {ElapsedMs}",
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}