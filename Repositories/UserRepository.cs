using Dapper;
using LogBackend.DTOs;
using LogBackend.Models;
using LogBackend.Utilities;

namespace LogBackend.Repositories;


public interface IUserRepository
{
    Task<User> Create(User Item);
    Task<bool> Update(User Item);

    Task<List<User>> GetAllUser(TagFilterDTO tagfilter);

    Task<User> GetUserById(int Id);
    Task<List<Tag>> GetTagUserById(int Id);

    Task<User> GetByEmail(string Email);

}


public class UserRepository : BaseRepository, IUserRepository
{

    public UserRepository(IConfiguration Configuration) : base(Configuration)
    {
    }
    public async Task<User> Create(User Item)
    {
        var query = $@"INSERT INTO ""{TableNames.user}"" (name,email,password,is_superuser )
        Values (@Name, @Email, @Password, @IsSuperuser) RETURNING *";
        using (var connection = NewConnection)
        {
            return await connection.QuerySingleOrDefaultAsync<User>(query, Item);

        }

    }
    public async Task<User> GetUserById(int Id)
    {
        var query = $@"SELECT * FROM ""{TableNames.user}"" WHERE id = @Id";
        using (var connection = NewConnection)
        {
            var Res = await connection.QuerySingleOrDefaultAsync<User>(query, new { Id });
            return Res;
        }
    }
    public async Task<List<Tag>> GetTagUserById(int Id)
    {
        var query = $@"SELECT * FROM ""{TableNames.tag}""t LEFT JOIN ""{TableNames.user_tag}"" ut ON ut.tag_id = t.id WHERE ut.user_id = @Id";
        using (var connection = NewConnection)
        {
            var Res = (await connection.QueryAsync<Tag>(query, new { Id = Id })).AsList();
            return Res;
        }
    }


    public async Task<User> GetByEmail(string Email)
    {
        var query = $@"SELECT * FROM ""{TableNames.user}"" WHERE email = @Email";
        using (var con = NewConnection)
        {
            return await con.QuerySingleOrDefaultAsync<User>(query, new { Email });
        }
    }

    public async Task<List<User>> GetAllUser(TagFilterDTO tagfilter)
    {
        List<User> res;

        var query = $@"SELECT * FROM ""{TableNames.user}""";

        if (tagfilter.Name is not null)
        {
            query += " WHERE name = @Name";
        }
        var paramsObj = new
        {
            Name = tagfilter?.Name,
        };
        using (var con = NewConnection)
        {
            res = (await con.QueryAsync<User>(query, paramsObj)).AsList();
        }
        return res;
        // var query = $@"SELECT * FROM ""{TableNames.user}""";

        // using (var con = NewConnection)
        // {
        //     return (await con.QueryAsync<User>(query)).AsList();
        // }
    }

    public async Task<bool> Update(User Item)
    {
        var query = $@"UPDATE ""{TableNames.user}"" SET status = @Status WHERE id = @Id";
        // , last_login = now()
        using (var con = NewConnection)

            return await con.ExecuteAsync(query, Item) > 0;
    }


}