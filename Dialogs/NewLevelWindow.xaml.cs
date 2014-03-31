using System;
using System.Collections.Generic;
using System.IO;
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
using VectorSpace.MapData.Components;

namespace VectorSpace.Dialogs
{
    /// <summary>
    /// Interaction logic for NewLevelWindow.xaml
    /// </summary>
    public partial class NewLevelWindow : Window
    {
        #region Variables & Properties
        public Map Map;
        #endregion


        #region Constructor
        public NewLevelWindow()
        {
            InitializeComponent();

            this.Title = "Create a new map";
            CreateBtn.Content = "Create";
            LevelName.Focus();
        }

        public NewLevelWindow(Map map)
        {
            this.Map = map;
            InitializeComponent();

            this.Title = "Edit map settings";
            LevelName.Text = map.Name;
            LevelLocation.Text = map.FilePath;
            LevelLocation.IsReadOnly = true;
            PropertiesDataGrid.ItemsSource = Map.Properties;

            AddPropertyBtn.IsEnabled = true;
            RemovePropertyBtn.IsEnabled = true;
            FileBrowseBtn.IsEnabled = false;

            CreateBtn.Content = "Save";
            if (LevelName.Text.Length > 0)
                CreateBtn.IsEnabled = true;
            else
                CreateBtn.IsEnabled = false;

            LevelName.Focus();
        }
        #endregion


        #region Directory Browse and Naming
        private void FileBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && Directory.Exists(dialog.SelectedPath))
            {
                Map = new Map();
                PropertiesDataGrid.ItemsSource = Map.Properties;
                AddPropertyBtn.IsEnabled = true;
                RemovePropertyBtn.IsEnabled = true;

                // Display path and get the file list
                LevelLocation.Text = dialog.SelectedPath;

                if (LevelName.Text.Length > 0)
                    CreateBtn.IsEnabled = true;
                else
                    CreateBtn.IsEnabled = false;
            }
            else
            {
                Map = null;
                PropertiesDataGrid.ItemsSource = null;
                AddPropertyBtn.IsEnabled = false;
                RemovePropertyBtn.IsEnabled = false;

                // Display path and get the file list
                LevelLocation.Text = null;

                CreateBtn.IsEnabled = false;
            }
        }

        private void LevelName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LevelName.Text.Length > 0 && LevelLocation.Text.Length > 0 && Map != null)
                CreateBtn.IsEnabled = true;
            else
                CreateBtn.IsEnabled = false;
        }
        #endregion


        #region Property Button Handlers
        private void AddPropertyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Map != null)
            {
                Map.AddProperty("Key", "Value");
            }
        }

        private void RemovePropertyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Map != null)
            {
                if (PropertiesDataGrid.SelectedItem != null)
                {
                    Map.RemoveProperty((ItemProperty)PropertiesDataGrid.SelectedItem);
                }
            }
        }
        #endregion


        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LevelName.Text.Length > 0 && LevelLocation.Text.Length > 0 &&
                LevelWidthText.Value.HasValue && LevelHeightText.Value.HasValue &&
                Map != null)
            {
                // Set basics data
                Map.SetName(LevelName.Text);
                Map.SetPath(LevelLocation.Text);
                Map.SetSize(LevelWidthText.Value.Value, LevelHeightText.Value.Value);

                DialogResult = true;
                this.Close();
            }
        }
    }
}
