using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinema.Repository.Migrations
{
    public partial class UpdatedRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInOrders_Tickets_OrderId",
                table: "TicketInOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInOrders_Orders_TicketId",
                table: "TicketInOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInOrders_Orders_OrderId",
                table: "TicketInOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInOrders_Tickets_TicketId",
                table: "TicketInOrders",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInOrders_Orders_OrderId",
                table: "TicketInOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInOrders_Tickets_TicketId",
                table: "TicketInOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInOrders_Tickets_OrderId",
                table: "TicketInOrders",
                column: "OrderId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInOrders_Orders_TicketId",
                table: "TicketInOrders",
                column: "TicketId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
