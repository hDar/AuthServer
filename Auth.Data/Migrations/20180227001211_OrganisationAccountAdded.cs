using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Auth.Data.Migrations
{
    public partial class OrganisationAccountAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrganisationId",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "OrganisationAccount",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AddedBy = table.Column<string>(maxLength: 250, nullable: true),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(maxLength: 500, nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Region = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationAccount", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganisationId",
                table: "AspNetUsers",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_OrganisationAccount_OrganisationId",
                table: "AspNetUsers",
                column: "OrganisationId",
                principalTable: "OrganisationAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_OrganisationAccount_OrganisationId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "OrganisationAccount");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrganisationId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationId",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
