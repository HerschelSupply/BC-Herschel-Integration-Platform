namespace InstrumentationTestApp
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
            this.btnSend = new System.Windows.Forms.Button();
            this.txtXmlPath = new System.Windows.Forms.TextBox();
            this.btnExceptionMsg = new System.Windows.Forms.Button();
            this.btnFlush = new System.Windows.Forms.Button();
            this.btnSendActivityToAurora = new System.Windows.Forms.Button();
            this.btnExceptionDbCall = new System.Windows.Forms.Button();
            this.btnLogMessagingException = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(61, 53);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(259, 38);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "Send Activity Msg";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtXmlPath
            // 
            this.txtXmlPath.Location = new System.Drawing.Point(61, 111);
            this.txtXmlPath.Name = "txtXmlPath";
            this.txtXmlPath.Size = new System.Drawing.Size(1007, 22);
            this.txtXmlPath.TabIndex = 1;
            this.txtXmlPath.Text = "C:\\Temp\\FTP Test\\20170922\\SALES_201709208\\SalesDoc OnRamp b24d7c72-af7f-4fb7-9c57" +
    "-3343043792c2.xml";
            // 
            // btnExceptionMsg
            // 
            this.btnExceptionMsg.Location = new System.Drawing.Point(360, 53);
            this.btnExceptionMsg.Name = "btnExceptionMsg";
            this.btnExceptionMsg.Size = new System.Drawing.Size(208, 38);
            this.btnExceptionMsg.TabIndex = 2;
            this.btnExceptionMsg.Text = "Send Exception Msg";
            this.btnExceptionMsg.UseVisualStyleBackColor = true;
            this.btnExceptionMsg.Click += new System.EventHandler(this.btnExceptionMsg_Click);
            // 
            // btnFlush
            // 
            this.btnFlush.Location = new System.Drawing.Point(619, 53);
            this.btnFlush.Name = "btnFlush";
            this.btnFlush.Size = new System.Drawing.Size(209, 38);
            this.btnFlush.TabIndex = 3;
            this.btnFlush.Text = "Flush";
            this.btnFlush.UseVisualStyleBackColor = true;
            this.btnFlush.Click += new System.EventHandler(this.btnFlush_Click);
            // 
            // btnSendActivityToAurora
            // 
            this.btnSendActivityToAurora.Location = new System.Drawing.Point(61, 173);
            this.btnSendActivityToAurora.Name = "btnSendActivityToAurora";
            this.btnSendActivityToAurora.Size = new System.Drawing.Size(259, 47);
            this.btnSendActivityToAurora.TabIndex = 5;
            this.btnSendActivityToAurora.Text = "Send Log Activity to Aurora";
            this.btnSendActivityToAurora.UseVisualStyleBackColor = true;
            this.btnSendActivityToAurora.Click += new System.EventHandler(this.btnSendActivityToAurora_Click);
            // 
            // btnExceptionDbCall
            // 
            this.btnExceptionDbCall.Location = new System.Drawing.Point(360, 173);
            this.btnExceptionDbCall.Name = "btnExceptionDbCall";
            this.btnExceptionDbCall.Size = new System.Drawing.Size(208, 47);
            this.btnExceptionDbCall.TabIndex = 6;
            this.btnExceptionDbCall.Text = "Send Exception to DB";
            this.btnExceptionDbCall.UseVisualStyleBackColor = true;
            this.btnExceptionDbCall.Click += new System.EventHandler(this.btnExceptionDbCall_Click);
            // 
            // btnLogMessagingException
            // 
            this.btnLogMessagingException.Location = new System.Drawing.Point(61, 261);
            this.btnLogMessagingException.Name = "btnLogMessagingException";
            this.btnLogMessagingException.Size = new System.Drawing.Size(259, 77);
            this.btnLogMessagingException.TabIndex = 7;
            this.btnLogMessagingException.Text = "Test LogMessagingException Method";
            this.btnLogMessagingException.UseVisualStyleBackColor = true;
            this.btnLogMessagingException.Click += new System.EventHandler(this.btnLogMessagingException_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1119, 544);
            this.Controls.Add(this.btnLogMessagingException);
            this.Controls.Add(this.btnExceptionDbCall);
            this.Controls.Add(this.btnSendActivityToAurora);
            this.Controls.Add(this.btnFlush);
            this.Controls.Add(this.btnExceptionMsg);
            this.Controls.Add(this.txtXmlPath);
            this.Controls.Add(this.btnSend);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtXmlPath;
        private System.Windows.Forms.Button btnExceptionMsg;
        private System.Windows.Forms.Button btnFlush;
        private System.Windows.Forms.Button btnSendActivityToAurora;
        private System.Windows.Forms.Button btnExceptionDbCall;
        private System.Windows.Forms.Button btnLogMessagingException;
    }
}

