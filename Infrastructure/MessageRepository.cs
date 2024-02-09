using System.Data;
using Dapper;
using Npgsql;

namespace helloworld.Infrastructure;

public class MessageRepository(NpgsqlDataSource db)
{

    //When you send a message,
    //you should insert the message into the database.
    public async Task<int> InsertMessage(string content, int room, string nickname)
    {
        var sql = @"INSERT INTO chatapp.message (""content"", ""room"", ""nickname"") VALUES (@content, @room, @nickname)";
        using (var conn = db.OpenConnection())
        {
            return await conn.ExecuteAsync(sql, new { content, room, nickname });

        }
    }
    
    //When you enter a room,
    //you should get recent (from the last 24 hours) messages from the database if
    //the entered "room ID" is the same as the room ID column value.
    
    public async Task<List<Message>> GetRecentMessages(int room)
    {
        var sql = @"SELECT * FROM chatapp.message WHERE ""room"" = @room AND ""timestamp"" >= @date";
        var date = DateTime.Now.AddDays(-1);
        await using var conn = db.OpenConnection();
        return (await conn.QueryAsync<Message>(sql, new { room, date })).ToList();
    }
    
}

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public int Room { get; set; }
    public string Nickname { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}