using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using RhinoTicketingSystem.Components.Layout;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class AddTblDocumentSerialize
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
        protected MainLayout MainLayout { get; set; }

        protected override async Task OnInitializedAsync()
        {
            tblDocumentSerialize = new RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentSerialize();

            tblDepartmentsForDepartmentId = await db_a79800_ticketService.GetTblDepartments();
        }
        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentSerialize tblDocumentSerialize;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblDepartment> tblDepartmentsForDepartmentId;

        protected async Task FormSubmit()
        {
            try
            {
                tblDocumentSerialize.CreatedBy = Security.User.Name;
                await db_a79800_ticketService.CreateTblDocumentSerialize(tblDocumentSerialize);
                DialogService.Close(tblDocumentSerialize);
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