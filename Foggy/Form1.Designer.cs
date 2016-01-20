namespace Foggy
{
    partial class Form1
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
            this.ib_fog = new Emgu.CV.UI.ImageBox();
            this.btn_loadimage = new System.Windows.Forms.Button();
            this.btn_setVision = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_vision = new System.Windows.Forms.TextBox();
            this.btn_setHorizon = new System.Windows.Forms.Button();
            this.btn_clearFog = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_addNoise = new System.Windows.Forms.Button();
            this.txt_skylevel = new System.Windows.Forms.TextBox();
            this.btn_setSkylevel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_horizon = new System.Windows.Forms.TextBox();
            this.btn_newObject = new System.Windows.Forms.Button();
            this.btn_objectsDone = new System.Windows.Forms.Button();
            this.btn_superpixels = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_saveObject = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cBox_colorBased = new System.Windows.Forms.ComboBox();
            this.btn_signDetection = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ib_fog)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ib_fog
            // 
            this.ib_fog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ib_fog.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
            this.ib_fog.Location = new System.Drawing.Point(251, 12);
            this.ib_fog.Name = "ib_fog";
            this.ib_fog.Size = new System.Drawing.Size(678, 400);
            this.ib_fog.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ib_fog.TabIndex = 2;
            this.ib_fog.TabStop = false;
            this.ib_fog.Paint += new System.Windows.Forms.PaintEventHandler(this.ib_fog_Paint);
            this.ib_fog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ib_fog_MouseDown);
            this.ib_fog.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ib_fog_MouseMove);
            this.ib_fog.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ib_fog_MouseUp);
            // 
            // btn_loadimage
            // 
            this.btn_loadimage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_loadimage.Location = new System.Drawing.Point(15, 12);
            this.btn_loadimage.Name = "btn_loadimage";
            this.btn_loadimage.Size = new System.Drawing.Size(219, 39);
            this.btn_loadimage.TabIndex = 3;
            this.btn_loadimage.Text = "Load Image";
            this.btn_loadimage.UseVisualStyleBackColor = true;
            this.btn_loadimage.Click += new System.EventHandler(this.btn_loadimage_Click);
            // 
            // btn_setVision
            // 
            this.btn_setVision.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_setVision.Location = new System.Drawing.Point(17, 22);
            this.btn_setVision.Name = "btn_setVision";
            this.btn_setVision.Size = new System.Drawing.Size(116, 30);
            this.btn_setVision.TabIndex = 4;
            this.btn_setVision.Text = "Set Vision";
            this.btn_setVision.UseVisualStyleBackColor = true;
            this.btn_setVision.Click += new System.EventHandler(this.btn_setVision_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(191, 27);
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
            this.txt_vision.Location = new System.Drawing.Point(149, 27);
            this.txt_vision.MaxLength = 3;
            this.txt_vision.Name = "txt_vision";
            this.txt_vision.ReadOnly = true;
            this.txt_vision.Size = new System.Drawing.Size(36, 19);
            this.txt_vision.TabIndex = 8;
            this.txt_vision.Text = "0";
            this.txt_vision.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btn_setHorizon
            // 
            this.btn_setHorizon.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_setHorizon.Location = new System.Drawing.Point(17, 58);
            this.btn_setHorizon.Name = "btn_setHorizon";
            this.btn_setHorizon.Size = new System.Drawing.Size(116, 30);
            this.btn_setHorizon.TabIndex = 11;
            this.btn_setHorizon.Text = "Set Horizon";
            this.btn_setHorizon.UseVisualStyleBackColor = true;
            this.btn_setHorizon.Click += new System.EventHandler(this.btn_setHorizon_Click);
            // 
            // btn_clearFog
            // 
            this.btn_clearFog.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_clearFog.Location = new System.Drawing.Point(17, 165);
            this.btn_clearFog.Name = "btn_clearFog";
            this.btn_clearFog.Size = new System.Drawing.Size(116, 30);
            this.btn_clearFog.TabIndex = 12;
            this.btn_clearFog.Text = "Clear Fog";
            this.btn_clearFog.UseVisualStyleBackColor = true;
            this.btn_clearFog.Click += new System.EventHandler(this.btn_clearFog_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_addNoise);
            this.groupBox1.Controls.Add(this.txt_skylevel);
            this.groupBox1.Controls.Add(this.btn_setSkylevel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txt_horizon);
            this.groupBox1.Controls.Add(this.btn_setHorizon);
            this.groupBox1.Controls.Add(this.btn_clearFog);
            this.groupBox1.Controls.Add(this.txt_vision);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_setVision);
            this.groupBox1.Location = new System.Drawing.Point(15, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 209);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fog";
            // 
            // btn_addNoise
            // 
            this.btn_addNoise.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_addNoise.Location = new System.Drawing.Point(17, 130);
            this.btn_addNoise.Name = "btn_addNoise";
            this.btn_addNoise.Size = new System.Drawing.Size(116, 30);
            this.btn_addNoise.TabIndex = 17;
            this.btn_addNoise.Text = "Add Noise";
            this.btn_addNoise.UseVisualStyleBackColor = true;
            this.btn_addNoise.Click += new System.EventHandler(this.btn_addNoise_Click);
            // 
            // txt_skylevel
            // 
            this.txt_skylevel.BackColor = System.Drawing.SystemColors.Window;
            this.txt_skylevel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_skylevel.Enabled = false;
            this.txt_skylevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_skylevel.Location = new System.Drawing.Point(149, 99);
            this.txt_skylevel.MaxLength = 4;
            this.txt_skylevel.Name = "txt_skylevel";
            this.txt_skylevel.ReadOnly = true;
            this.txt_skylevel.Size = new System.Drawing.Size(36, 19);
            this.txt_skylevel.TabIndex = 16;
            this.txt_skylevel.Text = "0";
            this.txt_skylevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btn_setSkylevel
            // 
            this.btn_setSkylevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_setSkylevel.Location = new System.Drawing.Point(17, 94);
            this.btn_setSkylevel.Name = "btn_setSkylevel";
            this.btn_setSkylevel.Size = new System.Drawing.Size(116, 30);
            this.btn_setSkylevel.TabIndex = 15;
            this.btn_setSkylevel.Text = "Set Skylevel";
            this.btn_setSkylevel.UseVisualStyleBackColor = true;
            this.btn_setSkylevel.Click += new System.EventHandler(this.btn_setSkylevel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(191, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "m";
            // 
            // txt_horizon
            // 
            this.txt_horizon.BackColor = System.Drawing.SystemColors.Window;
            this.txt_horizon.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_horizon.Enabled = false;
            this.txt_horizon.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_horizon.Location = new System.Drawing.Point(149, 64);
            this.txt_horizon.MaxLength = 3;
            this.txt_horizon.Name = "txt_horizon";
            this.txt_horizon.ReadOnly = true;
            this.txt_horizon.Size = new System.Drawing.Size(36, 19);
            this.txt_horizon.TabIndex = 13;
            this.txt_horizon.Text = "0";
            this.txt_horizon.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btn_newObject
            // 
            this.btn_newObject.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_newObject.Location = new System.Drawing.Point(17, 55);
            this.btn_newObject.Name = "btn_newObject";
            this.btn_newObject.Size = new System.Drawing.Size(116, 30);
            this.btn_newObject.TabIndex = 20;
            this.btn_newObject.Text = "New Object";
            this.btn_newObject.UseVisualStyleBackColor = true;
            this.btn_newObject.Click += new System.EventHandler(this.btn_newObject_Click);
            // 
            // btn_objectsDone
            // 
            this.btn_objectsDone.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_objectsDone.Location = new System.Drawing.Point(17, 91);
            this.btn_objectsDone.Name = "btn_objectsDone";
            this.btn_objectsDone.Size = new System.Drawing.Size(196, 30);
            this.btn_objectsDone.TabIndex = 19;
            this.btn_objectsDone.Text = "Done";
            this.btn_objectsDone.UseVisualStyleBackColor = true;
            this.btn_objectsDone.Click += new System.EventHandler(this.btn_objectsDone_Click);
            // 
            // btn_superpixels
            // 
            this.btn_superpixels.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_superpixels.Location = new System.Drawing.Point(17, 19);
            this.btn_superpixels.Name = "btn_superpixels";
            this.btn_superpixels.Size = new System.Drawing.Size(196, 30);
            this.btn_superpixels.TabIndex = 18;
            this.btn_superpixels.Text = "Analyse Regions";
            this.btn_superpixels.UseVisualStyleBackColor = true;
            this.btn_superpixels.Click += new System.EventHandler(this.superpixels_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_saveObject);
            this.groupBox2.Controls.Add(this.btn_superpixels);
            this.groupBox2.Controls.Add(this.btn_newObject);
            this.groupBox2.Controls.Add(this.btn_objectsDone);
            this.groupBox2.Location = new System.Drawing.Point(15, 272);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(219, 140);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Verticals";
            // 
            // btn_saveObject
            // 
            this.btn_saveObject.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_saveObject.Location = new System.Drawing.Point(139, 55);
            this.btn_saveObject.Name = "btn_saveObject";
            this.btn_saveObject.Size = new System.Drawing.Size(74, 30);
            this.btn_saveObject.TabIndex = 21;
            this.btn_saveObject.Text = "Save";
            this.btn_saveObject.UseVisualStyleBackColor = true;
            this.btn_saveObject.Click += new System.EventHandler(this.btn_saveObject_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.cBox_colorBased);
            this.groupBox3.Controls.Add(this.btn_signDetection);
            this.groupBox3.Location = new System.Drawing.Point(945, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(240, 133);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Traffic Sign Detection";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 24;
            this.label2.Text = "Color Based:";
            // 
            // cBox_colorBased
            // 
            this.cBox_colorBased.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBox_colorBased.FormattingEnabled = true;
            this.cBox_colorBased.Location = new System.Drawing.Point(10, 47);
            this.cBox_colorBased.Name = "cBox_colorBased";
            this.cBox_colorBased.Size = new System.Drawing.Size(224, 28);
            this.cBox_colorBased.TabIndex = 23;
            // 
            // btn_signDetection
            // 
            this.btn_signDetection.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_signDetection.Location = new System.Drawing.Point(10, 81);
            this.btn_signDetection.Name = "btn_signDetection";
            this.btn_signDetection.Size = new System.Drawing.Size(224, 30);
            this.btn_signDetection.TabIndex = 22;
            this.btn_signDetection.Text = "Start";
            this.btn_signDetection.UseVisualStyleBackColor = true;
            this.btn_signDetection.Click += new System.EventHandler(this.btn_signDetection_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1199, 424);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_loadimage);
            this.Controls.Add(this.ib_fog);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ib_fog)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox ib_fog;
        private System.Windows.Forms.Button btn_loadimage;
        private System.Windows.Forms.Button btn_setVision;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_vision;
        private System.Windows.Forms.Button btn_setHorizon;
        private System.Windows.Forms.Button btn_clearFog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_horizon;
        private System.Windows.Forms.TextBox txt_skylevel;
        private System.Windows.Forms.Button btn_setSkylevel;
        private System.Windows.Forms.Button btn_addNoise;
        private System.Windows.Forms.Button btn_superpixels;
        private System.Windows.Forms.Button btn_objectsDone;
        private System.Windows.Forms.Button btn_newObject;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_saveObject;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_signDetection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cBox_colorBased;
    }
}

