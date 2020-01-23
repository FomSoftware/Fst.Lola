using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using UserManager.DAL;

namespace FomMonitoringCore.Service
{
    public class MesService : IMesService
    {
        #region API

        private IFomMonitoringEntities _context;
        
        public MesService(IFomMonitoringEntities context)
        {
            _context = context;
        }


        public int? GetPlantDefaultByMachine(string machineSerial)
        {                          
            var userIds = _context.Set<Machine>().Where(m => m.Serial == machineSerial).SelectMany(n => n.UserMachineMapping).Select(um => um.UserId).ToList();
            var plant = _context.Set<Plant>().FirstOrDefault(p => userIds.Any(g => g == p.UserId));
            return plant?.Id;                
            
        }

        public int? GetOrSetPlantDefaultByUser(Guid userId)
        {            
            using (UserManagerEntities uent = new UserManagerEntities())
            {
                var userM = _context.Set<UserCustomerMapping>().FirstOrDefault(m => m.UserId == userId);
                var plant = _context.Set<Plant>().FirstOrDefault(p => userId == p.UserId);

                if(plant == null)
                {
                    var user = uent.Users.FirstOrDefault(u => u.ID == userM.UserId);
                    plant = AddPlant(user);
                }
                return plant?.Id;
            }
            
        }

        public int? GetOrSetPlantIdByPlantName(string plantName, string plantAddress, string machineSerial)
        {
            int? result = null;
            try
            {                
                Machine machine = _context.Set<Machine>().FirstOrDefault(m => m.Serial == machineSerial);
                UserManager.DAL.Users user;
                Plant plant = null;
                IEnumerable<Guid> userIds;
                userIds = _context.Set<Machine>().Where(m => m.Serial == machineSerial).SelectMany(n => n.UserMachineMapping).Select(um => um.UserId).ToList();
              
                using (UserManager.DAL.UserManagerEntities uent = new UserManager.DAL.UserManagerEntities())
                {
                    //l'utente a cui appartiene il plant tra tutti quelli 
                    //a cui la macchina è associata sarà sempre il customer
                    //e ve ne sarà sempre e solo uno
                    user = uent.Users.FirstOrDefault(u => userIds.Any(us => us == u.ID) && u.Roles_Users.Any(ur => ur.Roles.IdRole == (int)UserManager.Framework.Common.Enumerators.UserRole.Customer));
                }                

                if (machine != null && machine.Plant != null && machine.Plant.UserId == user?.ID)
                {                        
                    return machine.Plant.Id;
                }                   
                //se c'è il pant con il nome inviato dalla macchina lo associo
                if (!string.IsNullOrWhiteSpace(plantName))
                {
                    plant = _context.Set<Plant>().FirstOrDefault(f => f.Name == plantName && f.Address == plantAddress);
                }
                //se non c'è cerco il primo plant associato all'utente della macchina,
                //N.B. quando non ci sarà più solo il plant di default modificare
                    
                if (plant == null && user != null)
                {
                    plant = _context.Set<Plant>().FirstOrDefault(f => f.UserId == user.ID);
                }
                if (plant != null && user != null)
                {
                    plant.UserId = user.ID;
                }
                //se non c'è creo il plant di default per l'utente, 
                // precondizione: il record dalla VIP area che associa la macchina ad un utente deve esistere
                //altrimenti non si saprebbe a chi associare il plant, la macchina viene inserita con plantid null
                if (plant == null && user != null)
                {
                    plant = AddPlant(user, plantName, plantAddress);
                }

                if (plant != null)
                {
                    result = plant.Id;

                    _context.SaveChanges();
                }
                
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), plantName, plantAddress);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public Plant AddPlant(Users user, string plantName = null, string plantAddress = null)
        {
            Plant result = null;

            if (string.IsNullOrWhiteSpace(plantName))
            {
                plantName = "DEFAULT_PLANT_" + user.Username;
            }

            try
            {                
                Plant plant = new Plant();
                plant.Name = plantName;
                plant.Address = plantAddress;
                plant.UserId = user?.ID;
                _context.Set<Plant>().Add(plant);
                _context.SaveChanges();
                    
                result = plant;                                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), plantName, plantAddress);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        #endregion

        #region APP WEB

        public List<PlantModel> GetUserPlants(Guid UserID)
        {
            List<PlantModel> result = new List<PlantModel>();

            try
            {               
                var query = _context.Set<UserMachineMapping>().Where(w => w.UserId == UserID && w.Machine.PlantId != null).Select(s => s.Machine.Plant).Distinct().ToList();
                result = query.Adapt<List<PlantModel>>();                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), UserID.ToString());
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
            var currentState = machine.CurrentState.FirstOrDefault(cs => cs.LastUpdated.HasValue && cs.LastUpdated.Value.Date == date);
            var historyMessage = machine.HistoryMessage.Where(cs => cs.Day.HasValue && cs.Day.Value.Date == date && cs.MessagesIndex?.MessageTypeId == 11).ToList();
            var historyPiece = machine.HistoryPiece.Where(cs => cs.Day.HasValue && cs.Day.Value.Date == date && cs.Shift == null && cs.Operator == null).ToList();
            var historyEfficiency = machine.HistoryState.Where(cs => cs.Day.HasValue && cs.Day.Value.Date == date && cs.Shift == null && cs.Operator == null).ToList();


            var m = new MachineMesDataModel();
            m.Id = machine.Id;
            m.MachineId = machine.Id;
            m.ExpirationDate = machine.ExpirationDate;
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
                m.AlarmElapsedTime = historyMessage.Sum(hm => hm.ElapsedTime ?? 0);
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

                m.StateOverfeedAvg = historyEfficiency.Max(n => n.OverfeedAvg);
            }

            if (historyPiece.Any())
            {
                m.PieceCompletedCount = historyPiece.Sum(hm => hm.CompletedCount ?? 0);
                //m.PieceElapsedTime = historyPiece.Sum(hm => hm.ElapsedTime ?? 0);
                //m.PieceElapsedTimeProducing = historyPiece.Sum(hm => hm.ElapsedTimeProducing ?? 0);
                m.PieceElapsedTime = totalElapsed ;
                m.PieceElapsedTimeProducing = efficiencyValue;
            }





            return m;
        }

        public List<PlantModel> GetAllPlantsMachines()
        {
            List<PlantModel> result = new List<PlantModel>();

            try
            {               
                    var query = _context.Set<Machine>().Where(w => w.Plant != null).Select(s => s.Plant).Distinct().ToList();
                    result = query.Adapt<List<PlantModel>>();                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        #endregion


        public void CheckOfflineMachines()
        {
            string MachinId = null;
            try
            {
                List<Machine> machines = _context.Set<Machine>().Where(m => m.CurrentState.FirstOrDefault() != null && 
                                                            m.CurrentState.FirstOrDefault().StateId != (int)enState.Offline).ToList();

                foreach (Machine machine in machines)
                {
                    CurrentState st = machine.CurrentState.FirstOrDefault();
                    int interval = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("CheckOfflineInterval"));
                    if (st.LastUpdated < DateTime.UtcNow.AddSeconds(interval * -1))
                    {
                        st.LastUpdated = DateTime.UtcNow;
                        st.StateId = (int)enState.Offline;
                    }                   
                }
                _context.SaveChanges();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    MachinId, "");
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
        }

    }
}
