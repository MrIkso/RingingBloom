using RingingBloom.Common;
using RingingBloom.WWiseTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;

namespace RingingBloom
{
    public class PCKString
    {
        public string value;
        public uint index;

        public PCKString(BinaryReader br, SupportedGames mode, long start)
        {
            uint offset = br.ReadUInt32();
            index = br.ReadUInt32();
            long retval = br.BaseStream.Position;
            br.BaseStream.Seek(start + offset, SeekOrigin.Begin);
            if (mode == SupportedGames.MHRiseSwitch)
            {
                value = HelperFunctions.ReadNullTerminatedString(br);
            }
            else
            {
                value = HelperFunctions.ReadUniNullTerminatedString(br);
            }
            br.BaseStream.Seek(retval, SeekOrigin.Begin);

        }
        public PCKString(uint Index, string Value)
        {
            index = Index;
            value = Value;
        }
    }
    public class NPCKHeader
    {
        public SupportedGames mode = SupportedGames.None;
        public Labels labels = new Labels();
        public byte[] magic = { (byte)'A', (byte)'K', (byte)'P', (byte)'K' };
        public uint headerLength;
        uint version = 1;
        uint languageLength;
        uint bnkTableLength = 4;
        public uint wemTableLength;
        uint externalSize = 4;//this and bnkTable are not at all interpreted, but they explain the two extra uint 0000 - I assume it's count and they're just 0
        public List<PCKString> pckStrings = new List<PCKString>();
        public List<Wem> WemList = new List<Wem>();
        public List<Wem> BankList = new List<Wem>();

        //created constructor
        public NPCKHeader(SupportedGames Mode)
        {
            mode = Mode;
            pckStrings.Add(new PCKString(0, "sfx"));
        }
        //imported constructor
        public NPCKHeader(BinaryReader reader, SupportedGames Mode, string fileName)
        {
            mode = Mode;
            string labelPath = Directory.GetCurrentDirectory() + "/" + mode.ToString() + "/PCK/" + fileName + ".lbl";
            if (File.Exists(labelPath))
            {
                MessageBoxResult labelRead = MessageBox.Show("Label file found. Read labels?", "Labels", MessageBoxButton.YesNo);
                if (labelRead == MessageBoxResult.Yes)
                {
                    labels = new Labels(XmlReader.Create(labelPath));
                }
            }
            char[] magicBytes = reader.ReadChars(4);
            uint headerLen = reader.ReadUInt32();
            version = reader.ReadUInt32();
            languageLength = reader.ReadUInt32();
            bnkTableLength = reader.ReadUInt32();
            wemTableLength = reader.ReadUInt32();
            externalSize = reader.ReadUInt32();

            long stringHeaderStart = reader.BaseStream.Position;
            uint stringCount = reader.ReadUInt32();
            for (int i = 0; i < stringCount; i++)
            {
                PCKString stringData = new PCKString(reader, Mode, stringHeaderStart);
                pckStrings.Add(stringData);
            }
            reader.BaseStream.Seek(stringHeaderStart + languageLength, SeekOrigin.Begin);
            /*for (int i = 0; i < bnkTableLength / 4; i++)
            {
                reader.ReadUInt32();
            }*/
          //  reader.BaseStream.Seek(AlignSeek(reader.BaseStream, 4), SeekOrigin.Begin); //align to 4 bytes
           
            Debug.WriteLine($"Current position: {reader.BaseStream.Position:X}");

            #region Banks
            uint numBanks = reader.ReadUInt32();
            Debug.WriteLine($"Banks count: {numBanks}");
            BankList = GetFileEntries(reader, numBanks);
            #endregion

            #region Streamed
            uint numStreams = reader.ReadUInt32();
            Debug.WriteLine($"Streams count: {numStreams}");
            List<Wem> streams = GetFileEntries(reader, numStreams);
            #endregion

            uint numExternals = reader.ReadUInt32();
            Debug.WriteLine($"Externals count: {numExternals}\n");
            List<Wem> externals = GetFileEntries(reader, numExternals, true);
            // Read wem entry table
            foreach (Wem wem in BankList)
            {
                reader.BaseStream.Seek(AlignSeek(reader.BaseStream, wem.BlockSize), SeekOrigin.Begin);
                wem.file = reader.ReadBytes((int)wem.Length);
                string name;
                if (labels.wemLabels.ContainsKey(wem.Id))
                {
                    name = labels.wemLabels[wem.Id];
                }
                else
                {
                    name = "Imported Bank " + wem.Name;
                }
                wem.Name = name;
                WemList.Add(wem);
            }
            foreach (Wem wem in streams)
            {
                reader.BaseStream.Seek(AlignSeek(reader.BaseStream, wem.BlockSize), SeekOrigin.Begin);
                wem.file = reader.ReadBytes((int)wem.Length);
                string name;
                if (labels.wemLabels.ContainsKey(wem.Id))
                {
                    name = labels.wemLabels[wem.Id];
                }
                else
                {
                    name = "Imported Wem " + wem.Name;
                }
                wem.Name = name;
                WemList.Add(wem);
            }

            foreach (Wem wem in externals)
            {
                reader.BaseStream.Seek(AlignSeek(reader.BaseStream, wem.BlockSize), SeekOrigin.Begin);
                wem.file = reader.ReadBytes((int)wem.Length);
                string name;
                if (labels.wemLabels.ContainsKey(wem.Id))
                {
                    name = labels.wemLabels[wem.Id];
                }
                else
                {
                    name = "Imported Wem " + wem.Name;
                }
                wem.Name = name;
                WemList.Add(wem);
            }
        }

