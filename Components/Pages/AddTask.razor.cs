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
    public partial class AddTask
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

        private string documentSerial;
        protected override async Task OnInitializedAsync()
        {
            task = new RhinoTicketingSystem.Models.db_a79800_ticket.Task();

            tblEngineersForEngineerId = await db_a79800_ticketService.GetTblEngineers();

            taskTypesForTypeId = await db_a79800_ticketService.GetTaskTypes();

            taskStatusesForStatusId = await db_a79800_ticketService.GetTaskStatuses();
            //documentSerial = await GenerateDocumentSerial("Tasks");
        }
        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.Task task;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> tblEngineersForEngineerId;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> taskTypesForTypeId;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> taskStatusesForStatusId;

        [Inject]
        protected SecurityService Security { get; set; }

        #region Stop not needed
        /// <summary>
        /// Generate the New Serial 
        /// </summary>
        /// <returns></returns>
        //private async Task<string> GenerateDocumentSerial(string documentType)
        //{
        //    try
        //    {
        //        var lastDocument = await db_a79800_ticketService.GetTblDocumentSerializes(new Query
        //        {
        //            Filter = $"DocumentType == @0",
        //            FilterParameters = new object[] { documentType },
        //            OrderBy = "Id desc",
        //            Top = 1
        //        });

        //        var baseDocument = lastDocument.FirstOrDefault();

        //        if (baseDocument != null)
        //        {
        //            string baseCombination = baseDocument.Combination;
        //            var lastSerial = await db_a79800_ticketService.GenerateDocumentSequence(
        //                baseDocument.FirstSerial,
        //                baseDocument.SecondSerial,
        //                baseDocument.ThirdSerial,
        //                baseDocument.FourthSerial
        //            );
        //            return lastSerial;
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        NotificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
        //        return null;
        //    }
        //}
        #endregion

        private async Task FormSubmit()
        {
            try
            {
                // Get the last task with a serial number
                var lastTask = await db_a79800_ticketService.GetTasks(new Query
                {
                    OrderBy = "Id desc",
                    Top = 1
                });

                var lastTaskRecord = lastTask.FirstOrDefault();

                if (lastTaskRecord != null && !string.IsNullOrEmpty(lastTaskRecord.Title))
                {
                    string lastTitle = lastTaskRecord.Title;
                    string numberPart = lastTitle.Substring(lastTitle.Length - 3);
                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        task.Title = $"{lastTitle.Substring(0, lastTitle.Length - 3)}{(lastNumber + 1):D3}";
                    }
                    else
                    {
                        task.Title = $"{lastTitle}001";
                    }
                }
                else
                {
                    task.Title = "TASK001";
                }

                task.CreatedDate = DateTime.Now;
                await db_a79800_ticketService.CreateTask(task);
                DialogService.Close(task);
                NotificationService.Notify(NotificationSeverity.Success, "Success", "Task created successfully");
            }
            catch (Exception ex)
            {
                errorVisible = true;
                NotificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
        }





        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}