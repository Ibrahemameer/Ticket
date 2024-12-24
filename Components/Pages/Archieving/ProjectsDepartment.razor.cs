using Microsoft.AspNetCore.Components;
using Radzen;
using RhinoTicketingSystem.Models.db_a79800_ticket;
namespace RhinoTicketingSystem.Components.Pages.Archieving
{
    public partial class ProjectsDepartment
    {
        private IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblProject> projects;
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await LoadProjects();
        }

        private async System.Threading.Tasks.Task LoadProjects()
        {
            var query = new Query
            {
                Filter = "i => i.ProjectBranch == @0",
                FilterParameters = new object[] { "Projects Department" }
            };

            projects = await db_a79800_ticketService.GetTblProjects(query);
        }

        //private async System.Threading.Tasks.Task OpenDocuments(TblProject project)
        //{
        //    var parameters = new Dictionary<string, object>
        //    {
        //        { "ProjectId", project.Id }
        //    };
        //    NavigationManager.NavigateTo($"document-header?projectId={project.Id}");
        //}
        private void OpenDocuments(TblProject project)
        {
            NavigationManager.NavigateTo($"document-header/{project.Id}");
        }

    }
}
