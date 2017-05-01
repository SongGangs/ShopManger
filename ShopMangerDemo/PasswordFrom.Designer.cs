namespace ShopMangerDemo
{
    partial class PasswordFrom
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
            this.label1 = new System.Windows.Forms.Label();
            this.OldPassword = new System.Windows.Forms.TextBox();
            this.NewPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SurePassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.OK_Btu = new System.Windows.Forms.Button();
            this.Cancel_Btu = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(30, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "旧密码：";
            // 
            // OldPassword
            // 
            this.OldPassword.Location = new System.Drawing.Point(112, 6);
            this.OldPassword.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.OldPassword.Name = "OldPassword";
            this.OldPassword.Size = new System.Drawing.Size(272, 25);
            this.OldPassword.TabIndex = 1;
            // 
            // NewPassword
            // 
            this.NewPassword.Location = new System.Drawing.Point(112, 51);
            this.NewPassword.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.NewPassword.Name = "NewPassword";
            this.NewPassword.Size = new System.Drawing.Size(272, 25);
            this.NewPassword.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(30, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "新密码：";
            // 
            // SurePassword
            // 
            this.SurePassword.Location = new System.Drawing.Point(112, 95);
            this.SurePassword.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SurePassword.Name = "SurePassword";
            this.SurePassword.Size = new System.Drawing.Size(272, 25);
            this.SurePassword.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(12, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "确认密码：";
            // 
            // OK_Btu
            // 
            this.OK_Btu.Location = new System.Drawing.Point(66, 122);
            this.OK_Btu.Margin = new System.Windows.Forms.Padding(2);
            this.OK_Btu.Name = "OK_Btu";
            this.OK_Btu.Size = new System.Drawing.Size(87, 32);
            this.OK_Btu.TabIndex = 6;
            this.OK_Btu.Text = "确  认";
            this.OK_Btu.UseVisualStyleBackColor = true;
            this.OK_Btu.Click += new System.EventHandler(this.OK_Btu_Click);
            // 
            // Cancel_Btu
            // 
            this.Cancel_Btu.Location = new System.Drawing.Point(243, 122);
            this.Cancel_Btu.Margin = new System.Windows.Forms.Padding(2);
            this.Cancel_Btu.Name = "Cancel_Btu";
            this.Cancel_Btu.Size = new System.Drawing.Size(87, 32);
            this.Cancel_Btu.TabIndex = 7;
            this.Cancel_Btu.Text = "取  消";
            this.Cancel_Btu.UseVisualStyleBackColor = true;
            this.Cancel_Btu.Click += new System.EventHandler(this.Cancel_Btu_Click);
            // 
            // PasswordFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 159);
            this.Controls.Add(this.Cancel_Btu);
            this.Controls.Add(this.OK_Btu);
            this.Controls.Add(this.SurePassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NewPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OldPassword);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PasswordFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "密码修改";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox OldPassword;
        private System.Windows.Forms.TextBox NewPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SurePassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button OK_Btu;
        private System.Windows.Forms.Button Cancel_Btu;
    }
}