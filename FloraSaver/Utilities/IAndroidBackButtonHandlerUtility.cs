using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Utilities
{
    public interface IAndroidBackButtonHandlerUtility
    {
        public Task<bool> HandleBackButtonAsync();
    }
}
