using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace HotelSystem
{
    public partial class FormSystem : Form
    {
        private bool AddReserve()
        {
            Log.print("PeopleForm AddReserve");
            PeopleManagement person = new PeopleManagement();
            if (this.textBox1.Text != "")
            {
                person.name = this.textBox1.Text;
            }
            else
            {
                MessageBox.Show("录入时姓名不能为空");
                return false;
            }
            if (this.textBox2.Text.Length == 18)
            {
                person.id = this.textBox2.Text;
            }
            else
            {
                MessageBox.Show("身份证号格式错误(18位)");
                return false;
            }
            person.info = this.textBox3.Text;
            DateTime dt = DateTime.Now.AddDays(-10);
            if (this.dateTimePicker1.Value.CompareTo(dt) > 0)
            {
                person.checkin = this.dateTimePicker1.Value;
            }
            else
            {
                MessageBox.Show("请选择入住日期");
                return false;
            }
            if (this.dateTimePicker2.Value.CompareTo(dt) > 0)
            {
                person.checkout = this.dateTimePicker2.Value;
            }
            else
            {
                MessageBox.Show("请选择离店日期");
                return false;
            }
            if (this.dateTimePicker2.Value.CompareTo(this.dateTimePicker1.Value) <= 0)
            {
                MessageBox.Show("入住日期必须小于离店日期");
                return false;
            }
            person.bookTime = this.dateTimePicker3.Value;

            if (this.comboBox1.SelectedIndex >= 0)
            {
                person.roomNum = this.comboBox1.SelectedIndex + 1;////////////
            }
            else
            {
                MessageBox.Show("请选择房间号");
                return false;
            }
            if (this.comboBox2.SelectedIndex >= 0)
            {
                person.bedNum = this.comboBox2.SelectedIndex + 1;////////////
            }
            person.AddReserve(SysDb);////////////
            MessageBox.Show("添加成功\r\n" + person.ToString());
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.dateTimePicker1.Value = dt;
            this.dateTimePicker2.Value = dt;
            this.dateTimePicker3.Value = DateTime.Now;
            this.comboBox1.SelectedIndex = -1;
            this.comboBox2.SelectedIndex = -1;
            showStatus();
            return true;
        }

        private bool DelReserve()
        {
            Log.print("PeopleForm DelReserve");
            PeopleManagement person = new PeopleManagement();
            if (this.textBox2.Text.Length == 18)
            {
                person.id = this.textBox2.Text;
            }
            else
            {
                MessageBox.Show("身份证号格式错误(18位)");
                return false;
            }
            SQLiteDataReader r = SysDb.Query("SELECT name from " + Status.manager + "people WHERE id='" + person.id + "'");
            if (r.Read())
            {
                DialogResult result = MessageBox.Show("确认删除 " + r["name"] + " 的预定？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                r.Close();
                if (result == DialogResult.Yes)
                {
                    this.textBox2.Text = "";
                    person.DelReserve(SysDb);
                }
                else
                {
                    this.textBox2.Text = "";
                    return false;
                }
            }
            else
            {
                MessageBox.Show("没有查到此预定");
                this.textBox2.Text = "";
                r.Close();
                return false;
            }
            showStatus();
            return true;
        }
        private void QueryTable()
        {
            Log.print("PeopleForm QueryTable");
            PeopleManagement person = new PeopleManagement();
            DateTime dt = new DateTime(2021, 1, 1);
            if (this.dateTimePicker1.Value.CompareTo(dt) > 0)
            {
                person.checkin = this.dateTimePicker1.Value;
            }
            else
            {
                person.checkin = DateTime.Now.AddDays(-30);
            }
            if (this.dateTimePicker2.Value.CompareTo(dt) > 0)
            {
                person.checkout = this.dateTimePicker2.Value;
            }
            else
            {
                person.checkout = DateTime.Now.AddDays(30);
            }
            if (this.dateTimePicker2.Value.CompareTo(this.dateTimePicker1.Value) <= 0)
            {
                person.checkin = DateTime.Now.AddDays(-30);
                person.checkout = DateTime.Now.AddDays(30);
            }
            this.dbTable = person.QueryReserve(SysDb);
            dbTable.Columns[0].ColumnName = "姓名";
            dbTable.Columns[1].ColumnName = "身份证号";
            dbTable.Columns[2].ColumnName = "房间号";
            dbTable.Columns[3].ColumnName = "床位号";
            dbTable.Columns[4].ColumnName = "是否入住";
            dbTable.Columns[5].ColumnName = "入住时间";
            dbTable.Columns[6].ColumnName = "离店时间";
            dbTable.Columns[7].ColumnName = "预定时间";
            dbTable.Columns[8].ColumnName = "备注";
            this.dataGridView1.DataSource = dbTable;
            showStatus();
        }
        private bool Checkin()
        {
            Log.print("PeopleForm Checkin");
            PeopleManagement person = new PeopleManagement();
            void clear()
            {
                this.textBox2.Text = "";
                this.comboBox1.SelectedIndex = -1;
                this.comboBox2.SelectedIndex = -1;
            }
            //入住
            if (this.textBox2.Text.Length == 18)
            {
                person.id = this.textBox2.Text;
            }
            else
            {
                MessageBox.Show("身份证号格式错误(18位)");
                return false;
            }
            SQLiteDataReader r = SysDb.Query("SELECT * from " + Status.manager + "people WHERE id='" + person.id + "'");
            if (r.Read())
            {
                person.name = (string)r["name"];
                person.info = (string)r["info"];
                person.checkin = DateTime.Parse((string)r["checkin"]);
                person.checkout = DateTime.Parse((string)r["checkout"]);
                person.bookTime = DateTime.Parse((string)r["booktime"]);
                person.roomNum = (int)r["roomnum"];
                person.bedNum = (int)r["bednum"];
                if (this.comboBox1.SelectedIndex >= 0 && this.comboBox2.SelectedIndex >= 0)
                {
                    person.roomNum = this.comboBox1.SelectedIndex + 1;////////////
                    person.bedNum = this.comboBox2.SelectedIndex + 1;////////////
                }
                r.Close();
                DialogResult result = MessageBox.Show("确认入住信息：\r\n" + person.ToString(), "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (this.comboBox1.SelectedIndex < 0 || this.comboBox2.SelectedIndex < 0)
                    {
                        MessageBox.Show("请选择房间号和床位号");
                        return false;
                    }
                    clear();
                    person.isChecked = "是";
                    person.UpdateReserve(SysDb);
                }
                else
                {
                    clear();
                    return false;
                }
            }
            else
            {
                MessageBox.Show("没有查到此预定");
                clear();
                r.Close();
                return false;
            }
            showStatus();
            return true;
        }
    }
    public class PeopleManagement
    {
        public string id;
        public string name;
        public int roomNum;
        public int bedNum = 0;
        public string isChecked = "否";
        public string info;
        public DateTime checkin;
        public DateTime checkout;
        public DateTime bookTime = DateTime.Now;
        private string tableName = Status.manager + "people";



        public void AddReserve(SqlManage db)
        {
            Log.print("PeopleManagement AddReserve");
            db.Insert(tableName,
                "name, id, roomnum, bednum, ischecked, checkin, checkout, booktime, info",
                "'" + name + "','" + id + "'," + roomNum + "," + bedNum
                + ",'" + isChecked + "','" + checkin.ToString("yyyy-MM-dd")
                + "','" + checkout.ToString("yyyy-MM-dd") + "','" + bookTime.ToString("yyyy-MM-dd")
                + "','" + info + "'");
        }
        public void DelReserve(SqlManage Db)
        {
            Log.print("PeopleManagement DelReserve");
            Db.Delete(tableName, "id=" + this.id);
        }
        public void UpdateReserve(SqlManage Db)
        {
            Log.print("PeopleManagement UpdateReserve");
            Db.Update(tableName, "ischecked = '是', roomnum = " + roomNum + ", bednum = " + bedNum,
                "id = " + id + " and checkin = '" + checkin.ToString("yyyy-MM-dd") + "'");
        }
        public DataTable QueryReserve(SqlManage Db)
        {
            Log.print("PeopleManagement QueryReserve");
            string s1 = checkin.ToString("yyyy-MM-dd");
            string s2 = checkout.ToString("yyyy-MM-dd");
            string sql = "SELECT * FROM " + tableName
            + " WHERE checkin BETWEEN date('" +
            s1 + "') AND date('" +
            s2 + "') OR checkout BETWEEN date('" +
            s1 + "') AND date('" +
            s2 + "') OR (checkin <= date('" +
            s1 + "') AND checkout >= date('" +
            s2 + "'))";
            return Db.QueryTable(sql);
        }
        public DataTable QueryReserve2(SqlManage Db)
        {
            Log.print("PeopleManagement QueryReserve2");
            string s1 = checkin.ToString("yyyy-MM-dd");
            string sql = "SELECT * FROM " + tableName
            + " WHERE checkin <= date('" +
            s1 + "') AND checkout > date('" +
            s1 + "') ORDER BY roomnum,bednum ASC";
            return Db.QueryTable(sql);
        }
        public override string ToString()
        {
            string s = "";
            s = s + "姓名：" + name + "\r\n";
            s = s + "身份证：" + id + "\r\n";
            s = s + "房间号：" + roomNum + "\r\n";
            s = s + "床位号：" + bedNum + "\r\n";
            s = s + "入住日期：" + checkin.ToLongDateString() + "\r\n";
            s = s + "离店日期：" + checkout.ToLongDateString();
            return s;
        }
    }
}
