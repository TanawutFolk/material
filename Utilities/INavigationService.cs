using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    public interface INavigationService
    {
        void NavigateTo(UserControl userControl);
        void Clear();
    }
}
