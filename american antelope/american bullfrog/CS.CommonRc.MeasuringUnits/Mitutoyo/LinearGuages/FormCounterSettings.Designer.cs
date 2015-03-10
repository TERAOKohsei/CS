namespace CS.CommonRc.MeasuringUnits.Mitutoyo.LinearGuages {
    partial class FormCounterSettings {
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
            this.labelMeasuringUnitInformation = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxSerialPort = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxWaitTimeMs = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelMeasuringUnitInformation
            // 
            this.labelMeasuringUnitInformation.AutoSize = true;
            this.labelMeasuringUnitInformation.Location = new System.Drawing.Point(14, 11);
            this.labelMeasuringUnitInformation.Name = "labelMeasuringUnitInformation";
            this.labelMeasuringUnitInformation.Size = new System.Drawing.Size(43, 15);
            this.labelMeasuringUnitInformation.TabIndex = 0;
            this.labelMeasuringUnitInformation.Text = "測定機";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "通信ポート";
            // 
            // comboBoxSerialPort
            // 
            this.comboBoxSerialPort.FormattingEnabled = true;
            this.comboBoxSerialPort.Location = new System.Drawing.Point(87, 30);
            this.comboBoxSerialPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBoxSerialPort.Name = "comboBoxSerialPort";
            this.comboBoxSerialPort.Size = new System.Drawing.Size(251, 23);
            this.comboBoxSerialPort.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "9,600bps、7bit、パリティ偶数";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(63, 154);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(87, 29);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOKCancelApply_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(157, 154);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(87, 29);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonOKCancelApply_Click);
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(252, 154);
            this.buttonApply.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(87, 29);
            this.buttonApply.TabIndex = 6;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonOKCancelApply_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "測定待ち時間[ms]";
            // 
            // textBoxWaitTimeMs
            // 
            this.textBoxWaitTimeMs.Location = new System.Drawing.Point(118, 95);
            this.textBoxWaitTimeMs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxWaitTimeMs.Name = "textBoxWaitTimeMs";
            this.textBoxWaitTimeMs.Size = new System.Drawing.Size(76, 21);
            this.textBoxWaitTimeMs.TabIndex = 3;
            this.textBoxWaitTimeMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxWaitTimeMs.Click += new System.EventHandler(this.textBoxWaitTimeMs_Click);
            this.textBoxWaitTimeMs.Enter += new System.EventHandler(this.textBoxWaitTimeMs_Enter);
            this.textBoxWaitTimeMs.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxWaitTimeMs_Validating);
            // 
            // FormCounterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 198);
            this.Controls.Add(this.textBoxWaitTimeMs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxSerialPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelMeasuringUnitInformation);
            this.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCounterSettings";
            this.Text = "FormCounterSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMeasuringUnitInformation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxSerialPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxWaitTimeMs;
    }
}