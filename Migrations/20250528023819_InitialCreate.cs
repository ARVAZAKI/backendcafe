using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendcafe.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Carts_TransactionId",
                table: "Carts",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Transactions_TransactionId",
                table: "Carts",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Transactions_TransactionId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_TransactionId",
                table: "Carts");
        }
    }
}
