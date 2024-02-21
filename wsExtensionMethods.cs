using System.Text.Json;
using Fleck;
using lib;

namespace chatty;

public static class wsExtensionMethods
{
   public static void SendDto<T>(this IWebSocketConnection socket, T dto) where T : BaseDto
    {
        socket.Send(JsonSerializer.Serialize(dto, new JsonSerializerOptions
            { PropertyNameCaseInsensitive = true }));
    }
}