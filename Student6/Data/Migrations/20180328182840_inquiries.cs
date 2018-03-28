using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Student6.Data.Migrations
{
    public partial class inquiries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inquiry",
                columns: table => new
                {
                    InquiryId = table.Column<Guid>(nullable: false),
                    Question = table.Column<string>(nullable: true),
                    Response = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inquiry", x => x.InquiryId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inquiry");
        }
    }
}
