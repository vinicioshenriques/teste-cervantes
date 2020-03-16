using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrudPostgres
{
    public partial class Form2 : Form
    {
        public Form2(Int64 numeroEditando, string nomeEditando)
        {
            this.numeroEditando = numeroEditando;
            this.nomeEditando = nomeEditando;
            InitializeComponent();
            this.TopMost = true;
        }

        private Int64 numeroEditando;
        private string nomeEditando;

        private NpgsqlConnection conn;
        readonly string connstring = string.Format("Server={0}; Port={1};" +
                    "User Id={2}; Password={3}; Database={4};",
                    "localhost", "5432", "postgres", "root", "testecervantes");
        private NpgsqlCommand cmd;
        private string sql = null;


        private void Edit_Click(object sender, EventArgs e)
        {
            try
            {
                conn = new NpgsqlConnection(connstring);
                conn.Open();
                sql = @"select * from editar_contato(:_numero_antigo,:_numero_novo,:_nome_antigo,:_nome_novo)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_numero_antigo", this.numeroEditando);
                cmd.Parameters.AddWithValue("_numero_novo", Int64.Parse(numero.Value.ToString()));
                cmd.Parameters.AddWithValue("_nome_antigo", this.nomeEditando);
                cmd.Parameters.AddWithValue("_nome_novo", nome.Text);

                int resultado = (int)cmd.ExecuteScalar();

                if (resultado == 1)
                {
                    MessageBox.Show("Já existe um contato com este número", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (resultado == 2)
                {
                    MessageBox.Show("O número não pode ser igual a Zero.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (resultado == 3)
                {
                    MessageBox.Show("O campo nome deve ser preenchido.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    conn.Close();
                    MessageBox.Show("Contato Atualizado com sucesso.", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }

                conn.Close();
            }
            catch (Exception ex)
            { 
                MessageBox.Show("Erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            try
            {
                conn = new NpgsqlConnection(connstring);
                conn.Open();
                sql = @"select * from deletar_contato(:_numero)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_numero", this.numeroEditando);

                int resultado = (int)cmd.ExecuteScalar();

                if (resultado == 1)
                {
                    MessageBox.Show("Número nào existe no banco", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    conn.Close();
                    MessageBox.Show("Contato Excluido com sucesso.", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            nome.Text = nomeEditando;
            numero.Value = numeroEditando;
        }

        private void Form2_Closing(object sender, EventArgs e)
        {
            Form1.ActiveForm.Show();
            Form1.ActiveForm.Activate();
        }
    }
}
