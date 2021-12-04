using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSize = System.Drawing.Size;
using DPoint = System.Drawing.Point;

namespace ProcessResizer.Resizer
{
    internal class ResizerProcess
    {
        private static int INVALID_VALUE = -1;
        private Process _process;

        public string ProcessName { get => _process.ProcessName; }
        public DSize Size
        {
            get
            {
                if (!ResizerWinAPI.GetWindowRect(_process.MainWindowHandle, out ResizerRect rect))
                    throw new Exception("Size : GetWindowRect() 실패");

                return new DSize(rect.Right - rect.Left, rect.Bottom - rect.Top);
            }
            set
            {
                if (!ResizerWinAPI.SetWindowPos(
                    _process.MainWindowHandle,
                    (int)ResizerWindowFlags.Layer.HWND_TOP,
                    INVALID_VALUE,
                    INVALID_VALUE,
                    value.Width,
                    value.Height,
                    (uint)ResizerWindowFlags.WindowStyle.SWP_NOMOVE |
                    (uint)ResizerWindowFlags.WindowStyle.SWP_NOZORDER  |
                    (uint)ResizerWindowFlags.WindowStyle.SWP_DRAWFRAME
                ))
                    throw new Exception("Size : SetWindowPos() 실패");
            }
        }

        public DPoint Position
        {
            get
            {
                if (!ResizerWinAPI.GetWindowRect(_process.MainWindowHandle, out ResizerRect rect))
                    throw new Exception("Position : GetWindowRect() 실패");

                return new DPoint(rect.Left, rect.Top);
            }
            set
            {
                if (!ResizerWinAPI.SetWindowPos(
                       _process.MainWindowHandle,
                       (int)ResizerWindowFlags.Layer.HWND_TOP,
                       value.X,
                       value.Y,
                       INVALID_VALUE,
                       INVALID_VALUE,
                       (uint)ResizerWindowFlags.WindowStyle.SWP_NOSIZE |
                       (uint)ResizerWindowFlags.WindowStyle.SWP_NOZORDER |
                       (uint)ResizerWindowFlags.WindowStyle.SWP_DRAWFRAME
                   ))
                    throw new Exception("Position : SetWindowPos() 실패");
            }
        }


        public bool IsValid
        {
            get
            {
                foreach (var process in Process.GetProcesses())
                {
                    if (process.Id == _process.Id)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void SetWidth(int width)
        {
            DSize baseSize = Size;
            baseSize.Width = width;
            Size = baseSize;
        }

        public void SetHeight(int height)
        {
            DSize baseSize = Size;
            baseSize.Height = height;
            Size = baseSize;
        }

        public int GetWidth() => Size.Width;
        public int GetHeight() => Size.Height;
        public int AddWidth(int value) 
        {
            int newValue = GetWidth() + value;
            SetWidth(newValue);
            return newValue;
        }
        public int AddHeight(int value)
        {
            int newValue = GetHeight() + value;
            SetHeight(newValue);
            return newValue;
        }

        public string MainWindowTitle
        {
            get => _process.MainWindowTitle;
        }

        public ResizerProcess(Process process)
        {
            _process = process;
        }
    }
}
