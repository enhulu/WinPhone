using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackKeyDemo
{
    public interface IBackable
    {
        bool GoBack();
        Stack<IBackable> BackStack
        {
            get;
            set;
        }
    }
}
