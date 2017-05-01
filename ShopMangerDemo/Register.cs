using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopMangerDemo
{
    public partial class Register : Form
    {
        private static string SecurityCodes = "";
        private static bool IsHaveSend = false; //是否已发送验证码 不设置为static的话会
        private static string Hisfilepath = Application.StartupPath + "\\LoginHistoryTxt.txt";
        public Register()
        {
            InitializeComponent();
        }

        private void ManCheck_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
            {

                foreach (CheckBox chk in (sender as CheckBox).Parent.Controls)
                {

                    if (chk != sender)
                    {

                        chk.Checked = false;

                    }

                }

            }
        }

        private void WomenCheck_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
            {

                foreach (CheckBox chk in (sender as CheckBox).Parent.Controls)
                {

                    if (chk != sender)
                    {

                        chk.Checked = false;

                    }

                }

            }
        }

        //注册确定按钮
        private void OkBtu_Click(object sender, EventArgs e)
        {
            
            if (this.PasswordTXT.Text.Trim() == this.SurePasswordTXT.Text.Trim())
            {
                if (IsHaveSend == true&&this.SecurityCodeTXT.Text.Trim()==SecurityCodes)
                {
                    DataSet ds = Database.RunDataSet("select * from Person where 0=1");
                    DataTable dt = ds.Tables[0];
                    DataRow Newdr = dt.NewRow();
                    Newdr["PersonID"] = 0;
                    Newdr["PersonPN"] = this.PhoneNumberTXT.Text.Trim();
                    Newdr["PersonName"] = this.UserNameTXT.Text.Trim();
                    if (this.ManCheck.Checked == true)
                    {
                        Newdr["Gender"] = 0;
                    }
                    else if (this.WomenCheck.Checked == true)
                    {
                        Newdr["Gender"] = 1;
                    }
                    Newdr["Password"] = this.PasswordTXT.Text.Trim();
                    dt.Rows.Add(Newdr);
                    try
                    {
                        int k = Database.update("Person", "PersonID", dt);
                        MessageBox.Show("注册成功！", "温馨提示", MessageBoxButtons.OK);
                        StreamWriter streamWriter = new StreamWriter(Hisfilepath, true);
                        string strs = this.UserNameTXT.Text.Trim() + "*    |    *" + this.PasswordTXT.Text.Trim() + "*    |    *" + DateTime.Now.ToString("s");
                        streamWriter.WriteLine(strs);
                        streamWriter.Close();
                        //ShopCenter sc=new ShopCenter();
                        //sc.Show();
                        //this.Hide();
                        ShopCenter sc = new ShopCenter();//实例化一个Form2窗口 
                        sc.username = this.UserNameTXT.Text.Trim();//设置Form2中string1的值 
                        sc.SetValue();
                        sc.Show();
                        this.Close();
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("注册失败！", "温馨提示", MessageBoxButtons.OK);
                    } 
                }
                else
                {
                    MessageBox.Show("验证码不正确！", "温馨提示", MessageBoxButtons.OK);
                    this.SecurityCodeTXT.Focus();
                }
            }
            else
            {
                MessageBox.Show("两次密码不一致！", "温馨提示", MessageBoxButtons.OK);
                this.PasswordTXT.Focus();
                return;
            }


        }

        //获取验证码按钮
        private void SecurityCodeBtu_Click(object sender, EventArgs e)
        {
            if (MetarnetRegex.IsMobilePhone(this.PhoneNumberTXT.Text.Trim()))
            {
                    DataSet ds=
                        Database.RunDataSet("select * from Person where PersonPN='" +
                                            this.PhoneNumberTXT.Text.Trim() + "'");
                if (ds.Tables[0].Rows.Count != 0)
                {
                    MessageBox.Show("此手机号已被注册！", "温馨提示", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    SendMessage send = new SendMessage();
                    SecurityCodes = send.SendMessages(this.PhoneNumberTXT.Text.Trim());
                    if (!string.IsNullOrEmpty(SecurityCodes))
                    {
                        IsHaveSend = true;
                        MessageBox.Show("验证码已发送，请注意查收!", "温馨提示", MessageBoxButtons.OK);
                        this.SecurityCodeTXT.Focus();
                    }
                }
            }
            else
            {
                MessageBox.Show("手机号格式不正确，请检查！", "温馨提示", MessageBoxButtons.OK);
            } 
            
        }

        //取消按钮
        private void CancelBtu_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show(); 
            this.Close();
        }

       
    }
}
