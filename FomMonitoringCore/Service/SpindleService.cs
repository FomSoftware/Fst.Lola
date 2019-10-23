using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FomMonitoringCore.Service
{
    public class SpindleService : ISpindleService
    {
        private ISpindleRepository _spindleRepository;

        public SpindleService(ISpindleRepository spindleRepository)
        {
            _spindleRepository = spindleRepository;
        }

        public List<SpindleModel> GetSpindles(MachineInfoModel machine, bool xmodule = false)
        {
            List<SpindleModel> result = new List<SpindleModel>();

            try
            {

                    List<Spindle> query = null;
                    if (machine.Type.Id == (int)enMachineType.LineaTaglioLavoro)
                    {
                        Regex regex = new Regex(@"^[1-2]\d{2}$");
                        if (xmodule)
                        {
                            query = _spindleRepository.Get(w => w.MachineId == machine.Id, tracked: false).ToList().Where(w => regex.IsMatch(w.Code)).ToList();
                        }
                        else
                        {
                            query = _spindleRepository.Get(w => w.MachineId == machine.Id, tracked: false).ToList().Where(w => !regex.IsMatch(w.Code)).ToList();
                        }
                    }                    
                    else
                        query = _spindleRepository.Get(w => w.MachineId == machine.Id, tracked: false).ToList();
                    

                    result = query.Adapt<List<SpindleModel>>();

                    result = result.GroupBy(g => g.Code).Select(s => s.OrderByDescending(o => o.InstallDate).First()).ToList();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machine.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }
    }
}
