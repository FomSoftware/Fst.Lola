using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class ProductivityViewModel
    {
        public ProductivityVueModel vm_productivity { get; set; }

        public ChartViewModel opt_historical { get; set; }

        public ChartViewModel opt_operators { get; set; }

        public MachineInfoViewModel vm_machine_info { get; set; }

    }

    public class ProductivityVueModel
    {
        public KPIViewModel kpi { get; set; }
        public PieceViewModel piece { get; set; }
        public MaterialViewModel material { get; set; }
        public List<ProdDataModel> phases { get; set; }
        public List<ProdDataModel> operators { get; set; }
        public TimeViewModel time { get; set; }
    }

    public class PieceViewModel
    {
        public int total { get; set; }
        public ProdDataModel done { get; set; }
        public ProdDataModel redone { get; set; }
    }

    public class ErrorViewModel
    {
        public int? quantity { get; set; }       
    }

    public class MaterialViewModel
    {
        public int total { get; set; }
        public ProdDataModel bar { get; set; }
        public ProdDataModel cutoff { get; set; }
    }

    public class ProdDataModel
    {
        public string text { get; set; }
        public decimal? perc { get; set; }
        public int number { get; set; }
    }
}