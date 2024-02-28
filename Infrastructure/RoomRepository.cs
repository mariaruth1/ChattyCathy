using Dapper;
using Npgsql;

namespace chatty.Infrastructure;

public class RoomRepository(NpgsqlDataSource db)
{
    public async Task<Room?> InsertRoom(string name)
    {
        var sql = @"INSERT INTO chatapp.room (""room_name"") VALUES (@name) RETURNING room_id AS Id, room_name AS Name";
        await using var conn = db.OpenConnection();
        return await conn.QueryFirstOrDefaultAsync<Room>(sql, new { name });
    }
    
    public async Task<List<Room>> GetRooms()
    {
        var sql = @"SELECT * FROM chatapp.room";
        await using var conn = db.OpenConnection();
        return (await conn.QueryAsync<Room>(sql)).ToList();
    }
    
    public async Task DeleteRoom(int id)
    {
        var sql = @"DELETE FROM chatapp.room WHERE room_id = @id";
        await using var conn = db.OpenConnection();
        await conn.ExecuteAsync(sql, new { id });
    }
}

public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }