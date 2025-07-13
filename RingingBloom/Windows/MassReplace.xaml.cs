﻿using Microsoft.Win32;
using RingingBloom.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RingingBloom.Windows
{
    /// <summary>
    /// Interaction logic for PCKMassReplace.xaml
    /// </summary>
    public class ReplacingWem
    {
        public Wem wem { get; set; }
        public ulong replacingId { get; set; }

        public ReplacingWem(Wem newWem)
        {
            wem = newWem;
            replacingId = 0;
        }
        public ReplacingWem(Wem newWem, ulong iD)
        {
            wem = newWem;
            replacingId = iD;
        }
    }
    public class ReplacingWems
    {
        public ObservableCollection<ReplacingWem> wems { get; set; }
        public ObservableCollection<ulong> wemIds { get; set; }

        public ReplacingWems(List<ulong> ids)
        {
            wems = new ObservableCollection<ReplacingWem>();
            wemIds = new ObservableCollection<ulong>(ids);
        }
    }
    public partial class MassReplace : Window
    {
        public ReplacingWems holder;
        string ImportPath;
        public MassReplace(List<ulong> wemIdList, string import)
        {
            InitializeComponent();
            wemIdList.Insert(0, 0);
            holder = new ReplacingWems(wemIdList);
            if(import != null)
            {
                ImportPath = import;
            }
            WemGrab.DataContext = holder;
        }

        private void ImportWems(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (ImportPath != null)
            {
                openFile.InitialDirectory = ImportPath;
            }
            openFile.Multiselect = true;
            openFile.Filter = "WWise Wem files (*.wem)|*.wem";
            if (openFile.ShowDialog() == true)
            {
                foreach (string fileName in openFile.FileNames)
                {
                    Wem newWem = HelperFunctions.MakeWems(fileName, HelperFunctions.OpenFile(fileName));
                    //if less wems have been inserted than total wems in the bank
                    if (holder.wemIds.Count > holder.wems.Count+1)
                    {
                        //create the new wem with the wemID corresponding to its position
                        holder.wems.Add(new ReplacingWem(newWem, holder.wemIds[WemGrab.Items.Count+1]));
                    }
                    else {
                        //otherwise initialize at 0
                        holder.wems.Add(new ReplacingWem(newWem));
                    }
                    //WemGrab.Items.Add(new ReplacingWem(newWem,wemIds));
                }

            }
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
