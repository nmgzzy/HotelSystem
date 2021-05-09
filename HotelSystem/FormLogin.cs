using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HotelSystem
{
    public partial class FormLogin : Form
    {
        private bool signFlag = false;
        private SqlManage loginDb = new SqlManage("manager.db");
        Manager manager = new Manager("000", "000000");
        private FileStream file;
        private StreamReader sReader;
        private StreamWriter sWriter;
        public FormLogin()
        {
            InitializeComponent();
            loginDb.ConnectToDatabase();
            loginDb.Command("CREATE TABLE IF NOT EXISTS " +
                "manager(username TEXT NOT NULL," +
                "password TEXT NOT NULL)");
            Log.print("Connect To login Database");
            try
            {
                file = new FileStream("licence", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                sReader = new StreamReader(file, System.Text.Encoding.UTF8);
                this.textBox4.Text = sReader.ReadLine();
                sReader.Close();
                file.Close();
            }
            catch (Exception)
            {
                Log.print("read licence fail");
            }
        }
        private void TryLogin()
        {
            string licence = this.textBox4.Text.Trim();
            int ts = Login.CheckAuth(licence);
            if (ts >= 0)
            {
                MessageBox.Show("登录成功，有效期还剩" + ts.ToString() + "天");
                Status.manager = manager.Username;
                file = new FileStream("licence", FileMode.Create, FileAccess.Write);
                sWriter = new StreamWriter(file, System.Text.Encoding.UTF8);
                sWriter.WriteLine(licence);
                sWriter.Close();
                file.Close();
                this.Close();
                Status.LoginSuccess = true;
                Log.print("LoginSuccess");
            }
            else
            {
                MessageBox.Show("授权失败，请购买授权码！");
                Log.print("Auth fail");
            }
        }
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            signFlag = false;
            this.label3.Visible = false;
            this.textBox3.Visible = false;
            manager.Username = textBox1.Text.Trim();
            manager.Password = textBox2.Text.Trim();
            if (manager.Username == "administrator" && manager.Password == "admin123456")
            {
                TryLogin();
                return;
            }
            manager.Encrypt();
            SQLiteDataReader reader = loginDb.Select("*", "manager", "where username = '" + manager.SUsername + "'");
            if (reader.Read())
            {
                if (manager.SPassword == (string)reader["password"])
                {
                    TryLogin();
                }
                else
                {
                    //登录失败
                    MessageBox.Show("密码错误");
                    Log.print("pswd error");
                }
            }
            else
            {
                //登录失败
                MessageBox.Show("没有该用户");
                Log.print("no user");
            }
        }

        private void buttonSign_Click(object sender, EventArgs e)
        {
            if (signFlag == false)
            {
                signFlag = true;
                this.label3.Visible = true;
                this.textBox3.Visible = true;
            }
            else
            {
                if (this.textBox1.Text.Trim().Length > 1
                    && this.textBox2.Text.Trim().Length > 3
                    && this.textBox2.Text.Trim() == this.textBox3.Text.Trim()
                    && this.textBox1.Text.Trim() != "administrator")
                {
                    manager.Username = textBox1.Text.Trim();
                    manager.Password = textBox2.Text.Trim();
                    manager.Encrypt();
                    SQLiteDataReader reader = loginDb.Select("*", "manager", "where username = '" + manager.SUsername + "'");
                    if (reader.Read())
                    {
                        MessageBox.Show("用户名已存在");
                        Log.print("user exist");
                    }
                    else
                    {
                        loginDb.Insert("manager", "username, password", "'" + manager.SUsername + "', '" + manager.SPassword + "'");
                        MessageBox.Show("注册成功！");
                        Log.print("sign success");
                        signFlag = false;
                        this.label3.Visible = false;
                        this.textBox3.Visible = false;
                        this.textBox3.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("填写不符合要求，注册失败！");
                    Log.print("sign fail");
                }
            }
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.print("FormLoginClosing");
            loginDb.DisonnectToDatabase();
        }
    }

}
