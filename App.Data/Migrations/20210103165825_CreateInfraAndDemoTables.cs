using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App.Data.Migrations
{
    public partial class CreateInfraAndDemoTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUsers",
                table: "AppUsers");

            migrationBuilder.RenameTable(
                name: "AppUsers",
                newName: "Infra.User");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Infra.User",
                table: "Infra.User",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Demo.MQExecution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageID = table.Column<int>(nullable: false),
                    ExecutionResult = table.Column<string>(nullable: true),
                    SuccessDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo.MQExecution", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Demo.MQMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FetchDate = table.Column<DateTime>(nullable: true),
                    QueueDate = table.Column<DateTime>(nullable: true),
                    MessageData = table.Column<string>(nullable: true),
                    ExecuteDate = table.Column<DateTime>(nullable: true),
                    MSBatchID = table.Column<string>(nullable: true),
                    SuccessDate = table.Column<DateTime>(nullable: true),
                    FailureDate = table.Column<DateTime>(nullable: true),
                    FailureMessage = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo.MQMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Infra.AccessType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infra.AccessType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Infra.Assembly",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infra.Assembly", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Infra.CachingType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infra.CachingType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Infra.Class",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infra.Class", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Infra.Service",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceClassID = table.Column<int>(nullable: false),
                    ModelClassID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    MethodName = table.Column<string>(nullable: true),
                    CachingTypeID = table.Column<int>(nullable: false),
                    AccessTypeID = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infra.Service", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Demo.MQExecution");

            migrationBuilder.DropTable(
                name: "Demo.MQMessage");

            migrationBuilder.DropTable(
                name: "Infra.AccessType");

            migrationBuilder.DropTable(
                name: "Infra.Assembly");

            migrationBuilder.DropTable(
                name: "Infra.CachingType");

            migrationBuilder.DropTable(
                name: "Infra.Class");

            migrationBuilder.DropTable(
                name: "Infra.Service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Infra.User",
                table: "Infra.User");

            migrationBuilder.RenameTable(
                name: "Infra.User",
                newName: "AppUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUsers",
                table: "AppUsers",
                column: "Id");
        }
    }
}
