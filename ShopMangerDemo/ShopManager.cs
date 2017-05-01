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
    public partial class ShopManager : Form
    {
        private static string phonenumber = "";
        private static string SecurityCodes = "";
        private static bool IsHaveSend = false; //是否已发送验证码 
        private string usernames = "";
        private int userid = 0;
        private int itemupKey = -1;
        private int HistoryKey = -1;

        public string username
        {
            set { usernames = value; }
        }

        public void SetValue()
        {
            this.usernameLabel.Text = usernames;
        }
        public ShopManager()
        {
            InitializeComponent();
        }

        private void ExitLB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            Login login = new Login();
            login.ShowDialog();
        }

        private void PasswordLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PasswordFrom PF = new PasswordFrom();
            PF.username = usernames;
            PF.ShowDialog();
        }

        //显示个人中心界面
        private void UserCenterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (UserCenterPanel.Visible.Equals(false))
            {
                ItemUpPanel.Visible = UserCenterPanel.Visible;
                AddShoppingPanel.Visible = UserCenterPanel.Visible;
                OrdersHistoryPanel.Visible = UserCenterPanel.Visible;
                UserCenterPanel.Visible = !UserCenterPanel.Visible;
            }
            DataSet ds = Database.RunDataSet("select * from Seller where SellerID='" + userid + "'");

            UserNameTXT.Text = ds.Tables[0].Rows[0]["SellerName"].ToString();
            AddressTXT.Text = ds.Tables[0].Rows[0]["SellerADD"].ToString();
            IntroductTXT.Text = ds.Tables[0].Rows[0]["SellerInt"].ToString();
            PhoneNumberTXT.Text = ds.Tables[0].Rows[0]["SellerPN"].ToString();
            StoresNameTXT.Text = ds.Tables[0].Rows[0]["StoresName"].ToString();
        }

        //显示已有商品界面
        private void ItemUpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ItemUpPanel.Visible.Equals(false))
            {
                AddShoppingPanel.Visible = ItemUpPanel.Visible;
                UserCenterPanel.Visible = ItemUpPanel.Visible;
                OrdersHistoryPanel.Visible = ItemUpPanel.Visible;
                ItemUpPanel.Visible = !ItemUpPanel.Visible;
            }
            DataSet dss = Database.RunDataSet("select * from Catalog");
            CatalogcbB.DataSource = dss.Tables[0];
            CatalogcbB.ValueMember = "CatalogID";
            CatalogcbB.DisplayMember = "CatalogName";
            CatalogcbB.SelectedIndex = -1;
            CatalogcbB.Text = "商品种类";

            UPpanels.Controls.Clear();
            DataSet ds = Database.RunDataSet("select a.*,b.CatalogName from Product a join Catalog b on a.CatalogID=b.CatalogID");
            GetPanel(ds);
        }

        //商品界面添加控件
        private void GetPanel( DataSet dataSet)
        {
            DataSet ds = dataSet;
            itemupKey = ds.Tables[0].Rows.Count;
            if (itemupKey > 0)
            {
                for (int i = 0; i < itemupKey; i++)
                {
                    DataSet myds =
                        Database.RunDataSet("select * from ItemUp where ItemUpState='0'and ProductID='" +
                                            ds.Tables[0].Rows[i]["ProductID"].ToString() + "'");
                    if (myds.Tables[0].Rows.Count == 0)
                    {
                        //Label 的生成  商品ID
                        Label OrdersIDlabel = new Label();
                        //label.AutoSize = true  ;
                        //nameLabel.BackColor = System.Drawing.Color.Beige;
                        OrdersIDlabel.BackColor = System.Drawing.Color.Transparent;
                        OrdersIDlabel.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
                        OrdersIDlabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        OrdersIDlabel.Size = new System.Drawing.Size(100, 30);
                        OrdersIDlabel.Name = "ProductID" + i;
                        OrdersIDlabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        OrdersIDlabel.Text = "商品ID：" + ds.Tables[0].Rows[i]["ProductID"].ToString();
                        OrdersIDlabel.Location = new System.Drawing.Point(10, 0);

                        //上架按钮
                        LinkLabel ChangeLink = new LinkLabel();
                        ChangeLink.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
                        ChangeLink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                        ChangeLink.Location = new System.Drawing.Point(540, 0);
                        ChangeLink.Name = "ItemUplinks" + i; //待商议
                        ChangeLink.Size = new System.Drawing.Size(60, 30);
                        ChangeLink.Text = "上 架";
                        ChangeLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        ChangeLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(ChangeLink_LinkClicked);//这里缺事件


                        //Label 的生成  图片
                        Label imageLabel = new Label();
                        //label.AutoSize = true  ;
                        imageLabel.Size = new System.Drawing.Size(85, 120);
                        imageLabel.Name = "ImageSource" + i;
                        if (string.IsNullOrEmpty(ds.Tables[0].Rows[i]["ImageSource"].ToString()))
                        {
                            imageLabel.Text = "暂无图片";
                            imageLabel.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
                            imageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                            imageLabel.BorderStyle = BorderStyle.FixedSingle;
                        }
                        else
                        {
                            Image image = Image.FromFile(ds.Tables[0].Rows[i]["ImageSource"].ToString());
                            Image ReducedImage = image.GetThumbnailImage(85, 120, null, IntPtr.Zero);
                            imageLabel.BackgroundImage = ReducedImage;
                        }
                        imageLabel.Location = new System.Drawing.Point(15, 30);


                        //Label 的生成  商品名称
                        Label nameLabel = new Label();
                        nameLabel.BackColor = System.Drawing.Color.Transparent;
                        nameLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        nameLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        nameLabel.Size = new System.Drawing.Size(250, 55);
                        nameLabel.Name = "ProductName" + i;
                        nameLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                        nameLabel.Text = "商品名称：" + ds.Tables[0].Rows[i]["ProductName"].ToString();
                        nameLabel.Location = new System.Drawing.Point(100, 30);



                        //RichTextBox 的生成  商品介绍
                        RichTextBox IntLabel = new RichTextBox();
                        //IntLabel.BackColor = System.Drawing.Color.Transparent;
                        IntLabel.ReadOnly = true;
                        IntLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        IntLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        IntLabel.Size = new System.Drawing.Size(250, 50);
                        IntLabel.Name = "ProductInt" + i;
                        IntLabel.Text = "商品介绍：" + ds.Tables[0].Rows[i]["ProductInt"].ToString();
                        IntLabel.Location = new System.Drawing.Point(100, 85);

                        //Label 的生成  种类
                        Label PriceLabel = new Label();
                        //label.AutoSize = true  ;
                        //PriceLabel.BackColor = System.Drawing.Color.Beige;
                        PriceLabel.BackColor = System.Drawing.Color.Transparent;
                        PriceLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        PriceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        PriceLabel.Size = new System.Drawing.Size(180, 30);
                        PriceLabel.Name = "CatalogName" + i;
                        PriceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        PriceLabel.Text = "商品种类：" + ds.Tables[0].Rows[i]["CatalogName"].ToString();
                        PriceLabel.Location = new System.Drawing.Point(360, 35);

                        //Label 的生成  销量
                        Label NumsLabel = new Label();
                        //label.AutoSize = true  ;
                        //NumsLabel.BackColor = System.Drawing.Color.Beige;
                        NumsLabel.BackColor = System.Drawing.Color.Transparent;
                        NumsLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        NumsLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        NumsLabel.Size = new System.Drawing.Size(100, 30);
                        NumsLabel.Name = "Nums" + i;
                        NumsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        NumsLabel.Text = "销量：" + ds.Tables[0].Rows[i]["Num"].ToString();
                        NumsLabel.Location = new System.Drawing.Point(360, 90);


                        //Label 的生成  是否已上架
                        Label ItemUpLabel = new Label();
                        //label.AutoSize = true  ;
                        //NumsLabel.BackColor = System.Drawing.Color.Beige;
                        ItemUpLabel.BackColor = System.Drawing.Color.Transparent;
                        ItemUpLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        ItemUpLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        ItemUpLabel.Size = new System.Drawing.Size(70, 30);
                        ItemUpLabel.Name = "Nums" + i;
                        ItemUpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        ItemUpLabel.Text = "未上架";
                        ItemUpLabel.Location = new System.Drawing.Point(540, 66);




                        // Panel 的生成
                        Panel panel = new Panel();
                        panel.Controls.Add(OrdersIDlabel);
                        panel.Controls.Add(imageLabel);
                        panel.Controls.Add(nameLabel);
                        panel.Controls.Add(IntLabel);
                        panel.Controls.Add(PriceLabel);
                        panel.Controls.Add(NumsLabel);
                        panel.Controls.Add(ItemUpLabel);
                        panel.Controls.Add(ChangeLink);
                        panel.BackColor = System.Drawing.Color.Red;
                        panel.Location = new System.Drawing.Point(0, i * 165);
                        panel.Size = new System.Drawing.Size(612, 160);
                        panel.Name = "shoppingpan" + i;
                        this.UPpanels.Controls.Add(panel);
                    }
                    else 
                    {
                        //Label 的生成  商品ID
                        Label OrdersIDlabel = new Label();
                        //label.AutoSize = true  ;
                        //nameLabel.BackColor = System.Drawing.Color.Beige;
                        OrdersIDlabel.BackColor = System.Drawing.Color.Transparent;
                        OrdersIDlabel.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
                        OrdersIDlabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        OrdersIDlabel.Size = new System.Drawing.Size(100, 30);
                        OrdersIDlabel.Name = "ProductID" + i;
                        OrdersIDlabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        OrdersIDlabel.Text = "商品ID：" + ds.Tables[0].Rows[i]["ProductID"].ToString();
                        OrdersIDlabel.Location = new System.Drawing.Point(10, 0);

                        //下架按钮
                        LinkLabel ChangeLink = new LinkLabel();
                        ChangeLink.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
                        ChangeLink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                        ChangeLink.Location = new System.Drawing.Point(540, 0);
                        ChangeLink.Name = "ItemDownlinks" + i; //待商议
                        ChangeLink.Size = new System.Drawing.Size(60, 30);
                        ChangeLink.Text = "下 架";
                        ChangeLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        ChangeLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(ChangeLink_LinkClicked);//这里缺事件


                        //Label 的生成  图片
                        Label imageLabel = new Label();
                        //label.AutoSize = true  ;
                        imageLabel.Size = new System.Drawing.Size(85, 120);
                        imageLabel.Name = "ImageSource" + i;
                        if (string.IsNullOrEmpty(ds.Tables[0].Rows[i]["ImageSource"].ToString()))
                        {
                            imageLabel.Text = "暂无图片";
                            imageLabel.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
                            imageLabel.BorderStyle = BorderStyle.FixedSingle;
                            imageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        }
                        else
                        {
                            Image image = Image.FromFile(ds.Tables[0].Rows[i]["ImageSource"].ToString());
                            Image ReducedImage = image.GetThumbnailImage(85, 120, null, IntPtr.Zero);
                            imageLabel.BackgroundImage = ReducedImage;
                        }
                        imageLabel.Location = new System.Drawing.Point(15, 30);


                        //Label 的生成  商品名称
                        Label nameLabel = new Label();
                        nameLabel.BackColor = System.Drawing.Color.Transparent;
                        nameLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        nameLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        nameLabel.Size = new System.Drawing.Size(250, 55);
                        nameLabel.Name = "ProductName" + i;
                        nameLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                        nameLabel.Text = "商品名称：" + ds.Tables[0].Rows[i]["ProductName"].ToString();
                        nameLabel.Location = new System.Drawing.Point(100, 30);



                        //RichTextBox 的生成  商品介绍              
                        RichTextBox IntLabel = new RichTextBox();
                        //IntLabel.BackColor = System.Drawing.Color.Transparent;
                        IntLabel.ReadOnly = true;
                        IntLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        IntLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        IntLabel.Size = new System.Drawing.Size(250, 50);
                        IntLabel.Name = "ProductInt" + i;
                        IntLabel.Text = "商品介绍：" + ds.Tables[0].Rows[i]["ProductInt"].ToString();
                        IntLabel.Location = new System.Drawing.Point(100, 85);

                        //Label 的生成  种类
                        Label PriceLabel = new Label();
                        //label.AutoSize = true  ;
                        //PriceLabel.BackColor = System.Drawing.Color.Beige;
                        PriceLabel.BackColor = System.Drawing.Color.Transparent;
                        PriceLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        PriceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        PriceLabel.Size = new System.Drawing.Size(180, 30);
                        PriceLabel.Name = "CatalogName" + i;
                        PriceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        PriceLabel.Text = "商品种类：" + ds.Tables[0].Rows[i]["CatalogName"].ToString();
                        PriceLabel.Location = new System.Drawing.Point(360, 35);

                        //Label 的生成  销量
                        Label NumsLabel = new Label();
                        //label.AutoSize = true  ;
                        //NumsLabel.BackColor = System.Drawing.Color.Beige;
                        NumsLabel.BackColor = System.Drawing.Color.Transparent;
                        NumsLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        NumsLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        NumsLabel.Size = new System.Drawing.Size(100, 30);
                        NumsLabel.Name = "Nums" + i;
                        NumsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        NumsLabel.Text = "销量：" + ds.Tables[0].Rows[i]["SellerNum"].ToString();
                        NumsLabel.Location = new System.Drawing.Point(360, 90);


                        //Label 的生成  是否已上架
                        Label ItemUpLabel = new Label();
                        //label.AutoSize = true  ;
                        //NumsLabel.BackColor = System.Drawing.Color.Beige;
                        ItemUpLabel.BackColor = System.Drawing.Color.Transparent;
                        ItemUpLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                        ItemUpLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                        ItemUpLabel.Size = new System.Drawing.Size(70, 30);
                        ItemUpLabel.Name = "Nums" + i;
                        ItemUpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        ItemUpLabel.Text = "已上架";
                        ItemUpLabel.Location = new System.Drawing.Point(540, 66);

                        // Panel 的生成
                        Panel panel = new Panel();
                        panel.Controls.Add(OrdersIDlabel);
                        panel.Controls.Add(imageLabel);
                        panel.Controls.Add(nameLabel);
                        panel.Controls.Add(IntLabel);
                        panel.Controls.Add(PriceLabel);
                        panel.Controls.Add(NumsLabel);
                        panel.Controls.Add(ChangeLink);
                        panel.Controls.Add(ItemUpLabel);
                        panel.BackColor = System.Drawing.Color.Red;
                        panel.Location = new System.Drawing.Point(0, i * 165);
                        panel.Size = new System.Drawing.Size(612, 160);
                        panel.Name = "shoppingpan" + i;
                        this.UPpanels.Controls.Add(panel);
                    }
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
                label.Text = "抱歉 无符合你需要的商品！";
                label.Location = new System.Drawing.Point(0, 0);

                this.UPpanels.Controls.Add(label);
            }
        }


        //历史记录界面添加控件
        private void GetConctors(DataSet dataSet)
        {
            DataSet ds = dataSet;
            HistoryKey = ds.Tables[0].Rows.Count;
            if (HistoryKey > 0)
            {
                for (int i = 0; i < HistoryKey; i++)
                {
                    //Label 的生成  订单ID
                    Label OrdersIDlabel = new Label();
                    //label.AutoSize = true  ;
                    //nameLabel.BackColor = System.Drawing.Color.Beige;
                    OrdersIDlabel.BackColor = System.Drawing.Color.Transparent;
                    OrdersIDlabel.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
                    OrdersIDlabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    OrdersIDlabel.Size = new System.Drawing.Size(200, 30);
                    OrdersIDlabel.Name = "OrderID" + i;
                    OrdersIDlabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    OrdersIDlabel.Text = "订单ID：" + ds.Tables[0].Rows[i]["OrderID"].ToString();
                    OrdersIDlabel.Location = new System.Drawing.Point(10, 0);


                    //Label 的生成  下单日期
                    Label OrdersTimelabel = new Label();
                    //label.AutoSize = true  ;
                    //nameLabel.BackColor = System.Drawing.Color.Beige;
                    OrdersTimelabel.BackColor = System.Drawing.Color.Transparent;
                    OrdersTimelabel.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
                    OrdersTimelabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    OrdersTimelabel.Size = new System.Drawing.Size(270, 30);
                    OrdersTimelabel.Name = "OrderTime" + i;
                    OrdersTimelabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    OrdersTimelabel.Text = "下单日期：" + ds.Tables[0].Rows[i]["OrderTime"].ToString();
                    OrdersTimelabel.Location = new System.Drawing.Point(340, 0);



                    //Label 的生成  图片
                    Label imageLabel = new Label();
                    //label.AutoSize = true  ;
                    imageLabel.Size = new System.Drawing.Size(85, 120);
                    imageLabel.Name = "ImageSource" + i;
                    if (string.IsNullOrEmpty(ds.Tables[0].Rows[i]["ImageSource"].ToString()))
                    {
                        imageLabel.Text = "暂无图片";
                        imageLabel.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
                        imageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        imageLabel.BorderStyle = BorderStyle.FixedSingle;
                    }
                    else
                    {
                        Image image = Image.FromFile(ds.Tables[0].Rows[i]["ImageSource"].ToString());
                        Image ReducedImage = image.GetThumbnailImage(85, 120, null, IntPtr.Zero);
                        imageLabel.BackgroundImage = ReducedImage;
                    }
                    imageLabel.Location = new System.Drawing.Point(15, 30);

                    //Label 的生成  上架商品ID
                    Label ItemUpIDLabel = new Label();
                    ItemUpIDLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    ItemUpIDLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    ItemUpIDLabel.Size = new System.Drawing.Size(100, 30);
                    ItemUpIDLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    ItemUpIDLabel.Name = "ItemUpID" + i;
                    ItemUpIDLabel.Text = "上架ID：" + ds.Tables[0].Rows[i]["ItemUpID"].ToString();
                    ItemUpIDLabel.Location = new System.Drawing.Point(100, 30);

                    //Label 的生成  商品名称
                    Label nameLabel = new Label();
                    nameLabel.BackColor = System.Drawing.Color.Transparent;
                    nameLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    nameLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    nameLabel.Size = new System.Drawing.Size(150, 30);
                    nameLabel.Name = "ProductName" + i;
                    nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    nameLabel.Text = "商品名称：" + ds.Tables[0].Rows[i]["ProductName"].ToString();
                    nameLabel.Location = new System.Drawing.Point(250, 30);


                    //Label 的生成  商品种类
                    Label CatalogLabel = new Label();
                    //label.AutoSize = true  ;
                    //PriceLabel.BackColor = System.Drawing.Color.Beige;
                    CatalogLabel.BackColor = System.Drawing.Color.Transparent;
                    CatalogLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    CatalogLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    CatalogLabel.Size = new System.Drawing.Size(210, 30);
                    CatalogLabel.Name = "CatalogName" + i;
                    CatalogLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    CatalogLabel.Text = "商品种类：" + ds.Tables[0].Rows[i]["CatalogName"].ToString();
                    CatalogLabel.Location = new System.Drawing.Point(400, 30);


                    //TextBox 的生成  用户名
                    Label PersonNameLabel = new Label();
                    PersonNameLabel.BackColor = System.Drawing.Color.Transparent;
                    PersonNameLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    PersonNameLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    PersonNameLabel.Size = new System.Drawing.Size(150, 25);
                    PersonNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    PersonNameLabel.Name = "PersonName" + i;
                    PersonNameLabel.Text = "用户名：" + ds.Tables[0].Rows[i]["PersonName"].ToString();
                    PersonNameLabel.Location = new System.Drawing.Point(100, 60);

                    //Label 的生成  用户地址
                    Label AddressLabel = new Label();
                    AddressLabel.BackColor = System.Drawing.Color.Transparent;
                    AddressLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    AddressLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    AddressLabel.Size = new System.Drawing.Size(250, 25);
                    AddressLabel.Name = "AddressName" + i;
                    AddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    AddressLabel.Location = new System.Drawing.Point(250, 60);
                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["AddressID"].ToString()))
                    {
                        DataSet AddressDS = Database.RunDataSet("select * from Address where AddressID='" + ds.Tables[0].Rows[i]["AddressID"].ToString() + "'");
                        AddressLabel.Text = "用户地址：" + AddressDS.Tables[0].Rows[0]["AddressName"].ToString();
                    }
                    else
                    {
                        AddressLabel.Text = "用户地址：未知";

                    }

                    //Label 的生成  订单状态
                    Label OrderStatesNameLabel = new Label();
                    OrderStatesNameLabel.BackColor = System.Drawing.Color.Transparent;
                    OrderStatesNameLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    OrderStatesNameLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    OrderStatesNameLabel.Size = new System.Drawing.Size(150, 25);
                    OrderStatesNameLabel.Name = "OrderStatesName" + i;
                    OrderStatesNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    OrderStatesNameLabel.Text = "订单状态:" + ds.Tables[0].Rows[i]["OrderStatesName"].ToString();
                    OrderStatesNameLabel.Location = new System.Drawing.Point(100, 85);

                    
                    //Label 的生成  支付状态
                    Label PayStatesLabel = new Label();
                    PayStatesLabel.BackColor = System.Drawing.Color.Transparent;
                    PayStatesLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    PayStatesLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    PayStatesLabel.Size = new System.Drawing.Size(150, 25);
                    PayStatesLabel.Name = "PayStates" + i;
                    PayStatesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    PayStatesLabel.Location = new System.Drawing.Point(250, 85);
                    if (ds.Tables[0].Rows[i]["PayStates"].ToString()=="0")
                    {
                        PayStatesLabel.Text = "支付状态:已支付";
                    }
                    else if (ds.Tables[0].Rows[i]["PayStates"].ToString()=="1")
                    {
                        PayStatesLabel.Text = "支付状态:未支付";
                    }
                    else
                    {
                        PayStatesLabel.Text = "支付状态:不知";
                        
                    }

                    //Label 的生成  用户电话
                    Label PersonPNLabel = new Label();
                    PersonPNLabel.BackColor = System.Drawing.Color.Transparent;
                    PersonPNLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    PersonPNLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    PersonPNLabel.Size = new System.Drawing.Size(200, 25);
                    PersonPNLabel.Name = "PersonPN" + i;
                    PersonPNLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    PersonPNLabel.Text = "用户电话:" + ds.Tables[0].Rows[i]["PersonPN"].ToString();
                    PersonPNLabel.Location = new System.Drawing.Point(400, 85);

                    ////Label 的生成  备注
                    //RichTextBox CommentsLabels = new RichTextBox();
                    //CommentsLabels.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    //CommentsLabels.ForeColor = System.Drawing.SystemColors.WindowText;
                    //CommentsLabels.Size = new System.Drawing.Size(510, 50);
                    //CommentsLabels.Name = "Comments" + i;
                    //CommentsLabels.Text = "备注:" + ds.Tables[0].Rows[i]["Comments"].ToString();
                    //CommentsLabels.Location = new System.Drawing.Point(100, 110);

                    //Label 的生成  备注
                    Label CommentsLabels = new Label();
                    PersonPNLabel.BackColor = System.Drawing.Color.Transparent;
                    CommentsLabels.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    CommentsLabels.ForeColor = System.Drawing.SystemColors.WindowText;
                    CommentsLabels.Size = new System.Drawing.Size(350, 48);
                    CommentsLabels.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                    CommentsLabels.BorderStyle=BorderStyle.FixedSingle;
                    CommentsLabels.Name = "Comments" + i;
                    CommentsLabels.Text = "备注:" + ds.Tables[0].Rows[i]["Comments"].ToString();
                    CommentsLabels.Location = new System.Drawing.Point(100, 110);

                    //Label 的生成  单价
                    Label PriceLabel = new Label();
                    PriceLabel.BackColor = System.Drawing.Color.Transparent;
                    PriceLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    PriceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    PriceLabel.Size = new System.Drawing.Size(100, 25);
                    PriceLabel.Name = "SellPrice" + i;
                    PriceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    PriceLabel.Text = "单价:" + ds.Tables[0].Rows[i]["SellPrice"].ToString().Remove(4);
                    PriceLabel.Location = new System.Drawing.Point(450, 110);

                    //Label 的生成  购买数量
                    Label NumsLabel = new Label();
                    NumsLabel.BackColor = System.Drawing.Color.Transparent;
                    NumsLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    NumsLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    NumsLabel.Size = new System.Drawing.Size(60, 25);
                    NumsLabel.Name = "BuyNum" + i;
                    NumsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    NumsLabel.Text = "X " + ds.Tables[0].Rows[i]["BuyNum"].ToString();
                    NumsLabel.Location = new System.Drawing.Point(550, 110);

                    //Label 的生成  总金额
                    Label AllPriceLabel = new Label();
                    AllPriceLabel.BackColor = System.Drawing.Color.Transparent;
                    AllPriceLabel.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
                    AllPriceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
                    AllPriceLabel.Size = new System.Drawing.Size(160, 25);
                    AllPriceLabel.Name = "AllPrice" + i;
                    AllPriceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    ShopCenter sc=new ShopCenter();
                    AllPriceLabel.Text = "总金额：" + sc.ChangePrice(ds.Tables[0].Rows[i]["AllPrice"].ToString()) ;
                    AllPriceLabel.Location = new System.Drawing.Point(450, 135);


                    // Panel 的生成
                    Panel panel = new Panel();
                    panel.Controls.Add(OrdersIDlabel);
                    panel.Controls.Add(OrdersTimelabel);
                    panel.Controls.Add(ItemUpIDLabel);
                    panel.Controls.Add(imageLabel);
                    panel.Controls.Add(nameLabel);
                    panel.Controls.Add(CatalogLabel);
                    panel.Controls.Add(PersonNameLabel);
                    panel.Controls.Add(AddressLabel);
                    panel.Controls.Add(OrderStatesNameLabel);
                    panel.Controls.Add(PayStatesLabel);
                    panel.Controls.Add(PersonPNLabel);
                    panel.Controls.Add(CommentsLabels);
                    panel.Controls.Add(PriceLabel);
                    panel.Controls.Add(NumsLabel);
                    panel.Controls.Add(AllPriceLabel);
                    panel.BackColor = System.Drawing.Color.Red;
                    panel.Location = new System.Drawing.Point(0, i*165);
                    panel.Size = new System.Drawing.Size(612, 160);
                    this.Historypanels.Controls.Add(panel);
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
                label.Text = "抱歉 暂无符合你需要的订单！";
                label.Location = new System.Drawing.Point(0, 0);

                this.Historypanels.Controls.Add(label);
            }

        }
        //上架与 下架按钮
        private void ChangeLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string linknames = ((System.Windows.Forms.Control)(sender)).Name.Trim();

            for (int i = 0; i < itemupKey; i++)
            {
                if (linknames == "ItemDownlinks"+i)
                {
                    if (MessageBox.Show("你确定要下架吗？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
                    {
                        foreach (Control a in Application.OpenForms[1].Controls.Find("ProductID" + i, true))
                        {
                            DataSet ds = Database.RunDataSet("select * from ItemUp where ItemUpState='0' and ProductID='" +
                                                                    a.Text.Replace("商品ID：", null).Trim() + "'");
                            //for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                            //{
                            //    if (ds.Tables[0].Rows[0]["ItemUpState"].ToString().Equals(0))
                            //    {
                                    
                            //    }
                            //}
                            DataTable dt = ds.Tables[0];
                            ds.Tables[0].Rows[0]["ItemUpState"] = 1;//0表示上架，1=下架
                            try
                            {
                                int k = Database.update("ItemUp", "ItemUpID", dt);
                                MessageBox.Show("恭喜你，下架成功.", "温馨提示");
                                ItemUpLink_LinkClicked(null, null);

                            }
                            catch (Exception)
                            {
                                MessageBox.Show("很抱歉，下架失败.", "温馨提示");
                            }
                        }
                    }
                }
                else if (linknames == "ItemUplinks" + i)
                {
                    foreach (Control a in Application.OpenForms[1].Controls.Find("ProductName" + i, true))
                        foreach (Control b in Application.OpenForms[1].Controls.Find("ProductInt" + i, true))
                            foreach (Control c in Application.OpenForms[1].Controls.Find("CatalogName" + i, true))
                                foreach (Control d in Application.OpenForms[1].Controls.Find("ProductID" + i, true))
                                {
                                    DataSet ds =
                                        Database.RunDataSet("select * from Product where ProductID='" +
                                                            d.Text.Replace("商品ID：", null).Trim() + "'");

                                    ItemUp from = new ItemUp();
                                    from.SetValue(ds.Tables[0].Rows[0]["ImageSource"].ToString(),
                                        ds.Tables[0].Rows[0]["ProductName"].ToString(),
                                        ds.Tables[0].Rows[0]["ProductInt"].ToString(),
                                        c.Text.Replace("商品种类：", null).Trim(),
                                        ds.Tables[0].Rows[0]["ProductID"].ToString(), usernames);
                                    from.ShowDialog();
                                }
                    ItemUpLink_LinkClicked(null, null);
                }
            }

        }


        //显示添加商品界面
        private void AddShoppingLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (AddShoppingPanel.Visible.Equals(false))
            {
                ItemUpPanel.Visible = AddShoppingPanel.Visible;
                UserCenterPanel.Visible = AddShoppingPanel.Visible;
                OrdersHistoryPanel.Visible = AddShoppingPanel.Visible;
                AddShoppingPanel.Visible = !AddShoppingPanel.Visible;
            }

            DataSet ds = Database.RunDataSet("select * from Catalog");
            CatalogcomboBox.DataSource = ds.Tables[0];
            CatalogcomboBox.ValueMember = "CatalogID";
            CatalogcomboBox.DisplayMember = "CatalogName";
        }
       
        
        //显示订单记录界面
        private void OrdersHistoryLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (OrdersHistoryPanel.Visible.Equals(false))
            {
                ItemUpPanel.Visible = OrdersHistoryPanel.Visible;
                AddShoppingPanel.Visible = OrdersHistoryPanel.Visible;
                UserCenterPanel.Visible = OrdersHistoryPanel.Visible;
                OrdersHistoryPanel.Visible = !OrdersHistoryPanel.Visible;
            }
            //先将页面隐藏  等所有控件加载完毕再显示
            this.OrdersHistoryPanel.Visible = !this.OrdersHistoryPanel.Visible;
            //绑定商品种类
            DataSet dss = Database.RunDataSet("select * from Catalog");
            CataLogcbbOrders.DataSource = dss.Tables[0];
            CataLogcbbOrders.ValueMember = "CatalogID";
            CataLogcbbOrders.DisplayMember = "CatalogName";
            CataLogcbbOrders.SelectedIndex = -1;
            CataLogcbbOrders.Text = "商品种类";
            //绑定订单状态
            DataSet OrderStatesDS = Database.RunDataSet("select * from OrderStates");
            OrderStatesCombobox.DataSource = OrderStatesDS.Tables[0];
            OrderStatesCombobox.ValueMember = "OrderStatesID";
            OrderStatesCombobox.DisplayMember = "OrderStatesName";
            OrderStatesCombobox.SelectedIndex = -1;
            OrderStatesCombobox.Text = "订单状态";
            Historypanels.Controls.Clear();
            DataSet ds =
                Database.RunDataSet(
                    "select a.*,b.OrderStatesName,d.SellPrice,e.ProductName,e.ImageSource,f.PersonName,f.PersonPN,h.CatalogName from Orders a join OrderStates b on a.OrderStatesID=b.OrderStatesID join ItemUp d on a.ItemUpID=d.ItemUpID join Product e on d.ProductID=e.ProductID join Person f on f.PersonID=a.PersonID join Catalog h on e.CatalogID=h.CatalogID");
            GetConctors(ds);
            this.OrdersHistoryPanel.Visible = !this.OrdersHistoryPanel.Visible;
        }
        //选择图片
        private void SelectImgBtu_Click(object sender, EventArgs e)
        {
            this.openFileDialog.ShowDialog();
            string filenames=Path.GetExtension(openFileDialog.FileName.Trim());
            if (filenames == ".jpg" || filenames == ".png" || filenames == ".jpeg" || filenames == ".bmp" ||
                filenames == ".gif")
            {
                ImgLabel.Text = "";
                Image image = Image.FromFile(openFileDialog.FileName.Trim());
                Image ReducedImage = image.GetThumbnailImage(ImgLabel.Width, ImgLabel.Height, null, IntPtr.Zero);
                ImgLabel.BackgroundImage = ReducedImage;
            }
            else
            {
                openFileDialog.FileName = "";
                errorProvider1.SetError(SelectImgBtu, "只能添加图片！");
            }
        }

        //添加商品保存按钮
        private void SaveBtu_Click(object sender, EventArgs e)
        {
            DataSet ds = Database.RunDataSet("select * from Product ");
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["ProductName"].ToString() == ProductNameTXt.Text.Trim())
                    {
                        errorProvider1.SetError(ProductNameTXt, "该商品已存在！");
                        ProductNameTXt.Focus();
                        return;
                    }
                }
            }


            if (MessageBox.Show("你确定要添加商品吗？","温馨提示",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
            {
                DataSet Productds = Database.RunDataSet("select * from Product where 0=1");
                DataTable ProductDT = Productds.Tables[0];
                DataRow ProductRow = ProductDT.NewRow();
                ProductRow["ProductID"] = 0;
                //ProductRow["CatalogID"] = int.Parse(CatalogcomboBox.SelectedIndex.ToString()) + 1;
                ProductRow["CatalogID"] = CatalogcomboBox.SelectedValue.ToString();
                ProductRow["ProductName"] = ProductNameTXt.Text.Trim();
                ProductRow["Num"] = 0;//销量
                ProductRow["ProductInt"] = ProductIntTXt.Text.Trim();
                ProductRow["ImageSource"] = openFileDialog.FileName.Trim();
                ProductDT.Rows.Add(ProductRow);


                try
                {
                    int k = Database.update("Product", "ProductID", ProductDT);
                    MessageBox.Show("商品添加成功！", "温馨提示");

                    //刷新界面
                    ProductNameTXt.Text = "";
                    ProductIntTXt.Text = "";
                    ImgLabel.Text = "暂无图片";
                    ImgLabel.BackgroundImage = label3.BackgroundImage;
                    AddShoppingLink_LinkClicked(null, null);

                }
                catch (Exception)
                {
                    MessageBox.Show("商品添加失败！", "温馨提示");
                } 
            }

        }

        private void ShopManager_Load(object sender, EventArgs e)
        {
            DataSet ds = Database.RunDataSet("select * from Seller where SellerName='" + usernames + "'");
            userid = int.Parse(ds.Tables[0].Rows[0]["SellerID"].ToString());

            //加载时更新查询时间
            Times2.Value = DateTime.Now;
            Times1.Value =  DateTime.Now.AddDays(-1);
            

        }

        //个人中心保存按钮
        private void UserCenterOK_Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataSet ds = Database.RunDataSet("select * from Seller where SellerID='" + userid + "'");
            DataTable dt = ds.Tables[0];
            DataRow dr = dt.Rows[0];
            dr["SellerInt"] = this.IntroductTXT.Text.Trim();
            dr["StoresName"] = this.StoresNameTXT.Text.Trim();
            dr["SellerADD"] = this.AddressTXT.Text.Trim();
            if ((IsHaveSend && this.SecurityCodeTXT.Text.Trim() == SecurityCodes))
            {
                dr["SellerPN"] = this.PhoneNumberTXT.Text.Trim();
            }
            if (MessageBox.Show("你确定要保存修改吗？", "温馨提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int k = Database.update("Seller", "SellerID", dt);
                    MessageBox.Show("修改保存成功！", "温馨提示");
                }
                catch (Exception)
                {
                    MessageBox.Show("修改保存失败！", "温馨提示");
                }
            }
            UserCenterLink_LinkClicked(null,null);
        }

        //以下两项都是相互关联的
        //已有商品的 按商品种类查询
        private void CatalogcbB_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet ds=new DataSet();
            if (IsItemUpCBx.SelectedIndex.ToString() == "-1")
            {
                ds =
                    Database.RunDataSet(
                        "select a.*,b.CatalogName from Product a join Catalog b on a.CatalogID=b.CatalogID where b.CatalogID='" +
                        (int.Parse(CatalogcbB.SelectedIndex.ToString()) + 1) + "'");
            }
            else
            {
                if (IsItemUpCBx.SelectedIndex.ToString() == "0")
                {
                    ds =
                    Database.RunDataSet(
                        "select a.*,b.CatalogName from Product a join Catalog b on a.CatalogID=b.CatalogID where b.CatalogID='" +
                        (int.Parse(CatalogcbB.SelectedIndex.ToString()) + 1) +
                        "'and a.ProductID  in (select b.ProductID from ItemUp a join Product b on a.ProductID=b.ProductID where ItemUpState='" +
                        IsItemUpCBx.SelectedIndex.ToString() + "')");
                }
                else if (IsItemUpCBx.SelectedIndex.ToString() == "1")
                {
                    ds =
                    Database.RunDataSet(
                        "select a.*,b.CatalogName from Product a join Catalog b on a.CatalogID=b.CatalogID where b.CatalogID='" +
                        (int.Parse(CatalogcbB.SelectedIndex.ToString()) + 1) +
                        "'and a.ProductID  in (select b.ProductID from ItemUp a join Product b on a.ProductID=b.ProductID where ItemUpState='" +
                        IsItemUpCBx.SelectedIndex.ToString() + "') or a.ProductID not in(select ProductID from ItemUp)");
                }
            }

            UPpanels.Controls.Clear();
            GetPanel(ds);
        }

        //查看是否已上架
        private void IsItemUpCBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet ds=new DataSet();
            if (CatalogcbB.SelectedIndex.ToString()=="-1")
            {
                if (IsItemUpCBx.SelectedIndex.ToString()=="0")
                {
                    ds =
                Database.RunDataSet("select a.*,b.CatalogName from Product a join Catalog b on a.CatalogID=b.CatalogID where a.ProductID  in (select b.ProductID from ItemUp a join Product b on a.ProductID=b.ProductID where ItemUpState='" +
                               IsItemUpCBx.SelectedIndex.ToString() + "') ");
                }
                else if (IsItemUpCBx.SelectedIndex.ToString()=="1")
                {
                    ds =
                 Database.RunDataSet("select a.*,b.CatalogName from Product a join Catalog b on a.CatalogID=b.CatalogID where a.ProductID  in (select b.ProductID from ItemUp a join Product b on a.ProductID=b.ProductID where ItemUpState='" +
                                IsItemUpCBx.SelectedIndex.ToString() + "') or a.ProductID not in(select ProductID from ItemUp)"); 
                }
                 
            }
            else
            {
                if (IsItemUpCBx.SelectedIndex.ToString() == "0")
                {
                    ds =
                    Database.RunDataSet(
                        "select a.*,b.CatalogName from Product a join Catalog b on a.CatalogID=b.CatalogID where b.CatalogID='" +
                        (int.Parse(CatalogcbB.SelectedIndex.ToString()) + 1) +
                        "'and a.ProductID  in (select b.ProductID from ItemUp a join Product b on a.ProductID=b.ProductID where ItemUpState='" +
                        IsItemUpCBx.SelectedIndex.ToString() + "')");
                }
                else if (IsItemUpCBx.SelectedIndex.ToString() == "1")
                {
                    ds =
                    Database.RunDataSet(
                        "select a.*,b.CatalogName from Product a join Catalog b on a.CatalogID=b.CatalogID where b.CatalogID='" +
                        (int.Parse(CatalogcbB.SelectedIndex.ToString()) + 1) +
                        "'and a.ProductID  in (select b.ProductID from ItemUp a join Product b on a.ProductID=b.ProductID where ItemUpState='" +
                        IsItemUpCBx.SelectedIndex.ToString() + "') or a.ProductID not in(select ProductID from ItemUp)");
                }
            }
            
            UPpanels.Controls.Clear();
            GetPanel(ds);
        }

        //搜索按钮
        private void SearchBtu_Click(object sender, EventArgs e)
        {
            DataSet ds = Database.RunDataSet("select a.*,b.CatalogName from Product a join Catalog b on a.CatalogID=b.CatalogID where a.ProductName like '%" + this.SearchTXT.Text.Trim() + "%'");
            UPpanels.Controls.Clear();
            GetPanel(ds);
        }

        //订单记录中商品种类 分类
        private void CataLogcbbOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            if (OrderStatesCombobox.SelectedIndex.ToString()=="-1")
            {
                ds = Database.RunDataSet("select a.*,b.OrderStatesName,d.SellPrice,e.ProductName,e.ImageSource,f.PersonName,f.PersonPN,h.CatalogName from Orders a join OrderStates b on a.OrderStatesID=b.OrderStatesID join ItemUp d on a.ItemUpID=d.ItemUpID join Product e on d.ProductID=e.ProductID join Person f on f.PersonID=a.PersonID join Catalog h on e.CatalogID=h.CatalogID where h.CatalogID='" +
                        (int.Parse(CataLogcbbOrders.SelectedIndex.ToString()) + 1) + "'");
            }
            else
            {
                ds = Database.RunDataSet("select a.*,b.OrderStatesName,d.SellPrice,e.ProductName,e.ImageSource,f.PersonName,f.PersonPN,h.CatalogName from Orders a join OrderStates b on a.OrderStatesID=b.OrderStatesID join ItemUp d on a.ItemUpID=d.ItemUpID join Product e on d.ProductID=e.ProductID join Person f on f.PersonID=a.PersonID join Catalog h on e.CatalogID=h.CatalogID where h.CatalogID='" +
                        (int.Parse(CataLogcbbOrders.SelectedIndex.ToString()) + 1) + "' and b.OrderStatesID='" + (int.Parse(OrderStatesCombobox.SelectedIndex.ToString()) + 1)+"'");
            }
           
            
            Historypanels.Controls.Clear();
            GetConctors(ds);
        }

        //订单记录中通过 订单状态查看
        //其实OrderStatesCombobox.SelectedIndex 是整型  int
        private void OrderStatesCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            if (CataLogcbbOrders.SelectedIndex==-1)
            {
                ds = Database.RunDataSet("select a.*,b.OrderStatesName,d.SellPrice,e.ProductName,e.ImageSource,f.PersonName,f.PersonPN,h.CatalogName from Orders a join OrderStates b on a.OrderStatesID=b.OrderStatesID join ItemUp d on a.ItemUpID=d.ItemUpID join Product e on d.ProductID=e.ProductID join Person f on f.PersonID=a.PersonID join Catalog h on e.CatalogID=h.CatalogID where b.OrderStatesID='" +
                        (int.Parse(OrderStatesCombobox.SelectedIndex.ToString()) + 1) + "'");
            }
            else
            {
                ds = Database.RunDataSet("select a.*,b.OrderStatesName,d.SellPrice,e.ProductName,e.ImageSource,f.PersonName,f.PersonPN,h.CatalogName from Orders a join OrderStates b on a.OrderStatesID=b.OrderStatesID join ItemUp d on a.ItemUpID=d.ItemUpID join Product e on d.ProductID=e.ProductID join Person f on f.PersonID=a.PersonID join Catalog h on e.CatalogID=h.CatalogID where h.CatalogID='" +
                        (int.Parse(CataLogcbbOrders.SelectedIndex.ToString()) + 1) + "' and b.OrderStatesID='" + (int.Parse(OrderStatesCombobox.SelectedIndex.ToString()) + 1) + "'");
            }
            Historypanels.Controls.Clear();
            GetConctors(ds);
            
        }

        //根据日期查看订单 开始日期
        private void Times1_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt1 = this.Times1.Value;
            DateTime dt2 = this.Times2.Value;
            if (dt1 > dt2)
            {
                this.Times1.Value = dt2;
                MessageBox.Show("这个时间不能大于结束时间");
            }
        }
        //结束日期  这里要 提出 不能小于开始日期
        private void Times2_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt1=this.Times1.Value.Date;
            DateTime dt2 = this.Times2.Value.Date;
            if (dt1>dt2)
            {
                this.Times2.Value = dt1;
                MessageBox.Show("这个时间不能小于起始时间");
            }
        }

        private void TimeSearch_Btu_Click(object sender, EventArgs e)
        {
            DateTime dt1 = this.Times1.Value.Date;
            DateTime dt2 = this.Times2.Value.Date.AddDays(1);
            DataSet ds = new DataSet();
            if (CataLogcbbOrders.SelectedIndex == -1)
            {
                if (OrderStatesCombobox.SelectedIndex==-1)
                {
                    ds =
                        Database.RunDataSet(
                            "select a.*,b.OrderStatesName,d.SellPrice,e.ProductName,e.ImageSource,f.PersonName,f.PersonPN,h.CatalogName from Orders a join OrderStates b on a.OrderStatesID=b.OrderStatesID join ItemUp d on a.ItemUpID=d.ItemUpID join Product e on d.ProductID=e.ProductID join Person f on f.PersonID=a.PersonID join Catalog h on e.CatalogID=h.CatalogID where a.OrderTime between'" +
                            dt1 + "' and '" + dt2 + "'");
                }
                else
                {
                    ds =
                        Database.RunDataSet(
                            "select a.*,b.OrderStatesName,d.SellPrice,e.ProductName,e.ImageSource,f.PersonName,f.PersonPN,h.CatalogName from Orders a join OrderStates b on a.OrderStatesID=b.OrderStatesID join ItemUp d on a.ItemUpID=d.ItemUpID join Product e on d.ProductID=e.ProductID join Person f on f.PersonID=a.PersonID join Catalog h on e.CatalogID=h.CatalogID where b.OrderStatesID='" +
                            (OrderStatesCombobox.SelectedIndex + 1) + "' and a.OrderTime between'" + dt1 + "' and '" +
                            dt2 + "'");
                }
            }
            else
            {
                if (OrderStatesCombobox.SelectedIndex == -1)
                {
                    ds =
                        Database.RunDataSet(
                            "select a.*,b.OrderStatesName,d.SellPrice,e.ProductName,e.ImageSource,f.PersonName,f.PersonPN,h.CatalogName from Orders a join OrderStates b on a.OrderStatesID=b.OrderStatesID join ItemUp d on a.ItemUpID=d.ItemUpID join Product e on d.ProductID=e.ProductID join Person f on f.PersonID=a.PersonID join Catalog h on e.CatalogID=h.CatalogID where h.CatalogID='" +
                            (CataLogcbbOrders.SelectedIndex + 1) + "' and a.OrderTime between'" + dt1 + "' and '" + dt2 +
                            "'");
                }
                else
                {
                    ds =
                        Database.RunDataSet(
                            "select a.*,b.OrderStatesName,d.SellPrice,e.ProductName,e.ImageSource,f.PersonName,f.PersonPN,h.CatalogName from Orders a join OrderStates b on a.OrderStatesID=b.OrderStatesID join ItemUp d on a.ItemUpID=d.ItemUpID join Product e on d.ProductID=e.ProductID join Person f on f.PersonID=a.PersonID join Catalog h on e.CatalogID=h.CatalogID where h.CatalogID='" +
                            (CataLogcbbOrders.SelectedIndex + 1) + "' and b.OrderStatesID='" +
                            (OrderStatesCombobox.SelectedIndex + 1) + "' and a.OrderTime between'" + dt1 + "' and '" +
                            dt2 + "'");
                }
            }
            Historypanels.Controls.Clear();
            GetConctors(ds);
        }

        private void FirstHomeLB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UserCenterLink_LinkClicked(null, null);
        }


        

       
    }
}
