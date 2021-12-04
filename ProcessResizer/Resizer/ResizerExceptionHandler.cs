using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcessResizer.Resizer
{
    internal static class ResizerExceptionHandler
    {
        public static void Handle(Exception e)
        {
            MessageBox.Show(e.Message + "\n\n" + e.StackTrace);
        }
    }
}
