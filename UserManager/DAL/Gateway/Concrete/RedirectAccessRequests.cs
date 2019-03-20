using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserManager.DAL.Gateway.Concrete
{
    class RedirectAccessRequests
    {

        public static void InsertRedirectAccessRequests(Guid RequestID, Guid UserID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    DAL.RedirectAccessRequests NewRequestAccessRedirect = new DAL.RedirectAccessRequests()
                    {
                        RequestID = RequestID,
                        UserID = UserID
                    };

                    userManagerEntities.RedirectAccessRequests.Add(NewRequestAccessRedirect);

                    userManagerEntities.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(String.Format("Error: {0} (Error #111005 )", ex.Message));
            }
        }

        public static Guid GetUserIDFromRequestID(Guid RequestID, ref UserManagerEntities userManagerEntities)
        {
            DAL.RedirectAccessRequests AccessRequest = (from item in userManagerEntities.RedirectAccessRequests
                                                        where item.RequestID == RequestID
                                                        select item).FirstOrDefault();

            if (null == AccessRequest) { throw new Exception("Error: No redirect request found! (Error #111007"); }

            userManagerEntities.RedirectAccessRequests.Remove(AccessRequest);

            return (Guid)AccessRequest.UserID;
        }

    }
}
