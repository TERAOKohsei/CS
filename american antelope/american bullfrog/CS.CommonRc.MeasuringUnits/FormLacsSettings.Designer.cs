namespace CS.CommonRc.MeasuringUnits {
    partial class FormLacsSettings {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if ( disposing && (components != null) ) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxPortName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelMeasuringUnitInformation = new System.Windows.Forms.Label();
            this.textBoxWaitTimeMs = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(216, 128);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 13;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonOKCancelApply_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(135, 128);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonOKCancelApply_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(54, 128);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 11;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOKCancelApply_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "9,600bps、CR+LF";
            // 
            // comboBoxPortName
            // 
            this.comboBoxPortName.FormattingEnabled = true;
            this.comboBoxPortName.Location = new System.Drawing.Point(75, 24);
            this.comboBoxPortName.Name = "comboBoxPortName";
            this.comboBoxPortName.Size = new System.Drawing.Size(216, 20);
            this.comboBoxPortName.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "通信ポート";
            // 
            // labelMeasuringUnitInformation
            // 
            this.labelMeasuringUnitInformation.AutoSize = true;
            this.labelMeasuringUnitInformation.Location = new System.Drawing.Point(12, 9);
            this.labelMeasuringUnitInformation.Name = "labelMeasuringUnitInformation";
            this.labelMeasuringUnitInformation.Size = new System.Drawing.Size(41, 12);
            this.labelMeasuringUnitInformation.TabIndex = 7;
            this.labelMeasuringUnitInformation.Text = "測定機";
            // 
            // textBoxWaitTimeMs
            // 
            this.textBoxWaitTimeMs.Location = new System.Drawing.Point(116, 77);
            this.textBoxWaitTimeMs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxWaitTimeMs.Name = "textBoxWaitTimeMs";
            this.textBoxWaitTimeMs.Size = new System.Drawing.Size(76, 19);
            this.textBoxWaitTimeMs.TabIndex = 14;
            this.textBoxWaitTimeMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxWaitTimeMs.Click += new System.EventHandler(this.textBoxWaitTimeMs_Click);
            this.textBoxWaitTimeMs.Enter += new System.EventHandler(this.textBoxWaitTimeMs_Enter);
            this.textBoxWaitTimeMs.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxWaitTimeMs_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "測定待ち時間[ms]";
            // 
            // FormLacsSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 163);
            this.Controls.Add(this.textBoxWaitTimeMs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxPortName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelMeasuringUnitInformation);
            this.Name = "FormLacsSettings";
            this.Text = "FormLacsSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxPortName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelMeasuringUnitInformation;
        private System.Windows.Forms.TextBox textBoxWaitTimeMs;
        private System.Windows.Forms.Label label1;
    }
}