using Logbackend.Models;
using LogBackend.Utilities;
using LogBackend.Repositories;
using Dapper;
using LogBackend.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Logbackend.Repositories;

public interface ILogRepository
{
    Task<Log> Create(Log Item);
    Task<bool> Update(Log Item);
    Task<bool> DeleteLog(long Id);
    Task<List<Log>> GetAllLog(int Limit, int PageNumber);
    Task<Log> GetById(long Id);
    Task<List<Log>> GetTags(long id);
}


public class LogRepository : BaseRepository, ILogRepository
{
    // private readonly IMemoryCache _memoryCache;

    public LogRepository(IConfiguration configuration) : base(configuration)
    {
        // _memoryCache = memoryCache;

    }

    public async Task<Log> Create(Log Item)
    {
        var query = $@"INSERT INTO {TableNames.log} (title, description, stack_trace ) VALUES (@Title, @Description, @StackTrace) RETURNING *";
        using (var connection = NewConnection)
        {
            var res = await connection.QuerySingleOrDefaultAsync<Log>(query, Item);
            return res;
        }
    }


    public async Task<List<Log>> GetAllLog(int Limit, int PageNumber)
    {
        // var query = $@"SELECT * FROM {TableNames.log} ";
        //     using (var con = NewConnection)
        //     return (await con.QueryAsync<Log>(query)).AsList();

        // var Postmem = GetLog<List<Log>>(key: $"Log {Limit} : {PageNumber}");
        // if (Postmem is null)
        // {

        var query = $@"SELECT * FROM {TableNames.log}  ORDER BY Id OFFSET @PageNumber
	                LIMIT @Limit";

        using (var con = NewConnection)

            return (await con.QueryAsync<Log>(query, new { @PageNumber = (PageNumber - 1) * Limit, Limit })).AsList();
        //   _memoryCache.Set(key:"Log",Postmem, TimeSpan.FromMinutes(value:1));


        // return (await con.QueryAsync<Log>(query)).AsList();
        // }
        // return Postmem;
    }

    public async Task<Log> GetById(long Id)
    {
        var query = $@"SELECT * FROM {TableNames.log} WHERE id = @Id";
        using (var connection = NewConnection)
        {
            var Res = await connection.QuerySingleOrDefaultAsync<Log>(query, new { Id });
            return Res;
        }
    }

    // public async Task<bool> Update(Log Item)
    // {
    //     var query = $@"UPDATE ""{TableNames.log}"" SET description = @Description WHERE id = @Id";

    //     using (var con = NewConnection)

    //       return (await con.ExecuteAsync(query, Item)) >0;
    // }

    public async Task<bool> Update(Log Item)
    {
        var query = $@"UPDATE ""{TableNames.log}"" SET description = @Description
         WHERE id = @Id";

        using (var con = NewConnection)
            return (await con.ExecuteAsync(query, Item)) > 0;
    }
    public async Task<bool> DeleteLog(long Id)
    {
        var query = $@"DELETE FROM {TableNames.log} WHERE id = @Id";

        using (var con = NewConnection)
            return (await con.ExecuteAsync(query, new { Id }) > 0);
    }

    public async Task<List<Log>> GetTags(long Id)
    {
        var query = $@"SELECT * FROM ""{TableNames.log_tag}"" lt
        Left Join ""{TableNames.log}"" l ON l.id = lt.log_id WHERE lt.tag_id = @Id";
        using (var con = NewConnection)
        {
            return (await con.QueryAsync<Log>(query, new { Id })).AsList();
        }
    }
}