using System;
using System.Drawing;
using System.Windows.Forms;

namespace CountdownTimerApp
{
    public class CountdownTimerForm : Form
    {
        private TextBox txtTimeInput;
        private TextBox txtEventName;
        private PictureBox pictureBox;
        private Label lblCountdown;
        private Button btnStart, btnStop, btnReset, btnMiniMode, btnRemovePicture, btnToggleMode, btnPickColor;
        private System.Windows.Forms.Timer timer;
        private DateTime endTime;
        private bool isMiniMode = false;

        public CountdownTimerForm()
        {
            // Initialize components
            this.Text = "Countdown";
            this.Size = new Size(600, 400);

            // Event Name
            txtEventName = new TextBox { PlaceholderText = "Event Name", Location = new Point(20, 20), Width = 200 };
            this.Controls.Add(txtEventName);

            // Time Input
            txtTimeInput = new TextBox { PlaceholderText = "Enter time (e.g., '7d', '3w', '2m', '30s')", Location = new Point(20, 60), Width = 200 };
            this.Controls.Add(txtTimeInput);

            // Picture Box
            pictureBox = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(300, 20),
                Size = new Size(100, 100),
                BackColor = Color.LightGray
            };
            pictureBox.Click += PictureBox_Click;
            this.Controls.Add(pictureBox);

            // Countdown Label
            lblCountdown = new Label
            {
                Text = "00:00:00",
                Font = new Font("Arial", 24),
                Location = new Point(20, 100),
                AutoSize = true
            };
            this.Controls.Add(lblCountdown);

            // Buttons
            btnStart = new Button { Text = "Start", Location = new Point(20, 150) };
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);

            btnStop = new Button { Text = "Stop", Location = new Point(100, 150) };
            btnStop.Click += BtnStop_Click;
            this.Controls.Add(btnStop);

            btnReset = new Button { Text = "Reset", Location = new Point(180, 150) };
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            btnMiniMode = new Button { Text = "Mini Mode", Location = new Point(260, 150) };
            btnMiniMode.Click += BtnMiniMode_Click;
            this.Controls.Add(btnMiniMode);

            btnRemovePicture = new Button { Text = "Remove Picture", Location = new Point(300, 200), Width = 150 };
            btnRemovePicture.Click += BtnRemovePicture_Click;
            this.Controls.Add(btnRemovePicture);

            // Toggle Mode Button (persistent)
            btnToggleMode = new Button
            {
                Text = "Toggle Mode",
                Location = new Point(20, 200),
                Width = 100
            };
            btnToggleMode.Click += BtnMiniMode_Click;
            this.Controls.Add(btnToggleMode);

            // Pick Background Color Button
            btnPickColor = new Button { 
                Text = "Pick Background Color", Location = new Point(130, 200), Width = 150 };
                btnPickColor.Click += BtnPickColor_Click;
                this.Controls.Add(btnPickColor);

            // Timer setup
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1-second interval
            timer.Tick += Timer_Tick; // Attach the Tick event to the Timer_Tick method
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

        private void BtnRemovePicture_Click(object sender, EventArgs e)
        {
            pictureBox.Image = null;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (ParseTimeInput(txtTimeInput.Text, out TimeSpan duration))
            {
                endTime = DateTime.Now.Add(duration);
                timer.Start();
            }
            else
            {
                MessageBox.Show("Please enter a valid time format (e.g., '7d', '3w', '2m', '30s').");
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            timer.Stop();
            lblCountdown.Text = "00:00:00";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var remaining = endTime - DateTime.Now;
            if (remaining.TotalSeconds > 0)
            {
                lblCountdown.Text = remaining.ToString(@"dd\:hh\:mm\:ss");
            }
            else
            {
                timer.Stop();
                lblCountdown.Text = "Time's up!";
            }
        }

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
                this.Size = new Size(400, 300);
                foreach (Control control in this.Controls)
                {
                    control.Visible = true;
                }
                lblCountdown.Location = new Point(20, 100);
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

