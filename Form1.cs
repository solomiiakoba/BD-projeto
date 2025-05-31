using App.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace App
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=SOLOMIIA;Initial Catalog=Projeto;Integrated Security=True";
        private List<int> favoritosAnonimos = new List<int>();
        private int idUtilizadorAtual;
        public Form1(int userId = 0)
        {
            InitializeComponent();
            idUtilizadorAtual = userId;
            BtnClick(null, EventArgs.Empty);
            painel13.AutoScroll = false;
            LoadCategoriesFromDatabase();
            button2.Click += button2_Click;
            UpdateUIAfterLogin();
            CriarControlesPersonalizacao();
        }
        private void CriarControlesPersonalizacao()
        {
            button1.Click += (s, e) => AbrirFormularioPersonalizacao();
        }

        private void AbrirFormularioPersonalizacao()
        {
            if (idUtilizadorAtual == 0)
                {
                var result = MessageBox.Show("Você precisa estar logado para personalizar um trabalho. Deseja fazer login agora?",
                    "Login Necessário",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Register registro = new Register();
                    if (registro.ShowDialog() == DialogResult.OK)
                    {
                        idUtilizadorAtual = SessaoUtilizador.Id;
                        UpdateUIAfterLogin();
                        using (var form = new Personalizacao(idUtilizadorAtual))
                        {
                            form.ShowDialog();
                        }
                    }
                }
                }
            else
                {
                using (var form = new Personalizacao(idUtilizadorAtual))
                {
                    form.ShowDialog();
                }
            }
        }

        private void UpdateUIAfterLogin()
        {
            if (idUtilizadorAtual > 0)
            {
                userBtn.ButtonImage = Properties.Resources.user_solid; // Ícone de usuário logado
            }
            else
            {
                userBtn.ButtonImage = Properties.Resources.user_regular; // Ícone de usuário não logado
            }
        }
        private void LoadCategoriesFromDatabase()
        {
            try
            {
                List<CategoryInfo> categories = GetCategories();
                ShowCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Database Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<CategoryInfo> GetCategories()
        {
            List<CategoryInfo> categories = new List<CategoryInfo>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT c.id, c.nome_categoria, COUNT(t.id) as trabalhos_count
                               FROM Categoria c
                               LEFT JOIN Trabalho t ON c.id = t.id_categoria
                               GROUP BY c.id, c.nome_categoria
                               ORDER BY c.id";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new CategoryInfo
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                ItemCount = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB connection failed: " + ex.Message);
                }
            }
            return categories;
        }

        private void ShowCategories()
        {
            painel13.Controls.Clear();
            painel13.Padding = new Padding(20);
            painel13.Height = 200;
            painel13.Dock = DockStyle.None;
            FlowLayoutPanel flowCategories = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Dock = DockStyle.Bottom,
                Padding = new Padding(15),
                Margin = new Padding(0),
                AutoScroll = true,
                Height = 200,
                MaximumSize = new Size(0, 200)
            };

            List<CategoryInfo> categories = GetCategories();

            foreach (var category in categories)
            {
                CreateCategoryButton(category, flowCategories);
            }

            painel13.Controls.Add(flowCategories);
        }

        private void ShowArtists()
        {
            painel13.Controls.Clear();
            painel13.Padding = new Padding(20);
            painel13.Dock = DockStyle.Fill;
            Label artistsTitle = new Label
            {
                Text = "Artistas",
                Font = new Font("Tahoma", 16, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 30,
                ForeColor = Color.SteelBlue
            };
            painel13.Controls.Add(artistsTitle);
            FlowLayoutPanel flowArtists = new FlowLayoutPanel
            {
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Margin = new Padding(0),
                AutoSize = false
            };


            List<ArtistaInfo> artistas = GetArtistas();
            foreach (var artista in artistas)
            {
                CreateArtistaPanel(artista, flowArtists);
            }
            painel13.Controls.Add(flowArtists);
        }

        private List<ArtistaInfo> GetArtistas()
        {
            List<ArtistaInfo> artistas = new List<ArtistaInfo>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT a.id, u.nome, a.especialidade, a.bibliografia, a.disponibilidade, u.email, u.telefone
                               FROM Artista a
                               INNER JOIN Utilizador u ON a.id = u.id
                               ORDER BY u.nome";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            artistas.Add(new ArtistaInfo
                            {
                                Id = reader.GetInt32(0),
                                Nome = reader.GetString(1),
                                Especialidade = reader.GetString(2),
                                Bibliografia = reader.IsDBNull(3) ? "Não informado" : reader.GetString(3),
                                Disponibilidade = reader.IsDBNull(4) ? "Não informado" : reader.GetString(4),
                                Email = reader.GetString(5),
                                Telefone = reader.GetString(6)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar artistas: " + ex.Message);
                }
            }

            return artistas;
        }

        private void CreateArtistaPanel(ArtistaInfo artista, FlowLayoutPanel parentPanel)
        {
            Panel artistaPanel = new Panel
            {
                Width = 680,
                Height = 160,
                BackColor = Color.White,
                Margin = new Padding(0, 25, 0, 10),
                Tag = artista.Id,
                Cursor = Cursors.Hand
            };


            artistaPanel.Paint += (sender, e) =>
            {
                var panel = sender as Panel;
                var graphics = e.Graphics;
                var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var path = CreateRoundedRectangle(rect, 12);

                using (var brush = new LinearGradientBrush(
                    rect, Color.White, Color.FromArgb(240, 248, 255), 45))
                {
                    graphics.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.FromArgb(100, 149, 237), 2))
                {
                    graphics.DrawPath(pen, path);
                }
            };

            Label nomeLabel = new Label
            {
                Text = artista.Nome,
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 25, 112),
                TextAlign = ContentAlignment.MiddleLeft,
                Width = artistaPanel.Width - 20,
                Height = 25,
                Location = new Point(10, 10),
                BackColor = Color.Transparent
            };

            Label especialidadeLabel = new Label
            {
                Text = "Especialidade: " + artista.Especialidade,
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleLeft,
                Width = artistaPanel.Width - 20,
                Height = 20,
                Location = new Point(10, 38),
                BackColor = Color.Transparent
            };

            Label disponibilidadeLabel = new Label
            {
                Text = "Status: " + artista.Disponibilidade,
                Font = new Font("Tahoma", 8, FontStyle.Italic),
                ForeColor = artista.Disponibilidade.ToLower().Contains("disponível") ? Color.Green : Color.Firebrick,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = 150,
                Height = 20,
                Location = new Point(10, 105),
                BackColor = Color.Transparent
            };

            Label contatoLabel = new Label
            {
                Text = artista.Email,
                Font = new Font("Tahoma", 8),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleLeft,
                Width = artistaPanel.Width - 20,
                Height = 20,
                Location = new Point(10, 125),
                BackColor = Color.Transparent
            };
            Button verPortfolioBtn = new Button
            {
                Text = "Ver Portfólio",
                Font = new Font("Tahoma", 8, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.SteelBlue,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 30),
                Location = new Point(artistaPanel.Width - 130, 57),
                Cursor = Cursors.Hand,
                Tag = artista.Id
            };
            Button comBtn = new Button
            {
                Text = "Avaliar",
                Font = new Font("Tahoma", 8, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.SteelBlue,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 30),
                Location = new Point(artistaPanel.Width - 130, 95),
                Cursor = Cursors.Hand,
                Tag = artista.Id
            };
            comBtn.FlatAppearance.BorderSize = 0;
            comBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 149, 237);
            comBtn.Click += (s, e) =>
            {
                if (idUtilizadorAtual == 0)
                {
                    MessageBox.Show("Você precisa estar logado para avaliar um artista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int artistaId = (int)comBtn.Tag;

                using (var formAvaliacao = new AvaliacaoForm(idUtilizadorAtual, artistaId))
                {
                    if (formAvaliacao.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Obrigado pela sua avaliação!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };
            verPortfolioBtn.FlatAppearance.BorderSize = 0;
            verPortfolioBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 149, 237);
            verPortfolioBtn.Click += (s, e) =>
            {
                int artistaId = (int)((Button)s).Tag;
            };
            verPortfolioBtn.Click += ArtistaPanel_Click;
            artistaPanel.MouseEnter += (s, e) => { artistaPanel.BackColor = Color.FromArgb(245, 250, 255); };
            artistaPanel.MouseLeave += (s, e) => { artistaPanel.BackColor = Color.White; };

            artistaPanel.Controls.Add(nomeLabel);
            artistaPanel.Controls.Add(especialidadeLabel);
            artistaPanel.Controls.Add(disponibilidadeLabel);
            artistaPanel.Controls.Add(contatoLabel);
            parentPanel.Controls.Add(artistaPanel);
            artistaPanel.Controls.Add(verPortfolioBtn);
            artistaPanel.Controls.Add(comBtn);
        }

        private void ArtistaPanel_Click(object sender, EventArgs e)
        {
            Button clickedPanel = (Button)sender;
            int artistaId = (int)clickedPanel.Tag;
            ArtistaInfo artista = GetArtistaById(artistaId);
            string message = $"Artista: {artista.Nome}\n" +
                           $"Especialidade: {artista.Especialidade}\n" +
                           $"Email: {artista.Email}\n" +
                           $"Telefone: {artista.Telefone}\n" +
                           $"Status: {artista.Disponibilidade}\n\n" +
                           $"Bibliografia:\n{artista.Bibliografia}";

            MessageBox.Show(message, "Detalhes do Artista",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private ArtistaInfo GetArtistaById(int artistaId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT a.id, u.nome, a.especialidade, a.bibliografia, a.disponibilidade, u.email, u.telefone
                               FROM Artista a
                               INNER JOIN Utilizador u ON a.id = u.id
                               WHERE a.id = @ArtistaId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ArtistaId", artistaId);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ArtistaInfo
                            {
                                Id = reader.GetInt32(0),
                                Nome = reader.GetString(1),
                                Especialidade = reader.GetString(2),
                                Bibliografia = reader.IsDBNull(3) ? "Não informado" : reader.GetString(3),
                                Disponibilidade = reader.IsDBNull(4) ? "Não informado" : reader.GetString(4),
                                Email = reader.GetString(5),
                                Telefone = reader.GetString(6)
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar dados do artista: " + ex.Message);
                }
            }

            return new ArtistaInfo { Nome = "Artista não encontrado" };
        }

        private void ShowTrabalhos(int categoryId)
        {
            painel13.Controls.Clear();
            painel13.Padding = new Padding(20);
            painel13.Dock = DockStyle.Fill;

            Label categoryTitle = new Label
            {
                Text = GetCategoryName(categoryId),
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 30,
                ForeColor = Color.SteelBlue
            };
            painel13.Controls.Add(categoryTitle);

            FlowLayoutPanel flowTrabalhos = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = true,
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                Margin = new Padding(0),
                AutoScroll = true
            };

            List<TrabalhoInfo> trabalhos = GetTrabalhosByCategory(categoryId);

            foreach (var trabalho in trabalhos)
            {
                CreateTrabalhoPanel(trabalho, flowTrabalhos);
            }

            painel13.Controls.Add(flowTrabalhos);
        }

        private string GetCategoryName(int categoryId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT nome_categoria FROM Categoria WHERE id = @CategoryId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                try
                {
                    connection.Open();
                    return command.ExecuteScalar()?.ToString() ?? "Categoria Desconhecida";
                }
                catch
                {
                    return "Categoria Desconhecida";
                }
            }
        }

        private void CreateCategoryButton(CategoryInfo category, FlowLayoutPanel parentPanel)
        {
            Panel categoryPanel = new Panel
            {
                Width = 130,
                Height = 130,
                BackColor = Color.Red,
                Margin = new Padding(15),
                Tag = category.Id,
                Cursor = Cursors.Hand
            };

            categoryPanel.Paint += (sender, e) =>
            {
                var panel = sender as Panel;
                var graphics = e.Graphics;
                var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var path = CreateRoundedRectangle(rect, 15);

                using (var brush = new LinearGradientBrush(
                    rect, Color.White, Color.FromArgb(255, 235, 235), 45))
                {
                    graphics.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.FromArgb(255, 180, 180), 2))
                {
                    graphics.DrawPath(pen, path);
                }
            };

            Label nameLabel = new Label
            {
                Text = category.Name,
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(170, 50, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = categoryPanel.Width,
                Height = 50,
                Location = new Point(0, 40),
                BackColor = Color.Transparent
            };

            Label countLabel = new Label
            {
                Text = $"{category.ItemCount} trabalhos",
                Font = new Font("Tahoma", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = categoryPanel.Width,
                Height = 30,
                Location = new Point(0, 90),
                BackColor = Color.Transparent
            };

            categoryPanel.Click += CategoryPanel_Click;
            categoryPanel.MouseEnter += (s, e) => { categoryPanel.BackColor = Color.FromArgb(245, 245, 245); };
            categoryPanel.MouseLeave += (s, e) => { categoryPanel.BackColor = Color.White; };
            categoryPanel.Controls.Add(nameLabel);
            categoryPanel.Controls.Add(countLabel);
            parentPanel.Controls.Add(categoryPanel);
        }

        private void CreateTrabalhoPanel(TrabalhoInfo trabalho, FlowLayoutPanel parentPanel)
        {
            Panel trabalhoPanel = new Panel
            {
                Width = 200,
                Height = 180,
                BackColor = Color.Red,
                Margin = new Padding(15),
                Tag = trabalho.Id,
                Cursor = Cursors.Hand
            };

            trabalhoPanel.Paint += (sender, e) =>
            {
                var panel = sender as Panel;
                var graphics = e.Graphics;
                var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var path = CreateRoundedRectangle(rect, 10);

                using (var brush = new LinearGradientBrush(
                    rect, Color.White, Color.FromArgb(230, 240, 255), 45))
                {
                    graphics.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.SteelBlue, 2))
                {
                    graphics.DrawPath(pen, path);
                }
            };

            Label titleLabel = new Label
            {
                Text = trabalho.Titulo,
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(134, 37, 37),
                TextAlign = ContentAlignment.MiddleLeft,
                Width = trabalhoPanel.Width - 20,
                Height = 30,
                Location = new Point(10, 10), 
                BackColor = Color.Transparent
            };

            Label descLabel = new Label
            {
                Text = trabalho.Descricao,
                Font = new Font("Tahoma", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.TopLeft,
                Width = trabalhoPanel.Width - 20,
                Height = 20, 
                Location = new Point(10, 40), 
                BackColor = Color.Transparent
            };

            Label disponLabel = new Label
            {
                Text = trabalho.Disponibilidade,
                Font = new Font("Tahoma", 8),
                ForeColor = (trabalho.Disponibilidade == "à venda") ? Color.Green : Color.Red,
                Width = trabalhoPanel.Width - 20,
                Height = 20,
                Location = new Point(10, 80), // Ajuste da posição Y
                BackColor = Color.Transparent
            };

            Label precoLabel = new Label
            {
                Text = trabalho.Preco.ToString("C"),
                Font = new Font("Tahoma", 9, FontStyle.Italic),
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = trabalhoPanel.Width - 20,
                Height = 20,
                Location = new Point(10, 110),
                BackColor = Color.Transparent
            };

            // Botão Comprar
            Button comprarBtn = new Button
            {
                Text = "Comprar",
                Font = new Font("Tahoma", 8, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.SteelBlue,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(85, 25),
                Location = new Point(10, 140),
                Tag = trabalho.Id,
                Margin = new Padding(0, 0, 10, 0),
                Cursor = Cursors.Hand
            };
            comprarBtn.FlatAppearance.BorderSize = 0;
            comprarBtn.Click += (s, e) => {
                MessageBox.Show($"Comprar trabalho ID: {trabalho.Id}");
                // Implemente a lógica de compra aqui
            };

            // Botão Personalizar
            Button personalizarBtn = new Button
            {
                Text = "Personalizar",
                Font = new Font("Tahoma", 8, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.SteelBlue,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(85, 25),
                Location = new Point(100, 140), 
                Tag = trabalho.Id,
                Cursor = Cursors.Hand
            };
            personalizarBtn.FlatAppearance.BorderSize = 0;
            personalizarBtn.Click += (s, e) =>
            {
                if (trabalho.Disponibilidade != "à venda" && trabalho.Disponibilidade != "exposição")
                {
                    MessageBox.Show("Este trabalho não está disponível para personalização.",
                                   "Indisponível", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (idUtilizadorAtual == 0)
                {
                    var result = MessageBox.Show("Você precisa estar logado para personalizar trabalhos. Deseja fazer login agora?",
                                                 "Login Necessário",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        Register registro = new Register();
                        if (registro.ShowDialog() == DialogResult.OK)
                        {
                            idUtilizadorAtual = SessaoUtilizador.Id;
                            UpdateUIAfterLogin();
                        }
                        else
                        {
                            return; 
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                using (var form = new Personalizacao2(trabalho.Id))
                {
                    form.ShowDialog();
                }
            };
            trabalhoPanel.Controls.Add(titleLabel);
            trabalhoPanel.Controls.Add(descLabel);
            trabalhoPanel.Controls.Add(disponLabel);
            trabalhoPanel.Controls.Add(precoLabel);
            trabalhoPanel.Controls.Add(comprarBtn);
            trabalhoPanel.Controls.Add(personalizarBtn);
            trabalhoPanel.Click += TrabalhoPanel_Click;
            trabalhoPanel.MouseEnter += (s, e) => { trabalhoPanel.BackColor = Color.FromArgb(240, 245, 255); };
            trabalhoPanel.MouseLeave += (s, e) => { trabalhoPanel.BackColor = Color.White; };

            parentPanel.Controls.Add(trabalhoPanel);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);
            GraphicsPath path = new GraphicsPath();

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private List<TrabalhoInfo> GetTrabalhosByCategory(int categoryId)
        {
            List<TrabalhoInfo> trabalhos = new List<TrabalhoInfo>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, titulo, descricao, disponibilidade, preco FROM Trabalho WHERE id_categoria = @CategoryId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            trabalhos.Add(new TrabalhoInfo
                            {
                                Id = reader.GetInt32(0),
                                Titulo = reader.IsDBNull(1) ? "Sem título" : reader.GetString(1),
                                Descricao = reader.IsDBNull(2) ? "Sem descrição" : reader.GetString(2),
                                Disponibilidade = reader.IsDBNull(3) ? "à venda" : reader.GetString(3),
                                Preco = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar trabalhos: " + ex.Message);
                }
            }
            return trabalhos;
        }

        private void CategoryPanel_Click(object sender, EventArgs e)
        {
            Panel clickedPanel = (Panel)sender;
            int categoryId = (int)clickedPanel.Tag;
            ShowTrabalhos(categoryId);
        }

        private void TrabalhoPanel_Click(object sender, EventArgs e)
        {
            Panel clickedPanel = (Panel)sender;
            int trabalhoId = (int)clickedPanel.Tag;
            MessageBox.Show($"Trabalho selecionado: ID {trabalhoId}", "Trabalho Selecionado",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CreateCursoPanel(CursoInfo curso, FlowLayoutPanel parentPanel)
        {
            Panel cursoPanel = new Panel
            {
                Width = 680,
                Height = 180,
                BackColor = Color.White,
                Margin = new Padding(0, 20, 0, 10),
                Tag = curso.Id,
                Cursor = Cursors.Hand
            };

            cursoPanel.Paint += (sender, e) =>
            {
                var panel = sender as Panel;
                var graphics = e.Graphics;
                var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var path = CreateRoundedRectangle(rect, 12);

                using (var brush = new LinearGradientBrush(
                    rect, Color.White, Color.FromArgb(240, 248, 255), 45))
                {
                    graphics.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.FromArgb(100, 149, 237), 2))
                {
                    graphics.DrawPath(pen, path);
                }
            };

            Label nomeCursoLabel = new Label
            {
                Text = curso.NomeCurso,
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 25, 112),
                Location = new Point(10, 10),
                Size = new Size(500, 25),
                BackColor = Color.Transparent
            };

            Label artistaLabel = new Label
            {
                Text = "Organizado por: " + curso.NomeArtista,
                Font = new Font("Tahoma", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(10, 35),
                Size = new Size(300, 20),
                BackColor = Color.Transparent
            };

            Label descricaoLabel = new Label
            {
                Text = curso.Descricao,
                Font = new Font("Tahoma", 8),
                ForeColor = Color.Black,
                Location = new Point(10, 58),
                Size = new Size(500, 20),
                BackColor = Color.Transparent
            };

            Label detalhesLabel = new Label
            {
                Text = $"Formato: {curso.Formato}  |  Início: {curso.DataInicio:dd/MM/yyyy}  |  Fim: {curso.DataFim:dd/MM/yyyy}",
                Font = new Font("Tahoma", 8),
                ForeColor = Color.Gray,
                Location = new Point(10, 78),
                Size = new Size(500, 20),
                BackColor = Color.Transparent
            };

            Label capacidadePrecoLabel = new Label
            {
                Text = $"Capacidade: {curso.Capacidade}  |  Preço: {curso.Preco:C}",
                Font = new Font("Tahoma", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 90, 90),
                Location = new Point(10, 98),
                Size = new Size(300, 20),
                BackColor = Color.Transparent
            };

            Label niveisLabel = new Label
            {
                Text = "Níveis: " + curso.NiveisTurmas,
                Font = new Font("Tahoma", 8, FontStyle.Italic),
                ForeColor = Color.DarkSlateGray,
                Location = new Point(10, 118),
                Size = new Size(400, 20),
                BackColor = Color.Transparent
            };

            Component.Button detalhesBtn = new Component.Button
            {
                ButtonText = "Ver detalhes",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Size = new Size(110, 40),
                Location = new Point(cursoPanel.Width - 130, 130),
                GradientTopColor = Color.SteelBlue,
                GradientBottomColor = Color.RoyalBlue,
                BorderRadius = 15,
                TextColor = Color.White,
                Tag = curso.Id,
                Cursor = Cursors.Hand
            };

            Component.Button inscrBtn = new Component.Button
            {
                ButtonText = "Inscrever-me",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Size = new Size(110, 40),
                Location = new Point(cursoPanel.Width - 130, 80),
                GradientTopColor = Color.SteelBlue,
                GradientBottomColor = Color.RoyalBlue,
                BorderRadius = 15,
                TextColor = Color.White,
                Tag = curso.Id,
                Cursor = Cursors.Hand
            };
            inscrBtn.Click += (s, e) =>
            {
                if (idUtilizadorAtual == 0)
                {
                    var result = MessageBox.Show("Você precisa estar logado para adicionar cursos aos favoritos. Deseja fazer login agora?",
                                       "Login Necessário",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        Register registro = new Register();
                        if (registro.ShowDialog() == DialogResult.OK)
                        {
                            // Atualiza o ID do usuário após login bem-sucedido
                            idUtilizadorAtual = SessaoUtilizador.Id;
                            UpdateUIAfterLogin();
                        }
                        using (var form = new Form2(curso.Id, idUtilizadorAtual))
                        {
                            form.ShowDialog();
                        }
                    }
                }
            };

            Component.Button favBtn = new Component.Button
            {
                ButtonText = "",
                ButtonImage = Properties.Resources.heart_regular__1_,
                Size = new Size(40, 40),
                Location = new Point(cursoPanel.Width - 130, 30),
                BackgroundColor = Color.Transparent,
                GradientBottomColor = Color.Transparent,
                GradientTopColor = Color.Transparent,
                BorderRadius = 20,
                Tag = curso.Id,
                Cursor = Cursors.Hand
            };
            // ver a cor depois
            bool isFavorito = (idUtilizadorAtual == 0) ?
                favoritosAnonimos.Contains(curso.Id) :
                IsCursoFavorito(curso.Id);

            if (isFavorito)
            {
                favBtn.ButtonImage = Properties.Resources.heart_solid__2_;
            }
            else
            {
                favBtn.ButtonImage = Properties.Resources.heart_regular__1_;
            }

            // Eventos
            detalhesBtn.Click += (s, e) =>
            {
                int cursoId = (int)((Component.Button)s).Tag;
                MessageBox.Show($"Curso: {curso.NomeCurso}\nOrganizado por: {curso.NomeArtista}\nDescrição: {curso.Descricao}\n\nData: {curso.DataInicio:dd/MM/yyyy} a {curso.DataFim:dd/MM/yyyy}\nFormato: {curso.Formato}\nPreço: {curso.Preco:C}\nCapacidade: {curso.Capacidade}\nNíveis: {curso.NiveisTurmas}",
                    "Detalhes do Curso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            favBtn.Click += (s, e) =>
            {
                int cursoId = (int)((Component.Button)s).Tag;
                if (idUtilizadorAtual == 0)
                {
                    var result = MessageBox.Show("Você precisa estar logado para adicionar cursos aos favoritos. Deseja fazer login agora?",
                                       "Login Necessário",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        Register registro = new Register();
                        if (registro.ShowDialog() == DialogResult.OK)
                        {
                            // Atualiza o ID do usuário após login bem-sucedido
                            idUtilizadorAtual = SessaoUtilizador.Id;
                            UpdateUIAfterLogin();
                        }
                    }
                }
                else
                {
                    ToggleFavorito(cursoId);
                }
            };
            cursoPanel.MouseEnter += (s, e) => { cursoPanel.BackColor = Color.FromArgb(245, 250, 255); };
            cursoPanel.MouseLeave += (s, e) => { cursoPanel.BackColor = Color.White; };

            cursoPanel.Controls.Add(nomeCursoLabel);
            cursoPanel.Controls.Add(artistaLabel);
            cursoPanel.Controls.Add(descricaoLabel);
            cursoPanel.Controls.Add(detalhesLabel);
            cursoPanel.Controls.Add(capacidadePrecoLabel);
            cursoPanel.Controls.Add(niveisLabel);
            cursoPanel.Controls.Add(detalhesBtn);
            cursoPanel.Controls.Add(inscrBtn);
            cursoPanel.Controls.Add(favBtn);
            parentPanel.Controls.Add(cursoPanel);
        }

        private List<CursoInfo> GetCursosComNiveis()
        {
            List<CursoInfo> cursos = new List<CursoInfo>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                c.id AS CursoId,
                c.nome AS NomeCurso,
                c.preco,
                c.descricao,
                c.data_inicio,
                c.data_fim,
                c.formato,
                c.capacidade,
                u.nome AS NomeArtista,
                t.nivel AS NivelTurma
            FROM Curso c
            JOIN Artista a ON c.id_artista = a.id
            JOIN Utilizador u ON a.id = u.id
            LEFT JOIN Turma t ON t.id_curso = c.id
            ORDER BY c.data_inicio, c.id, t.nivel;
            ";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cursos.Add(new CursoInfo
                            {
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                NomeCurso = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Preco = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                                Descricao = reader.IsDBNull(3) ? "Sem descrição" : reader.GetString(3),
                                DataInicio = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                                DataFim = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                                Formato = reader.IsDBNull(6) ? "Formato não informado" : reader.GetString(6),
                                Capacidade = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                NomeArtista = reader.IsDBNull(8) ? "Desconhecido" : reader.GetString(8),
                                NiveisTurmas = reader.IsDBNull(9) ? "Não definidos" : reader.GetString(9)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar cursos: " + ex.Message);
                }
            }
            return cursos;
        }
        private void ShowCursos()
        {
            painel13.Controls.Clear();
            painel13.Padding = new Padding(20);
            painel13.Dock = DockStyle.Fill;
            Label cursosTitle = new Label
            {
                Text = "Cursos",
                Font = new Font("Tahoma", 16, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 30,
                ForeColor = Color.SteelBlue
            };
            painel13.Controls.Add(cursosTitle);
            FlowLayoutPanel flowCursos = new FlowLayoutPanel
            {
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Margin = new Padding(0),
                AutoSize = false
            };

            List<CursoInfo> cursos = GetCursosComNiveis();
            foreach (var curso in cursos)
            {
                CreateCursoPanel(curso, flowCursos);
            }
            painel13.Controls.Add(flowCursos);
        }

        private bool IsCursoFavorito(int cursoId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT dbo.fn_IsFavorito(@IdUtilizador, @IdCurso)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IdUtilizador", idUtilizadorAtual);
                    command.Parameters.AddWithValue("@IdCurso", cursoId);

                    object result = command.ExecuteScalar();
                    return Convert.ToBoolean(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar favorito: " + ex.Message);
                    return false;
                }
            }
        }
        private void ToggleFavorito(int cursoId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("sp_ToggleFavorito", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdUtilizador", idUtilizadorAtual);
                    command.Parameters.AddWithValue("@IdCurso", cursoId);

                    SqlParameter resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, 50);
                    resultadoParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(resultadoParam);

                    SqlParameter isAdicionadoParam = new SqlParameter("@IsAdicionado", SqlDbType.Bit);
                    isAdicionadoParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(isAdicionadoParam);

                    command.ExecuteNonQuery();

                    string resultado = resultadoParam.Value.ToString();
                    bool isAdicionado = Convert.ToBoolean(isAdicionadoParam.Value);
                    if (idUtilizadorAtual == 0)
                    {
                        if (isAdicionado)
                            favoritosAnonimos.Add(cursoId);
                        else
                            favoritosAnonimos.Remove(cursoId);
                    }

                    MessageBox.Show(resultado, "Favoritos", MessageBoxButtons.OK,
                                  isAdicionado ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                    RefreshCurrentView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao atualizar favoritos: " + ex.Message);
                }
            }
        }

        private List<CursoInfo> GetFavoritosByUser()
        {
            List<CursoInfo> favoritos = new List<CursoInfo>();

            if (idUtilizadorAtual == 0)
            {
                return GetCursosByIds(favoritosAnonimos);
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("sp_GetFavoritosByUser", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdUtilizador", idUtilizadorAtual);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            favoritos.Add(new CursoInfo
                            {
                                Id = Convert.ToInt32(reader["CursoId"]),
                                NomeCurso = reader["NomeCurso"].ToString(),
                                Descricao = reader["descricao"].ToString(),
                                Preco = Convert.ToDecimal(reader["preco"]),
                                DataInicio = Convert.ToDateTime(reader["data_inicio"]),
                                DataFim = Convert.ToDateTime(reader["data_fim"]),
                                Formato = reader["formato"].ToString(),
                                Capacidade = Convert.ToInt32(reader["capacidade"]),
                                NomeArtista = reader["NomeArtista"].ToString(),
                                DataFavorito = Convert.ToDateTime(reader["data_favorito"]),
                                TotalFavoritos = Convert.ToInt32(reader["TotalFavoritos"])

                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar favoritos: " + ex.Message);
                }
            }

            return favoritos;
        }

        private List<CursoInfo> GetCursosByIds(List<int> cursoIds)
        {
            List<CursoInfo> cursos = new List<CursoInfo>();
            if (cursoIds.Count == 0) return cursos;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string ids = string.Join(",", cursoIds);
                    string query = $@"
                SELECT 
                    c.id AS CursoId,
                    c.nome AS NomeCurso,
                    c.descricao,
                    c.preco,
                    c.data_inicio,
                    c.data_fim,
                    c.formato,
                    c.capacidade,
                    u.nome AS NomeArtista
                FROM Curso c
                INNER JOIN Artista a ON c.id_artista = a.id
                INNER JOIN Utilizador u ON a.id = u.id
                WHERE c.id IN ({ids})";

                    SqlCommand command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cursos.Add(new CursoInfo
                            {
                                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                NomeCurso = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Preco = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                                Descricao = reader.IsDBNull(3) ? "Sem descrição" : reader.GetString(3),
                                DataInicio = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                                DataFim = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                                Formato = reader.IsDBNull(6) ? "Formato não informado" : reader.GetString(6),
                                Capacidade = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                NomeArtista = reader.IsDBNull(8) ? "Desconhecido" : reader.GetString(8),
                                NiveisTurmas = reader.IsDBNull(9) ? "Não definidos" : reader.GetString(9)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar dados dos cursos: " + ex.Message);
                }
            }

            return cursos;
        }
        private void ShowFavoritos()
        {
            painel13.Controls.Clear();
            painel13.Padding = new Padding(20);
            painel13.Dock = DockStyle.Fill;

            Label favoritosTitle = new Label
            {
                Text = "Meus Favoritos",
                Font = new Font("Tahoma", 16, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 30,
                ForeColor = Color.SteelBlue
            };
            painel13.Controls.Add(favoritosTitle);

            FlowLayoutPanel flowFavoritos = new FlowLayoutPanel
            {
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Margin = new Padding(0),
                AutoSize = false

            };

            List<CursoInfo> favoritos = GetFavoritosByUser();

            if (favoritos.Count == 0)
            {
                Label noFavoritosLabel = new Label
                {
                    Text = "Você ainda não tem cursos favoritos.\nNavigue pelos cursos e clique no ❤️ para adicionar aos favoritos!",
                    Font = new Font("Tahoma", 12),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(0, 30, 0, 0)
                };
                flowFavoritos.Controls.Add(noFavoritosLabel);
            }
            else
            {
                foreach (var curso in favoritos)
                {
                    CreateCursoPanel(curso, flowFavoritos);
                }
            }
            painel13.Controls.Add(flowFavoritos);
        }
        private void RefreshCurrentView()
        {
            foreach (var btn in MenuPanel.Controls.OfType<Component.Button>())
            {
                if (btn.BackColor == Color.MistyRose) // Botão ativo
                {
                    switch (btn.Name)
                    {
                        case "Homebtn":
                            ShowCategories();
                            break;
                        case "ArtistsButton":
                            ShowArtists();
                            break;
                        case "CursesBtn":
                            ShowCursos();
                            break;
                        case "FavouritesBtn":
                            ShowFavoritos();
                            break;
                    }
                    break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowCategories();
        }

        private void BtnClick(object sender, EventArgs e)
        {
            string buttonName;
            if (sender == null)
            {
                buttonName = "Homebtn";
            }
            else
            {
                Component.Button clickedButton = (Component.Button)sender;
                buttonName = clickedButton.Name;
            }

            foreach (var btn in MenuPanel.Controls.OfType<Component.Button>())
            {
                btn.BackColor = btn.Name == buttonName ? Color.MistyRose : Color.Transparent;
                switch (btn.Name)
                {
                    case "Homebtn":
                        btn.ButtonImage = Properties.Resources.house_solid;
                        if (buttonName == "Homebtn") ShowCategories();
                        break;
                    case "ArtistsButton":
                        btn.ButtonImage = Properties.Resources.paintbrush_solid;
                        if (buttonName == "ArtistsButton") ShowArtists();
                        break;
                    case "CursesBtn":
                        btn.ButtonImage = Properties.Resources.graduation_cap_solid;
                        if (buttonName == "CursesBtn") ShowCursos();
                        break;
                    case "FavouritesBtn":
                        btn.ButtonImage = Properties.Resources.heart_solid__2_;
                        if (buttonName == "FavouritesBtn") ShowFavoritos();
                        break;
                }
            }
        }
        private void painel13_Paint(object sender, PaintEventArgs e) { }
        private void button2_Paint(object sender, PaintEventArgs e) { }
        private void button1_Paint_1(object sender, PaintEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void painel11_Paint(object sender, PaintEventArgs e) { }
        private void ArtistsButton_Paint(object sender, PaintEventArgs e) { }
        private void CursesBtn_Paint(object sender, PaintEventArgs e) { }
        private void FavouritesBtn_Paint(object sender, PaintEventArgs e) { }
        private void button1_Paint(object sender, PaintEventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void button2_Paint_1(object sender, PaintEventArgs e) { }
        private void button2_Paint_2(object sender, PaintEventArgs e) { }
        private void button3_Paint(object sender, PaintEventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e) { }

        private void userBtn_Paint(object sender, PaintEventArgs e)
        {
        }
        private void userBtn_Click(object sender, EventArgs e)
        {
            if (idUtilizadorAtual == 0)
            {
                Register registro = new Register();
                var result = registro.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Atualiza o ID do usuário após login bem-sucedido
                    idUtilizadorAtual = SessaoUtilizador.Id;
                    UpdateUIAfterLogin();
                    RefreshCurrentView();

                    // Mostra mensagem de boas-vindas
                    MessageBox.Show($"Bem-vindo de volta, {SessaoUtilizador.Nome}!", "Login",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                var result = MessageBox.Show("Deseja sair da sua conta?", "Logout",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Limpa a sessão e volta ao modo anônimo
                    SessaoUtilizador.LimparSessao();
                    idUtilizadorAtual = 0;
                    UpdateUIAfterLogin();
                    RefreshCurrentView();

                    MessageBox.Show("Você saiu da sua conta.", "Logout",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                }
            }
        }
    }

    public class CategoryInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ItemCount { get; set; }
    }

    public class TrabalhoInfo
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public string Disponibilidade { get; set; }
    }

    public class ArtistaInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Especialidade { get; set; }
        public string Bibliografia { get; set; }
        public string Disponibilidade { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
    }

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

}