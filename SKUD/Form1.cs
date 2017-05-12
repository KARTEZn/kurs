using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System ;

namespace SKUD
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                SQLite A = new SQLite();
                List<List<string>> Query = A.RQuery("SELECT password FROM [users] WHERE username = '"+textBox1.Text+"'");
                foreach (List<string> temp in Query)
                {
                    if (temp[0] == textBox2.Text)
                    {
                        MainForm _MainForm = new MainForm();

                        //таблица сотрудники
                        

                        A.RQuery("SELECT _id as 'номер', name as 'имя', family as 'фамилия', patr as 'отчество', dolj as 'должность', otdel as 'отдел' FROM [employees]");
                        _MainForm.Grid.DataSource = A.t;
                        _MainForm.Grid.ReadOnly = true;


                        //таблица график
                        A.RQuery("SELECT b.family as 'фамилия', a.`in` as 'с', a.out as 'по' FROM [graf] a, [employees] b where a.numb=b._id");
                        _MainForm.Grid2.DataSource = A.t;
                        _MainForm.Grid2.ReadOnly = true;

                        //таблица журнал
                        A.RQuery("SELECT b.family as 'фамилия', a.`in` as 'приход', a.out as 'уход', a.dot as 'дата' FROM [journal] a, [employees] b where a.numb = b._id order by a.dot desc, a.out desc");
                        
                        _MainForm.dataGridView1.DataSource = A.t;
                        _MainForm.dataGridView1.ReadOnly = true;
                        try
                        {
                            _MainForm.dataGridView1.Columns[1].DefaultCellStyle.Format = "HH:mm";
                            _MainForm.dataGridView1.Columns[2].DefaultCellStyle.Format = "HH:mm";
                        }
                        catch { }
                        //_MainForm.dataGridView1.Columns[2].ValueType = typeof(DateTime);
                        //

                        //
                        //
                        //



                        _MainForm.Show();
                        this.Hide();
                    }
                    else MessageBox.Show("Неверно указаны учетные данные!");
                }
            }
            else MessageBox.Show("Не введены учетные данные!");
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
