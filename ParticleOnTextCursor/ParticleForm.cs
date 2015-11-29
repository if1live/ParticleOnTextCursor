// http://www.codeproject.com/Articles/34520/Getting-Position-Inside-Any-Appli
//////////////////////////////////////////////////////////////////////////
//                                                                      //
//  Anybody can Use, Modify, Redistribute this code freely. If this     // 
//  module has been helpful to you then just leave a comment on Website //
//                                                                      //
//////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ParticleOnTextCursor
{
    public partial class ParticleForm : Form
    {
        public ParticleForm()
        {
            InitializeComponent();
            // http://stackoverflow.com/questions/4387680/transparent-background-on-winforms
            // Transparent background on winforms?
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;

            // http://stackoverflow.com/questions/7482922/remove-the-title-bar-in-windows-forms
            // Remove the title bar in Windows Forms
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            // http://stackoverflow.com/questions/505167/how-do-i-make-a-winforms-app-go-full-screen
            // How do I make a WinForms app go Full Screen
            this.WindowState = FormWindowState.Maximized;


            // Processing events from Hooks involves message queue complexities.
            // Timer has been used just to avoid that Mouse and Keyboard hooking                           
            // and to keep things simple. 
            timer1.Start();
            particleTimer.Start();

#if (!DEBUG)
            txtCaretX.Hide();
            txtCaretY.Hide();
            lblCurrentApp.Hide();            
#endif
        }

        #region Data Members & Structures 

        [StructLayout(LayoutKind.Sequential)]    // Required by user32.dll
        public struct RECT
        {
            public uint Left;
            public uint Top;
            public uint Right;
            public uint Bottom;
        };

        [StructLayout(LayoutKind.Sequential)]    // Required by user32.dll
        public struct GUITHREADINFO
        {
            public uint cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;

        };
        GUITHREADINFO guiInfo;                     // To store GUI Thread Information
        Point caretPosition;                     // To store Caret Position  
        String prevActiveProcess = "";

#endregion

#region winform always on top

        // http://www.c-sharpcorner.com/UploadFile/kirtan007/make-form-stay-always-on-top-of-every-window/
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private void Form1_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
#endregion


#region DllImports 


        /*- Retrieves Title Information of the specified window -*/
        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        /*- Retrieves Id of the thread that created the specified window -*/
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(int hWnd, out uint lpdwProcessId);

        /*- Retrieves information about active window or any specific GUI thread -*/
        [DllImport("user32.dll", EntryPoint = "GetGUIThreadInfo")]
        public static extern bool GetGUIThreadInfo(uint tId, out GUITHREADINFO threadInfo);

        /*- Retrieves Handle to the ForeGroundWindow -*/
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /*- Converts window specific point to screen specific -*/
        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, out Point position);


#endregion

#region Particle
        private ParticleEmitter emitter = new ParticleEmitter();

        private int tickCount = 0;
        private void RenderParticle(Graphics g)
        {
            int size = 4;
            List<Color> colors = emitter.AvailableColors;
            foreach(Color c in colors)
            {
                var allParticles = emitter.Particles;
                var particles = allParticles.FindAll(p => p.color == c);

                Brush brush = new SolidBrush(c);
                foreach(Particle p in particles)
                {
                    g.FillRectangle(brush, p.x, p.y, size, size);
                }
            }

#if DEBUG
            // for tick debuggging
            tickCount = (tickCount + 1) % 100;
            Pen myPen = new Pen(Color.DeepSkyBlue);
            myPen.Width = 5;
            g.DrawLine(myPen, 1, 1, tickCount, 1);
#endif
        }

#endregion


#region Event Handlers 
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RenderParticle(g);
        }

        private void particleTimer_Tick(object sender, EventArgs e)
        {
            emitter.Update(particleTimer.Interval);
            Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // If Tooltip window is active window (Suppose user clicks on the Tooltip Window)
            if (GetForegroundWindow() == this.Handle)
            {
                // then do no processing
                return;
            }

            // Get Current active Process
            string activeProcess = GetActiveProcess();

            // If window explorer is active window (eg. user has opened any drive)
            // Or for any failure when activeProcess is nothing               
            if ((activeProcess.ToLower().Contains("explorer") | (activeProcess == string.Empty)))
            {
                // Dissappear Tooltip
                this.Visible = false;
            }
            else
            {
                Point prevCaretPosition = new Point(caretPosition.X, caretPosition.Y);

                // Otherwise Calculate Caret position
                EvaluateCaretPosition();

                bool isParticleRequired = IsParticleRequired(prevActiveProcess, activeProcess, prevCaretPosition, caretPosition);
                if (isParticleRequired)
                {
                    // Adjust ToolTip according to the Caret
                    // 커서 위치에 맞춰서 창을 이동시킬 필요는 없을듯
                    //AdjustUI();
                    emitter.Emit(caretPosition.X - 25, caretPosition.Y - 35);
                }

                // Display current active Process on Tooltip
                lblCurrentApp.Text = activeProcess;
                this.Visible = true;
            }

            // 기존에 활성화되어있던 프로세스 이름 저장해두기
            // 창이 바뀌는 경우는 파티클 효과 무시하려고
            prevActiveProcess = activeProcess;
        }
