//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace yp_osago
{
    using System;
    using System.Collections.Generic;
    
    public partial class policies
    {
        public int policy_id { get; set; }
        public string policy_number { get; set; }
        public string insurance_company { get; set; }
        public Nullable<System.DateTime> issue_date { get; set; }
        public Nullable<System.DateTime> expiration_date { get; set; }
        public int driver_id { get; set; }
        public int car_id { get; set; }
        public string driving_license_series { get; set; }
        public string driving_license_number { get; set; }
        public decimal cost { get; set; }
    
        public virtual cars cars { get; set; }
        public virtual drivers drivers { get; set; }
    }
}
