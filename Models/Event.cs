using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaviUcakInterviewTask.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime Time { get; set; }
        public bool IsPaid { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string EventType { get; set; }
    }
}