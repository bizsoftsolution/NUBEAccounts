﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NUBEAccounts.Pl.CTRL
{
    public class DataGridNumericColumn : DataGridTextColumn
    {
        protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
        {
            TextBox edit = editingElement as TextBox;
            edit.PreviewTextInput += OnPreviewTextInput;

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            try
            {

                var txt = e.OriginalSource as TextBox;

                if (e.Text.EndsWith("."))
                {

                    e.Handled = txt.Text.Contains(".");
                }
                else
                {
                    Convert.ToDecimal(e.Text);
                }

            }
            catch
            {
                // Show some kind of error message if you want

                // Set handled to true

                e.Handled = true;
            }
        }
    }
}
