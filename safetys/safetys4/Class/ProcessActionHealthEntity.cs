using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace safetys4.Class
{
    public class ProcessActionHealthEntity
    {
        public string TypeControl { get; set; }
        public string Action { get; set; }
        public string ResponsiblePerson { get; set; }
        public string DueDate { get; set; }
        public string DateComplete { get; set; }
        public string Remark { get; set; }
        public string DoctorOpinionFile { get; set; }
        public string RecoveryPlanFile { get; set; }
        public string AttachmentFile { get; set; }
        public int Action_status_id { get; set; }
        public int id { get; set; }
    }
}