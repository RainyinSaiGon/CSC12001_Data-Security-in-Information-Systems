using System;
using System.Windows.Forms;
using System.Drawing;

namespace MedicalDataSystem.Forms
{
    public partial class BaseMedicalForm : Form
    {
        public BaseMedicalForm()
        {
            // InitializeComponent();
            SetupBaseUi();
        }

        private void SetupBaseUi()
        {
            Button btnLogout = new Button
            {
                Text = "Đăng xuất",
                Size = new Size(100, 30),
                Location = new Point(this.ClientSize.Width - 110, this.ClientSize.Height - 40),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.LightCoral
            };
            btnLogout.Click += (s, e) => {
                if (MessageBox.Show("Bạn có muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    
                    
                    this.DialogResult = DialogResult.OK; // Đánh dấu là đóng Form để Logout
                    this.Close(); // Đóng Form hiện tại
                }
            };
            this.Controls.Add(btnLogout);
        }
    }
}