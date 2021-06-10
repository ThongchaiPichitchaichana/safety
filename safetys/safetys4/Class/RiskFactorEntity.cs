using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace safetys4.Class
{
    public class RiskFactorEntity
    {
        public int risk_factor_relate_work_id { get; set; }
        public string Other { get; set; }
        public string Year { get; set; }
        public string Result { get; set; }
        public int Duration_risk_factor_id { get; set; }

        public string File_risk_factor { get; set; }
        public string monitoring_results { get; set; }

        public string monitoring_environment { get; set; }

        public string Status { get; set; }
        public int id { get; set; }
    }
}