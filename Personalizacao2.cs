using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace App
{
    public partial class Personalizacao2 : Form
    {
        private int trabalhoId;
        private string connectionString = "Data Source=SOLOMIIA;Initial Catalog=Projeto;Integrated Security=True";
        private TrabalhoDetalhes trabalhoAtual;
        private decimal precoBase = 0;
        private decimal precoFinal = 0;
        private Label lblDetalhesTrabalho;
        
        public Personalizacao2(int trabalhoId)
        {
            InitializeComponent();
            this.trabalhoId = trabalhoId;
            ConfigurarInterface();
            CarregarDadosTrabalho();
            CarregarOpcoesPersonalizacao();
        }

        private void ConfigurarInterface()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            CriarControlesPersonalizacao();
        }

        private void CriarControlesPersonalizacao()
        {

            lblDetalhesTrabalho = new Label
            {
                Text = "detalhes...",
                Font = new Font("Tahoma", 10),
                ForeColor = Color.Black,
                Location = new Point(10, 10),
                Size = new Size(780, 100),
                AutoSize = false
            };
            pnlDetalhes.Controls.Add(lblDetalhesTrabalho);
            Trabalho_Pnl.Controls.Add(pnlDetalhes);
            comboTamanho.SelectedIndexChanged += CalcularPrecoFinal;
            comboTamanho.DropDownStyle = ComboBoxStyle.DropDownList;

            Trabalho_Pnl.Controls.Add(comboTamanho);
            cmbTecnica.SelectedIndexChanged += CalcularPrecoFinal;

            cmbMaterial.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMaterial.SelectedIndexChanged += CalcularPrecoFinal;
            Trabalho_Pnl.Controls.Add(cmbMaterial);

            txtDescricaoPersonalizada.Multiline = true;
            txtDescricaoPersonalizada.ScrollBars = ScrollBars.Vertical;
            txtDescricaoPersonalizada.Size = new Size(350, 80);

            dtpDataEntrega.Format = DateTimePickerFormat.Short;
            dtpDataEntrega.Font = new Font("Tahoma", 9);
            dtpDataEntrega.Size = new Size(200, 25);
            dtpDataEntrega.MinDate = DateTime.Now.AddDays(7);
            dtpDataEntrega.Value = DateTime.Now.AddDays(14);
            Trabalho_Pnl.Controls.Add(dtpDataEntrega);
            lblPrecoFinal = new Label
            {
                Text = "Preço Final: €0,00",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                Location = new Point(10, 15),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlPreco.Controls.Add(lblPrecoFinal);
            Trabalho_Pnl.Controls.Add(pnlPreco);
            btnConfirmar.Click -= BtnConfirmar_Click; // para evitar duplicação
            btnConfirmar.Click += BtnConfirmar_Click;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            Trabalho_Pnl.Controls.Add(btnCancelar);
        }

        private void CarregarDadosTrabalho()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            t.id,
                            t.titulo,
                            t.descricao,
                            t.preco,
                            t.disponibilidade,
                            t.estilo,
                            t.dimensoes,
                            t.material,
                            t.tecnica,
                            c.nome_categoria,
                            u.nome AS nome_artista,
                            a.especialidade
                        FROM Trabalho t
                        INNER JOIN Categoria c ON t.id_categoria = c.id
                        INNER JOIN Portfolio p ON c.id_portfolio = p.id
                        INNER JOIN Artista a ON p.artista = a.id
                        INNER JOIN Utilizador u ON a.id = u.id
                        WHERE t.id = @TrabalhoId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TrabalhoId", trabalhoId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            trabalhoAtual = new TrabalhoDetalhes
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Titulo = reader.IsDBNull(reader.GetOrdinal("titulo")) ? "Sem título" : reader.GetString(reader.GetOrdinal("titulo")),
                                Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? "Sem descrição" : reader.GetString(reader.GetOrdinal("descricao")),
                                Preco = reader.IsDBNull(reader.GetOrdinal("preco")) ? 0 : reader.GetDecimal(reader.GetOrdinal("preco")),
                                Disponibilidade = reader.IsDBNull(reader.GetOrdinal("disponibilidade")) ? "" : reader.GetString(reader.GetOrdinal("disponibilidade")),
                                Estilo = reader.IsDBNull(reader.GetOrdinal("estilo")) ? "Não informado" : reader.GetString(reader.GetOrdinal("estilo")),
                                Dimensoes = reader.IsDBNull(reader.GetOrdinal("dimensoes")) ? "Não informado" : reader.GetString(reader.GetOrdinal("dimensoes")),
                                Material = reader.IsDBNull(reader.GetOrdinal("material")) ? "Não informado" : reader.GetString(reader.GetOrdinal("material")),
                                Tecnica = reader.IsDBNull(reader.GetOrdinal("tecnica")) ? "Não informado" : reader.GetString(reader.GetOrdinal("tecnica")),
                                NomeCategoria = reader.GetString(reader.GetOrdinal("nome_categoria")),
                                NomeArtista = reader.GetString(reader.GetOrdinal("nome_artista")),
                                Especialidade = reader.GetString(reader.GetOrdinal("especialidade"))
                            };

                        precoBase = trabalhoAtual.Preco;
                            AtualizarDetalhesTrabalho();
                        }
                        else
                        {
                            MessageBox.Show("Trabalho não encontrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do trabalho: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void AtualizarDetalhesTrabalho()
        {
            if (trabalhoAtual != null)
            {
                string detalhes = $"Título: {trabalhoAtual.Titulo}\n" +
                                $"Artista: {trabalhoAtual.NomeArtista} ({trabalhoAtual.Especialidade})\n" +
                                $"Categoria: {trabalhoAtual.NomeCategoria}\n" +
                                $"Estilo: {trabalhoAtual.Estilo}\n" +
                                $"Dimensões Originais: {trabalhoAtual.Dimensoes}\n" +
                                $"Material Original: {trabalhoAtual.Material}\n" +
                                $"Técnica Original: {trabalhoAtual.Tecnica}\n" +
                                $"Preço Base: {trabalhoAtual.Preco:C}";

                lblDetalhesTrabalho.Text = detalhes;
            }
        }

        private void CarregarOpcoesPersonalizacao()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    CarregarComboBox(comboTamanho, "tamanho", connection);
                    CarregarComboBox(cmbTecnica, "tecnica", connection);
                    CarregarComboBox(cmbMaterial, "material", connection);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar opções de personalização: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarComboBox(ComboBox combo, string tipoOpcao, SqlConnection connection)
        {
            string query = @"SELECT id, valor_formatado FROM vw_OpcoesPersonalizacao 
                    WHERE tipo_opcao = @TipoOpcao ORDER BY preco_adicional";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TipoOpcao", tipoOpcao);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<OpcaoPersonalizacao> opcoes = new List<OpcaoPersonalizacao>();

                    while (reader.Read())
                    {
                        opcoes.Add(new OpcaoPersonalizacao
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Valor = reader.GetString(reader.GetOrdinal("valor_formatado"))
                        });
                    }

                    combo.DataSource = opcoes;
                    combo.DisplayMember = "Valor";
                    combo.ValueMember = "Id";
                }
            }
        }

        private void CalcularPrecoFinal(object sender, EventArgs e)
        {
            if (comboTamanho.SelectedItem == null || cmbTecnica.SelectedItem == null || cmbMaterial.SelectedItem == null)
                return;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT dbo.fn_CalcularPrecoPersonalizacao(@PrecoBase, @IdTamanho, @IdTecnica, @IdMaterial)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PrecoBase", precoBase);

                    // Get the ID from the SelectedItem, not SelectedValue
                    command.Parameters.AddWithValue("@IdTamanho", ((OpcaoPersonalizacao)comboTamanho.SelectedItem).Id);
                    command.Parameters.AddWithValue("@IdTecnica", ((OpcaoPersonalizacao)cmbTecnica.SelectedItem).Id);
                    command.Parameters.AddWithValue("@IdMaterial", ((OpcaoPersonalizacao)cmbMaterial.SelectedItem).Id);

                    object result = command.ExecuteScalar();
                    precoFinal = Convert.ToDecimal(result);

                    lblPrecoFinal.Text = $"Preço Final: {precoFinal:C}";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao calcular preço: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            btnConfirmar.Enabled = false; // Prevent double-click

            // Validation
            if (string.IsNullOrWhiteSpace(txtDescricaoPersonalizada.Text))
            {
                MessageBox.Show("Por favor, forneça uma descrição para a personalização.",
                              "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnConfirmar.Enabled = true;
                return;
            }

            if (dtpDataEntrega.Value < DateTime.Now.AddDays(7))
            {
                MessageBox.Show("A data de entrega deve ser pelo menos 7 dias a partir de hoje.",
                              "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnConfirmar.Enabled = true;
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("sp_CriarEncomendaPersonalizada", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        int idCliente = SessaoUtilizador.Id > 0 ? SessaoUtilizador.Id : 1;

                        // Input parameters
                        command.Parameters.AddWithValue("@IdCliente", idCliente);
                        command.Parameters.AddWithValue("@IdTrabalho", trabalhoId);
                        command.Parameters.AddWithValue("@IdTamanho", ((OpcaoPersonalizacao)comboTamanho.SelectedItem).Id);
                        command.Parameters.AddWithValue("@IdTecnica", ((OpcaoPersonalizacao)cmbTecnica.SelectedItem).Id);
                        command.Parameters.AddWithValue("@IdMaterial", ((OpcaoPersonalizacao)cmbMaterial.SelectedItem).Id);
                        command.Parameters.AddWithValue("@DescricaoPersonalizada", txtDescricaoPersonalizada.Text);
                        command.Parameters.AddWithValue("@DataEntregaDesejada", dtpDataEntrega.Value);

                        // Output parameters
                        SqlParameter outputParam = new SqlParameter("@NovoIdEncomenda", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(outputParam);

                        SqlParameter errorParam = new SqlParameter("@MensagemErro", SqlDbType.VarChar, 500);
                        errorParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errorParam);

                        command.ExecuteNonQuery();

                        // Get results
                        int novoIdEncomenda = outputParam.Value != DBNull.Value ? (int)outputParam.Value : -1;
                        string mensagemErro = errorParam.Value != DBNull.Value ? (string)errorParam.Value : "";

                        if (novoIdEncomenda == -1)
                        {
                            MessageBox.Show(mensagemErro, "Erro de Validação",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            btnConfirmar.Enabled = true;
                            return;
                        }

                        MessageBox.Show($"Personalização solicitada com sucesso!\n" +
                                      $"Número da encomenda: {novoIdEncomenda}\n" +
                                      $"Preço final: {precoFinal:C}\n" +
                                      $"Data de entrega: {dtpDataEntrega.Value:dd/MM/yyyy}\n\n" +
                                      $"Você receberá uma confirmação em breve.",
                                      "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar encomenda: {ex.Message}",
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnConfirmar.Enabled = true;
            }
        }


        private void button1_Paint(object sender, PaintEventArgs e)
        {

        }
        public class TrabalhoDetalhes
        {
            public int Id { get; set; }
            public string Titulo { get; set; }
            public string Descricao { get; set; }
            public decimal Preco { get; set; }
            public string Disponibilidade { get; set; }
            public string Estilo { get; set; }
            public string Dimensoes { get; set; }
            public string Material { get; set; }
            public string Tecnica { get; set; }
            public string NomeCategoria { get; set; }
            public string NomeArtista { get; set; }
            public string Especialidade { get; set; }
        }
        public class OpcaoPersonalizacao
        {
            public int Id { get; set; }
            public string Valor { get; set; }

            public override string ToString()
            {
                return Valor;
            }
        }


        private void Trabalho_Pnl_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
