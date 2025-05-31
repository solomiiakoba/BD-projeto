using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace App
{
    public partial class Register : Form
    {
        private string connectionString = "Server=SOLOMIIA;Database=Projeto;Integrated Security=true;";
        private string tipoUtilizadorSelecionado = "";

        public Register()
        {
            InitializeComponent();
            RegisterPnl.Visible = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            RegisterPnl.Visible = false;
        }

        private void linkReg_Click(object sender, EventArgs e)
        {
            RegisterPnl.Visible = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e) { }

        private void artistSignup_Paint(object sender, PaintEventArgs e) { }

        private void clientSignup_Paint(object sender, PaintEventArgs e) { }

        // ========================================
        // MÉTODOS DE AUTENTICAÇÃO
        // ========================================

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            string senha = textBox2.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_LoginUtilizador", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@senha", senha);

                        SqlParameter paramUserId = new SqlParameter("@userId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        SqlParameter paramNome = new SqlParameter("@nome", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                        SqlParameter paramTipo = new SqlParameter("@tipo", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Output };
                        SqlParameter paramResultado = new SqlParameter("@resultado", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        SqlParameter paramMensagem = new SqlParameter("@mensagem", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };

                        cmd.Parameters.Add(paramUserId);
                        cmd.Parameters.Add(paramNome);
                        cmd.Parameters.Add(paramTipo);
                        cmd.Parameters.Add(paramResultado);
                        cmd.Parameters.Add(paramMensagem);

                        cmd.ExecuteNonQuery();

                        int resultado = (int)paramResultado.Value;
                        string mensagem = paramMensagem.Value.ToString();

                        if (resultado == 1)
                        {
                            int userId = (int)paramUserId.Value;
                            string nome = paramNome.Value.ToString();
                            string tipo = paramTipo.Value.ToString();

                            SessaoUtilizador.Id = userId;
                            SessaoUtilizador.Nome = nome;
                            SessaoUtilizador.Email = email;
                            SessaoUtilizador.Tipo = tipo;

                            MessageBox.Show($"Bem-vindo, {nome}!", "Login Realizado",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Define o resultado do diálogo como OK
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(mensagem, "Erro de Login",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro de conexão: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para signup como artista
        private void btnSignupArtist_Click(object sender, EventArgs e)
        {
            tipoUtilizadorSelecionado = "artista";
            RegistrarUtilizador();
        }

        // Método para signup como cliente
        private void btnSignupClient_Click(object sender, EventArgs e)
        {
            tipoUtilizadorSelecionado = "cliente";
            RegistrarUtilizador();
        }

        // Método principal de registro
        private void RegistrarUtilizador()
        {
            string nome = textBox3.Text.Trim();
            string email = textBox4.Text.Trim();
            string senha = textBox5.Text;

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Por favor, insira um email válido.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (senha.Length < 6)
            {
                MessageBox.Show("A senha deve ter pelo menos 6 caracteres.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_RegistrarUtilizador", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parâmetros de entrada
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@telefone", "000000000"); // Padrão temporário
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@senha", senha);
                        cmd.Parameters.AddWithValue("@endereco", "Não informado"); // Padrão temporário
                        cmd.Parameters.AddWithValue("@tipo", tipoUtilizadorSelecionado);

                        // Parâmetros específicos baseado no tipo
                        if (tipoUtilizadorSelecionado == "artista")
                        {
                            cmd.Parameters.AddWithValue("@especialidade", "Não especificado");
                            cmd.Parameters.AddWithValue("@metodo_pagamento", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@especialidade", DBNull.Value);
                            cmd.Parameters.AddWithValue("@metodo_pagamento", DBNull.Value);
                        }

                        // Parâmetros de saída
                        SqlParameter paramResultado = new SqlParameter("@resultado", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        SqlParameter paramMensagem = new SqlParameter("@mensagem", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };

                        cmd.Parameters.Add(paramResultado);
                        cmd.Parameters.Add(paramMensagem);

                        cmd.ExecuteNonQuery();

                        int resultado = (int)paramResultado.Value;
                        string mensagem = paramMensagem.Value.ToString();

                        if (resultado > 0)
                        {
                            MessageBox.Show($"Registro realizado com sucesso!\nID: {resultado}\n{mensagem}",
                                "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpar campos
                            LimparCamposRegistro();

                            // Voltar para o painel de login
                            RegisterPnl.Visible = false;
                        }
                        else
                        {
                            MessageBox.Show(mensagem, "Erro no Registro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro de conexão: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================
        // MÉTODOS AUXILIARES
        // ========================================

        // Validar formato de email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Limpar campos de registro
        private void LimparCamposRegistro()
        {
            textBox3.Text = "";
            textBox4.Text = "";
            textBox4.Text = "";
            tipoUtilizadorSelecionado = "";
        }

        // Verificar se email já existe (opcional - pode ser chamado no evento Leave do campo email)
        private bool EmailJaExiste(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_VerificarEmail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@email", email);

                        SqlParameter paramExiste = new SqlParameter("@existe", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(paramExiste);

                        cmd.ExecuteNonQuery();

                        return (bool)paramExiste.Value;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // Event handler para verificar email em tempo real (opcional)
        private void txtRegEmail_Leave(object sender, EventArgs e)
        {
            string email = textBox4.Text.Trim();
            if (!string.IsNullOrEmpty(email) && EmailJaExiste(email))
            {
                MessageBox.Show("Este email já está registrado.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
            }
        }
    }

    public static class SessaoUtilizador
    {
        public static int Id { get; set; }
        public static string Nome { get; set; }
        public static string Email { get; set; }
        public static string Tipo { get; set; }

        public static void LimparSessao()
        {
            Id = 0;
            Nome = "";
            Email = "";
            Tipo = "";
        }
        public static bool EstaLogado => Id > 0;
    }
}