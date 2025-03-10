
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stumana.WPF.Stores;

namespace Stumana.WPF.Commands
{
    public class CancelCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            ModalNavigationStore.Instance.Close();
        }
    }

}
