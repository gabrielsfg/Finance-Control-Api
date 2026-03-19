using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceControl.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBankTableWithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('America/Sao_Paulo', now())"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Banks",
                columns: new[] { "Id", "Code", "Country", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { 1, "001", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Banco do Brasil" },
                    { 2, "237", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bradesco" },
                    { 3, "341", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Itaú Unibanco" },
                    { 4, "104", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Caixa Econômica Federal" },
                    { 5, "033", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Santander Brasil" },
                    { 6, "260", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nubank" },
                    { 7, "077", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Inter" },
                    { 8, "336", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "C6 Bank" },
                    { 9, "341", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "XP Investimentos" },
                    { 10, "380", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PicPay" },
                    { 11, "756", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sicoob" },
                    { 12, "748", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sicredi" },
                    { 13, "208", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "BTG Pactual" },
                    { 14, "735", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Neon" },
                    { 15, "323", "BR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mercado Pago" },
                    { 16, "CHASUS33", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "JPMorgan Chase" },
                    { 17, "BOFAUS3N", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bank of America" },
                    { 18, "WFBIUS6S", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wells Fargo" },
                    { 19, "CITIUS33", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Citibank" },
                    { 20, "USBKUS44", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "U.S. Bancorp" },
                    { 21, "SNTRUS3A", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Truist Financial" },
                    { 22, "PNCCUS33", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PNC Financial" },
                    { 23, "GSCOUSS", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Goldman Sachs" },
                    { 24, "HIBKUS44", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Capital One" },
                    { 25, "AEIBUS33", "US", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "American Express" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banks");
        }
    }
}
