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

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class Dashboard
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


        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> tblTicketForDashboard;



        protected string search = "";
        protected decimal finishTicketsRate;
        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            ////this is for just engineers
            tblTicketForDashboard = await db_a79800_ticketService.GetTblTickets(new Query { Filter = $@"i => i.TicketNumber.Contains(@0) || i.TicketHeader.Contains(@0) || i.TicketDescription.Contains(@0) || i.UserEmail.Contains(@0)", FilterParameters = new object[] { search }, Expand = "TblCategory,TblEngineer,TblEmployee" });

            //this is for All Tickets
            listOfTickets = await db_a79800_ticketService.GetTicketsCountsByEng();
            listOfEngineers = await db_a79800_ticketService.GetEngineersForTicketsCountsByEng();    
            GetfinishTicketsRate();

            GetTicketsByEngineer();

        }
        
        protected void GetfinishTicketsRate()
        {
            var allTickets = tblTicketForDashboard?.Where(p => p.TicketId > 0).Count();
            var solvedTickets = tblTicketForDashboard?.Where(p => p.StatusId == 4).Count();
            if (allTickets !=0)
            {
                finishTicketsRate = (Convert.ToDecimal(solvedTickets)) / (Convert.ToDecimal(allTickets));

            }
            else
            {
                finishTicketsRate = 0;
            }

        }
    }
}