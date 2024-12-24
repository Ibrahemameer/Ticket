using Microsoft.AspNetCore.Components;

namespace RhinoTicketingSystem.Services
{
    public class MenuService
    {
        public List<MenuItem> GetMenuItems()
        {
            return new List<MenuItem>
        {
            new MenuItem { Text = "Home", Path = "", RequiresAuthentication = true },
            new MenuItem
            {
                Text = "IT Department",
                Icon = "settings",
                RequiredRoles = new[] { "Admin", "Super Admin", "Engineer", "Manager" },
                Children = new List<MenuItem>
                {
                    new MenuItem { Text = "Tasks", Path = "tasks" },
                    new MenuItem { Text = "Add Task", Path = "add-task" },
                    // Adding other IT Department menu items
                    new MenuItem { Text = "All Tickets", Path = "all-tickets" },
                    new MenuItem { Text = "My Assigned Tickets", Path = "my-assigned-tickets" },
                    new MenuItem { Text = "DocumentHeader", Path = "document-header" },
                    new MenuItem { Text = "DocumentAttachment", Path = "document-attachment" },
                    new MenuItem { Text = "Project", Path = "project" },
                    new MenuItem { Text = "DocumentSerializes", Path = "tbl-document-serializes" },
                    new MenuItem { Text = "Departments", Path = "tbl-departments" },
                }
            },
            new MenuItem
            {
                Text = "Archive Department",
                Icon = "settings",
                RequiredRoles = new[] { "Admin", "Super Admin", "Engineer", "Manager" },
                Children = new List<MenuItem>
                {
                    new MenuItem { Text = "Projects Department", Path = "projects-department" },
                    new MenuItem { Text = "Sigma Consulting", Path = "sigma-consulting" },
                    //new MenuItem { Text = "Cab Stone", Path = "all-tickets" },
                    new MenuItem { Text = "Rhino Constructions", Path = "rhino-constructions" },
                    //new MenuItem { Text = "Free Falcon", Path = "document-header" },
                    //new MenuItem { Text = "DocumentAttachment", Path = "document-attachment" },
                    //new MenuItem { Text = "Project", Path = "project" },
                    //new MenuItem { Text = "DocumentSerializes", Path = "tbl-document-serializes" },
                    //new MenuItem { Text = "Departments", Path = "tbl-departments" },
                }
            },
            // Adding other main menu items
            new MenuItem
            {
                Text = "Settings",
                Icon = "settings",
                RequiredRoles = new[] { "Admin", "Super Admin", "Engineer", "Manager" },
                Children = new List<MenuItem>
                {
                    new MenuItem { Text = "Categories", Path = "tbl-categories" },
                    new MenuItem { Text = "Employees", Path = "tbl-employees" },
                    // Adding other IT Department menu items
                    new MenuItem { Text = "Engineers", Path = "tbl-engineers" },
                    new MenuItem { Text = "OneDrive Management", Path = "manage-archive" },
                    new MenuItem { Text = "Email Management", Path = "email-management" },
                    
                }
            },
            new MenuItem
            {
                Text = "Reports",
                Icon = "settings",
                RequiredRoles = new[] { "Admin", "Super Admin", "Engineer", "Manager" },
                Children = new List<MenuItem>
                {
                    new MenuItem { Text = "Dashboard", Path = "dashboard" },
                    new MenuItem { Text = "Opened Tickets", Path = "active-tickets" },
                    

                }
            },
        };
        }
    }

    public class MenuItem
    {
        public string Text { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public bool RequiresAuthentication { get; set; }
        public string[] RequiredRoles { get; set; }
        public List<MenuItem> Children { get; set; }
    }

}
