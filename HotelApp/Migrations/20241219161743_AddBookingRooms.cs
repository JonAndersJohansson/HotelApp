using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRoom_Bookings_BookingId",
                table: "BookingRoom");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRoom_Rooms_RoomId",
                table: "BookingRoom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingRoom",
                table: "BookingRoom");

            migrationBuilder.DropColumn(
                name: "RoomNumberAsID",
                table: "BookingRoom");

            migrationBuilder.RenameTable(
                name: "BookingRoom",
                newName: "BookingRooms");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRoom_RoomId",
                table: "BookingRooms",
                newName: "IX_BookingRooms_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRoom_BookingId",
                table: "BookingRooms",
                newName: "IX_BookingRooms_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingRooms",
                table: "BookingRooms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRooms_Bookings_BookingId",
                table: "BookingRooms",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRooms_Rooms_RoomId",
                table: "BookingRooms",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRooms_Bookings_BookingId",
                table: "BookingRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRooms_Rooms_RoomId",
                table: "BookingRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingRooms",
                table: "BookingRooms");

            migrationBuilder.RenameTable(
                name: "BookingRooms",
                newName: "BookingRoom");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRooms_RoomId",
                table: "BookingRoom",
                newName: "IX_BookingRoom_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRooms_BookingId",
                table: "BookingRoom",
                newName: "IX_BookingRoom_BookingId");

            migrationBuilder.AddColumn<short>(
                name: "RoomNumberAsID",
                table: "BookingRoom",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingRoom",
                table: "BookingRoom",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRoom_Bookings_BookingId",
                table: "BookingRoom",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRoom_Rooms_RoomId",
                table: "BookingRoom",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
