using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenStem.ClassModules
{
    public class modPublicVariable
    {
        public static string UpdateLockType = "WITH (ROWLOCK)";
        public static string ReadLockType = "WITH (NOLOCK)";
        public static string CompanyName;
        public static string Company;
        public static string UserID;
    }
}
