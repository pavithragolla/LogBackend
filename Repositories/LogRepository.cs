using Logbackend.Models;
using LogBackend.Utilities;
using LogBackend.Repositories;
using Dapper;
using LogBackend.Models;
using Microsoft.Extensions.Caching.Memory;
using LogBackend.DTOs;
using logbackend.Services;

namespace Logbackend.Repositories;

public interface ILogRepository
{
    Task<Log> Create(Log Item);
    Task<bool> Update(Log Item, List<int> tags);
    Task<bool> DeleteLog(long Id);
    // Task<List<Log>> GetAllLog(int Limit, int PageNumber);
    Task<List<Log>> GetAllLog(DateFilterDTO dateFilter, LogFilterDTO logfilter = null);
    Task<List<Log>> GetAllUserLog(DateFilterDTO dateFilter, LogFilterDTO logfilter = null);
    Task<Log> GetById(long id);
    Task<List<Tag>> GetTags(long id);
    Task seenId(int Id, long id);
    Task<List<TagTypeDTO>> GetLogTagTypesById(long id);
    Task<bool> SoftDelete(long Id);

    Task sendPushNotification();

    // Task<Log> SetReadStatus(long Id, int UserId, int LogId);
    // Task<bool> unseen(int Id, long id);

}


public class LogRepository : BaseRepository, ILogRepository
{

    private readonly IPushNotificationService _pushNotification;
    private readonly ILogger<LogRepository> _logger;

    public LogRepository(IConfiguration configuration, IPushNotificationService pushNotification, ILogger<LogRepository> logger) : base(configuration)
    {
        _pushNotification = pushNotification;
        _logger = logger;
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

    public async Task<List<Log>> GetAllLog(DateFilterDTO dateFilter, LogFilterDTO logfilter = null)
    {
        List<Log> res;
        // List<Tag> res;

        var query = $@"SELECT * FROM ""{TableNames.log}"" ";


        if (logfilter.Title is not null)
        {
            query += " WHERE title = @Title";
        }
        // // var paramsObj = new
        // {
        //     Name = tagfilter?.Name,
        // };
        // using (var con = NewConnection)
        // {
        //     res = (await con.QueryAsync<Tag>(query, paramsObj)).AsList();
        // }
        // return res;


        if (dateFilter is not null && (dateFilter.FromDate.HasValue || dateFilter.ToDate.HasValue))
        {
            if (dateFilter.FromDate is null) dateFilter.FromDate = DateTime.MinValue;
            if (dateFilter.ToDate is null) dateFilter.ToDate = DateTime.Now;
            query += "WHERE created_at BETWEEN  @FromDate AND  @ToDate";


        }

        // if (tagfilter.Name is not null)
        // {
        //     query += " WHERE name = @Name";
        // }

        var paramsObj = new
        {

            FromDate = dateFilter?.FromDate,
            ToDate = dateFilter?.ToDate,
            Title = logfilter?.Title,


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



    public async Task<bool> Update(Log Item, List<int> tags)
    {
        var query = $@"UPDATE ""{TableNames.log}"" SET  description = @Description, updated_by_user_id = @UpdatedByUserId, updated_at = now() WHERE id = @Id";
        // using (var connection = NewConnection)
        // {
        //     return (await connection.ExecuteAsync(query, Item)) > 0;
        // }
        var TagDelete = $@"DELETE FROM ""{TableNames.log_tag}"" WHERE log_id = @Id";
        var TagInsert = $@"INSERT INTO ""{TableNames.log_tag}"" (log_id, tag_id) VALUES (@LogId, @TagId)";
        // var query = $@"UPDATE""{TableNames.log_tag}"" SET name = @name";

        using (var con = NewConnection)
            if ((await con.ExecuteAsync(query, Item)) > 0)
            {
                await con.ExecuteAsync(TagDelete, new { Id = Item.Id });
                foreach (var tagId in tags)
                    await con.QuerySingleOrDefaultAsync(TagInsert, new { LogId = Item.Id, TagId = tagId });
                return true;
            }
            else
                return false;
    }
    public async Task<bool> DeleteLog(long Id)
    {
        var query = $@"DELETE FROM ""{TableNames.log}"" WHERE id = @Id";

        using (var con = NewConnection)
            return (await con.ExecuteAsync(query, new { Id }) > 0);
    }
    public async Task<bool> SoftDelete(long Id)
    {
        var query = $@"UPDATE  ""{TableNames.log}"" SET partially_deleted = true WHERE id = @Id";
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

    // public async Task<Log> SetReadStatus(long Id, int UserId, int LogId)
    // {
    //     var query = $@"INSERT INTO ""{TableNames.log_seen}"" ( user_id, log_id) VALUES (@UserId, @Logid) RETURNING *";

    //     using (var con = NewConnection)
    //     {
    //         return await con.QuerySingleOrDefaultAsync(query, new { UserId = UserId, LogId = Id });

    //     }
    // }
    // public async Task<bool> unseen(int Id, long id)
    // {
    //     var query = $@"DELETE FROM ""{TableNames.log_seen}"" WHERE user_id = @Id AND log_id = @Logid";

    //     using (var con = NewConnection)
    //         return (await con.ExecuteAsync(query, new { Id = Id, LogId = id }) > 0);
    // }

    public async Task<List<Log>> GetAllUserLog(DateFilterDTO dateFilter, LogFilterDTO logfilter = null)
    {
        var query = $@"SELECT * FROM ""{TableNames.log}"" WHERE partially_deleted = false";
        // var query = $@"SELECT * FROM ""{TableNames.user}"" WHERE id = @id";
        // {ut} ut  lf logtag lt on lt.tag_id= ut.tag_idlf logtable l on l.id =lt.log_id where ut.user_id = @userId;
        // var query = $@"SELECT *FROM {TableNames.user_tag} ut LEFT JOIN {TableNames.log_tag} lt ON lt.tag_id = ut.tag_id LEFT JOIN {TableNames.log} l ON l.id =lt.log_id WHERE ut.user_id = @userId";
        // long userId,
        // long userId,
        // srearch, markseen,upd user,get single, update tag,delete log

        using (var con = NewConnection)
        {
            return (await con.QueryAsync<Log>(query)).AsList();
        }
    }

    public async Task sendPushNotification()
    {
        var query = $@"SELECT * FROM {TableNames.user_login} WHERE notification_token != null";
        List<UserLogin> loggedUser = new List<UserLogin>();
        using (var con = NewConnection)
            loggedUser = (await con.QueryAsync<UserLogin>(query)).AsList();

        Console.WriteLine(loggedUser);
        var notificationData = new PushNotificationData
        {
            BodyText = "Resolve entry",
            TitleText = "You have got new log entry"
        };

        var pn = _pushNotification.SendAll(loggedUser.Select(x => notificationData with
        {
            NotificationToken = x.NotificationToken,
        }).AsList());

    }
}