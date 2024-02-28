using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Fleck;
using chatty.core;
using chatty.Infrastructure;
using chatty.Services;
using lib;
using System.Net.Http;
using System.Net.Http.Headers;

namespace chatty.ClientEvents;

public class ClientWantsToBroadcastToRoomDto : BaseDto
{
    public string Message { get; set; } = null!;
    public int RoomId { get; set; }
}

public class ClientWantsToBroadcastToRoom(MessageService messageService) : BaseEventHandler<ClientWantsToBroadcastToRoomDto>
{
    public override async Task Handle(ClientWantsToBroadcastToRoomDto dto, IWebSocketConnection socket)
    {
        await isMessageToxic(dto.Message);
        var date = DateTime.Now;
        var insertedMessage = new ChatMessage();
        try
        {
            insertedMessage = await messageService.InsertMessage(dto.Message, dto.RoomId,
                StateService.Connections[socket.ConnectionInfo.Id].Username, date);
        }
        catch (Exception)
        {
            throw new Exception("Failed to send message.");
        }

        var message = new ServerBroadcastsMessageToRoom();
        if (insertedMessage != null)
        {
            message = new ServerBroadcastsMessageToRoom()
            {
                Message = insertedMessage.Content,
                Username = insertedMessage.Nickname,
                Timestamp = insertedMessage.Timestamp,
            };
        }

        StateService.BroadcastToRoom(dto.RoomId, message);
    }

    private async Task isMessageToxic(string message)
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, $"https://badcontentbad.cognitiveservices.azure.com/contentsafety/text:analyze?api-version=2023-10-01");

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("CONTENT_SAFETY_KEY"));

        request.Content = new StringContent(JsonSerializer.Serialize(new ContentSafetyDto { text = message }));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var contentSafetyResponse = JsonSerializer.Deserialize<ContentSafetyResponse>(responseBody);
        var isToxic = contentSafetyResponse.categoriesAnalysis.Count(x => x.severity > 1) > 0;
        if (isToxic)
        {
            throw new ValidationException("You are toxic.");
        }
    }
}

public class ServerBroadcastsMessageToRoom : BaseDto
{
    public string Message { get; set; }
    public string Username { get; set; }
    public string Timestamp { get; set; }
}

public class ContentSafetyDto
{
    public string text { get; set; }
    public List<string> categories = new List<string> { "Hate", "Violence" };
    public string outputType = "FourSeverityLevels";
}

public class CategoriesAnalysis
{
    public string category { get; set; }
    public int severity { get; set; }
}

public class ContentSafetyResponse
{
    public List<CategoriesAnalysis> categoriesAnalysis { get; set; }
}