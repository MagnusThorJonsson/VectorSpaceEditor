using System;
using System.Collections.Generic;
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

        public List<Texture> Textures;

        public TextureLibrary TextureLibrary;
        #endregion

        #region Constructors
        public TextureManagerWindow()
        {
            Textures = new List<Texture>();
            TextureLibrary = null;
            InitializeComponent();

            TextureLibName.Focus();
        }
        #endregion


        #region Loading Handlers
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
                {
                    if (ImageExtensions.Contains(System.IO.Path.GetExtension(files[i]).ToUpperInvariant()))
                        Textures.Add(new Texture(files[i]));
                }

                // Bind to the texture list box and enable the create button
                TextureListBox.ItemsSource = Textures;

                if (Textures.Count > 0 && TextureLibName.Text.Length > 0)
                    CreateBtn.IsEnabled = true;
                else
                    CreateBtn.IsEnabled = false;
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


        #region Texture Preview Handlers
        private void TextureListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TextureListBox.SelectedItem != null)
            {
                TexturePreviewImage.Source = ((Texture)TextureListBox.SelectedItem).Source;
                TextureNameLabel.Content = ((Texture)TextureListBox.SelectedItem).Name;
                TextureSizeLabel.Content = ((Texture)TextureListBox.SelectedItem).Size.X + " x " + ((Texture)TextureListBox.SelectedItem).Size.Y;

                RemovePropertyBtn.IsEnabled = true;
                AddPropertyBtn.IsEnabled = true;

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
        private void clearTexturePreview()
        {
            RemovePropertyBtn.IsEnabled = false;
            AddPropertyBtn.IsEnabled = false;

            TexturePreviewImage.Source = null;
            TextureNameLabel.Content = null;
            TextureSizeLabel.Content = null;

            PropertiesDataGrid.ItemsSource = null;
        }
        #endregion


        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextureLibName.Text != null && Textures != null && Textures.Count > 0)
            {
                // Populate library
                TextureLibrary = new TextureLibrary(
                    TextureLibName.Text,
                    Textures
                );

                DialogResult = true;
                this.Close();
            }
        }
    }
}
