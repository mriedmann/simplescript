using exampleservice.Framework.BaseFramework;
using System;
using System.Threading.Tasks;

namespace exampleservice.CustomerService.Contract
{
    internal abstract class CustomerHandlerBase<T> where T : CustomerCommandBase
    {
        internal abstract Task<EventBase> Handle(T command);

        protected virtual void VerifyIputArguments(T command)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }
        }

    }
}