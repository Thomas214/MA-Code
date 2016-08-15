namespace Foggy
{
    partial class mainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageBox = new Emgu.CV.UI.ImageBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_vision = new System.Windows.Forms.TextBox();
            this.btn_clearFog = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxK = new System.Windows.Forms.CheckBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.checkBoxSky = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_multipleSignDetection = new System.Windows.Forms.Button();
            this.cBox_colorBased = new System.Windows.Forms.ComboBox();
            this.btn_signDetection = new System.Windows.Forms.Button();
            this.btn_showResults = new System.Windows.Forms.Button();
            this.btn_loadGroundTruth = new System.Windows.Forms.Button();
            this.btn_loadMultipleImages = new System.Windows.Forms.Button();
            this.btn_previous = new System.Windows.Forms.Button();
            this.btn_next = new System.Windows.Forms.Button();
            this.label_imageNrLeft = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label_imageNrRight = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
            this.imageBox.Location = new System.Drawing.Point(263, 20);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(678, 400);
            this.imageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageBox.TabIndex = 2;
            this.imageBox.TabStop = false;
            this.imageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ib_fog_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(191, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "m";
            // 
            // txt_vision
            // 
            this.txt_vision.BackColor = System.Drawing.SystemColors.Window;
            this.txt_vision.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_vision.Enabled = false;
            this.txt_vision.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_vision.Location = new System.Drawing.Point(163, 22);
            this.txt_vision.MaxLength = 3;
            this.txt_vision.Name = "txt_vision";
            this.txt_vision.ReadOnly = true;
            this.txt_vision.Size = new System.Drawing.Size(27, 19);
            this.txt_vision.TabIndex = 8;
            this.txt_vision.Text = "∞";
            this.txt_vision.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btn_clearFog
            // 
            this.btn_clearFog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_clearFog.Location = new System.Drawing.Point(145, 70);
            this.btn_clearFog.Name = "btn_clearFog";
            this.btn_clearFog.Size = new System.Drawing.Size(68, 30);
            this.btn_clearFog.TabIndex = 12;
            this.btn_clearFog.Text = "No Fog";
            this.btn_clearFog.UseVisualStyleBackColor = true;
            this.btn_clearFog.Click += new System.EventHandler(this.btn_clearFog_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_clearFog);
            this.groupBox1.Controls.Add(this.checkBoxK);
            this.groupBox1.Controls.Add(this.txt_vision);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.checkBoxSky);
            this.groupBox1.Location = new System.Drawing.Point(15, 111);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 109);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fog Augmentation";
            // 
            // checkBoxK
            // 
            this.checkBoxK.AutoSize = true;
            this.checkBoxK.Location = new System.Drawing.Point(16, 60);
            this.checkBoxK.Name = "checkBoxK";
            this.checkBoxK.Size = new System.Drawing.Size(123, 17);
            this.checkBoxK.TabIndex = 38;
            this.checkBoxK.Text = "Fog Extinction Noise";
            this.checkBoxK.UseVisualStyleBackColor = true;
            this.checkBoxK.CheckedChanged += new System.EventHandler(this.checkBoxK_CheckedChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 50;
            this.trackBar1.Location = new System.Drawing.Point(6, 19);
            this.trackBar1.Maximum = 500;
            this.trackBar1.Minimum = 50;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(151, 45);
            this.trackBar1.TabIndex = 40;
            this.trackBar1.TickFrequency = 50;
            this.trackBar1.Value = 500;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.trackBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // checkBoxSky
            // 
            this.checkBoxSky.AutoSize = true;
            this.checkBoxSky.Location = new System.Drawing.Point(16, 83);
            this.checkBoxSky.Name = "checkBoxSky";
            this.checkBoxSky.Size = new System.Drawing.Size(104, 17);
            this.checkBoxSky.TabIndex = 39;
            this.checkBoxSky.Text = "Sky Value Noise";
            this.checkBoxSky.UseVisualStyleBackColor = true;
            this.checkBoxSky.CheckedChanged += new System.EventHandler(this.checkBoxSky_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_multipleSignDetection);
            this.groupBox3.Controls.Add(this.cBox_colorBased);
            this.groupBox3.Controls.Add(this.btn_signDetection);
            this.groupBox3.Location = new System.Drawing.Point(15, 226);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(230, 126);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Traffic Sign Detection";
            // 
            // btn_multipleSignDetection
            // 
            this.btn_multipleSignDetection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_multipleSignDetection.Location = new System.Drawing.Point(17, 87);
            this.btn_multipleSignDetection.Name = "btn_multipleSignDetection";
            this.btn_multipleSignDetection.Size = new System.Drawing.Size(196, 30);
            this.btn_multipleSignDetection.TabIndex = 24;
            this.btn_multipleSignDetection.Text = "Detect in ALL Images";
            this.btn_multipleSignDetection.UseVisualStyleBackColor = true;
            this.btn_multipleSignDetection.Click += new System.EventHandler(this.btn_multipleSignDetection_Click);
            // 
            // cBox_colorBased
            // 
            this.cBox_colorBased.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBox_colorBased.FormattingEnabled = true;
            this.cBox_colorBased.Location = new System.Drawing.Point(17, 19);
            this.cBox_colorBased.Name = "cBox_colorBased";
            this.cBox_colorBased.Size = new System.Drawing.Size(196, 28);
            this.cBox_colorBased.TabIndex = 23;
            // 
            // btn_signDetection
            // 
            this.btn_signDetection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_signDetection.Location = new System.Drawing.Point(17, 52);
            this.btn_signDetection.Name = "btn_signDetection";
            this.btn_signDetection.Size = new System.Drawing.Size(196, 30);
            this.btn_signDetection.TabIndex = 22;
            this.btn_signDetection.Text = "Detect in current Image";
            this.btn_signDetection.UseVisualStyleBackColor = true;
            this.btn_signDetection.Click += new System.EventHandler(this.btn_signDetection_Click);
            // 
            // btn_showResults
            // 
            this.btn_showResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_showResults.Location = new System.Drawing.Point(17, 19);
            this.btn_showResults.Name = "btn_showResults";
            this.btn_showResults.Size = new System.Drawing.Size(196, 30);
            this.btn_showResults.TabIndex = 25;
            this.btn_showResults.Text = "Show Results";
            this.btn_showResults.UseVisualStyleBackColor = true;
            this.btn_showResults.Click += new System.EventHandler(this.btn_showResults_Click);
            // 
            // btn_loadGroundTruth
            // 
            this.btn_loadGroundTruth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_loadGroundTruth.Location = new System.Drawing.Point(17, 19);
            this.btn_loadGroundTruth.Name = "btn_loadGroundTruth";
            this.btn_loadGroundTruth.Size = new System.Drawing.Size(196, 30);
            this.btn_loadGroundTruth.TabIndex = 32;
            this.btn_loadGroundTruth.Text = "Load Ground Truth";
            this.btn_loadGroundTruth.UseVisualStyleBackColor = true;
            this.btn_loadGroundTruth.Click += new System.EventHandler(this.btn_loadGroundTruth_Click);
            // 
            // btn_loadMultipleImages
            // 
            this.btn_loadMultipleImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_loadMultipleImages.Location = new System.Drawing.Point(17, 55);
            this.btn_loadMultipleImages.Name = "btn_loadMultipleImages";
            this.btn_loadMultipleImages.Size = new System.Drawing.Size(196, 30);
            this.btn_loadMultipleImages.TabIndex = 34;
            this.btn_loadMultipleImages.Text = "Load Images";
            this.btn_loadMultipleImages.UseVisualStyleBackColor = true;
            this.btn_loadMultipleImages.Click += new System.EventHandler(this.btn_loadMultipleImages_Click);
            // 
            // btn_previous
            // 
            this.btn_previous.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_previous.Location = new System.Drawing.Point(510, 425);
            this.btn_previous.Name = "btn_previous";
            this.btn_previous.Size = new System.Drawing.Size(43, 25);
            this.btn_previous.TabIndex = 35;
            this.btn_previous.Text = "<";
            this.btn_previous.UseVisualStyleBackColor = true;
            this.btn_previous.Click += new System.EventHandler(this.btn_previous_Click);
            // 
            // btn_next
            // 
            this.btn_next.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_next.Location = new System.Drawing.Point(644, 425);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(43, 25);
            this.btn_next.TabIndex = 36;
            this.btn_next.Text = ">";
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Click += new System.EventHandler(this.btn_next_Click);
            // 
            // label_imageNrLeft
            // 
            this.label_imageNrLeft.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label_imageNrLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_imageNrLeft.Location = new System.Drawing.Point(557, 428);
            this.label_imageNrLeft.Name = "label_imageNrLeft";
            this.label_imageNrLeft.Size = new System.Drawing.Size(39, 20);
            this.label_imageNrLeft.TabIndex = 37;
            this.label_imageNrLeft.Text = "0";
            this.label_imageNrLeft.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_loadGroundTruth);
            this.groupBox4.Controls.Add(this.btn_loadMultipleImages);
            this.groupBox4.Location = new System.Drawing.Point(15, 11);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(230, 94);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Loading Files";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btn_showResults);
            this.groupBox5.Location = new System.Drawing.Point(15, 358);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(230, 59);
            this.groupBox5.TabIndex = 35;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Results";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(593, 428);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 20);
            this.label2.TabIndex = 41;
            this.label2.Text = "/";
            // 
            // label_imageNrRight
            // 
            this.label_imageNrRight.AutoSize = true;
            this.label_imageNrRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_imageNrRight.Location = new System.Drawing.Point(603, 428);
            this.label_imageNrRight.Name = "label_imageNrRight";
            this.label_imageNrRight.Size = new System.Drawing.Size(18, 20);
            this.label_imageNrRight.TabIndex = 42;
            this.label_imageNrRight.Text = "0";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 455);
            this.Controls.Add(this.label_imageNrRight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label_imageNrLeft);
            this.Controls.Add(this.btn_next);
            this.Controls.Add(this.btn_previous);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.imageBox);
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Foggy";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_vision;
        private System.Windows.Forms.Button btn_clearFog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_signDetection;
        private System.Windows.Forms.ComboBox cBox_colorBased;
        private System.Windows.Forms.Button btn_loadGroundTruth;
        private System.Windows.Forms.Button btn_multipleSignDetection;
        private System.Windows.Forms.Button btn_loadMultipleImages;
        private System.Windows.Forms.Button btn_showResults;
        private System.Windows.Forms.Button btn_previous;
        private System.Windows.Forms.Button btn_next;
        private System.Windows.Forms.Label label_imageNrLeft;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBoxK;
        private System.Windows.Forms.CheckBox checkBoxSky;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_imageNrRight;
    }
}

