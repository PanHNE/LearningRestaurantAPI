using System;

namespace RestaurantAPI.Exceptions
{
    public class AlreadyExists : Exception
    {
        public AlreadyExists(string message) : base(message) 
        {
            
        }
    }
}
