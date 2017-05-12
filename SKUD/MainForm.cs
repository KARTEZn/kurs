using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SKUD
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddEmp _AddEmp = new AddEmp();
            _AddEmp._MainForm = this;
            _AddEmp.Text = "Добавлние сотрудника";
            _AddEmp.button1.Text = "Добавить";
            _AddEmp.name.Text = "";
            _AddEmp.family.Text = "";
            _AddEmp.patr.Text = "";
            _AddEmp.dolj.Text = "";
            _AddEmp.oklad.Text = "";
            _AddEmp.otdel.Text = "";
            _AddEmp.intime.Text = "8:00:00";
            _AddEmp.outtime.Text = "17:00:00";
            _AddEmp.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Grid.CurrentRow.Cells[0].Value.ToString() != "")
            {
                SQLite A = new SQLite();
                List<List<string>> reader = A.RQuery("select * from [employees] where _id = '"+ Grid.CurrentRow.Cells[0].Value.ToString() + "'");
                AddEmp _AddEmp = new AddEmp();
                _AddEmp._MainForm = this;
                _AddEmp.Text = "Изменение данных сотрудника";
                _AddEmp.button1.Text = "Изменить";
                foreach (List<string> temp in reader)
                {
                    _AddEmp.name.Text = temp[2];
                    _AddEmp.family.Text = temp[3];
                    _AddEmp.patr.Text = temp[4];
                    _AddEmp.dolj.Text = temp[5];
                    _AddEmp.otdel.Text = temp[6];
                    _AddEmp.oklad.Text = temp[7];
                }
                reader.Clear();
                reader = A.RQuery("select * from [graf] where numb = '" + Grid.CurrentRow.Cells[0].Value.ToString() + "'");
                foreach (List<string> temp in reader)
                {
                    _AddEmp.intime.Text = temp[2];
                    _AddEmp.outtime.Text = temp[3];
                }
                _AddEmp.id = Grid.CurrentRow.Cells[0].Value.ToString();
                _AddEmp.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Grid.CurrentRow.Cells[0].Value.ToString() != "")
            {
                var result = MessageBox.Show("Удалить запись сотрудника [" + Grid.CurrentRow.Cells[2].Value.ToString() + "] ?", "Удаление записи", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (result.ToString() == "OK")
                {
                    SQLite A = new SQLite();
                    A.WQuery("DELETE FROM [employees] where _id='"+ Grid.CurrentRow.Cells[0].Value.ToString() + "'");
                    A.WQuery("DELETE FROM [graf] where numb='" + Grid.CurrentRow.Cells[0].Value.ToString() + "'");
                    A.RQuery("SELECT _id as 'номер', name as 'имя', family as 'фамилия', patr as 'отчество', dolj as 'должность', otdel as 'отдел' FROM [employees]");
                    this.Grid.DataSource = A.t;
                    this.Grid.ReadOnly = true;
                    //таблица график
                    A.RQuery("SELECT b.family as 'фамилия', a.`in` as 'с', a.out as 'по' FROM [graf] a, [employees] b where a.numb=b._id");
                    this.Grid2.DataSource = A.t;
                    this.Grid2.ReadOnly = true;
                }

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                //таблица график
                SQLite A = new SQLite();
                A.RQuery("SELECT b.family as 'фамилия', a.`in` as 'с', a.out as 'по' FROM [graf] a, [employees] b where b.family = '" + textBox1.Text + "' and a.numb=b._id");
                this.Grid2.DataSource = A.t;
                this.Grid2.ReadOnly = true;
            }
            else
            {
                SQLite A = new SQLite();
                A.RQuery("SELECT b.family as 'фамилия', a.`in` as 'с', a.out as 'по' FROM [graf] a, [employees] b where a.numb=b._id");
                this.Grid2.DataSource = A.t;
                this.Grid2.ReadOnly = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //таблица журнал
            if (textBox2.Text != "")
            {

                SQLite A = new SQLite();

                List<List<string>> reader = A.RQuery("SELECT b._id FROM [journal] a, [employees] b where b.family = '"+textBox2.Text+"' and a.numb = b._id and a.dot = date('now')");

                if (reader.Count > 0)
                {
                    try
                    {
                        string id = reader[0][0];
                        A.WQuery("UPDATE [journal] set out='" + DateTime.Now.ToShortTimeString() + "' where numb='" + id + "' and dot = date('now')");
                    }
                    catch { }
                }
                else
                {
                    reader.Clear();
                    reader = A.RQuery("SELECT _id FROM [employees] where family = '" + textBox2.Text + "'");
                    if (reader.Count > 0)
                    {
                        string id = reader[0][0];
                        try
                        {
                            A.WQuery("INSERT INTO [journal](numb, `in`, out, dot) values('" + id + "', '" + DateTime.Now.ToShortTimeString() + "', null, date('now'))");
                        }
                        catch { }
                    }
                    else
                    {
                        MessageBox.Show("Сотрудник с таким учетным номером не найден!");
                    }
                }
               
                //
                A.RQuery("SELECT b.family as 'фамилия', a.`in` as 'приход', a.out as 'уход', a.dot as 'дата' FROM [journal] a, [employees] b where a.numb = b._id order by a.dot desc, a.out desc");
                
                this.dataGridView1.DataSource = A.t;
                this.dataGridView1.ReadOnly = true;

                try
                {
                    this.dataGridView1.Columns[1].DefaultCellStyle.Format = "HH:mm";
                    this.dataGridView1.Columns[2].DefaultCellStyle.Format = "HH:mm";
                }
                catch { }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("На вашем компьютере отсуствует приложение Microsoft Office Excel!");
                return;
            }

            SQLite A = new SQLite();

            string[] date1 = dateTimePicker1.Value.Date.ToShortDateString().Split('.');
            string[] date2 = dateTimePicker2.Value.Date.ToShortDateString().Split('.');

            List<List<string>> reader = A.RQuery("select a._id as 'номер', a.name as 'имя', a.family as 'фамилия', a.patr as 'отчество', a.dolj as 'должность', a.otdel as 'отдел', b.`in` as 'график прихода', b.out as 'график ухода', c.`in` as 'приход', c.out as 'уход', c.dot as 'дата' from [employees] a, [graf] b, [journal] c where b.numb = a._id and c.numb = a._id and c.dot between date('"+date1[2]+"-"+date1[1]+"-"+date1[0]+"') and date('"+date2[2]+"-"+date2[1]+"-"+date2[0]+"') order by c.dot");
            this.Grid4.DataSource = A.t;
            this.Grid4.ReadOnly = true;

            try
            {
                this.Grid4.Columns[8].DefaultCellStyle.Format = "HH:mm";
                this.Grid4.Columns[9].DefaultCellStyle.Format = "HH:mm";
            }
            catch { }
            
            if (reader.Count > 0)
            {
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                xlWorkSheet.Cells[5, 1] = "Номер";
                xlWorkSheet.Cells[5, 2] = "Имя";
                xlWorkSheet.Cells[5, 3] = "Фамилия";
                xlWorkSheet.Cells[5, 4] = "Отчество";
                xlWorkSheet.Cells[5, 5] = "Должность";
                xlWorkSheet.Cells[5, 6] = "Отдел";
                xlWorkSheet.Cells[5, 7] = "График прихода";
                xlWorkSheet.Cells[5, 8] = "График ухода";
                xlWorkSheet.Cells[5, 9] = "Приход";
                xlWorkSheet.Cells[5, 10] = "Уход";
                xlWorkSheet.Cells[5, 11] = "Дата";

                xlWorkSheet.Cells[5, 1].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 2].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 3].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 4].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 5].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 6].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 7].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 8].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 9].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 10].HorizontalAlignment = Excel.Constants.xlCenter;
                xlWorkSheet.Cells[5, 11].HorizontalAlignment = Excel.Constants.xlCenter;

                xlWorkSheet.Cells[5, 1].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 2].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 3].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 4].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 5].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 6].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 7].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 8].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 9].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 10].EntireRow.Font.Bold = true;
                xlWorkSheet.Cells[5, 11].EntireRow.Font.Bold = true; 

                int i = 6;

                xlWorkSheet.Range[xlWorkSheet.Cells[5, 1], xlWorkSheet.Cells[i, 11]].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                xlWorkSheet.Range[xlWorkSheet.Cells[5, 1], xlWorkSheet.Cells[i, 11]].Borders.Weight = 2d;

                

                foreach (List<string> value in reader)
                {
                    xlWorkSheet.Cells[i, 1] = value[0];
                    xlWorkSheet.Cells[i, 1].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 2] = value[1];
                    xlWorkSheet.Cells[i, 2].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 3] = value[2];
                    xlWorkSheet.Cells[i, 3].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 4] = value[3];
                    xlWorkSheet.Cells[i, 4].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 5] = value[4];
                    xlWorkSheet.Cells[i, 5].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 6] = value[5];
                    xlWorkSheet.Cells[i, 6].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 7] = value[6];
                    xlWorkSheet.Cells[i, 7].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 8] = value[7];
                    xlWorkSheet.Cells[i, 8].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 9] = value[8];
                    xlWorkSheet.Cells[i, 9].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 10] = value[9];
                    xlWorkSheet.Cells[i, 10].HorizontalAlignment = Excel.Constants.xlCenter;
                    xlWorkSheet.Cells[i, 11] = value[10];
                    xlWorkSheet.Cells[i, 11].HorizontalAlignment = Excel.Constants.xlCenter;
                    i++;
                }

                xlWorkSheet.Range[xlWorkSheet.Cells[6, 1], xlWorkSheet.Cells[i-1, 11]].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                xlWorkSheet.Range[xlWorkSheet.Cells[6, 1], xlWorkSheet.Cells[i-1, 11]].Borders.Weight = 2d;

                xlWorkSheet.Columns.AutoFit();

                string directory = System.Environment.CurrentDirectory + "\\Отчет.xls";
                MessageBox.Show(@directory);
                xlWorkBook.SaveAs(@directory, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);
                MessageBox.Show("Файл отчета успешно сформирован!");
            }
            else
            {
                MessageBox.Show("Количество выбранных записей за данный период = 0. Отчет не будет создан.");
            }

            

        }

        private void button8_Click(object sender, EventArgs e)
        {
            //if (textBox1.Text != "")
            //{
                //таблица график
                if (textBox3.Text == "") return;
                SQLite A = new SQLite();
                List<List<string>> reader = A.RQuery("select a.family, a.name, a.patr, a.oklad, a.dolj, a.otdel, b.out, b.`in`, a._id from [employees] a, [graf] b where a._id = b.numb and family = '"+textBox3.Text+"'");
                fio.Text = reader[0][0].ToString() + ' ' + reader[0][1].ToString()[0] + '.' + reader[0][2].ToString()[0] + '.';
                count_days.Text = "за "+Convert.ToInt32(dateTimePicker4.Value.Date.Day - dateTimePicker3.Value.Date.Day)+" дней";
                oklads.Text = "оклад: " + reader[0][3].ToString() + " руб.";
                doljs.Text = "должность: "+reader[0][4].ToString();
                otdels.Text = "отдел: "+reader[0][5].ToString();
                int outs = 0;
                string[] timeoff = reader[0][6].ToString().Split(':');
                int ins = 0;
                string[] timeon = reader[0][7].ToString().Split(':');
                try
                {
                    outs = Convert.ToInt32(timeoff[0]);
                }
                catch
                {
                    outs = 0;
                }
                try
                {
                    ins = Convert.ToInt32(timeon[0]);
                }
                catch
                {
                    ins = 0;
                }
                int timed = (outs - ins)*22;
                string okl = reader[0][3].ToString();
                string _id = reader[0][8].ToString();

                string[] date1 = dateTimePicker3.Value.Date.ToShortDateString().Split('.');
                string[] date2 = dateTimePicker4.Value.Date.ToShortDateString().Split('.');

                reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('" + date1[2] + "-" + date1[1] + "-" + date1[0] + "') and date('" + date2[2] + "-" + date2[1] + "-" + date2[0] + "')");

                int alltime = 0;

                foreach (List<string> value in reader)
                {
                    ins = 0;
                    outs = 0;

                    string[] timoff = value[1].Split(' ');
                    string[] timon = value[0].Split(' ');
                    try
                    {
                        outs = Convert.ToInt32(timoff[1].Substring(0,2));
                    }
                    catch
                    {
                        outs = 0;
                    }

                    try
                    {
                        ins = Convert.ToInt32(timon[1].Substring(0, 2));
                    }
                    catch
                    {
                        ins = 0;
                    }

                    alltime += (outs - ins);
                }

                if (alltime < timed)
                {
                    double poddox = 0;

                    try
                    {
                        //MessageBox.Show(alltime.ToString());
                        //MessageBox.Show(Convert.ToDouble(okl).ToString());
                        //MessageBox.Show(timed.ToString());
                        poddox = Math.Round((((alltime * Convert.ToDouble(okl)) / timed)),2);
                    }
                    catch
                    {
                        poddox = 0;
                    }
                    //MessageBox.Show(poddox.ToString());
                    try
                    {
                        pookladu.Text = "по окладу: " + poddox.ToString() + " руб.";
                    }
                    catch
                    {
                        pookladu.Text = "по окладу: 0 руб.";
                    }
                    premia.Text = "премия: 0 руб.";
                    //MessageBox.Show(poddox.ToString());
                    try
                    {
                        podox.Text = "подоходный налог: " + Math.Round(poddox * 0.14, 2).ToString() + " руб.";
                    }
                    catch
                    {
                        podox.Text = "подоходный налог: 0 руб.";
                    }
                    // MessageBox.Show(poddox.ToString());
                    try
                    {
                        pens.Text = "пенсионные начисления: " + Math.Round(poddox * 0.01,2).ToString() + " руб.";
                    }
                    catch
                    {
                        pens.Text = "пенсионные начисления: 0 руб.";
                    }
                    //MessageBox.Show(poddox.ToString());
                    try
                    {
                        itog_u.Text = "Итого удержано: "+Math.Round((poddox * 0.14) + (poddox * 0.01),2).ToString()+" руб.";
                    }
                    catch
                    {
                        itog_u.Text = "Итого удержано: 0 руб.";
                    }

                    try
                    {
                        itog_n.Text = "Итого начислено: " + (poddox - Math.Round((poddox * 0.14) + (poddox * 0.01), 2)).ToString() + " руб.";
                    }
                    catch
                    {
                        itog_n.Text = "Итого начислено: 0 руб.";
                    }
                }
                else
                {
                    double poddox = 0;

                    try
                    {
                        //MessageBox.Show(alltime.ToString());
                        //MessageBox.Show(Convert.ToDouble(okl).ToString());
                        //MessageBox.Show(timed.ToString());
                        poddox = Math.Round((((alltime * Convert.ToDouble(okl)) / timed)), 2);
                    }
                    catch
                    {
                        poddox = 0;
                    }
                    //MessageBox.Show(poddox.ToString());
                    try
                    {
                        pookladu.Text = "по окладу: " + okl + " руб.";
                    }
                    catch
                    {
                        pookladu.Text = "по окладу: 0 руб.";
                    }
                    try
                    {
                        premia.Text = "премия: "+Math.Round((poddox - Convert.ToDouble(okl)),2).ToString()+" руб.";
                    }
                    catch
                    {
                        premia.Text = "премия: 0 руб.";
                    }
                    //MessageBox.Show(poddox.ToString());
                    try
                    {
                        podox.Text = "подоходный налог: " + Math.Round(poddox * 0.14, 2).ToString() + " руб.";
                    }
                    catch
                    {
                        podox.Text = "подоходный налог: 0 руб.";
                    }
                    // MessageBox.Show(poddox.ToString());
                    try
                    {
                        pens.Text = "пенсионные начисления: " + Math.Round(poddox * 0.01, 2).ToString() + " руб.";
                    }
                    catch
                    {
                        pens.Text = "пенсионные начисления: 0 руб.";
                    }
                    //MessageBox.Show(poddox.ToString());
                    try
                    {
                        itog_u.Text = "Итого удержано: " + Math.Round((poddox * 0.14) + (poddox * 0.01), 2).ToString() + " руб.";
                    }
                    catch
                    {
                        itog_u.Text = "Итого удержано: 0 руб.";
                    }

                    try
                    {
                        itog_n.Text = "Итого начислено: " + (poddox - Math.Round((poddox * 0.14) + (poddox * 0.01), 2)).ToString() + " руб.";
                    }
                    catch
                    {
                        itog_n.Text = "Итого начислено: 0 руб.";
                    }
                }

                

                ///pookladu.Text = "по окладу: " + timed.ToString();
                
                

                //this.SRas.DataSource = A.t;
                //this.SRas.ReadOnly = true;
            //}
        }

        private void label59_Click(object sender, EventArgs e)
        {

        }

        private void count_day_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "") return;
            SQLite A = new SQLite();
            List<List<string>> reader = A.RQuery("select a.family, a.name, a.patr, a.oklad, a.dolj, a.otdel, b.out, b.`in`, a._id from [employees] a, [graf] b where a._id = b.numb and family = '" + textBox4.Text + "'");
            label61.Text = reader[0][0].ToString() + ' ' + reader[0][1].ToString()[0] + '.' + reader[0][2].ToString()[0] + '.';
            label59.Text = "за " + comboBox1.Text + " месяц";
            label58.Text = "оклад: " + reader[0][3].ToString() + " руб.";
            label57.Text = "должность: " + reader[0][4].ToString();
            label56.Text = "отдел: " + reader[0][5].ToString();
            int outs = 0;
            string[] timeoff = reader[0][6].ToString().Split(':');
            int ins = 0;
            string[] timeon = reader[0][7].ToString().Split(':');
            try
            {
                outs = Convert.ToInt32(timeoff[0]);
            }
            catch
            {
                outs = 0;
            }
            try
            {
                ins = Convert.ToInt32(timeon[0]);
            }
            catch
            {
                ins = 0;
            }
            int timed = (outs - ins) * 22;
            string okl = reader[0][3].ToString();
            string _id = reader[0][8].ToString();


            switch (comboBox1.Text)
            {
                case "Январь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-01-01') and date('2017-01-31') ");
                    break;
                case "Февраль":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-02-01') and date('2017-02-28') ");
                    break;
                case "Март":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-03-01') and date('2017-03-31') ");
                    break;
                case "Апрель":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-04-01') and date('2017-04-30') ");
                    break;
                case "Май":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-05-01') and date('2017-05-31') ");
                    break;
                case "Июнь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-06-01') and date('2017-06-30') ");
                    break;
                case "Июль":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-07-01') and date('2017-07-31') ");
                    break;
                case "Август":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-08-01') and date('2017-08-31') ");
                    break;
                case "Сентябрь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-09-01') and date('2017-09-30') ");
                    break;
                case "Октябрь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-10-01') and date('2017-10-31') ");
                    break;
                case "Ноябрь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-11-01') and date('2017-11-30') ");
                    break;
                case "Декабрь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-12-01') and date('2017-12-31') ");
                    break;
                default:
                    break;
            }

            //MessageBox.Show(reader[0][2]);

            int alltime = 0;

            foreach (List<string> value in reader)
            {
                ins = 0;
                outs = 0;
                string[] timoff = value[1].Split(' ');
                string[] timon = value[0].Split(' ');
                try
                {
                    outs = Convert.ToInt32(timoff[1].Substring(0, 2));
                }
                catch
                {
                    outs = 0;
                }

                try
                {
                    ins = Convert.ToInt32(timon[1].Substring(0, 2));
                }
                catch
                {
                    ins = 0;
                }

                alltime += (outs - ins);
            }

            if (alltime < timed)
            {
                double poddox = 0;

                try
                {
                    //MessageBox.Show(alltime.ToString());
                    //MessageBox.Show(Convert.ToDouble(okl).ToString());
                    //MessageBox.Show(timed.ToString());
                    poddox = Math.Round((((alltime * Convert.ToDouble(okl)) / timed)), 2);
                }
                catch
                {
                    poddox = 0;
                }
                //MessageBox.Show(poddox.ToString());
                try
                {
                    label33.Text = "по окладу: " + poddox.ToString() + " руб.";
                }
                catch
                {
                    label33.Text = "по окладу: 0 руб.";
                }
                label32.Text = "премия: 0 руб.";
                //MessageBox.Show(poddox.ToString());
                try
                {
                    label31.Text = "подоходный налог: " + Math.Round(poddox * 0.14, 2).ToString() + " руб.";
                }
                catch
                {
                    label31.Text = "подоходный налог: 0 руб.";
                }
                // MessageBox.Show(poddox.ToString());
                try
                {
                    label30.Text = "пенсионные начисления: " + Math.Round(poddox * 0.01, 2).ToString() + " руб.";
                }
                catch
                {
                    label30.Text = "пенсионные начисления: 0 руб.";
                }
                //MessageBox.Show(poddox.ToString());
                try
                {
                    label28.Text = "Итого удержано: " + Math.Round((poddox * 0.14) + (poddox * 0.01), 2).ToString() + " руб.";
                }
                catch
                {
                    label28.Text = "Итого удержано: 0 руб.";
                }

                try
                {
                    label29.Text = "Итого начислено: " + (poddox - Math.Round((poddox * 0.14) + (poddox * 0.01), 2)).ToString() + " руб.";
                }
                catch
                {
                    label29.Text = "Итого начислено: 0 руб.";
                }
            }
            else
            {
                double poddox = 0;

                try
                {
                    //MessageBox.Show(alltime.ToString());
                    //MessageBox.Show(Convert.ToDouble(okl).ToString());
                    //MessageBox.Show(timed.ToString());
                    poddox = Math.Round((((alltime * Convert.ToDouble(okl)) / timed)), 2);
                }
                catch
                {
                    poddox = 0;
                }
                //MessageBox.Show(poddox.ToString());
                try
                {
                    label33.Text = "по окладу: " + okl + " руб.";
                }
                catch
                {
                    label33.Text = "по окладу: 0 руб.";
                }
                try
                {
                    label32.Text = "премия: " + Math.Round((poddox - Convert.ToDouble(okl)), 2).ToString() + " руб.";
                }
                catch
                {
                    label32.Text = "премия: 0 руб.";
                }
                //MessageBox.Show(poddox.ToString());
                try
                {
                    label31.Text = "подоходный налог: " + Math.Round(poddox * 0.14, 2).ToString() + " руб.";
                }
                catch
                {
                    label31.Text = "подоходный налог: 0 руб.";
                }
                // MessageBox.Show(poddox.ToString());
                try
                {
                    label30.Text = "пенсионные начисления: " + Math.Round(poddox * 0.01, 2).ToString() + " руб.";
                }
                catch
                {
                    label30.Text = "пенсионные начисления: 0 руб.";
                }
                //MessageBox.Show(poddox.ToString());
                try
                {
                    label28.Text = "Итого удержано: " + Math.Round((poddox * 0.14) + (poddox * 0.01), 2).ToString() + " руб.";
                }
                catch
                {
                    label28.Text = "Итого удержано: 0 руб.";
                }

                try
                {
                    label29.Text = "Итого начислено: " + (poddox - Math.Round((poddox * 0.14) + (poddox * 0.01), 2)).ToString() + " руб.";
                }
                catch
                {
                    label29.Text = "Итого начислено: 0 руб.";
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "") return;
            SQLite A = new SQLite();
            List<List<string>> reader = A.RQuery("select a._id from [employees] a where a.family = '" + textBox5.Text + "'");
            string _id = reader[0][0].ToString();

            switch (comboBox2.Text)
            {
                case "Январь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-01-01') and date('2017-01-31') ");
                    break;
                case "Февраль":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-02-01') and date('2017-02-28') ");
                    break;
                case "Март":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-03-01') and date('2017-03-31') ");
                    break;
                case "Апрель":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-04-01') and date('2017-04-30') ");
                    break;
                case "Май":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-05-01') and date('2017-05-31') ");
                    break;
                case "Июнь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-06-01') and date('2017-06-30') ");
                    break;
                case "Июль":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-07-01') and date('2017-07-31') ");
                    break;
                case "Август":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-08-01') and date('2017-08-31') ");
                    break;
                case "Сентябрь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-09-01') and date('2017-09-30') ");
                    break;
                case "Октябрь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-10-01') and date('2017-10-31') ");
                    break;
                case "Ноябрь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-11-01') and date('2017-11-30') ");
                    break;
                case "Декабрь":
                    reader = A.RQuery("select `in`, out, dot from [journal] where numb = '" + _id + "' and dot between date('2017-12-01') and date('2017-12-31') ");
                    break;
                default:
                    break;
            }

            if (reader.Count > 0)
            {
                int[] mas = new int[31];

                foreach (List<string> value in reader)
                {
                    int ins = 0;
                    int outs = 0;
                    try
                    {

                        string[] timoff = value[1].Split(' ');
                        string[] timon = value[0].Split(' ');

                        try
                        {
                            outs = Convert.ToInt32(timoff[1].Substring(0, 2));
                        }
                        catch
                        {
                            outs = 0;
                        }

                        try
                        {
                            ins = Convert.ToInt32(timon[1].Substring(0, 2));
                        }
                        catch
                        {
                            ins = 0;
                        }


                    }
                    catch { }

                    switch (value[2].Substring(0, 2))
                    {
                        case "01":
                            try
                            {
                                mas[0] = outs - ins;
                            }
                            catch { }
                            break;
                        case "02":
                            try
                            {
                                mas[1] = outs - ins;
                            }
                            catch { }
                            break;
                        case "03":
                            try
                            {
                                mas[2] = outs - ins;
                            }
                            catch { }
                            break;
                        case "04":
                            try
                            {
                                mas[3] = outs - ins;
                            }
                            catch { }
                            break;
                        case "05":
                            try
                            {
                                mas[4] = outs - ins;
                            }
                            catch { }
                            break;
                        case "06":
                            try
                            {
                                mas[5] = outs - ins;
                            }
                            catch { }
                            break;
                        case "07":
                            try
                            {
                                mas[6] = outs - ins;
                            }
                            catch { }
                            break;
                        case "08":
                            try
                            {
                                mas[7] = outs - ins;
                            }
                            catch { }
                            break;
                        case "09":
                            try
                            {
                                mas[8] = outs - ins;
                            }
                            catch { }
                            break;
                        case "10":
                            try
                            {
                                mas[9] = outs - ins;
                            }
                            catch { }
                            break;
                        case "11":
                            try
                            {
                                mas[10] = outs - ins;
                            }
                            catch { }
                            break;
                        case "12":
                            try
                            {
                                mas[11] = outs - ins;
                            }
                            catch { }
                            break;
                        case "13":
                            try
                            {
                                mas[12] = outs - ins;
                            }
                            catch { }
                            break;
                        case "14":
                            try
                            {
                                mas[13] = outs - ins;
                            }
                            catch { }
                            break;
                        case "15":
                            try
                            {
                                mas[14] = outs - ins;
                            }
                            catch { }
                            break;
                        case "16":
                            try
                            {
                                mas[15] = outs - ins;
                            }
                            catch { }
                            break;
                        case "17":
                            try
                            {
                                mas[16] = outs - ins;
                            }
                            catch { }
                            break;
                        case "18":
                            try
                            {
                                mas[17] = outs - ins;
                            }
                            catch { }
                            break;
                        case "19":
                            try
                            {
                                mas[18] = outs - ins;
                            }
                            catch { }
                            break;
                        case "20":
                            try
                            {
                                mas[19] = outs - ins;
                            }
                            catch { }
                            break;
                        case "21":
                            try
                            {
                                mas[20] = outs - ins;
                            }
                            catch { }
                            break;
                        case "22":
                            try
                            {
                                mas[21] = outs - ins;
                            }
                            catch { }
                            break;
                        case "23":
                            try
                            {
                                mas[22] = outs - ins;
                            }
                            catch { }
                            break;
                        case "24":
                            try
                            {
                                mas[23] = outs - ins;
                            }
                            catch { }
                            break;
                        case "25":
                            try
                            {
                                mas[24] = outs - ins;
                            }
                            catch { }
                            break;
                        case "26":
                            try
                            {
                                mas[25] = outs - ins;
                            }
                            catch { }
                            break;
                        case "27":
                            try
                            {
                                mas[26] = outs - ins;
                            }
                            catch { }
                            break;
                        case "28":
                            try
                            {
                                mas[27] = outs - ins;
                            }
                            catch { }
                            break;
                        case "29":
                            try
                            {
                                mas[28] = outs - ins;
                            }
                            catch { }
                            break;
                        case "30":
                            try
                            {
                                mas[29] = outs - ins;
                            }
                            catch { }
                            break;
                        case "31":
                            try
                            {
                                mas[30] = outs - ins;
                            }
                            catch { }
                            break;
                        default:
                            break;
                    }

                }
                chart1.Series[0].Points.Clear();
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column; // тут сами поизменяет/повыбирайте тип вывода графика
                for (int i = 1; i < 32; i++)
                {
                    chart1.Series[0].Points.AddXY(i, mas[i-1]);
                }
            }
            else
            {
                chart1.Series[0].Points.Clear();
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column; // тут сами поизменяет/повыбирайте тип вывода графика
                int[] mas = new int[31];
                for (int i = 1; i < 32; i++)
                {
                    chart1.Series[0].Points.AddXY(i, mas[i-1]);
                }
            }


            
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
