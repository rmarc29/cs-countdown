using System;
using System.Drawing;
using System.Windows.Forms;

namespace CountdownTimerApp
{
    public class CountdownTimerForm : Form
    {
        private TextBox txtTimeInput, txtEventName;
        private PictureBox pictureBox;
        private Label lblCountdown;
        private Button btnStart, btnStop, btnReset, btnMiniMode, btnRemovePicture, btnToggleMode, btnPickColor;
        private System.Windows.Forms.Timer timer;
        private DateTime endTime;
        private bool isMiniMode = false;

        public CountdownTimerForm()
        {
            this.Text = "Countdown";
            this.Size = new Size(600, 400);
            this.MinimumSize = new Size(400, 300);

            txtEventName = new TextBox { PlaceholderText = "Event Name", Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Location = new Point(20, 20), Width = 200 };
            this.Controls.Add(txtEventName);

            txtTimeInput = new TextBox { PlaceholderText = "Enter time (e.g., '7d', '3w', '2m', '30s')", Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Location = new Point(20, 60), Width = 200 };
            this.Controls.Add(txtTimeInput);

            pictureBox = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(300, 20),
                Size = new Size(100, 100),
                BackColor = Color.LightGray,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            pictureBox.Click += PictureBox_Click;
            this.Controls.Add(pictureBox);

            lblCountdown = new Label
            {
                Text = "00:00:00",
                Font = new Font("Arial", 24),
                Location = new Point(20, 100),
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(lblCountdown);

            btnStart = new Button { Text = "Start", Location = new Point(20, 150), Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);

            btnStop = new Button { Text = "Stop", Location = new Point(100, 150), Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            btnStop.Click += BtnStop_Click;
            this.Controls.Add(btnStop);

            btnReset = new Button { Text = "Reset", Location = new Point(180, 150), Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            btnMiniMode = new Button { Text = "Mini Mode", Location = new Point(260, 150), Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            btnMiniMode.Click += BtnMiniMode_Click;
            this.Controls.Add(btnMiniMode);

            btnRemovePicture = new Button { Text = "Remove Picture", Location = new Point(300, 200), Width = 150, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnRemovePicture.Click += BtnRemovePicture_Click;
            this.Controls.Add(btnRemovePicture);

            btnToggleMode = new Button { Text = "Toggle Mode", Location = new Point(20, 200), Width = 100, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnToggleMode.Click += BtnMiniMode_Click;
            this.Controls.Add(btnToggleMode);

            btnPickColor = new Button { Text = "Pick Background Color", Location = new Point(130, 200), Width = 150, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnPickColor.Click += BtnPickColor_Click;
            this.Controls.Add(btnPickColor);

            timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += Timer_Tick;

            this.Resize += CountdownTimerForm_Resize;
        }

        private void CountdownTimerForm_Resize(object sender, EventArgs e)
        {
            lblCountdown.Left = (this.ClientSize.Width - lblCountdown.Width) / 2;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = Image.FromFile(ofd.FileName);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void BtnRemovePicture_Click(object sender, EventArgs e) => pictureBox.Image = null;
        private void BtnStart_Click(object sender, EventArgs e) { if (ParseTimeInput(txtTimeInput.Text, out TimeSpan duration)) { endTime = DateTime.Now.Add(duration); timer.Start(); } else { MessageBox.Show("Invalid time format"); } }
        private void BtnStop_Click(object sender, EventArgs e) => timer.Stop();
        private void BtnReset_Click(object sender, EventArgs e) { timer.Stop(); lblCountdown.Text = "00:00:00"; }
        private void Timer_Tick(object sender, EventArgs e) { var remaining = endTime - DateTime.Now; lblCountdown.Text = remaining.TotalSeconds > 0 ? remaining.ToString(@"dd\:hh\:mm\:ss") : "Time's up!"; if (remaining.TotalSeconds <= 0) timer.Stop(); }

        private void BtnMiniMode_Click(object sender, EventArgs e)
        {
            isMiniMode = !isMiniMode;
            if (isMiniMode)
            {
                this.Size = new Size(200, 100);
                lblCountdown.Location = new Point(20, 20);
                foreach (Control control in this.Controls)
                {
                    if (control != lblCountdown && control != btnToggleMode) control.Visible = false;
                }
            }
            else
            {
                this.Size = new Size(600, 400);
                foreach (Control control in this.Controls)
                {
                    control.Visible = true;
                }
            }
        }

        private void BtnPickColor_Click(object sender, EventArgs e)
        {
            using ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.BackColor = colorDialog.Color;
            }
        }

        private bool ParseTimeInput(string input, out TimeSpan duration)
        {
            duration = TimeSpan.Zero;
            if (string.IsNullOrWhiteSpace(input)) return false;
            try { int value = int.Parse(input[..^1]); char unit = input[^1]; duration = unit switch { 'm' => TimeSpan.FromMinutes(value), 'h' => TimeSpan.FromHours(value), 'd' => TimeSpan.FromDays(value), 'w' => TimeSpan.FromDays(value * 7), _ => throw new Exception("Invalid unit") }; return true; } catch { return false; }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new CountdownTimerForm());
        }
    }
}
