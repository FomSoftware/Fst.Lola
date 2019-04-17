﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserManager.Service
{
    public interface IGroupService
    {
        void ModifyGroup(UserManager.DAL.Groups group);

        void DeleteGroup(Guid GroupID);

        List<DAL.Groups> GetGroups();
        
        DAL.Groups GetGroup(Guid GroupsID);

        List<UserManager.DAL.Groups> GetListOfNonDeletedGroups(DAL.UserManagerEntities ModelloEntity);

    }
}