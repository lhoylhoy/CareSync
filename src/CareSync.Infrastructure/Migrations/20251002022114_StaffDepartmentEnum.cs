using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StaffDepartmentEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add a temporary int column
            migrationBuilder.AddColumn<int>(
                name: "DepartmentInt",
                table: "Staff",
                type: "int",
                nullable: false,
                defaultValue: 99); // Default to Other

            // 2. Map existing string Department values to enum ints
            // NOTE: This uses broad LIKE patterns and direct matches; adjust as needed if you have additional legacy values.
            migrationBuilder.Sql(@"
UPDATE Staff
SET DepartmentInt = CASE
    WHEN Department IS NULL OR LTRIM(RTRIM(Department)) = '' THEN 99 -- Other
    WHEN Department IN ('Administration','Admin') THEN 1
    WHEN Department IN ('Nursing','Nurse') THEN 2
    WHEN Department = 'Pharmacy' THEN 3
    WHEN Department IN ('Laboratory','Lab') THEN 4
    WHEN Department = 'Radiology' THEN 5
    WHEN Department IN ('Cardiology','Cardio') THEN 6
    WHEN Department IN ('Emergency','ER','Emerg') THEN 7
    WHEN Department IN ('Pediatrics','Peds') THEN 8
    WHEN Department = 'Oncology' THEN 9
    WHEN Department = 'Surgery' THEN 10
    WHEN Department = 'Billing' THEN 11
    WHEN Department IN ('IT','I.T.','Information Technology','Tech') THEN 12
    WHEN Department = 'Maintenance' THEN 13
    WHEN Department = 'Security' THEN 14
    ELSE 99 -- Other
END;

UPDATE Staff SET DepartmentInt = 99 WHERE DepartmentInt IS NULL;

");

            // 3. Drop old string column
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Staff");

            // 4. Rename temp column to Department
            migrationBuilder.RenameColumn(
                name: "DepartmentInt",
                table: "Staff",
                newName: "Department");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse: add string column, copy values back (as names), drop int, rename
            migrationBuilder.AddColumn<string>(
                name: "DepartmentString",
                table: "Staff",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Other");

            migrationBuilder.Sql(@"
UPDATE Staff SET DepartmentString = CASE Department
    WHEN 1 THEN 'Administration'
    WHEN 2 THEN 'Nursing'
    WHEN 3 THEN 'Pharmacy'
    WHEN 4 THEN 'Laboratory'
    WHEN 5 THEN 'Radiology'
    WHEN 6 THEN 'Cardiology'
    WHEN 7 THEN 'Emergency'
    WHEN 8 THEN 'Pediatrics'
    WHEN 9 THEN 'Oncology'
    WHEN 10 THEN 'Surgery'
    WHEN 11 THEN 'Billing'
    WHEN 12 THEN 'It'
    WHEN 13 THEN 'Maintenance'
    WHEN 14 THEN 'Security'
    ELSE 'Other'
END;

");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Staff");

            migrationBuilder.RenameColumn(
                name: "DepartmentString",
                table: "Staff",
                newName: "Department");
        }
    }
}
