﻿using Microsoft.Win32;
using RingingBloom.Common;
using RingingBloom.NBNK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    /// Interaction logic for BNKEditor.xaml
    /// </summary>
    public partial class BNKEditor : Window
    {
        SupportedGames mode = SupportedGames.MHWorld;
        NBNKFile nbnk = null;
        private string ImportPath = null;
        private string ExportPath = null;
        private bool LabelsChanged = false;
        private List<ulong> changedIds = new List<ulong>();
        public BNKEditor(SupportedGames Mode,Options options)
        {
            InitializeComponent();
            mode = Mode;
            if(options.defaultImport != null)
            {
                ImportPath = options.defaultImport;
            }
            if (options.defaultImport != null)
            {
                ExportPath = options.defaultExport;
            }
        }

        public void ImportBNK(object sender, RoutedEventArgs e)
        {
            SaveLabels(sender, new CancelEventArgs());
            LabelsChanged = false;
            OpenFileDialog importFile = new OpenFileDialog();
            if(ImportPath != null)
            {
                importFile.InitialDirectory = ImportPath;
            }
            importFile.Multiselect = false;
            importFile.Filter = "WWise Soundbank file (*.bnk)|*.bnk";
            switch (mode)
            {
                case SupportedGames.MHWorld:
                    importFile.Filter += "|Monster Hunter World WWise Soundbank (*.nbnk)|*.nbnk";
                    importFile.Filter = "All supported files (*.bnk,*.nbnk)|*.bnk;*.nbnk|" + importFile.Filter;
                    break;
                case SupportedGames.MHRise:
                    importFile.Filter += "|Monster Hunter Rise Switch WWise Soundbank (*.x64)|*.bnk.2.x64";
                    importFile.Filter += "|Monster Hunter Rise English WWise Soundbank (*.En)|*.bnk.2.x64.En";
                    importFile.Filter += "|Monster Hunter Rise Japanese WWise Soundbank (*.Ja)|*.bnk.2.x64.Ja";
                    importFile.Filter += "|Monster Hunter Rise Fictional WWise Soundbank (*.Fc)|*.bnk.2.x64.Fc";
                    importFile.Filter = "All supported files (*.bnk,*.x64,*.En,*.Ja,*.Fc)|*.bnk;*.bnk.2.x64;*.bnk.2.x64.En;*.bnk.2.x64.Ja;*.bnk.2.x64.Fc|" + importFile.Filter;
                    break;
                case SupportedGames.MHRiseSwitch:
                    importFile.Filter += "|Monster Hunter Rise Switch WWise Soundbank (*.nsw)|*.bnk.2.nsw";
                    importFile.Filter += "|Monster Hunter Rise English WWise Soundbank (*.En)|*.bnk.2.nsw.En";
                    importFile.Filter += "|Monster Hunter Rise Japanese WWise Soundbank (*.Ja)|*.bnk.2.nsw.Ja";
                    importFile.Filter += "|Monster Hunter Rise Fictional WWise Soundbank (*.Fc)|*.bnk.2.nsw.Fc";
                    importFile.Filter = "All supported files (*.bnk,*.nsw,*.En,*.Ja,*.Fc)|*.bnk;*.bnk.2.nsw;*.bnk.2.nsw.En;*.bnk.2.nsw.Ja;*.bnk.2.nsw.Fc|" + importFile.Filter;
                    break;
                case SupportedGames.RE2DMC5:
                    importFile.Filter += "|RE Engine WWise Soundbank (*.x64)|*.bnk.2.x64";
                    importFile.Filter += "|RE Engine English WWise Soundbank (*.En)|*.bnk.2.x64.En";
                    importFile.Filter += "|RE Engine Japanese WWise Soundbank (*.Ja)|*.bnk.2.x64.Ja";
                    importFile.Filter = "All supported files (*.bnk,*.x64,*.En,*.Ja)|*.bnk;*.bnk.2.x64;*.bnk.2.x64.En;*.bnk.2.x64.Ja|" + importFile.Filter;
                    break;
                case SupportedGames.RE3R:
                    importFile.Filter += "|RE Engine WWise Soundbank (*.stm)|*.bnk.2.stm";
                    importFile.Filter += "|RE Engine German WWise Soundbank (*.De)|*.bnk.2.stm.De";
                    importFile.Filter += "|RE Engine English WWise Soundbank (*.En)|*.bnk.2.stm.En";
                    importFile.Filter += "|RE Engine Spanish WWise Soundbank (*.Es)|*.bnk.2.stm.Es";
                    importFile.Filter += "|RE Engine French WWise Soundbank (*.Fr)|*.bnk.2.stm.Fr";
                    importFile.Filter += "|RE Engine Italian WWise Soundbank (*.It)|*.bnk.2.stm.It";
                    importFile.Filter += "|RE Engine Japanese WWise Soundbank (*.Ja)|*.bnk.2.stm.Ja";
                    importFile.Filter += "|RE Engine Chinese WWise Soundbank (*.ZhCN)|*.bnk.2.stm.ZhCN";
                    importFile.Filter = "All supported files (*.bnk,*.stm,*.En,*.Ja,...)|*.bnk;*.bnk.2.stm;*.bnk.2.stm.De;*.bnk.2.stm.En;*.bnk.2.stm.Es;*.bnk.2.stm.Fr;*.bnk.2.stm.It;*.bnk.2.stm.Ja;*.bnk.2.stm.ZhCN|" + importFile.Filter;
                    break;
                case SupportedGames.RE8:
                    importFile.Filter += "|RE Engine WWise Soundbank (*.x64)|*.bnk.2.x64";
                    importFile.Filter += "|RE Engine WWise Soundbank (*.stm)|*.bnk.2.stm";
                    importFile.Filter += "|RE Engine German WWise Soundbank (*.De)|*.bnk.2.x64.De;*.bnk.2.stm.De";
                    importFile.Filter += "|RE Engine English WWise Soundbank (*.En)|*.bnk.2.x64.En;*.bnk.2.stm.En";
                    importFile.Filter += "|RE Engine Spanish WWise Soundbank (*.Es)|*.bnk.2.x64.Es;*.bnk.2.stm.Es";
                    importFile.Filter += "|RE Engine French WWise Soundbank (*.Fr)|*.bnk.2.x64.Fr;*.bnk.2.stm.Fr";
                    importFile.Filter += "|RE Engine Italian WWise Soundbank (*.It)|*.bnk.2.x64.It;*.bnk.2.stm.It";
                    importFile.Filter += "|RE Engine Japanese WWise Soundbank (*.Ja)|*.bnk.2.x64.Ja;*.bnk.2.stm.Ja";
                    importFile.Filter += "|RE Engine Russian WWise Soundbank (*.Ru)|*.bnk.2.x64.Ru;*.bnk.2.stm.Ru";
                    importFile.Filter += "|RE Engine Chinese WWise Soundbank (*.ZhCN)|*.bnk.2.x64.ZhCN;*.bnk.2.stm.ZhCN";
                    importFile.Filter = "All supported files (*.pck,*.x64,*.stm,*.En,...)|*.pck;*.bnk.2.x64;*.bnk.2.stm;*.bnk.2.x64.De;*.bnk.2.stm.De;*.bnk.2.x64.En;*.bnk.2.stm.En;*.bnk.2.x64.Es;*.bnk.2.stm.Es;*.bnk.2.x64.Fr;*.bnk.2.stm.Fr;*.bnk.2.x64.It;*.bnk.2.stm.It;*.bnk.2.x64.Ja;*.bnk.2.stm.Ja;*.bnk.2.x64.Ru;*.bnk.2.stm.Ru;*.bnk.2.x64.ZhCN;*.bnk.2.stm.ZhCN|" + importFile.Filter; 
                    break;
                default:
                    break;
            }
            if (importFile.ShowDialog() == true)
            {
                BinaryReader readFile = new BinaryReader(new FileStream(importFile.FileName, FileMode.Open), Encoding.ASCII);
                nbnk = new NBNKFile(readFile, mode);
                readFile.Close();
                PopulateTreeView(false);
            }
        }

        private void AutomaticReplace(object sender, RoutedEventArgs e)
        {
            if (nbnk == null || nbnk.DataIndex == null || nbnk.DataIndex.wemList.Count == 0)
            {
                MessageBox.Show("0 Bnk Files Loaded");
                return;
            }

            var dialog = new OpenFileDialog();
            if (ImportPath != null)
            {
                dialog.InitialDirectory = ImportPath;
            }
            dialog.CheckFileExists = false;
            dialog.FileName = "The folder requires the .wem files to have the same names as the IDs";

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            string folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);

            List<ulong> replacedIds = new List<ulong>();
            int totalReplaced = 0;

            for (int i = 0; i < nbnk.DataIndex.wemList.Count; i++)
            {
                ulong wemId = nbnk.DataIndex.wemList[i].Id;

                // To verify a corresponding ID.
                string filePath = System.IO.Path.Combine(folderPath, wemId.ToString() + ".wem");

                if (File.Exists(filePath))
                {
                    try
                    {
                        ulong newId = nbnk.DataIndex.wemList[i].Id;
                        Wem newWem = new Wem(System.IO.Path.GetFileName(filePath), newId.ToString(), new BinaryReader(File.Open(filePath, FileMode.Open)));
                        nbnk.DataIndex.wemList[i] = newWem;

                        replacedIds.Add(wemId);
                        totalReplaced++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error replacing {wemId}.wem: {ex.Message}");
                    }
                }
            }

            PopulateTreeView(true);

            if (totalReplaced > 0)
            {
                string IDreplaced = string.Join(", ", replacedIds);
                MessageBox.Show($"{totalReplaced} Wem files replaced\n\nReplaced ID's: {IDreplaced}",
                    "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No .wem files were replaced. Check if the filenames match the .wem IDs in the BNK.");
            }
        }

        public void ExportBNK(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            if (ExportPath != null)
            {
                saveFile.InitialDirectory = ExportPath;
            }
            saveFile.Filter = "WWise Soundbank file (*.bnk)|*.bnk";
            switch (mode)
            {
                case SupportedGames.MHWorld:
                    saveFile.Filter += "|Monster Hunter World WWise Soundbank (*.nbnk)|*.nbnk";
                    saveFile.Filter = "All supported files (*.bnk,*.nbnk)|*.bnk;*.nbnk|" + saveFile.Filter;
                    break;
                case SupportedGames.MHRise:
                    saveFile.Filter += "|Monster Hunter Rise Switch WWise Soundbank (*.x64)|*.bnk.2.x64";
                    saveFile.Filter += "|Monster Hunter Rise English WWise Soundbank (*.En)|*.bnk.2.x64.En";
                    saveFile.Filter += "|Monster Hunter Rise Japanese WWise Soundbank (*.Ja)|*.bnk.2.x64.Ja";
                    saveFile.Filter += "|Monster Hunter Rise Fictional WWise Soundbank (*.Fc)|*.bnk.2.x64.Fc";
                    saveFile.Filter = "All supported files (*.bnk,*.x64,*.En,*.Ja,*.Fc)|*.bnk;*.bnk.2.x64;*.bnk.2.x64.En;*.bnk.2.x64.Ja;*.bnk.2.x64.Fc|" + saveFile.Filter;
                    break;
                case SupportedGames.MHRiseSwitch:
                    saveFile.Filter += "|Monster Hunter Rise Switch WWise Soundbank (*.nsw)|*.bnk.2.nsw";
                    saveFile.Filter += "|Monster Hunter Rise English WWise Soundbank (*.En)|*.bnk.2.nsw.En";
                    saveFile.Filter += "|Monster Hunter Rise Japanese WWise Soundbank (*.Ja)|*.bnk.2.nsw.Ja";
                    saveFile.Filter += "|Monster Hunter Rise Fictional WWise Soundbank (*.Fc)|*.bnk.2.nsw.Fc";
                    saveFile.Filter = "All supported files (*.bnk,*.nsw,*.En,*.Ja,*.Fc)|*.bnk;*.bnk.2.nsw;*.bnk.2.nsw.En;*.bnk.2.nsw.Ja;*.bnk.2.nsw.Fc|" + saveFile.Filter;
                    break;
                case SupportedGames.RE2DMC5:
                    saveFile.Filter += "|RE Engine WWise Soundbank (*.x64)|*.bnk.2.x64";
                    saveFile.Filter += "|RE Engine English WWise Soundbank (*.En)|*.bnk.2.x64.En";
                    saveFile.Filter += "|RE Engine Japanese WWise Soundbank (*.Ja)|*.bnk.2.x64.Ja";
                    saveFile.Filter = "All supported files (*.bnk,*.x64,*.En,*.Ja)|*.bnk;*.bnk.2.x64;*.bnk.2.x64.En;*.bnk.2.x64.Ja|" + saveFile.Filter;
                    break;
                case SupportedGames.RE3R:
                    saveFile.Filter += "|RE Engine WWise Soundbank (*.stm)|*.bnk.2.stm";
                    saveFile.Filter += "|RE Engine German WWise Soundbank (*.De)|*.bnk.2.stm.De";
                    saveFile.Filter += "|RE Engine English WWise Soundbank (*.En)|*.bnk.2.stm.En";
                    saveFile.Filter += "|RE Engine Spanish WWise Soundbank (*.Es)|*.bnk.2.stm.Es";
                    saveFile.Filter += "|RE Engine French WWise Soundbank (*.Fr)|*.bnk.2.stm.Fr";
                    saveFile.Filter += "|RE Engine Italian WWise Soundbank (*.It)|*.bnk.2.stm.It";
                    saveFile.Filter += "|RE Engine Japanese WWise Soundbank (*.Ja)|*.bnk.2.stm.Ja";
                    saveFile.Filter += "|RE Engine Chinese WWise Soundbank (*.ZhCN)|*.bnk.2.stm.ZhCN";
                    saveFile.Filter = "All supported files (*.bnk,*.stm,*.En,*.Ja,...)|*.bnk;*.bnk.2.stm;*.bnk.2.stm.De;*.bnk.2.stm.En;*.bnk.2.stm.Es;*.bnk.2.stm.Fr;*.bnk.2.stm.It;*.bnk.2.stm.Ja;*.bnk.2.stm.ZhCN|" + saveFile.Filter;
                    break;
                case SupportedGames.RE8:
                    saveFile.Filter += "|RE Engine WWise Soundbank (*.x64)|*.bnk.2.x64";
                    saveFile.Filter += "|RE Engine WWise Soundbank (*.stm)|*.bnk.2.stm";
                    saveFile.Filter += "|RE Engine German WWise Soundbank (*.De)|*.bnk.2.x64.De;*.bnk.2.stm.De";
                    saveFile.Filter += "|RE Engine English WWise Soundbank (*.En)|*.bnk.2.x64.En;*.bnk.2.stm.En";
                    saveFile.Filter += "|RE Engine Spanish WWise Soundbank (*.Es)|*.bnk.2.x64.Es;*.bnk.2.stm.Es";
                    saveFile.Filter += "|RE Engine French WWise Soundbank (*.Fr)|*.bnk.2.x64.Fr;*.bnk.2.stm.Fr";
                    saveFile.Filter += "|RE Engine Italian WWise Soundbank (*.It)|*.bnk.2.x64.It;*.bnk.2.stm.It";
                    saveFile.Filter += "|RE Engine Japanese WWise Soundbank (*.Ja)|*.bnk.2.x64.Ja;*.bnk.2.stm.Ja";
                    saveFile.Filter += "|RE Engine Russian WWise Soundbank (*.Ru)|*.bnk.2.x64.Ru;*.bnk.2.stm.Ru";
                    saveFile.Filter += "|RE Engine Chinese WWise Soundbank (*.ZhCN)|*.bnk.2.x64.ZhCN;*.bnk.2.stm.ZhCN";
                    saveFile.Filter = "All supported files (*.bnk,*.x64,*.stm,*.En,...)|*.bnk;*.bnk.2.x64;*.bnk.2.stm;*.bnk.2.x64.De;*.bnk.2.stm.De;*.bnk.2.x64.En;*.bnk.2.stm.En;*.bnk.2.x64.Es;*.bnk.2.stm.Es;*.bnk.2.x64.Fr;*.bnk.2.stm.Fr;*.bnk.2.x64.It;*.bnk.2.stm.It;*.bnk.2.x64.Ja;*.bnk.2.stm.Ja;*.bnk.2.x64.Ru;*.bnk.2.stm.Ru;*.bnk.2.x64.ZhCN;*.bnk.2.stm.ZhCN|" + saveFile.Filter;
                    break;
                default:
                    break;
            }
            if (saveFile.ShowDialog() == true)
            {
                nbnk.ExportNBNK(new BinaryWriter(new FileStream(saveFile.FileName,FileMode.OpenOrCreate)));
            }
        }
        
        private void PopulateTreeView(bool isDidxExpand)
        {
            treeView1.Items.Clear();
            if (nbnk.BankHeader != null)
            {
                //should always be true
                TreeViewItem bkhd = new TreeViewItem();
                bkhd.Header = "Bank Header";
                bkhd.Foreground = HelperFunctions.GetBrushFromHex("#AAAAAA");
                List<string> tag = new List<string>();
                tag.Add("BKHD");
                bkhd.Tag = tag;
                treeView1.Items.Add(bkhd);
            }
            if (nbnk.DataIndex != null)
            {
                TreeViewItem didx = new TreeViewItem();
                didx.Header = "Data Index";
                didx.Foreground = HelperFunctions.GetBrushFromHex("#AAAAAA");
                List<string> tag = new List<string>();
                tag.Add("DIDX");
                tag.Add("-1");
                didx.Tag = tag;
                //ContextMenu contextMenu = FindResource("DIDXRightClick") as ContextMenu;
                //didx.ContextMenu = contextMenu;
                didx.IsExpanded = isDidxExpand;
                for (int i = 0; i < nbnk.DataIndex.wemList.Count; i++)
                {
                    TreeViewItem wem = new TreeViewItem();
                    wem.Header = nbnk.DataIndex.wemList[i].Name;
                    wem.Foreground = HelperFunctions.GetBrushFromHex("#AAAAAA");
                    List<string> wemTag = new List<string>();
                    wemTag.Add("DIDX");
                    wemTag.Add(i.ToString());
                    wem.Tag = wemTag;
                    //ContextMenu cm = FindResource("WemRightClick") as ContextMenu;
                    //this is horrible but good enough to handle this
                    //cm.Tag = wemTag;
                    //wem.ContextMenu = cm;
                    didx.Items.Add(wem);
                }
                treeView1.Items.Add(didx);
            }
            for (int i = 0; i < nbnk.holding.Count; i++)
            {
                byte[] magic = { nbnk.holding[i][0], nbnk.holding[i][1], nbnk.holding[i][2], nbnk.holding[i][3] };
                TreeViewItem holdingItem = new TreeViewItem();
                holdingItem.Header = Encoding.UTF8.GetString(magic);
                holdingItem.Foreground = HelperFunctions.GetBrushFromHex("#AAAAAA");
                List<string> tag = new List<string>();
                tag.Add(Encoding.UTF8.GetString(magic));
                holdingItem.Tag = tag;
                treeView1.Items.Add(holdingItem);
            }
            treeView1_SelectedItemChanged(0, new RoutedEventArgs());
        }


        public void treeView1_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if(treeView1.SelectedItem != null)
            {
                TreeViewItem ti = (TreeViewItem)treeView1.SelectedItem;
                List<string> tag = (List<string>)ti.Tag;
                if (tag[0] == "BKHD")
                {
                    ContentController.Content = nbnk.BankHeader;
                }
                else if (tag[0] == "DIDX")
                {
                    if(tag[1] != "-1")//block it from trying to look at the "Data Index" tree item
                    {
                        ContentController.Content = nbnk.DataIndex.wemList[Convert.ToInt32(tag[1])];
                    }
                }
                else
                {
                    ContentController.Content = null;
                }
            }
            else
            {
                ContentController.Content = null;
            }

        }
        private void Import_Wems(object sender, RoutedEventArgs e)
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
                    nbnk.DataIndex.AddWem(fileName,0, new BinaryReader(File.Open(fileName, FileMode.Open)));
                }

            }
            PopulateTreeView(true);

        }

        private void Replace_Wem(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            ContextMenu cm = (ContextMenu)item.Parent;
            List<string> tag = (List<string>)cm.Tag;
            OpenFileDialog openFile = new OpenFileDialog();
            if (ImportPath != null)
            {
                openFile.InitialDirectory = ImportPath;
            }
            openFile.Multiselect = false;
            openFile.Filter = "WWise Wem files (*.wem)|*.wem";
            if (openFile.ShowDialog() == true)
            {
                foreach (string fileName in openFile.FileNames)
                {
                    if(tag[0] == "DIDX")
                    {
                        ulong newId = nbnk.DataIndex.wemList[Convert.ToInt32(tag[1])].Id;
                        Wem newWem = new Wem(fileName, newId.ToString(), new BinaryReader(File.Open(fileName, FileMode.Open)));
                        nbnk.DataIndex.wemList[Convert.ToInt32(tag[1])] = newWem;
                    }
                }

            }
            PopulateTreeView(true);
        }

        private void Export_Wems(object sender, RoutedEventArgs e)
        {
            OpenFileDialog exportFile = new OpenFileDialog();
            if (ExportPath != null)
            {
                exportFile.InitialDirectory = ExportPath;
            }
            exportFile.CheckFileExists = false;
            exportFile.FileName = "Save Here";
            if (exportFile.ShowDialog() == true)
            {
                string fullPath = exportFile.FileName;
                string savePath = System.IO.Path.GetDirectoryName(fullPath);
                MessageBoxResult exportIds = MessageBox.Show("Export with names?", "Export", MessageBoxButton.YesNo);
                foreach (Wem newWem in nbnk.DataIndex.wemList)
                {
                    string name;
                    if(exportIds == MessageBoxResult.Yes)
                    {
                        name = savePath + "\\" + newWem.Name + ".wem";
                    }
                    else
                    {
                        name = savePath + "\\" + newWem.Id + ".wem";
                    }
                    BinaryWriter bw = new BinaryWriter(new FileStream(name, FileMode.OpenOrCreate));
                    bw.Write(newWem.file);
                    bw.Close();
                }

            }


        }

        private void Delete_Wem(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem)sender;
                ContextMenu cm = (ContextMenu)item.Parent;
                List<string> tag = (List<string>)cm.Tag;
                if (tag[0] == "DIDX")
                {
                    nbnk.DataIndex.wemList.RemoveAt(Convert.ToInt32(tag[1]));
                }
                PopulateTreeView(true);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("File not Loaded!");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("No entry selected!");
            }
            PopulateTreeView(true);
        }

        private void LabelChanged(object sender, RoutedEventArgs e)
        {
            if(LabelsChanged == false)
            {
                LabelsChanged = true;
            }
            TextBox textbox = (TextBox)sender;
            Wem nWem = (Wem)textbox.DataContext;
            if(!changedIds.Contains(nWem.Id))
            {
                changedIds.Add(nWem.Id);
            }

        }

        private void treeView1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Make sure this is the right button.
            if (e.RightButton != MouseButtonState.Pressed) return;

            TreeViewItem currentItem = treeView1.SelectedItem as TreeViewItem;
            
            if (currentItem == null) return;
            if(currentItem.Tag != null)
            {
                List<string> tag = currentItem.Tag as List<string>;
                switch (tag[0])
                {
                    case "DIDX":
                        if(tag[1] != "-1")
                        {
                            treeView1.ContextMenu = FindResource("WemRightClick") as ContextMenu;
                            treeView1.ContextMenu.Tag = tag;
                        }
                        else
                        {
                            treeView1.ContextMenu = FindResource("DIDXRightClick") as ContextMenu;
                            treeView1.ContextMenu.Tag = tag;
                        }
                        break;
                }
            }
            
        }

        private void SaveLabels(object sender, CancelEventArgs e)
        {
            if (LabelsChanged)
            {
                //prompt user
                MessageBoxResult saveLabels = MessageBox.Show("Save changed labels?", "", MessageBoxButton.YesNo);
                if(saveLabels == MessageBoxResult.Yes)
                {
                    nbnk.labels.Export(Directory.GetCurrentDirectory() + "/" + mode.ToString() + "/BNK/" + nbnk.BankHeader.dwSoundbankID.ToString()+".lbl",nbnk.DataIndex.wemList,changedIds);
                }
            }
            LabelsChanged = false;
            changedIds = new List<ulong>();
        }

        private void MassReplace(object sender, RoutedEventArgs e)
        {
            List<ulong> wemIds = new List<ulong>();
            for (int i = 0; i < nbnk.DataIndex.wemList.Count; i++)
            {
                wemIds.Add(nbnk.DataIndex.wemList[i].Id);
            }
            MassReplace mass = new MassReplace(wemIds, ImportPath);
            if (mass.ShowDialog() == true)
            {
                for (int i = 0; i < mass.holder.wems.Count; i++)
                {
                    int index = nbnk.DataIndex.wemList.FindIndex(x => x.Id == mass.holder.wems[i].replacingId);
                    Wem newWem = mass.holder.wems[i].wem;
                    if (index != -1)
                    {
                        newWem.Id = nbnk.DataIndex.wemList[index].Id;
                        newWem.LanguageEnum = nbnk.DataIndex.wemList[index].LanguageEnum;
                        nbnk.DataIndex.wemList[index] = newWem;
                        PopulateTreeView(true);
                    }
                }
            }
        }
    }
    public class LanguageConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint val = (uint)value;
            switch (val)
            {
                case 1:
                    return "Arabic";
                case 2:
                    return "Bulgarian";
                case 3:
                    return "Chinese(HK)";
                case 4:
                    return "Chinese(PRC)";
                case 5:
                    return "Chinese(TW)";
                case 6:
                    return "Czech";
                case 7:
                    return "Danish";
                case 8:
                    return "Dutch";
                case 9:
                    return "English(Australia)";
                case 10:
                    return "English(India)";
                case 11:
                    return "English(UK)";
                case 12:
                    return "English(US)";
                case 13:
                    return "Finnish";
                case 14:
                    return "French(Canada)";
                case 15:
                    return "French(France)";
                case 16:
                    return "German";
                case 17:
                    return "Greek";
                case 18:
                    return "Hebrew";
                case 19:
                    return "Hungarian";
                case 20:
                    return "Indonesian";
                case 21:
                    return "Italian";
                case 22:
                    return "Japanese";
                case 23:
                    return "Korean";
                case 24:
                    return "Latin";
                case 25:
                    return "Norwegian";
                case 26:
                    return "Polish";
                case 27:
                    return "Portuguese(Brazil)";
                case 28:
                    return "Portuguese(Portugal)";
                case 29:
                    return "Romanian";
                case 30:
                    return "Russian";
                case 31:
                    return "Slovenian";
                case 32:
                    return "Spanish(Mexico)";
                case 33:
                    return "Spanish(Spain)";
                case 34:
                    return "Spanish(US)";
                case 35:
                    return "Swedish";
                case 36:
                    return "Turkish";
                case 37:
                    return "Ukranian";
                case 38:
                    return "Vietnamese";
                default:
                    return "SFX";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ChunkSelector : DataTemplateSelector
    {
        public DataTemplate BKHDTemplate { get; set; }
        public DataTemplate WemTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                if (item is BKHD)
                {
                    return BKHDTemplate;
                }
                else if (item is Wem)
                {
                    return WemTemplate;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
