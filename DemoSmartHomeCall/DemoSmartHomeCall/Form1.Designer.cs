namespace DemoSmartHomeCall
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
            this.deviceList = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // deviceList
            // 
            this.deviceList.HelpVisible = false;
            this.deviceList.Location = new System.Drawing.Point(13, 14);
            this.deviceList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.deviceList.Name = "deviceList";
            this.deviceList.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.deviceList.Size = new System.Drawing.Size(442, 674);
            this.deviceList.TabIndex = 0;
            this.deviceList.ToolbarVisible = false;
            this.deviceList.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.deviceList_PropertyValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 702);
            this.Controls.Add(this.deviceList);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Smart Home Dashboard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }

        public System.Windows.Forms.PropertyGrid deviceList;
        #endregion
    }
}