using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiBook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "security_audit_logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RiskLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RiskScore = table.Column<int>(type: "integer", nullable: false),
                    HttpMethod = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Path = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    QueryString = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RemoteIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReasonsSummary = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_security_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_security_audit_logs_RecordedAtUtc",
                table: "security_audit_logs",
                column: "RecordedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_security_audit_logs_RiskScore",
                table: "security_audit_logs",
                column: "RiskScore");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "security_audit_logs");
        }
    }
}
