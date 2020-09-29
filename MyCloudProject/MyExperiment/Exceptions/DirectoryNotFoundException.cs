using System;

namespace MyExperiment.Exceptions
{
    public class DirectoryNotFoundException : Exception
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public DirectoryNotFoundException(String message) : base(message)
        {
            
        }
    }
}