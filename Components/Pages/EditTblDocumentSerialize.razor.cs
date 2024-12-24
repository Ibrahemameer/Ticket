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
    public partial class EditTblDocumentSerialize
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
            tblDocumentSerialize = await db_a79800_ticketService.GetTblDocumentSerializeById(Id);

            tblDepartmentsForDepartmentId = await db_a79800_ticketService.GetTblDepartments();
        }
        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentSerialize tblDocumentSerialize;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblDepartment> tblDepartmentsForDepartmentId;

        protected async Task FormSubmit()
        {
            try
            {
                await db_a79800_ticketService.UpdateTblDocumentSerialize(Id, tblDocumentSerialize);
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


        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
           db_a79800_ticketService.Reset();
            hasChanges = false;
            canEdit = true;

            tblDocumentSerialize = await db_a79800_ticketService.GetTblDocumentSerializeById(Id);
        }
    }
}