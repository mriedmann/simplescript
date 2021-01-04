using exampleservice.Framework.BaseFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Contract
{
    interface IHandlerBase<T>
    {
        internal abstract Task<EventBase> Handle<T>(CustomerCommandBase command);
    }
}
