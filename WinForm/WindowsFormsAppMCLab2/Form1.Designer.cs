namespace WindowsFormsAppMCLab9
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.buttonOpenPort = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSlave1 = new System.Windows.Forms.TextBox();
            this.textBoxSlave2 = new System.Windows.Forms.TextBox();
            this.buttonSlave1 = new System.Windows.Forms.Button();
            this.buttonSlave2 = new System.Windows.Forms.Button();
            this.buttonClear1 = new System.Windows.Forms.Button();
            this.buttonClear2 = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(337, 31);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 33);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.Click += new System.EventHandler(this.comboBox1_Click);
            // 
            // buttonOpenPort
            // 
            this.buttonOpenPort.BackColor = System.Drawing.Color.White;
            this.buttonOpenPort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonOpenPort.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpenPort.Location = new System.Drawing.Point(473, 30);
            this.buttonOpenPort.Name = "buttonOpenPort";
            this.buttonOpenPort.Size = new System.Drawing.Size(122, 45);
            this.buttonOpenPort.TabIndex = 1;
            this.buttonOpenPort.Text = "Open";
            this.buttonOpenPort.UseVisualStyleBackColor = false;
            this.buttonOpenPort.Click += new System.EventHandler(this.buttonOpenPort_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(171, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 30);
            this.label1.TabIndex = 8;
            this.label1.Text = "COM port:";
            // 
            // textBoxSlave1
            // 
            this.textBoxSlave1.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSlave1.Location = new System.Drawing.Point(51, 197);
            this.textBoxSlave1.Multiline = true;
            this.textBoxSlave1.Name = "textBoxSlave1";
            this.textBoxSlave1.ReadOnly = true;
            this.textBoxSlave1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSlave1.Size = new System.Drawing.Size(305, 170);
            this.textBoxSlave1.TabIndex = 11;
            this.textBoxSlave1.Visible = false;
            // 
            // textBoxSlave2
            // 
            this.textBoxSlave2.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSlave2.Location = new System.Drawing.Point(437, 197);
            this.textBoxSlave2.Multiline = true;
            this.textBoxSlave2.Name = "textBoxSlave2";
            this.textBoxSlave2.ReadOnly = true;
            this.textBoxSlave2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSlave2.Size = new System.Drawing.Size(304, 170);
            this.textBoxSlave2.TabIndex = 12;
            this.textBoxSlave2.Visible = false;
            // 
            // buttonSlave1
            // 
            this.buttonSlave1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSlave1.Location = new System.Drawing.Point(85, 104);
            this.buttonSlave1.Name = "buttonSlave1";
            this.buttonSlave1.Size = new System.Drawing.Size(151, 54);
            this.buttonSlave1.TabIndex = 13;
            this.buttonSlave1.Text = "From slave 1";
            this.buttonSlave1.UseVisualStyleBackColor = true;
            this.buttonSlave1.Visible = false;
            this.buttonSlave1.Click += new System.EventHandler(this.buttonSlave1_Click);
            // 
            // buttonSlave2
            // 
            this.buttonSlave2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSlave2.Location = new System.Drawing.Point(544, 104);
            this.buttonSlave2.Name = "buttonSlave2";
            this.buttonSlave2.Size = new System.Drawing.Size(151, 54);
            this.buttonSlave2.TabIndex = 14;
            this.buttonSlave2.Text = "From slave 2";
            this.buttonSlave2.UseVisualStyleBackColor = true;
            this.buttonSlave2.Visible = false;
            this.buttonSlave2.Click += new System.EventHandler(this.buttonSlave2_Click);
            // 
            // buttonClear1
            // 
            this.buttonClear1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClear1.Location = new System.Drawing.Point(161, 389);
            this.buttonClear1.Name = "buttonClear1";
            this.buttonClear1.Size = new System.Drawing.Size(75, 43);
            this.buttonClear1.TabIndex = 15;
            this.buttonClear1.Text = "Clear";
            this.buttonClear1.UseVisualStyleBackColor = true;
            this.buttonClear1.Visible = false;
            this.buttonClear1.Click += new System.EventHandler(this.buttonClear1_Click);
            // 
            // buttonClear2
            // 
            this.buttonClear2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClear2.Location = new System.Drawing.Point(544, 389);
            this.buttonClear2.Name = "buttonClear2";
            this.buttonClear2.Size = new System.Drawing.Size(75, 43);
            this.buttonClear2.TabIndex = 16;
            this.buttonClear2.Text = "Clear";
            this.buttonClear2.UseVisualStyleBackColor = true;
            this.buttonClear2.Visible = false;
            this.buttonClear2.Click += new System.EventHandler(this.buttonClear2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(778, 444);
            this.Controls.Add(this.buttonClear2);
            this.Controls.Add(this.buttonClear1);
            this.Controls.Add(this.buttonSlave2);
            this.Controls.Add(this.buttonSlave1);
            this.Controls.Add(this.textBoxSlave2);
            this.Controls.Add(this.textBoxSlave1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOpenPort);
            this.Controls.Add(this.comboBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "Form1";
            this.Text = "Lab_9";
            this.TransparencyKey = System.Drawing.Color.Gray;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button buttonOpenPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSlave1;
        private System.Windows.Forms.TextBox textBoxSlave2;
        private System.Windows.Forms.Button buttonSlave1;
        private System.Windows.Forms.Button buttonSlave2;
        private System.Windows.Forms.Button buttonClear1;
        private System.Windows.Forms.Button buttonClear2;
        private System.IO.Ports.SerialPort serialPort1;
    }
}

