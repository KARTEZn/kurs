using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKUD
{
    public partial class AddEmp : Form
    {
        public MainForm _MainForm;
        public string id;

        public AddEmp()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Добавить")
            { 
                try
                {
                    SQLite A = new SQLite();
                    if (A.WQuery("INSERT INTO [employees](numb, name, family, patr, dolj, otdel, oklad) values('нд', '"+name.Text+"', '"+family.Text+"', '"+patr.Text+"', '"+dolj.Text+"', '"+otdel.Text+"', '"+oklad.Text+"')"))
                    {
                        A.WQuery("INSERT INTO [graf](numb, `in`, out) values((select _id from [employees] where name='"+name.Text+"' and family='"+family.Text+"' and patr='"+patr.Text+"'), '"+intime.Text+"', '"+outtime.Text+"')");
                        A.RQuery("SELECT _id as 'номер', name as 'имя', family as 'фамилия', patr as 'отчество', dolj as 'должность', otdel as 'отдел' FROM [employees]");
                        _MainForm.Grid.DataSource = A.t;
                        _MainForm.Grid.ReadOnly = true;
                        //таблица график
                        A.RQuery("SELECT b.family as 'фамилия', a.`in` as 'с', a.out as 'по' FROM [graf] a, [employees] b where a.numb=b._id");
                        _MainForm.Grid2.DataSource = A.t;
                        _MainForm.Grid2.ReadOnly = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка: Добавление пользователя не было выполнено!");
                    }
                
                }
                catch(Exception exp)
                {
                    MessageBox.Show("Ошибка: "+exp.Message+".");
                    Close();
                }
            }
            else
            {
                SQLite A = new SQLite();
                if (A.WQuery("UPDATE [employees] set numb = '"+id+"', name = '"+ name.Text + "', family = '"+family.Text+"', patr = '"+patr.Text+"', dolj = '"+dolj.Text+"', otdel = '"+otdel.Text+"' where _id = '"+id+"'"))
                {
                    A.WQuery("UPDATE [graf] set `in` = '"+intime.Text+"', out = '"+outtime.Text+"' where numb = '"+id+"'");
                    A.RQuery("SELECT _id as 'номер', name as 'имя', family as 'фамилия', patr as 'отчество', dolj as 'должность', otdel as 'отдел' FROM [employees]");
                    _MainForm.Grid.DataSource = A.t;
                    _MainForm.Grid.ReadOnly = true;
                    //таблица график
                    A.RQuery("SELECT b.family as 'фамилия', a.`in` as 'с', a.out as 'по' FROM [graf] a, [employees] b where a.numb=b._id");
                    _MainForm.Grid2.DataSource = A.t;
                    _MainForm.Grid2.ReadOnly = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка: Добавление пользователя не было выполнено!");
                }
            }
        }

        private void oklad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }
    }
}
