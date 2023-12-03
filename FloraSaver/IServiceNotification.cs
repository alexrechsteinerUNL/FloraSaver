using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver
{
    public interface IServiceNotification
    {
        void Start();

        void Stop();
    }
}