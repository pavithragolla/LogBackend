using Logbackend.Models;
using LogBackend.Utilities;
using LogBackend.Repositories;
using Dapper;
using LogBackend.Models;
using Microsoft.Extensions.Caching.Memory;
using LogBackend.DTOs;

namespace Logbackend.Repositories;

public interface ILogRepository
{
    Task<Log> Create(Log Item);
    Task<bool> Update(Log Item);
    Task<bool> DeleteLog(long Id);
    // Task<List<Log>> GetAllLog(int Limit, int PageNumber);
    Task<List<Log>> GetAllLog(DateFilterDTO dateFilter);
    Task<Log> GetById(long id);
    Task<List<Tag>> GetTags(long id);
    Task seenId(int Id, long id);
    Task<List<TagTypeDTO>> GetLogTagTypesById(long id);

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


    // public async Task<List<Log>> GetAllLog(int Limit, int PageNumber)
    // {

    //     var query = $@"SELECT * FROM {TableNames.log}  ORDER BY Id OFFSET @PageNumber
    //                 LIMIT @Limit";

    //     using (var con = NewConnection)

    //         return (await con.QueryAsync<Log>(query, new { @PageNumber = (PageNumber - 1) * Limit, Limit })).AsList();





    // }

    public async Task<List<Log>> GetAllLog(DateFilterDTO dateFilter)
    {
        List<Log> res;



        var query = $@"SELECT * FROM ""{TableNames.log}"" ";



        if (dateFilter is not null && (dateFilter.FromDate.HasValue || dateFilter.ToDate.HasValue))
        {
            if (dateFilter.FromDate is null) dateFilter.FromDate = DateTime.MinValue;
            if (dateFilter.ToDate is null) dateFilter.ToDate = DateTime.Now;
            query += "WHERE created_at BETWEEN  @FromDate AND  @ToDate";
        }

        var paramsObj = new
        {

            FromDate = dateFilter?.FromDate,
            ToDate = dateFilter?.ToDate,

        };
        using (var con = NewConnection)
        {
            res = (await con.QueryAsync<Log>(query, paramsObj)).AsList();
        }
        return (res);
    }

    public async Task<Log> GetById(long id)
    {
        var query = $@"SELECT * FROM {TableNames.log} WHERE id = @Id";
        using (var connection = NewConnection)
        {
            var Res = await connection.QuerySingleOrDefaultAsync<Log>(query, new { id });
            return Res;
        }


    }



    public async Task<bool> Update(Log Item)
    {
        var query = $@"UPDATE ""{TableNames.log}"" SET  description = @Description, read_status = @ReadStatus, updated_by_user_id = @UpdatedByUserId, updated_at = now() WHERE id = @Id";
        using (var connection = NewConnection)
        {
            return (await connection.ExecuteAsync(query, Item)) > 0;
        }
        // var TagDelete = $@"DELETE FROM ""{TableNames.log_tag}"" WHERE log_id = @Id";
        //   var TagInsert = $@"INSERT INTO ""{TableNames.log_tag}"" (log_id, tag_id) VALUES (@LogId, @TagId)";
        // // var query = $@"UPDATE""{TableNames.log_tag}"" SET name = @name";

        // using (var con = NewConnection)
        //    if( (await con.ExecuteAsync(query, Item)) > 0)
        //     {
        //         await con.ExecuteAsync(TagDelete, new { Id = Item.Id });
        //         foreach (var tag in Item.tag)
        //         {
        //             await con.ExecuteAsync(TagInsert, new { LogId = Item.Id, TagId = tag.Id });
        //         }
        //     }
        //     return true;
    }
    public async Task<bool> DeleteLog(long Id)
    {
        var query = $@"DELETE FROM {TableNames.log} WHERE id = @Id";

        using (var con = NewConnection)
            return (await con.ExecuteAsync(query, new { Id }) > 0);
    }

    public async Task<List<Tag>> GetTags(long Id)
    {
        var query = $@"SELECT * FROM ""{TableNames.tag}"" t
        Left Join ""{TableNames.log_tag}"" lt ON lt.tag_id = t.id WHERE lt.log_id = @Id";


        //   var query =$@"SELECT * FROM ""{TableNames.tag}"" t
        //   left Join ""{TableNames.log_tag}"" lt ON lt.tag_id = t.id left join ""{TableNames.log}"" l ON l.id = lt.log_id
        //     Left Join ""{TableNames.tag_type}"" tt ON tt.id = t.type_id WHERE t.id = @Id";
        using (var con = NewConnection)
        {
            return (await con.QueryAsync<Tag>(query, new { Id })).AsList();
        }
    }


    public async Task seenId(int Id, long id)
    {
        var query = $@"INSERT INTO ""{TableNames.log_seen}"" ( user_id, log_id) VALUES (@UserId, @Logid) RETURNING *";

        using (var con = NewConnection)
        {
            var res = await con.QuerySingleOrDefaultAsync(query, new { UserId = Id, LogId = id });

        }
        // return res;
    }

    public async Task<List<TagTypeDTO>> GetLogTagTypesById(long id)
    {
        var query = $@"  SELECT * FROM ""{TableNames.log}"" l
	  left Join ""{TableNames.log_tag}"" lt ON lt.tag_id = l.id left join ""{TableNames.tag}"" t ON t.id = lt.tag_id
        Left Join ""{TableNames.tag_type}"" tt ON tt.id = t.type_id WHERE t.id = @Id";
        using (var con = NewConnection)
        {
            return (await con.QueryAsync<TagTypeDTO>(query, new { Id = id })).AsList();

        }
    }

}