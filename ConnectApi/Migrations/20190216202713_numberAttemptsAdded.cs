using Microsoft.EntityFrameworkCore.Migrations;

namespace ConnectApi.Migrations
{
    public partial class numberAttemptsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserOtpInfos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "NumberOfAttempts",
                table: "UserOtpInfos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfAttempts",
                table: "UserOtpInfos");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserOtpInfos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
