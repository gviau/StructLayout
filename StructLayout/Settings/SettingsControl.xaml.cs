﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace StructLayout
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SolutionSettings Options { set; get; }
        private SettingsWindow Win { set; get; }

        public SettingsControl(SettingsWindow window, SolutionSettings settings)
        {
            Win = window;
            Options = settings;
            InitializeComponent();
            CreateGrid();
        }
        private void CreateGrid()
        {
            PropertyInfo[] properties = typeof(SolutionSettings).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var elementGrid = new Grid();
                elementGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(170) });
                elementGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1,GridUnitType.Star) });

                var label = new Label();

                var customAttributes = (UIDescription[])property.GetCustomAttributes(typeof(UIDescription), true);
                if (customAttributes.Length > 0 && customAttributes[0] != null)
                {
                    var description = customAttributes[0];
                    label.Content = description.Label == null ? property.Name : description.Label;
                    label.ToolTip = description.Tooltip;
                }
                else
                {
                    label.Content = property.Name;
                }

                Grid.SetColumn(label, 0);
                elementGrid.Children.Add(label);

                if (property.PropertyType == typeof(string))
                {
                    var inputControl = new TextBox();
                    inputControl.VerticalAlignment = VerticalAlignment.Center;
                    inputControl.SetBinding(TextBox.TextProperty, new Binding("Options."+property.Name));
                    inputControl.Margin = new Thickness(5);
                    Grid.SetColumn(inputControl, 1);
                    elementGrid.Children.Add(inputControl);

                    //TODO ~ ramonv ~ add some sort of tooltip with resolved macros for compatible strings ( Add to UI description )
                }
                else if (property.PropertyType == typeof(bool))
                {
                    var inputControl = new CheckBox();
                    inputControl.VerticalAlignment = VerticalAlignment.Center;
                    inputControl.SetBinding(CheckBox.IsCheckedProperty,new Binding("Options." + property.Name));
                    inputControl.Margin = new Thickness(5);
                    Grid.SetColumn(inputControl, 1);
                    elementGrid.Children.Add(inputControl);
                }

                optionStack.Children.Add(elementGrid);
            }
        }

        private void ApplyChanges()
        {
            var manager = SettingsManager.Instance;
            manager.Settings = Options;
            manager.Save();
        }

        public void ButtonCancel_OnSave(object o, object e)
        {
            ApplyChanges();
            Win.Close();
        }
    }
}
