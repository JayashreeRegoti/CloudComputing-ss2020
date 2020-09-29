using System;

namespace MyExperiment.Exceptions
{
    public class ObjectShouldNotBeNUllException : Exception
    {
        public ObjectShouldNotBeNUllException(string message)
            : base(message)
        {

        }
    }
}