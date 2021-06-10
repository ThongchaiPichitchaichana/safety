using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace safetys4.Class
{
    public class OccupationalHealthEntity
    {
        public int occupational_health_report_id { get; set; }
        public string RepeatHealthCheck { get; set; }
        public string FileHealthCheck { get; set; }
        public string FlieRepeatHealthCheck { get; set; }
        public string abnormal_audiogram { get; set; }
        public string hearing_threshold_level { get; set; }
        public string chronic_diseases_ear { get; set; }
        public string specify_chronic_diseases_ear { get; set; }
        public string abnormal_pulmonary_function { get; set; }
        public string smoked_cigarettes { get; set; }
        public int cigarette_per_day { get; set; }
        public int smoked_years { get; set; }
        public int smoked_months { get; set; }
        public string smoked_cigarettes_other { get; set; }
        public string Status { get; set; }
        public int id { get; set; }

    }
}