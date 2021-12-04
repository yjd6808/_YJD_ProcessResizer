using ProcessResizer.Resizer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DSize = System.Drawing.Size;
using DPoint = System.Drawing.Point;
using System.Security.Principal;

namespace ProcessResizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ResizerSetting _setting = ResizerSetting.GetInstance();
        private ResizerProcess _selectedResizerProcess = null;
        private bool _isProcessListViewPressed = false;
        private bool _isUpdatingHeight = false;
        private bool _isUpdatingWidth = false;

        public MainWindow()
        {
            PriviliegeChecking();
            InitializeComponent();
            UpdateProcessList();
        }

        private void PriviliegeChecking()
        {
            bool isElevated = false;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (isElevated == false)
            {
                MessageBox.Show("관리자 권한으로 실행해주세요");
                Application.Current.Shutdown();
            }
        }

        public void UpdateProcessList()
        {
            _mainWindowViewModel.Clear();

            foreach (var process in Process.GetProcesses())
            {
                // 윈도우 핸들이 존재하는 것만..
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    _mainWindowViewModel.Add(new ResizerProcess(process));
                }
            }
        }

        private void _buttonUpdateProcessList_Click(object sender, RoutedEventArgs e)
        {
            UpdateProcessList();
        }

        

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "[0-9]+");
        }


        private void TextBoxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(_textBoxWidth.Text, out int width))
            {
                if (width > _sliderWidth.Maximum)
                {
                    width = (int)_sliderWidth.Maximum;
                    _textBoxWidth.Text = width.ToString();
                }

                UpdateWidth(width, true, false);
            }
        }

        private void TextBoxHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(_textBoxHeight.Text, out int height))
            {
                if (height > _sliderHeight.Maximum)
                {
                    height = (int)_sliderHeight.Maximum;
                    _textBoxHeight.Text = height.ToString();
                }

                UpdateHeight(height, true, false);
            }
        }

        private void ListViewProcess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isProcessListViewPressed)
            {
                if (e.AddedItems.Count != 1)
                    return;

                ResizerProcess process = e.AddedItems[0] as ResizerProcess;

                if (process == null)
                    return;

                _textBlockSelectedProcessName.Text = process.ProcessName;
                _selectedResizerProcess = process;
                UpdateHeight(_selectedResizerProcess.GetHeight());
                UpdateWidth(_selectedResizerProcess.GetWidth());
            }
        }

        private void ListViewProcess_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isProcessListViewPressed = true;
        }

        private void ButtonApplyHeight_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedResizerProcess == null || !_selectedResizerProcess.IsValid)
            {
                MessageBox.Show("프로세스를 업데이트 후 다시 선택해주세요.");
                return;
            }

            int.TryParse(_textBoxHeight.Text, out int height);

            if (height < _sliderHeight.Minimum || height > _sliderHeight.Maximum)
            {
                MessageBox.Show("100이상 1920이하의 크기만 적용가능합니다.");
                return;
            }

            _selectedResizerProcess.SetHeight(height);
            UpdateHeight(height);
        }

        private void ButtonApplyWidth_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedResizerProcess == null || !_selectedResizerProcess.IsValid)
            {
                MessageBox.Show("프로세스를 업데이트 후 다시 선택해주세요.");
                return;
            }

            int.TryParse(_textBoxWidth.Text, out int width);

            if (width < _sliderWidth.Minimum || width > _sliderWidth.Maximum)
            {
                MessageBox.Show("100이상 1080이하의 크기만 적용가능합니다.");
                return;
            }

            _selectedResizerProcess.SetWidth(width);
            UpdateWidth(width);
        }

        private void SliderWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdatingWidth)
                return;

            if (_selectedResizerProcess == null)
                return;

            // 쓰레드로 처리하도록 변경.
            Task.Run(() => _selectedResizerProcess.SetWidth((int)e.NewValue));
        }

        private void SliderHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdatingHeight)
                return;

            if (_selectedResizerProcess == null)
                return;

            Task.Run(() => _selectedResizerProcess.SetHeight((int)e.NewValue));
        }

        private void Slider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_selectedResizerProcess == null || !_selectedResizerProcess.IsValid)
            {
                MessageBox.Show("프로세스를 업데이트 후 다시 선택해주세요.");
                e.Handled = true;
                return;
            }
        }

        private void ButtonMinusHeight_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedResizerProcess == null || !_selectedResizerProcess.IsValid)
            {
                MessageBox.Show("프로세스를 업데이트 후 다시 선택해주세요.");
                return;
            }

            Task.Run(() => _selectedResizerProcess.AddHeight(-1))
                .ContinueWith((updatedValue) => UpdateHeight(updatedValue.Result));
        }

        private void ButtonPlusHeight_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedResizerProcess == null || !_selectedResizerProcess.IsValid)
            {
                MessageBox.Show("프로세스를 업데이트 후 다시 선택해주세요.");
                return;
            }

            Task.Run(() => _selectedResizerProcess.AddHeight(1))
                .ContinueWith((updatedValue) => UpdateHeight(updatedValue.Result));
        }

        private void UpdateHeight(int updatedHeight, bool applySlider = true, bool applyTextBox = true)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                _isUpdatingHeight = true;

                if (applySlider)
                    _sliderHeight.Value = updatedHeight;

                if (applyTextBox)
                    _textBoxHeight.Text = updatedHeight.ToString();

                _isUpdatingHeight = false;
            });
        }


        private void ButtonPlusWidth_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedResizerProcess == null || !_selectedResizerProcess.IsValid)
            {
                MessageBox.Show("프로세스를 업데이트 후 다시 선택해주세요.");
                return;
            }

            Task.Run(() => _selectedResizerProcess.AddWidth(1))
                .ContinueWith((updatedValue) => UpdateWidth(updatedValue.Result));
        }

        private void ButtonMinusWidth_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedResizerProcess == null || !_selectedResizerProcess.IsValid)
            {
                MessageBox.Show("프로세스를 업데이트 후 다시 선택해주세요.");
                return;
            }

            Task.Run(() => _selectedResizerProcess.AddWidth(-1))
                .ContinueWith((updatedValue) => UpdateWidth(updatedValue.Result));
        }

        private void UpdateWidth(int updatedWidth, bool applySlider = true, bool applyTextBox = true)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                _isUpdatingWidth = true;

                if (applySlider)
                    _sliderWidth.Value = updatedWidth;

                if (applyTextBox)
                    _textBoxWidth.Text = updatedWidth.ToString();

                _isUpdatingWidth = false;
            });
        }
    }
}
