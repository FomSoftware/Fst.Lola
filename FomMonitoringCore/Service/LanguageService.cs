using System.Globalization;
using FomMonitoringCore.SqlServer.Repository;

namespace FomMonitoringCore.Service
{
    public class LanguageService : ILanguageService
    {
        private readonly IMessageLanguagesRepository _languagesRepository;

        public LanguageService(IMessageLanguagesRepository languagesRepository)
        {
            _languagesRepository = languagesRepository;
        }
        public int? GetCurrentLanguage()
        {
            var la = _languagesRepository.GetFirstOrDefault(lan => lan.DotNetCulture.StartsWith(CultureInfo.CurrentCulture.Name));

            return la?.Id;
        }
    }
}