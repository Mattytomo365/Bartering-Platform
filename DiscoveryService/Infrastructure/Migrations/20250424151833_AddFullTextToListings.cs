using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextToListings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Create the full-text catalog if it doesn’t exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 
                      FROM sys.fulltext_catalogs 
                     WHERE name = N'ListingCatalog'
                )
                    CREATE FULLTEXT CATALOG ListingCatalog
                    WITH ACCENT_SENSITIVITY = OFF;
                ",
                suppressTransaction: true
            );

            // 2) Create the full-text index on Title + Description
            migrationBuilder.Sql(@"
                CREATE FULLTEXT INDEX ON dbo.Listings
                (
                    Title       LANGUAGE 1033,  -- English
                    Description LANGUAGE 1033
                )
                KEY INDEX PK_Listings
                ON ListingCatalog
                WITH CHANGE_TRACKING AUTO;
                ",
                suppressTransaction: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Tear down the full-text index if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                      FROM sys.fulltext_indexes fi
                      JOIN sys.objects o 
                        ON fi.object_id = o.object_id
                     WHERE o.name = 'Listings'
                )
                DROP FULLTEXT INDEX ON dbo.Listings;
                ",
                suppressTransaction: true
            );

            // And drop the catalog
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 
                      FROM sys.fulltext_catalogs 
                     WHERE name = N'ListingCatalog'
                )
                    DROP FULLTEXT CATALOG ListingCatalog;
                ", 
                suppressTransaction: true
            );
        }
    }
}