        private List<Wem> GetFileEntries(BinaryReader br, uint wemCount, bool isUINT64 = false)
        {
            List<Wem> wemList = new List<Wem>((int)wemCount);
            for (int i = 0; i < wemCount; i++)
            {
                ulong id = isUINT64 ? br.ReadUInt64() : br.ReadUInt32();
                uint blockSize = br.ReadUInt32();
                uint length = br.ReadUInt32();
                uint offset = br.ReadUInt32();
                uint languageEnum = br.ReadUInt32();
                Wem newWem = new Wem(i.ToString(), id, blockSize, length, offset, languageEnum);
                wemList.Add(newWem);
            }
            return wemList;
        }

        public long AlignSeek(Stream inputStream, long alignTo, SeekOrigin origin = SeekOrigin.Current)
        {
            long seekBytes = inputStream.Position % alignTo;
            if (seekBytes > 0)
            {
                long nextPos = alignTo - seekBytes;
                switch (origin)
                {
                    case SeekOrigin.Begin: inputStream.Position = nextPos; break;
                    case SeekOrigin.Current: inputStream.Position += nextPos; break;
                    case SeekOrigin.End: inputStream.Position = inputStream.Length - nextPos; break;
                }
            }

            return inputStream.Position;
        }

        public List<string> GetLanguages()
        {
            List<string> langList = new List<string>();
            for (int i = 0; i < pckStrings.Count; i++)
            {
                try
                {
                    langList.Insert((int)pckStrings[i].index, pckStrings[i].value);
                }
                catch (ArgumentOutOfRangeException)
                {
                    langList.Add(pckStrings[i].value);//thought is that should order be 2 0 1, the 2 is added, then 0 is inserted in front of it and then 1 is inserted in front of it
                }
            }
            return langList;
        }

        public List<byte> GenerateLanguageBytes(SupportedGames mode)
        {
            List<byte> languageBytes = new List<byte>();

            byte[] count = BitConverter.GetBytes(pckStrings.Count);
            for (int i = 0; i < count.Length; i++)
            {
                languageBytes.Add(count[i]);
            }
            //get "header" size for use later
            int headerSize = 4 + (pckStrings.Count * 8);
            //now generate string table and offsets
            List<byte> stringTable = new List<byte>();
            int[] offsets = new int[pckStrings.Count];
            List<string> strings = new List<string>();
            for (int i = 0; i < pckStrings.Count; i++)
            {
                strings.Add(pckStrings[i].value);
            }
            strings.Sort();//alphabetically
            for (int i = 0; i < strings.Count; i++)
            {
                byte[] asBytes;
                if (mode == SupportedGames.MHRiseSwitch)
                {
                    asBytes = Encoding.UTF8.GetBytes(strings[i]);
                }
                else
                {
                    asBytes = Encoding.Unicode.GetBytes(strings[i]);
                }

                for (int j = 0; j < pckStrings.Count; j++)
                {
                    if (pckStrings[j].value == strings[i])
                    {
                        offsets[j] = Math.Max(0, headerSize + stringTable.Count);
                        for (int k = 0; k < asBytes.Length; k++)
                        {
                            stringTable.Add(asBytes[k]);
                        }
                        if (mode == SupportedGames.MHRiseSwitch)
                        {
                            stringTable.Add(0);
                        }
                        else
                        {
                            stringTable.Add(0);
                            stringTable.Add(0);
                        }
                    }
                }
            }
            //finally add pckString data to languageBytes
            for (int i = 0; i < pckStrings.Count; i++)
            {
                byte[] offset = BitConverter.GetBytes(offsets[i]);
                for (int j = 0; j < offset.Length; j++)
                {
                    languageBytes.Add(offset[j]);
                }
                byte[] index = BitConverter.GetBytes(pckStrings[i].index);
                for (int j = 0; j < index.Length; j++)
                {
                    languageBytes.Add(index[j]);
                }
            }
            //concat string table
            for (int i = 0; i < stringTable.Count; i++)
            {
                languageBytes.Add(stringTable[i]);
            }
            //align 4
            while (languageBytes.Count % 4 != 0)
            {
                languageBytes.Add(0);
            }

            return languageBytes;
        }

