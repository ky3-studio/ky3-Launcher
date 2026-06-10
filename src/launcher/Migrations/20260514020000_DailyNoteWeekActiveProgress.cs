using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kyxsan.Migrations
{
    /// <inheritdoc />
    public partial class DailyNoteWeekActiveProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WeekActiveProgressDotVisible",
                table: "daily_notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WeekActiveProgressNotify",
                table: "daily_notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WeekActiveProgressNotifySuppressed",
                table: "daily_notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeekActiveProgressDotVisible",
                table: "daily_notes");

            migrationBuilder.DropColumn(
                name: "WeekActiveProgressNotify",
                table: "daily_notes");

            migrationBuilder.DropColumn(
                name: "WeekActiveProgressNotifySuppressed",
                table: "daily_notes");
        }
    }
}
