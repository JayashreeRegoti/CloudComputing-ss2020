using System;

namespace MyExperiment.Exceptions
{
    public class StorageException : Exception
    {
        public StorageException(string message) : base(message)
        {
            
        }
    }
}