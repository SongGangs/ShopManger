using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopMangerDemo
{
    public partial class PasswordFrom : Form
    {

        public PasswordFrom()
        {
            InitializeComponent();
        }
        private string usernames = "";

        public string username
        {
            set { usernames = value; }
        }
       
        //确认修改密码
        private void OK_Btu_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(NewPassword.Text.Trim()) && !string.IsNullOrEmpty(SurePassword.Text.Trim()))
            {
                if (NewPassword.Text.Trim() == SurePassword.Text.Trim())
                {
                    try
                    {
                        DataSet ds = Database.RunDataSet("select * from Person where PersonName='" + usernames + "'");
                        DataTable dt = ds.Tables[0];
                        if (OldPassword.Text.Trim() == dt.Rows[0]["Password"].ToString())
                        {
                            dt.Rows[0]["Password"] = NewPassword.Text.Trim();
                            if (MessageBox.Show("你确定要修改密码吗？", "温馨提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                try
                                {
                                    int k = Database.update("Person", "PersonID", dt);
                                    MessageBox.Show("密码修改成功！", "温馨提示");
                                    this.Close();
                                    return;
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("密码修改失败！", "温馨提示");
                                    return;
                                }
                            }

                        }
                        else
                        {
                            MessageBox.Show("你输入的旧密码不对，请重新输入！", "温馨提示");
                            this.OldPassword.Focus();
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        DataSet dss = Database.RunDataSet("select * from Seller where SellerName='" + usernames + "'");
                        DataTable dt = dss.Tables[0];
                        if (OldPassword.Text.Trim() == dt.Rows[0]["Password"].ToString())
                        {
                            dt.Rows[0]["Password"] = NewPassword.Text.Trim();
                            if (MessageBox.Show("你确定要修改密码吗？", "温馨提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                try
                                {
                                    int k = Database.update("Seller", "SellerID", dt);
                                    MessageBox.Show("密码修改成功！", "温馨提示");
                                    this.Close();
                                    return;
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("密码修改失败！", "温馨提示");
                                    return;
                                }
                            }

                        }
                        else
                        {
                            MessageBox.Show("你输入的旧密码不对，请重新输入！", "温馨提示");
                            this.OldPassword.Focus();
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("输入的两次新密码不一致！", "温馨提示");
                    this.SurePassword.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show("新密码不能为空！", "温馨提示");
                this.NewPassword.Focus();
                return;
            }
        }

        private void Cancel_Btu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
    }
}
