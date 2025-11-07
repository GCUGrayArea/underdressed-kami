using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "job_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contractors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    formatted_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    job_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false),
                    base_latitude = table.Column<double>(type: "double precision", precision: 9, scale: 6, nullable: false),
                    base_longitude = table.Column<double>(type: "double precision", precision: 9, scale: 6, nullable: false),
                    base_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contractors", x => x.id);
                    table.ForeignKey(
                        name: "FK_contractors_job_types_job_type_id",
                        column: x => x.job_type_id,
                        principalTable: "job_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    formatted_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    job_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    customer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    location_latitude = table.Column<double>(type: "double precision", precision: 9, scale: 6, nullable: false),
                    location_longitude = table.Column<double>(type: "double precision", precision: 9, scale: 6, nullable: false),
                    location_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    desired_date = table.Column<DateTime>(type: "date", nullable: false),
                    desired_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    estimated_duration_hours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    assigned_contractor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    scheduled_start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.id);
                    table.ForeignKey(
                        name: "FK_jobs_contractors_assigned_contractor_id",
                        column: x => x.assigned_contractor_id,
                        principalTable: "contractors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_jobs_job_types_job_type_id",
                        column: x => x.job_type_id,
                        principalTable: "job_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "weekly_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contractor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weekly_schedules", x => x.id);
                    table.ForeignKey(
                        name: "FK_weekly_schedules_contractors_contractor_id",
                        column: x => x.contractor_id,
                        principalTable: "contractors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "job_types",
                columns: new[] { "id", "description", "is_active", "name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Specializes in ceramic, porcelain, and natural stone tile installation", true, "Tile Installer" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Specializes in carpet installation and replacement", true, "Carpet Installer" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Specializes in hardwood floor installation, refinishing, and repair", true, "Hardwood Specialist" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_contractors_formatted_id",
                table: "contractors",
                column: "formatted_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contractors_is_active",
                table: "contractors",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_contractors_job_type_id",
                table: "contractors",
                column: "job_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_contractors_rating",
                table: "contractors",
                column: "rating");

            migrationBuilder.CreateIndex(
                name: "ix_job_types_is_active",
                table: "job_types",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_job_types_name",
                table: "job_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_jobs_assigned_contractor_id",
                table: "jobs",
                column: "assigned_contractor_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_desired_date",
                table: "jobs",
                column: "desired_date");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_formatted_id",
                table: "jobs",
                column: "formatted_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_jobs_job_type_id",
                table: "jobs",
                column: "job_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_status",
                table: "jobs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_weekly_schedules_contractor_day",
                table: "weekly_schedules",
                columns: new[] { "contractor_id", "day_of_week" });

            migrationBuilder.CreateIndex(
                name: "ix_weekly_schedules_contractor_id",
                table: "weekly_schedules",
                column: "contractor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "weekly_schedules");

            migrationBuilder.DropTable(
                name: "contractors");

            migrationBuilder.DropTable(
                name: "job_types");
        }
    }
}
