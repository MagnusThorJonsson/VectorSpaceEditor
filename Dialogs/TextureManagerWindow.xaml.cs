using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
    /// Interaction logic for TextureManagerWindow.xaml
    /// </summary>
    public partial class TextureManagerWindow : Window
    {
        #region Variables & Properties
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".JPEG", ".BMP", ".GIF", ".PNG", ".TIF", ".TIFF" };

        public ObservableCollection<Texture> Textures;

        public TextureLibrary TextureLibrary;

        private bool _isEdit;
        private string _mapPath;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a TextureManagerWindow used to create a new library
        /// </summary>
        /// <param name="mapPath">The current map project path</param>
        public TextureManagerWindow(string mapPath)
        {
            _mapPath = mapPath;
            Textures = new ObservableCollection<Texture>();
            TextureLibrary = null;
            InitializeComponent();

            TextureLibName.Focus();

            CreateBtn.Content = "Create";
            
            CreateLibraryPanel.Visibility = Visibility.Visible;
            AddTexturePanel.Visibility = Visibility.Collapsed;

            _isEdit = false;
        }

        /// <summary>
        /// Constructs a TextureManagerWindow used to edit an already existing library
        /// </summary>
        /// <param name="mapPath">The current map project path</param>
        /// <param name="library">The library to edit</param>
        public TextureManagerWindow(string mapPath, TextureLibrary library)
        {
            _mapPath = mapPath;
            TextureLibrary = library;
            Textures = library.Textures;

            InitializeComponent();

            TextureLibName.Focus();

            CreateBtn.Content = "Save";

            // Disable the folder browse
            FileBrowseBtn.IsEnabled = false;

            // Bind to the texture list box and enable the create button
            TextureListBox.ItemsSource = Textures;

            TextureLibName.Text = TextureLibrary.Name;

            CreateLibraryPanel.Visibility = Visibility.Collapsed;
            AddTexturePanel.Visibility = Visibility.Visible;

            _isEdit = true;
        }
        #endregion


        #region Button Handlers
        private void FileBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && Directory.Exists(dialog.SelectedPath))
            {
                TextureListBox.ItemsSource = null;
                clearTexturePreview();
                Textures.Clear();

                // Display path and get the file list
                TextureLibLocation.Text = dialog.SelectedPath;
                string[] files = Directory.GetFiles(dialog.SelectedPath);

                // Filter out any file that isn't an image and add to texture list
                for (int i = 0; i < files.Length; i++)
                    addFileAsTexture(files[i]);

                // Bind to the texture list box and enable the create button
                TextureListBox.ItemsSource = Textures;

                if (Textures.Count > 0 && TextureLibName.Text.Length > 0)
                    CreateBtn.IsEnabled = true;
                else
                    CreateBtn.IsEnabled = false;
            }
        }

        private void AddTextureBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "All Images|*.jpeg;*.jpg;*.png;*.gif;*.bmp;*.tif;*.tiff" +
                         "|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp|TIF Files (*.tif)|*.tif|TIFF Files (*.tiff)|*.tiff";

            Nullable<bool> result = dlg.ShowDialog();
            if (result != null || result == true)
            {
                addFileAsTexture(dlg.FileName);
            }
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextureLibName.Text != null && Textures != null && Textures.Count > 0)
            {
                // If we're not editing we create a Texture Library, else we just update the name
                if (!_isEdit)
                {
                    // Populate library
                    TextureLibrary = new TextureLibrary(
                        TextureLibName.Text,
                        Textures
                    );
                }
                else
                    TextureLibrary.Name = TextureLibName.Text;

                DialogResult = true;
                this.Close();
            }
        }


        private void TextureLibName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextureLibName.Text.Length > 0 && TextureListBox.ItemsSource != null && Textures.Count > 0)
                CreateBtn.IsEnabled = true;
            else
                CreateBtn.IsEnabled = false;
        }
        #endregion


        #region Move Origin Handlers
        private void MoveOriginBtn_Click(object sender, RoutedEventArgs e)
        {
        }
        #endregion


        #region Texture Preview Handlers
        private void TextureListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TextureListBox.SelectedItem != null)
            {
                TexturePreviewImage.Source = ((Texture)TextureListBox.SelectedItem).Source;
                TextureNameLabel.Content = ((Texture)TextureListBox.SelectedItem).Name;
                TextureSizeLabel.Content = ((Texture)TextureListBox.SelectedItem).Size.Width + " x " + ((Texture)TextureListBox.SelectedItem).Size.Height;
                TextureOriginLabel.Content = ((Texture)TextureListBox.SelectedItem).Origin.X + " x " + ((Texture)TextureListBox.SelectedItem).Origin.Y;

                RemovePropertyBtn.IsEnabled = true;
                AddPropertyBtn.IsEnabled = true;
                MoveOriginBtn.IsEnabled = true;
                AddCollisionBtn.IsEnabled = true;

                PropertiesDataGrid.ItemsSource = ((Texture)TextureListBox.SelectedItem).Properties;
            }
            else
                clearTexturePreview();
        }
        #endregion


        #region Property Button Handlers
        private void AddPropertyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextureListBox.SelectedItem != null)
            {
                ((Texture)TextureListBox.SelectedItem).AddProperty("Key", "Value");
            }
        }

        private void RemovePropertyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextureListBox.SelectedItem != null)
            {
                if (PropertiesDataGrid.SelectedItem != null)
                {
                    ((Texture)TextureListBox.SelectedItem).RemoveProperty((ItemProperty)PropertiesDataGrid.SelectedItem);
                }
            }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Adds a file as a texture
        /// </summary>
        /// <param name="file">The file to add</param>
        private void addFileAsTexture(string file)
        {
            if (ImageExtensions.Contains(System.IO.Path.GetExtension(file).ToUpperInvariant()))
            {
                Textures.Add(new Texture(file));
            }
        }

        /// <summary>
        /// Clears the texture preview
        /// </summary>
        private void clearTexturePreview()
        {
            RemovePropertyBtn.IsEnabled = false;
            AddPropertyBtn.IsEnabled = false;
            MoveOriginBtn.IsEnabled = false;
            AddCollisionBtn.IsEnabled = false;

            TexturePreviewImage.Source = null;
            TextureNameLabel.Content = null;
            TextureSizeLabel.Content = null;

            PropertiesDataGrid.ItemsSource = null;
        }
        #endregion


    }
}
