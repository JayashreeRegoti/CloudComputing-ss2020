using System;

namespace LearningFoundation.DataMappers
{
    internal class MLException : Exception
    {
        public MLException()
        {
        }

        public MLException(string message) : base(message)
        {
        }

        public MLException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}