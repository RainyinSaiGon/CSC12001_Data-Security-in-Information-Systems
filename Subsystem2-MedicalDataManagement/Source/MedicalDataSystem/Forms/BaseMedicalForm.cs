using System;
using System.Windows.Forms;
using System.Drawing;

namespace MedicalDataSystem.Forms
{
    public partial class BaseMedicalForm : Form
    {
        private readonly Button _logoutButton = new();

        public BaseMedicalForm()
        {
            // InitializeComponent();
            SetupBaseUi();
        }

        private void SetupBaseUi()
        {
            _logoutButton.Text = "Đăng xuất";
            _logoutButton.Size = new Size(100, 30);
            _logoutButton.Location = new Point(this.ClientSize.Width - 110, this.ClientSize.Height - 40);
            _logoutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _logoutButton.BackColor = Color.LightCoral;

            _logoutButton.Click += (s, e) => {
                if (MessageBox.Show("Bạn có muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    
                    
                    this.DialogResult = DialogResult.OK; // Đánh dấu là đóng Form để Logout
                    this.Close(); // Đóng Form hiện tại
                }
            };

            this.Controls.Add(_logoutButton);
            _logoutButton.BringToFront();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (!ReferenceEquals(e.Control, _logoutButton))
            {
                _logoutButton.BringToFront();
            }
        }
    }
}