using System;
using System.Drawing;
using System.Windows.Forms;

namespace PasswordManager
{
    public class EntryForm : Form
    {
        public PasswordEntry Entry { get; private set; }
        private string masterPassword;
        private bool isEditMode;

        private TextBox txtTitle, txtUsername, txtPassword, txtNotes;
        private CheckBox chkShowPassword;
        private Button btnGenerate, btnOK, btnCancel;
        private GroupBox groupGenerator;
        private NumericUpDown numLength;
        private CheckBox chkUppercase, chkLowercase, chkDigits, chkSpecial;
        private Button btnGenerateNow;

        public EntryForm(string masterPwd, PasswordEntry entryToEdit = null)
        {
            masterPassword = masterPwd;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(560, 420);

            if (entryToEdit != null)
            {
                Entry = entryToEdit;
                isEditMode = true;
                this.Text = "Редактирование записи";
            }
            else
            {
                Entry = new PasswordEntry();
                this.Text = "Новая запись";
            }

            // Элементы
            Label lblTitle = new Label() { Text = "Ресурс/Сайт:", Location = new Point(20, 20), AutoSize = true };
            txtTitle = new TextBox() { Location = new Point(120, 17), Size = new Size(250, 20) };
            Label lblUsername = new Label() { Text = "Логин:", Location = new Point(20, 50), AutoSize = true };
            txtUsername = new TextBox() { Location = new Point(120, 47), Size = new Size(250, 20) };
            Label lblPassword = new Label() { Text = "Пароль:", Location = new Point(20, 80), AutoSize = true };
            txtPassword = new TextBox() { Location = new Point(120, 77), Size = new Size(190, 20), UseSystemPasswordChar = true };
            btnGenerate = new Button() { Text = "Генератор...", Location = new Point(320, 75), Size = new Size(90, 23) };
            chkShowPassword = new CheckBox() { Text = "Показать", Location = new Point(420, 79), AutoSize = true };
            chkShowPassword.CheckedChanged += (s, e) => txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

            Label lblNotes = new Label() { Text = "Заметки:", Location = new Point(20, 110), AutoSize = true };
            txtNotes = new TextBox() { Location = new Point(120, 107), Size = new Size(390, 60), Multiline = true };

            // Генератор
            groupGenerator = new GroupBox() { Text = "Генератор паролей", Location = new Point(23, 180), Size = new Size(487, 100), Visible = false };
            Label lblLength = new Label() { Text = "Длина (4-32):", Location = new Point(20, 27), AutoSize = true };
            numLength = new NumericUpDown() { Location = new Point(97, 25), Size = new Size(60, 20), Minimum = 4, Maximum = 32, Value = 12 };
            chkUppercase = new CheckBox() { Text = "A-Z", Location = new Point(200, 25), AutoSize = true, Checked = true };
            chkLowercase = new CheckBox() { Text = "a-z", Location = new Point(200, 48), AutoSize = true, Checked = true };
            chkDigits = new CheckBox() { Text = "0-9", Location = new Point(320, 25), AutoSize = true, Checked = true };
            chkSpecial = new CheckBox() { Text = "!@#$", Location = new Point(320, 48), AutoSize = true };
            btnGenerateNow = new Button() { Text = ">>", Location = new Point(430, 35), Size = new Size(50, 30) };
            btnGenerateNow.Click += BtnGenerateNow_Click;

            groupGenerator.Controls.AddRange(new Control[] { btnGenerateNow, chkSpecial, chkDigits, chkLowercase, chkUppercase, lblLength, numLength });

            btnOK = new Button() { Text = "OK", Location = new Point(340, 300), Size = new Size(80, 30) };
            btnOK.Click += BtnOK_Click;
            btnCancel = new Button() { Text = "Отмена", Location = new Point(430, 300), Size = new Size(80, 30) };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnGenerate.Click += (s, e) => groupGenerator.Visible = !groupGenerator.Visible;

            Controls.AddRange(new Control[] { btnCancel, btnOK, groupGenerator, chkShowPassword, btnGenerate, txtPassword, lblPassword, txtUsername, lblUsername, txtTitle, lblTitle, txtNotes, lblNotes });

            if (isEditMode)
            {
                txtTitle.Text = Entry.Title;
                txtUsername.Text = Entry.Username;
                txtPassword.Text = EncryptionHelper.Decrypt(Entry.EncryptedPassword, masterPassword);
                txtNotes.Text = Entry.Notes;
            }
        }

        private void BtnGenerateNow_Click(object sender, EventArgs e)
        {
            if (!chkUppercase.Checked && !chkLowercase.Checked && !chkDigits.Checked && !chkSpecial.Checked)
            {
                MessageBox.Show("Выберите хотя бы один тип символов!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string chars = "";
            if (chkUppercase.Checked) chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (chkLowercase.Checked) chars += "abcdefghijklmnopqrstuvwxyz";
            if (chkDigits.Checked) chars += "0123456789";
            if (chkSpecial.Checked) chars += "!@#$%^&*()_-+=<>?";
            Random rand = new Random();
            int len = (int)numLength.Value;
            char[] pwd = new char[len];
            for (int i = 0; i < len; i++) pwd[i] = chars[rand.Next(chars.Length)];
            txtPassword.Text = new string(pwd);
            chkShowPassword.Checked = true;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitle.Text)) { MessageBox.Show("Введите название ресурса!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrEmpty(txtUsername.Text)) { MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrEmpty(txtPassword.Text)) { MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            Entry.Title = txtTitle.Text;
            Entry.Username = txtUsername.Text;
            Entry.EncryptedPassword = EncryptionHelper.Encrypt(txtPassword.Text, masterPassword);
            Entry.Notes = txtNotes.Text;
            Entry.ModifiedDate = DateTime.Now;
            if (!isEditMode) Entry.CreatedDate = DateTime.Now;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}