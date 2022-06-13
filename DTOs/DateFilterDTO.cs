using Microsoft.AspNetCore.Mvc;

namespace LogBackend.DTOs;

public record DateFilterDTO
{
    private DateTimeOffset? _fromDate = null;
    private DateTimeOffset? _toDate = null;

    [FromQuery(Name = "from_date")]
    public DateTimeOffset? FromDate
    {
        get => _fromDate;
        set => _fromDate = value;
    }
    [FromQuery(Name = "to_date")]
    public DateTimeOffset? ToDate
    {
        get => this._toDate?.AddDays(1).Subtract(new TimeSpan(0, 0, 0, 0, 1));
        set => _toDate = value;
    }

    public record DateFilterSortingDTO : DateFilterDTO
    {
        [FromQuery(Name = "sort_created_at")]
        public string SortCreatedAt { get; set; } = null;
    }


    public record DateOfJoiningSortingDTO
    {
        [FromQuery(Name = "sort_by_date_of_joining")]
        public string SortByDateOfJoining { get; set; } = null;
    }
}
