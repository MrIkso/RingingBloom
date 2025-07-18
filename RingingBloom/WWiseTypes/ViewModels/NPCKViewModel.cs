﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using RingingBloom.Common;
using RingingBloom.Windows;

namespace RingingBloom.WWiseTypes.ViewModels
{
    public class NPCKViewModel  : BaseViewModel
    {
        public NPCKHeader npck = null;//shouldn't have to bind directly to this

        public ObservableCollection<Wem> wems 
        { 
            get {
                if(npck == null)
                {
                    return new ObservableCollection<Wem>();
                }
                else
                {
                    return new ObservableCollection<Wem>(npck.WemList);
                }
                
            } 
            set {
                npck.WemList = new List<Wem>(value);
                OnPropertyChanged("wems"); 
            }
        }
        public ObservableCollection<string> languages
        { 
            get 
            { 
                if(npck == null)
                {
                    return new ObservableCollection<string>();
                }
                else
                {
                    return new ObservableCollection<string>(npck.GetLanguages());
                }
            }
            set => throw new NotImplementedException();
        }

        public NPCKViewModel()
        {
            npck = null;
        }

        public void SetNPCK(NPCKHeader file)
        {
            npck = file;
            OnPropertyChanged("wems");
            OnPropertyChanged("languages");
        }

        public void AddWems(string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                Wem newWem = HelperFunctions.MakeWems(fileName, HelperFunctions.OpenFile(fileName));
                npck.WemList.Add(newWem);
            }
            OnPropertyChanged("wems");
        }

        public void ReplaceWem(Wem newWem,int index)
        {
            newWem.Id = npck.WemList[index].Id;
            newWem.LanguageEnum = npck.WemList[index].LanguageEnum;
            npck.WemList[index] = newWem;
            OnPropertyChanged("wems");
        }
        public void DeleteWem(int index)
        {
            npck.WemList.RemoveAt(index);
            OnPropertyChanged("wems");
        }

        public void ExportWems(MessageBoxResult exportIds, string savePath)
        {
            foreach (Wem newWem in npck.WemList)
            {
                string name;
                if (exportIds == MessageBoxResult.Yes)
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

        public void ExportNPCK(string fileName, SupportedGames mode)
        {
            npck.ExportFile(fileName);
            if (mode == SupportedGames.RE2DMC5 || mode == SupportedGames.RE3R || mode == SupportedGames.MHRise || mode == SupportedGames.RE8)
            {
                npck.ExportHeader(fileName + ".nonstream");
            }
        }

        public void IDReplace(string[] id2)
        {
            for (int i = 0; i < id2.Length; i++)
            {
                try
                {
                    npck.WemList[i].Id = Convert.ToUInt32(id2[i]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
            OnPropertyChanged("wems");
        }

        public void ExportLabels(SupportedGames mode, string currentFileName, List<ulong> changedIds)
        {
            npck.labels.Export(Directory.GetCurrentDirectory() + "/" + mode.ToString() + "/PCK/" + currentFileName + ".lbl", npck.WemList, changedIds);
        }

        public List<ulong> GetWemIds()
        {
            List<ulong> wemIds = new List<ulong>();
            for (int i = 0; i < npck.WemList.Count; i++)
            {
                wemIds.Add(npck.WemList[i].Id);
            }
            return wemIds;
        }

        public void MassReplace(List<ReplacingWem> mass)
        {
            for (int i = 0; i < mass.Count; i++)
            {
                int index = npck.WemList.FindIndex(x => x.Id == mass[i].replacingId);
                Wem newWem = mass[i].wem;
                if (index != -1)
                {
                    newWem.Id = npck.WemList[index].Id;
                    newWem.LanguageEnum = npck.WemList[index].LanguageEnum;
                    npck.WemList[index] = newWem;
                }
            }
            OnPropertyChanged("Wems");
        }
    }
}
