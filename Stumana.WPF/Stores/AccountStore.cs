
using Stumana.DataAcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.WPF.Stores
{
    public class AccountStore
    {
        public User? CurrentUser { get; set; }
        private AccountStore() { }

        private static AccountStore _instance;

        public static AccountStore Instance
        {
            get
            {
                if (_instance == null) _instance = new AccountStore();
                return _instance;
            }
        }

       
    }
}
