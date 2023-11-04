using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeradorPlanilhaHoras
{

    public partial class Configuracoes : Form
    {
        string nomeColaborador = "";
        string emailColaborador = "";
        string emailGestor = "";
        public Configuracoes()
        {
            InitializeComponent();

            string caminhoArquivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Brunsker\config.txt");

            string[] linhas = File.ReadAllLines(caminhoArquivo);

            foreach (string linha in linhas)
            {
                if (linha.Contains("Nome"))
                {
                    string[] nome = linha.Split('-');

                    nomeColaborador = nome[1];
                    break;
                }
            }

            foreach (string linha in linhas)
            {
                if (linha.Contains("Email"))
                {
                    string[] nome = linha.Split('-');

                    emailColaborador = nome[1];
                    break;
                }
            }

            foreach (string linha in linhas)
            {
                if (linha.Contains("EmailGestor"))
                {
                    string[] nome = linha.Split('-');

                    emailGestor = nome[1];
                    break;
                }
            }

            textBox1.Text = nomeColaborador;
            textBox2.Text = emailColaborador;
            textBox3.Text = emailGestor;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {



        }

        private void button5_Click(object sender, EventArgs e)
        {
            string nomeColaborador = "Nome - " + textBox1.Text;
            string emailColaborador = "Email - " + textBox2.Text;
            string emailGestor = "EmailGestor - " + textBox3.Text;

            string caminhoArquivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Brunsker\config.txt");




            // Escreve o texto no arquivo (sobrescreve o arquivo se já existir)
            using (StreamWriter writer = new StreamWriter(caminhoArquivo))
            {
                writer.Write(string.Empty);
                writer.WriteLine(nomeColaborador + "\n\n" + emailColaborador + "\n\n" + emailGestor);
            }

            Form1 formPrincipal = new Form1();

            formPrincipal.preencheNomeCSV(nomeColaborador);

            Configuracoes config = new Configuracoes();
            this.Close();

            string message2 = "Informações atualizadas com sucesso !";
            string caption2 = "Arquivo atualizado !";
            MessageBoxButtons buttons2 = MessageBoxButtons.OK;
            DialogResult result2;

            result2 = MessageBox.Show(message2, caption2, buttons2);

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
          
                if (textBox1.Text != nomeColaborador || textBox2.Text != emailColaborador || textBox3.Text != emailGestor)
                {
                    string message2 = "Há informações modificadas não salvas, clicar em 'OK' implicará na perda dessas informações!\n\n Deseja continuar ?";
                    string caption2 = "Informações Pendentes !";

                    MessageBoxButtons buttons2 = MessageBoxButtons.YesNo;
                    DialogResult result2;

                    result2 = MessageBox.Show(message2, caption2, buttons2);

                    if (result2 == DialogResult.Yes)
                    {
                        this.Close();
                    }
            }
            else
            {
                this.Close();
            }
        }
    }
}
