﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Ticaret.Migrations.User
{
    public partial class denemeuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nchar(100)", nullable: false),
                    UserPassword = table.Column<string>(type: "nchar(64)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ViewUsers = table.Column<bool>(type: "bit", nullable: false),
                    CreatUser = table.Column<bool>(type: "bit", nullable: false),
                    DeleteUser = table.Column<bool>(type: "bit", nullable: false),
                    EditUser = table.Column<bool>(type: "bit", nullable: false),
                    ViewSellers = table.Column<bool>(type: "bit", nullable: false),
                    CreateSeller = table.Column<bool>(type: "bit", nullable: false),
                    DeleteSeller = table.Column<bool>(type: "bit", nullable: false),
                    EditSeller = table.Column<bool>(type: "bit", nullable: false),
                    ViewCategories = table.Column<bool>(type: "bit", nullable: false),
                    CreateCategory = table.Column<bool>(type: "bit", nullable: false),
                    DeleteCategory = table.Column<bool>(type: "bit", nullable: false),
                    EditCategory = table.Column<bool>(type: "bit", nullable: false),
                    DeleteProduct = table.Column<bool>(type: "bit", nullable: false),
                    EditProduct = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
