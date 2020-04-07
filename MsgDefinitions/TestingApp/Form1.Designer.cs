namespace TestingApp
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
            this.btnFilterToSting = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFilterToSting
            // 
            this.btnFilterToSting.Location = new System.Drawing.Point(39, 39);
            this.btnFilterToSting.Name = "btnFilterToSting";
            this.btnFilterToSting.Size = new System.Drawing.Size(158, 43);
            this.btnFilterToSting.TabIndex = 0;
            this.btnFilterToSting.Text = "Filter ToString";
            this.btnFilterToSting.UseVisualStyleBackColor = true;
            this.btnFilterToSting.Click += new System.EventHandler(this.btnFilterToSting_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 253);
            this.Controls.Add(this.btnFilterToSting);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFilterToSting;
    }
}

