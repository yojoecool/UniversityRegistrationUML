//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UniversityRegistration.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AddRequest
    {
        public int Id { get; set; }
        public Nullable<int> ClassID { get; set; }
        public Nullable<int> StudentID { get; set; }
        public Nullable<bool> Processed { get; set; }
    }
}
