using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PasswordManager
{
    public class MainForm : Form
    {
        private List<PasswordEntry> entries;
        private string masterPassword;
        private ListBox lstEntries;
        private TextBox txtSearch;
        private Label lblTitleValue, lblUsernameValue, lblNotesValue, lblCreatedValue, lblModifiedValue;

        public MainForm(string password)
        {
            masterPassword = password;
            entries = new List<PasswordEntry>();
            this.Text = "Password Manager";
            this.Size = new Size(660, 500);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += (s, e) => SaveData();

            // Элементы
            lstEntries = new ListBox() { Font = new Font("Consolas", 10F), Location = new Point(12, 38), Size = new Size(250, 340) };
            lstEntries.SelectedIndexChanged += (s, e) => { if (lstEntries.SelectedItem != null) ShowDetails((PasswordEntry)lstEntries.SelectedItem); };
            lstEntries.DoubleClick += (s, e) => EditEntry();

            txtSearch = new TextBox() { Location = new Point(58, 12), Size = new Size(204, 20) };
            txtSearch.TextChanged += (s, e) => RefreshList();

            Label lblSearch = new Label() { Text = "Поиск:", Location = new Point(12, 15), AutoSize = true };

            Button btnAdd = new Button() { Text = "Добавить", Location = new Point(280, 12), Size = new Size(80, 30) };
            btnAdd.Click += (s, e) => AddEntry();
            Button btnEdit = new Button() { Text = "Редактировать", Location = new Point(370, 12), Size = new Size(80, 30) };
            btnEdit.Click += (s, e) => EditEntry();
            Button btnDelete = new Button() { Text = "Удалить", Location = new Point(460, 12), Size = new Size(80, 30) };
            btnDelete.Click += (s, e) => DeleteEntry();
            Button btnCopyUser = new Button() { Text = "Копировать логин", Location = new Point(280, 390), Size = new Size(120, 30) };
            btnCopyUser.Click += (s, e) => CopyUsername();
            Button btnCopyPass = new Button() { Text = "Копировать пароль", Location = new Point(410, 390), Size = new Size(120, 30) };
            btnCopyPass.Click += (s, e) => CopyPassword();
            Button btnExit = new Button() { Text = "Выход", Location = new Point(540, 390), Size = new Size(80, 30) };
            btnExit.Click += (s, e) => Close();

            GroupBox groupDetails = new GroupBox() { Text = "Детали записи", Location = new Point(280, 50), Size = new Size(340, 320) };
            Label lblTitle = new Label() { Text = "Ресурс:", Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold), Location = new Point(10, 25), AutoSize = true };
            lblTitleValue = new Label() { Text = "---", Location = new Point(80, 25), AutoSize = true };
            Label lblUsername = new Label() { Text = "Логин:", Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold), Location = new Point(10, 55), AutoSize = true };
            lblUsernameValue = new Label() { Text = "---", Location = new Point(80, 55), AutoSize = true };
            Label lblNotes = new Label() { Text = "Заметки:", Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold), Location = new Point(10, 85), AutoSize = true };
            lblNotesValue = new Label() { Text = "---", Location = new Point(80, 85), MaximumSize = new Size(250, 0), AutoSize = true };
            Label lblCreated = new Label() { Text = "Дата создания:", Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold), Location = new Point(10, 200), AutoSize = true };
            lblCreatedValue = new Label() { Text = "---", Location = new Point(115, 200), AutoSize = true };
            Label lblModified = new Label() { Text = "Дата изменения:", Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold), Location = new Point(10, 225), AutoSize = true };
            lblModifiedValue = new Label() { Text = "---", Location = new Point(133, 225), AutoSize = true };

            groupDetails.Controls.AddRange(new Control[] { lblModifiedValue, lblModified, lblCreatedValue, lblCreated, lblNotesValue, lblNotes, lblUsernameValue, lblUsername, lblTitleValue, lblTitle });

            Controls.AddRange(new Control[] { groupDetails, btnExit, btnCopyPass, btnCopyUser, btnDelete, btnEdit, btnAdd, lblSearch, txtSearch, lstEntries });

            LoadData();
        }

        private void LoadData()
        {
            var loaded = DataManager.LoadData(masterPassword);
            entries = loaded ?? new List<PasswordEntry>();
            RefreshList();
        }

        private void RefreshList()
        {
            lstEntries.Items.Clear();
            string search = txtSearch.Text.ToLower();
            var filtered = entries.Where(e => e.Title.ToLower().Contains(search) || e.Username.ToLower().Contains(search)).ToList();
            foreach (var e in filtered) lstEntries.Items.Add(e);
            if (lstEntries.Items.Count > 0 && lstEntries.SelectedIndex == -1) lstEntries.SelectedIndex = 0;
            else ClearDetails();
        }

        private void ClearDetails()
        {
            lblTitleValue.Text = "---";
            lblUsernameValue.Text = "---";
            lblNotesValue.Text = "---";
            lblCreatedValue.Text = "---";
            lblModifiedValue.Text = "---";
        }

        private void ShowDetails(PasswordEntry entry)
        {
            lblTitleValue.Text = entry.Title;
            lblUsernameValue.Text = entry.Username;
            lblNotesValue.Text = string.IsNullOrEmpty(entry.Notes) ? "(нет заметок)" : entry.Notes;
            lblCreatedValue.Text = entry.CreatedDate.ToString("dd.MM.yyyy HH:mm");
            lblModifiedValue.Text = entry.ModifiedDate.ToString("dd.MM.yyyy HH:mm");
        }

        private void SaveData()
        {
            try { DataManager.SaveData(entries, masterPassword); }
            catch (Exception ex) { MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void AddEntry()
        {
            var form = new EntryForm(masterPassword);
            if (form.ShowDialog() == DialogResult.OK)
            {
                entries.Add(form.Entry);
                SaveData();
                RefreshList();
            }
        }

        private void EditEntry()
        {
            if (lstEntries.SelectedItem == null) { MessageBox.Show("Выберите запись.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var selected = (PasswordEntry)lstEntries.SelectedItem;
            var form = new EntryForm(masterPassword, selected);
            if (form.ShowDialog() == DialogResult.OK)
            {
                int index = entries.FindIndex(x => x.Title == selected.Title && x.CreatedDate == selected.CreatedDate);
                if (index >= 0) entries[index] = form.Entry;
                SaveData();
                RefreshList();
            }
        }

        private void DeleteEntry()
        {
            if (lstEntries.SelectedItem == null) { MessageBox.Show("Выберите запись.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var selected = (PasswordEntry)lstEntries.SelectedItem;
            if (MessageBox.Show($"Удалить \"{selected.Title}\"?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                entries.RemoveAll(x => x.Title == selected.Title && x.CreatedDate == selected.CreatedDate);
                SaveData();
                RefreshList();
            }
        }

        private void CopyUsername()
        {
            if (lstEntries.SelectedItem == null) { MessageBox.Show("Выберите запись.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            Clipboard.SetText(((PasswordEntry)lstEntries.SelectedItem).Username);
            MessageBox.Show("Логин скопирован.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CopyPassword()
        {
            if (lstEntries.SelectedItem == null) { MessageBox.Show("Выберите запись.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var selected = (PasswordEntry)lstEntries.SelectedItem;
            string pwd = EncryptionHelper.Decrypt(selected.EncryptedPassword, masterPassword);
            Clipboard.SetText(pwd);
            MessageBox.Show("Пароль скопирован.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}