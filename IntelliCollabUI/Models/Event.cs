using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelliCollabUI.Models
{
    public class EventType
    {
        public string EVENT_ID  {get;set;}
        public string EVENT_NAME { get; set; }
    }

    public class Event
    {
        public string EVENT_ID { get; set; }
        public string EVENT_NAME { get; set; }
        public string EVENT_DESC { get; set; }
        public string EVENT_TYPE { get; set; }
        public string EVENT_TYPE_NAME { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public string STR_START_DATE { get; set; }
        public string STR_END_DATE { get; set; }
        public int NO_DAYS { get; set; }

    }
}