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
            this.btnActivate = new System.Windows.Forms.Button();
            this.btnProcessConfirmations = new System.Windows.Forms.Button();
            this.btnCancellations = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnActivate
            // 
            this.btnActivate.Location = new System.Drawing.Point(13, 13);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(157, 36);
            this.btnActivate.TabIndex = 0;
            this.btnActivate.Text = "Activate On-Ramp";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // btnProcessConfirmations
            // 
            this.btnProcessConfirmations.Location = new System.Drawing.Point(13, 56);
            this.btnProcessConfirmations.Name = "btnProcessConfirmations";
            this.btnProcessConfirmations.Size = new System.Drawing.Size(186, 41);
            this.btnProcessConfirmations.TabIndex = 1;
            this.btnProcessConfirmations.Text = "Process Confirmations";
            this.btnProcessConfirmations.UseVisualStyleBackColor = true;
            this.btnProcessConfirmations.Click += new System.EventHandler(this.btnProcessConfirmations_Click);
            // 
            // btnCancellations
            // 
            this.btnCancellations.Location = new System.Drawing.Point(12, 103);
            this.btnCancellations.Name = "btnCancellations";
            this.btnCancellations.Size = new System.Drawing.Size(186, 41);
            this.btnCancellations.TabIndex = 1;
            this.btnCancellations.Text = "Process Cancellations";
            this.btnCancellations.UseVisualStyleBackColor = true;
            this.btnCancellations.Click += new System.EventHandler(this.btnCancellations_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.btnCancellations);
            this.Controls.Add(this.btnProcessConfirmations);
            this.Controls.Add(this.btnActivate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btnProcessConfirmations;
        private System.Windows.Forms.Button btnCancellations;
    }
}

