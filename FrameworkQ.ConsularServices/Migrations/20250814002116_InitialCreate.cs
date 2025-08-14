using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FrameworkQ.ConsularServices.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateSequence(
                name: "sequence_seq",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "permission",
                schema: "public",
                columns: table => new
                {
                    permission_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    permission_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission", x => x.permission_id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                schema: "public",
                columns: table => new
                {
                    role_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    role_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "service_info",
                schema: "public",
                columns: table => new
                {
                    service_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    service_name = table.Column<string>(type: "text", nullable: false),
                    service_description = table.Column<string>(type: "text", nullable: false),
                    usual_service_days = table.Column<int>(type: "integer", nullable: false),
                    service_fee = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_info", x => x.service_id);
                });

            migrationBuilder.CreateTable(
                name: "station",
                schema: "public",
                columns: table => new
                {
                    station_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    station_name = table.Column<string>(type: "text", nullable: false),
                    queue_status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station", x => x.station_id);
                });

            migrationBuilder.CreateTable(
                name: "token",
                schema: "public",
                columns: table => new
                {
                    token_id = table.Column<string>(type: "text", nullable: false),
                    generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    appointment_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    mobile_no = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    service_type = table.Column<long[]>(type: "bigint[]", nullable: true),
                    passport_no = table.Column<string>(type: "text", nullable: true),
                    nid_no = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_token", x => x.token_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "role_permission_map",
                schema: "public",
                columns: table => new
                {
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    permission_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission_map", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK_role_permission_map_permission_permission_id",
                        column: x => x.permission_id,
                        principalSchema: "public",
                        principalTable: "permission",
                        principalColumn: "permission_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permission_map_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_instance",
                schema: "public",
                columns: table => new
                {
                    service_instance_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    service_info_id = table.Column<long>(type: "bigint", nullable: false),
                    payment_made_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delivery_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delivered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    attachments_received = table.Column<string[]>(type: "text[]", nullable: true),
                    token_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_instance", x => x.service_instance_id);
                    table.ForeignKey(
                        name: "FK_service_instance_service_info_service_info_id",
                        column: x => x.service_info_id,
                        principalSchema: "public",
                        principalTable: "service_info",
                        principalColumn: "service_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_instance_token_token_id",
                        column: x => x.token_id,
                        principalSchema: "public",
                        principalTable: "token",
                        principalColumn: "token_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_user_map",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_user_map", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_role_user_map_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_user_map_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "station_log",
                schema: "public",
                columns: table => new
                {
                    log_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    station_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station_log", x => x.log_id);
                    table.ForeignKey(
                        name: "FK_station_log_station_station_id",
                        column: x => x.station_id,
                        principalSchema: "public",
                        principalTable: "station",
                        principalColumn: "station_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_station_log_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "permission",
                columns: new[] { "permission_id", "permission_name" },
                values: new object[,]
                {
                    { 2L, "UPDATE_USER" },
                    { 3L, "DELETE_USER" },
                    { 4L, "DISABLE_USER" },
                    { 5L, "CHANGE_PASSWORD" },
                    { 12L, "UPDATE_ROLE" },
                    { 14L, "DELETE_ROLE" },
                    { 22L, "UPDATE_SERVICE_INFO" },
                    { 23L, "MODIFY_ROLE" },
                    { 32L, "CREATE_TOKEN" },
                    { 33L, "UPDATE_TOKEN" },
                    { 42L, "UPDATE_SERVICE_INSTANCE" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_map_permission_id",
                schema: "public",
                table: "role_permission_map",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_user_map_role_id",
                schema: "public",
                table: "role_user_map",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_instance_service_info_id",
                schema: "public",
                table: "service_instance",
                column: "service_info_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_instance_token_id",
                schema: "public",
                table: "service_instance",
                column: "token_id");

            migrationBuilder.CreateIndex(
                name: "IX_station_log_station_id",
                schema: "public",
                table: "station_log",
                column: "station_id");

            migrationBuilder.CreateIndex(
                name: "IX_station_log_user_id",
                schema: "public",
                table: "station_log",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_permission_map",
                schema: "public");

            migrationBuilder.DropTable(
                name: "role_user_map",
                schema: "public");

            migrationBuilder.DropTable(
                name: "service_instance",
                schema: "public");

            migrationBuilder.DropTable(
                name: "station_log",
                schema: "public");

            migrationBuilder.DropTable(
                name: "permission",
                schema: "public");

            migrationBuilder.DropTable(
                name: "role",
                schema: "public");

            migrationBuilder.DropTable(
                name: "service_info",
                schema: "public");

            migrationBuilder.DropTable(
                name: "token",
                schema: "public");

            migrationBuilder.DropTable(
                name: "station",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user",
                schema: "public");

            migrationBuilder.DropSequence(
                name: "sequence_seq",
                schema: "public");
        }
    }
}
