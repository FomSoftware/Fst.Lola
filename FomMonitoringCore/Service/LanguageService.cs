using System.Globalization;
using FomMonitoringCore.Repository.SQL;

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