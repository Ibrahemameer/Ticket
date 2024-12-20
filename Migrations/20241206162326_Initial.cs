using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhinoTicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "OneDriveArchivingHeaders",
                columns: table => new
                {
                    ArchiveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneDriveArchivingHeaders", x => x.ArchiveId);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatuses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Categories",
                schema: "dbo",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Categories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_ChartOfAccounts",
                schema: "dbo",
                columns: table => new
                {
                    Chart_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chart_Code = table.Column<int>(type: "int", nullable: true),
                    Chart_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account_Level = table.Column<int>(type: "int", nullable: true),
                    ParentAccount = table.Column<int>(type: "int", nullable: true),
                    Accoutn_Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Direction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsConnectedWithContact = table.Column<bool>(type: "bit", nullable: true),
                    IsConnectedWithCostCenter = table.Column<bool>(type: "bit", nullable: true),
                    IsConnectedWithBusinessType = table.Column<bool>(type: "bit", nullable: true),
                    IsManualJournalNotAllowed = table.Column<bool>(type: "bit", nullable: true),
                    IsConnectedWithProject = table.Column<bool>(type: "bit", nullable: true),
                    FinancialStatementClassificationGroupId = table.Column<int>(type: "int", nullable: true),
                    AccountingSerial = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_ChartOfAccounts", x => x.Chart_ID);
                    table.ForeignKey(
                        name: "FK_Tbl_ChartOfAccounts_Tbl_ChartOfAccounts_ParentAccount",
                        column: x => x.ParentAccount,
                        principalSchema: "dbo",
                        principalTable: "Tbl_ChartOfAccounts",
                        principalColumn: "Chart_ID");
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Employees",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelNumber = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Site = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Center = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Employees", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Engineers",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EngineerCode = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Engineers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Status",
                schema: "dbo",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Status", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "OneDriveArchivingDetails",
                columns: table => new
                {
                    ArchiveDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArchiveId = table.Column<int>(type: "int", nullable: false),
                    AttachedFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachedFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachedFileSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneDriveArchivingDetails", x => x.ArchiveDetailId);
                    table.ForeignKey(
                        name: "FK_OneDriveArchivingDetails_OneDriveArchivingHeaders_ArchiveId",
                        column: x => x.ArchiveId,
                        principalTable: "OneDriveArchivingHeaders",
                        principalColumn: "ArchiveId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngineerId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskStatuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "dbo",
                        principalTable: "TaskStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "dbo",
                        principalTable: "TaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Tbl_Engineers_EngineerId",
                        column: x => x.EngineerId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Engineers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Tickets",
                schema: "dbo",
                columns: table => new
                {
                    TicketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttchedFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttchedFileSize = table.Column<long>(type: "bigint", nullable: true),
                    EngineerComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    EngineerId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Tickets", x => x.TicketID);
                    table.ForeignKey(
                        name: "FK_Tbl_Tickets_Tbl_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Categories",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK_Tbl_Tickets_Tbl_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Employees",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Tbl_Tickets_Tbl_Engineers_EngineerId",
                        column: x => x.EngineerId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Engineers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TblTaskDetails",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    EngineerId = table.Column<int>(type: "int", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TaskstatusId = table.Column<int>(type: "int", nullable: true),
                    taskStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngineerComment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTaskDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TblTaskDetails_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "dbo",
                        principalTable: "Tasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TblTaskDetails_Tbl_Engineers_EngineerId",
                        column: x => x.EngineerId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Engineers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Tbl_ReassignTicket",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    EngineerId = table.Column<int>(type: "int", nullable: true),
                    ReassignedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReassignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReassignedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProblemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_ReassignTicket", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tbl_ReassignTicket_Tbl_Engineers_EngineerId",
                        column: x => x.EngineerId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Engineers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Tbl_ReassignTicket_Tbl_Status_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Status",
                        principalColumn: "StatusId");
                    table.ForeignKey(
                        name: "FK_Tbl_ReassignTicket_Tbl_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Tickets",
                        principalColumn: "TicketID");
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Ticketattachments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    AttachedFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachedFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    attachedFileSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Ticketattachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tbl_Ticketattachments_Tbl_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "dbo",
                        principalTable: "Tbl_Tickets",
                        principalColumn: "TicketID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OneDriveArchivingDetails_ArchiveId",
                table: "OneDriveArchivingDetails",
                column: "ArchiveId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EngineerId",
                schema: "dbo",
                table: "Tasks",
                column: "EngineerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StatusId",
                schema: "dbo",
                table: "Tasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TypeId",
                schema: "dbo",
                table: "Tasks",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_ChartOfAccounts_ParentAccount",
                schema: "dbo",
                table: "Tbl_ChartOfAccounts",
                column: "ParentAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_ReassignTicket_EngineerId",
                schema: "dbo",
                table: "Tbl_ReassignTicket",
                column: "EngineerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_ReassignTicket_StatusId",
                schema: "dbo",
                table: "Tbl_ReassignTicket",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_ReassignTicket_TicketId",
                schema: "dbo",
                table: "Tbl_ReassignTicket",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Ticketattachments_TicketId",
                schema: "dbo",
                table: "Tbl_Ticketattachments",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Tickets_CategoryID",
                schema: "dbo",
                table: "Tbl_Tickets",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Tickets_EmployeeId",
                schema: "dbo",
                table: "Tbl_Tickets",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Tickets_EngineerId",
                schema: "dbo",
                table: "Tbl_Tickets",
                column: "EngineerId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTaskDetails_EngineerId",
                schema: "dbo",
                table: "TblTaskDetails",
                column: "EngineerId");

            migrationBuilder.CreateIndex(
                name: "IX_TblTaskDetails_TaskId",
                schema: "dbo",
                table: "TblTaskDetails",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OneDriveArchivingDetails");

            migrationBuilder.DropTable(
                name: "Tbl_ChartOfAccounts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_ReassignTicket",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_Ticketattachments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TblTaskDetails",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OneDriveArchivingHeaders");

            migrationBuilder.DropTable(
                name: "Tbl_Status",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_Tickets",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_Categories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_Employees",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TaskStatuses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TaskTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tbl_Engineers",
                schema: "dbo");
        }
    }
}
