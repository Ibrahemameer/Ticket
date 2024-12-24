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
    public partial class EditDocumentHeader
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
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            tblDocumentHeder = await db_a79800_ticketService.GetTblDocumentHederById(Id);

            tblProjectsForProjectId = await db_a79800_ticketService.GetTblProjects();
        }
        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentHeder tblDocumentHeder;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblProject> tblProjectsForProjectId;

        protected async Task FormSubmit()
        {
            try
            {
                await db_a79800_ticketService.UpdateTblDocumentHeder(Id, tblDocumentHeder);
                DialogService.Close(tblDocumentHeder);
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


        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
           db_a79800_ticketService.Reset();
            hasChanges = false;
            canEdit = true;

            tblDocumentHeder = await db_a79800_ticketService.GetTblDocumentHederById(Id);
        }
    }
}