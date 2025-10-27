// LLMService.cs
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace snakegame
{
    public class DeepSeekRequest
    {
        public string Model { get; set; } = "deepseek-chat";
        public DeepSeekMessage[] Messages { get; set; } = Array.Empty<DeepSeekMessage>();
        public double Temperature { get; set; } = 0.7;
    }

    public class DeepSeekMessage
    {
        public string Role { get; set; } = "user";
        public string Content { get; set; } = "";
    }

    public class DeepSeekResponse
    {
        public DeepSeekChoice[] Choices { get; set; } = Array.Empty<DeepSeekChoice>();
    }

    public class DeepSeekChoice
    {
        public DeepSeekMessage Message { get; set; } = new();
    }

    public class LLMService
    {
        // 替换为你的 DeepSeek API Key（从 https://platform.deepseek.com 获取）
        private readonly string apiKey = "sk-44fa664c8bf844b6a98fbcbc54770684";
        private readonly string endpoint = "https://api.deepseek.com/v1/chat/completions";

        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };

        public async Task<string> GetResponseAsync(string prompt)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            client.DefaultRequestHeaders.Add("User-Agent", "SnakeGame/1.0");

            var requestBody = new DeepSeekRequest
            {
                Messages = new[] { new DeepSeekMessage { Role = "user", Content = prompt } },
                Temperature = 0.5 // 更稳定输出
            };

            string json = JsonSerializer.Serialize(requestBody, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(endpoint, content);
                string responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"DeepSeek API 错误 ({response.StatusCode}): {responseText}";
                }

                // 使用 JsonSerializer 反序列化响应（也可用 JsonDocument，这里统一用类型）
                var resp = JsonSerializer.Deserialize<DeepSeekResponse>(responseText, jsonOptions);
                return resp?.Choices.Length > 0
                    ? resp.Choices[0].Message.Content.Trim()
                    : "DeepSeek 未返回有效内容";
            }
            catch (Exception ex)
            {
                return $"调用 DeepSeek 失败: {ex.Message}";
            }
        }
    }
}