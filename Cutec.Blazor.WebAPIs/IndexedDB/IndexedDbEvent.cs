using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs
{
    public enum IndexedDbEvent
    {
        // There are older versions of the database open, so this version cannot open.
        Blocked,

        // This connection is blocking a future version of the database from opening.
        Blocking,

        // Called if the browser abnormally terminates the connection. This is not called when `db.close()` is called.
        Terminated,
    }
}
