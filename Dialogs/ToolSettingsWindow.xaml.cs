using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VectorSpace.Dialogs
{
    /// <summary>
    /// Interaction logic for ApplicationSettingsWindow.xaml
    /// </summary>
    public partial class ToolSettingsWindow : Window
    {
        private Color _canvasBackgroundColor;

        private Color _canvasGridColor;
        private Point _canvasGridSize;

        public ToolSettingsWindow()
        {
            InitializeComponent();

            this.CanvasColorPicker.SelectedColor = _canvasBackgroundColor = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.CanvasBackgroundColor);

            this.GridColorPicker.SelectedColor = _canvasGridColor = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.CanvasGridColor);

            _canvasGridSize = new Point();
            _canvasGridSize.X = Properties.Settings.Default.CanvasGridSize.X;
            this.GridWidth.Text = _canvasGridSize.X.ToString(); 
            _canvasGridSize.Y = Properties.Settings.Default.CanvasGridSize.Y;
            this.GridHeight.Text = _canvasGridSize.Y.ToString();
        }


        private void saveSettings()
        {
            // Canvas
            Properties.Settings.Default.CanvasBackgroundColor = _canvasBackgroundColor.ToString();
            
            // Grid
            Properties.Settings.Default.CanvasGridColor = _canvasGridColor.ToString();
            Properties.Settings.Default.CanvasGridSize = new Point(int.Parse(this.GridWidth.Text), int.Parse(this.GridHeight.Text));

            Properties.Settings.Default.Save();
        }


        #region Button Click Handlers
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
            DialogResult = true;
            this.Close();
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
        }

        private void CanvasColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            _canvasBackgroundColor = this.CanvasColorPicker.SelectedColor;
        }

        private void GridColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            _canvasGridColor = this.GridColorPicker.SelectedColor;
        }
        #endregion
    }
}
