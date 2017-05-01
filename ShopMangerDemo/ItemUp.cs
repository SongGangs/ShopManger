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
    public partial class ItemUp : Form
    {

        private string usernames = "";
        private string proIDs = "";
        private int userid = 0;
        //public string username
        //{
        //    set { usernames = value; }
        //}

        public void SetValue(string imgsource,string productNm,string proInt, string CatalogNm,string proID,string username )
        {
            if (!string.IsNullOrEmpty(imgsource))
            {
                ImgLabel.Text = "";
                Image image = Image.FromFile(imgsource);
                Image reduceimg = image.GetThumbnailImage(ImgLabel.Width, ImgLabel.Height, null, IntPtr.Zero);
                ImgLabel.BackgroundImage = reduceimg;  
            }
            else
            {
                ImgLabel.Text = "暂无图片";
            }
            this.ProductNameTXt.Text = productNm;
            this.CatalogTXt.Text = CatalogNm;
            this.proIDs = proID;
            this.ProductIntTXt.Text = proInt;
            this.usernames = username;
        }
        public ItemUp()
        {
            InitializeComponent();
        }

        private void SaveBtu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ItemUpNumTXt.Text))
            {
                MessageBox.Show("上架数量不能为空！", "温馨提示");
                return;
            }
            if (string.IsNullOrEmpty(StockPriceTXt.Text))
            {
                MessageBox.Show("进货单价不能为空！", "温馨提示");
                return;
            }
            if (string.IsNullOrEmpty(SellPriceTXt.Text))
            {
                MessageBox.Show("出售单价不能为空！", "温馨提示");
                return;
            }
            if (MessageBox.Show("你确定要上架吗？","温馨提示",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
            {
                DataSet ds = Database.RunDataSet("select * from Product");
                DataSet ItemUpds = Database.RunDataSet("select * from  ItemUp where 0=1");
                DataTable ItemUpDT = ItemUpds.Tables[0];
                DataRow ItemUpRow = ItemUpDT.NewRow();

                ItemUpRow["ItemUpID"] = 0;
                ItemUpRow["ProductID"] = proIDs;
                ItemUpRow["SellerID"] = userid;
                ItemUpRow["ItemUpNum"] = ItemUpNumTXt.Text.Trim();
                ItemUpRow["StockPrice"] = StockPriceTXt.Text.Trim();
                ItemUpRow["SellPrice"] = SellPriceTXt.Text.Trim();
                ItemUpRow["RemainNum"] = ItemUpNumTXt.Text.Trim();
                ItemUpRow["ItemUpTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ItemUpRow["ItemUpState"] = 0;
                ItemUpDT.Rows.Add(ItemUpRow);
                try
                {
                    int j = Database.update("ItemUp", "ItemUpID", ItemUpDT);
                    MessageBox.Show("商品上架成功！", "温馨提示");
                    this.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("商品上架失败！", "温馨提示");

                } 
            }
        }

        private void ItemUp_Load(object sender, EventArgs e)
        {
            DataSet ds = Database.RunDataSet("select * from Seller where SellerName='" + usernames + "'");
            userid = int.Parse(ds.Tables[0].Rows[0]["SellerID"].ToString());

        }


        //只能输入小数与浮点数
        private void ItemUpNumTXt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 46 && !char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                errorProvider1.SetError(ItemUpNumTXt,"只能输入数字或浮点数");
            }
        }
    }
}
