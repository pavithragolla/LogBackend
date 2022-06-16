using Dapper;
using LogBackend.Utilities;

namespace LogBackend.Repositories;

public interface IUserLoginRepository
{
    Task SetUserLogin(long userId, string deviceId, string NotificationToken = null, string UserAgent = null);
}
public class UserLoginRepository : BaseRepository, IUserLoginRepository
{
    public UserLoginRepository(IConfiguration Configuration) : base(Configuration)
    {
    }
    public async Task SetUserLogin(long userId, string deviceId, string NotificationToken = null, string UserAgent = null)
    {
        var query = $@"INSERT INTO ""{TableNames.user_login}"" (user_id, device_id, notification_token, user_agent) VALUES (@UserId, @DeviceId, @NotificationToken, @UserAgent)";

        var lastloginquery = $@"UPDATE ""{TableNames.user}"" SET last_login = NOW() WHERE id = @UserId";
        using (var connection = NewConnection)
        {
            await connection.QuerySingleOrDefaultAsync(query, new { UserId = userId, DeviceId = deviceId, NotificationToken = NotificationToken, UserAgent = UserAgent });
            await connection.ExecuteAsync(lastloginquery, new { UserId = userId });
        }
    }
}