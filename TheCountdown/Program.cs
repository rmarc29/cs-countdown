using System;
using System.Drawing;
using System.Windows.Forms;

namespace CountdownTimerApp
{
    public class CountdownTimerForm : Form
    {
        private TextBox txtTimeInput,
            txtEventName;
        private PictureBox pictureBox;
        private Label lblCountdown;
        private Button btnStart,
            btnStop,
            btnReset,
            btnMiniMode,
            btnRemovePicture,
            btnToggleMode,
            btnPickColor;
        private System.Windows.Forms.Timer timer;
        private DateTime endTime;
        private bool isMiniMode = false;

        public CountdownTimerForm()
        {
            this.Text = "Countdown";
            this.Size = new Size(600, 400);
            this.MinimumSize = new Size(400, 300);

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 4,
                AutoSize = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.Controls.Add(layout);

            txtEventName = new TextBox { PlaceholderText = "Event Name", Dock = DockStyle.Fill };
            layout.Controls.Add(txtEventName, 0, 0);
            layout.SetColumnSpan(txtEventName, 2);

            txtTimeInput = new TextBox
            {
                PlaceholderText = "Enter time (e.g., '7d', '3w', '2m', '30s')",
                Dock = DockStyle.Fill
            };
            layout.Controls.Add(txtTimeInput, 0, 1);
            layout.SetColumnSpan(txtTimeInput, 2);

            pictureBox = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(100, 100),
                BackColor = Color.LightGray,
                Dock = DockStyle.Fill
            };
            pictureBox.Click += PictureBox_Click;
            layout.Controls.Add(pictureBox, 2, 0);
            layout.SetRowSpan(pictureBox, 2);

            lblCountdown = new Label
            {
                Text = "00:00:00",
                Font = new Font("Arial", 24),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            layout.Controls.Add(lblCountdown, 0, 2);
            layout.SetColumnSpan(lblCountdown, 3);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            layout.Controls.Add(buttonPanel, 0, 3);
            layout.SetColumnSpan(buttonPanel, 3);

            btnStart = new Button { Text = "Start" };
            btnStart.Click += BtnStart_Click;
            buttonPanel.Controls.Add(btnStart);

            btnStop = new Button { Text = "Stop" };
            btnStop.Click += BtnStop_Click;
            buttonPanel.Controls.Add(btnStop);

            btnReset = new Button { Text = "Reset" };
            btnReset.Click += BtnReset_Click;
            buttonPanel.Controls.Add(btnReset);

            btnMiniMode = new Button { Text = "Mini Mode" };
            btnMiniMode.Click += BtnMiniMode_Click;
            buttonPanel.Controls.Add(btnMiniMode);

            btnRemovePicture = new Button { Text = "Remove Picture" };
            btnRemovePicture.Click += BtnRemovePicture_Click;
            buttonPanel.Controls.Add(btnRemovePicture);

            btnToggleMode = new Button { Text = "Toggle Mode" };
            btnToggleMode.Click += BtnMiniMode_Click;
            buttonPanel.Controls.Add(btnToggleMode);

            btnPickColor = new Button { Text = "Pick Background Color" };
            btnPickColor.Click += BtnPickColor_Click;
            buttonPanel.Controls.Add(btnPickColor);

            timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += Timer_Tick;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = Image.FromFile(ofd.FileName);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void BtnRemovePicture_Click(object sender, EventArgs e) => pictureBox.Image = null;
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (ParseTimeInput(txtTimeInput.Text, out TimeSpan duration))
            {
                endTime = DateTime.Now.Add(duration);
                timer.Start();
            }
            else
            {
                MessageBox.Show("Invalid time format");
            }
        }
        private void BtnStop_Click(object sender, EventArgs e) => timer.Stop();
        private void BtnReset_Click(object sender, EventArgs e)
        {
            timer.Stop();
            lblCountdown.Text = "00:00:00";
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            var remaining = endTime - DateTime.Now;
            lblCountdown.Text =
                remaining.TotalSeconds > 0 ? remaining.ToString(@"dd\:hh\:mm\:ss") : "Time's up!";
            if (remaining.TotalSeconds <= 0)
                timer.Stop();
        }

        private void BtnMiniMode_Click(object sender, EventArgs e)
        {
            isMiniMode = !isMiniMode;
            this.Size = isMiniMode ? new Size(200, 100) : new Size(600, 400);
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
            if (string.IsNullOrWhiteSpace(input))
                return false;
            try
            {
                int value = int.Parse(input[..^1]);
                char unit = input[^1];
                duration = unit switch
                {
                    'm' => TimeSpan.FromMinutes(value),
                    'h' => TimeSpan.FromHours(value),
                    'd' => TimeSpan.FromDays(value),
                    'w' => TimeSpan.FromDays(value * 7),
                    _ => throw new Exception("Invalid unit")
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new CountdownTimerForm());
        }
    }
}
