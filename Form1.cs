using Leaf.xNet;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace ProxyCheked
{
    public partial class Form1 : Form
    {
        Thread[] Potok = new Thread[1];
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) // Старт проги
        {
            Potok[0] = new Thread(GiveProxy);
            Potok[0].Start();
        }

        private void GiveProxy()// Получие прокси
        {
            var count = 0;
            Invoke((MethodInvoker)delegate () { count = richTextBox2.Lines.Count(); });

            for (var i = 0; i < count; i++)
            {
                var data = new HttpRequest();
                var lines = string.Empty;
                Invoke((MethodInvoker)delegate () { lines = richTextBox2.Lines[i]; });
                var response = data.Get(lines).ToString();
                RegularVirag(response);
            }
        }

        private void RegularVirag(string response)// вытаскивание проксей
        {
            var regex = new Regex(@"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}:\d{1,5}");
            var matches = regex.Matches(response);

            if (matches.Count > 0)
                foreach (Match match in matches)
                    Invoke((MethodInvoker)delegate () { richTextBox1.Text += match.Value + "\n"; });//поток
        }
        private void button2_Click(object sender, EventArgs e)//Стоп поиска
        {
            Potok[0].Abort();
        }

        private void button3_Click(object sender, EventArgs e)//Сохранине проксей
        {

        }

        private void button4_Click(object sender, EventArgs e)//Открытие сайтов
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)//proxy
        {
            groupBox1.Text = "Proxy [" + richTextBox1.Lines.Count().ToString() + "]";// отображение кол-во проксей
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)//sites
        {

        }
    }
}
