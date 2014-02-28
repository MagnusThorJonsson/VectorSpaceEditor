using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace VectorSpace.Behaviours
{
    public static class LayerItemSelectionBehaviour
    {
        #region Properties
        public static readonly DependencyProperty ClickSelectionProperty =
            DependencyProperty.RegisterAttached(
                "ClickSelection",
                typeof(bool),
                typeof(LayerItemSelectionBehaviour),
                new UIPropertyMetadata(false, OnClickSelectionChanged)
            );
        #endregion

        #region Click Event Handlers
        public static bool GetClickSelection(DependencyObject obj)
        {
            return (bool)obj.GetValue(ClickSelectionProperty);
        }

        public static void SetClickSelection(DependencyObject obj, bool value)
        {
            obj.SetValue(ClickSelectionProperty, value);
        }

        public static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListBox listBox = sender as ListBox;
                var valid = e.AddedItems[0];
                foreach (var item in new ArrayList(listBox.SelectedItems))
                {
                    if (item != valid)
                    {
                        listBox.SelectedItems.Remove(item);
                    }
                }
            }
        }

        private static void OnClickSelectionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ListBox listBox = dpo as ListBox;
            if (listBox != null)
            {
                if ((bool)e.NewValue == true)
                {
                    listBox.SelectionMode = SelectionMode.Multiple;
                    listBox.SelectionChanged += OnSelectionChanged;
                }
                else
                {
                    listBox.SelectionChanged -= OnSelectionChanged;
                }
            }
        }
        #endregion
    }
}
