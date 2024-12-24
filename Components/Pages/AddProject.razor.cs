using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using RhinoTicketingSystem.Services;
using RhinoTicketingSystem.Models.db_a79800_ticket;
using Task = System.Threading.Tasks.Task;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class AddProject
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

        [Inject]
        protected MenuService MenuService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            tblProject = new RhinoTicketingSystem.Models.db_a79800_ticket.TblProject();
            LoadBranchOptions();
            // Load all document serials without filtering
            documentSerials = await db_a79800_ticketService.GetTblDocumentSerializes();
        }
        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblProject tblProject;
        protected IEnumerable<TblDocumentSerialize> documentSerials;

        private List<string> branchOptions;

        private void OnSerialChange(object value)
        {
            if (value != null)
            {
                var selectedSerial = documentSerials.FirstOrDefault(s => s.Id == (int)value);
                if (selectedSerial != null)
                {
                    tblProject.DocumentSerialId = selectedSerial.Id;
                    tblProject.DocumentSerial = selectedSerial.Combination;
                }
            }
        }
        private void LoadBranchOptions()
        {
            var archiveDept = MenuService.GetMenuItems()
                .FirstOrDefault(m => m.Text == "Archive Department");

            branchOptions = archiveDept?.Children
                .Select(c => c.Text)
                .ToList();
        }
        protected async Task FormSubmit()
        {
            try
            {
                // Set creation metadata
                tblProject.CreatedIn = DateTime.Now;
                tblProject.CreatedBy = Security.User.Name;

                await db_a79800_ticketService.CreateTblProject(tblProject);
                DialogService.Close(tblProject);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }


        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }


        protected bool hasChanges = false;
        protected bool canEdit = true;

        [Inject]
        protected SecurityService Security { get; set; }
    }
}