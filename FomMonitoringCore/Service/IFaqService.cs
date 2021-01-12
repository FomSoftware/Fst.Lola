using System.Collections.Generic;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service
{
    public interface IFaqService
    {
        List<Faq> GetFaqs();
        List<Faq> GetInternalFaqs();
    }
}