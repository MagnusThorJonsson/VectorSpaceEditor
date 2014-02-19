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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VectorSpace.Dialogs;
using VectorSpace.MapData;
using VectorSpace.MapData.Components;
using VectorSpace.MapData.Interfaces;
using VectorSpace.MapData.MapItems;
using Xceed.Wpf.Toolkit;
using VectorSpace.Controls;

namespace VectorSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Map Map { get { return _currentMap; } }
        private Map _currentMap;

        #region Variables & Properties
        private VisualBrush canvasGrid;
        private bool _isGridShowing;

        private IHasProperties _currentPropertyContainer;
        #endregion


        #region Constructor & Initialization
        /// <summary>
        /// Application Main View Constructor
        /// </summary>
        public MainWindow()
        {
            _currentMap = null;
            _isGridShowing = false;
            InitializeComponent();
            loadResources();
        }

        /// <summary>
        /// Loads resources
        /// </summary>
        private void loadResources()
        {
            // CANVAS BACKGROUND COLOR
            this.Background = this.MainCanvasPanel.Background = (Brush)new BrushConverter().ConvertFromString(Properties.Settings.Default.CanvasBackgroundColor);
            loadCanvasGrid();
        }

        /// <summary>
        /// Loads or updates the Canvas Grid with the values from the User Settings configuration
        /// </summary>
        private void loadCanvasGrid()
        {
            if (canvasGrid == null)
            {
                canvasGrid = new VisualBrush();
                canvasGrid.TileMode = TileMode.Tile;
                canvasGrid.ViewportUnits = canvasGrid.ViewboxUnits = BrushMappingMode.Absolute;
                Rectangle visual = new Rectangle();
                visual.StrokeThickness = 1.0;
                canvasGrid.Visual = visual;
            }

            canvasGrid.Viewbox = new Rect(0, 0, Properties.Settings.Default.CanvasGridSize.X, Properties.Settings.Default.CanvasGridSize.Y);
            canvasGrid.Viewport = new Rect(-1, -1, Properties.Settings.Default.CanvasGridSize.X, Properties.Settings.Default.CanvasGridSize.Y);
            ((Rectangle)canvasGrid.Visual).Width = Properties.Settings.Default.CanvasGridSize.X + 1;
            ((Rectangle)canvasGrid.Visual).Height = Properties.Settings.Default.CanvasGridSize.Y + 1;
            ((Rectangle)canvasGrid.Visual).Stroke = (Brush)new BrushConverter().ConvertFromString(Properties.Settings.Default.CanvasGridColor);

            // Refresh canvas grid
            showCanvasGrid(_isGridShowing);
        }
        #endregion


        #region MenuItem Handlers & Helpers
        
        #region MenuItem File Handlers

        #region File New Command Handlers
        private void MenuItem_File_New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; // Always
        }

        private void MenuItem_File_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewLevelWindow levelWindow = new NewLevelWindow();
            levelWindow.Owner = this;
            levelWindow.ShowDialog();

            if (levelWindow.DialogResult.HasValue && levelWindow.DialogResult.Value == true)
            {
                // Attach map and enable menu items
                _currentMap = levelWindow.Map;
                ToolMenuItem_TextureManager.IsEnabled = true;
                assingPropertyGrid(_currentMap);

                // Set canvas size and display it
                MainCanvasPanel.Visibility = System.Windows.Visibility.Visible;
                LevelCanvas.ItemsSource = _currentMap.MapItems;
                LevelCanvas.IsEnabled = true;
                LevelCanvas.Width = _currentMap.Size.X;
                LevelCanvas.Height = _currentMap.Size.Y;

                // Bind the texture libraries to the Canvas Tab Item Control
                CanvasItemsTab.ItemsSource = _currentMap.TextureLibraries;

                // Layers panel
                RemoveLayerBtn.IsEnabled = true;
                AddLayerBtn.IsEnabled = true;
                LayersListBox.ItemsSource = _currentMap.Layers;
                LayersListBox.SelectedIndex = 0;

                enableTools(true);
            }
        }
        #endregion

        #region File Open Command Handlers
        private void MenuItem_File_Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; // Always
        }

        private void MenuItem_File_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
        #endregion

        #region File Close Command Handlers
        private void MenuItem_File_Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_currentMap != null)
            {
                e.CanExecute = true;
            }
            else
                e.CanExecute = false;
        }

        private void MenuItem_File_Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            enableTools(false);
            MainCanvasPanel.Visibility = System.Windows.Visibility.Hidden;
            LevelCanvas.IsEnabled = false;
            // TODO: Probably need to handle this in a better way
            _currentMap = null;
        }
        #endregion


        /// <summary>
        /// Exit application
        /// </summary>
        private void MenuItem_File_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion


        #region MenuItem Edit Handlers
        private void MenuItem_Edit_Settings(object sender, RoutedEventArgs e)
        {
            ApplicationSettingsWindow settingsDlg = new ApplicationSettingsWindow();
            settingsDlg.Owner = this;
            settingsDlg.ShowDialog();

            if (settingsDlg.DialogResult.HasValue)
            {
                // Do everything that doesn't depend on the user pressing the OK button
                loadResources();

                // If the OK button was pressed
                if (settingsDlg.DialogResult.Value)
                {
                }
            }
        }
        #endregion


        #region MenuItem View Handlers
        private void MenuItem_View_ShowGridChecked(object sender, RoutedEventArgs e)
        {
            _isGridShowing = true;
            showCanvasGrid(_isGridShowing);
        }

        private void MenuItem_View_ShowGridUnchecked(object sender, RoutedEventArgs e)
        {
            _isGridShowing = false;
            showCanvasGrid(_isGridShowing);
        }
        #endregion


        #region MenuItem Tools Handlers
        private void MenuItem_Tools_TextureManager(object sender, RoutedEventArgs e)
        {
            TextureManagerWindow txtManagerWindow = new TextureManagerWindow();
            txtManagerWindow.Owner = this;
            txtManagerWindow.ShowDialog();

            if (_currentMap != null && txtManagerWindow.DialogResult.HasValue && txtManagerWindow.DialogResult.Value == true && txtManagerWindow.TextureLibrary != null)
            {
                // Add texture library to the map
                _currentMap.AddTextureLibrary(txtManagerWindow.TextureLibrary);
                CanvasItemsTab.SelectedIndex = CanvasItemsTab.Items.Count - 1;

                //TEST
                _currentMap.AddItem(
                    0,
                    new TextureItem(0, "testing", _currentMap.TextureLibraries[0].Textures[0], new WorldPosition(new System.Drawing.Point(50, 50), new System.Drawing.Point(), 1f, 1f, 0f), 1)
                );

            }
        }
        #endregion


        #region MenuItem View Helpers
        /// <summary>
        /// Show or hide the Canvas Grid
        /// </summary>
        /// <param name="showGrid">True for grid overlay</param>
        private void showCanvasGrid(bool showGrid)
        {
            if (!showGrid)
            {
                this.LevelCanvas.Background = null;
            }
            else
                this.LevelCanvas.Background = canvasGrid;
        }
        #endregion


        #endregion


        #region Private Helper Methods
        /// <summary>
        /// Assigns object properties to the property grid
        /// </summary>
        /// <param name="property">Property holder</param>
        private void assingPropertyGrid(IHasProperties property)
        {
            _currentPropertyContainer = property;
            PropertyNameText.Text = property.Name;
            PropertiesDataGrid.ItemsSource = property.Properties;

            AddPropertyBtn.IsEnabled = true;
            RemovePropertyBtn.IsEnabled = true;
        }

        /// <summary>
        /// Enables or disables the toolbar buttons
        /// </summary>
        /// <param name="isEnabled">True to enable</param>
        private void enableTools(bool isEnabled)
        {
            SelectToolBtn.IsEnabled = isEnabled;
            PaintToolBtn.IsEnabled = isEnabled;
            TransformToolBtn.IsEnabled = isEnabled;
            EditToolBtn.IsEnabled = isEnabled;
        }
        #endregion


        #region Property Button Handlers
        /// <summary>
        /// Adds a property to the currently selected property container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPropertyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPropertyContainer != null)
            {
                _currentPropertyContainer.AddProperty("Key", "Value");
            }
        }

        /// <summary>
        /// Removes the selected property from the currently selected property container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemovePropertyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPropertyContainer != null)
            {
                if (PropertiesDataGrid.SelectedItem != null)
                {
                    _currentPropertyContainer.RemoveProperty((ItemProperty)PropertiesDataGrid.SelectedItem);
                }
            }
        }
        #endregion


        private void AddLayerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMap != null)
            {
                _currentMap.AddLayer();
            }
        }

        private void RemoveLayerBtn_Click(object sender, RoutedEventArgs e)
        {

        }



        private Image draggedImage;
        private Point mousePosition;
        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = e.Source as Image;

            if (image != null && LevelCanvas.CaptureMouse())
            {
                mousePosition = e.GetPosition(LevelCanvas);
                draggedImage = image;
                Panel.SetZIndex(draggedImage, 1); // in case of multiple images
            }
        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedImage != null)
            {
                LevelCanvas.ReleaseMouseCapture();
                Panel.SetZIndex(draggedImage, 0);
                draggedImage = null;
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (draggedImage != null)
            {
                Point position = e.GetPosition(LevelCanvas);
                Vector offset = position - mousePosition;
                mousePosition = position;
                Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
            }
        }
    }
}
