using Dapper;
using LogBackend.Models;
using LogBackend.Repositories;
using LogBackend.Utilities;

namespace LogBackend.Repositories;


public interface ITagRepository
{
    Task<Tag> Create(Tag Item);
    Task<List<Tag>> GetTagsByLogId(long Id);
    Task<List<Tag>> GetAllTags();
    Task<Tag> GetById(long Id);
    Task<bool> DeleteTag(long Id);


}

public class TagRepository : BaseRepository, ITagRepository
{
    public TagRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<Tag> Create(Tag Item)
    {
        var query = $@"INSERT INTO {TableNames.tag} (name, type_id) VALUES (@Name, @TypeId) RETURNING *";
         using (var connection = NewConnection)
        {
            return await connection.QuerySingleOrDefaultAsync<Tag>(query, Item);

        }
    }

    public async Task<List<Tag>> GetTagsByLogId(long LogId)
    {
        var query = $@"SELECT * FROM ""{TableNames.log_tag}"" lt LEFT JOIN ""{TableNames.tag}"" t  ON t.id = lt.tag_id  where lt.log_id = @LogId";
        // var query = $@"select * from ""{TableNames.user_tag}"" ut left join ""{TableNames.tag}"" t on t.id = ut.tag_id where ut.user_id = @Id";
        // type name left join
        using (var con = NewConnection)
        {
            return (await con.QueryAsync<Tag>(query, new { LogId })).AsList();
        }
    }


    public async Task<List<Tag>> GetAllTags()
    {
       var query = $@"SELECT * FROM {TableNames.tag}";
        using (var con = NewConnection)
        {
            return (await con.QueryAsync<Tag>(query)).AsList();
        }
    }


    // public async Task<bool> DeleteTag(long Id)
    // {
    //     var query = $@"DELETE FROM {TableNames.tag} WHERE id = @Id";

    //     using (var con = NewConnection)
    //         return (await con.ExecuteAsync(query, new { Id }) > 0);
    // }

     public async Task<bool> DeleteTag(long Id)
    {
        var query = $@"DELETE FROM {TableNames.tag} WHERE id = @Id";

        using (var con = NewConnection)
            return (await con.ExecuteAsync(query, new { Id }) > 0);
    }

      public async Task<Tag> GetById(long Id)
    {
        var query = $@"SELECT * FROM {TableNames.tag} WHERE id = @Id";
        using (var connection = NewConnection)
        {
            var Res = await connection.QuerySingleOrDefaultAsync<Tag>(query, new { Id });
            return Res;
        }
    }
}