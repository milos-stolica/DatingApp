using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.API.Migrations
{
    public partial class MessageHubGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageHubGroups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageHubGroups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "MessageHubConnections",
                columns: table => new
                {
                    MessageHubConnectionId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    MessageHubGroupName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageHubConnections", x => x.MessageHubConnectionId);
                    table.ForeignKey(
                        name: "FK_MessageHubConnections_MessageHubGroups_MessageHubGroupName",
                        column: x => x.MessageHubGroupName,
                        principalTable: "MessageHubGroups",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageHubConnections_MessageHubGroupName",
                table: "MessageHubConnections",
                column: "MessageHubGroupName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageHubConnections");

            migrationBuilder.DropTable(
                name: "MessageHubGroups");
        }
    }
}
