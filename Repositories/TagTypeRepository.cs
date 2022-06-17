using Dapper;
using LogBackend.Models;
using LogBackend.Repositories;
using LogBackend.Utilities;

namespace LogBackend.DTOs;

public interface ITagTypeRepository
{
    Task<List<TagType>> GetAllTagTypes();
}



public class TagTypeRepository : BaseRepository, ITagTypeRepository
{
    public TagTypeRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<TagType>> GetAllTagTypes()
    {
        var query = $@"SELECT * FROM ""{TableNames.tag_type}"" ";
        using (var con = NewConnection)
        {
            return (await con.QueryAsync<TagType>(query)).AsList();
        }
    }
}