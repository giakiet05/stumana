﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAccess.Exceptions
{
    public class UsernameAlreadyExistsException : Exception
    {


        public string Username { get; set; } 

        public UsernameAlreadyExistsException(string username)
        {
            Username = username;
        }

        public UsernameAlreadyExistsException(string? message, string username) : base(message)
        {
            Username = username;
        }

        public UsernameAlreadyExistsException(string? message, Exception? innerException, string username) : base(message, innerException)
        {
            Username = username;
        }

     
    }
}
