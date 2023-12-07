using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamUpAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add_User_Password_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PasswordVerified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordVerified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "UserProfiles");
        }
    }
}
