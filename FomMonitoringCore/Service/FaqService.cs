using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service
{
    class FaqService : IFaqService
    {
        private readonly IFomMonitoringEntities _context;

        public FaqService(IFomMonitoringEntities context)
        {
            _context = context;
        }
        public List<Faq> GetFaqs()
        {
           return _context.Set<Faq>().Where(f => f.IsVisible == true && f.IsInternalMenu == false).OrderBy(f => f.Order).ToList();

        }

        public List<Faq> GetInternalFaqs()
        {
            return _context.Set<Faq>().Where(f => f.IsVisible == true && f.IsInternalMenu == true).OrderBy(f => f.Order).ToList();

        }
    }
}
