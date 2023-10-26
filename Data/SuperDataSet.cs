using System.Data;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels.ResponseModels;
using System.Diagnostics;

public class SuperDataSet
{
    public string? ErrorText { get; set; } = default;
    public string? SqlQueryText { get; set; } = default;
    // public Tokens? Tokens { get; set; } = 
    public UsageResponse? UsageResponse { get; set; } = default;
    public DataTable? DataTable { get; set; } = default;

    public SuperDataSet() { }

    public async Task<bool> Get(string naturalQuery)
    {
        OpenAi openAi = new("openai-api-key.txt");
        DatabaseHelper databaseHelper = new("Northwind.db");
        var chatMessages = ChatMessagesHelper.PrepareChatMessages(databaseHelper);

        chatMessages.Add(ChatMessage.FromUser(naturalQuery));

        ChatCompletionCreateResponse completionResult;
        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            completionResult = await openAi.GetCompletionResults(chatMessages);
            stopwatch.Stop();
            Console.WriteLine($"AI execution time: {stopwatch.Elapsed.TotalSeconds:F1} seconds");

            //  _logger.LogInformation("Query execution time: {Time}", stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            ErrorText = ex.Message;
            return false;
        }

        if (completionResult.Error != null)
        {
            ErrorText = completionResult.Error.Message;
            return false;
        };

        if (!completionResult.Successful)
        {
            ErrorText = "Completion was not successful!";
            return false;
        }
        UsageResponse = completionResult.Usage;

        SqlQueryText = completionResult.Choices.First().Message.Content;

        Console.WriteLine($"SQL query: {SqlQueryText}");

        if (SqlQueryText.ToUpper().StartsWith("SELECT"))
        {
            try
            {
                DataTable = databaseHelper.GetQueryResults(SqlQueryText);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return false;
            }
        }
        return true;
    }
}