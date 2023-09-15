using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.Globalization;
using System.Net.Mail;

namespace GeradorPlanilhaHoras
{
    public partial class Form1 : Form
    {
        private Stopwatch stopwatch;
        public Form1()
        {
            InitializeComponent();
        }

        string cliente = "";
        string motivo = "";
        string data = "";
        string horaInicio = "";
        string horaFim = "";
        string horaInicioCompleta = "";
        string horaFimCompleta = "";
        string nomeColaborador = "";
        string emailColaborador = "";

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        public void Form1_Load(object sender, EventArgs e)
        {
            stopwatch = new Stopwatch();


            string caminhoArquivo = "C:\\GeradorPlanilhaHoras\\config.txt";

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

            textBoxNome.Text = nomeColaborador;
            textBoxEmail.Text = emailColaborador;
            string totalHorasTrabalhadas = retornaHorasTrabalhadasTotal();

            label6.Text = totalHorasTrabalhadas;

        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (!String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text))
            {
                stopwatch.Start();

                cliente = textBox1.Text;
                motivo = textBox2.Text;
                data = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                horaInicio = DateTime.Now.Hour + ":" + DateTime.Now.Minute.ToString("00.##");
                horaInicioCompleta = DateTime.Now.ToString();
            }
            else
            {
                string message = "Preencha todos os campos antes de inicializar o Timer !";
                string caption = "Informações faltantes";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);

            }


        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (stopwatch.IsRunning)
            {


                stopwatch.Stop();

                horaFim = DateTime.Now.Hour + ":" + DateTime.Now.Minute.ToString("00.##");
                horaFimCompleta = DateTime.Now.ToString();

                preencheArquivoCSV(cliente, motivo, data, horaInicio, horaFim, horaInicioCompleta, horaFimCompleta);

                stopwatch.Reset();

                string totalHorasTrabalhadas = retornaHorasTrabalhadasTotal();

                label6.Text = totalHorasTrabalhadas;

                preencheHoraToTalCSV(totalHorasTrabalhadas);
            }
            else
            {
                string message = "O Timer não foi inicializado !";
                string caption = "Timer não Inicializado !";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
            }

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            this.label1.Text = string.Format("{0:hh\\:mm\\:ss}", stopwatch.Elapsed);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void preencheHoraToTalCSV(string horaTotal)
        {

            try
            {

                CultureInfo culture = new CultureInfo("pt-BR");
                DateTimeFormatInfo dtfi = culture.DateTimeFormat;

                string caminhoArquivo = "C:\\GeradorPlanilhaHoras\\horasExtras.xlsx";

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(caminhoArquivo)))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    worksheet.Cells[3, 2].Value = horaTotal;
     

