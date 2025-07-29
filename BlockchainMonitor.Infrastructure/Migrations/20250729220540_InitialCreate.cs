using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockchainMonitor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockchainData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Height = table.Column<long>(type: "INTEGER", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LatestUrl = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PreviousHash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PreviousUrl = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PeerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    UnconfirmedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HighFeePerKb = table.Column<long>(type: "bigint", nullable: true),
                    MediumFeePerKb = table.Column<long>(type: "bigint", nullable: true),
                    LowFeePerKb = table.Column<long>(type: "bigint", nullable: true),
                    HighGasPrice = table.Column<long>(type: "bigint", nullable: true),
                    MediumGasPrice = table.Column<long>(type: "bigint", nullable: true),
                    LowGasPrice = table.Column<long>(type: "bigint", nullable: true),
                    HighPriorityFee = table.Column<long>(type: "bigint", nullable: true),
                    MediumPriorityFee = table.Column<long>(type: "bigint", nullable: true),
                    LowPriorityFee = table.Column<long>(type: "bigint", nullable: true),
                    BaseFee = table.Column<long>(type: "bigint", nullable: true),
                    LastForkHeight = table.Column<long>(type: "bigint", nullable: true),
                    LastForkHash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockchainData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainData_CreatedAt",
                table: "BlockchainData",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainData_Name",
                table: "BlockchainData",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockchainData");
        }
    }
}
