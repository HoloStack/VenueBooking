using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace venueBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddVenueEventTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Venues_EventTypes_EventTypeId",
                table: "Venues");

            migrationBuilder.DropIndex(
                name: "IX_Venues_EventTypeId",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "EventTypeId",
                table: "Venues");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Events",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VenueEventTypes",
                columns: table => new
                {
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    EventTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueEventTypes", x => new { x.VenueId, x.EventTypeId });
                    table.ForeignKey(
                        name: "FK_VenueEventTypes_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "EventTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VenueEventTypes_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VenueId_BookingDate",
                table: "Bookings",
                columns: new[] { "VenueId", "BookingDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VenueEventTypes_EventTypeId",
                table: "VenueEventTypes",
                column: "EventTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VenueEventTypes");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_VenueId_BookingDate",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "EventTypeId",
                table: "Venues",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "ImageUrl",
                keyValue: null,
                column: "ImageUrl",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Events",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_EventTypeId",
                table: "Venues",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VenueId",
                table: "Bookings",
                column: "VenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Venues_EventTypes_EventTypeId",
                table: "Venues",
                column: "EventTypeId",
                principalTable: "EventTypes",
                principalColumn: "EventTypeId");
        }
    }
}
