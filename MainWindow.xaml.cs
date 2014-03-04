﻿using System;
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
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Media.Media3D;

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

                //TEST
                _currentMap.AddItem(
                    0,
                    TextureItem.Create(
                        0, 
                        _currentMap.TextureLibraries[0].Textures[0].Name + "_" + _currentMap.Layers[0].Items.Count, // TODO: Need a unique id generator for item names/ids
                        _currentMap.TextureLibraries[0].Textures[0], 
                        new WorldPosition(new System.Drawing.Point(50, 50), new System.Drawing.Point(), 1f, 1f, 0f), 
                        1
                    )
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
            _currentPropertyContainer = property;
            PropertyNameText.Text = property.Name;
            PropertiesDataGrid.ItemsSource = property.Properties;

            AddPropertyBtn.IsEnabled = true;
            RemovePropertyBtn.IsEnabled = true;
        }
        #endregion
        

        #region Layer Event Handlers
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
                    _currentMap.AddLayer(layerWindow.Layer);
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
                _currentMap.RemoveLayer((Layer)LayersListBox.SelectedItem);
            }
        }
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
            switch (EditState)
            {
                case ApplicationEditState.Select:
                    _currentlySelectedImage = e.OriginalSource as Image;
                    if (_currentlySelectedImage != null)
                    {
                        _lastMousePosition = e.GetPosition(LevelCanvas);
                    }
                    break;
            }
        }
        private Image _currentlySelectedImage = null;
        private Point _lastMousePosition = new Point();

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
                    _currentlySelectedImage = null;
                    _lastMousePosition = new Point();
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
            displayStatusTip(e.GetPosition(LevelCanvas));

            switch (EditState)
            {
                case ApplicationEditState.Select:
                    if (e.LeftButton == MouseButtonState.Pressed && _currentlySelectedImage != null)
                    {
                        Point mousePos = e.GetPosition(LevelCanvas);
                        TextureItem item = (TextureItem)_currentlySelectedImage.DataContext;
                        item.Move(
                            (int)(mousePos.X - _lastMousePosition.X), 
                            (int)(mousePos.Y - _lastMousePosition.Y)
                        );
                        _lastMousePosition = mousePos;
                    }
                    break;
            }
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
            if (e.Handled == false && e.Data.GetDataPresent("Texture"))
            {
                ItemsControl canvasControl = (ItemsControl)sender;
                Texture item = (Texture)((WeakReference)e.Data.GetData("Texture")).Target;

                if (canvasControl != null && item != null && e.AllowedEffects.HasFlag(DragDropEffects.Copy))
                {
                    Point mousePos = e.GetPosition(LevelCanvas);
                    _currentMap.AddItem(0,
                        TextureItem.Create(
                            0,
                            item.Name + "_" + _currentMap.Layers[0].Items.Count, // TODO: Need a unique id generator for item names/ids
                            item,
                            new WorldPosition(new System.Drawing.Point((int)mousePos.X - item.Origin.X, (int)mousePos.Y - item.Origin.Y), new System.Drawing.Point(), 1f, 1f, 0f),
                            1
                        )
                    );
                }
            }

            e.Handled = true;
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
            _selectedLibraryItem = ((ListBox)sender).SelectedIndex;
        }

        /// <summary>
        /// Updates the selected library 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LibraryTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedLibrary = ((TabControl)sender).SelectedIndex;
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
            if (image != null && e.LeftButton == MouseButtonState.Pressed)
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




        #region Might use later
        public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            List<T> logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }
        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }

        public static List<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            List<T> children = new List<T>();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    children.AddRange(FindChildren<T>(child));
                }
                else
                {
                    children.Add((T)child);
                }
            }

            return children;
        }
        #endregion



    }
}
