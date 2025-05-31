using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App{
    public partial class Form2 : Form
    {
        private int cursoId;
        private int clienteId; // ID do cliente logado
        private string connectionString = "Data Source=SOLOMIIA;Initial Catalog=Projeto;Integrated Security=True";
        private CursoInfo cursoAtual;
        private List<TurmaInfo> turmasDisponiveis;
        private decimal precoFinal = 0;

        public Form2(int cursoId, int clienteId)
        {
            InitializeComponent();
            this.cursoId = cursoId;
            this.clienteId = clienteId;
            ConfigurarInterface();
            CarregarDadosCurso();
            CarregarTurmasDisponiveis();
        }

        private void ConfigurarInterface()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Size = new Size(800, 600);
            this.Text = "Inscrição em Curso";
            CriarControlesInscricao();
        }

        private void CriarControlesInscricao()
        {
            lblDetalhesCurso1 = new Label
            {
                Text = "detalhes...",
                Font = new Font("Tahoma", 10),
                ForeColor = Color.Black,
                Location = new Point(10, 10),
                Size = new Size(780, 100),
                AutoSize = false
            };
            Inscricao_Pnl.Controls.Add(lblDetalhesCurso1);
            cmbTurma.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTurma.Size = new Size(275,24);
            Main_Pnl.Controls.Add(cmbTurma);
            cmbTurma.SelectedIndexChanged += CmbTurma_SelectedIndexChanged;

            // Info de capacidade
            lblCapacidadeInfo = new Label
            {
                Font = new Font("Tahoma",12),
                ForeColor = Color.DarkBlue,
                Size = new Size(500, 25)
            };
            panel1.Controls.Add(lblCapacidadeInfo);

            comboNivel.DropDownStyle = ComboBoxStyle.DropDownList;
            comboNivel.Items.AddRange(new string[] { "Todos", "Iniciante", "Intermediário", "Avançado" });
            comboNivel.SelectedIndex = 0;
            comboNivel.SelectedIndexChanged += ComboNivel_SelectedIndexChanged;

            cmbHorario.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHorario.Items.Add("Todos");
            cmbHorario.SelectedIndex = 0;
            cmbHorario.SelectedIndexChanged += CmbHorario_SelectedIndexChanged;

            cmbMetodoPagamento.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMetodoPagamento.Items.AddRange(new string[] { "MB Way", "Cartão de Crédito", "PayPal" });
            cmbMetodoPagamento.SelectedIndex = 0;

            lblPrecoFinal = new Label
            {
                Text = "Preço Final: €0,00",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleLeft,
            };
            // Botões
            pnlPrecoCurso.Controls.Add(lblPrecoFinal);
            btnConfirmarCurso.Click += BtnConfirmarCurso_Click;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
        }

        private void CarregarDadosCurso()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            c.id, c.nome, c.descricao, c.preco, c.data_inicio, c.data_fim, 
                            c.formato, c.capacidade, u.nome AS nome_artista
                        FROM Curso c
                        INNER JOIN Artista a ON c.id_artista = a.id
                        INNER JOIN Utilizador u ON a.id = u.id
                        WHERE c.id = @cursoId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@cursoId", cursoId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cursoAtual = new CursoInfo
                                {
                                    Id = (int)reader["id"],
                                    NomeCurso = reader["nome"].ToString(),
                                    Descricao = reader["descricao"] == DBNull.Value ? "" : reader["descricao"].ToString(),
                                    Preco = (decimal)reader["preco"],
                                    DataInicio = (DateTime)reader["data_inicio"],
                                    DataFim = (DateTime)reader["data_fim"],
                                    Formato = reader["formato"] == DBNull.Value ? "" : reader["formato"].ToString(),
                                    Capacidade = (int)reader["capacidade"],
                                    NomeArtista = reader["nome_artista"].ToString()
                                };

                                AtualizarDetalhesTrabalho();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do curso: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarTurmasDisponiveis()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("sp_ObterTurmasDisponiveis", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_curso", cursoId);

                        turmasDisponiveis = new List<TurmaInfo>();
                        HashSet<string> horarios = new HashSet<string>();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var turma = new TurmaInfo
                                {
                                    Id = (int)reader["id"],
                                    Nome = reader["nome"].ToString(),
                                    Nivel = reader["nivel"].ToString(),
                                    Horario = reader["horario"].ToString(),
                                    Capacidade = (int)reader["capacidade"],
                                    CapacidadeDisponivel = (int)reader["capacidade_disponivel"],
                                    StatusDisponibilidade = reader["status_disponibilidade"].ToString()
                                };
                                turmasDisponiveis.Add(turma);
                                horarios.Add(turma.Horario);
                            }
                        }

                        // Atualizar combo de horários
                        cmbHorario.Items.Clear();
                        cmbHorario.Items.Add("Todos");
                        foreach (string horario in horarios.OrderBy(h => h))
                        {
                            cmbHorario.Items.Add(horario);
                        }
                        cmbHorario.SelectedIndex = 0;

                        AtualizarComboTurmas();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar turmas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarComboTurmas()
        {
            string nivelSelecionado = comboNivel.SelectedItem?.ToString();
            string horarioSelecionado = cmbHorario.SelectedItem?.ToString();

            var turmasFiltradas = turmasDisponiveis.Where(t =>
                (nivelSelecionado == "Todos" || t.Nivel == nivelSelecionado) &&
                (horarioSelecionado == "Todos" || t.Horario == horarioSelecionado) &&
                t.CapacidadeDisponivel > 0
            ).ToList();

            cmbTurma.Items.Clear();
            foreach (var turma in turmasFiltradas)
            {
                string displayText = $"{turma.Nome} - {turma.Nivel} - {turma.Horario} ({turma.CapacidadeDisponivel} vagas)";
                cmbTurma.Items.Add(new ComboBoxItem { Text = displayText, Value = turma });
            }

            if (cmbTurma.Items.Count > 0)
            {
                cmbTurma.SelectedIndex = 0;
            }
            else
            {
                lblCapacidadeInfo.Text = "Nenhuma turma disponível com os filtros selecionados.";
                lblCapacidadeInfo.ForeColor = Color.Red;
            }
        }

        private void AtualizarDetalhesTrabalho()
        {
            if (cursoAtual != null)
            {
                string detalhes = $"Nome do Curso: {cursoAtual.NomeCurso}\n" +
                                $"Artista: {cursoAtual.NomeArtista}\n" +
                                $"Descrição: {cursoAtual.Descricao}\n" +
                                $"Data Início: {cursoAtual.DataInicio:dd/MM/yyyy}\n" +
                                $"Data Fim: {cursoAtual.DataFim:dd/MM/yyyy}\n" +
                                $"Formato: {cursoAtual.Formato}\n" +
                                $"Preço Base: €{cursoAtual.Preco:F2}";

                lblDetalhesCurso1.Text = detalhes;
            }
        }

        private void CalcularPrecoFinal()
        {
            if (cmbTurma.SelectedItem != null && cursoAtual != null)
            {
                var itemSelecionado = (ComboBoxItem)cmbTurma.SelectedItem;
                var turmaSelecionada = (TurmaInfo)itemSelecionado.Value;

                // Calcular preço baseado no nível
                decimal multiplicador = 1.0m;
                switch (turmaSelecionada.Nivel)
                {
                    case "Iniciante": multiplicador = 1.0m; break;
                    case "Intermediário": multiplicador = 1.2m; break;
                    case "Avançado": multiplicador = 1.5m; break;
                }

                precoFinal = cursoAtual.Preco * multiplicador;
                lblPrecoFinal.Text = $"Preço Final: €{precoFinal:F2}";

                // Atualizar info de capacidade
                lblCapacidadeInfo.Text = $"Vagas disponíveis: {turmaSelecionada.CapacidadeDisponivel}/{turmaSelecionada.Capacidade}";
                lblCapacidadeInfo.ForeColor = turmaSelecionada.CapacidadeDisponivel > 5 ? Color.Green : Color.Orange;
            }
        }

        private void CmbTurma_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcularPrecoFinal();
        }

        private void ComboNivel_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarComboTurmas();
        }

        private void CmbHorario_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarComboTurmas();
        }

        private void BtnConfirmarCurso_Click(object sender, EventArgs e)
        {
            if (ValidarInscricao())
            {
                RealizarInscricao();
            }
        }

        private bool ValidarInscricao()
        {
            if (cmbTurma.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione uma turma.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbMetodoPagamento.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione um método de pagamento.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var itemSelecionado = (ComboBoxItem)cmbTurma.SelectedItem;
            var turmaSelecionada = (TurmaInfo)itemSelecionado.Value;

            if (turmaSelecionada.CapacidadeDisponivel <= 0)
            {
                MessageBox.Show("A turma selecionada não possui vagas disponíveis.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void RealizarInscricao()
        {
            try
            {
                var itemSelecionado = (ComboBoxItem)cmbTurma.SelectedItem;
                var turmaSelecionada = (TurmaInfo)itemSelecionado.Value;
                string metodoPagamento = cmbMetodoPagamento.SelectedItem.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("sp_RealizarInscricao", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parâmetros de entrada
                        command.Parameters.AddWithValue("@id_cliente", clienteId);
                        command.Parameters.AddWithValue("@id_curso", cursoId);
                        command.Parameters.AddWithValue("@id_turma", turmaSelecionada.Id);
                        command.Parameters.AddWithValue("@metodo_pagamento", metodoPagamento);

                        // Parâmetros de saída
                        SqlParameter resultadoParam = new SqlParameter("@resultado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(resultadoParam);

                        SqlParameter mensagemParam = new SqlParameter("@mensagem", SqlDbType.VarChar, 255)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(mensagemParam);

                        command.ExecuteNonQuery();

                        int resultado = (int)resultadoParam.Value;
                        string mensagem = mensagemParam.Value?.ToString() ?? "";

                        if (resultado == 1)
                        {
                            MessageBox.Show(mensagem, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao realizar inscrição: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Classes auxiliares
        public class CursoInfo
        {
            public int Id { get; set; }
            public string NomeCurso { get; set; }
            public decimal Preco { get; set; }
            public string Descricao { get; set; }
            public DateTime DataInicio { get; set; }
            public DateTime DataFim { get; set; }
            public string Formato { get; set; }
            public int Capacidade { get; set; }
            public string NomeArtista { get; set; }
            public string NiveisTurmas { get; set; }
            public DateTime DataFavorito { get; set; }
            public int TotalFavoritos { get; set; }
        }

        public class TurmaInfo
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public string Nivel { get; set; }
            public string Horario { get; set; }
            public int Capacidade { get; set; }
            public int CapacidadeDisponivel { get; set; }
            public string StatusDisponibilidade { get; set; }
        }

        public class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        // Métodos de eventos vazios para compatibilidade
        private void lblTitulo_Click(object sender, EventArgs e) { }
        private void comboNivel_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cmbHorario_SelectedIndexChanged(object sender, EventArgs e) { }

        private void Main_Pnl_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbMetodoPagamento_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblPrecoFinal_Click(object sender, EventArgs e)
        {

        }

        private void pnlPrecoCurso_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCancelar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbHorario_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboNivel_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cmbTurma_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void lblTamanho_Click(object sender, EventArgs e)
        {

        }

        private void lblSecaoPersonalizacao_Click(object sender, EventArgs e)
        {

        }

        private void Inscricao_Pnl_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblDetalhesCurso_Click(object sender, EventArgs e)
        {

        }

        private void lblSecaoDetalhes_Click(object sender, EventArgs e)
        {

        }

        private void lblTitulo_Click_1(object sender, EventArgs e)
        {

        }

        private void btnConfirmarCurso_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblDetalhesCurso1_Click(object sender, EventArgs e)
        {

        }
    }

    // Classe para gerenciar inscrições (uso opcional)
    public static class GerenciadorInscricoes
    {
        private static string connectionString = "Data Source=SOLOMIIA;Initial Catalog=Projeto;Integrated Security=True";

        public static List<InscricaoInfo> ObterInscricoesCliente(int clienteId)
        {
            List<InscricaoInfo> inscricoes = new List<InscricaoInfo>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            i.id, i.data, i.status,
                            c.nome AS nome_curso,
                            u.nome AS nome_artista,
                            p.valor, p.metodo AS metodo_pagamento, p.status AS status_pagamento
                        FROM Inscricao i
                        INNER JOIN Curso c ON i.id_curso = c.id
                        INNER JOIN Artista a ON c.id_artista = a.id
                        INNER JOIN Utilizador u ON a.id = u.id
                        LEFT JOIN Pagamento p ON i.id_pagamento = p.id
                        WHERE i.id_cliente = @clienteId
                        ORDER BY i.data DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@clienteId", clienteId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inscricoes.Add(new InscricaoInfo
                                {
                                    Id = (int)reader["id"],
                                    Data = (DateTime)reader["data"],
                                    Status = reader["status"].ToString(),
                                    NomeCurso = reader["nome_curso"].ToString(),
                                    NomeArtista = reader["nome_artista"].ToString(),
                                    Valor = reader["valor"] == DBNull.Value ? 0 : (decimal)reader["valor"],
                                    MetodoPagamento = reader["metodo_pagamento"] == DBNull.Value ? "" : reader["metodo_pagamento"].ToString(),
                                    StatusPagamento = reader["status_pag"] == DBNull.Value ? "" : reader["status_pag"].ToString()
                                });

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter inscrições: {ex.Message}");
            }

            return inscricoes;
        }

        public static bool CancelarInscricao(int inscricaoId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Atualizar status da inscrição
                            string updateInscricao = "UPDATE Inscricao SET status = 'cancelada' WHERE id = @inscricaoId";
                            using (SqlCommand command = new SqlCommand(updateInscricao, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@inscricaoId", inscricaoId);
                                command.ExecuteNonQuery();
                            }

                            // Atualizar status do pagamento
                            string updatePagamento = @"
                                UPDATE Pagamento 
                                SET status = 'cancelado' 
                                WHERE id = (SELECT id_pagamento FROM Inscricao WHERE id = @inscricaoId)";
                            using (SqlCommand command = new SqlCommand(updatePagamento, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@inscricaoId", inscricaoId);
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public class InscricaoInfo
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Status { get; set; }
        public string NomeCurso { get; set; }
        public string NomeArtista { get; set; }
        public decimal Valor { get; set; }
        public string MetodoPagamento { get; set; }
        public string StatusPagamento { get; set; }
    }
}