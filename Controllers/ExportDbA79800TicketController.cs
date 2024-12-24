using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using RhinoTicketingSystem.Data;

namespace RhinoTicketingSystem.Controllers
{
    public partial class Exportdb_a79800_ticketController : ExportController
    {
        private readonly db_a79800_ticketContext context;
        private readonly db_a79800_ticketService service;

        public Exportdb_a79800_ticketController(db_a79800_ticketContext context, db_a79800_ticketService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/db_a79800_ticket/tasks/csv")]
        [HttpGet("/export/db_a79800_ticket/tasks/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTasksToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTasks(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tasks/excel")]
        [HttpGet("/export/db_a79800_ticket/tasks/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTasksToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTasks(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/taskstatuses/csv")]
        [HttpGet("/export/db_a79800_ticket/taskstatuses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskStatusesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTaskStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/taskstatuses/excel")]
        [HttpGet("/export/db_a79800_ticket/taskstatuses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskStatusesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTaskStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tasktypes/csv")]
        [HttpGet("/export/db_a79800_ticket/tasktypes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskTypesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTaskTypes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tasktypes/excel")]
        [HttpGet("/export/db_a79800_ticket/tasktypes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskTypesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTaskTypes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblcategories/csv")]
        [HttpGet("/export/db_a79800_ticket/tblcategories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblCategoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblCategories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblcategories/excel")]
        [HttpGet("/export/db_a79800_ticket/tblcategories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblCategoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblCategories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblchartofaccounts/csv")]
        [HttpGet("/export/db_a79800_ticket/tblchartofaccounts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblChartOfAccountsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblChartOfAccounts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblchartofaccounts/excel")]
        [HttpGet("/export/db_a79800_ticket/tblchartofaccounts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblChartOfAccountsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblChartOfAccounts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblemployees/csv")]
        [HttpGet("/export/db_a79800_ticket/tblemployees/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblEmployeesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblEmployees(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblemployees/excel")]
        [HttpGet("/export/db_a79800_ticket/tblemployees/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblEmployeesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblEmployees(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblengineers/csv")]
        [HttpGet("/export/db_a79800_ticket/tblengineers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblEngineersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblEngineers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblengineers/excel")]
        [HttpGet("/export/db_a79800_ticket/tblengineers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblEngineersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblEngineers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblreassigntickets/csv")]
        [HttpGet("/export/db_a79800_ticket/tblreassigntickets/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblReassignTicketsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblReassignTickets(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblreassigntickets/excel")]
        [HttpGet("/export/db_a79800_ticket/tblreassigntickets/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblReassignTicketsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblReassignTickets(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblstatuses/csv")]
        [HttpGet("/export/db_a79800_ticket/tblstatuses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblStatusesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblstatuses/excel")]
        [HttpGet("/export/db_a79800_ticket/tblstatuses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblStatusesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbltickets/csv")]
        [HttpGet("/export/db_a79800_ticket/tbltickets/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblTicketsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblTickets(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbltickets/excel")]
        [HttpGet("/export/db_a79800_ticket/tbltickets/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblTicketsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblTickets(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbltaskdetails/csv")]
        [HttpGet("/export/db_a79800_ticket/tbltaskdetails/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblTaskDetailsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblTaskDetails(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbltaskdetails/excel")]
        [HttpGet("/export/db_a79800_ticket/tbltaskdetails/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblTaskDetailsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblTaskDetails(), Request.Query, false), fileName);
        }
        [HttpGet("/export/db_a79800_ticket/tbldocumentattachments/csv")]
        [HttpGet("/export/db_a79800_ticket/tbldocumentattachments/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblDocumentAttachmentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblDocumentAttachments(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbldocumentattachments/excel")]
        [HttpGet("/export/db_a79800_ticket/tbldocumentattachments/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblDocumentAttachmentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblDocumentAttachments(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbldocumentheders/csv")]
        [HttpGet("/export/db_a79800_ticket/tbldocumentheders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblDocumentHedersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblDocumentHeders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbldocumentheders/excel")]
        [HttpGet("/export/db_a79800_ticket/tbldocumentheders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblDocumentHedersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblDocumentHeders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblprojects/csv")]
        [HttpGet("/export/db_a79800_ticket/tblprojects/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblProjectsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblProjects(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tblprojects/excel")]
        [HttpGet("/export/db_a79800_ticket/tblprojects/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblProjectsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblProjects(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbldocumentserializes/csv")]
        [HttpGet("/export/db_a79800_ticket/tbldocumentserializes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblDocumentSerializesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblDocumentSerializes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbldocumentserializes/excel")]
        [HttpGet("/export/db_a79800_ticket/tbldocumentserializes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblDocumentSerializesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblDocumentSerializes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbldepartments/csv")]
        [HttpGet("/export/db_a79800_ticket/tbldepartments/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblDepartmentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTblDepartments(), Request.Query, false), fileName);
        }

        [HttpGet("/export/db_a79800_ticket/tbldepartments/excel")]
        [HttpGet("/export/db_a79800_ticket/tbldepartments/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTblDepartmentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTblDepartments(), Request.Query, false), fileName);
        }
    }
}
