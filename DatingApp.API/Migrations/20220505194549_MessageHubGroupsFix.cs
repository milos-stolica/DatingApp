using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.API.Migrations
{
    public partial class MessageHubGroupsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageHubConnections_MessageHubGroups_MessageHubGroupName",
                table: "MessageHubConnections");

            migrationBuilder.DropIndex(
                name: "IX_MessageHubConnections_MessageHubGroupName",
                table: "MessageHubConnections");

            migrationBuilder.DropColumn(
                name: "MessageHubGroupName",
                table: "MessageHubConnections");

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "MessageHubConnections",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MessageHubConnections_GroupId",
                table: "MessageHubConnections",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageHubConnections_MessageHubGroups_GroupId",
                table: "MessageHubConnections",
                column: "GroupId",
                principalTable: "MessageHubGroups",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageHubConnections_MessageHubGroups_GroupId",
                table: "MessageHubConnections");

            migrationBuilder.DropIndex(
                name: "IX_MessageHubConnections_GroupId",
                table: "MessageHubConnections");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "MessageHubConnections");

            migrationBuilder.AddColumn<string>(
                name: "MessageHubGroupName",
                table: "MessageHubConnections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageHubConnections_MessageHubGroupName",
                table: "MessageHubConnections",
                column: "MessageHubGroupName");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageHubConnections_MessageHubGroups_MessageHubGroupName",
                table: "MessageHubConnections",
                column: "MessageHubGroupName",
                principalTable: "MessageHubGroups",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
