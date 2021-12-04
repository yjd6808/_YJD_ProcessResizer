using ProcessResizer.Model;
using ProcessResizer.Resizer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessResizer.ViewModel
{
    internal class MainWindowViewModel
    {
        private readonly ResizerProcessCollection _processes = new ResizerProcessCollection();

        public void Add(ResizerProcess resizerProcess)
        {
            _processes.Add(resizerProcess);
        }

        public void Clear()
        {
            _processes.Clear();
        }

        public ObservableCollection<ResizerProcess> ResizerProcesses
        {
            get { return _processes; }
        }
    }
}
