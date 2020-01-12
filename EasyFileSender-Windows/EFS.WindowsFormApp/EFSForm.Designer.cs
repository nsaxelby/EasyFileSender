namespace EFS.WindowsFormApp
{
    partial class EFSForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.transfersPanel = new System.Windows.Forms.Panel();
            this.yourIPLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.selectedIpLabel = new System.Windows.Forms.Label();
            this.sendFileButton = new System.Windows.Forms.Button();
            this.dragAndDropPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.transfersListBox1 = new EFS.WindowsFormApp.Controls.TransfersListBox();
            this.clientListBox = new EFS.WindowsFormApp.ClientListBox();
            this.panel1.SuspendLayout();
            this.transfersPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.dragAndDropPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.clientListBox);
            this.panel1.Location = new System.Drawing.Point(12, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(225, 400);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Clients";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(241, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Your IP: ";
            // 
            // transfersPanel
            // 
            this.transfersPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transfersPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.transfersPanel.Controls.Add(this.transfersListBox1);
            this.transfersPanel.Location = new System.Drawing.Point(244, 38);
            this.transfersPanel.Name = "transfersPanel";
            this.transfersPanel.Size = new System.Drawing.Size(544, 371);
            this.transfersPanel.TabIndex = 3;
            // 
            // yourIPLabel
            // 
            this.yourIPLabel.AutoSize = true;
            this.yourIPLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yourIPLabel.Location = new System.Drawing.Point(295, 13);
            this.yourIPLabel.Name = "yourIPLabel";
            this.yourIPLabel.Size = new System.Drawing.Size(58, 13);
            this.yourIPLabel.TabIndex = 4;
            this.yourIPLabel.Text = "unknown";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "IP";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(136)))), ((int)(((byte)(199)))));
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Location = new System.Drawing.Point(12, 38);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(225, 26);
            this.panel3.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(189, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Type";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(136)))), ((int)(((byte)(199)))));
            this.panel4.Controls.Add(this.selectedIpLabel);
            this.panel4.Location = new System.Drawing.Point(244, 38);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(544, 26);
            this.panel4.TabIndex = 1;
            // 
            // selectedIpLabel
            // 
            this.selectedIpLabel.AutoSize = true;
            this.selectedIpLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedIpLabel.Location = new System.Drawing.Point(11, 6);
            this.selectedIpLabel.Name = "selectedIpLabel";
            this.selectedIpLabel.Size = new System.Drawing.Size(73, 13);
            this.selectedIpLabel.TabIndex = 1;
            this.selectedIpLabel.Text = "Selected IP";
            // 
            // sendFileButton
            // 
            this.sendFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendFileButton.Location = new System.Drawing.Point(244, 415);
            this.sendFileButton.Name = "sendFileButton";
            this.sendFileButton.Size = new System.Drawing.Size(155, 23);
            this.sendFileButton.TabIndex = 5;
            this.sendFileButton.Text = "Send File";
            this.sendFileButton.UseVisualStyleBackColor = true;
            this.sendFileButton.Click += new System.EventHandler(this.SendFileButton_Click);
            // 
            // dragAndDropPanel
            // 
            this.dragAndDropPanel.AllowDrop = true;
            this.dragAndDropPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dragAndDropPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(136)))), ((int)(((byte)(199)))));
            this.dragAndDropPanel.Controls.Add(this.label5);
            this.dragAndDropPanel.Location = new System.Drawing.Point(406, 416);
            this.dragAndDropPanel.Name = "dragAndDropPanel";
            this.dragAndDropPanel.Size = new System.Drawing.Size(382, 22);
            this.dragAndDropPanel.TabIndex = 6;
            this.dragAndDropPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragAndDropPanel_DragDrop);
            this.dragAndDropPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragAndDropPanel_DragEnter);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(109, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(155, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Drag and Drop file here to send";
            // 
            // transfersListBox1
            // 
            this.transfersListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.transfersListBox1.FormattingEnabled = true;
            this.transfersListBox1.ItemHeight = 75;
            this.transfersListBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.transfersListBox1.Location = new System.Drawing.Point(-1, 22);
            this.transfersListBox1.Name = "transfersListBox1";
            this.transfersListBox1.Size = new System.Drawing.Size(548, 304);
            this.transfersListBox1.TabIndex = 0;
            // 
            // clientListBox
            // 
            this.clientListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clientListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.clientListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.clientListBox.ForeColor = System.Drawing.Color.Black;
            this.clientListBox.FormattingEnabled = true;
            this.clientListBox.ItemHeight = 18;
            this.clientListBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.clientListBox.Location = new System.Drawing.Point(-1, 22);
            this.clientListBox.Name = "clientListBox";
            this.clientListBox.Size = new System.Drawing.Size(225, 382);
            this.clientListBox.TabIndex = 0;
            this.clientListBox.SelectedValueChanged += new System.EventHandler(this.ClientListBox_SelectedValueChanged);
            // 
            // EFSForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dragAndDropPanel);
            this.Controls.Add(this.sendFileButton);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.yourIPLabel);
            this.Controls.Add(this.transfersPanel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "EFSForm";
            this.Text = "Easy File Sender";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EFSForm_FormClosing);
            this.Load += new System.EventHandler(this.EFSForm_Load);
            this.panel1.ResumeLayout(false);
            this.transfersPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.dragAndDropPanel.ResumeLayout(false);
            this.dragAndDropPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel transfersPanel;
        private System.Windows.Forms.Label yourIPLabel;
        private ClientListBox clientListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label selectedIpLabel;
        private System.Windows.Forms.Button sendFileButton;
        private System.Windows.Forms.Panel dragAndDropPanel;
        private System.Windows.Forms.Label label5;
        private Controls.TransfersListBox transfersListBox1;
    }
}

