using System;
namespace ndf5.Exceptions
{
    public class RequiredMessageNotUnderstood : System.IO.IOException
    {
        public RequiredMessageNotUnderstood(
            String aMessage = null,
            Exception aInternalException = null) : base(
                aMessage,
                aInternalException)
        {
            
        }
    }
}
