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
using VectorSpace.MapData;

namespace VectorSpace.Dialogs
{
    /// <summary>
    /// Interaction logic for NewLayerWindow.xaml
    /// </summary>
    public partial class NewLayerWindow : Window
    {
        #region Variables & Properties
        private int _nextLayerId;

        /// <summary>
        /// The layer that was created
        /// </summary>
        public Layer Layer { get { return _layer; } }
        private Layer _layer;
        #endregion


        #region Constructor
        /// <summary>
        /// Creates a new layer window dialog
        /// </summary>
        /// <param name="nextLayerId">The next available layer id</param>
        public NewLayerWindow(int nextLayerId)
        {
            InitializeComponent();

            _nextLayerId = nextLayerId;
            LayerName.Text = "Layer " + _nextLayerId;
            CreateBtn.IsEnabled = true;
        }
        #endregion


        #region Event Handlers
        private void LayerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LayerName.Text.Length > 0)
                CreateBtn.IsEnabled = true;
            else
                CreateBtn.IsEnabled = false;
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LayerName.Text.Length > 0)
            {
                _layer = new Layer(_nextLayerId.ToString(), LayerName.Text);

                DialogResult = true;
                this.Close();
            }
        }
        #endregion
    }
}
