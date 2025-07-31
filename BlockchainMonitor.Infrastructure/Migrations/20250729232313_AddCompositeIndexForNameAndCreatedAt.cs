using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockchainMonitor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeIndexForNameAndCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BlockchainData_Name_CreatedAt",
                table: "BlockchainData",
                columns: new[] { "Name", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlockchainData_Name_CreatedAt",
                table: "BlockchainData");
        }
    }
}
