using System;
using System.Windows.Forms;

namespace PasswordManager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoginForm login = new LoginForm();
            if (login.ShowDialog() == DialogResult.OK)
                Application.Run(new MainForm(login.MasterPassword));
        }
    }
}