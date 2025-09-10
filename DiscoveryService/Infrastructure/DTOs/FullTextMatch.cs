using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DTOs;

public class FullTextMatch
{
    // maps the SQL column [KEY] → C# property ListingId
    [Column("KEY")]
    public Guid ListingId { get; set; }

    // maps the SQL column [RANK] → C# property Rank
    [Column("RANK")]
    public int Rank { get; set; }

}
