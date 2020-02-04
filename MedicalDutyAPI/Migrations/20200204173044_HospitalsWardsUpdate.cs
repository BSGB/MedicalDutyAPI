using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalDutyAPI.Migrations
{
    public partial class HospitalsWardsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "hospital_id",
                table: "users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ward_id",
                table: "users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "hospitals",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    street = table.Column<string>(nullable: true),
                    zip = table.Column<string>(nullable: true),
                    city = table.Column<string>(nullable: true),
                    district = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hospitals", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wards",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<int>(nullable: false),
                    hospital_id = table.Column<string>(nullable: true),
                    HospitalId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wards", x => x.id);
                    table.ForeignKey(
                        name: "FK_wards_hospitals_HospitalId1",
                        column: x => x.HospitalId1,
                        principalTable: "hospitals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_hospital_id",
                table: "users",
                column: "hospital_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_ward_id",
                table: "users",
                column: "ward_id");

            migrationBuilder.CreateIndex(
                name: "IX_wards_HospitalId1",
                table: "wards",
                column: "HospitalId1");

            migrationBuilder.AddForeignKey(
                name: "FK_users_hospitals_hospital_id",
                table: "users",
                column: "hospital_id",
                principalTable: "hospitals",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_wards_ward_id",
                table: "users",
                column: "ward_id",
                principalTable: "wards",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_hospitals_hospital_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_wards_ward_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "wards");

            migrationBuilder.DropTable(
                name: "hospitals");

            migrationBuilder.DropIndex(
                name: "IX_users_hospital_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_ward_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "hospital_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "ward_id",
                table: "users");
        }
    }
}
