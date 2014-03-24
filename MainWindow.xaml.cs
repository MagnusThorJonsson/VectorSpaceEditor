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
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Media.Media3D;
using VectorSpace.Adorners;
using System.Collections.ObjectModel;

namespace VectorSpace
{
    public enum ApplicationEditState
    { 
        Select,
        Create,
        Transform,
        Edit // ?
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Variables & Properties
        public Map Map { get { return _currentMap; } }
        private Map _currentMap;

        public static ApplicationEditState EditState { get { return _currentEditState; } }
        private static ApplicationEditState _currentEditState;

        private IHasProperties _currentPropertyContainer;
        
        private VisualBrush canvasGrid;
        private bool _isGridShowing;

        private int _selectedLibrary;
        private int _selectedLibraryItem;
        private int _selectedLayerIndex;

        private Image _currentlySelectedImage;
        private Point _lastMousePosition;
        
        private bool _isContextOpen = false;
        #endregion


        #region Constructor & Initialization
        /// <summary>
        /// Application Main View Constructor
        /// </summary>
        public MainWindow()
        {
            _currentEditState = ApplicationEditState.Select;
            _currentMap = null;
            _isGridShowing = false;
            _selectedLibrary = 0;
            _selectedLibraryItem = 0;
            _selectedLayerIndex = 0;

            _currentlySelectedImage = null;
            _lastMousePosition = new Point();

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

                // Set the application edit state to select mode, enable tools and toggle the Select tool as in use
                _currentEditState = ApplicationEditState.Select;
                enableTools(true);
                toggleToolButton(SelectToolBtn);
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


        #region Toolbar Button Methods

        #region Shortcut Key Commands
        /// <summary>
        /// Select Tool Shortcut Command (Ctrl + V)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExecutedSelectToolShortcutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _currentEditState = ApplicationEditState.Select;
            toggleToolButton(SelectToolBtn);
        }
        public static RoutedCommand SelectToolShortcutCommand = new RoutedCommand();

        /// <summary>
        /// Paint Tool Shortcut Command (Ctrl + P)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExecutedPaintToolShortcutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _currentEditState = ApplicationEditState.Create;
            toggleToolButton(PaintToolBtn);
        }
        public static RoutedCommand PaintToolShortcutCommand = new RoutedCommand();

