using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pulse.Persistence.Migrations
{
    public partial class InstructorSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "instructor_settings",
                schema: "pulse",
                columns: table => new
                {
                    instructor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_timeout_hours = table.Column<decimal>(type: "numeric", nullable: false),
                    emoticon_tap_delay_seconds = table.Column<decimal>(type: "numeric", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    modified_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_instructor_settings", x => x.instructor_id);
                    table.ForeignKey(
                        name: "fk_instructor_settings_application_user_instructor_id",
                        column: x => x.instructor_id,
                        principalSchema: "pulse",
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "instructor_settings",
                schema: "pulse");
        }
    }
}
