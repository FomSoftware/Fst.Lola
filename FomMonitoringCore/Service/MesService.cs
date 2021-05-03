
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service
{
    public class MesService : IMesService
    {
        #region API

        private readonly IFomMonitoringEntities _context;
        
        public MesService(IFomMonitoringEntities context)
        {
            _context = context;
        }


        public int? GetOrSetPlantDefaultByUser(Guid userId)
        {            

                var userM = _context.Set<UserCustomerMapping>().FirstOrDefault(m => m.UserId == userId);
                var plant = _context.Set<Plant>().FirstOrDefault(p => userId == p.UserId);

                if(plant == null)
                {
                    var user = _context.Set<Users>().FirstOrDefault(u => u.ID == userM.UserId);
                    plant = AddPlant(user);
                }
                return plant?.Id;
            
            
        }

        public Plant AddPlant(Users user, string plantName = null, string plantAddress = null)
        {
            Plant result = null;

            if (string.IsNullOrWhiteSpace(plantName))
            {
                plantName = "Default_" + user.Username;
            }

            try
            {                
                var plant = new Plant();
                plant.Name = plantName;
                plant.Address = plantAddress;
                plant.UserId = user?.ID;
                _context.Set<Plant>().Add(plant);
                _context.SaveChanges();
                    
                result = plant;                                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), plantName, plantAddress);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        #endregion

        #region APP WEB

        public List<PlantModel> GetUserPlants(Guid userId)
        {
            var result = new List<PlantModel>();

            try
            {               
                var query = _context.Set<UserMachineMapping>().Where(w => w.UserId == userId && w.Machine.PlantId != null).Select(s => s.Machine.Plant).Distinct().ToList();
                result = query.Adapt<List<PlantModel>>();                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), userId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        

        public List<MesUserMachinesModel> GetPlantData(PlantModel plant)
        {
            var result = new List<MesUserMachinesModel>();

            try
            {

                var date = DateTime.UtcNow.Date;
                var machines = _context.Set<Machine>().Where(n => n.PlantId == plant.Id && (n.ExpirationDate == null || n.ExpirationDate > date)).ToList();

                var dtos = machines.Select(CreateMesMachineDto).ToList();


                //List<usp_MesUserMachines_Result> query = _context.usp_MesUserMachines(plant.Id, DateTime.UtcNow.Date).ToList();
                result = dtos.Adapt<List<MesUserMachinesModel>>().OrderBy(n => n.Id).ToList();                
            }
            catch (Exception ex)
            {
                var errMessage = $"{ex.GetStringLog()} (PlantID = '{plant?.Id ?? 0}')";
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        private MachineMesDataModel CreateMesMachineDto(Machine machine)
        {
            var date = DateTime.UtcNow.Date;
            var currentState = _context.Set<CurrentState>().FirstOrDefault(cs => machine.Id == cs.MachineId && cs.LastUpdated != null && DbFunctions.TruncateTime(cs.LastUpdated) == DbFunctions.TruncateTime(date));
            var historyMessage = _context.Set<HistoryMessage>().Where(cs => machine.Id == cs.MachineId && cs.Day != null && DbFunctions.TruncateTime(cs.Day) == DbFunctions.TruncateTime(date) && cs.MessagesIndex != null && cs.MessagesIndex.MessageTypeId == 11).ToList();
            var historyPiece = _context.Set<HistoryPiece>().Where(cs => machine.Id == cs.MachineId && DbFunctions.TruncateTime(cs.Day) == DbFunctions.TruncateTime(date) && cs.Shift == null && cs.Operator == null).ToList();
            var historyEfficiency = _context.Set<HistoryState>().Where(cs => machine.Id == cs.MachineId && cs.Day != null && DbFunctions.TruncateTime(cs.Day) == DbFunctions.TruncateTime(date) && cs.Shift == null && cs.Operator == null).ToList();
            var stateMachine = _context.Set<StateMachine>().Where(cs => machine.Id == cs.MachineId && cs.Day != null && DbFunctions.TruncateTime(cs.Day) == DbFunctions.TruncateTime(date))
                .OrderByDescending(s => s.StartTime).FirstOrDefault();

            var m = new MachineMesDataModel
            {
                Id = machine.Id,
                MachineId = machine.Id,
                ExpirationDate = machine.ExpirationDate
            };
            if (currentState != null)
            {
                m.ActualJobCode = currentState.JobCode;
                m.ActualJobPerc = currentState.JobTotalPieces > 0 ? currentState.JobProducedPieces * 100 / currentState.JobTotalPieces : 0;
                m.ActualOperator = currentState.Operator;
                m.ActualStateCode = currentState.StateId == 3 ? currentState.StateTransitionCode : string.Empty;
                m.ActualStateId = currentState.StateId;
            }

            if (historyMessage.Any())
            {
                m.AlarmCount = historyMessage.Sum(hm => hm.Count ?? 0);
            }

            long? efficiencyValue = 0;
            long? totalElapsed = 0;

            if (historyEfficiency.Any())
            {
                
                if (machine.MachineTypeId == (int)enMachineType.Troncatrice || machine.MachineTypeId == (int)enMachineType.CentroLavoro)
                {
                    efficiencyValue = historyEfficiency.Where(he => he.StateId == 1 || he.StateId == 4)
                        .Sum(he => he.ElapsedTime);

                    totalElapsed = historyEfficiency.Where(he => he.StateId > 0)
                        .Sum(he => he.ElapsedTime);

                    m.StateEfficiency = totalElapsed > 0 ? efficiencyValue * 100 / totalElapsed : 0;
                }
                else
                {
                    efficiencyValue = historyEfficiency.Where(he => he.StateId == 1)
                        .Sum(he => he.ElapsedTime);

                    totalElapsed = historyEfficiency.Where(he => he.StateId > 0)
                        .Sum(he => he.ElapsedTime);

                    m.StateEfficiency = totalElapsed > 0 ? efficiencyValue * 100 / totalElapsed : 0;

                }

                //m.StateOverfeedAvg = historyEfficiency.Max(n => n.OverfeedAvg);
            }
            //prendo l'overfeed dell'ultimo state
            if (stateMachine != null)
            {
                m.StateOverfeedAvg = stateMachine.Overfeed;
            }

            if (historyPiece.Any())
            {
                m.PieceCompletedCount = historyPiece.Sum(hm => hm.CompletedCount ?? 0);
                m.PieceRedoneCount = historyPiece.Sum(hm => hm.RedoneCount ?? 0);
                //m.PieceElapsedTime = historyPiece.Sum(hm => hm.ElapsedTime ?? 0);
                //m.PieceElapsedTimeProducing = historyPiece.Sum(hm => hm.ElapsedTimeProducing ?? 0);
                m.PieceElapsedTime = totalElapsed ;
                m.PieceElapsedTimeProducing = efficiencyValue;
            }





            return m;
        }

        public List<PlantModel> GetAllPlantsMachines()
        {
            var result = new List<PlantModel>();

            try
            {               
                var query = _context.Set<Machine>().Where(w => w.Plant != null).Select(s => s.Plant).Distinct().ToList();
                result = query.Adapt<List<PlantModel>>();                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<PlantModel> GetAllPlantsRole(enRole role)
        {
            var result = new List<PlantModel>();

            try
            {
                var idUsers = _context.Set<Roles_Customer>().Where(e => e.Roles.IdRole == (int) role).Select(e => e.CustomerID)
                    .ToList();

                var query = _context.Set<UserMachineMapping>().Where(w => idUsers.Contains(w.UserId) && w.Machine.PlantId != null).Select(s => s.Machine.Plant).Distinct().ToList();
                result = query.Adapt<List<PlantModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        #endregion


        public void CheckOfflineMachines()
        {
            try
            {
                var machines = _context.Set<Machine>().Include("CurrentState").Where(m => m.CurrentState.FirstOrDefault() != null && 
                                                            m.CurrentState.FirstOrDefault().StateId != (int)enState.Offline).ToList();

                foreach (var machine in machines)
                {
                    var st = machine.CurrentState.FirstOrDefault();
                    var interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("CheckOfflineInterval"));
                    if (st == null || !(st.LastUpdated < DateTime.UtcNow.AddSeconds(interval * -1))) continue;
                    st.LastUpdated = DateTime.UtcNow;
                    st.StateId = (int)enState.Offline;
                }
                _context.SaveChanges();
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    null, "");
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
        }

    }
}
