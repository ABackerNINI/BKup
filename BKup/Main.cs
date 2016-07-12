using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BKup {
    public partial class Main : Form {

        private ArrayList entries;
        private Button[] buttons;
        private Panel panel;



        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);//置顶,超过任务栏

        public Main() {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e) {
            //
            // this
            //
            SetWindowPos(this.Handle, -1, 0, 0, 0, 0, 1 | 2);//置顶,超过任务栏
            this.BackColor = Color.MidnightBlue;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = new Size(Common.MAIN_WINDOW_WIDTH, Screen.PrimaryScreen.Bounds.Height);
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - Common.MAIN_WINDOW_WIDTH, 0);


            entries = BKup.ReadEntry(Application.StartupPath + Common.PATH_CONFIG);///////////////

            //
            // panel
            //
            this.panel = new Panel();
            this.panel.Font = new Font(Common.FONT, 13);
            this.panel.ForeColor = Color.White;
            this.panel.Location = new Point(0, 80);
            this.panel.Size = new Size(Common.MAIN_WINDOW_WIDTH, Math.Min(entries.Count * Common.BUTTON_HEIGHT, this.Height - 160));

            this.Controls.Add(this.panel);/////
            //
            // buttons
            //
            buttons = new Button[entries.Count];
            for (int i = 0; i < entries.Count; ++i) {
                buttons[i] = new Button();
                buttons[i].Name = i.ToString();
                buttons[i].AutoEllipsis = true;
                switch (((Common.Entry)entries[i]).status) {
                    case Common.Status.Paused:
                        buttons[i].BackColor = System.Drawing.Color.Gray;
                        break;
                    case Common.Status.SrcNotExists:
                        buttons[i].BackColor = System.Drawing.Color.Gray;
                        new ToolTip().SetToolTip(buttons[i], "备份项目不存在!");
                        break;
                }
                if (((Common.Entry)entries[i]).src_path.Length < 30)
                    buttons[i].Text = "   " + ((Common.Entry)entries[i]).src_path;
                else
                    buttons[i].Text = "   " + ((Common.Entry)entries[i]).src_path.Substring(0, 27) + "...";
                buttons[i].FlatStyle = FlatStyle.Flat;
                buttons[i].FlatAppearance.BorderSize = 0;
                buttons[i].FlatAppearance.BorderColor = System.Drawing.Color.MidnightBlue;
                buttons[i].TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                buttons[i].Location = new System.Drawing.Point(0, i * Common.BUTTON_HEIGHT);
                buttons[i].Size = new System.Drawing.Size(Common.MAIN_WINDOW_WIDTH, Common.BUTTON_HEIGHT);

                buttons[i].MouseEnter += Main_MouseEnter;
                buttons[i].MouseLeave += Main_MouseLeave;
                buttons[i].Click += Main_Click;

                panel.Controls.Add(buttons[i]);/////
            }
        }

        void Main_Click(object sender, EventArgs e) {
            if (!(sender is Button))
                return;

            this.panel.Size = new Size(Common.MAIN_WINDOW_WIDTH, Math.Min((entries.Count + 1) * Common.BUTTON_HEIGHT, this.Height - 160));

            Button b = (Button)sender;
            int index = Int32.Parse(b.Name);
        }

        void Main_MouseLeave(object sender, EventArgs e) {
            if (sender is Button) {
                Button b = (Button)sender;
                if (b.Size.Height == 50 && ((Common.Entry)entries[Int32.Parse(b.Name)]).status == Common.Status.Normal)
                    b.BackColor = System.Drawing.Color.MidnightBlue;
            }
        }

        void Main_MouseEnter(object sender, EventArgs e) {
            if (sender is Button) {
                Button b = (Button)sender;
                if (((Common.Entry)entries[Int32.Parse(b.Name)]).status == Common.Status.Normal) {
                    if (b.Size.Height == 50)
                        b.BackColor = System.Drawing.Color.SteelBlue;
                }

            }
        }

    }
}
