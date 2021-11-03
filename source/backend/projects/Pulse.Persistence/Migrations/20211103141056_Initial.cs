using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Pulse.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pulse");

            migrationBuilder.CreateTable(
                name: "application_user",
                schema: "pulse",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "emoticon",
                schema: "pulse",
                columns: table => new
                {
                    emoticon_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    color = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    sort_index = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    modified_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_emoticon", x => x.emoticon_id);
                });

            migrationBuilder.CreateTable(
                name: "identity_role",
                schema: "pulse",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "identity_refresh_token",
                schema: "pulse",
                columns: table => new
                {
                    identity_refresh_token_id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    expired = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_refresh_token", x => x.identity_refresh_token_id);
                    table.ForeignKey(
                        name: "fk_identity_refresh_token_application_user_application_user_id",
                        column: x => x.application_user_id,
                        principalSchema: "pulse",
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identity_user_claim",
                schema: "pulse",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_identity_user_claim_application_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "pulse",
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identity_user_login",
                schema: "pulse",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user_login", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_identity_user_login_application_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "pulse",
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identity_user_token",
                schema: "pulse",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user_token", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_identity_user_token_application_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "pulse",
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "instructor_emoticon",
                schema: "pulse",
                columns: table => new
                {
                    instructor_emoticon_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    color = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    sort_index = table.Column<int>(type: "integer", nullable: false),
                    instructor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    modified_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_instructor_emoticon", x => x.instructor_emoticon_id);
                    table.ForeignKey(
                        name: "fk_instructor_emoticon_application_user_instructor_id",
                        column: x => x.instructor_id,
                        principalSchema: "pulse",
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "instructor_profile",
                schema: "pulse",
                columns: table => new
                {
                    instructor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    subject = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    image = table.Column<byte[]>(type: "bytea", nullable: true),
                    small_image = table.Column<byte[]>(type: "bytea", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    modified_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_instructor_profile", x => x.instructor_id);
                    table.ForeignKey(
                        name: "fk_instructor_profile_application_user_instructor_id",
                        column: x => x.instructor_id,
                        principalSchema: "pulse",
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "the_class",
                schema: "pulse",
                columns: table => new
                {
                    class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    instructor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    modified_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_the_class", x => x.class_id);
                    table.ForeignKey(
                        name: "fk_the_class_application_user_instructor_id",
                        column: x => x.instructor_id,
                        principalSchema: "pulse",
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identity_role_claim",
                schema: "pulse",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_identity_role_claim_identity_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "pulse",
                        principalTable: "identity_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identity_user_role",
                schema: "pulse",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_identity_user_role_application_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "pulse",
                        principalTable: "application_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_identity_user_role_identity_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "pulse",
                        principalTable: "identity_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session",
                schema: "pulse",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    finished = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    modified_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session", x => x.session_id);
                    table.ForeignKey(
                        name: "fk_session_the_class_class_id",
                        column: x => x.class_id,
                        principalSchema: "pulse",
                        principalTable: "the_class",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_checkin",
                schema: "pulse",
                columns: table => new
                {
                    session_checkin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    checkin_type = table.Column<int>(type: "integer", nullable: false),
                    started = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    finished = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session_checkin", x => x.session_checkin_id);
                    table.ForeignKey(
                        name: "fk_session_checkin_session_session_id",
                        column: x => x.session_id,
                        principalSchema: "pulse",
                        principalTable: "session",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_student",
                schema: "pulse",
                columns: table => new
                {
                    session_student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    modified_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session_student", x => x.session_student_id);
                    table.ForeignKey(
                        name: "fk_session_student_session_session_id",
                        column: x => x.session_id,
                        principalSchema: "pulse",
                        principalTable: "session",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "emoticon_tap",
                schema: "pulse",
                columns: table => new
                {
                    emoticon_tap_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    instructor_emoticon_id = table.Column<Guid>(type: "uuid", nullable: false),
                    time_ticks = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_emoticon_tap", x => x.emoticon_tap_id);
                    table.ForeignKey(
                        name: "fk_emoticon_tap_instructor_emoticon_instructor_emoticon_id",
                        column: x => x.instructor_emoticon_id,
                        principalSchema: "pulse",
                        principalTable: "instructor_emoticon",
                        principalColumn: "instructor_emoticon_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_emoticon_tap_session_student_session_student_id",
                        column: x => x.session_student_id,
                        principalSchema: "pulse",
                        principalTable: "session_student",
                        principalColumn: "session_student_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_question",
                schema: "pulse",
                columns: table => new
                {
                    session_question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_checkin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    dismissed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session_question", x => x.session_question_id);
                    table.ForeignKey(
                        name: "fk_session_question_session_checkin_session_checkin_id",
                        column: x => x.session_checkin_id,
                        principalSchema: "pulse",
                        principalTable: "session_checkin",
                        principalColumn: "session_checkin_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_session_question_session_student_session_student_id",
                        column: x => x.session_student_id,
                        principalSchema: "pulse",
                        principalTable: "session_student",
                        principalColumn: "session_student_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "email_index",
                schema: "pulse",
                table: "application_user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "user_name_index",
                schema: "pulse",
                table: "application_user",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_emoticon_tap_instructor_emoticon_id",
                schema: "pulse",
                table: "emoticon_tap",
                column: "instructor_emoticon_id");

            migrationBuilder.CreateIndex(
                name: "ix_emoticon_tap_session_student_id",
                schema: "pulse",
                table: "emoticon_tap",
                column: "session_student_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_refresh_token_application_user_id",
                schema: "pulse",
                table: "identity_refresh_token",
                column: "application_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_refresh_token_token",
                schema: "pulse",
                table: "identity_refresh_token",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "role_name_index",
                schema: "pulse",
                table: "identity_role",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identity_role_claim_role_id",
                schema: "pulse",
                table: "identity_role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_claim_user_id",
                schema: "pulse",
                table: "identity_user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_login_user_id",
                schema: "pulse",
                table: "identity_user_login",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_role_role_id",
                schema: "pulse",
                table: "identity_user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_instructor_emoticon_instructor_id",
                schema: "pulse",
                table: "instructor_emoticon",
                column: "instructor_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_class_id",
                schema: "pulse",
                table: "session",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_checkin_session_id",
                schema: "pulse",
                table: "session_checkin",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_question_session_checkin_id",
                schema: "pulse",
                table: "session_question",
                column: "session_checkin_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_question_session_student_id",
                schema: "pulse",
                table: "session_question",
                column: "session_student_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_student_session_id",
                schema: "pulse",
                table: "session_student",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_the_class_instructor_id",
                schema: "pulse",
                table: "the_class",
                column: "instructor_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emoticon",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "emoticon_tap",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "identity_refresh_token",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "identity_role_claim",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "identity_user_claim",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "identity_user_login",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "identity_user_role",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "identity_user_token",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "instructor_profile",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "session_question",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "instructor_emoticon",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "identity_role",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "session_checkin",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "session_student",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "session",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "the_class",
                schema: "pulse");

            migrationBuilder.DropTable(
                name: "application_user",
                schema: "pulse");
        }
    }
}
