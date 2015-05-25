using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Common
{
    public enum AppStatus
    {
        New = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3
    }

    public enum AppLogType
    {
        AppLog = 0,
        AppError = 1
    }
}
