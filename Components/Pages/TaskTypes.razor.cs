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
    public partial class TaskTypes
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

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> taskTypes;

        protected RadzenDataGrid<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            taskTypes = await db_a79800_ticketService.GetTaskTypes(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            taskTypes = await db_a79800_ticketService.GetTaskTypes(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddTaskType>("Add TaskType", null);
            await grid0.Reload();
        }

        protected async Task EditRow(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType args)
        {
            await DialogService.OpenAsync<EditTaskType>("Edit TaskType", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TaskType taskType)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await db_a79800_ticketService.DeleteTaskType(taskType.Id);

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
                    Detail = $"Unable to delete TaskType"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await db_a79800_ticketService.ExportTaskTypesToCSV(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "TaskTypes");
            }

            if (args == null || args.Value == "xlsx")
            {
                await db_a79800_ticketService.ExportTaskTypesToExcel(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "TaskTypes");
            }
        }
    }
}