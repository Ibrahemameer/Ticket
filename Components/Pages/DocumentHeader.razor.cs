using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using RhinoTicketingSystem.Models.db_a79800_ticket;
using Task = System.Threading.Tasks.Task;
using RhinoTicketingSystem.Components.Pages.Archieving;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class DocumentHeader
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

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentHeder> tblDocumentHeders;

        protected RadzenDataGrid<RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentHeder> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }
        [Parameter]
        public int? ProjectId { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            if (ProjectId.HasValue)
            {
                var query = new Query
                {
                    Filter = "i => i.ProjectId == @0",
                    FilterParameters = new object[] { ProjectId.Value },
                    Expand = "TblProject"
                };

                tblDocumentHeders = await db_a79800_ticketService.GetTblDocumentHeders(query);
            }
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await grid0.GoToPage(0);

            var filters = new List<string>();
            var parameters = new List<object>();

            if (ProjectId.HasValue)
            {
                filters.Add("ProjectId == @" + parameters.Count);
                parameters.Add(ProjectId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                filters.Add($@"(DocumentSerial.Contains(@{parameters.Count}) || 
                      Name.Contains(@{parameters.Count}) || 
                      Subject.Contains(@{parameters.Count}) || 
                      TblProject.Name.Contains(@{parameters.Count}) || 
                      CreatedBy.Contains(@{parameters.Count}))");
                parameters.Add(search);
            }

            var query = new Query
            {
                Filter = filters.Any() ? string.Join(" && ", filters) : null,
                FilterParameters = parameters.ToArray(),
                Expand = "TblProject"
            };

            tblDocumentHeders = await db_a79800_ticketService.GetTblDocumentHeders(query);
        }



        protected override async Task OnInitializedAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var segments = uri.Segments;

            if (segments.Length > 2 && int.TryParse(segments[2], out int projectId))
            {
                ProjectId = projectId;

                var query = new Query
                {
                    Filter = "i => i.ProjectId == @0",
                    FilterParameters = new object[] { ProjectId },
                    Expand = "TblProject"
                };

                tblDocumentHeders = await db_a79800_ticketService.GetTblDocumentHeders(query);
            }
            else
            {
                // Load all documents if no ProjectId is specified
                var query = new Query
                {
                    Expand = "TblProject"
                };

                tblDocumentHeders = await db_a79800_ticketService.GetTblDocumentHeders(query);
            }
        }


        protected async Task AddButtonClick(MouseEventArgs args)
        {
            var parameters = new Dictionary<string, object>
            {
                { "ProjectId", ProjectId }
            };
            await DialogService.OpenAsync<AddDocumentHeader>("Add Document", parameters);
            await grid0.Reload();
        }

        protected async Task EditRow(RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentHeder args)
        {
            await DialogService.OpenAsync<EditDocumentHeader>("Edit TblDocumentHeder", new Dictionary<string, object> { { "Id", args.Id } });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentHeder tblDocumentHeder)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await db_a79800_ticketService.DeleteTblDocumentHeder(tblDocumentHeder.Id);

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
                    Detail = $"Unable to delete TblDocumentHeder"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await db_a79800_ticketService.ExportTblDocumentHedersToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "TblProject",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "TblDocumentHeders");
            }

            if (args == null || args.Value == "xlsx")
            {
                await db_a79800_ticketService.ExportTblDocumentHedersToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "TblProject",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "TblDocumentHeders");
            }
        }
        private void GoBack()
        {
            NavigationManager.NavigateTo("projects-department");
        }
        protected async Task OpenAttachmentForm(TblDocumentHeder document)
        {
            var parameters = new Dictionary<string, object>
                 {
                     { "DocumentId", document.Id }  // Make sure this matches the Parameter name in AddDocumentAttachment
                 };

            await DialogService.OpenAsync<UploadDocumentAttachment>("Add Document Attachment",
                parameters,
                new DialogOptions
                {
                    Width = "80vw",
                    Height = "80vh",
                    Left = "10vw",
                    Top = "10vh",
                    Resizable = true,
                    Draggable = true
                });

            await grid0.Reload();
        }


    }
}