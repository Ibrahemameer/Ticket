using System;

namespace RhinoTicketingSystem.Models
{
    public class FileOperationLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string Operation { get; set; }
        public DateTime Timestamp { get; set; }
    }
}