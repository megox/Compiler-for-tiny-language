using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_COMPILER
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string source_code = textBox1.Text;
            Tiny_compiler.Start_Compiling(source_code);
            Print_Tokens();
            treeView1.Nodes.Add(Parser.PrintParseTree(Tiny_compiler.treeroot));
            Print_Errors();

        }
        void Print_Tokens()
        {
            for (int i = 0; i < Tiny_compiler.Tiny_Scanner.Tokens.Count; i++)
            {
                dataGridView1.Rows.Add(Tiny_compiler.Tiny_Scanner.Tokens.ElementAt(i).lexema, Tiny_compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type);
            }
        }
        void Print_Errors()
        {
            foreach (string i in Tiny_compiler.Tiny_Scanner.errors)
            {
                textBox2.Text += i;
                textBox2.Text += "\r\n";
            }
            foreach (string i in Tiny_compiler.Tiny_Parser.errors)
            {
                textBox2.Text += i;
                textBox2.Text += "\r\n";
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            dataGridView1.Rows.Clear();
            treeView1.Nodes.Clear();
            Tiny_compiler.Tiny_Scanner.Tokens.Clear();
            Tiny_compiler.Tiny_Scanner.errors.Clear();
            Tiny_compiler.Tiny_Parser.errors.Clear();
            Tiny_compiler.TokenStream.Clear();
        }
    }
}
