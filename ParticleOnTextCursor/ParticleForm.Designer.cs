namespace ParticleOnTextCursor
{
    partial class ParticleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtCaretX = new System.Windows.Forms.TextBox();
            this.txtCaretY = new System.Windows.Forms.TextBox();
            this.lblCurrentApp = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.particleTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // txtCaretX
            // 
            this.txtCaretX.Location = new System.Drawing.Point(13, 13);
            this.txtCaretX.Name = "txtCaretX";
            this.txtCaretX.Size = new System.Drawing.Size(157, 21);
            this.txtCaretX.TabIndex = 0;
            // 
            // txtCaretY
            // 
            this.txtCaretY.Location = new System.Drawing.Point(13, 41);
            this.txtCaretY.Name = "txtCaretY";
            this.txtCaretY.Size = new System.Drawing.Size(157, 21);
            this.txtCaretY.TabIndex = 1;
            // 
            // lblCurrentApp
            // 
            this.lblCurrentApp.Location = new System.Drawing.Point(13, 69);
            this.lblCurrentApp.Name = "lblCurrentApp";
            this.lblCurrentApp.Size = new System.Drawing.Size(157, 21);
            this.lblCurrentApp.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // particleTimer
            // 
            this.particleTimer.Interval = 20;
            this.particleTimer.Tick += new System.EventHandler(this.particleTimer_Tick);
            // 
            // ParticleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(182, 125);
            this.Controls.Add(this.lblCurrentApp);
            this.Controls.Add(this.txtCaretY);
            this.Controls.Add(this.txtCaretX);
            this.Name = "ParticleForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCaretX;
        private System.Windows.Forms.TextBox txtCaretY;
        private System.Windows.Forms.TextBox lblCurrentApp;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer particleTimer;
    }
}

