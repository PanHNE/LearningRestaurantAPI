using System;

namespace RestaurantAPI.Exceptions
{
    public class Timeout4sException : Exception
    {
        public Timeout4sException(string message) : base(message)
        {
            
        }
    }
}
