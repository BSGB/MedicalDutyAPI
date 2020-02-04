using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalDutyAPI.Migrations
{
    public partial class HospitalsWardsUpdateV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_hospitals_hospital_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_wards_hospitals_HospitalId1",
                table: "wards");

            migrationBuilder.DropIndex(
                name: "IX_wards_HospitalId1",
                table: "wards");

            migrationBuilder.DropIndex(
                name: "IX_users_hospital_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "HospitalId1",
                table: "wards");

            migrationBuilder.DropColumn(
                name: "hospital_id",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "hospital_id",
                table: "wards",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_wards_hospital_id",
                table: "wards",
                column: "hospital_id");

            migrationBuilder.AddForeignKey(
                name: "FK_wards_hospitals_hospital_id",
                table: "wards",
                column: "hospital_id",
                principalTable: "hospitals",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_wards_hospitals_hospital_id",
                table: "wards");

            migrationBuilder.DropIndex(
                name: "IX_wards_hospital_id",
                table: "wards");

            migrationBuilder.AlterColumn<string>(
                name: "hospital_id",
                table: "wards",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "HospitalId1",
                table: "wards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "hospital_id",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_wards_HospitalId1",
                table: "wards",
                column: "HospitalId1");

            migrationBuilder.CreateIndex(
                name: "IX_users_hospital_id",
                table: "users",
                column: "hospital_id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_hospitals_hospital_id",
                table: "users",
                column: "hospital_id",
                principalTable: "hospitals",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_wards_hospitals_HospitalId1",
                table: "wards",
                column: "HospitalId1",
                principalTable: "hospitals",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
