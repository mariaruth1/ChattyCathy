using Fleck;

namespace helloworld.Services;

public class WebSocketWrapper(IWebSocketConnection connection)
{
    public IWebSocketConnection Connection { get; set; } = connection;
    public string Username { get; set; } = "";
}

public static class StateService
{
    public static Dictionary<Guid, WebSocketWrapper> Connections = new();
    public static Dictionary<int, HashSet<Guid>> Rooms = new();

    public static bool AddConnections(IWebSocketConnection ws)
    {
        return Connections.TryAdd(ws.ConnectionInfo.Id, new WebSocketWrapper(ws));
    }
    
    public static bool AddToRoom(IWebSocketConnection ws, int room)
    {
        if (!Rooms.ContainsKey(room))
        {
            Rooms.Add(room, new HashSet<Guid>());
        }
        return Rooms[room].Add(ws.ConnectionInfo.Id);
    }

    public static void BroadcastToRoom(int room, string message)
    {
       if (Rooms.TryGetValue(room, out var guids))
       {
           foreach (var guid in guids)
           {
               if (Connections.TryGetValue(guid, out var ws))
               {
                   ws.Connection.Send(message);
               }
           }
       }
    }
}



















