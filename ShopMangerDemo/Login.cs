using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopMangerDemo
{
    public partial class Login : Form
    {
        Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();

        private static string filepath = Application.StartupPath + "\\UserInfoTxt.txt";
        private static string Hisfilepath = Application.StartupPath + "\\LoginHistoryTxt.txt";
        public Login()
        {
            InitializeComponent();
            this.TimeLable.Text = DateTime.Now.ToString("yyyy-M-d dddd");
        }
        
        private void Login_Load(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            int k = -1;
            if (fs.Length > 0)
            {
                //最后登陆的显示靠前
                string str = sr.ReadToEnd();
                string[] con = str.Split('\n');
                while (!string.IsNullOrEmpty(con[k + 1]))
                {
                    UserInfo userInfo = new UserInfo();
                    //con[k + 1].Replace("\r", null );
                    string[] ds = con[k + 1].Split('|');
                    userInfo.UserName = ds[0].Trim();;
                    userInfo.UserPwd = ds[1].Trim();
                    users.Add(userInfo.UserName, userInfo);
                    k++;
                }
                while (k>-1)
                {
                    if (!string.IsNullOrEmpty(con[k]))
                    {
                        string[] ds = con[k].Split('|');
                        string names = ds[0].Trim();
                        UserNameCombobox.Items.Add(names);
                        k--;
                    }
                }
                //string r = sr.ReadLine();
                //while (!string.IsNullOrEmpty(r))
                //{
                //    string[] ds = r.Split('|');
                //    string names = ds[0];
                //    string pw = ds[1];
                //    userInfo.UserName = names;
                //    userInfo.UserPwd = pw;
                //    UserNameCombobox.Items.Add(userInfo.UserName);
                //    k++;
                //    //UserNameCombobox.DisplayMember = userInfo.UserName;
                //    users.Add(userInfo.UserName, userInfo);
                //    r = sr.ReadLine();
                //}
                UserNameCombobox.Text = UserNameCombobox.Items[0].ToString();
            }
            sr.Close();
            fs.Close();
        }



        private void LoginBtu_Click(object sender, EventArgs e)
        {
            string username = this.UserNameCombobox.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("请输入用户名！", "温馨提示", MessageBoxButtons.OKCancel);
                this.UserNameCombobox.Focus();
                return;
            }
            string password = this.PasswordTXt.Text.Trim();
            try
            {
                //手机号与用户名都可以登陆
                DataSet ds = Database.RunDataSet(String.Format("select * from Person where PersonName='{0}' or PersonPN='{1}'", username, username));
                //DataSet ds = Database.RunDataSet(String.Format("select * from Person where PersonName='{0}' ", username));
                if (password == ds.Tables[0].Rows[0]["Password"].ToString())
                {
                    UserInfo user = new UserInfo();
                    user.UserName = UserNameCombobox.Text;
                    user.UserPwd = PasswordTXt.Text.Trim();
                    StreamWriter streamWriter=new StreamWriter(Hisfilepath,true);
                    string strs = user.UserName + "*    |    *" + user.UserPwd + "*    |    *" + DateTime.Now.ToString("s");
                    streamWriter.WriteLine(strs);
                    streamWriter.Close();
                    //当选择记住密码时  将用户名和密码都写到文件上
                    if (checkBox1.Checked)
                    {
                        string nowtime = DateTime.Now.ToString("G");
                        if (!users.ContainsKey(user.UserName))
                        {
                            StreamWriter sw = new StreamWriter(filepath, true);
                            //string linestr = user.UserName + "|" + user.UserPwd+"|"+nowtime;
                            string linestr = user.UserName + "|" + user.UserPwd;
                            users.Add(user.UserName, user);
                            sw.WriteLine(linestr);
                            sw.Close();
                        }
                        else
                        {
                            StreamReader sr = new StreamReader(filepath, true);
                            string content = sr.ReadToEnd(); //这个就是文本内容
                            string str = user.UserName + "|" + users[user.UserName].UserPwd+ "\r\n";
                            content = content.Replace(str, "");
                            sr.Close();
                            System.IO.File.Delete(filepath);
                            StreamWriter sw = new StreamWriter(filepath,true);
                            users.Remove(user.UserName);
                            users.Add(user.UserName, user);
                            sw.Write(content);
                            sw.WriteLine(user.UserName + "|" + user.UserPwd);
                            sw.Close();

                        }
                    }

                    //当不记住密码时  只将用户名写到文件上
                    else
                    {
                        if (!users.ContainsKey(user.UserName))
                        {
                            StreamWriter sw = new StreamWriter(filepath, true);
                            string content = user.UserName + "|";
                            users.Add(user.UserName, user);
                            sw.WriteLine(content);
                            sw.Close();
                        }
                        else
                        {
                            StreamReader sr = new StreamReader(filepath, true);
                            string content = sr.ReadToEnd(); //这个就是文本内容
                            string str = user.UserName + "|" + users[user.UserName].UserPwd + "\r\n";
                            content = content.Replace(str, "");
                            sr.Close();
                            System.IO.File.Delete(filepath);
                            StreamWriter sw = new StreamWriter(filepath, true);
                            users.Remove(user.UserName);
                            users.Add(user.UserName, user);
                            sw.Write(content);
                            sw.WriteLine(user.UserName + "|" );
                            sw.Close();
                        }
                    }
                    ShopCenter sc = new ShopCenter();//实例化一个Form2窗口 
                    sc.username = ds.Tables[0].Rows[0]["PersonName"].ToString();//设置Form2中string1的值 
                    sc.SetValue();
                    sc.Show();
                    this.Hide();
                }
                else
                {
                    this.PasswordTXt.Focus();
                    MessageBox.Show("请检查密码是否正确！", "温馨提示", MessageBoxButtons.OKCancel);
                }

            }
            catch (Exception)
            {
                DataSet ds2 = Database.RunDataSet(String.Format("select * from Seller where SellerName='{0}' or SellerPN='{1}'", username, username));
                //DataSet ds2 = Database.RunDataSet(String.Format("select * from Seller where SellerName='{0}' ", username));

                if (ds2.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("用户名不存在！", "温馨提示", MessageBoxButtons.OKCancel);
                    this.UserNameCombobox.Focus();
                    return;
                }
                if (password == ds2.Tables[0].Rows[0]["Password"].ToString())
                {
                    UserInfo user = new UserInfo();
                    user.UserName = UserNameCombobox.Text;
                    user.UserPwd = PasswordTXt.Text.Trim();
                    StreamWriter streamWriter = new StreamWriter(Hisfilepath, true);
                    string strs = user.UserName + "*    |    *" + user.UserPwd + "*    |    *" + DateTime.Now.ToString("s");
                    streamWriter.WriteLine(strs);
                    streamWriter.Close();
                    //当选择记住密码时  将用户名和密码都写到文件上
                    if (checkBox1.Checked)
                    {
                        string nowtime = DateTime.Now.ToString("G");
                        if (!users.ContainsKey(user.UserName))
                        {
                            StreamWriter sw = new StreamWriter(filepath, true);
                            //string linestr = user.UserName + "|" + user.UserPwd+"|"+nowtime;
                            string linestr = user.UserName + "|" + user.UserPwd;
                            users.Add(user.UserName, user);
                            sw.WriteLine(linestr);
                            sw.Close();
                        }
                        else
                        {
                            StreamReader sr = new StreamReader(filepath, true);
                            string content = sr.ReadToEnd(); //这个就是文本内容
                            string str = user.UserName + "|" + users[user.UserName].UserPwd + "\r\n";
                            content = content.Replace(str, "");
                            sr.Close();
                            System.IO.File.Delete(filepath);
                            StreamWriter sw = new StreamWriter(filepath, true);
                            users.Remove(user.UserName);
                            users.Add(user.UserName, user);
                            sw.Write(content);
                            sw.WriteLine(user.UserName + "|" + user.UserPwd);
                            sw.Close();

                        }
                    }

                    //当不记住密码时  只将用户名写到文件上
                    else
                    {
                        if (!users.ContainsKey(user.UserName))
                        {
                            StreamWriter sw = new StreamWriter(filepath, true);
                            string content = user.UserName + "|";
                            users.Add(user.UserName, user);
                            sw.WriteLine(content);
                            sw.Close();
                        }
                        else
                        {
                            StreamReader sr = new StreamReader(filepath, true);
                            string content = sr.ReadToEnd(); //这个就是文本内容
                            string str = user.UserName + "|" + users[user.UserName].UserPwd + "\r\n";
                            content = content.Replace(str, "");
                            sr.Close();
                            System.IO.File.Delete(filepath);
                            StreamWriter sw = new StreamWriter(filepath, true);
                            users.Remove(user.UserName);
                            users.Add(user.UserName, user);
                            sw.Write(content);
                            sw.WriteLine(user.UserName + "|");
                            sw.Close();
                        }
                    }
                    //ShopCenter sc = new ShopCenter();
                    //sc.Show();
                    //this.Hide();
                    ShopManager lForm = new ShopManager();//实例化一个Form2窗口 
                    lForm.username = ds2.Tables[0].Rows[0]["SellerName"].ToString();//设置Form2中string1的值
                    lForm.SetValue();
                    lForm.Show();
                    this.Hide();
                }
                else
                {
                    this.PasswordTXt.Focus();
                    MessageBox.Show("请检查密码是否正确！", "温馨提示", MessageBoxButtons.OKCancel);
                }
            }
        }

        private void RegisterBtu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register form=new Register();
            form.Show();
            this.Hide();
        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                LoginBtu_Click(null, null);
            }
        }

        private void PasswordTXt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                LoginBtu_Click(null, null);
            }

        }

        private void UserNameCombobox_TextChanged(object sender, EventArgs e)
        {
            if (users.Count >= 0)
            {
                if (UserNameCombobox.Text != "")
                {
                    if (users.ContainsKey(UserNameCombobox.Text) && !string .IsNullOrEmpty(users[UserNameCombobox.Text.Trim()].UserPwd))
                    {
                        PasswordTXt.Text = users[UserNameCombobox.Text].UserPwd;
                        checkBox1.Checked = true;
                    }
                    else
                    {
                        PasswordTXt.Text = "";
                        checkBox1.Checked = false;
                    }
                }
                else
                {
                    PasswordTXt.Text = "";
                    checkBox1.Checked = false;
                }
            }
            else
            {
                PasswordTXt.Text = "";
                checkBox1.Checked = false;
                
            }
        }


        

        


        
    }
}
