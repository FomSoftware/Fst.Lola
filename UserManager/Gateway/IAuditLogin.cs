using System;
using System.Collections.Generic;

namespace UserManager.Gateway
{
    public interface IAuditLogin
    {
        void InsertAuditLogin(bool accessed, string username, Guid? userId, string message);
    }
}