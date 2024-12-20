using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class Index
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
        protected SecurityService Security { get; set; }
        protected RadzenDataGrid<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> grid0;


        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> tblTickets;



        protected string search = "";

        
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            tblTickets = await db_a79800_ticketService.GetTblTicketsForUser(Security.User.Email, new Query { Filter = $@"i => i.TicketNumber.Contains(@0) || i.TicketHeader.Contains(@0) || i.TicketDescription.Contains(@0) || i.Attachment.Contains(@0) || i.TicketStatus.Contains(@0) || i.UserEmail.Contains(@0) ", FilterParameters = new object[] { search }, Expand = "TblCategory,TblEngineer,TblEmployee" });
        }
        protected override async Task OnInitializedAsync()
        {
            tblTickets = await db_a79800_ticketService.GetTblTicketsForUser(Security.User.Email, new Query { Filter = $@"i => i.TicketNumber.Contains(@0) || i.TicketHeader.Contains(@0) || i.TicketDescription.Contains(@0) || i.Attachment.Contains(@0) || i.TicketStatus.Contains(@0) || i.UserEmail.Contains(@0) ", FilterParameters = new object[] { search }, Expand = "TblCategory,TblEngineer,TblEmployee" });
        }

        protected async Task OpenTaskPageForEngineer(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket args)
        {
            await DialogService.OpenAsync<EngineerProceedWithAssignedTicket>("Ticket Proceed", new Dictionary<string, object> { { "TicketId", args.TicketId } });
        }
       
        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddTblTicket>("Describe Your Issue", null);
            await grid0.Reload();
        }
        protected async Task OpenAttachmentForm(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket args)
        {
            await DialogService.OpenAsync<AddTicketAttachments>("Add ticket attachment to my ticket", new Dictionary<string, object> { { "TicketId", args.TicketId } });
        }
    }

    
}