using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class Tasks
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public db_a79800_ticketService db_a79800_ticketService { get; set; }

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.Task> tasks;

        protected RadzenDataGrid<RhinoTicketingSystem.Models.db_a79800_ticket.Task> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            tasks = await db_a79800_ticketService.GetTasks(new Query { Filter = $@"i => i.Title.Contains(@0)", FilterParameters = new object[] { search }, Expand = "TblEngineer,TaskType,TaskStatus" });
        }

            protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> tblEngineersForEngineerId;

            protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> taskTypesForTypeId;

            protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> taskStatusesForStatusId;
        protected override async Task OnInitializedAsync()
        {
            tasks = await db_a79800_ticketService.GetTasks(new Query { Filter = $@"i => i.Title.Contains(@0)", FilterParameters = new object[] { search }, Expand = "TblEngineer,TaskType,TaskStatus" });

            tblEngineersForEngineerId = await db_a79800_ticketService.GetTblEngineers();

            taskTypesForTypeId = await db_a79800_ticketService.GetTaskTypes();

            taskStatusesForStatusId = await db_a79800_ticketService.GetTaskStatuses();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await grid0.InsertRow(new RhinoTicketingSystem.Models.db_a79800_ticket.Task());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.Task task)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await db_a79800_ticketService.DeleteTask(task.Id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Task"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await db_a79800_ticketService.ExportTasksToCSV(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "TblEngineer,TaskType,TaskStatus",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Tasks");
            }

            if (args == null || args.Value == "xlsx")
            {
                await db_a79800_ticketService.ExportTasksToExcel(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "TblEngineer,TaskType,TaskStatus",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Tasks");
            }
        }

        protected async Task GridRowUpdate(RhinoTicketingSystem.Models.db_a79800_ticket.Task args)
        {
            try
            {
                await db_a79800_ticketService.UpdateTask(args.Id, args);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                      Severity = NotificationSeverity.Error,
                      Summary = $"Error",
                      Detail = $"Unable to update Task"
                });
            }
        }

        protected async Task GridRowCreate(RhinoTicketingSystem.Models.db_a79800_ticket.Task args)
        {
            try
            {
                await db_a79800_ticketService.CreateTask(args);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                      Severity = NotificationSeverity.Error,
                      Summary = $"Error",
                      Detail = $"Unable to create Task"
                });
            }
            await grid0.Reload();
        }

        protected async Task EditButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.Task data)
        {
            await grid0.EditRow(data);
        }

        protected async Task SaveButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.Task data)
        {
            await grid0.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.Task data)
        {
            grid0.CancelEditRow(data);
            await db_a79800_ticketService.CancelTaskChanges(data);
        }

        protected RhinoTicketingSystem.Models.db_a79800_ticket.Task taskChild;
        protected async Task GetChildData(RhinoTicketingSystem.Models.db_a79800_ticket.Task args)
        {
            taskChild = args;
            var TblTaskDetailsResult = await db_a79800_ticketService.GetTblTaskDetails(new Query { Filter = $@"i => i.TaskId == {args.Id}", Expand = "Task,TblEngineer" });
            if (TblTaskDetailsResult != null)
            {
                args.TblTaskDetails = TblTaskDetailsResult.ToList();
            }
        }
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail tblTaskDetailTblTaskDetails;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.Task> tasksForTaskIdTblTaskDetails;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> tblEngineersForEngineerIdTblTaskDetails;

        protected RadzenDataGrid<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail> TblTaskDetailsDataGrid;

        protected async Task TblTaskDetailsAddButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.Task data)
        {

            await TblTaskDetailsDataGrid.InsertRow(new RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail());

        }

        protected async Task TblTaskDetailsCancelButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail data)
        {
            TblTaskDetailsDataGrid.CancelEditRow(data);
            await db_a79800_ticketService.CancelTblTaskDetailChanges(data);
            await TblTaskDetailsDataGrid.Reload();
        }

        protected async Task TblTaskDetailsSaveButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail data)
        {
            await TblTaskDetailsDataGrid.UpdateRow(data);
            await db_a79800_ticketService.UpdateTblTaskDetail(data.Id, data);
            await TblTaskDetailsDataGrid.Reload();
        }

        protected async Task TblTaskDetailsEditButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail data)
        {

            tasksForTaskIdTblTaskDetails = await db_a79800_ticketService.GetTasks();

            tblEngineersForEngineerIdTblTaskDetails = await db_a79800_ticketService.GetTblEngineers();

            await TblTaskDetailsDataGrid.EditRow(data);
        }

        protected async Task TblTaskDetailsDeleteButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail tblTaskDetail)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await db_a79800_ticketService.DeleteTblTaskDetail(tblTaskDetail.Id);

                    await GetChildData(taskChild);

                    if (deleteResult != null)
                    {
                        await TblTaskDetailsDataGrid.Reload();
                    }
                }
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete TblTaskDetail"
                });
            }
        }

        string lastFilter;

        [Inject]
        protected SecurityService Security { get; set; }
        protected async void Grid0Render(DataGridRenderEventArgs<RhinoTicketingSystem.Models.db_a79800_ticket.Task> args)
        {
            if (grid0.Query.Filter != lastFilter)
            {
                taskChild = grid0.View.FirstOrDefault();
            }

            if (grid0.Query.Filter != lastFilter && taskChild != null)
            {
                await grid0.SelectRow(taskChild);
            }

            lastFilter = grid0.Query.Filter;
        }
    }
}