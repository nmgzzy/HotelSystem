using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace HotelSystem
{
    public partial class FormSystem : Form
    {
        private void AddRoom()
        {
            Log.print("RoomForm AddRoom");
            RoomSet.RoomInfo roomInfo;
            if (!int.TryParse(this.textBox1.Text, out roomInfo.RoomId))
            {
                MessageBox.Show("请输入数字房间号");
                return;
            }
            if (roomSet.IsIdExist(roomInfo.RoomId))
            {
                MessageBox.Show("房间号不能重复");
                return;
            }
            if (this.textBox2.Text != "")
            {
                roomInfo.RoomName = this.textBox2.Text;
            }
            else
            {
                MessageBox.Show("请输入房间名");
                return;
            }
            if (!int.TryParse(this.textBox3.Text, out roomInfo.BedNum))
            {
                MessageBox.Show("请输入数字房间人数");
                return;
            }
            if (roomInfo.BedNum > 10)
            {
                MessageBox.Show("房间人数不能大于10");
                return;
            }
            if (this.comboBox3.SelectedIndex > -1)
            {
                roomInfo.type = this.comboBox3.SelectedIndex;//0b1g2g
            }
            else
            {
                MessageBox.Show("请输入房间类型");
                return;
            }
            roomSet.AddRoom(roomInfo);
            UpdateRoomSet();
            showStatus();
            MessageBox.Show("添加成功");
        }
        private void DelRoom()
        {
            Log.print("RoomForm DelRoom");
            RoomSet.RoomInfo roomInfo;
            if (!int.TryParse(this.textBox1.Text, out roomInfo.RoomId))
            {
                MessageBox.Show("请输入数字房间号");
                return;
            }
            if (!roomSet.IsIdExist(roomInfo.RoomId))
            {
                MessageBox.Show("没有该房间号");
                return;
            }
            roomInfo = roomSet.QueryRoom(roomInfo.RoomId);
            DialogResult result = MessageBox.Show("确认删除 " 
                + roomInfo.RoomId +" "+ roomInfo.RoomName + " 房间？", 
                "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.textBox1.Text = "";
                if (roomSet.DelRoom(roomInfo))
                {
                    MessageBox.Show("删除成功");
                }
                else
                {
                    MessageBox.Show("删除失败");
                }
            }
            else
            {
                this.textBox1.Text = "";
            }
            showStatus();
            UpdateRoomSet();
        }
        void QueryRoomSet()
        {
            Log.print("RoomForm QueryRoomSet");
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            column = new DataColumn();
            column.DataType = typeof(int);
            column.ColumnName = "房间号";
            column.ReadOnly = true;
            column.Unique = true;
            table.Columns.Add(column);
            column = new DataColumn();
            column.DataType = typeof(String);
            column.ColumnName = "房间名";
            column.ReadOnly = true;
            table.Columns.Add(column);
            column = new DataColumn();
            column.DataType = typeof(String);
            column.ColumnName = "房间类型";
            column.ReadOnly = true;
            table.Columns.Add(column);
            column = new DataColumn();
            column.DataType = typeof(int);
            column.ColumnName = "床位数";
            column.ReadOnly = true;
            table.Columns.Add(column);
            for (int i = 0; i < roomSet.Room.Count; i++)
            {
                row = table.NewRow();
                row["房间号"] = roomSet[i].RoomId;
                row["房间名"] = roomSet[i].RoomName;
                row["房间类型"] = RoomSet.TypeName[roomSet[i].type];
                row["床位数"] = roomSet[i].BedNum;
                table.Rows.Add(row);
            }
            this.dataGridView1.DataSource = table;
            showStatus();
            UpdateRoomSet();
        }
        void UpdateRoomSet()
        {
            Log.print("RoomForm UpdateRoomSet");
            this.comboBox1.Items.Clear();
            for (int i = 0; i < roomSet.RoomNum; i++)
            {
                this.comboBox1.Items.Add(roomSet[i].RoomId);
            }
        }
        void UpdateBedSet()
        {
            Log.print("RoomForm UpdateBedSet");
            int roomid = int.Parse(this.comboBox1.SelectedItem.ToString());
            this.comboBox2.Items.Clear();
            for (int i = 0, j = 1; i < roomSet.QueryRoom(roomid).BedNum; i++, j++)
            {
                this.comboBox2.Items.Add(j);
            }
        }
        void QueryRoom()
        {
            Log.print("RoomForm QueryRoom");
            PeopleManagement person = new PeopleManagement();
            DateTime start = dateTimePicker1.Value;
            DateTime end = dateTimePicker2.Value;
            dbTable = new DataTable();
            DataColumn column;
            DataRow row;
            string[] Cname = { "日期", "房间号", "房间名", "房间类型", "床位数", "未选床位人数", "旅客1", "旅客2", 
                "旅客3", "旅客4", "旅客5", "旅客6", "旅客7", "旅客8", "旅客9", "旅客10" };
            Type[] types = { typeof(string), typeof(int), typeof(string), typeof(string), typeof(int), typeof(int),
                typeof(string), typeof(string),typeof(string), typeof(string), typeof(string),
                typeof(string), typeof(string),typeof(string), typeof(string), typeof(string)};
            for (int i = 0; i < Cname.Length; i++)
            {
                column = new DataColumn();
                column.DataType = types[i];
                column.ColumnName = Cname[i];
                column.ReadOnly = true;
                dbTable.Columns.Add(column);
            }
            for (DateTime idate = start; idate < end; idate = idate.AddDays(1))
            {
                person.checkin = idate;
                DataTable ptable = person.QueryReserve2(SysDb);
                for (int iroom = 0; iroom < roomSet.RoomNum; iroom++)
                {
                    DataRow[] pr = ptable.Select("roomnum = " + roomSet[iroom].RoomId, "bednum ASC");
                    row = dbTable.NewRow();
                    row["日期"] = idate.ToString("yyyy-MM-dd");
                    row["房间号"] = roomSet[iroom].RoomId;
                    int rn = roomSet[iroom].RoomId;
                    RoomSet.RoomInfo room = roomSet.QueryRoom(rn);
                    row["房间名"] = roomSet[iroom].RoomName;
                    row["房间类型"] = RoomSet.TypeName[room.type];
                    row["床位数"] = room.BedNum;
                    int count = 0, countUnChoose = 0;
                    for (int ibed = 0; ibed < pr.Length; ibed++)
                    {
                        if ((int)pr[ibed]["bednum"] > 0)
                        {
                            row["旅客" + (ibed + 1)] = pr[ibed]["name"];
                        }
                        else
                        {
                            countUnChoose++;
                        }
                        count++;
                    }
                    row["未选床位人数"] = countUnChoose;
                    if (count > 0)
                    {
                        dbTable.Rows.Add(row);
                    }
                }
            }
            this.dataGridView1.DataSource = this.dbTable;
        }
        void QueryRoomToday(out int boy, out int girl, out int total)
        {
            Log.print("RoomForm QueryRoomToday");
            PeopleManagement person = new PeopleManagement();
            person.checkin = DateTime.Now;
            DataTable ptable = person.QueryReserve2(SysDb);
            boy = 0;
            girl = 0;
            total = 0;
            for (int i = 0; i < ptable.Rows.Count; i++)
            {
                DataRow row = ptable.Rows[i];
                RoomSet.RoomInfo room = roomSet.QueryRoom((int)row["roomnum"]);
                if (room.type == 0)
                {
                    boy++;
                }
                else if(room.type == 1)
                {
                    girl++;
                }
                total++;
            }
        }
    }
    [Serializable]
    public class RoomSet
    {
        [Serializable]
        public struct RoomInfo
        {
            public int RoomId;
            public string RoomName;
            public int BedNum;
            public int type;
            public static bool operator ==(RoomInfo b, RoomInfo c)
            {
                return b.RoomId == c.RoomId;
            }
            public static bool operator !=(RoomInfo b, RoomInfo c)
            {
                return b.RoomId != c.RoomId;
            }
        }
        public static readonly string[] TypeName = new string[] { "男生房间", "女生房间", "通用房间" };
        public List<RoomInfo> Room = new List<RoomInfo>();
        public RoomInfo this[int index]
        {
            get
            {
                return Room[index];
            }
        }
        public int RoomNum
        {
            get
            {
                return Room.Count;
            }
        }
        public int BedNumTotal
        {
            get
            {
                return countBed(-1);
            }
        }
        public int BedNumBoy
        {
            get
            {
                return countBed(0);
            }
        }
        public int BedNumGirl
        {
            get
            {
                return countBed(1);
            }
        }
        public int BedNumGeneral
        {
            get
            {
                return countBed(2);
            }
        }
        private int countBed(int type)
        {
            int count = 0;
            foreach (var i in Room)
            {
                if (i.type == type || type == -1)
                {
                    count += i.BedNum;
                }
            }
            return count;
        }
        public void AddRoom(RoomInfo ri)
        {
            Log.print("Room AddRoom");
            Room.Add(ri);
        }
        public bool DelRoom(RoomInfo ri)
        {
            Log.print("Room DelRoom");
            foreach (RoomInfo r in Room)
            {
                if (r == ri)
                {
                    Room.Remove(r);
                    return true;
                }
            }
            return false;
        }
        public bool IsIdExist(int id)
        {
            foreach (RoomInfo r in Room)
            {
                if (r.RoomId == id)
                {
                    return true;
                }
            }
            return false;
        }
        public static void Save(RoomSet rs)
        {
            Log.print("Room Save");
            FileStream fs = new FileStream("roomset.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, rs);
            fs.Close();

        }
        public static RoomSet Load()
        {
            Log.print("Room Load");
            RoomSet rs;
            try
            {
                FileStream fs = new FileStream("roomset.dat", FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();
                rs = bf.Deserialize(fs) as RoomSet;
                fs.Close();
            }
            catch (Exception)
            {
                rs = new RoomSet();
            }
            return rs;
        }
        public RoomInfo QueryRoom(int id)
        {
            Log.print("Room QueryRoom");
            for (int i = 0; i < Room.Count; i++)
            {
                if (Room[i].RoomId == id)
                {
                    return Room[i];
                }
            }
            RoomInfo roomInfo;
            roomInfo.RoomId = -1;
            roomInfo.RoomName = "";
            roomInfo.type = -1;
            roomInfo.BedNum = 0;
            return roomInfo;
        }
    }
}
