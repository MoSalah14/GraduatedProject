using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutbornE_commerce.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeSections", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeSections");
        }
    }
}
