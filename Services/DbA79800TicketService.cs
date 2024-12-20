using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;
using RhinoTicketingSystem.Controllers;

using RhinoTicketingSystem.Data;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using System.Collections;
using static RhinoTicketingSystem.Components.Pages.TestForFileUploadAndList;
using DocumentFormat.OpenXml.InkML;
using RhinoTicketingSystem.Models.DbA79800Ticket;

namespace RhinoTicketingSystem
{
    public partial class db_a79800_ticketService
    {
        db_a79800_ticketContext Context
        {
            get
            {
                return this.context;
            }
        }

        private readonly db_a79800_ticketContext context;
        private readonly NavigationManager navigationManager;
        private UploadController upload { get; set; }

        public db_a79800_ticketService(db_a79800_ticketContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }
        public async Task<OneDriveArchivingHeader> GetArchiveHeaderByFolderName(string folderName)
        {
            return await context.OneDriveArchivingHeaders
                .FirstOrDefaultAsync(h => h.FolderName == folderName);
        }
        public async Task<OneDriveArchivingHeader> CreateArchiveHeader(OneDriveArchivingHeader header)
        {
            try
            {
                header.CreatedDate = DateTime.Now;
                context.OneDriveArchivingHeaders.Add(header);
                await context.SaveChangesAsync();
                return header;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OneDriveArchivingDetail> CreateArchiveDetail(OneDriveArchivingDetail detail)
        {
            try
            {
                detail.CreatedDate = DateTime.Now;
                context.OneDriveArchivingDetails.Add(detail);
                await context.SaveChangesAsync();
                return detail;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteArchiveDetail(string fileName)
        {
            try
            {
                var detail = await context.OneDriveArchivingDetails
                    .FirstOrDefaultAsync(d => d.AttachedFileName == fileName);

                if (detail != null)
                {
                    context.OneDriveArchivingDetails.Remove(detail);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task ExportTasksToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tasks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tasks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTasksToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tasks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tasks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTasksRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.Task> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.Task>> GetTasks(Query query = null)
        {
            var items = Context.Tasks.AsQueryable();

            items = items.Include(i => i.TblEngineer);
            items = items.Include(i => i.TaskStatus);
            items = items.Include(i => i.TaskType);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTasksRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTaskGet(RhinoTicketingSystem.Models.db_a79800_ticket.Task item);
        partial void OnGetTaskById(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.Task> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.Task> GetTaskById(int id)
        {
            var items = Context.Tasks
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.TblEngineer);
            items = items.Include(i => i.TaskStatus);
            items = items.Include(i => i.TaskType);

            OnGetTaskById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTaskGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTaskCreated(RhinoTicketingSystem.Models.db_a79800_ticket.Task item);
        partial void OnAfterTaskCreated(RhinoTicketingSystem.Models.db_a79800_ticket.Task item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.Task> CreateTask(RhinoTicketingSystem.Models.db_a79800_ticket.Task task)
        {
            OnTaskCreated(task);

            var existingItem = Context.Tasks
                              .Where(i => i.Id == task.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.Tasks.Add(task);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(task).State = EntityState.Detached;
                throw;
            }

            OnAfterTaskCreated(task);

            return task;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.Task> CancelTaskChanges(RhinoTicketingSystem.Models.db_a79800_ticket.Task item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTaskUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.Task item);
        partial void OnAfterTaskUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.Task item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.Task> UpdateTask(int id, RhinoTicketingSystem.Models.db_a79800_ticket.Task task)
        {
            OnTaskUpdated(task);

            var itemToUpdate = Context.Tasks
                              .Where(i => i.Id == task.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(task);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTaskUpdated(task);

            return task;
        }

        partial void OnTaskDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.Task item);
        partial void OnAfterTaskDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.Task item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.Task> DeleteTask(int id)
        {
            var itemToDelete = Context.Tasks
                              .Where(i => i.Id == id)
                              .Include(i => i.TblTaskDetails)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTaskDeleted(itemToDelete);


            Context.Tasks.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTaskDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTaskStatusesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/taskstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/taskstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTaskStatusesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/taskstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/taskstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTaskStatusesRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus>> GetTaskStatuses(Query query = null)
        {
            var items = Context.TaskStatuses.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTaskStatusesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTaskStatusGet(RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus item);
        partial void OnGetTaskStatusById(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> GetTaskStatusById(int id)
        {
            var items = Context.TaskStatuses
                              .AsNoTracking()
                              .Where(i => i.Id == id);


            OnGetTaskStatusById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTaskStatusGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTaskStatusCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus item);
        partial void OnAfterTaskStatusCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> CreateTaskStatus(RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus taskstatus)
        {
            OnTaskStatusCreated(taskstatus);

            var existingItem = Context.TaskStatuses
                              .Where(i => i.Id == taskstatus.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TaskStatuses.Add(taskstatus);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(taskstatus).State = EntityState.Detached;
                throw;
            }

            OnAfterTaskStatusCreated(taskstatus);

            return taskstatus;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> CancelTaskStatusChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTaskStatusUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus item);
        partial void OnAfterTaskStatusUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> UpdateTaskStatus(int id, RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus taskstatus)
        {
            OnTaskStatusUpdated(taskstatus);

            var itemToUpdate = Context.TaskStatuses
                              .Where(i => i.Id == taskstatus.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(taskstatus);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTaskStatusUpdated(taskstatus);

            return taskstatus;
        }

        partial void OnTaskStatusDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus item);
        partial void OnAfterTaskStatusDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> DeleteTaskStatus(int id)
        {
            var itemToDelete = Context.TaskStatuses
                              .Where(i => i.Id == id)
                              .Include(i => i.Tasks)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTaskStatusDeleted(itemToDelete);


            Context.TaskStatuses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTaskStatusDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTaskTypesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tasktypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tasktypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTaskTypesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tasktypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tasktypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTaskTypesRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType>> GetTaskTypes(Query query = null)
        {
            var items = Context.TaskTypes.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTaskTypesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTaskTypeGet(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType item);
        partial void OnGetTaskTypeById(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> GetTaskTypeById(int id)
        {
            var items = Context.TaskTypes
                              .AsNoTracking()
                              .Where(i => i.Id == id);


            OnGetTaskTypeById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTaskTypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTaskTypeCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType item);
        partial void OnAfterTaskTypeCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> CreateTaskType(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType tasktype)
        {
            OnTaskTypeCreated(tasktype);

            var existingItem = Context.TaskTypes
                              .Where(i => i.Id == tasktype.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TaskTypes.Add(tasktype);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tasktype).State = EntityState.Detached;
                throw;
            }

            OnAfterTaskTypeCreated(tasktype);

            return tasktype;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> CancelTaskTypeChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTaskTypeUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType item);
        partial void OnAfterTaskTypeUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> UpdateTaskType(int id, RhinoTicketingSystem.Models.db_a79800_ticket.TaskType tasktype)
        {
            OnTaskTypeUpdated(tasktype);

            var itemToUpdate = Context.TaskTypes
                              .Where(i => i.Id == tasktype.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tasktype);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTaskTypeUpdated(tasktype);

            return tasktype;
        }

        partial void OnTaskTypeDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType item);
        partial void OnAfterTaskTypeDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TaskType item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> DeleteTaskType(int id)
        {
            var itemToDelete = Context.TaskTypes
                              .Where(i => i.Id == id)
                              .Include(i => i.Tasks)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTaskTypeDeleted(itemToDelete);


            Context.TaskTypes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTaskTypeDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTblCategoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblcategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblcategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTblCategoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblcategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblcategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTblCategoriesRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory>> GetTblCategories(Query query = null)
        {
            var items = Context.TblCategories.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblCategoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTblCategoryGet(RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory item);
        partial void OnGetTblCategoryByCategoryId(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> GetTblCategoryByCategoryId(int categoryid)
        {
            var items = Context.TblCategories
                              .AsNoTracking()
                              .Where(i => i.CategoryId == categoryid);


            OnGetTblCategoryByCategoryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTblCategoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTblCategoryCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory item);
        partial void OnAfterTblCategoryCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> CreateTblCategory(RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory tblcategory)
        {
            OnTblCategoryCreated(tblcategory);

            var existingItem = Context.TblCategories
                              .Where(i => i.CategoryId == tblcategory.CategoryId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TblCategories.Add(tblcategory);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tblcategory).State = EntityState.Detached;
                throw;
            }

            OnAfterTblCategoryCreated(tblcategory);

            return tblcategory;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> CancelTblCategoryChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTblCategoryUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory item);
        partial void OnAfterTblCategoryUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> UpdateTblCategory(int categoryid, RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory tblcategory)
        {
            OnTblCategoryUpdated(tblcategory);

            var itemToUpdate = Context.TblCategories
                              .Where(i => i.CategoryId == tblcategory.CategoryId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tblcategory);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTblCategoryUpdated(tblcategory);

            return tblcategory;
        }

        partial void OnTblCategoryDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory item);
        partial void OnAfterTblCategoryDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> DeleteTblCategory(int categoryid)
        {
            var itemToDelete = Context.TblCategories
                              .Where(i => i.CategoryId == categoryid)
                              .Include(i => i.TblTickets)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTblCategoryDeleted(itemToDelete);


            Context.TblCategories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTblCategoryDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTblChartOfAccountsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblchartofaccounts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblchartofaccounts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTblChartOfAccountsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblchartofaccounts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblchartofaccounts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTblChartOfAccountsRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount>> GetTblChartOfAccounts(Query query = null)
        {
            var items = Context.TblChartOfAccounts.AsQueryable();

            items = items.Include(i => i.TblChartOfAccount1);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblChartOfAccountsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTblChartOfAccountGet(RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount item);
        partial void OnGetTblChartOfAccountByChartId(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount> GetTblChartOfAccountByChartId(int chartid)
        {
            var items = Context.TblChartOfAccounts
                              .AsNoTracking()
                              .Where(i => i.ChartId == chartid);

            items = items.Include(i => i.TblChartOfAccount1);

            OnGetTblChartOfAccountByChartId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTblChartOfAccountGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTblChartOfAccountCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount item);
        partial void OnAfterTblChartOfAccountCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount> CreateTblChartOfAccount(RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount tblchartofaccount)
        {
            OnTblChartOfAccountCreated(tblchartofaccount);

            var existingItem = Context.TblChartOfAccounts
                              .Where(i => i.ChartId == tblchartofaccount.ChartId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TblChartOfAccounts.Add(tblchartofaccount);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tblchartofaccount).State = EntityState.Detached;
                throw;
            }

            OnAfterTblChartOfAccountCreated(tblchartofaccount);

            return tblchartofaccount;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount> CancelTblChartOfAccountChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTblChartOfAccountUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount item);
        partial void OnAfterTblChartOfAccountUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount> UpdateTblChartOfAccount(int chartid, RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount tblchartofaccount)
        {
            OnTblChartOfAccountUpdated(tblchartofaccount);

            var itemToUpdate = Context.TblChartOfAccounts
                              .Where(i => i.ChartId == tblchartofaccount.ChartId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tblchartofaccount);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTblChartOfAccountUpdated(tblchartofaccount);

            return tblchartofaccount;
        }

        partial void OnTblChartOfAccountDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount item);
        partial void OnAfterTblChartOfAccountDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount> DeleteTblChartOfAccount(int chartid)
        {
            var itemToDelete = Context.TblChartOfAccounts
                              .Where(i => i.ChartId == chartid)
                              .Include(i => i.TblChartOfAccounts1)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTblChartOfAccountDeleted(itemToDelete);


            Context.TblChartOfAccounts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTblChartOfAccountDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTblEmployeesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblemployees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblemployees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTblEmployeesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblemployees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblemployees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTblEmployeesRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee>> GetTblEmployees(Query query = null)
        {
            var items = Context.TblEmployees.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblEmployeesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTblEmployeeGet(RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee item);
        partial void OnGetTblEmployeeById(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> GetTblEmployeeById(int id)
        {
            var items = Context.TblEmployees
                              .AsNoTracking()
                              .Where(i => i.Id == id);


            OnGetTblEmployeeById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTblEmployeeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTblEmployeeCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee item);
        partial void OnAfterTblEmployeeCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> CreateTblEmployee(RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee tblemployee)
        {
            OnTblEmployeeCreated(tblemployee);

            var existingItem = Context.TblEmployees
                              .Where(i => i.Id == tblemployee.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TblEmployees.Add(tblemployee);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tblemployee).State = EntityState.Detached;
                throw;
            }

            OnAfterTblEmployeeCreated(tblemployee);

            return tblemployee;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> CancelTblEmployeeChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTblEmployeeUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee item);
        partial void OnAfterTblEmployeeUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> UpdateTblEmployee(int id, RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee tblemployee)
        {
            OnTblEmployeeUpdated(tblemployee);

            var itemToUpdate = Context.TblEmployees
                              .Where(i => i.Id == tblemployee.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tblemployee);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTblEmployeeUpdated(tblemployee);

            return tblemployee;
        }

        partial void OnTblEmployeeDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee item);
        partial void OnAfterTblEmployeeDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> DeleteTblEmployee(int id)
        {
            var itemToDelete = Context.TblEmployees
                              .Where(i => i.Id == id)
                              .Include(i => i.TblTickets)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTblEmployeeDeleted(itemToDelete);


            Context.TblEmployees.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTblEmployeeDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTblEngineersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblengineers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblengineers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTblEngineersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblengineers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblengineers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTblEngineersRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer>> GetTblEngineers(Query query = null)
        {
            var items = Context.TblEngineers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblEngineersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTblEngineerGet(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer item);
        partial void OnGetTblEngineerById(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> GetTblEngineerById(int id)
        {
            var items = Context.TblEngineers
                              .AsNoTracking()
                              .Where(i => i.Id == id);


            OnGetTblEngineerById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTblEngineerGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTblEngineerCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer item);
        partial void OnAfterTblEngineerCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> CreateTblEngineer(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer tblengineer)
        {
            OnTblEngineerCreated(tblengineer);

            var existingItem = Context.TblEngineers
                              .Where(i => i.Id == tblengineer.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TblEngineers.Add(tblengineer);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tblengineer).State = EntityState.Detached;
                throw;
            }

            OnAfterTblEngineerCreated(tblengineer);

            return tblengineer;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> CancelTblEngineerChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTblEngineerUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer item);
        partial void OnAfterTblEngineerUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> UpdateTblEngineer(int id, RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer tblengineer)
        {
            OnTblEngineerUpdated(tblengineer);

            var itemToUpdate = Context.TblEngineers
                              .Where(i => i.Id == tblengineer.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tblengineer);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTblEngineerUpdated(tblengineer);

            return tblengineer;
        }

        partial void OnTblEngineerDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer item);
        partial void OnAfterTblEngineerDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> DeleteTblEngineer(int id)
        {
            var itemToDelete = Context.TblEngineers
                              .Where(i => i.Id == id)
                              .Include(i => i.Tasks)
                              .Include(i => i.TblReassignTickets)
                              .Include(i => i.TblTickets)
                              .Include(i => i.TblTaskDetails)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTblEngineerDeleted(itemToDelete);


            Context.TblEngineers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTblEngineerDeleted(itemToDelete);

            return itemToDelete;
        }
        [Inject]
        protected NotificationService NotificationService { get; set; }
        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> DeleteEngineerAfterCheckForHisTickets(int id)
        {
            var itemToDelete = Context.TblEngineers
                              .Where(i => i.Id == id)
                              .Include(i => i.Tasks)
                              .Include(i => i.TblReassignTickets)
                              .Include(i => i.TblTickets)
                              .Include(i => i.TblTaskDetails)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }
            else
            {
                //first we check if this engineer has any tickets
                var deletedEngineerTickets = itemToDelete.TblReassignTickets.FirstOrDefault();
                if (deletedEngineerTickets != null)
                {
                    throw new ApplicationException("This Engineer has related Tickets");
                    //NotificationService.Notify(new NotificationMessage
                    //{
                    //    Severity = NotificationSeverity.Error,
                    //    Summary = $"Error",
                    //    Detail = $"This Engineer has related Tickets : Unable to delete this Engineer"
                    //});
                }
                OnTblEngineerDeleted(itemToDelete);


                Context.TblEngineers.Remove(itemToDelete);

                try
                {
                    Context.SaveChanges();
                }
                catch
                {
                    Context.Entry(itemToDelete).State = EntityState.Unchanged;
                    throw;
                }

                OnAfterTblEngineerDeleted(itemToDelete);
            }
            

            return itemToDelete;
        }
        void t()
        {
            
        }
        public async Task ExportTblReassignTicketsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblreassigntickets/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblreassigntickets/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTblReassignTicketsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblreassigntickets/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblreassigntickets/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTblReassignTicketsRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket>> GetTblReassignTickets(Query query = null)
        {
            var items = Context.TblReassignTickets.AsQueryable();

            items = items.Include(i => i.TblEngineer);
            items = items.Include(i => i.TblStatus);
            items = items.Include(i => i.TblTicket);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblReassignTicketsRead(ref items);

            return await Task.FromResult(items);
        }
        partial void OnTblTicketAttachmentsRead(ref IQueryable<RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment> items);

        public async Task<IList<RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment>> GetTblTicketAttachments(int ticketId)
        {
            var items = Context.TblTicketattachments.Where(p=>p.TicketId== ticketId).ToList();

            return await Task.FromResult(items);
        }
        public async Task<bool> DeleteAttachmentAsync(int attachmentId)
        {
            var attachment = await context.TblTicketattachments.FindAsync(attachmentId);
            if (attachment != null)
            {
                context.TblTicketattachments.Remove(attachment);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> GetTblReassignTicketByEngineerId(int engineerId)
        {
            var items = Context.TblReassignTickets
                              .AsNoTracking()
                              .Where(i => i.EngineerId == engineerId);

            items = items.Include(i => i.TblEngineer);
            items = items.Include(i => i.TblStatus);
            items = items.Include(i => i.TblTicket);

            OnGetTblReassignTicketById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTblReassignTicketGet(itemToReturn);

            return await Task.FromResult(items.FirstOrDefault());
        }
        partial void OnTblReassignTicketGet(RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket item);
        partial void OnGetTblReassignTicketById(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> GetTblReassignTicketById(int id)
        {
            var items = Context.TblReassignTickets
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.TblEngineer);
            items = items.Include(i => i.TblStatus);
            items = items.Include(i => i.TblTicket);

            OnGetTblReassignTicketById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTblReassignTicketGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTblReassignTicketCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket item);
        partial void OnAfterTblReassignTicketCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> CreateTblReassignTicket(RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket tblreassignticket)
        {
            OnTblReassignTicketCreated(tblreassignticket);

            var existingItem = Context.TblReassignTickets
                              .Where(i => i.Id == tblreassignticket.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TblReassignTickets.Add(tblreassignticket);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tblreassignticket).State = EntityState.Detached;
                throw;
            }

            OnAfterTblReassignTicketCreated(tblreassignticket);

            return tblreassignticket;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> CancelTblReassignTicketChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTblReassignTicketUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket item);
        partial void OnAfterTblReassignTicketUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> UpdateTblReassignTicket(int id, RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket tblreassignticket)
        {
            OnTblReassignTicketUpdated(tblreassignticket);

            var itemToUpdate = Context.TblReassignTickets
                              .Where(i => i.Id == tblreassignticket.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                //throw new Exception("Item no longer available");

                await CreateTblReassignTicket(tblreassignticket);
            }
            else
            {
                var entryToUpdate = Context.Entry(itemToUpdate);
                entryToUpdate.CurrentValues.SetValues(tblreassignticket);
                entryToUpdate.State = EntityState.Modified;

                Context.SaveChanges();

                OnAfterTblReassignTicketUpdated(tblreassignticket);
            }


            return tblreassignticket;
        }

        partial void OnTblReassignTicketDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket item);
        partial void OnAfterTblReassignTicketDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> DeleteTblReassignTicket(int id)
        {
            var itemToDelete = Context.TblReassignTickets
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTblReassignTicketDeleted(itemToDelete);


            Context.TblReassignTickets.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTblReassignTicketDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTblStatusesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTblStatusesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tblstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tblstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTblStatusesRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus>> GetTblStatuses(Query query = null)
        {
            var items = Context.TblStatuses.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblStatusesRead(ref items);

            return await Task.FromResult(items);
        }
        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus>> GetTblStatusesForProceeding(Query query = null)
        {
            var items = Context.TblStatuses.Where(s=>s.StatusId!=1).AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblStatusesRead(ref items);

            return await Task.FromResult(items);
        }
        partial void OnTblStatusGet(RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus item);
        partial void OnGetTblStatusByStatusId(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> GetTblStatusByStatusId(int statusid)
        {
            var items = Context.TblStatuses
                              .AsNoTracking()
                              .Where(i => i.StatusId == statusid);


            OnGetTblStatusByStatusId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTblStatusGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTblStatusCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus item);
        partial void OnAfterTblStatusCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> CreateTblStatus(RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus tblstatus)
        {
            OnTblStatusCreated(tblstatus);

            var existingItem = Context.TblStatuses
                              .Where(i => i.StatusId == tblstatus.StatusId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TblStatuses.Add(tblstatus);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tblstatus).State = EntityState.Detached;
                throw;
            }

            OnAfterTblStatusCreated(tblstatus);

            return tblstatus;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> CancelTblStatusChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTblStatusUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus item);
        partial void OnAfterTblStatusUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> UpdateTblStatus(int statusid, RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus tblstatus)
        {
            OnTblStatusUpdated(tblstatus);

            var itemToUpdate = Context.TblStatuses
                              .Where(i => i.StatusId == tblstatus.StatusId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tblstatus);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTblStatusUpdated(tblstatus);

            return tblstatus;
        }

        partial void OnTblStatusDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus item);
        partial void OnAfterTblStatusDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> DeleteTblStatus(int statusid)
        {
            var itemToDelete = Context.TblStatuses
                              .Where(i => i.StatusId == statusid)
                              .Include(i => i.TblReassignTickets)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTblStatusDeleted(itemToDelete);


            Context.TblStatuses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTblStatusDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTblTicketsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tbltickets/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tbltickets/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTblTicketsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tbltickets/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tbltickets/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTblTicketsRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket>> GetTblTickets(Query query = null)
        {
            var items = Context.TblTickets.AsQueryable();

            items = items.Include(i => i.TblCategory);
            items = items.Include(i => i.TblEmployee);
            items = items.Include(i => i.TblEngineer);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblTicketsRead(ref items);

            return await Task.FromResult(items);
        }
        //this is for get user related tickets
        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket>> GetTblTicketsForUser(string userEmail,Query query = null)
        {
            var items = Context.TblTickets
                .Where(t=>t.UserEmail==userEmail).AsQueryable();

            items = items.Include(i => i.TblCategory);
            items = items.Include(i => i.TblEmployee);
            items = items.Include(i => i.TblEngineer);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblTicketsRead(ref items);

            return await Task.FromResult(items);
        }

        //this is for get user related tickets
        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket>> GetTblTicketsForEngineer(string engEmail, Query query = null)
        {
            var items = Context.TblTickets
                .Where(t=>t.EngEmail==engEmail).AsQueryable();

            items = items.Include(i => i.TblCategory);
            items = items.Include(i => i.TblEmployee);
            items = items.Include(i => i.TblEngineer);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblTicketsRead(ref items);

            return await Task.FromResult(items);
        }


        //this is for getting just one record
        //partial void OnTblTicketGet(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);
        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket>> GetTblTicketsCountByEngineerForDashboard( Query query = null)
        {
            var items = Context.TblTickets.AsQueryable().Where(p => p.EngineerId != null);

            items = items.Include(i => i.TblCategory);
            items = items.Include(i => i.TblEmployee);
            items = items.Include(i => i.TblEngineer);

            

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblTicketsRead(ref items);

            return await Task.FromResult(items);
        }
        public async Task<IList<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket>> GetTicketsCountsByEng()
        {
            var items = context.TblTickets.Where(t => t.EngineerId != null).Include(t=>t.TblEngineer).ToList();
            return items;
        }
        public async Task<IList<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer>> GetEngineersForTicketsCountsByEng()
        {
            var items = context.TblEngineers.ToList();
            return items;
        }
        
        partial void OnTblTicketGet(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);
        partial void OnGetTblTicketByTicketId(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> GetTblTicketByTicketId(int ticketid)
        {
            var items = Context.TblTickets
                              .AsNoTracking()
                              .Where(i => i.TicketId == ticketid);

            items = items.Include(i => i.TblCategory);
            items = items.Include(i => i.TblEmployee);
            items = items.Include(i => i.TblEngineer);

            OnGetTblTicketByTicketId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTblTicketGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }
        
        public  void  CreateTicketAttachment(List<FileAttchment> fileAttachments)
        {
            List<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> listOfTickets = new List<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket>();
            foreach (var item in fileAttachments)
            {
                RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket ticket = new Models.db_a79800_ticket.TblTicket();
                ticket.AttchedFileName = item.Filename;
                ticket.Attachment = item.FilePath;
                ticket.TicketHeader=item.Filename;
                ticket.Date = DateTime.Now;
               
                listOfTickets.Add(ticket);
            }
            
            context.TblTickets.AddRange(listOfTickets);
            context.SaveChanges();
             
        }
        public void CreateAttachmentForTicketByTicketId(List<FileAttchment> fileAttachments)
        {
            List<RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment> listOfAttachments = new List<RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment>();
            foreach (var item in fileAttachments)
            {
                RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment newAttachment = new Models.DbA79800Ticket.TblTicketattachment();
                newAttachment.AttachedFileName = item.Filename;
                newAttachment.AttachedFilePath = item.FilePath;
                newAttachment.attachedFileSize = item.FileSize;
                newAttachment.FileType=item.FileType;
                newAttachment.CreatedDate = DateTime.Now;
                newAttachment.TicketId = item.TicketId;
                listOfAttachments.Add(newAttachment);
            }

            context.TblTicketattachments.AddRange(listOfAttachments);
            context.SaveChanges();

        }
        partial void OnTblTicketCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);
        partial void OnAfterTblTicketCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> CreateTblTicket(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblticket)
        {
            OnTblTicketCreated(tblticket);

            var existingItem = Context.TblTickets
                              .Where(i => i.TicketId == tblticket.TicketId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TblTickets.Add(tblticket);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tblticket).State = EntityState.Detached;
                throw;
            }

            OnAfterTblTicketCreated(tblticket);

            return tblticket;
        }
        partial void OnTblTicketUpdated_WithUserEmail(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);
        partial void OnAfterTblTicketUpdated_WithUserEmail(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);
        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> CreateTicket_WithUpdatingUserEmail(string userEmail, int ticketid,
            RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblticket)
        {
            OnTblTicketUpdated_WithUserEmail(tblticket);

            var itemToUpdate = Context.TblTickets
                              .Where(i => i.TicketId == tblticket.TicketId)
                              .FirstOrDefault();

            var ticketStatus = context.TblStatuses.FirstOrDefault(p => p.StatusId == 1);
            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            itemToUpdate.UserEmail = userEmail;
            itemToUpdate.Date = DateTime.Now;
            itemToUpdate.StatusId = ticketStatus.StatusId;
            itemToUpdate.TicketStatus = ticketStatus.Description;
            Context.SaveChanges();

            OnAfterTblTicketUpdated_WithUserEmail(tblticket);


            #region this part is to create ticket details on table reassignticket with ticketId

            var itemDeatilsTocreate = Context.TblReassignTickets
                              .Where(i => i.TicketId == itemToUpdate.TicketId)
                              .FirstOrDefault();

            if (itemDeatilsTocreate != null)
            {
                itemDeatilsTocreate.ReassignedDate = DateTime.Now;
                itemDeatilsTocreate.ReassignedBy = userEmail;
                itemDeatilsTocreate.UserEmail = userEmail;
                Context.SaveChanges();
            }
            #endregion
            return tblticket;
        }
        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> CreateTicket_WithUpdatingUserEmailandAttachment(string attachment
            , string userEmail, int ticketid,
            RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblticket)
        {
            OnTblTicketUpdated_WithUserEmail(tblticket);

            var itemToUpdate = Context.TblTickets
                              .Where(i => i.TicketId == tblticket.TicketId)
                              .FirstOrDefault();


            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            itemToUpdate.UserEmail = userEmail;
            itemToUpdate.Date = DateTime.Now;
            itemToUpdate.Attachment = attachment;
            Context.SaveChanges();

            OnAfterTblTicketUpdated_WithUserEmail(tblticket);


            #region this part is to create ticket details on table reassignticket with ticketId

            var itemDeatilsTocreate = Context.TblReassignTickets
                              .Where(i => i.TicketId == itemToUpdate.TicketId)
                              .FirstOrDefault();

            if (itemDeatilsTocreate != null)
            {
                itemDeatilsTocreate.ReassignedDate = DateTime.Now;
                itemDeatilsTocreate.ReassignedBy = userEmail;
                itemDeatilsTocreate.UserEmail = userEmail;
                Context.SaveChanges();
            }
            #endregion
            return tblticket;
        }
        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> CancelTblTicketChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTblTicketUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);
        partial void OnAfterTblTicketUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> UpdateTblTicket(int ticketid, 
            RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblticket)
        {
            OnTblTicketUpdated(tblticket);

            var itemToUpdate = Context.TblTickets
                              .Where(i => i.TicketId == tblticket.TicketId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tblticket);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTblTicketUpdated(tblticket);

            return tblticket;
        }
        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> UpdateTblTicketWIthEngineerComment(string userEmail, int ticketid,
            RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblticket)
        {
            OnTblTicketUpdated(tblticket);

            var itemToUpdate = Context.TblTickets
                              .Where(i => i.TicketId == tblticket.TicketId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tblticket);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            //here the part of creating ticket Details

            var ticketStatus = context.TblStatuses
                .FirstOrDefault(p => p.StatusId == itemToUpdate.StatusId);

            var engineerDetails = context.TblEngineers
                .Where(e => e.Id == itemToUpdate.EngineerId)
                .FirstOrDefault();

            ticketDetails = new RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket();
            //and here we update ticketDetails (reassign the ticket to an engineer )
            // and we get this point by creating a record inside tbl_Reassignticket
            ticketDetails.TicketId = itemToUpdate.TicketId;
            ticketDetails.EngineerId = itemToUpdate.EngineerId;
            ticketDetails.ReassignedBy = userEmail;
            ticketDetails.ReassignedDate = DateTime.Now;
            ticketDetails.StatusId = itemToUpdate.StatusId;
            ticketDetails.UserEmail = itemToUpdate.UserEmail;
            ticketDetails.ReassignedTo = engineerDetails.EngEmail;
            ticketDetails.ProblemDescription = itemToUpdate.EngineerComment;
            //context.TblReassignTickets.Add(ticketDetails);
            await CreateTblReassignTicket(ticketDetails);

            //and finally update ticket ststus to reAssigned
            itemToUpdate.TicketStatus = ticketStatus.Description;
            
            itemToUpdate.EngEmail = engineerDetails.EngEmail;
            context.SaveChanges();

            OnAfterTblTicketUpdated(tblticket);

            return tblticket;
        }
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket ticketDetails;
        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> UpdateTblTicketWithUpdatingDetails(string userEmail
            , int ticketid,
            RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblticket)
        {
            OnTblTicketUpdated(tblticket);

            var itemToUpdate = Context.TblTickets
                              .Where(i => i.TicketId == tblticket.TicketId)
                              .FirstOrDefault();
            var ticketStatus = context.TblStatuses
                .FirstOrDefault(p => p.StatusId == 2);
            
            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }
            else
            {
                //her we update all the table tblTicket
                var entryToUpdate = Context.Entry(itemToUpdate);
                entryToUpdate.CurrentValues.SetValues(tblticket);
                entryToUpdate.State = EntityState.Modified;

                Context.SaveChanges();

                var engineerDetails = context.TblEngineers
                .Where(e => e.Id == itemToUpdate.EngineerId)
                .FirstOrDefault();

                ticketDetails = new RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket();
                //and here we update ticketDetails (reassign the ticket to an engineer )
                // and we get this point by creating a record inside tbl_Reassignticket
                ticketDetails.TicketId = itemToUpdate.TicketId;
                ticketDetails.EngineerId = itemToUpdate.EngineerId;
                ticketDetails.ReassignedBy = userEmail;
                ticketDetails.ReassignedDate = DateTime.Now;
                ticketDetails.StatusId = ticketStatus.StatusId;
                ticketDetails.UserEmail = itemToUpdate.UserEmail;
                ticketDetails.ReassignedTo = engineerDetails.EngEmail;
                //context.TblReassignTickets.Add(ticketDetails);
                await CreateTblReassignTicket(ticketDetails);


                //and finally update ticket ststus to reAssigned
                itemToUpdate.TicketStatus = ticketStatus.Description;
                itemToUpdate.StatusId = ticketStatus.StatusId;
                itemToUpdate.EngEmail=engineerDetails.EngEmail;
                context.SaveChanges();
                OnAfterTblTicketUpdated(tblticket);
            }


            return tblticket;
        }
        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> UpdateTblTicketWithAttachment(string attachment, int ticketid, RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblticket)
        {
            OnTblTicketUpdated(tblticket);

            var itemToUpdate = Context.TblTickets
                              .Where(i => i.TicketId == tblticket.TicketId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }
            itemToUpdate.Attachment = attachment;
            Context.SaveChanges();
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tblticket);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTblTicketUpdated(tblticket);

            return tblticket;
        }
        partial void OnTblTicketDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);
        partial void OnAfterTblTicketDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> DeleteTblTicket(int ticketid)
        {
            var itemToDelete = Context.TblTickets
                              .Where(i => i.TicketId == ticketid)
                              .Include(i => i.TblReassignTickets)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTblTicketDeleted(itemToDelete);


            Context.TblTickets.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTblTicketDeleted(itemToDelete);

            return itemToDelete;
        }

        public async Task ExportTblTaskDetailsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tbltaskdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tbltaskdetails/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTblTaskDetailsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/db_a79800_ticket/tbltaskdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/db_a79800_ticket/tbltaskdetails/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTblTaskDetailsRead(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail> items);

        public async Task<IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail>> GetTblTaskDetails(Query query = null)
        {
            var items = Context.TblTaskDetails.AsQueryable();

            items = items.Include(i => i.TblEngineer);
            items = items.Include(i => i.Task);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTblTaskDetailsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTblTaskDetailGet(RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail item);
        partial void OnGetTblTaskDetailById(ref IQueryable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail> items);


        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail> GetTblTaskDetailById(int id)
        {
            var items = Context.TblTaskDetails
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.TblEngineer);
            items = items.Include(i => i.Task);

            OnGetTblTaskDetailById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTblTaskDetailGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTblTaskDetailCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail item);
        partial void OnAfterTblTaskDetailCreated(RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail> CreateTblTaskDetail(RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail tbltaskdetail)
        {
            OnTblTaskDetailCreated(tbltaskdetail);

            var existingItem = Context.TblTaskDetails
                              .Where(i => i.Id == tbltaskdetail.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.TblTaskDetails.Add(tbltaskdetail);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tbltaskdetail).State = EntityState.Detached;
                throw;
            }

            OnAfterTblTaskDetailCreated(tbltaskdetail);

            return tbltaskdetail;
        }

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail> CancelTblTaskDetailChanges(RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTblTaskDetailUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail item);
        partial void OnAfterTblTaskDetailUpdated(RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail> UpdateTblTaskDetail(int id, RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail tbltaskdetail)
        {
            OnTblTaskDetailUpdated(tbltaskdetail);

            var itemToUpdate = Context.TblTaskDetails
                              .Where(i => i.Id == tbltaskdetail.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tbltaskdetail);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTblTaskDetailUpdated(tbltaskdetail);

            return tbltaskdetail;
        }

        partial void OnTblTaskDetailDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail item);
        partial void OnAfterTblTaskDetailDeleted(RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail item);

        public async Task<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail> DeleteTblTaskDetail(int id)
        {
            var itemToDelete = Context.TblTaskDetails
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnTblTaskDetailDeleted(itemToDelete);


            Context.TblTaskDetails.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTblTaskDetailDeleted(itemToDelete);

            return itemToDelete;
        }
    }
}