                    package.Save();


                }


            }
            catch (Exception ex)
            {
                string message = "Erro ao enviar o arquivo CSV ! Não foi possível calcular ou inserir o total de horas !";
                string caption = "Erro ao atualizar o arquivo CSV !";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
            }
        }



        private void preencheArquivoCSV(string cliente, string motivo, string data, string horaInicio, string horaFim, string horaInicioCompleta, string horaFimCompleta)
        {

            try
            {

                DateTime horaIniComple = DateTime.Parse(horaInicioCompleta);
                DateTime horaFimComple = DateTime.Parse(horaFimCompleta);
                TimeSpan tempoTrabalhado = (horaFimComple - horaIniComple);
                DateTime diaSemanaComple = DateTime.Now;


                CultureInfo culture = new CultureInfo("pt-BR");
                DateTimeFormatInfo dtfi = culture.DateTimeFormat;

                string diaSemana = dtfi.GetDayName(diaSemanaComple.DayOfWeek).ToUpper();

                string totalTrabalhado = tempoTrabalhado.Hours.ToString("00.##") + ":" + tempoTrabalhado.Minutes.ToString("00.##") + ":" + tempoTrabalhado.Seconds.ToString("00.##");

                string caminhoArquivo = "C:\\GeradorPlanilhaHoras\\horasExtras.xlsx";

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(caminhoArquivo)))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    // última linha com dados
                    int ultimaLinhaComDados = worksheet.Dimension.End.Row + 1;

                    string formulaData = string.Format("=TEXTO(B{0}; \"DDDD\")", ultimaLinhaComDados);


                    worksheet.Cells[ultimaLinhaComDados, 1].Value = cliente;
                    worksheet.Cells[ultimaLinhaComDados, 2].Value = data;
                    worksheet.Cells[ultimaLinhaComDados, 3].Value = diaSemana;
                    worksheet.Cells[ultimaLinhaComDados, 4].Value = horaInicio;
                    worksheet.Cells[ultimaLinhaComDados, 5].Value = horaFim;
                    worksheet.Cells[ultimaLinhaComDados, 6].Style.Numberformat.Format = "hh:mm:ss";
                    worksheet.Cells[ultimaLinhaComDados, 6].Value = totalTrabalhado;
                    worksheet.Cells[ultimaLinhaComDados, 7].Value = motivo;

                    package.Save();





                    string message = "Arquivo atualizado com sucesso !";
                    string caption = "Arquivo atualizado !";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    result = MessageBox.Show(message, caption, buttons);
                }


            }
            catch (Exception ex)
            {
                string message = "Erro ao atualizar o arquivo CSV !" + ex.ToString();
                string caption = "Erro ao atualizar o arquivo CSV !";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!stopwatch.IsRunning)
            {

                string caminhoArquivo = "C:\\GeradorPlanilhaHoras\\config.txt";

                string[] linhas = File.ReadAllLines(caminhoArquivo);


                foreach (string linha in linhas)
                {
                    if (linha.Contains("Email"))
                    {
                        string[] nome = linha.Split('-');

                        emailColaborador = nome[1];
                        break;
                    }
                }

                if (!String.IsNullOrEmpty(emailColaborador))
                {
                    enviaEmail();
                }
                else
                {
                    string message = "Não será possível realizar o envio de sua planilha de horas. Motivo : Insira seu e-mail no campo de e-mail e confirme com o botão 'OK' ! ";
                    string caption = "Informações faltantes";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    result = MessageBox.Show(message, caption, buttons);
                }
            }
            else
            {
                string message3 = "Pause o Timer antes de enviar sua planilha !";
                string caption3 = "Erro ao enviar planilha!";
                MessageBoxButtons buttons3 = MessageBoxButtons.OK;
                DialogResult result4;

                result4 = MessageBox.Show(message3, caption3, buttons3);
            }

        }

        public void enviaEmail()
        {
            string totalHorasTrabalhadas = retornaHorasTrabalhadasTotal();

            preencheHoraToTalCSV(totalHorasTrabalhadas);

            string messageA = "Deseja realmente enviar a planilha de horas ?";
            string captionA = "Atenção !";
            MessageBoxButtons buttonsA = MessageBoxButtons.YesNo;
            DialogResult resultA;

            resultA = MessageBox.Show(messageA, captionA, buttonsA);

            if (resultA == System.Windows.Forms.DialogResult.Yes)
            {

                string caminhoArquivo = "C:\\GeradorPlanilhaHoras\\config.txt";

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

                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress(emailColaborador);
                    mail.To.Add("vitoralves1604@gmail.com");
                    mail.Subject = "Planilha de Horas Extras";
                    mail.Body = $"Bom dia, \n\n" +
                                $"Segue em anexo minha planilha de horas extras.\n\n" +
                                $"\n\n" +
                                $"Quaisquer dúvidas estou à disposição !\n\n" +
                                $"\n\n" +
                                $"Atenciosamente, {nomeColaborador}";



                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment("C:\\GeradorPlanilhaHoras\\horasExtras.xlsx");
                    mail.Attachments.Add(attachment);

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("joao.simiao@brunsker.com.br", "110657756011");
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);

                    string message = "Arquivo enviado com sucesso por e-mail !";
                    string caption = "Arquivo enviado !";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    result = MessageBox.Show(message, caption, buttons);
                }
                catch (Exception ex)
                {
                    string message = "Erro ao enviar o arquivo CSV !" + ex.ToString();
                    string caption = "Erro ao enviar o arquivo CSV por e-mail !";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;

                    result = MessageBox.Show(message, caption, buttons);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }

            }
        }

        public string retornaHorasTrabalhadasTotal()
        {
            string caminhoArquivo = "C:\\GeradorPlanilhaHoras\\horasExtras.xlsx";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            TimeSpan somaTotal = TimeSpan.Zero;

            using (var package = new ExcelPackage(new FileInfo(caminhoArquivo)))
            {

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // última linha com dados
                int ultimaLinhaComDados = worksheet.Dimension.End.Row;
                string coluna = "H";
                string[] horasDeTrabalho = new string[(ultimaLinhaComDados - 5) + 1];
                int linhaPosicao = 0;

                for (int i = 5; i <= ultimaLinhaComDados; i++)
                {
                    // Lê o valor da célula na coluna "H" (outra forma de representar a coluna)
                    var cellValue = worksheet.Cells[i, 6].Value;

                    horasDeTrabalho[linhaPosicao] = cellValue.ToString();
                    
                    linhaPosicao++;
                }
               

                foreach (string hora in horasDeTrabalho)
                {
                   
                    TimeSpan horas = TimeSpan.Parse(hora);

                
                    somaTotal = somaTotal.Add(horas);
                }

            }

            return somaTotal.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!stopwatch.IsRunning)
            {

                string message = "Deseja realmente limpar a planilha de horas ?";
                string caption = "Atenção !";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {

                    try
                    {
                        this.Cursor = Cursors.WaitCursor;

                        string caminhoArquivo = "C:\\GeradorPlanilhaHoras\\horasExtras.xlsx";

                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(new FileInfo(caminhoArquivo)))
                        {

                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];


                            for (int i = 5; i <= 100; i++)
                            {
                                worksheet.DeleteRow(5);
                                package.Save();
                            }

                            string totalHorasTrabalhadas = retornaHorasTrabalhadasTotal();

                            label6.Text = totalHorasTrabalhadas;

                            string message2 = "Arquivo atualizado com sucesso !";
                            string caption2 = "Arquivo atualizado !";
                            MessageBoxButtons buttons2 = MessageBoxButtons.OK;
                            DialogResult result2;

                            result = MessageBox.Show(message2, caption2, buttons2);
                        }


                    }
                    catch (Exception ex)
                    {
                        string message3 = "Erro ao limpar o arquivo CSV !" + ex.ToString();
                        string caption3 = "Erro ao limpar o arquivo CSV !";
                        MessageBoxButtons buttons3 = MessageBoxButtons.OK;
                        DialogResult result3;

                        result = MessageBox.Show(message3, caption3, buttons3);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
            else
            {
                string message3 = "Pause o Timer antes de limpar sua planilha !";
                string caption3 = "Erro ao limpar planilha!";
                MessageBoxButtons buttons3 = MessageBoxButtons.OK;
                DialogResult result4;

                result4 = MessageBox.Show(message3, caption3, buttons3);
            }
        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string nomeColaborador = "Nome - " + textBoxNome.Text;
            string emailColaborador = "Email - " + textBoxEmail.Text;

            string caminhoArquivo = "C:\\GeradorPlanilhaHoras\\config.txt";

            // Escreve o texto no arquivo (sobrescreve o arquivo se já existir)
            using (StreamWriter writer = new StreamWriter(caminhoArquivo))
            {
                writer.Write(string.Empty);
                writer.WriteLine(nomeColaborador + "\n\n" + emailColaborador);
            }

            preencheNomeCSV(textBoxNome.Text);

            string message = "Configurações de e-mail e nome salvas com sucesso !";
            string caption = "Atenção !";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);
        }

        private void preencheNomeCSV(string nome)
        {

            try
            {

                CultureInfo culture = new CultureInfo("pt-BR");
                DateTimeFormatInfo dtfi = culture.DateTimeFormat;

                string caminhoArquivo = "C:\\GeradorPlanilhaHoras\\horasExtras.xlsx";

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(caminhoArquivo)))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    worksheet.Cells[2, 2].Value = nome;


                    package.Save();


                }


            }
            catch (Exception ex)
            {
                string message = "Erro ao enviar o arquivo CSV ! Não foi possível calcular ou inserir o total de horas !";
                string caption = "Erro ao atualizar o arquivo CSV !";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
            }
        }

        private void textBoxNome_TextChanged(object sender, EventArgs e)
        {




        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}

