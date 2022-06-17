using Dapper;
using Logbackend.Models;
using LogBackend.DTOs;
using LogBackend.Models;
using LogBackend.Repositories;
using LogBackend.Utilities;

namespace LogBackend.Repositories;


public interface ITagRepository
{
    Task<Tag> Create(Tag Item);
    Task<List<Log>> GetTagsByLogId(long Id);
    Task<List<Tag>> GetAllTags(TagFilterDTO tagfilter);
    Task<Tag> GetById(long Id);
    Task<bool> UpdateTag(Tag Item);
    // Task<bool> DeleteTag(long Id);
    // Task<List<Log>> GetTagByLog(long Id);
    Task<List<TagTypeDTO>> GetTagTypeByLogId(int id);


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

    public async Task<List<Log>> GetTagsByLogId(long LogId)
    {

        var query = $@"SELECT * FROM""{TableNames.log}"" l
        Left Join""{TableNames.log_tag}""lt ON lt.log_id = l.id WHERE lt.tag_id = @Id";




        //     var query = $@"SELECT * FROM ""{TableNames.tag}"" t
        //    left Join ""{TableNames.log_tag}"" lt ON lt.tag_id = t.id left join ""{TableNames.log}"" l ON l.id = lt.log_id
        //      Left Join ""{TableNames.tag_type}"" tt ON tt.id = t.type_id WHERE t.id = @Id";

        // type name left join
        using (var con = NewConnection)
        {
            return (await con.QueryAsync<Log>(query, new { LogId })).AsList();
        }
    }
    public async Task<List<TagTypeDTO>> GetTagTypeByLogId(int id)
    {

        // var query = $@"SELECT * FROM""{TableNames.tag_type}"" tt
        // Left Join""{TableNames.tag}""t ON t.type_id = tt.id WHERE t.id = @Id";

        var query = $@"SELECT * FROM ""{TableNames.tag}"" t
	   left Join ""{TableNames.log_tag}"" lt ON lt.tag_id = t.id left join ""{TableNames.log}"" l ON l.id = lt.log_id
         Left Join ""{TableNames.tag_type}"" tt ON tt.id = t.type_id WHERE t.id = @Id";

        using (var con = NewConnection)
        {
            return (await con.QueryAsync<TagTypeDTO>(query, new { Id = id })).AsList();
        }
    }


    // public async Task<List<Tag>> GetAllTags()
    // {
    //     var query = $@"SELECT * FROM {TableNames.tag}";
    //     using (var con = NewConnection)
    //     {
    //         return (await con.QueryAsync<Tag>(query)).AsList();
    //     }
    // }


    public async Task<List<Tag>> GetAllTags(TagFilterDTO tagfilter)
    {
        List<Tag> res;
        var query = $@"SELECT * FROM ""{TableNames.tag}"" ";
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
            res = (await con.QueryAsync<Tag>(query, paramsObj)).AsList();
        }
        return res;
    }



    // public async Task<bool> DeleteTag(long Id)
    // {
    //     var query = $@"DELETE FROM {TableNames.tag} WHERE id = @Id";

    //     using (var con = NewConnection)
    //         return (await con.ExecuteAsync(query, new { Id }) > 0);
    // }

    // public async Task<bool> DeleteTag(long Id)
    // {
    //     var query = $@"DELETE FROM {TableNames.tag} WHERE id = @Id";

    //     using (var con = NewConnection)
    //         return (await con.ExecuteAsync(query, new { Id }) > 0);
    // }

    public async Task<Tag> GetById(long Id)
    {
        var query = $@"SELECT * FROM {TableNames.tag} WHERE id = @Id";
        using (var connection = NewConnection)
        {
            var Res = await connection.QuerySingleOrDefaultAsync<Tag>(query, new { Id });
            return Res;
        }
    }

    public async Task<bool> UpdateTag(Tag Item)
    {
        var query = $@"UPDATE ""{TableNames.tag}"" SET name = @Name WHERE id = @Id";
        using (var con = NewConnection)
        {
            return await con.ExecuteAsync(query, Item) > 0;
        }
    }
}