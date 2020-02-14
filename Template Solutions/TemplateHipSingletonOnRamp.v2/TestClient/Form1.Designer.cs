namespace SqsQueueTester
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
            this.btnActivate = new System.Windows.Forms.Button();
            this.btmPayment = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnActivate
            // 
            this.btnActivate.Location = new System.Drawing.Point(126, 64);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(138, 55);
            this.btnActivate.TabIndex = 0;
            this.btnActivate.Text = "Activate Service";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // btmPayment
            // 
            this.btmPayment.Location = new System.Drawing.Point(126, 141);
            this.btmPayment.Name = "btmPayment";
            this.btmPayment.Size = new System.Drawing.Size(138, 48);
            this.btmPayment.TabIndex = 1;
            this.btmPayment.Text = "Test Payment Logic";
            this.btmPayment.UseVisualStyleBackColor = true;
            this.btmPayment.Click += new System.EventHandler(this.btmPayment_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 278);
            this.Controls.Add(this.btmPayment);
            this.Controls.Add(this.btnActivate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btmPayment;
    }
}

