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
            this.btn_loadimage = new System.Windows.Forms.Button();
            this.btn_setVision = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_vision = new System.Windows.Forms.TextBox();
            this.btn_setHorizon = new System.Windows.Forms.Button();
            this.btn_clearFog = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_horizonDistance = new System.Windows.Forms.Button();
            this.btn_loadDepthmap = new System.Windows.Forms.Button();
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
            this.btn_multipleSignDetection = new System.Windows.Forms.Button();
            this.cBox_colorBased = new System.Windows.Forms.ComboBox();
            this.btn_signDetection = new System.Windows.Forms.Button();
            this.btn_undoEnhancement = new System.Windows.Forms.Button();
            this.btn_enhancement = new System.Windows.Forms.Button();
            this.cBox_enhancement = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_compareImages = new System.Windows.Forms.Button();
            this.txt_compare = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_loadGroundTruth = new System.Windows.Forms.Button();
            this.btn_loadMultipleImages = new System.Windows.Forms.Button();
            this.btn_showResults = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
            this.imageBox.Location = new System.Drawing.Point(261, 11);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(678, 400);
            this.imageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageBox.TabIndex = 2;
            this.imageBox.TabStop = false;
            this.imageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ib_fog_Paint);
            this.imageBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ib_fog_MouseDown);
            this.imageBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ib_fog_MouseMove);
            this.imageBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ib_fog_MouseUp);
            // 
            // btn_loadimage
            // 
            this.btn_loadimage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_loadimage.Location = new System.Drawing.Point(32, 48);
            this.btn_loadimage.Name = "btn_loadimage";
            this.btn_loadimage.Size = new System.Drawing.Size(196, 30);
            this.btn_loadimage.TabIndex = 3;
            this.btn_loadimage.Text = "Load Image + Depthmap";
            this.btn_loadimage.UseVisualStyleBackColor = true;
            this.btn_loadimage.Click += new System.EventHandler(this.btn_loadimage_Click);
            // 
            // btn_setVision
            // 
            this.btn_setVision.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_setVision.Location = new System.Drawing.Point(17, 144);
            this.btn_setVision.Name = "btn_setVision";
            this.btn_setVision.Size = new System.Drawing.Size(116, 30);
            this.btn_setVision.TabIndex = 4;
            this.btn_setVision.Text = "Set Visibility";
            this.btn_setVision.UseVisualStyleBackColor = true;
            this.btn_setVision.Click += new System.EventHandler(this.btn_setVision_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(191, 153);
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
            this.txt_vision.Location = new System.Drawing.Point(149, 153);
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
            this.btn_setHorizon.Location = new System.Drawing.Point(344, 139);
            this.btn_setHorizon.Name = "btn_setHorizon";
            this.btn_setHorizon.Size = new System.Drawing.Size(116, 30);
            this.btn_setHorizon.TabIndex = 11;
            this.btn_setHorizon.Text = "Set Horizon";
            this.btn_setHorizon.UseVisualStyleBackColor = true;
            this.btn_setHorizon.Visible = false;
            this.btn_setHorizon.Click += new System.EventHandler(this.btn_setHorizon_Click);
            // 
            // btn_clearFog
            // 
            this.btn_clearFog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_clearFog.Location = new System.Drawing.Point(17, 216);
            this.btn_clearFog.Name = "btn_clearFog";
            this.btn_clearFog.Size = new System.Drawing.Size(116, 30);
            this.btn_clearFog.TabIndex = 12;
            this.btn_clearFog.Text = "Clear Fog";
            this.btn_clearFog.UseVisualStyleBackColor = true;
            this.btn_clearFog.Click += new System.EventHandler(this.btn_clearFog_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_horizonDistance);
            this.groupBox1.Controls.Add(this.btn_loadDepthmap);
            this.groupBox1.Controls.Add(this.btn_addNoise);
            this.groupBox1.Controls.Add(this.txt_skylevel);
            this.groupBox1.Controls.Add(this.btn_setSkylevel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btn_clearFog);
            this.groupBox1.Controls.Add(this.txt_vision);
            this.groupBox1.Controls.Add(this.txt_horizon);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_setVision);
            this.groupBox1.Location = new System.Drawing.Point(15, 123);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 253);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fog";
            // 
            // btn_horizonDistance
            // 
            this.btn_horizonDistance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_horizonDistance.Location = new System.Drawing.Point(17, 57);
            this.btn_horizonDistance.Name = "btn_horizonDistance";
            this.btn_horizonDistance.Size = new System.Drawing.Size(116, 45);
            this.btn_horizonDistance.TabIndex = 32;
            this.btn_horizonDistance.Text = "Set Horizon Distance";
            this.btn_horizonDistance.UseVisualStyleBackColor = true;
            this.btn_horizonDistance.Click += new System.EventHandler(this.btn_horizonDistance_Click);
            // 
            // btn_loadDepthmap
            // 
            this.btn_loadDepthmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_loadDepthmap.Location = new System.Drawing.Point(17, 19);
            this.btn_loadDepthmap.Name = "btn_loadDepthmap";
            this.btn_loadDepthmap.Size = new System.Drawing.Size(196, 32);
            this.btn_loadDepthmap.TabIndex = 31;
            this.btn_loadDepthmap.Text = "Load Depthmap";
            this.btn_loadDepthmap.UseVisualStyleBackColor = true;
            this.btn_loadDepthmap.Click += new System.EventHandler(this.btn_loadDepthmap_Click);
            // 
            // btn_addNoise
            // 
            this.btn_addNoise.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_addNoise.Location = new System.Drawing.Point(17, 180);
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
            this.txt_skylevel.Location = new System.Drawing.Point(149, 117);
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
            this.btn_setSkylevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_setSkylevel.Location = new System.Drawing.Point(17, 108);
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
            this.label3.Location = new System.Drawing.Point(191, 76);
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
            this.txt_horizon.Location = new System.Drawing.Point(149, 76);
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
            this.groupBox2.Location = new System.Drawing.Point(344, 188);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 140);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Vertical Objects";
            this.groupBox2.Visible = false;
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
            this.groupBox3.Controls.Add(this.btn_multipleSignDetection);
            this.groupBox3.Controls.Add(this.cBox_colorBased);
            this.groupBox3.Controls.Add(this.btn_signDetection);
            this.groupBox3.Location = new System.Drawing.Point(947, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(230, 140);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Traffic Sign Detection";
            // 
            // btn_multipleSignDetection
            // 
            this.btn_multipleSignDetection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_multipleSignDetection.Location = new System.Drawing.Point(17, 89);
            this.btn_multipleSignDetection.Name = "btn_multipleSignDetection";
            this.btn_multipleSignDetection.Size = new System.Drawing.Size(196, 30);
            this.btn_multipleSignDetection.TabIndex = 24;
            this.btn_multipleSignDetection.Text = "Start [400,100]";
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
            this.btn_signDetection.Location = new System.Drawing.Point(17, 53);
            this.btn_signDetection.Name = "btn_signDetection";
            this.btn_signDetection.Size = new System.Drawing.Size(196, 30);
            this.btn_signDetection.TabIndex = 22;
            this.btn_signDetection.Text = "Start";
            this.btn_signDetection.UseVisualStyleBackColor = true;
            this.btn_signDetection.Click += new System.EventHandler(this.btn_signDetection_Click);
            // 
            // btn_undoEnhancement
            // 
            this.btn_undoEnhancement.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_undoEnhancement.Location = new System.Drawing.Point(817, 217);
            this.btn_undoEnhancement.Name = "btn_undoEnhancement";
            this.btn_undoEnhancement.Size = new System.Drawing.Size(77, 30);
            this.btn_undoEnhancement.TabIndex = 29;
            this.btn_undoEnhancement.Text = "Undo";
            this.btn_undoEnhancement.UseVisualStyleBackColor = true;
            this.btn_undoEnhancement.Visible = false;
            this.btn_undoEnhancement.Click += new System.EventHandler(this.btn_undoEnhancement_Click);
            // 
            // btn_enhancement
            // 
            this.btn_enhancement.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_enhancement.Location = new System.Drawing.Point(674, 217);
            this.btn_enhancement.Name = "btn_enhancement";
            this.btn_enhancement.Size = new System.Drawing.Size(141, 30);
            this.btn_enhancement.TabIndex = 28;
            this.btn_enhancement.Text = "Start";
            this.btn_enhancement.UseVisualStyleBackColor = true;
            this.btn_enhancement.Visible = false;
            this.btn_enhancement.Click += new System.EventHandler(this.btn_enhancement_Click);
            // 
            // cBox_enhancement
            // 
            this.cBox_enhancement.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBox_enhancement.FormattingEnabled = true;
            this.cBox_enhancement.Location = new System.Drawing.Point(674, 183);
            this.cBox_enhancement.Name = "cBox_enhancement";
            this.cBox_enhancement.Size = new System.Drawing.Size(220, 28);
            this.cBox_enhancement.TabIndex = 27;
            this.cBox_enhancement.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(670, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 20);
            this.label4.TabIndex = 26;
            this.label4.Text = "Enhancement:";
            this.label4.Visible = false;
            // 
            // btn_compareImages
            // 
            this.btn_compareImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_compareImages.Location = new System.Drawing.Point(15, 382);
            this.btn_compareImages.Name = "btn_compareImages";
            this.btn_compareImages.Size = new System.Drawing.Size(151, 26);
            this.btn_compareImages.TabIndex = 30;
            this.btn_compareImages.Text = "Compare with original Image";
            this.btn_compareImages.UseVisualStyleBackColor = true;
            this.btn_compareImages.Click += new System.EventHandler(this.btn_compareImages_Click);
            // 
            // txt_compare
            // 
            this.txt_compare.BackColor = System.Drawing.SystemColors.Window;
            this.txt_compare.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_compare.Enabled = false;
            this.txt_compare.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_compare.Location = new System.Drawing.Point(172, 384);
            this.txt_compare.MaxLength = 3;
            this.txt_compare.Name = "txt_compare";
            this.txt_compare.ReadOnly = true;
            this.txt_compare.Size = new System.Drawing.Size(55, 19);
            this.txt_compare.TabIndex = 18;
            this.txt_compare.Text = "0";
            this.txt_compare.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(232, 384);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 20);
            this.label5.TabIndex = 18;
            this.label5.Text = "%";
            // 
            // btn_loadGroundTruth
            // 
            this.btn_loadGroundTruth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_loadGroundTruth.Location = new System.Drawing.Point(32, 12);
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
            this.btn_loadMultipleImages.Location = new System.Drawing.Point(32, 84);
            this.btn_loadMultipleImages.Name = "btn_loadMultipleImages";
            this.btn_loadMultipleImages.Size = new System.Drawing.Size(196, 30);
            this.btn_loadMultipleImages.TabIndex = 34;
            this.btn_loadMultipleImages.Text = "Load Multiple Images";
            this.btn_loadMultipleImages.UseVisualStyleBackColor = true;
            this.btn_loadMultipleImages.Click += new System.EventHandler(this.btn_loadMultipleImages_Click);
            // 
            // btn_showResults
            // 
            this.btn_showResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_showResults.Location = new System.Drawing.Point(964, 180);
            this.btn_showResults.Name = "btn_showResults";
            this.btn_showResults.Size = new System.Drawing.Size(196, 30);
            this.btn_showResults.TabIndex = 25;
            this.btn_showResults.Text = "Show Results";
            this.btn_showResults.UseVisualStyleBackColor = true;
            this.btn_showResults.Click += new System.EventHandler(this.btn_showResults_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1189, 420);
            this.Controls.Add(this.btn_showResults);
            this.Controls.Add(this.btn_loadMultipleImages);
            this.Controls.Add(this.btn_undoEnhancement);
            this.Controls.Add(this.btn_loadGroundTruth);
            this.Controls.Add(this.btn_enhancement);
            this.Controls.Add(this.cBox_enhancement);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txt_compare);
            this.Controls.Add(this.btn_compareImages);
            this.Controls.Add(this.btn_setHorizon);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_loadimage);
            this.Controls.Add(this.imageBox);
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Foggy";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox;
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
        private System.Windows.Forms.ComboBox cBox_colorBased;
        private System.Windows.Forms.Button btn_enhancement;
        private System.Windows.Forms.ComboBox cBox_enhancement;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_undoEnhancement;
        private System.Windows.Forms.Button btn_compareImages;
        private System.Windows.Forms.TextBox txt_compare;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_loadDepthmap;
        private System.Windows.Forms.Button btn_horizonDistance;
        private System.Windows.Forms.Button btn_loadGroundTruth;
        private System.Windows.Forms.Button btn_multipleSignDetection;
        private System.Windows.Forms.Button btn_loadMultipleImages;
        private System.Windows.Forms.Button btn_showResults;
    }
}

