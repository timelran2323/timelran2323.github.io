using System;

namespace PasswordManager
{
    [Serializable]
    public class PasswordEntry
    {
        public string Title { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public PasswordEntry()
        {
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public override string ToString() => $"{Title} - {Username}";
    }
}