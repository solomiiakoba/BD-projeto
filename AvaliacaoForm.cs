using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing;

namespace App
{
    public partial class AvaliacaoForm : Form
    {
        private int clienteId;
        private int artistaId;
        private string connectionString = "Data Source=SOLOMIIA;Initial Catalog=Projeto;Integrated Security=True";

        public AvaliacaoForm(int clienteId, int artistaId)
        {
            InitializeComponent();
            this.clienteId = clienteId;
            this.artistaId = artistaId;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Avaliar Artista";
            this.Size = new Size(350, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Nota (1-5)
            var lblNota = new Label
            {
                Text = "Nota (1-5):",
                Location = new Point(20, 20),
                AutoSize = true
            };

            var numNota = new NumericUpDown
            {
                Location = new Point(120, 20),
                Width = 50,
                Minimum = 1,
                Maximum = 5,
                DecimalPlaces = 1,
                Increment = 0.5m
            };

            // Comentário
            var lblComentario = new Label
            {
                Text = "Comentário:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            var txtComentario = new TextBox
            {
                Location = new Point(120, 60),
                Width = 200,
                MaxLength = 40 // Como definido na tabela
            };

            // Botões
            var btnConfirmar = new Button
            {
                Text = "Confirmar",
                Location = new Point(120, 120),
                Width = 100,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };

            btnConfirmar.Click += (s, e) => SalvarAvaliacao((double)numNota.Value, txtComentario.Text);

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(230, 120),
                Width = 100,
                BackColor = Color.Gray,
                ForeColor = Color.White
            };

            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblNota, numNota,
                lblComentario, txtComentario,
                btnConfirmar, btnCancelar
            });
        }

        private void SalvarAvaliacao(double nota, string comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario))
            {
                MessageBox.Show("Por favor, insira um comentário.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Obter o próximo ID disponível
                    int novoId = 1;
                    string queryMaxId = "SELECT ISNULL(MAX(id), 0) + 1 FROM Avaliacao";
                    using (SqlCommand cmd = new SqlCommand(queryMaxId, connection))
                    {
                        novoId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Inserir a avaliação
                    string query = @"INSERT INTO Avaliacao 
                                    (id, data, nota, comentario, id_cliente, id_artista) 
                                    VALUES 
                                    (@id, @data, @nota, @comentario, @id_cliente, @id_artista)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", novoId);
                        cmd.Parameters.AddWithValue("@data", DateTime.Today);
                        cmd.Parameters.AddWithValue("@nota", nota);
                        cmd.Parameters.AddWithValue("@comentario", comentario);
                        cmd.Parameters.AddWithValue("@id_cliente", clienteId);
                        cmd.Parameters.AddWithValue("@id_artista", artistaId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Avaliação registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar avaliação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}