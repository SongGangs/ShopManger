using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ShopMangerDemo
{
    public partial class ShopCenter : Form
    {
        private static string phonenumber = "";
        private static string SecurityCodes = "";
        private static bool IsHaveSend = false; //是否已发送验证码 
        private string usernames = "";
        private int values = -1;
        private string PersonID = "";
        private int shopvalues = -1;
        private int storebalues = -1;
        private int ordersValue = -1;

        public string username
        {
            set { usernames = value; }
        }

        public void SetValue()
        {
            this.usernameLabel.Text = usernames;
        }

        public ShopCenter()
        {
            InitializeComponent();
        }

        private void ShopCenter_Load(object sender, EventArgs e)
        {
            DataSet ds = Database.RunDataSet("select * from Person where PersonName='" + usernames + "'");
            PersonID = ds.Tables[0].Rows[0]["PersonID"].ToString();
        }
        //性别之间只能单选
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

        //价格的转换通用方法
        public string ChangePrice(string price)
        {
            string[] str = price.Split('.');
            price = str[0] + "." + str[1].Remove(2);
            return price;
        }
        //显示密码管理界面
        private void PasswordLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PasswordFrom PF = new PasswordFrom();
            PF.username = usernames;
            PF.ShowDialog();
        }

        //显示商城界面
        private void ShoppingLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!ShoppingPanel.Visible)
            {
                ShoppingPanel.Visible = !ShoppingPanel.Visible;
                HistoryShoppingPanel.Visible = !ShoppingPanel.Visible;
                OrdersHistoryPanel.Visible = !ShoppingPanel.Visible;
                UserCenterPanel.Visible = !ShoppingPanel.Visible;
            }

            this.AllpriceLabel.Text = "0 元";
            DataSet ds = Database.RunDataSet("select * from Catalog");
            CatalogComboBox.DataSource = ds.Tables[0];
            CatalogComboBox.ValueMember = "CatalogID";
            CatalogComboBox.SelectedIndex = -1;
            CatalogComboBox.DisplayMember = "CatalogName";
            CatalogComboBox.Text = "商品种类";

            DataSet Productds = Database.RunDataSet("select a.*,b.SellPrice from Product a join ItemUp b on a.ProductID=b.ProductID where b.ItemUpState='0'");//0=上架
            DataTable dt = Productds.Tables[0];
            storebalues = dt.Rows.Count;
            //dataGridView1.DataSource = dt;        若用此句话 界面将自动绑定数据 并且与前台自定义空间无关
            //为dataGridView 手动绑定数据
            dataGridView1.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView1.Rows.Add();
                Image image = Image.FromFile(dt.Rows[i]["ImageSource"].ToString());
                Image ReducedImage;
                //Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                ReducedImage = image.GetThumbnailImage(dataGridView1.Rows[i].Cells["ImageSource"].Size.Width,
                dataGridView1.Rows[i].Cells["ImageSource"].Size.Height, null, IntPtr.Zero);
                dataGridView1[0, i].Value = ReducedImage;
                //dataGridView1[1,i].Value = dt.Rows[i]["ProductName"].ToString();
                dataGridView1.Rows[i].Cells["ProductName"].Value = dt.Rows[i]["ProductName"].ToString();
                dataGridView1.Rows[i].Cells["SellPrice"].Value = ChangePrice(dt.Rows[i]["SellPrice"].ToString()) + "元";
                dataGridView1.Rows[i].Cells["ProductInt"].Value = dt.Rows[i]["ProductInt"].ToString();//商品介绍
                dataGridView1.Rows[i].Cells["Num"].Value = dt.Rows[i]["SellerNum"].ToString();//销量
                dataGridView1.Rows[i].Cells["ReduceBtu"].Value = "-";
                dataGridView1.Rows[i].Cells["Nums"].Value = "0";//购买数量
                dataGridView1.Rows[i].Cells["AddBtu"].Value = "+";
                dataGridView1.Rows[i].Cells["AddShoppingBtu"].Value = "加入购物车";
            }

        }

        //显示订单记录界面
        private void OrdersHistoryLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!OrdersHistoryPanel.Visible)
            {
                OrdersHistoryPanel.Visible = !OrdersHistoryPanel.Visible;
                ShoppingPanel.Visible = !OrdersHistoryPanel.Visible;
                HistoryShoppingPanel.Visible = !OrdersHistoryPanel.Visible;
                UserCenterPanel.Visible = !OrdersHistoryPanel.Visible;
            }

            OrdersHistoryPanel.Controls.Clear();
            DataSet ds =
                Database.RunDataSet(
                    "select c.*,b.SellPrice,a.BuyNum,a.OrderID,a.OrderStatesID,a.Allprice,d.* from Orders a join ItemUp b on a.ItemUpID=b.ItemUpID join Product c on c.ProductID=b.ProductID join OrderStates d on a.OrderStatesID=d.OrderStatesID where a.PersonID='" +
                    PersonID + "' and a.PayStates=0 and a.OrderStatesID!=1");
            ordersValue = ds.Tables[0].Rows.Count;
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {


                    //Label 的生成  订单号
                    Label OrdersIDlabel = new Label();
                    //label.AutoSize = true  ;
                    //nameLabel.BackColor = System.Drawing.Color.Beige;
                    OrdersIDlabel.BackColor = System.Drawing.Color.Transparent;
                    OrdersIDlabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    OrdersIDlabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    OrdersIDlabel.Name = "ordersIDs" + i;
                    OrdersIDlabel.Size = new System.Drawing.Size(160, 30);
                    OrdersIDlabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    OrdersIDlabel.Text = "订单号：" + ds.Tables[0].Rows[i]["OrderID"].ToString();
                    OrdersIDlabel.Location = new System.Drawing.Point(5, 0);




                    //Label 的生成  图片
                    Label imageLabel = new Label();
                    imageLabel.Size = new System.Drawing.Size(85, 120);
                    Image image = Image.FromFile(ds.Tables[0].Rows[i]["ImageSource"].ToString());
                    Image ReducedImage = image.GetThumbnailImage(Width, Height, null, IntPtr.Zero);
                    imageLabel.BackgroundImage = ReducedImage;
                    imageLabel.Location = new System.Drawing.Point(15, 30);


                    //Label 的生成  商品名称
                    Label nameLabel = new Label();
                    nameLabel.BackColor = System.Drawing.Color.Transparent;
                    nameLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    nameLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    nameLabel.Size = new System.Drawing.Size(230, 55);
                    nameLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                    nameLabel.Text = "商品名称：" + ds.Tables[0].Rows[i]["ProductName"].ToString();
                    nameLabel.Location = new System.Drawing.Point(100, 30);

                    //Label 的生成  订单状态
                    Label Paystate = new Label();
                    Paystate.BackColor = System.Drawing.Color.Transparent;
                    Paystate.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    Paystate.ForeColor = System.Drawing.SystemColors.WindowText;
                    Paystate.Size = new System.Drawing.Size(120, 30);
                    Paystate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                    Paystate.Text = "订单状态：" + ds.Tables[0].Rows[i]["OrderStatesName"].ToString();
                    Paystate.Location = new System.Drawing.Point(340, 45);

                    //Label 的生成  商品介绍
                    Label IntLabel = new Label();
                    IntLabel.BackColor = System.Drawing.Color.Transparent;
                    IntLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    IntLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    IntLabel.Size = new System.Drawing.Size(240, 50);
                    IntLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                    IntLabel.Text = "商品介绍：" + ds.Tables[0].Rows[i]["ProductInt"].ToString();
                    IntLabel.Location = new System.Drawing.Point(100, 85);

                    //LinkLabel 的生成  改变订单状态
                    LinkLabel ChangeLink = new LinkLabel();
                    ChangeLink.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
                    ChangeLink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                    ChangeLink.Location = new System.Drawing.Point(340, 85);
                    ChangeLink.Name = "ChangeOSLink" + i; //待商议
                    ChangeLink.Size = new System.Drawing.Size(120, 30);
                    ChangeLink.Text = "确认收货";
                    ChangeLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    ChangeLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(ChangeLink_LinkClicked);//这里缺事件

                    //Label 的生成  单价
                    Label PriceLabel = new Label();
                    PriceLabel.BackColor = System.Drawing.Color.Transparent;
                    PriceLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    PriceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    PriceLabel.Size = new System.Drawing.Size(100, 25);
                    PriceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    PriceLabel.Text = "单价：" + ChangePrice(ds.Tables[0].Rows[i]["SellPrice"].ToString());
                    PriceLabel.Location = new System.Drawing.Point(100, 135);

                    //Label 的生成  购买数量
                    Label NumsLabel = new Label();
                    NumsLabel.BackColor = System.Drawing.Color.Transparent;
                    NumsLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    NumsLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    NumsLabel.Size = new System.Drawing.Size(50, 25);
                    NumsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    NumsLabel.Text = "X" + ds.Tables[0].Rows[i]["BuyNum"].ToString();
                    NumsLabel.Location = new System.Drawing.Point(230, 135);

                    //Label 的生成  总价
                    Label Allpay = new Label();
                    Allpay.BackColor = System.Drawing.Color.Transparent;
                    Allpay.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    Allpay.ForeColor = System.Drawing.SystemColors.WindowText;
                    Allpay.Size = new System.Drawing.Size(100, 25);
                    Allpay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    Allpay.Text = "总金额：" + ChangePrice(ds.Tables[0].Rows[i]["Allprice"].ToString());
                    Allpay.Location = new System.Drawing.Point(310, 135);

                    // Panel 的生成
                    Panel panel = new Panel();
                    panel.Controls.Add(OrdersIDlabel);
                    panel.Controls.Add(imageLabel);
                    panel.Controls.Add(nameLabel);
                    panel.Controls.Add(Paystate);
                    panel.Controls.Add(IntLabel);
                    if (ds.Tables[0].Rows[i]["OrderStatesID"].ToString() == "2")
                    {
                        panel.Controls.Add(ChangeLink);
                    }
                    panel.Controls.Add(PriceLabel);
                    panel.Controls.Add(NumsLabel);
                    panel.Controls.Add(Allpay);
                    panel.BackColor = System.Drawing.Color.Red;
                    panel.Location = new System.Drawing.Point(0, i * 170);
                    panel.Size = new System.Drawing.Size(460, 160);
                    this.OrdersHistoryPanel.Controls.Add(panel);
                }
            }
            else
            {
                //Label 的生成  
                Label label = new Label();
                label.BackColor = System.Drawing.Color.Transparent;
                label.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold);
                label.ForeColor = System.Drawing.Color.Red;
                label.Size = new System.Drawing.Size(500, 40);
                label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                label.Text = "抱歉 你暂未向购物车加入什么商品！";
                label.Location = new System.Drawing.Point(0, 0);

                this.HistoryShoppingDispalyPanel.Controls.Add(label);
            }
        }

        //显示购物车界面
        private void HistoryShoppingLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!HistoryShoppingPanel.Visible)
            {
                HistoryShoppingPanel.Visible = !HistoryShoppingPanel.Visible;
                OrdersHistoryPanel.Visible = !HistoryShoppingPanel.Visible;
                ShoppingPanel.Visible = !HistoryShoppingPanel.Visible;
                UserCenterPanel.Visible = !HistoryShoppingPanel.Visible;
            }
            HistoryShoppingDispalyPanel.Controls.Clear();
            DataSet ds =
                Database.RunDataSet(
                    "select c.*,b.SellPrice,a.BuyNum,a.OrderID from Orders a join ItemUp b on a.ItemUpID=b.ItemUpID join Product c on c.ProductID=b.ProductID where a.PersonID='" +
                    PersonID + "' and a.PayStates=1 and a.OrderStatesID=1");
            shopvalues = ds.Tables[0].Rows.Count;
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //CheckBox 的生成
                    CheckBox checkBox = new CheckBox();
                    checkBox.Location = new System.Drawing.Point(0, 0);
                    checkBox.Name = "checkBox" + i;
                    checkBox.Size = new System.Drawing.Size(15, 30);
                    checkBox.CheckedChanged += new System.EventHandler(CheckBoxs_CheckedChanged);//这里缺事件

                    //Label 的生成  订单号
                    Label OrdersIDlabel = new Label();
                    //label.AutoSize = true  ;
                    //nameLabel.BackColor = System.Drawing.Color.Beige;
                    OrdersIDlabel.BackColor = System.Drawing.Color.Transparent;
                    OrdersIDlabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    OrdersIDlabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    OrdersIDlabel.Size = new System.Drawing.Size(200, 30);
                    OrdersIDlabel.Name = "OrderID" + i;
                    OrdersIDlabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    OrdersIDlabel.Text = "订单号：" + ds.Tables[0].Rows[i]["OrderID"].ToString();
                    OrdersIDlabel.Location = new System.Drawing.Point(25, 0);


                    //LinkLabel 的生成  编辑按钮
                    LinkLabel ChangeLink = new LinkLabel();
                    ChangeLink.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
                    ChangeLink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                    ChangeLink.Location = new System.Drawing.Point(500, 0);
                    ChangeLink.Name = "ChangeLink" + i; //待商议
                    ChangeLink.Size = new System.Drawing.Size(60, 30);
                    ChangeLink.Text = "编 辑";
                    ChangeLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    ChangeLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(ChangeLink_LinkClicked);//这里缺事件


                    //Label 的生成  图片
                    Label imageLabel = new Label();
                    //label.AutoSize = true  ;
                    imageLabel.Size = new System.Drawing.Size(85, 120);
                    imageLabel.Name = "ImageSource" + i;
                    Image image = Image.FromFile(ds.Tables[0].Rows[i]["ImageSource"].ToString());
                    Image ReducedImage = image.GetThumbnailImage(Width, Height, null, IntPtr.Zero);
                    imageLabel.BackgroundImage = ReducedImage;
                    imageLabel.Location = new System.Drawing.Point(15, 30);


                    //Label 的生成  商品名称
                    Label nameLabel = new Label();
                    //label.AutoSize = true  ;
                    //nameLabel.BackColor = System.Drawing.Color.Beige;
                    nameLabel.BackColor = System.Drawing.Color.Transparent;
                    nameLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    nameLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    nameLabel.Size = new System.Drawing.Size(250, 55);
                    nameLabel.Name = "ProductName" + i;
                    nameLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                    nameLabel.Text = "商品名称：" + ds.Tables[0].Rows[i]["ProductName"].ToString();
                    nameLabel.Location = new System.Drawing.Point(100, 30);



                    //Label 的生成  商品介绍
                    Label IntLabel = new Label();
                    //label.AutoSize = true  ;
                    //IntLabel.BackColor = System.Drawing.Color.Beige;
                    IntLabel.BackColor = System.Drawing.Color.Transparent;
                    IntLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    IntLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    IntLabel.Size = new System.Drawing.Size(250, 50);
                    IntLabel.Name = "ProductInt" + i;
                    IntLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                    IntLabel.Text = "商品介绍：" + ds.Tables[0].Rows[i]["ProductInt"].ToString();
                    IntLabel.Location = new System.Drawing.Point(100, 85);

                    //ComboBox 地址
                    // 
                    DataSet myDataSet = Database.RunDataSet("select * from Address where PersonID='" + PersonID + "'");
                    DataTable myDataTable = myDataSet.Tables[0];
                    ComboBox comboBox = new ComboBox();
                    comboBox.Font = new System.Drawing.Font("宋体", 10F);
                    comboBox.FormattingEnabled = true;
                    comboBox.Location = new System.Drawing.Point(350, 30);
                    comboBox.Name = "Address" + i;
                    comboBox.Size = new System.Drawing.Size(150, 30);
                    comboBox.Text = "请选择地址";
                    comboBox.DataSource = myDataTable;
                    comboBox.ValueMember = "AddressID";
                    comboBox.DisplayMember = "AddressName";

                    //Label 的生成  单价
                    Label PriceLabel = new Label();
                    //label.AutoSize = true  ;
                    //PriceLabel.BackColor = System.Drawing.Color.Beige;
                    PriceLabel.BackColor = System.Drawing.Color.Transparent;
                    PriceLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    PriceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    PriceLabel.Size = new System.Drawing.Size(100, 25);
                    PriceLabel.Name = "Price" + i;
                    PriceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    PriceLabel.Text = "单价：" + ChangePrice(ds.Tables[0].Rows[i]["SellPrice"].ToString());
                    PriceLabel.Location = new System.Drawing.Point(100, 135);

                    //Label 的生成  购买数量
                    Label NumsLabel = new Label();
                    //label.AutoSize = true  ;
                    //NumsLabel.BackColor = System.Drawing.Color.Beige;
                    NumsLabel.BackColor = System.Drawing.Color.Transparent;
                    NumsLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    NumsLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    NumsLabel.Size = new System.Drawing.Size(50, 25);
                    NumsLabel.Name = "Nums" + i;
                    NumsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    NumsLabel.Text = "X" + ds.Tables[0].Rows[i]["BuyNum"].ToString();
                    NumsLabel.Location = new System.Drawing.Point(360, 135);


                    // Panel 的生成
                    Panel panel = new Panel();
                    panel.Controls.Add(checkBox);
                    panel.Controls.Add(OrdersIDlabel);
                    panel.Controls.Add(ChangeLink);
                    panel.Controls.Add(imageLabel);
                    panel.Controls.Add(nameLabel);
                    panel.Controls.Add(comboBox);
                    panel.Controls.Add(IntLabel);
                    panel.Controls.Add(PriceLabel);
                    panel.Controls.Add(NumsLabel);
                    panel.BackColor = System.Drawing.Color.Red;
                    panel.Location = new System.Drawing.Point(0, i * 170);
                    panel.Size = new System.Drawing.Size(560, 160);
                    panel.Name = "shoppingpan" + i;
                    this.HistoryShoppingDispalyPanel.Controls.Add(panel);
                }
            }
            else
            {
                //Label 的生成  
                Label label = new Label();
                label.BackColor = System.Drawing.Color.Transparent;
                label.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold);
                label.ForeColor = System.Drawing.Color.Red;
                label.Size = new System.Drawing.Size(500, 40);
                label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                label.Text = "抱歉 你暂未向购物车加入什么商品！";
                label.Location = new System.Drawing.Point(0, 0);

                this.HistoryShoppingDispalyPanel.Controls.Add(label);
            }
        }

        //显示个人中心界面
        private void UserCenterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!UserCenterPanel.Visible)
            {
                UserCenterPanel.Visible = !UserCenterPanel.Visible;
                ShoppingPanel.Visible = !UserCenterPanel.Visible;
                HistoryShoppingPanel.Visible = !UserCenterPanel.Visible;
                OrdersHistoryPanel.Visible = !UserCenterPanel.Visible;
            }
            //从数据库读取信息到个人中心
            this.UserNameTXT.Text = usernames;
            //DataSet ds =
            //    Database.RunDataSet(
            //        "select a.*,b.AddressName from Person a join Address b on a.PersonID=b.PersonID where a.PersonName='" +
            //        usernames + "'");
            DataSet ds = Database.RunDataSet("select * from Person where PersonID='" + PersonID + "'");
            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Gender"].ToString()))
            {
                if (int.Parse(ds.Tables[0].Rows[0]["Gender"].ToString()) == 0)
                {
                    this.ManCheck.Checked = true;
                }
                else if (int.Parse(ds.Tables[0].Rows[0]["Gender"].ToString()) == 1)
                {
                    this.WomenCheck.Checked = true;
                }
            }
            this.AgeTXT.Text = ds.Tables[0].Rows[0]["Ages"].ToString();
            this.PhoneNumberTXT.Text = ds.Tables[0].Rows[0]["PersonPN"].ToString();
            phonenumber = ds.Tables[0].Rows[0]["PersonPN"].ToString();
            this.EmailTXT.Text = ds.Tables[0].Rows[0]["Emails"].ToString();
            DataSet addressDataSet =
                Database.RunDataSet("select * from Address where PersonID='" +
                                    ds.Tables[0].Rows[0]["PersonID"].ToString() + "'");

            values = addressDataSet.Tables[0].Rows.Count;
            //先清空AddressPanel这个区域上的控件，以免删除后的控件不依然存在。
            this.AddressPanel.Controls.Clear();
            this.AddressPanel.Controls.Add(Add_addressPanel);
            //使添加地址文本框始终为空
            this.Add_addressTXT.Text = "";
            if (values > 0)
            {
                for (int i = 0; i < values; i++)
                {
                    //Label 的生成
                    Label label = new Label();
                    //label.AutoSize = true  ;
                    label.BackColor = System.Drawing.Color.Beige;
                    //label.Dock = System.Windows.Forms.DockStyle.Left;
                    label.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    label.ForeColor = System.Drawing.SystemColors.WindowText;
                    label.Size = new System.Drawing.Size(257, 25);
                    label.Name = "AddressLabel" + i; //待商议
                    label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    label.Text = addressDataSet.Tables[0].Rows[i]["AddressName"].ToString();
                    label.Location = new System.Drawing.Point(0, 0);


                    //LinkLabel 的生成
                    LinkLabel linkLabel = new LinkLabel();
                    linkLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
                    linkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                    linkLabel.Location = new System.Drawing.Point(257, 0);
                    linkLabel.Name = "AddressLink" + i; //待商议
                    linkLabel.Size = new System.Drawing.Size(51, 25);
                    linkLabel.Text = "删 除";
                    linkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    linkLabel.LinkClicked +=
                        new System.Windows.Forms.LinkLabelLinkClickedEventHandler(Delete_addressLink_LinkClicked);
                    //这里需要写个删除  事件

                    // Panel 的生成
                    Panel panel = new Panel();
                    panel.Controls.Add(label);
                    panel.Controls.Add(linkLabel);
                    panel.Location = new System.Drawing.Point(0, i * 30);
                    panel.Size = new System.Drawing.Size(401, 25);
                    this.AddressPanel.Controls.Add(panel);
                }
            }

            if (values > 0)
            {
                if (values < 3)
                {
                    //当地址小于3个时，把添加地址区域显示
                    Add_addressPanel.Visible = UserCenterPanel.Visible;
                    Add_addressPanel.Location = new System.Drawing.Point(0, values * 30);
                }
                else
                {
                    Add_addressPanel.Visible = !UserCenterPanel.Visible;
                    Label label = new Label();
                    label.BackColor = System.Drawing.Color.Transparent;
                    label.ForeColor = System.Drawing.Color.Red;
                    label.Size = new System.Drawing.Size(331, 25);
                    label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    label.Text = "你的地址已经达到三个，不能再增加！";
                    label.Location = new System.Drawing.Point(0, values * 30);
                    this.AddressPanel.Controls.Add(label);
                }
            }
            else
            {
                Add_addressPanel.Location = new System.Drawing.Point(0, 0);
            }


        }

        //个人中心保存按钮
        private void UserCenterOK_Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataSet ds = Database.RunDataSet("select * from Person where PersonID='" + PersonID + "'");
            DataTable dt = ds.Tables[0];
            DataRow dr = dt.Rows[0];
            if (this.ManCheck.Checked == true)
            {
                dr["Gender"] = 0;
            }
            else if (this.WomenCheck.Checked == true)
            {
                dr["Gender"] = 1;
            }

            dr["Emails"] = this.EmailTXT.Text.Trim();
            if (!string.IsNullOrEmpty(this.AgeTXT.Text.Trim()))
            {
                dr["Ages"] = this.AgeTXT.Text.Trim();
            }
            if ((IsHaveSend && this.SecurityCodeTXT.Text.Trim() == SecurityCodes))
            {
                dr["PersonPN"] = this.PhoneNumberTXT.Text.Trim();
            }
            if (MessageBox.Show("你确定要保存修改吗？", "温馨提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int k = Database.update("Person", "PersonID", dt);
                    MessageBox.Show("修改保存成功！", "温馨提示");
                }
                catch (Exception)
                {
                    MessageBox.Show("修改保存失败！", "温馨提示");
                }
            }
        }

        //获取验证码按钮
        private void SecurityCodeLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.PhoneNumberTXT.Text.Trim() != phonenumber)
            {
                if (MetarnetRegex.IsMobilePhone(this.PhoneNumberTXT.Text.Trim()))
                {
                    DataSet ds =
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
            else
            {
                MessageBox.Show("手机号未修改！", "温馨提示", MessageBoxButtons.OK);
            }
        }

        //添加地址按钮
        private void Add_addressLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataSet addressDataSet = Database.RunDataSet("select * from Address where 0=1");
            DataTable dt = addressDataSet.Tables[0];
            DataRow newdr = dt.NewRow();
            newdr["AddressID"] = 0;
            newdr["AddressName"] = Add_addressTXT.Text.Trim();
            newdr["PersonID"] = PersonID;
            newdr["Isusually"] = 1;
            dt.Rows.Add(newdr);
            if (MessageBox.Show("你确定要添加吗？", "温馨提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int k = Database.update("Address", "AddressID", dt);
                    MessageBox.Show("地址添加成功！", "温馨提示");
                }
                catch (Exception)
                {
                    MessageBox.Show("地址添加失败！", "温馨提示");
                }
            }
            UserCenterLink_LinkClicked(null, null);
        }

        //删除地址按钮
        private void Delete_addressLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string linknames = ((System.Windows.Forms.Control)(sender)).Name.Trim();
            for (int i = 0; i < values; i++)
            {
                if (linknames == "AddressLink" + i)
                {
                    deleteAddress(i);
                }
            }
            UserCenterLink_LinkClicked(null, null);

        }

        //删除地址的通用方法
        private void deleteAddress(int i)
        {
            DataSet ds = Database.RunDataSet("select * from  Address where PersonID='" + PersonID + "'");
            string labelnames = "AddressLabel" + i;
            //根据控件Name属性寻找其控件
            foreach (Control c in Application.OpenForms[1].Controls.Find(labelnames, true))
            {
                if (MessageBox.Show("你确定要删除吗？", "温馨提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        int k =
                            Database.ExecuteNonQuery("delete from Address where AddressID='" +
                                                     ds.Tables[0].Rows[i]["AddressID"].ToString() + "'");
                        MessageBox.Show("删除成功！", "温馨提示");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("删除失败！", "温馨提示");
                    }
                }
            }
        }

        private void ExitLB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            Login login = new Login();
            login.ShowDialog();
        }

        //搜索按钮
        private void SearchBtu_Click(object sender, EventArgs e)
        {
            DataSet ds = Database.RunDataSet("select a.*,b.SellPrice from Product a join ItemUp b on a.ProductID=b.ProductID where a.ProductName like '%" + this.SearchTXT.Text.Trim() + "%'");
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                dataGridView1.Rows.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    Image image = Image.FromFile(dt.Rows[i]["ImageSource"].ToString());
                    Image ReducedImage;
                    //Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    ReducedImage = image.GetThumbnailImage(Width, Height, null, IntPtr.Zero);
                    dataGridView1[0, i].Value = ReducedImage;
                    //dataGridView1[1,i].Value = dt.Rows[i]["ProductName"].ToString();
                    dataGridView1.Rows[i].Cells["ProductName"].Value = dt.Rows[i]["ProductName"].ToString();
                    dataGridView1.Rows[i].Cells["SellPrice"].Value = dt.Rows[i]["SellPrice"].ToString().Remove(4) + "元";
                    dataGridView1.Rows[i].Cells["ProductInt"].Value = dt.Rows[i]["ProductInt"].ToString();//商品介绍
                    dataGridView1.Rows[i].Cells["Num"].Value = dt.Rows[i]["Num"].ToString();//销量
                    dataGridView1.Rows[i].Cells["ReduceBtu"].Value = "-";
                    dataGridView1.Rows[i].Cells["Nums"].Value = "0";//购买数量
                    dataGridView1.Rows[i].Cells["AddBtu"].Value = "+";
                    dataGridView1.Rows[i].Cells["AddShoppingBtu"].Value = "加入购物车";
                }
            }
        }



        //对dataGridView里的button加入点击事件
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView1[e.ColumnIndex, e.RowIndex].GetType().Name == "DataGridViewButtonCell")
                {
                    if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString() == "-")   //减少按钮
                    {
                        if (!string.IsNullOrEmpty(dataGridView1[e.ColumnIndex + 1, e.RowIndex].Value.ToString()) && int.Parse(dataGridView1[e.ColumnIndex + 1, e.RowIndex].Value.ToString()) > 0)
                        {
                            dataGridView1[e.ColumnIndex + 1, e.RowIndex].Value =
                                (int.Parse(dataGridView1[e.ColumnIndex + 1, e.RowIndex].Value.ToString()) - 1).ToString();
                        }
                        else
                        {
                            dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value = "0";
                        }
                    }
                    else if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString() == "+")  //增加按钮
                    {
                        if (!string.IsNullOrEmpty(dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value.ToString()) && int.Parse(dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value.ToString()) >= 0)
                        {
                            dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value =
                                (int.Parse(dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value.ToString()) + 1).ToString();
                        }
                        else
                        {
                            dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value = "0";
                        }
                    }
                    else if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString() == "加入购物车")    //加入购物车按钮
                    {
                        DataSet myDataSet = Database.RunDataSet("select a.* from ItemUp a join Product b on a.ProductID=b.ProductID where b.ProductName='" + dataGridView1[1, e.RowIndex].Value.ToString() + "'");
                        DataSet orderDataSet =
                            Database.RunDataSet(
                                "select a.* from Orders a join ItemUp b on a.ItemUpID=b.ItemUpID where a.PersonID='" +
                                PersonID + "' and b.ProductID='" + myDataSet.Tables[0].Rows[0]["ProductID"].ToString() +
                                "' and a.PayStates=1 and a.OrderStatesID=1");
                        if (orderDataSet.Tables[0].Rows.Count <= 0)
                        {
                            DataTable dt = orderDataSet.Tables[0];
                            DataRow dr = dt.NewRow();
                            string orderids = DateTime.Now.ToString("yyyyMMddhhmmss");
                            dr["OrderID"] = orderids;
                            dr["ItemUpID"] = myDataSet.Tables[0].Rows[0]["ItemUpID"].ToString();
                            dr["SellerID"] = myDataSet.Tables[0].Rows[0]["SellerID"].ToString();
                            dr["BuyNum"] = dataGridView1[e.ColumnIndex - 2, e.RowIndex].Value.ToString();
                            dr["OrderStatesID"] = 1; //订单状态=1时，表示未提交
                            dr["OrderTime"] = DateTime.Now.ToString("s");
                            dr["PayStates"] = 1; //支付状态=1时 表示未支付
                            dr["PersonID"] = PersonID; //用户ID
                            dr["AllPrice"] =
                                (Double.Parse(myDataSet.Tables[0].Rows[0]["SellPrice"].ToString()) *
                                 Double.Parse(dataGridView1[e.ColumnIndex - 2, e.RowIndex].Value.ToString())).ToString();
                            //dr["AllPrice"] = "10";
                            dt.Rows.Add(dr);
                            try
                            {
                                Int64 k = Database.update2("Orders", "OrderID", dt);
                                AllpriceLabel.Text = "0 元";
                                for (int i = 0; i < storebalues; i++)
                                {
                                    AllpriceLabel.Text = (Double.Parse(dataGridView1.Rows[i].Cells["SellPrice"].Value.ToString().Replace("元", null)) *
                                                          Double.Parse(dataGridView1.Rows[i].Cells["Nums"].Value.ToString()) +
                                                          Double.Parse(AllpriceLabel.Text.Replace("元", "").Trim())).ToString() + " 元";
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            DataTable dt = orderDataSet.Tables[0];
                            dt.Rows[0]["BuyNum"] =
                                (int.Parse(dataGridView1[e.ColumnIndex - 2, e.RowIndex].Value.ToString()) +
                                 int.Parse(dt.Rows[0]["BuyNum"].ToString())).ToString();
                            try
                            {
                                Int64 k = Database.update2("Orders", "OrderID", dt);

                                AllpriceLabel.Text = "0 元";
                                for (int i = 0; i < storebalues; i++)
                                {
                                    AllpriceLabel.Text = (Double.Parse(dataGridView1.Rows[i].Cells["SellPrice"].Value.ToString().Replace("元", null)) *
                                                          Double.Parse(dataGridView1.Rows[i].Cells["Nums"].Value.ToString()) +
                                                          Double.Parse(AllpriceLabel.Text.Replace("元", "").Trim())).ToString() + " 元";
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {
            }

        }

        //购物车中的编辑按钮
        private void ChangeLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string linknames = ((System.Windows.Forms.Control)(sender)).Name.Trim();
            string orderidstr = "";
            for (int i = 0; i < shopvalues; i++)
            {
                if (linknames == "ChangeLink" + i)
                {
                    foreach (Control cs in Application.OpenForms[1].Controls.Find("Nums" + i, true))
                    {
                        if (cs.Visible.Equals(true))
                            cs.Visible = !cs.Visible;
                    }

                    //DataSet ds = Database.RunDataSet("select a.* from  Orders a join ItemUp b on a.ItemUpID=b.ItemUpID join Product c on c.ProductID=b.ProductID where a.PersonID='" + PersonID + "' and c.ProductName='"+"sdaaaaaas"+"'");
                    string labelnames = "shoppingpan" + i;
                    foreach (Control c in Application.OpenForms[1].Controls.Find(labelnames, true))
                        foreach (Control control in Application.OpenForms[1].Controls.Find("Nums" + i, true))
                        {
                            //LinkLabel 的生成  删除按钮
                            LinkLabel DeleteLinks = new LinkLabel();
                            DeleteLinks.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
                            DeleteLinks.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                            DeleteLinks.Location = new System.Drawing.Point(0, 0);
                            DeleteLinks.Name = "DeleteLinks" + i;
                            DeleteLinks.Size = new System.Drawing.Size(60, 25);
                            DeleteLinks.Text = "删 除";
                            DeleteLinks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                            DeleteLinks.LinkClicked +=
                                new System.Windows.Forms.LinkLabelLinkClickedEventHandler(LinkLabels_LinkClicked);
                            //这里缺事件


                            //LinkLabel 的生成  减少按钮
                            LinkLabel reduceLinks = new LinkLabel();
                            reduceLinks.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
                            reduceLinks.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                            reduceLinks.Location = new System.Drawing.Point(0, 35);
                            reduceLinks.Name = "reduceLinks" + i;
                            reduceLinks.Size = new System.Drawing.Size(15, 30);
                            reduceLinks.Text = "-";
                            reduceLinks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                            reduceLinks.LinkClicked +=
                                new System.Windows.Forms.LinkLabelLinkClickedEventHandler(LinkLabels_LinkClicked);
                            //这里缺事件

                            //Label 的生成  购买数量
                            TextBox NumsTxt = new TextBox();
                            NumsTxt.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
                            NumsTxt.ForeColor = System.Drawing.SystemColors.WindowText;
                            NumsTxt.Size = new System.Drawing.Size(30, 30);
                            NumsTxt.Name = "NumsTxt" + i;
                            NumsTxt.ReadOnly = true;
                            NumsTxt.Text = control.Text.Replace("X", "").Trim();
                            NumsTxt.TextAlign = HorizontalAlignment.Center;
                            NumsTxt.Location = new System.Drawing.Point(15, 35);

                            //LinkLabel 的生成  添加按钮
                            LinkLabel addLinks = new LinkLabel();
                            addLinks.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
                            addLinks.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                            addLinks.Location = new System.Drawing.Point(45, 35);
                            addLinks.Name = "addLinks" + i;
                            addLinks.Size = new System.Drawing.Size(15, 30);
                            addLinks.Text = "+";
                            addLinks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                            addLinks.LinkClicked +=
                                new System.Windows.Forms.LinkLabelLinkClickedEventHandler(LinkLabels_LinkClicked);
                            //这里缺事件

                            //LinkLabel 的生成  关闭按钮
                            LinkLabel CloseLinks = new LinkLabel();
                            CloseLinks.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                            CloseLinks.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                            CloseLinks.Location = new System.Drawing.Point(0, 65);
                            CloseLinks.Name = "CloseLinks" + i;
                            CloseLinks.Size = new System.Drawing.Size(60, 20);
                            CloseLinks.Text = "关 闭";
                            CloseLinks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                            CloseLinks.LinkClicked +=
                                new System.Windows.Forms.LinkLabelLinkClickedEventHandler(LinkLabels_LinkClicked);


                            // Panel 的生成
                            Panel panel = new Panel();
                            panel.Controls.Add(DeleteLinks);
                            panel.Controls.Add(reduceLinks);
                            panel.Controls.Add(NumsTxt);
                            panel.Controls.Add(addLinks);
                            panel.Controls.Add(CloseLinks);
                            panel.BackColor = System.Drawing.Color.Transparent;
                            panel.Location = new System.Drawing.Point(500, 40);
                            panel.Size = new System.Drawing.Size(60, 85);
                            panel.Name = "changepanels" + i;
                            c.Controls.Add(panel);

                            //orderidstr = c.Text.Replace("订单号：","").Trim();
                        }
                }
            }
            for (int j = 0; j < ordersValue; j++)
            {
                if (linknames == "ChangeOSLink" + j)
                {
                    foreach (Control control in Application.OpenForms[1].Controls.Find("ordersIDs" + j, true))
                    {
                        string orderid = control.Text.Replace("订单号：", null).Trim();
                        DataSet ds = Database.RunDataSet("select * from  Orders where OrderID='" + orderid + "'");
                        DataTable dt = ds.Tables[0];
                        dt.Rows[0]["OrderStatesID"] = 3;
                        if (MessageBox.Show("你确定已收货吗？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                            DialogResult.Yes)
                        {
                            try
                            {
                                Int64 k = Database.update2("Orders", "OrderID", dt);
                                OrdersHistoryLink_LinkClicked(null, null);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
        }

        //购物车对订单的编辑炒作
        private void LinkLabels_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string ordernums = "";
            string linknames = ((System.Windows.Forms.Control)(sender)).Name.Trim();
            for (int i = 0; i < shopvalues; i++)
            {
                foreach (Control c in Application.OpenForms[1].Controls.Find("OrderID" + i, true))
                {
                    ordernums = c.Text.Replace("订单号：", "").Trim();
                }
                //删除
                if (linknames == "DeleteLinks" + i)
                {

                    if (MessageBox.Show("你确定要删除吗？", "温馨提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            int k = Database.ExecuteNonQuery("delete from Orders where OrderID='" + ordernums + "'");
                            MessageBox.Show("删除成功！", "温馨提示");
                            HistoryShoppingLink_LinkClicked(null, null);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("删除失败！", "温馨提示");
                        }
                    }
                }

                //数量减少
                if (linknames == "reduceLinks" + i)
                {
                    foreach (Control c in Application.OpenForms[1].Controls.Find("NumsTxt" + i, true))
                    {
                        if (!string.IsNullOrEmpty(c.Text.Trim()) && int.Parse(c.Text.Trim()) >= 0)
                        {
                            c.Text = (int.Parse(c.Text.Trim()) - 1).ToString();
                            try
                            {
                                int k =
                                    Database.ExecuteNonQuery("update  Orders set BuyNum='" + c.Text.Trim() +
                                                             "' where OrderID='" + ordernums + "'");
                                foreach (Control cs in Application.OpenForms[1].Controls.Find("Nums" + i, true))
                                    cs.Text = "X" + c.Text.Trim();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                //数量增加
                if (linknames == "addLinks" + i)
                {
                    foreach (Control c in Application.OpenForms[1].Controls.Find("NumsTxt" + i, true))
                    {
                        if (!string.IsNullOrEmpty(c.Text.Trim()) && int.Parse(c.Text.Trim()) >= 0)
                        {
                            c.Text = (int.Parse(c.Text.Trim()) + 1).ToString();
                            try
                            {
                                int k =
                                    Database.ExecuteNonQuery("update  Orders set BuyNum='" + c.Text.Trim() +
                                                             "' where OrderID='" + ordernums + "'");
                                foreach (Control cs in Application.OpenForms[1].Controls.Find("Nums" + i, true))
                                    cs.Text = "X" + c.Text.Trim();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                //关闭编辑界面 CloseLinks
                if (linknames == "CloseLinks" + i)
                {
                    foreach (Control c in Application.OpenForms[1].Controls.Find("changepanels" + i, true))
                    {
                        if (c.Visible.Equals(true))
                        {
                            c.Visible = !c.Visible;
                            foreach (Control cs in Application.OpenForms[1].Controls.Find("Nums" + i, true))
                                cs.Visible = !c.Visible;
                        }
                    }
                }
            }
        }

        //购物车的 选择
        private void CheckBoxs_CheckedChanged(object sender, EventArgs e)
        {
            TotalPriceLabel.Text = "0 元";
            for (int i = 0; i < shopvalues; i++)
            {
                foreach (Control c in Application.OpenForms[1].Controls.Find("checkBox" + i, true))
                {
                    if ((c as CheckBox).Checked == true)
                    {
                        foreach (Control Pricect in Application.OpenForms[1].Controls.Find("Price" + i, true))
                            foreach (Control Numsct in Application.OpenForms[1].Controls.Find("Nums" + i, true))
                            {
                                TotalPriceLabel.Text =
                                    (double.Parse(Pricect.Text.Replace("单价：", "").Trim()) * int.Parse(Numsct.Text.Replace("X", "").Trim()) +
                                     double.Parse(TotalPriceLabel.Text.Replace("元", "").Trim())).ToString() + " 元";
                            }
                    }
                }
            }
        }

        private void OKbtu_Click(object sender, EventArgs e)
        {

        }

        //购物车结算按钮
        private void OrdersBtu_Click(object sender, EventArgs e)
        {
            string ordersid = "", addressid = "";
            for (int i = 0; i < shopvalues; i++)
            {
                foreach (Control c in Application.OpenForms[1].Controls.Find("checkBox" + i, true))
                {
                    if ((c as CheckBox).Checked == true)
                    {
                        foreach (Control OrderIDct in Application.OpenForms[1].Controls.Find("OrderID" + i, true))
                            foreach (Control addressCt in Application.OpenForms[1].Controls.Find("Address" + i, true))
                            {
                                if ((addressCt as ComboBox).SelectedValue != null)
                                {
                                    ordersid += "+" + OrderIDct.Text.Replace("订单号：", " ").Trim();
                                    addressid += "+" + (addressCt as ComboBox).SelectedValue.ToString();
                                }
                                else
                                {
                                    ordersid += "+" + OrderIDct.Text.Replace("订单号：", " ").Trim();
                                    addressid += "+" + "无地址";
                                }
                            }
                    }
                }
            }
            if (!string.IsNullOrEmpty(ordersid))
            {
                if (MessageBox.Show("你确定付款吗？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    int k = ordersid.Split('+').Length;
                    string[] a = ordersid.Split('+');
                    string[] b = addressid.Split('+');

                    for (int i = 1; i < k; i++)
                    {
                        if (!string.IsNullOrEmpty(b[i]))
                        {
                            DataSet ds = Database.RunDataSet("select * from  Orders where OrderID='" + a[i] + "'");
                            DataTable dt = ds.Tables[0];
                            dt.Rows[0]["OrderStatesID"] = 2;
                            dt.Rows[0]["PayStates"] = 0;
                            dt.Rows[0]["BuyTime"] = DateTime.Now.ToString("s");
                            dt.Rows[0]["Comments"] = "这个好吃的很，有想要的快来哦，机不可失，失不再来。快点抓住机会哈！";
                            dt.Rows[0]["AddressID"] = b[i];
                            try
                            {
                                Int64 j = Database.update2("Orders", "OrderID", dt);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("抱歉，付款失败！", "温馨提示");
                            }
                        }
                        else
                        {
                            MessageBox.Show("抱歉，订单号为：" + a[i] + "的订单付款失败！", "温馨提示");
                        }
                    }
                    MessageBox.Show("恭喜，付款成功！", "温馨提示");
                }
                HistoryShoppingLink_LinkClicked(null, null);
                CheckBoxs_CheckedChanged(null, null);
            }
            ////一下其实也可以付款，但是在提示部分有bug 所以用了上面的方法
            //for (int i = 0; i < shopvalues; i++)
            //{
            //    foreach (Control c in Application.OpenForms[1].Controls.Find("checkBox" + i, true))
            //    {
            //        if ((c as CheckBox).Checked == true)
            //        {
            //            foreach (Control OrderIDct in Application.OpenForms[1].Controls.Find("OrderID" + i, true))
            //                foreach (Control addressCt in Application.OpenForms[1].Controls.Find("Address" + i, true))
            //                {
            //                    //    DataSet myds =
            //                    //          Database.RunDataSet("select * from  Address where PersonID='" + PersonID + "'");

            //                    if ((addressCt as ComboBox).SelectedValue != null)
            //                    {
            //                        DataSet ds =
            //                            Database.RunDataSet("select * from  Orders where OrderID='" +
            //                                                OrderIDct.Text.Replace("订单号：", " ").Trim() + "'");
            //                        DataTable dt = ds.Tables[0];
            //                        dt.Rows[0]["OrderStatesID"] = 2;
            //                        dt.Rows[0]["PayStates"] = 0;
            //                        dt.Rows[0]["BuyTime"] = DateTime.Now.ToString("s");
            //                        dt.Rows[0]["Comments"] = "这个好吃的很，有想要的快来哦，机不可失，失不再来。快点抓住机会哈！";
            //                        dt.Rows[0]["AddressID"] = (addressCt as ComboBox).SelectedValue.ToString();

            //                        if (
            //                            MessageBox.Show("你确定付款吗？", "温馨提示", MessageBoxButtons.YesNo,
            //                                MessageBoxIcon.Question) == DialogResult.Yes)
            //                        {
            //                            try
            //                            {
            //                                Int64 k = Database.update2("Orders", "OrderID", dt);


            //                            }
            //                            catch (Exception)
            //                            {
            //                                MessageBox.Show("抱歉，付款失败！", "温馨提示");
            //                                return;
            //                            }

            //                        }
            //                    }
            //                    else
            //                    {
            //                        MessageBox.Show("请选择地址！", "温馨提示");
            //                        return;
            //                    }
            //                }
            //        }
            //    }
            //}
            //MessageBox.Show("恭喜，付款成功！", "温馨提示");
            //HistoryShoppingLink_LinkClicked(null, null);
            //CheckBoxs_CheckedChanged(null, null);
        }

        private void CatalogComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet ds =
                Database.RunDataSet(
                    "select a.*,b.SellPrice from Product a join ItemUp b on a.ProductID=b.ProductID join Catalog c on a.CatalogID=c.CatalogID where b.ItemUpState='0' and c.CatalogID='" +
                    (this.CatalogComboBox.SelectedIndex + 1) + "'");
            DataTable dt = ds.Tables[0];
            storebalues = dt.Rows.Count;
            dataGridView1.Rows.Clear();
            //if (this.CatalogComboBox.SelectedIndex!=-1)
            if (storebalues >= 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    Image image = Image.FromFile(dt.Rows[i]["ImageSource"].ToString());
                    Image ReducedImage;
                    //Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    ReducedImage = image.GetThumbnailImage(dataGridView1.Rows[i].Cells["ImageSource"].Size.Width,
                    dataGridView1.Rows[i].Cells["ImageSource"].Size.Height, null, IntPtr.Zero);
                    dataGridView1[0, i].Value = ReducedImage;
                    //dataGridView1[1,i].Value = dt.Rows[i]["ProductName"].ToString();
                    dataGridView1.Rows[i].Cells["ProductName"].Value = dt.Rows[i]["ProductName"].ToString();
                    dataGridView1.Rows[i].Cells["SellPrice"].Value = ChangePrice(dt.Rows[i]["SellPrice"].ToString()) + "元";
                    dataGridView1.Rows[i].Cells["ProductInt"].Value = dt.Rows[i]["ProductInt"].ToString();//商品介绍
                    dataGridView1.Rows[i].Cells["Num"].Value = dt.Rows[i]["SellerNum"].ToString();//销量
                    dataGridView1.Rows[i].Cells["ReduceBtu"].Value = "-";
                    dataGridView1.Rows[i].Cells["Nums"].Value = "0";//购买数量
                    dataGridView1.Rows[i].Cells["AddBtu"].Value = "+";
                    dataGridView1.Rows[i].Cells["AddShoppingBtu"].Value = "加入购物车";
                }
            }
        }
    }
}
