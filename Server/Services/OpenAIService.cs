using AIChefRemsey.Server.Models;
using AIChefRemsey.Shared;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using System.Collections.Generic;
using AIChefRemsey.Client.Pages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AIChefRemsey.Server.Services
{
    public class OpenAIService : IOpenAIAPI
    {

        private readonly IConfiguration _configuration;
        private static readonly string _baseURL = "https://api.openai.com/v1/";
        private static readonly HttpClient _httpClient = new();
        private readonly JsonSerializerOptions _jsonOptions;

        //Build the function objects
        // build the function object so the AI will return JSON formatted object
        private static ChatFunction.Parameter _recipeIdeaParameter = new()
        {
            // describes one Idea
            Type = "object",
            Required = new string[] { "index", "title", "description" },
            Properties = new
            {
                // provide a type and description for each property of the Idea model
                Index = new ChatFunction.Property
                {
                    Type = "number",
                    Description = "A unique identifier for this object",
                },
                Title = new ChatFunction.Property
                {
                    Type = "string",
                    Description = "The name for a recipe to create"
                },
                Description = new ChatFunction.Property
                {
                    Type = "string",
                    Description = "A description of the recipe"
                }
            }
        };

        private static ChatFunction _ideaFunction = new()
        {
            // describe the function we want an argument for from the AI
            Name = "CreateRecipe",
            // this description ensures we get 5 ideas back
            Description = "Generates recipes for each idea in an array of five recipe ideas",
            Parameters = new
            {
                // OpenAI requires that the parameters are an object, so we need to wrap our array in an object
                Type = "object",
                Properties = new
                {
                    Data = new // our array will come back in an object in the Data property
                    {
                        Type = "array",
                        // further ensures the AI will create 5 recipe ideas
                        Description = "An array of five recipe ideas",
                        Items = _recipeIdeaParameter
                    }
                }
            }
        };

        public OpenAIService(IConfiguration configuration)
        {
            _configuration=configuration;
            var apiKey = _configuration["OpenAI:OpenAIKey"] ?? Environment.GetEnvironmentVariable("OpenAiKey"); //Local or Host
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", apiKey);

            _jsonOptions = new()
            {
                PropertyNameCaseInsensitive= true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        private static ChatFunction.Parameter _recipeParameter = new()
        {
            Type = "object",
            Description = "The recipe to display",
            Required = new[] { "title", "ingredients", "instructions", "summary" },
            Properties = new
            {
                Title = new
                {
                    Type = "string",
                    Description = "The title of the recipe to display",
                },
                Ingredients = new
                {
                    Type = "array",
                    Description = "An array of all the ingredients mentioned in the recipe instructions",
                    Items = new { Type = "string" }
                },
                Instructions = new
                {
                    Type = "array",
                    Description = "An array of each step for cooking this recipe",
                    Items = new { Type = "string" }
                },
                Summary = new
                {
                    Type = "string",
                    Description = "A summary description of what this recipe creates",
                },
            },
        };

        private static ChatFunction _recipeFunction = new()
        {
            Name = "DisplayRecipe",
            Description = "Displays the recipe from the parameter to the user",
            Parameters = new
            {
                Type = "object",
                Properties = new
                {
                    Data = _recipeParameter
                },
            }
        };

      

        async Task<List<Idee>> IOpenAIAPI.MaakReceptenIdee(string maaltijdmoment, List<string> ingredientenLijst)
        {
            {
                string url = $"{_baseURL}chat/completions";
                string systemPrompt = "Je bent een world-renwed chef. I will send you a list of ingredients and a meal time. You will respond with 5 ideas for dishes.";
                string userPrompt = "";
                string ingredientenPrompt = "";

                string ingrediënten = string.Join(",", ingredientenLijst);

                if (string.IsNullOrEmpty(ingrediënten))
                {
                    ingredientenPrompt = "Suggest some ingredients for me";
                }
                else
                {
                    ingredientenPrompt = $"I have {ingrediënten}";
                }

                userPrompt = $"The meal i want to cook is {maaltijdmoment}. {ingredientenPrompt}";
                ChatMessage systemMessage = new()
                {
                    Role = "system",
                    Content = $"{systemPrompt}"

                };

                ChatMessage userMessage = new()
                {
                    Role = "user",
                    Content = $"{userPrompt}"
                };

                ChatRequest request = new()
                {
                    Model = "gpt-3.5-turbo-0613",
                    Messages = new[] { systemMessage, userMessage },
                    Functions = new[] { _ideaFunction },
                    FunctionCall = new { Name = _ideaFunction.Name }
                };

                HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync(url, request, _jsonOptions);

                ChatResponse? response = await httpResponse.Content.ReadFromJsonAsync<ChatResponse>();

                ChatFunctionResponse? functionResponse = response.Choices?
                            .FirstOrDefault(m => m.Message?.FunctionCall is not null)?
                            .Message?
                            .FunctionCall;

                Result<List<Idee>> ideeResultaten = new();

                if (functionResponse?.Arguments is not null)
                {
                    try
                    {
                        ideeResultaten = JsonSerializer.Deserialize<Result<List<Idee>>>(functionResponse.Arguments, _jsonOptions);
                    }
                    catch (Exception ex)
                    {

                        ideeResultaten = new()
                        {
                            Exception = ex,
                            ErrorMessage = await httpResponse.Content.ReadAsStringAsync()
                        };
                    }

                }
                return ideeResultaten?.Data ?? new List<Idee>();
            }
        }

        public async Task<Recept?> MaakRecept(string title, List<string> ingredientenLijst)
        {
            string url = $"{_baseURL}chat/completions";
            string systemPrompt = "Your are a world-renowned chef. Create the recipe with ingredients, instructions and a summary";
            string userPrompt = $"Create a {title} recipe";

            ChatMessage userMessage = new()
            {
                Role="user",
                Content = $"{systemPrompt} {userPrompt}"
            };

            ChatRequest chatrequest = new()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = new[] { userMessage },
                Functions = new[] { _recipeFunction },
                FunctionCall = new { Name = _recipeFunction.Name }
            };
            HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync(url, chatrequest, _jsonOptions);
            ChatResponse? response = await httpResponse.Content.ReadFromJsonAsync<ChatResponse?>();

            ChatFunctionResponse? functionResponse = response?.Choices?
                                                              .FirstOrDefault(m => m.Message?.FunctionCall is not null)?
                                                              .Message?
                                                              .FunctionCall;

            Result<Recept>? recept = new();
            if (functionResponse?.Arguments is not null)
            {
                try
                {
                    recept = JsonSerializer.Deserialize<Result<Recept>>(functionResponse.Arguments, _jsonOptions);
                }
                catch (Exception ex)
                {
                    recept = new()
                    {
                        Exception = ex,
                        ErrorMessage = await httpResponse.Content.ReadAsStringAsync()
                    };
                }
            }

            return recept?.Data;
        }

        public async Task<ReceptAfbeelding?> MaakReceptAfbeelding(string receptTitle)
        {
            string url = $"{_baseURL}images/generations";
            string userPrompt = $"Create a restaurant product shot for {receptTitle}";

            ImageGenerationRequest request = new()
            {
                Prompt = userPrompt,
            };

            HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync(url, request, _jsonOptions);

            ReceptAfbeelding? receptAfbeelding = null;
            try
            {
                httpResponse.EnsureSuccessStatusCode();
                receptAfbeelding = await httpResponse.Content.ReadFromJsonAsync<ReceptAfbeelding>(); 

            }
            catch
            {
                Console.WriteLine("Error: Kan de afbeelding niet ophalen");

            }
            return receptAfbeelding;
        }
    }
}

