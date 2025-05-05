
using Stumana.DataAcess.Models;

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
