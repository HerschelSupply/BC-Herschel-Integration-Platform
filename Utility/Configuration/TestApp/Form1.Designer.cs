namespace TestApp
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
            this.btnStartRequest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtQueueName = new System.Windows.Forms.TextBox();
            this.txtStartResponse = new System.Windows.Forms.TextBox();
            this.btnEndRequest = new System.Windows.Forms.Button();
            this.txtQueueNameHistory = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStartRequest
            // 
            this.btnStartRequest.Location = new System.Drawing.Point(34, 78);
            this.btnStartRequest.Name = "btnStartRequest";
            this.btnStartRequest.Size = new System.Drawing.Size(190, 23);
            this.btnStartRequest.TabIndex = 0;
            this.btnStartRequest.Text = "Start Processing Queue";
            this.btnStartRequest.UseVisualStyleBackColor = true;
            this.btnStartRequest.Click += new System.EventHandler(this.btnStartRequest_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Queue Name:";
            // 
            // txtQueueName
            // 
            this.txtQueueName.Location = new System.Drawing.Point(132, 38);
            this.txtQueueName.Name = "txtQueueName";
            this.txtQueueName.Size = new System.Drawing.Size(196, 22);
            this.txtQueueName.TabIndex = 2;
            // 
            // txtStartResponse
            // 
            this.txtStartResponse.Location = new System.Drawing.Point(246, 78);
            this.txtStartResponse.Name = "txtStartResponse";
            this.txtStartResponse.Size = new System.Drawing.Size(100, 22);
            this.txtStartResponse.TabIndex = 3;
            // 
            // btnEndRequest
            // 
            this.btnEndRequest.Location = new System.Drawing.Point(34, 125);
            this.btnEndRequest.Name = "btnEndRequest";
            this.btnEndRequest.Size = new System.Drawing.Size(190, 23);
            this.btnEndRequest.TabIndex = 0;
            this.btnEndRequest.Text = "End Processing Queue";
            this.btnEndRequest.UseVisualStyleBackColor = true;
            this.btnEndRequest.Click += new System.EventHandler(this.btnEndRequest_Click);
            // 
            // txtQueueNameHistory
            // 
            this.txtQueueNameHistory.Location = new System.Drawing.Point(375, 60);
            this.txtQueueNameHistory.Multiline = true;
            this.txtQueueNameHistory.Name = "txtQueueNameHistory";
            this.txtQueueNameHistory.Size = new System.Drawing.Size(329, 339);
            this.txtQueueNameHistory.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(375, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Request History";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 425);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtQueueNameHistory);
            this.Controls.Add(this.txtStartResponse);
            this.Controls.Add(this.txtQueueName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnEndRequest);
            this.Controls.Add(this.btnStartRequest);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartRequest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtQueueName;
        private System.Windows.Forms.TextBox txtStartResponse;
        private System.Windows.Forms.Button btnEndRequest;
        private System.Windows.Forms.TextBox txtQueueNameHistory;
        private System.Windows.Forms.Label label2;
    }
}

