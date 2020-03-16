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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private NpgsqlConnection conn;
        readonly string connstring = string.Format("Server={0}; Port={1};"+ 
                    "User Id={2}; Password={3}; Database={4};",
                    "localhost", "5432", "postgres", "root", "testecervantes");
        private NpgsqlCommand cmd;
        private string sql = null;

        private void Carregar_Contatos()
        {
            conn = new NpgsqlConnection(connstring);
            try
            {
                conn.Open();

                string strSQL = "SELECT NUMERO,NOME FROM contatos";
                cmd = new NpgsqlCommand(strSQL, conn);

                try
                {
                    NpgsqlDataAdapter objAdp = new NpgsqlDataAdapter(cmd);

                    DataTable dtLista = new DataTable();

                    objAdp.Fill(dtLista);

                    dgDados.DataSource = dtLista;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    conn.Close();
                }


            }
            catch (Exception ex)
            {             
                MessageBox.Show("Erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }


        private void Add_Contato_Click(object sender, EventArgs e)
        {
            try
            {
                conn = new NpgsqlConnection(connstring);
                conn.Open();
                sql = @"select * from add_contatos(:_numero,:_nome)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_numero", Int64.Parse(numero.Value.ToString()));
                cmd.Parameters.AddWithValue("_nome", nome.Text);

                int resultado = (int)cmd.ExecuteScalar();

                if(resultado == 1)
                {
                    MessageBox.Show("Já existe um contato com este número", "Erro", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(resultado == 2)
                {
                    MessageBox.Show("O número não pode ser igual a Zero.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(resultado == 3)
                {
                    MessageBox.Show("O campo Nome deve ser preenchido.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    conn.Close();
                    MessageBox.Show("Contato salvo com sucesso.", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Carregar_Contatos();
                }

                conn.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            Carregar_Contatos();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Carregar_Contatos();
        }

        private void DgDados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                new Form2(Int64.Parse(dgDados.Rows[e.RowIndex].Cells[0].Value.ToString()), dgDados.Rows[e.RowIndex].Cells[1].Value.ToString()).Show();
                Form2.ActiveForm.Activate();
            }
            
        }

    }
}