        public void ExportHeader(string aFilePath)
        {
            //TODO: rewrite this in a way that lets me use it for nonstream and stream
            wemTableLength = (uint)(WemList.Count * 20) + 4;
            List<byte> languageBytes = GenerateLanguageBytes(mode);
            headerLength = (uint)(wemTableLength + 20 + languageBytes.Count + bnkTableLength + externalSize);
            BinaryWriter bw = new BinaryWriter(File.Create(aFilePath));
            bw.Write(magic);
            bw.Write(headerLength);
            bw.Write(version);
            bw.Write(languageBytes.Count);
            bw.Write(bnkTableLength);//const in Capcom pcks
            bw.Write(wemTableLength);
            bw.Write(externalSize);//const in Capcom pcks
            for (int i = 0; i < languageBytes.Count; i++)
            {
                bw.Write(languageBytes[i]);
            }
            bw.Write((int)0);
            bw.Write(WemList.Count);
            uint currentOffset = headerLength + 8;//the +8 is because magic and header length are not included in header length
            foreach (Wem wem in WemList)
            {
                bw.Write(wem.Id);
                bw.Write(1);
                bw.Write(wem.Length);
                bw.Write(currentOffset);
                currentOffset += wem.Length;
                bw.Write(wem.LanguageEnum);
            }
            bw.Write((int)0);
            bw.Close();
        }

        public void ExportFile(string aFilePath)
        {
            wemTableLength = (uint)(WemList.Count * 20) + 4;
            List<byte> languageBytes = GenerateLanguageBytes(mode);
            headerLength = (uint)(wemTableLength + 20 + languageBytes.Count + bnkTableLength + externalSize);
            BinaryWriter bw = new BinaryWriter(File.Create(aFilePath));
            bw.Write(magic);
            bw.Write(headerLength);
            bw.Write(version);
            bw.Write(languageBytes.Count);
            bw.Write(bnkTableLength);//const in Capcom pcks
            bw.Write(wemTableLength);
            bw.Write(externalSize);//const in Capcom pcks
            for (int i = 0; i < languageBytes.Count; i++)
            {
                bw.Write(languageBytes[i]);
            }
            bw.Write((int)0);
            bw.Write(WemList.Count);
            uint currentOffset = headerLength + 8;
            //later, may need to separate into an "offsets" list and write files afterwards rather than immediately
            foreach (Wem bank in BankList)
            {
                bw.Write(bank.Id);
                bw.Write(1);
                bw.Write(bank.Length);
                bw.Write(currentOffset);
                int workingOffset = (int)bw.BaseStream.Position;
                bw.Seek((int)currentOffset, SeekOrigin.Begin);
                bw.Write(bank.file);
                bw.Seek(workingOffset, SeekOrigin.Begin);
                currentOffset += bank.Length;
                bw.Write(bank.LanguageEnum);
            }

            foreach (Wem wem in WemList)
            {
                bw.Write(wem.Id);
                bw.Write(1);
                bw.Write(wem.Length);
                bw.Write(currentOffset);
                int workingOffset = (int)bw.BaseStream.Position;
                bw.Seek((int)currentOffset, SeekOrigin.Begin);
                bw.Write(wem.file);
                bw.Seek(workingOffset, SeekOrigin.Begin);
                currentOffset += wem.Length;
                bw.Write(wem.LanguageEnum);
            }
            bw.Close();
        }
    }
}
