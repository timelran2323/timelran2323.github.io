using System;
using System.Drawing;
using System.Windows.Forms;

namespace PasswordManager
{
    public class LoginForm : Form
    {
        public string MasterPassword { get; private set; }
        private bool isNewUser;
        private TextBox txtPassword, txtConfirm;
        private CheckBox chkShowPassword, chkShowConfirm;
        private Button btnLogin, btnCancel;
        private Label lblTitle, lblPassword, lblConfirm;

        public LoginForm()
        {
            this.Size = new Size(500, 270);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Авторизация";

            lblTitle = new Label() { Text = "Вход в Password Manager", Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true };
            lblPassword = new Label() { Text = "Мастер-пароль:", Location = new Point(30, 70), AutoSize = true };
            txtPassword = new TextBox() { Location = new Point(140, 67), Size = new Size(180, 20), UseSystemPasswordChar = true };
            chkShowPassword = new CheckBox() { Text = "Показать пароль", Location = new Point(330, 69), AutoSize = true };
            chkShowPassword.CheckedChanged += (s, e) => txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

            lblConfirm = new Label() { Text = "Подтверждение:", Location = new Point(30, 100), AutoSize = true, Visible = false };
            txtConfirm = new TextBox() { Location = new Point(140, 97), Size = new Size(180, 20), UseSystemPasswordChar = true, Visible = false };
            chkShowConfirm = new CheckBox() { Text = "Показать пароль", Location = new Point(330, 99), AutoSize = true, Visible = false };
            chkShowConfirm.CheckedChanged += (s, e) => txtConfirm.UseSystemPasswordChar = !chkShowConfirm.Checked;

            btnLogin = new Button() { Text = "Войти", Location = new Point(140, 150), Size = new Size(100, 30) };
            btnLogin.Click += BtnLogin_Click;
            btnCancel = new Button() { Text = "Отмена", Location = new Point(260, 150), Size = new Size(100, 30) };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(lblTitle);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(chkShowPassword);
            Controls.Add(lblConfirm);
            Controls.Add(txtConfirm);
            Controls.Add(chkShowConfirm);
            Controls.Add(btnLogin);
            Controls.Add(btnCancel);

            isNewUser = !DataManager.DataFileExists();
            if (isNewUser)
            {
                lblTitle.Text = "Создание мастер-пароля";
                lblPassword.Text = "Новый пароль:";
                btnLogin.Text = "Создать";
                this.Text = "Первый запуск";
                lblConfirm.Visible = true;
                txtConfirm.Visible = true;
                chkShowConfirm.Visible = true;
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string pwd = txtPassword.Text.Trim();
            if (string.IsNullOrEmpty(pwd))
            {
                MessageBox.Show("Введите мастер-пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isNewUser)
            {
                string confirm = txtConfirm.Text.Trim();
                if (pwd != confirm)
                {
                    MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (pwd.Length < 4)
                {
                    MessageBox.Show("Мастер-пароль должен быть не менее 4 символов!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MasterPassword = pwd;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                var test = DataManager.LoadData(pwd);
                if (test != null)
                {
                    MasterPassword = pwd;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный мастер-пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
        }
    }
}