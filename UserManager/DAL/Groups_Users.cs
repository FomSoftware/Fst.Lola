//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UserManager.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Groups_Users
    {
        public System.Guid ID { get; set; }
        public System.Guid GroupID { get; set; }

        [ForeignKey("Users")]
        public System.Guid UserID { get; set; }
    
        public virtual Groups Groups { get; set; }
        public virtual Users Users { get; set; }
    }
}
