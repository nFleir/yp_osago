using System.Data.Entity.Core.Metadata.Edm;
using System.Windows;

namespace yp_osago
{
    public partial class App : Application
    {
        public int CurrentUserID { get; set; }
        public int CurrentPositionID { get; set; }

        public void SetCurrentUserID(int ID)
        {
            CurrentUserID = ID;
        }

        public void SetCurrentRoleID(int role_id)
        {
            CurrentPositionID = role_id;
        }
    }
}
