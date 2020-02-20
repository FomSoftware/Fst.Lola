using System;
using System.Collections.Generic;

namespace UserManager.Service.Concrete
{
    public class GroupService : IGroupService
    {
        public void ModifyGroup(DAL.Groups Group)
        {
            DAL.Gateway.Concrete.Groups.ModifyGroup(Group);
        }

        public void DeleteGroup(Guid GroupID)
        {
            DAL.Gateway.Concrete.Groups.DeleteGroup(GroupID);
        }

        public List<UserManager.DAL.Groups> GetListOfNonDeletedGroups(DAL.UserManagerEntities ModelloEntity)
        {
            return DAL.Gateway.Concrete.Groups.GetListOfNonDeletedGroups(ModelloEntity);

        }
        public List<DAL.Groups> GetGroups()
        {            
            return DAL.Gateway.Concrete.Groups.GetGroups();    
        }

        public static List<DAL.Groups> GetUserGroups(Guid UserID)
        {
            return DAL.Gateway.Concrete.Groups.GetGroupsFromUserID(UserID);
        }

        public static void AddGroupToUser(Guid UserID, Guid GroupID)
        {
            DAL.Gateway.Concrete.Groups.AddGroupsToUser(UserID, new List<Guid>() { GroupID });
        }

        public DAL.Groups GetGroup(Guid GroupsID)
        {
            return DAL.Gateway.Concrete.Groups.GetGroups(GroupsID);
        }
        public static void AddGroupsToUser(Guid UserID, List<Guid> GroupsID)
        {
            DAL.Gateway.Concrete.Groups.AddGroupsToUser(UserID, GroupsID);
        }
    }
}