        /// <summary>
        /// Transform Tool Shortcut Command (Ctrl + V)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExecutedTransformToolShortcutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _currentEditState = ApplicationEditState.Transform;
            toggleToolButton(TransformToolBtn);
        }
        public static RoutedCommand TransformToolShortcutCommand = new RoutedCommand();

        /// <summary>
        /// Edit Tool Shortcut Command (Ctrl + E)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExecutedEditToolShortcutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _currentEditState = ApplicationEditState.Edit;
            toggleToolButton(EditToolBtn);
        }
        public static RoutedCommand EditToolShortcutCommand = new RoutedCommand();


        public void CanExecuteToolShortcutCommands(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_currentMap != null)
            {
                e.CanExecute = true;
            }
            else
                e.CanExecute = false;
        }
        #endregion

        #region Toolbar Event Handlers
        private void SelectToolBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentEditState = ApplicationEditState.Select;
            toggleToolButton((ToggleButton)sender);
        }

        private void PaintToolBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentEditState = ApplicationEditState.Create;
            toggleToolButton((ToggleButton)sender);
        }

        private void TransformToolBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentEditState = ApplicationEditState.Transform;
            toggleToolButton((ToggleButton)sender);
        }

        private void EditToolBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentEditState = ApplicationEditState.Edit;
            toggleToolButton((ToggleButton)sender);
        }
        #endregion

        #region Toolbar Private Helper Methods
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

        /// <summary>
        /// Clears the tool buttons
        /// </summary>
        /// <param name="enabledButton">The active tool button (null for no button selected)</param>
        private void toggleToolButton(ToggleButton enabledButton = null)
        {
            SelectToolBtn.IsChecked = false;
            PaintToolBtn.IsChecked = false;
            TransformToolBtn.IsChecked = false;
            EditToolBtn.IsChecked = false;

            if (enabledButton != null)
                enabledButton.IsChecked = true;
        }
        #endregion

        #endregion


        #region Property Grid & Button Handlers
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

        /// <summary>
        /// Assigns object properties to the property grid
        /// </summary>
        /// <param name="property">Property holder</param>
        private void assingPropertyGrid(IHasProperties property)
        {
            if (property == null)
            {
                _currentPropertyContainer = null;
                PropertyNameText.Text = "N/A";
                PropertiesDataGrid.ItemsSource = null;
                AddPropertyBtn.IsEnabled = false;
                RemovePropertyBtn.IsEnabled = false;
            }
            else
            {
                _currentPropertyContainer = property;
                PropertyNameText.Text = property.Name;
                PropertiesDataGrid.ItemsSource = property.Properties;
                AddPropertyBtn.IsEnabled = true;
                RemovePropertyBtn.IsEnabled = true;
            }
        }
        #endregion


        #region Layer List Handlers 

        #region Layer List Event Handlers
        /// <summary>
        /// Updates the selected layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedLayerIndex = ((ListBox)sender).SelectedIndex;

            // Remove adorners
            removeAdorner(_currentlySelectedImage);
        }

        /// <summary>
        /// Updates the selected layer item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerItemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem lbItem = ((ListBox)sender).SelectedItem as ListBoxItem;
            if (lbItem != null)
            {
                TextureItem item = lbItem.DataContext as TextureItem;
                if (item != null)
                {
                }
            }

            // Remove adorners
            removeAdorner(_currentlySelectedImage);
        }
        #endregion

        #region Layer Button Event Handlers
        /// <summary>
        /// Add layer button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddLayerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMap != null)
            {
                NewLayerWindow layerWindow = new NewLayerWindow(_currentMap.NextLayerId);
                layerWindow.Owner = this;
                layerWindow.ShowDialog();

                if (layerWindow.DialogResult.HasValue && layerWindow.DialogResult.Value == true)
                {
                    // Remove adorners
                    removeAdorner(_currentlySelectedImage);

                    // Add layer and auto select
                    _currentMap.AddLayer(layerWindow.Layer);
                    _selectedLayerIndex = LayersListBox.Items.Count - 1;
                    LayersListBox.SelectedIndex = _selectedLayerIndex;
                    _currentlySelectedImage = null;
                }
            }
        }

        /// <summary>
        /// Remove layer button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveLayerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMap != null)
            {
                // Remove adorners
                removeAdorner(_currentlySelectedImage);

                int currentIndex = _selectedLayerIndex;
                if (_currentMap.RemoveLayer((Layer)LayersListBox.SelectedItem))
                {
                    // If we manage to remove the layer we select the layer below
                    if (currentIndex > 0)
                        currentIndex--;
                    else if (currentIndex < 0)
                        currentIndex = 0;

                    _selectedLayerIndex = currentIndex;
                    LayersListBox.SelectedIndex = currentIndex;
                    _currentlySelectedImage = null;
                }
            }
        }
        #endregion

        #endregion


        #region Library & Canvas Methods

        #region Canvas Drag Helpers
        /// <summary>
        /// Handles the Left Mouse button click on the Canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // We don't do anything if the context menu is open
            if (_isContextOpen)
                return;

            switch (EditState)
            {
                case ApplicationEditState.Select:
                    // Remove adorner from the cached item
                    if (_currentlySelectedImage != null)
                    {
                        removeAdorner(_currentlySelectedImage);
                        TextureItem item = _currentlySelectedImage.DataContext as TextureItem;
                        if (item != null)
                            item.IsSelected = false;
                    }

                    // Add an adorner if an image was clicked
                    _currentlySelectedImage = e.OriginalSource as Image;
                    if (_currentlySelectedImage != null)
                    {
                        selectMapTextureItem(_currentlySelectedImage);
                    }
                    else
                        assingPropertyGrid(_currentMap);
                    break;
            }
        }
        
        /// <summary>
        /// Handles the Left Mouse button release on the Canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (EditState)
            {
                case ApplicationEditState.Select:
                    //_lastMousePosition = new Point();
                    break;
            }
        }

        /// <summary>
        /// Handles the Canvas Mouse Move event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // We don't do anything if the context menu is open
            if (_isContextOpen)
                return;

            Point mousePos = e.GetPosition(LevelCanvas);
            displayStatusTip(mousePos);

            switch (EditState)
            {
                case ApplicationEditState.Select:
                    if (e.LeftButton == MouseButtonState.Pressed && _currentlySelectedImage != null)
                    {

                        // If item is on the currently selected layer we allow the move
                        Layer layer = LayersListBox.Items[_selectedLayerIndex] as Layer;
                        TextureItem item = (TextureItem)_currentlySelectedImage.DataContext;
                        if (item != null && layer != null && item.Layer == layer.Id)
                        {
                            item.Move(
                                (int)(mousePos.X - _lastMousePosition.X),
                                (int)(mousePos.Y - _lastMousePosition.Y)
                            );
                        }
                    }
                    break;
            }

            _lastMousePosition = mousePos;
        }

        #endregion

        #region Canvas Library Drag & Drop Helpers
        /// <summary>
        /// Handles DragOver event for the canvas library item drag & drop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelCanvas_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Texture"))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// Handles the library item drag & drop on to the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Handled == false && e.Data.GetDataPresent("Texture") && _currentMap != null)
            {
                ItemsControl canvasControl = (ItemsControl)sender;
                Layer layer = LayersListBox.Items[_selectedLayerIndex] as Layer;
                Texture item = (Texture)((WeakReference)e.Data.GetData("Texture")).Target;

                if (canvasControl != null && item != null && layer != null &&
                    _selectedLayerIndex >= 0 && _selectedLayerIndex < _currentMap.Layers.Count && 
                    e.AllowedEffects.HasFlag(DragDropEffects.Copy))
                {
                    Point mousePos = e.GetPosition(LevelCanvas);
                    _currentMap.CreateItem(
                        layer.Id,
                        item.Name + "_" + _currentMap.Layers[0].Items.Count, // TODO: Need a unique id generator for item names/ids
                        item,
                        new WorldPosition(new System.Drawing.Point((int)mousePos.X - item.Origin.X, (int)mousePos.Y - item.Origin.Y), new System.Drawing.Point(), 1f, 1f, 0f)
                    );
                }
            }

            e.Handled = true;
        }
        #endregion

        #region Canvas Selection Helpers
        /// <summary>
        /// Handles the selection of map texture items
        /// </summary>
        /// <param name="image">The texture item being selected</param>
        private void selectMapTextureItem(Image image)
        {
            // If the item we're trying to select isn't on the same layer we don't allow selection
            TextureItem selectedItem = image.DataContext as TextureItem;
            Layer layer = LayersListBox.Items[_selectedLayerIndex] as Layer;
            if (selectedItem != null && layer != null && selectedItem.Layer == layer.Id)
            {
                selectedItem.IsSelected = true;
                assingPropertyGrid((IHasProperties)image.DataContext);
                AdornerLayer.GetAdornerLayer(image).Add(new MapItemSelectionAdorner(image, LevelCanvas));
            }
            else
                assingPropertyGrid(_currentMap);
        }

        /// <summary>
        /// Removes an adorner from an element
        /// </summary>
        /// <param name="uiElement">The element to remove from</param>
        private void removeAdorner(UIElement uiElement)
        {
            if (uiElement == null)
                return;

            AdornerLayer adLayer = AdornerLayer.GetAdornerLayer(uiElement);
            if (adLayer != null)
            {
                Adorner[] itemAdorners = adLayer.GetAdorners(uiElement);
                if (itemAdorners != null)
                {
                    // Shitty hack, we only remove the first one (only using one atm anyways)
                    AdornerLayer.GetAdornerLayer(uiElement).Remove(itemAdorners[0]);
                }
            }
        }
        #endregion

        #region Library Selection Events
        /// <summary>
        /// Updates the selected library item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LibraryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentMap != null && _selectedLibrary >= 0)
            {
                _selectedLibraryItem = ((ListBox)sender).SelectedIndex;

                if (_selectedLibraryItem < _currentMap.TextureLibraries[_selectedLibrary].Textures.Count)
                    assingPropertyGrid(_currentMap.TextureLibraries[_selectedLibrary].Textures[_selectedLibraryItem]);
            }
            else
                assingPropertyGrid(null);
        }

        /// <summary>
        /// Updates the selected library 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LibraryTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedLibrary = ((TabControl)sender).SelectedIndex;

            assingPropertyGrid(null);
        }
        #endregion

        #region Library Events
        /// <summary>
        /// Initiates a library item drag and drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LibraryItem_MouseMove(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;
            if (_currentMap.Layers.Count > 0 && image != null && e.LeftButton == MouseButtonState.Pressed)
            {
                // Package the data.
                DataObject data = new DataObject(
                    "Texture",
                    new WeakReference(_currentMap.TextureLibraries[_selectedLibrary].Textures[_selectedLibraryItem])
                );

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Copy);
            }
        }

        /// <summary>
        /// Changes canvas feedback cursor icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LibraryItem_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // These Effects values are set in the drop target's 
            // DragOver event handler. 
            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                Mouse.SetCursor(Cursors.Cross);
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Pen);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }
            e.Handled = true;
        }
        #endregion 

        #endregion


        #region Canvas Item Context Menu Handlers

        #region Item Order Handlers
        /// <summary>
        /// Moves the Canvas Item to the top of the layer it is on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasItemContext_BringToFront(object sender, RoutedEventArgs e)
        {
            if (_currentlySelectedImage != null)
            {
                Layer layer = LayersListBox.Items[_selectedLayerIndex] as Layer;
                TextureItem item = (TextureItem)_currentlySelectedImage.DataContext;
                if (item != null && layer != null && item.Layer == layer.Id)
                    _currentMap.BringToFront(item);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Moves the Canvas Item up by one item on the layer it is on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasItemContext_BringForward(object sender, RoutedEventArgs e)
        {
            if (_currentlySelectedImage != null)
            {
                Layer layer = LayersListBox.Items[_selectedLayerIndex] as Layer;
                TextureItem item = (TextureItem)_currentlySelectedImage.DataContext;
                if (item != null && layer != null && item.Layer == layer.Id)
                {
                    _currentMap.IncrementItemZ(item);
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Moves the Canvas Item to the bottom of the layer it is on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasItemContext_SendToBack(object sender, RoutedEventArgs e)
        {
            if (_currentlySelectedImage != null)
            {
                Layer layer = LayersListBox.Items[_selectedLayerIndex] as Layer;
                TextureItem item = (TextureItem)_currentlySelectedImage.DataContext;
                if (item != null && layer != null && item.Layer == layer.Id)
                    _currentMap.SendToBack(item);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Moves the Canvas Item down by one item on the layer it is on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasItemContext_SendBackward(object sender, RoutedEventArgs e)
        {
            if (_currentlySelectedImage != null)
            {
                Layer layer = LayersListBox.Items[_selectedLayerIndex] as Layer;
                TextureItem item = (TextureItem)_currentlySelectedImage.DataContext;
                if (item != null && layer != null && item.Layer == layer.Id)
                {
                    _currentMap.DecrementItemZ(item);
                }
            }

            e.Handled = true;
        }
        #endregion

        #region Context Menu Open & Close Event Handlers
        /// <summary>
        /// Handles the context menu closing event for canvas items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasItem_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            _isContextOpen = false;
        }

        /// <summary>
        /// Handles the event just before 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            _isContextOpen = true;
            switch (EditState)
            {
                case ApplicationEditState.Select:
                    if (_currentlySelectedImage != null)
                    {
                        removeAdorner(_currentlySelectedImage);
                        // Cancel the context menu if we're not on the same layer
                        Layer layer = LayersListBox.Items[_selectedLayerIndex] as Layer;
                        TextureItem item = (TextureItem)_currentlySelectedImage.DataContext;
                        if (item != null && layer != null && item.Layer != layer.Id)
                        {
                            e.Handled = true;
                            return;
                        }
                    }

                    // Add an adorner if an image was clicked
                    _currentlySelectedImage = e.OriginalSource as Image;
                    if (_currentlySelectedImage != null)
                    {
                        selectMapTextureItem(_currentlySelectedImage);
                    }
                    else
                    {
                        // Cancel the context menu if no image was selected
                        assingPropertyGrid(_currentMap);
                        e.Handled = true;
                    }
                    break;
            }
        }
        #endregion

        #endregion


        #region Status Bar Helpers
        /// <summary>
        /// Displays the mouse position in the status bar
        /// </summary>
        /// <param name="mouse">The mouse position</param>
        private void displayStatusTip(Point mouse)
        {
            StatusBarText.Text = "Canvas Mouse Position: (" + (int)mouse.X + ", " + (int)mouse.Y + ")";
        }

        /// <summary>
        /// Displays a message in the status bar
        /// </summary>
        /// <param name="tip">The message tip to display</param>
        private void displayStatusTip(String tip)
        {
            StatusBarText.Text = tip;
        }
        #endregion

    }
}
