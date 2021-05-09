using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace HotelSystem
{
    public partial class FormSystem : Form
    {
        private SqlManage SysDb = new SqlManage("info.db");
        private DataTable dbTable;
        private RoomSet roomSet = RoomSet.Load();
        public FormSystem()
        {
            InitializeComponent();
            if (Status.manager == "administrator")
            {
                this.menuStrip1.Items[2].Enabled = true;
                Log.print("login as admin");
            }
            else
            {
                this.menuStrip1.Items[2].Enabled = false;
                Log.print("login as user");
            }
        }

        private void FormSystem_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + Status.manager;
            SysDb.ConnectToDatabase();
            SysDb.Command("CREATE TABLE IF NOT EXISTS "+ Status.manager +
                "people(name TEXT NOT NULL," +
                "id TEXT NOT NULL," +
                "roomnum INT," +
                "bednum INT," +
                "ischecked TEXT," +
                "checkin TEXT," +
                "checkout TEXT," +
                "booktime TEXT," +
                "info TEXT )");
            Log.print("Connect To sys Database");
            ShowPage();
            UpdateRoomSet();
            showStatus();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Log.print("button1_Click at page" + Status.page);
            switch (Status.page)
            {
                case Status.Epage.people:
                    AddReserve();
                    break;
                case Status.Epage.room:
                    AddRoom();
                    RoomSet.Save(roomSet);
                    break;
                case Status.Epage.admin:

                    break;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Log.print("button2_Click at page" + Status.page);
            switch (Status.page)
            {
                case Status.Epage.people:
                    DelReserve();
                    break;
                case Status.Epage.room:
                    DelRoom();
                    RoomSet.Save(roomSet);
                    break;
                case Status.Epage.admin:

                    break;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Log.print("button3_Click at page" + Status.page);
            switch (Status.page)
            {
                case Status.Epage.people:
                    QueryTable();
                    break;
                case Status.Epage.room:
                    QueryRoomSet();
                    break;
                case Status.Epage.admin:

                    break;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Log.print("button4_Click at page" + Status.page);
            switch (Status.page)
            {
                case Status.Epage.people:
                    ExportForm(this.dbTable, "顾客管理");
                    break;
                case Status.Epage.room:

                    break;
                case Status.Epage.admin:

                    break;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Log.print("button5_Click at page" + Status.page);
            switch (Status.page)
            {
                case Status.Epage.people:
                    Checkin();
                    break;
                case Status.Epage.room:
                    QueryRoom();
                    break;
                case Status.Epage.admin:

                    break;
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            Log.print("button6_Click at page" + Status.page);
            switch (Status.page)
            {
                case Status.Epage.people:

                    break;
                case Status.Epage.room:
                    ExportForm(this.dbTable, "房间管理");
                    break;
                case Status.Epage.admin:

                    break;
            }
        }
        private void FormSystem_FormClosing(object sender, FormClosingEventArgs e)
        {
            SysDb.DisonnectToDatabase();
            Log.print("FormSystem_FormClosing");
        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Log.print("menuStrip1_ItemClicked "+ e.ClickedItem.Text);
            switch (e.ClickedItem.Text)
            {
                case "顾客管理":
                    Status.page = Status.Epage.people;
                    break;
                case "房间管理":
                    Status.page = Status.Epage.room;
                    break;
                case "管理员账户":
                    Status.page = Status.Epage.admin;
                    break;
                case "使用说明":
                    Status.page = Status.Epage.info;
                    break;
            }
            ShowPage();
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            if (Status.page == Status.Epage.people)
            {
                this.labelDate1.Text = "3开始日期";
                this.labelDate2.Text = "3结束日期";
            }
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            if (Status.page == Status.Epage.people)
            {
                this.labelDate1.Text = "13入住日期";
                this.labelDate2.Text = "13离店日期";
            }
        }
        private bool ExportForm(DataTable table, string name)
        {
            Log.print("ExportForm");
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择保存位置";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return false;
                }
                string path = dialog.SelectedPath + "\\" + name
                    + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                int rowNumber = table.Rows.Count;
                int columnNumber = table.Columns.Count;

                if (rowNumber == 0)
                {
                    MessageBox.Show("没有任何数据可以导入到Excel文件！");
                    return false;
                }
                ExportToCsv(table, path);
            }
            return true;
        }

        public static void ExportToCsv(DataTable dt, string path)
        {
            Log.print("ExportToCsv");
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                // 添加列名称  
                sw.Write(dt.Columns[k].ColumnName.ToString());
                if (k < dt.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(Environment.NewLine);
            // 添加行数据  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    // 根据列数追加行数据  
                    if (row[j].ToString().Length > 11)
                    {
                        sw.Write("'");
                    }
                    sw.Write(row[j].ToString());
                    if (j < dt.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(Environment.NewLine);
            }
            sw.Close();
            fs.Close();
            MessageBox.Show("保存成功");
        }
        private void ShowPage()
        {
            Log.print("ShowPage" + Status.page);
            switch (Status.page)
            {
                case Status.Epage.admin:
                    this.textBoxInfo.Visible = false;
                    this.dataGridView1.Visible = true;
                    break;
                case Status.Epage.people:
                    this.labelTxt1.Text = "1姓名";
                    this.labelTxt2.Text = "125身份证号";
                    this.labelTxt3.Text = "1备注";
                    this.labelDate1.Text = "13入住日期";
                    this.labelDate2.Text = "13离店日期";
                    this.labelDate3.Text = "1预定日期";
                    this.labelC1.Text = "15房间号";
                    this.labelC2.Text = "15床位号";
                    this.button1.Text = "1预定录入";
                    this.button2.Text = "2取消预定";
                    this.button3.Text = "3查询数据";
                    this.button4.Text = "4导出数据";
                    this.button5.Text = "5确认入住";

                    this.dateTimePicker1.Value = DateTime.Now.AddDays(-10);
                    this.dateTimePicker2.Value = DateTime.Now.AddDays(-10);
                    this.dateTimePicker3.Value = DateTime.Now;

                    this.textBoxInfo.Visible = false;
                    this.dataGridView1.Visible = true;
                    this.labelTxt1.Visible = true;
                    this.labelTxt2.Visible = true;
                    this.labelTxt3.Visible = true;
                    this.labelDate1.Visible = true;
                    this.labelDate2.Visible = true;
                    this.labelDate3.Visible = true;
                    this.labelC1.Visible = true;
                    this.labelC2.Visible = true;
                    this.labelC3.Visible = false;
                    this.button1.Visible = true;
                    this.button2.Visible = true;
                    this.button3.Visible = true;
                    this.button4.Visible = true;
                    this.button5.Visible = true;
                    this.button6.Visible = false;

                    this.textBox1.Visible = true;
                    this.textBox2.Visible = true;
                    this.textBox3.Visible = true;
                    this.comboBox1.Visible = true;
                    this.comboBox2.Visible = true;
                    this.comboBox3.Visible = false;
                    this.dateTimePicker1.Visible = true;
                    this.dateTimePicker2.Visible = true;
                    this.dateTimePicker3.Visible = true;
                    break;
                case Status.Epage.room:
                    this.labelTxt1.Text = "12设置房间号";
                    this.labelTxt2.Text = "1设置房间名";
                    this.labelTxt3.Text = "1床位数";
                    this.labelDate1.Text = "3开始日期";
                    this.labelDate2.Text = "3结束日期";
                    this.labelC3.Text = "1设置房间类型";
                    this.button1.Text = "1增加房间";
                    this.button2.Text = "2删除房间";
                    this.button3.Text = "查询剩余房间";
                    this.button5.Text = "3查询入住信息";
                    this.button6.Text = "导出数据";

                    this.textBoxInfo.Visible = false;
                    this.dataGridView1.Visible = true;
                    this.labelTxt1.Visible = true;
                    this.labelTxt2.Visible = true;
                    this.labelTxt3.Visible = true;
                    this.labelDate1.Visible = true;
                    this.labelDate2.Visible = true;
                    this.labelDate3.Visible = false;//
                    this.labelC1.Visible = false;
                    this.labelC2.Visible = false;//
                    this.labelC3.Visible = true;//
                    this.button1.Visible = true;
                    this.button2.Visible = true;
                    this.button3.Visible = true;
                    this.button4.Visible = false;
                    this.button5.Visible = true;
                    this.button6.Visible = true;

                    this.textBox1.Visible = true;
                    this.textBox2.Visible = true;
                    this.textBox3.Visible = true;
                    this.comboBox1.Visible = false;
                    this.comboBox2.Visible = false;
                    this.comboBox3.Visible = true;
                    this.dateTimePicker1.Visible = true;
                    this.dateTimePicker2.Visible = true;
                    this.dateTimePicker3.Visible = false;
                    break;
                case Status.Epage.info:
                    this.textBoxInfo.Visible = true;
                    this.dataGridView1.Visible = false;
                    this.labelTxt1.Visible = false;
                    this.labelTxt2.Visible = false;
                    this.labelTxt3.Visible = false;
                    this.labelDate1.Visible = false;
                    this.labelDate2.Visible = false;
                    this.labelDate3.Visible = false;
                    this.labelC1.Visible = false;
                    this.labelC2.Visible = false;
                    this.labelC3.Visible = false;
                    this.button1.Visible = false;
                    this.button2.Visible = false;
                    this.button3.Visible = false;
                    this.button4.Visible = false;
                    this.button5.Visible = false;
                    this.button6.Visible = false;

                    this.textBox1.Visible = false;
                    this.textBox2.Visible = false;
                    this.textBox3.Visible = false;
                    this.comboBox1.Visible = false;
                    this.comboBox2.Visible = false;
                    this.comboBox3.Visible = false;
                    this.dateTimePicker1.Visible = false;
                    this.dateTimePicker2.Visible = false;
                    this.dateTimePicker3.Visible = false;
                    FileStream file = new FileStream("info.txt", FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(file, System.Text.Encoding.UTF8);
                    this.textBoxInfo.Text = reader.ReadToEnd();
                    reader.Close();
                    file.Close();
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex >= 0)
            {
                UpdateBedSet();
            }
        }

        private void showStatus()
        {
            int boy, girl, total;
            QueryRoomToday(out boy, out girl, out total);
            this.statusStrip1.Items[0].Text = "房间总数：" + roomSet.RoomNum;
            this.statusStrip1.Items[1].Text = "今日剩余床位：" + (roomSet.BedNumTotal - total) + "/" + roomSet.BedNumTotal;
            this.statusStrip1.Items[2].Text = "剩余男生床位：" + (roomSet.BedNumBoy - boy) + "/" + roomSet.BedNumBoy;
            this.statusStrip1.Items[3].Text = "剩余女生床位：" + (roomSet.BedNumGirl - girl) + "/" + roomSet.BedNumGirl;
        }
    }
}
