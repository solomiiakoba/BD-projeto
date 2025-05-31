using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace App
{
    public partial class Personalizacao : Form
    {
        private string connectionString = "Data Source=SOLOMIIA;Initial Catalog=Projeto;Integrated Security=True";
        private int idCliente; 

        // Controles do formulário
        private TextBox textBoxDescricao;
        private string imagemPath = "";
        private decimal precoAtual = 0;

        public Personalizacao(int clienteId)
        {
            InitializeComponent();
            this.idCliente = clienteId;
            InitializeCustomComponents();
            CarregarDadosIniciais();
        }

        private void InitializeCustomComponents()
        {
            // PictureBox da imagem
            pictureBoxImagem.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxImagem.SizeMode = PictureBoxSizeMode.Zoom;

            buttonSelecionarImagem.Click += ButtonSelecionarImagem_Click;

            comboBoxCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxTamanho.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxTecnica.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMaterial.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxArtista.DropDownStyle = ComboBoxStyle.DropDownList;

            comboBoxCategoria.SelectedIndexChanged += ComboBoxCategoria_SelectedIndexChanged;
            comboBoxTamanho.SelectedIndexChanged += ComboBox_PrecoChanged;
            comboBoxTecnica.SelectedIndexChanged += ComboBox_PrecoChanged;
            comboBoxMaterial.SelectedIndexChanged += ComboBox_PrecoChanged;

            // ComboBoxes
            comboBoxCategoria.Size = new Size(250, 25);
            comboBoxTamanho.Size = new Size(250, 25);
            comboBoxTecnica.Size = new Size(250, 25);
            comboBoxMaterial.Size = new Size(250, 25);
            comboBoxArtista.Size = new Size(250, 25);

            dateTimePickerEntrega.Format = DateTimePickerFormat.Short;
            dateTimePickerEntrega.MinDate = DateTime.Now.AddDays(7);
            dateTimePickerEntrega.Size = new Size(250, 25);
            labelPreco.Text = "€0,00";
            labelPreco.Size = new Size(100, 25);
            labelPreco.Font = new Font("Tahoma", 10, FontStyle.Bold);
            labelPreco.ForeColor = Color.Green;

            // Caixa de texto para descrição
            textBoxDescricao = new TextBox
            {
                Size = new Size(300, 90),
                Location = new Point(20, 290),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Text = "Descreva detalhadamente o que pretende..."
            };

            // Botões
            buttonConfirmar.Text = "Confirmar";
            buttonConfirmar.Click += ButtonConfirmar_Click;

            buttonCancelar.Text = "Cancelar";
            buttonCancelar.Click += (s, e) => this.Close();
            Trabalho_Pnl.Controls.AddRange(new Control[] {
                dateTimePickerEntrega,
                pictureBoxImagem,
                textBoxDescricao,
                buttonConfirmar,
                buttonCancelar
        });

            // Adiciona o painel ao formulário
            this.Controls.Add(Trabalho_Pnl);
        }


        private void CarregarDadosIniciais()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ObterDadosPersonalizacao", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Primeiro result set: Categorias
                            while (reader.Read())
                            {
                                comboBoxCategoria.Items.Add(new ComboBoxItem
                                {
                                    Text = reader["nome_categoria"].ToString(),
                                    Value = Convert.ToInt32(reader["id"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComboBoxCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCategoria.SelectedItem != null)
            {
                int categoriaId = ((ComboBoxItem)comboBoxCategoria.SelectedItem).Value;

                // Limpar seleções anteriores
                comboBoxTecnica.SelectedIndex = -1;
                comboBoxMaterial.SelectedIndex = -1;
                comboBoxArtista.SelectedIndex = -1;

                labelPreco.Text = "€0.00";
                CarregarArtistas(categoriaId);
                CarregarOpcoesCategoria(categoriaId);
            }
        }

        private void CarregarOpcoesCategoria(int categoriaId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ObterOpcoesPorCategoria", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_categoria", categoriaId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            comboBoxTecnica.Items.Clear();
                            comboBoxMaterial.Items.Clear();

                            comboBoxTamanho.Items.Clear(); // Limpa para recarregar
                            while (reader.Read())
                            {
                                comboBoxTamanho.Items.Add(reader["valor"].ToString());
                            }

                            // Segundo result set: Técnicas específicas da categoria
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    comboBoxTecnica.Items.Add(reader["valor"].ToString());
                                }
                            }

                            // Terceiro result set: Materiais específicos da categoria
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    comboBoxMaterial.Items.Add(reader["valor"].ToString());
                                }
                            }
                        }
                    }
                }

                // Verificar se há opções disponíveis
                if (comboBoxTecnica.Items.Count == 0)
                {
                    MessageBox.Show("Nenhuma técnica disponível para esta categoria.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (comboBoxMaterial.Items.Count == 0)
                {
                    MessageBox.Show("Nenhum material disponível para esta categoria.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar opções: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarArtistas(int categoriaId)
        {
            try
            {
                comboBoxArtista.Items.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ObterArtistasPorCategoria", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_categoria", categoriaId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string nome = reader["nome"].ToString();
                                string especialidade = reader["especialidade"].ToString();
                                decimal mediaAvaliacao = reader["media_avaliacao"] != DBNull.Value ?
                                    Convert.ToDecimal(reader["media_avaliacao"]) : 0;

                                string displayText = $"{nome} - {especialidade} (★{mediaAvaliacao:F1})";

                                comboBoxArtista.Items.Add(new ComboBoxItem
                                {
                                    Text = displayText,
                                    Value = Convert.ToInt32(reader["id"])
                                });
                            }
                        }
                    }
                }

                if (comboBoxArtista.Items.Count == 0)
                {
                    MessageBox.Show("Nenhum artista disponível para esta categoria.", "Informação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar artistas: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComboBox_PrecoChanged(object sender, EventArgs e)
        {
            CalcularPreco();
        }

        private void CalcularPreco()
        {
            if (comboBoxTamanho.SelectedItem != null &&
                comboBoxTecnica.SelectedItem != null &&
                comboBoxMaterial.SelectedItem != null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT dbo.fn_CalcularPrecoPersonalizacao2(@tamanho, @tecnica, @material)", conn))
                        {
                            cmd.Parameters.AddWithValue("@tamanho", comboBoxTamanho.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@tecnica", comboBoxTecnica.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@material", comboBoxMaterial.SelectedItem.ToString());

                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                precoAtual = Convert.ToDecimal(result);
                                labelPreco.Text = $"€{precoAtual:F2}";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao calcular preço: {ex.Message}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ButtonSelecionarImagem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                ofd.Title = "Selecionar Imagem de Referência";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Validar tamanho do arquivo (máx 5MB)
                        FileInfo fileInfo = new FileInfo(ofd.FileName);
                        if (fileInfo.Length > 5 * 1024 * 1024)
                        {
                            MessageBox.Show("A imagem deve ter no máximo 5MB.", "Arquivo muito grande",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        pictureBoxImagem.Image = Image.FromFile(ofd.FileName);
                        imagemPath = ofd.FileName;

                        // Criar cópia da imagem na pasta do projeto
                        string fileName = $"ref_{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(ofd.FileName)}";
                        string destPath = Path.Combine(Application.StartupPath, "imagens_referencia", fileName);

                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                        File.Copy(ofd.FileName, destPath, true);
                        imagemPath = fileName; // Guardar apenas o nome do arquivo
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao carregar imagem: {ex.Message}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ButtonConfirmar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario()) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_CriarEncomendaPersonalizada2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                        cmd.Parameters.AddWithValue("@id_categoria", ((ComboBoxItem)comboBoxCategoria.SelectedItem).Value);
                        cmd.Parameters.AddWithValue("@id_artista", ((ComboBoxItem)comboBoxArtista.SelectedItem).Value);
                        cmd.Parameters.AddWithValue("@tecnica", comboBoxTecnica.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@tamanho", comboBoxTamanho.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@material", comboBoxMaterial.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@descricao", textBoxDescricao.Text);
                        cmd.Parameters.AddWithValue("@data_entrega", dateTimePickerEntrega.Value);
                        cmd.Parameters.AddWithValue("@imagem_path", imagemPath);
                        cmd.Parameters.AddWithValue("@metodo_pagamento", "MB Way"); // Pode ser selecionável

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int idEncomenda = Convert.ToInt32(reader["id_encomenda"]);
                                decimal precoFinal = Convert.ToDecimal(reader["preco_final"]);

                                MessageBox.Show($"Encomenda personalizada criada com sucesso!\n\n" +
                                              $"Número da encomenda: {idEncomenda}\n" +
                                              $"Preço total: €{precoFinal:F2}\n" +
                                              $"Data de entrega: {dateTimePickerEntrega.Value:dd/MM/yyyy}\n\n" +
                                              "Receberá uma confirmação por email em breve.",
                                              "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar encomenda: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrEmpty(imagemPath))
            {
                MessageBox.Show("Por favor, selecione uma imagem de referência.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (comboBoxCategoria.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione uma categoria.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (comboBoxArtista.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione um artista.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (comboBoxTamanho.SelectedItem == null || comboBoxTecnica.SelectedItem == null || comboBoxMaterial.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todas as especificações (tamanho, técnica e material).", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxDescricao.Text))
            {
                MessageBox.Show("Por favor, forneça uma descrição do trabalho.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dateTimePickerEntrega.Value < DateTime.Now.AddDays(7))
            {
                MessageBox.Show("A data de entrega deve ser pelo menos 7 dias no futuro.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

    }

    // Classe auxiliar para ComboBox
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}