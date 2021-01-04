using exampleservice.Framework.BaseFramework;

namespace exampleservice.CustomerService.Events
{
    public class GenericErrorEvent : EventBase
    {
        public GenericErrorEvent()
        {
            base.Message = "An error occured...";
        }

        public GenericErrorEvent(string message, int? code = null)
        {
            base.Message = message;

            if (code.HasValue)
                base.ErrorCode = code.Value;
        }
    }
}
