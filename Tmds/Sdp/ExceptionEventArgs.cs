using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tmds.Sdp
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }
        public ExceptionEventArgs(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            Exception = exception;
        }
    }
}
