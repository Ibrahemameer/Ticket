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

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class AddDocumentHeader
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

        [Parameter]
        public int? ProjectId { get; set; }
        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblProject> tblProjectsForProjectId;
        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            tblDocumentHeder = new RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentHeder();

            if (ProjectId.HasValue)
            {
                tblDocumentHeder.ProjectId = ProjectId.Value;
                // Load only the specific project
                tblProjectsForProjectId = await db_a79800_ticketService.GetTblProjects(new Query
                {
                    Filter = "i => i.Id == @0",
                    FilterParameters = new object[] { ProjectId.Value }
                });

                // Set initial document serial
                tblDocumentHeder.DocumentSerial = await GetNextDocumentSerial(ProjectId.Value);
            }
            else
            {
                // Load all projects if no ProjectId specified
                tblProjectsForProjectId = await db_a79800_ticketService.GetTblProjects();
            }
        }
        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentHeder tblDocumentHeder;

        // Removed duplicate declaration of tblProjectsForProjectId

        protected async Task FormSubmit()
        {
            try
            {
                // Get fresh serial number before saving

                tblDocumentHeder.DocumentSerial = await GetNextDocumentSerial(ProjectId.Value);


                tblDocumentHeder.Createdin = DateTime.Now;
                tblDocumentHeder.CreatedBy = Security.User.Name;

                await db_a79800_ticketService.CreateTblDocumentHeder(tblDocumentHeder);
                DialogService.Close(tblDocumentHeder);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }


        protected async System.Threading.Tasks.Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }


        protected bool hasChanges = false;
        protected bool canEdit = true;

        [Inject]
        protected SecurityService Security { get; set; }

        private async Task<string> GetNextDocumentSerial(int projectId)
        {
            // First get the project to get its document serial pattern
            var project = await db_a79800_ticketService.GetTblProjectById(projectId);
            if (project == null || string.IsNullOrEmpty(project.DocumentSerial))
            {
                return null; // or handle this case as needed
            }

            // Get the last document header for this project
            var lastDocument = await db_a79800_ticketService.GetTblDocumentHeders(new Query
            {
                Filter = "ProjectId == @0",
                FilterParameters = new object[] { projectId },
                OrderBy = "Id desc",
                Top = 1
            });

            var baseDocument = lastDocument.FirstOrDefault();

            if (baseDocument != null && !string.IsNullOrEmpty(baseDocument.DocumentSerial))
            {
                // Extract and increment number
                string numberPart = baseDocument.DocumentSerial.Substring(baseDocument.DocumentSerial.LastIndexOf('-') + 1);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    return $"{project.DocumentSerial}-{(lastNumber + 1):D3}";
                }
            }

            // If no previous documents exist, start with 001
            return $"{project.DocumentSerial}-001";
        }




    }


}