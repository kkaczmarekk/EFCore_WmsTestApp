using System;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WmsApp.Persistence.Items.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoriesOwnedString = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CategoryBranchString = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedByUserName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
                    DeletedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
                    UpdatedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedByUserName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
                    DeletedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
                    UpdatedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedByUserName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
                    DeletedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
                    UpdatedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CategoriesOwners",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesOwners", x => new { x.OwnerId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CategoriesOwners_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriesOwners_OwnerType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "OwnerType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategoriesOwners_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemCashed",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ReviewsAverageVote = table.Column<double>(type: "float", nullable: false),
                    ReviewsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCashed", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_ItemCashed_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoteStars = table.Column<int>(type: "int", nullable: false),
                    VoterName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemReview_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesOwners_CategoryId",
                table: "CategoriesOwners",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesOwners_TypeId",
                table: "CategoriesOwners",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemReview_ItemId",
                table: "ItemReview",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_IsDeleted",
                table: "Items",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Owners_IsDeleted",
                table: "Owners",
                column: "IsDeleted");  
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriesOwners");

            migrationBuilder.DropTable(
                name: "ItemCashed");

            migrationBuilder.DropTable(
                name: "ItemReview");

            migrationBuilder.DropTable(
                name: "OwnerType");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