#endregion

#region Methods 

        private bool IsParticleRequired(String prevActiveProcess, String currActiveProcess, Point prevCaretPosition, Point currCaretPosition)
        {
            if ((currActiveProcess.ToLower().Contains("explorer") | (currActiveProcess == string.Empty)))
            {
                return false;
            }
#if DEBUG
            if (currActiveProcess == "devenv")
            {
                // skip visual studio
                return false;
            }
#endif
            if (prevActiveProcess != currActiveProcess)
            {
                return false;
            }
            if (prevCaretPosition == currCaretPosition)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// This function will adjust Tooltip position and
        /// will keep it always inside the screen area.
        /// </summary>
        private void AdjustUI()
        {
            // Get Current Screen Resolution
            Rectangle workingArea = SystemInformation.WorkingArea;

            // If current caret position throws Tooltip outside of screen area
            // then do some UI adjustment.
            if (caretPosition.X + this.Width > workingArea.Width)
            {
                caretPosition.X = caretPosition.X - this.Width - 50;
            }

            if (caretPosition.Y + this.Height > workingArea.Height)
            {
                caretPosition.Y = caretPosition.Y - this.Height - 50;
            }

            this.Left = caretPosition.X;
            this.Top = caretPosition.Y;
        }

        /// <summary>
        /// Evaluates Cursor Position with respect to client screen.
        /// </summary>
        private void EvaluateCaretPosition()
        {
            caretPosition = new Point();

            // Fetch GUITHREADINFO
            GetCaretPosition();

            caretPosition.X = (int)guiInfo.rcCaret.Left + 25;
            caretPosition.Y = (int)guiInfo.rcCaret.Bottom + 25;

            ClientToScreen(guiInfo.hwndCaret, out caretPosition);

            txtCaretX.Text = (caretPosition.X).ToString();
            txtCaretY.Text = caretPosition.Y.ToString();

        }

        /// <summary>
        /// Get the caret position
        /// </summary>
        public void GetCaretPosition()
        {
            guiInfo = new GUITHREADINFO();
            guiInfo.cbSize = (uint)Marshal.SizeOf(guiInfo);

            // Get GuiThreadInfo into guiInfo
            GetGUIThreadInfo(0, out guiInfo);
        }

        /// <summary>
        /// Retrieves name of active Process.
        /// </summary>
        /// <returns>Active Process Name</returns>
        private string GetActiveProcess()
        {
            const int nChars = 256;
            int handle = 0;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = (int)GetForegroundWindow();

            // If Active window has some title info
            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                uint lpdwProcessId;
                uint dwCaretID = GetWindowThreadProcessId(handle, out lpdwProcessId);
                uint dwCurrentID = (uint)Thread.CurrentThread.ManagedThreadId;
                return Process.GetProcessById((int)lpdwProcessId).ProcessName;
            }
            // Otherwise either error or non client region
            return String.Empty;
        }

#endregion
    }
}
