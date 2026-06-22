using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PasswordManager
{
    public static class DataManager
    {
        private static readonly string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PasswordManager");
        private static readonly string DataFile = Path.Combine(DataDirectory, "passwords.dat");

        public static void SaveData(List<PasswordEntry> entries, string masterPassword)
        {
            if (!Directory.Exists(DataDirectory)) Directory.CreateDirectory(DataDirectory);
            XmlSerializer serializer = new XmlSerializer(typeof(List<PasswordEntry>));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, entries);
                string encrypted = EncryptionHelper.Encrypt(writer.ToString(), masterPassword);
                File.WriteAllText(DataFile, encrypted);
            }
        }

        public static List<PasswordEntry> LoadData(string masterPassword)
        {
            if (!File.Exists(DataFile)) return new List<PasswordEntry>();
            string encrypted = File.ReadAllText(DataFile);
            string xml = EncryptionHelper.Decrypt(encrypted, masterPassword);
            if (string.IsNullOrEmpty(xml)) return null;
            XmlSerializer serializer = new XmlSerializer(typeof(List<PasswordEntry>));
            using (StringReader reader = new StringReader(xml))
                return (List<PasswordEntry>)serializer.Deserialize(reader);
        }

        public static bool DataFileExists() => File.Exists(DataFile);
    }
}