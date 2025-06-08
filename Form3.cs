using App.Properties;
using Component;
using Microsoft.Win32;
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
    public partial class Form3 : Form
    {
        private string connectionString = "Data Source=tcp:mednat.ieeta.pt\\SQLSERVER,8101;Initial Catalog=p11g6;User Id=p11g6;Password=-1680827155@BD";
        private List<int> favoritosAnonimos = new List<int>();
        private int idUtilizadorAtual;
        public Form3(int userId = 0)
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
                userBtn.ButtonImage = Properties.Resources.user_solid;
            }
            else
            {
                userBtn.ButtonImage = Properties.Resources.user_regular;
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
                               FROM projeto.Categoria c
                               LEFT JOIN projeto.Trabalho t ON c.id = t.id_categoria
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


        private void ShowProjetos()
        {
            painel13.Controls.Clear();
            Label label = new Label
            {
                Text = "Conteúdo dos Projetos",
                Font = new Font("Tahoma", 14),
                Dock = DockStyle.Top,
                ForeColor = Color.Black
            };
            painel13.Controls.Add(label);
        }

        private void ShowEstatisticas()
        {
            painel13.Controls.Clear();
            Label label = new Label
            {
                Text = "Estatísticas do sistema",
                Font = new Font("Tahoma", 14),
                Dock = DockStyle.Top,
                ForeColor = Color.Black
            };
            painel13.Controls.Add(label);
        }

        private void ShowDefinicoes()
        {
            painel13.Controls.Clear();
            Label label = new Label
            {
                Text = "Definições do utilizador",
                Font = new Font("Tahoma", 14),
                Dock = DockStyle.Top,
                ForeColor = Color.Black
            };
            painel13.Controls.Add(label);
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
                               FROM projeto.Artista a
                               INNER JOIN projeto.Utilizador u ON a.id = u.id
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
            System.Windows.Forms.Button verPortfolioBtn = new System.Windows.Forms.Button
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
            System.Windows.Forms.Button comBtn = new System.Windows.Forms.Button
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
                int artistaId = (int)((System.Windows.Forms.Button)s).Tag;
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
            System.Windows.Forms.Button clickedPanel = (System.Windows.Forms.Button)sender;
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
                               FROM projeto.Artista a
                               INNER JOIN projeto.Utilizador u ON a.id = u.id
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
                string query = "SELECT nome_categoria FROM projeto.Categoria WHERE id = @CategoryId";
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

            System.Windows.Forms.Button comprarBtn = new System.Windows.Forms.Button
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
            };

            System.Windows.Forms.Button personalizarBtn = new System.Windows.Forms.Button
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
                string query = "SELECT id, titulo, descricao, disponibilidade, preco FROM projeto.Trabalho WHERE id_categoria = @CategoryId";
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
            FROM projeto.Curso c
            JOIN projeto.Artista a ON c.id_artista = a.id
            JOIN projeto.Utilizador u ON a.id = u.id
            LEFT JOIN projeto.Turma t ON t.id_curso = c.id
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

        private void ShowPainelGestaoArtista()
        {
            painel13.Controls.Clear();
            painel13.Dock = DockStyle.Fill;
            painel13.Padding = new Padding(20);


            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(10),
                Margin = new Padding(10),
            };

            System.Windows.Forms.Button btnPortfolio = new System.Windows.Forms.Button
            {
                Text = "Gerir Portfólios",
                Size = new Size(150, 40),
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            System.Windows.Forms.Button btnCursos = new System.Windows.Forms.Button
            {
                Text = "Gerir Cursos",
                Size = new Size(150, 40),
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                BackColor = Color.Teal,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            System.Windows.Forms.Button btnTurmas = new System.Windows.Forms.Button
            {
                Text = "Gerir Turmas",
                Size = new Size(150, 40),
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                BackColor = Color.Indigo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnPortfolio.Click += (s, e) => ShowPortfoliosDoArtista();
            btnCursos.Click += (s, e) => ShowCursosDoArtista();
            btnTurmas.Click += (s, e) => ShowTurmasDoArtista();

            flow.Controls.Add(btnPortfolio);
            flow.Controls.Add(btnCursos);
            flow.Controls.Add(btnTurmas);
            painel13.Controls.Add(flow);
        }

        private void ShowPortfoliosDoArtista()
        {
            if (idUtilizadorAtual == 0)
            {
                MessageBox.Show("É necessário iniciar sessão para ver os seus portfólios.");
                return;
            }

            painel13.Controls.Clear();
            painel13.Dock = DockStyle.Fill;
            painel13.Padding = new Padding(20);

            FlowLayoutPanel lista = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Padding = new Padding(10)
            };

            List<PortfolioInfo> portfolios = GetPortfoliosDoArtista(idUtilizadorAtual);

            // Botão Novo Portfólio
            System.Windows.Forms.Button btnNovoPortfolio = new System.Windows.Forms.Button
            {
                Text = "➕ Novo Portfólio",
                Size = new Size(150, 40),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 15)
            };

            btnNovoPortfolio.Click += (s, e) =>
            {
                MostrarFormularioPortfolio();
            };

            painel13.Controls.Add(btnNovoPortfolio);

            foreach (var portfolio in portfolios)
            {
                Panel portfolioPanel = new Panel
                {
                    Width = 700,
                    Height = 100,
                    BackColor = Color.White,
                    Margin = new Padding(0, 0, 0, 10),
                    Padding = new Padding(10),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label nomeLabel = new Label
                {
                    Text = $"#{portfolio.Id} - {portfolio.Nome}",
                    Font = new Font("Tahoma", 11, FontStyle.Bold),
                    ForeColor = Color.FromArgb(25, 25, 112),
                    Location = new Point(10, 10),
                    AutoSize = true
                };

                // Botão Editar
                System.Windows.Forms.Button btnEditar = new System.Windows.Forms.Button
                {
                    Text = "Editar",
                    Size = new Size(80, 30),
                    Location = new Point(portfolioPanel.Width - 190, 50),
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Tahoma", 8, FontStyle.Bold),
                    Tag = portfolio
                };

                btnEditar.Click += (s, e) =>
                {
                    PortfolioInfo p = (PortfolioInfo)((System.Windows.Forms.Button)s).Tag;
                    MostrarFormularioPortfolio(p);
                };

                // Botão Eliminar
                System.Windows.Forms.Button btnApagar = new System.Windows.Forms.Button
                {
                    Text = "Eliminar",
                    Size = new Size(80, 30),
                    Location = new Point(portfolioPanel.Width - 100, 50),
                    BackColor = Color.Firebrick,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Tahoma", 8, FontStyle.Bold),
                    Tag = portfolio.Id
                };

                btnApagar.Click += (s, e) =>
                {
                    int id = (int)((System.Windows.Forms.Button)s).Tag;
                    var confirm = MessageBox.Show("Tem certeza que deseja apagar este portfólio?", "Confirmação", MessageBoxButtons.YesNo);
                    if (confirm == DialogResult.Yes)
                    {
                        ApagarPortfolio(id);
                    }
                };

                System.Windows.Forms.Button btnVerTrabalhos = new System.Windows.Forms.Button
                {
                    Text = "Ver Trabalhos",
                    Size = new Size(120, 30),
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Tahoma", 9, FontStyle.Bold),
                    Tag = portfolio.Id, 
                    Location = new Point(portfolioPanel.Width - 140, 10)
                };

                btnVerTrabalhos.Click += (s, e) =>
                {
                    int portfolioId = (int)((System.Windows.Forms.Button)s).Tag;
                    MostrarTrabalhosDoPortfolio(portfolioId);
                };


                portfolioPanel.Controls.Add(btnVerTrabalhos);
                portfolioPanel.Controls.Add(nomeLabel);
                portfolioPanel.Controls.Add(btnEditar);
                portfolioPanel.Controls.Add(btnApagar);
                lista.Controls.Add(portfolioPanel);
            }

            painel13.Controls.Add(lista);
        }

        private void MostrarTrabalhosDoPortfolio(int idPortfolio)
        {
            painel13.Controls.Clear();
            painel13.Dock = DockStyle.Fill;
            painel13.Padding = new Padding(20);

            Label titulo = new Label
            {
                Text = $"Trabalhos do Portfólio #{idPortfolio}",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = Color.SteelBlue,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            painel13.Controls.Add(titulo);

            FlowLayoutPanel lista = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Padding = new Padding(10)
            };

            List<TrabalhoInfo> trabalhos = GetTrabalhosDoPortfolio(idPortfolio);

            foreach (var trabalho in trabalhos)
            {
                Panel trabalhoPanel = new Panel
                {
                    Width = 700,
                    Height = 100,
                    BackColor = Color.White,
                    Margin = new Padding(0, 0, 0, 10),
                    Padding = new Padding(10),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label tituloLabel = new Label
                {
                    Text = $"#{trabalho.Id} - {trabalho.Titulo}",
                    Font = new Font("Tahoma", 11, FontStyle.Bold),
                    ForeColor = Color.FromArgb(25, 25, 112),
                    Location = new Point(10, 10),
                    AutoSize = true
                };

                Label precoLabel = new Label
                {
                    Text = $"Preço: {trabalho.Preco:C} | Disponibilidade: {trabalho.Disponibilidade}",
                    Font = new Font("Tahoma", 9),
                    Location = new Point(10, 40),
                    AutoSize = true
                };

                Label descricaoLabel = new Label
                {
                    Text = $"Descrição: {trabalho.Descricao}",
                    Font = new Font("Tahoma", 9),
                    Location = new Point(10, 65),
                    AutoSize = true
                };

                trabalhoPanel.Controls.Add(tituloLabel);
                trabalhoPanel.Controls.Add(precoLabel);
                trabalhoPanel.Controls.Add(descricaoLabel);

                lista.Controls.Add(trabalhoPanel);
            }

            painel13.Controls.Add(lista);
        }


        private void ApagarPortfolio(int idPortfolio)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GerirPortfolio", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", idPortfolio);
                cmd.Parameters.AddWithValue("@Nome", ""); // dummy
                cmd.Parameters.AddWithValue("@ArtistaId", idUtilizadorAtual);
                cmd.Parameters.AddWithValue("@Acao", "REMOVER");

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Portfólio eliminado com sucesso!", "Sucesso");
            ShowPortfoliosDoArtista(); // Recarrega
        }

        private void MostrarFormularioPortfolio(PortfolioInfo portfolio = null)
        {
            var oldPanel = painel13.Controls.OfType<Panel>().FirstOrDefault(p => p.BackColor == Color.AliceBlue);
            if (oldPanel != null)
            {
                painel13.Controls.Remove(oldPanel);
            }

            Panel formPanel = new Panel
            {
                Width = 700,
                Height = 150,
                BackColor = Color.AliceBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 0, 10)
            };

            TextBox txtNome = new TextBox { Width = 300 };

            if (portfolio != null)
            {
                txtNome.Text = portfolio.Nome;
            }

            System.Windows.Forms.Button btnGuardar = new System.Windows.Forms.Button
            {
                Text = portfolio == null ? "Adicionar" : "Atualizar",
                BackColor = Color.Teal,
                ForeColor = Color.White,
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                Width = 100,
                Height = 35
            };

            btnGuardar.Click += (s, e) =>
            {
                string nome = txtNome.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GerirPortfolio", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    int idPortfolio = portfolio == null ? ObterProximoIdDisponivel("Portfolio") : portfolio.Id;

                    cmd.Parameters.AddWithValue("@Id", idPortfolio);
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@ArtistaId", idUtilizadorAtual);
                    cmd.Parameters.AddWithValue("@Acao", portfolio == null ? "INSERIR" : "ATUALIZAR");

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Portfólio salvo com sucesso!", "Sucesso");
                ShowPortfoliosDoArtista(); // Recarrega
            };

            // Layout
            FlowLayoutPanel inputs = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Padding = new Padding(10)
            };

            inputs.Controls.Add(new Label
            {
                Text = "Nome do Portfólio *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(txtNome);
            inputs.Controls.Add(btnGuardar);

            formPanel.Controls.Add(inputs);
            painel13.Controls.Add(formPanel);
            painel13.Controls.SetChildIndex(formPanel, 0);
        }


        private List<PortfolioInfo> GetPortfoliosDoArtista(int idArtista)
        {
            List<PortfolioInfo> lista = new List<PortfolioInfo>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT id, nome FROM projeto.Portfolio WHERE artista = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idArtista);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new PortfolioInfo
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1)
                        });
                    }
                }
            }

            return lista;
        }

        private List<TrabalhoInfo> GetTrabalhosDoPortfolio(int idPortfolio)
        {
            List<TrabalhoInfo> lista = new List<TrabalhoInfo>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetTrabalhosDoPortfolio", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PortfolioId", idPortfolio);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new TrabalhoInfo
                        {
                            Id = reader.GetInt32(0),
                            Titulo = reader.GetString(1),
                            Descricao = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Preco = reader.GetDecimal(4),
                            Disponibilidade = reader.GetString(5)
                        });
                    }
                }
            }

            return lista;
        }


        private List<CursoInfo> GetCursosDoArtista(int idArtista)
        {
            List<CursoInfo> lista = new List<CursoInfo>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT c.id, c.nome, c.preco, c.descricao, c.data_inicio, c.data_fim,
                   c.formato, c.capacidade, u.nome as nome_artista
            FROM projeto.Curso c
            INNER JOIN projeto.Artista a ON c.id_artista = a.id
            INNER JOIN projeto.Utilizador u ON a.id = u.id
            WHERE a.id = @idArtista";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idArtista", idArtista);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new CursoInfo
                        {
                            Id = reader.GetInt32(0),
                            NomeCurso = reader.GetString(1),
                            Preco = reader.GetDecimal(2),
                            Descricao = reader.IsDBNull(3) ? "Sem descrição" : reader.GetString(3),
                            DataInicio = reader.GetDateTime(4),
                            DataFim = reader.GetDateTime(5),
                            Formato = reader.IsDBNull(6) ? "N/A" : reader.GetString(6),
                            Capacidade = reader.GetInt32(7),
                            NomeArtista = reader.GetString(8)
                        });
                    }
                }
            }

            return lista;
        }

        private void ShowCursosDoArtista()
        {
            if (idUtilizadorAtual == 0)
            {
                MessageBox.Show("É necessário iniciar sessão para ver os seus cursos.");
                return;
            }

            painel13.Controls.Clear();
            painel13.Dock = DockStyle.Fill;
            painel13.Padding = new Padding(20);

            FlowLayoutPanel lista = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Padding = new Padding(10)
            };

            List<CursoInfo> cursos = GetCursosDoArtista(idUtilizadorAtual);

            // Botão Novo Curso
            System.Windows.Forms.Button btnNovoCurso = new System.Windows.Forms.Button
            {
                Text = "➕ Novo Curso",
                Size = new Size(150, 40),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 15)
            };

            btnNovoCurso.Click += (s, e) =>
            {
                MostrarFormularioCurso();
            };

            painel13.Controls.Add(btnNovoCurso);

            // Listar Cursos
            foreach (var curso in cursos)
            {
                Panel cursoPanel = new Panel
                {
                    Width = 700,
                    Height = 180,
                    BackColor = Color.White,
                    Margin = new Padding(0, 0, 0, 10),
                    Padding = new Padding(10),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label nomeLabel = new Label
                {
                    Text = $"#{curso.Id} - {curso.NomeCurso} | {curso.Preco}€ | {curso.Formato}",
                    Font = new Font("Tahoma", 10, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Dock = DockStyle.Top,
                    Height = 25
                };

                Label dataLabel = new Label
                {
                    Text = $"{curso.DataInicio:dd/MM/yyyy} até {curso.DataFim:dd/MM/yyyy}",
                    Font = new Font("Tahoma", 9),
                    Dock = DockStyle.Top,
                    Height = 20
                };

                Label capacidadeLabel = new Label
                {
                    Text = $"Capacidade: {curso.Capacidade}",
                    Font = new Font("Tahoma", 9),
                    Dock = DockStyle.Top,
                    Height = 20
                };

                Label descLabel = new Label
                {
                    Text = $"Descrição: {curso.Descricao}",
                    Font = new Font("Tahoma", 9, FontStyle.Italic),
                    Dock = DockStyle.Top,
                    Height = 40
                };

                // Botão Editar
                System.Windows.Forms.Button btnEditar = new System.Windows.Forms.Button
                {
                    Text = "Editar",
                    Size = new Size(80, 30),
                    Location = new Point(cursoPanel.Width - 190, 120),
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Tahoma", 8, FontStyle.Bold),
                    Tag = curso
                };

                btnEditar.Click += (s, e) =>
                {
                    CursoInfo c = (CursoInfo)((System.Windows.Forms.Button)s).Tag;
                    MostrarFormularioCurso(c);
                };

                // Botão Apagar
                System.Windows.Forms.Button btnApagar = new System.Windows.Forms.Button
                {
                    Text = "Eliminar",
                    Size = new Size(80, 30),
                    Location = new Point(cursoPanel.Width - 100, 120),
                    BackColor = Color.Firebrick,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Tahoma", 8, FontStyle.Bold),
                    Tag = curso.Id
                };

                btnApagar.Click += (s, e) =>
                {
                    int id = (int)((System.Windows.Forms.Button)s).Tag;
                    var confirm = MessageBox.Show("Tem certeza que deseja apagar este curso?", "Confirmação", MessageBoxButtons.YesNo);
                    if (confirm == DialogResult.Yes)
                    {
                        ApagarCurso(id);
                        ShowCursosDoArtista();
                    }
                };

                cursoPanel.Controls.Add(descLabel);
                cursoPanel.Controls.Add(capacidadeLabel);
                cursoPanel.Controls.Add(dataLabel);
                cursoPanel.Controls.Add(nomeLabel);
                cursoPanel.Controls.Add(btnEditar);
                cursoPanel.Controls.Add(btnApagar);
                lista.Controls.Add(cursoPanel);
            }

            painel13.Controls.Add(lista);
        }

        private void ApagarCurso(int idCurso)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GerirCurso", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", idCurso);
                cmd.Parameters.AddWithValue("@Nome", "");
                cmd.Parameters.AddWithValue("@Preco", 0);
                cmd.Parameters.AddWithValue("@Descricao", "");
                cmd.Parameters.AddWithValue("@DataInicio", DateTime.Now);
                cmd.Parameters.AddWithValue("@DataFim", DateTime.Now);
                cmd.Parameters.AddWithValue("@Formato", "");
                cmd.Parameters.AddWithValue("@Capacidade", 0);
                cmd.Parameters.AddWithValue("@IdArtista", idUtilizadorAtual);
                cmd.Parameters.AddWithValue("@Acao", "REMOVER");

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Curso eliminado com sucesso!", "Sucesso");
        }

        private void MostrarFormularioCurso(CursoInfo curso = null)
        {
            // Remover se já houver um painel aberto
            var oldPanel = painel13.Controls.OfType<Panel>().FirstOrDefault(p => p.BackColor == Color.AliceBlue);
            if (oldPanel != null)
            {
                painel13.Controls.Remove(oldPanel);
            }

            Panel formPanel = new Panel
            {
                Width = 700,
                Height = 400,
                BackColor = Color.AliceBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 0, 10)
            };

            TextBox txtNome = new TextBox { Width = 300 };
            NumericUpDown numPreco = new NumericUpDown { Minimum = 0, Maximum = 10000, DecimalPlaces = 2, Value = 10, Width = 100 };
            TextBox txtDescricao = new TextBox { Width = 300, Height = 60, Multiline = true };
            DateTimePicker dtInicio = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 120 };
            DateTimePicker dtFim = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 120 };
            ComboBox comboFormato = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
            comboFormato.Items.AddRange(new[] { "Presencial", "Online", "Híbrido" });
            NumericUpDown numCapacidade = new NumericUpDown { Minimum = 1, Maximum = 100, Value = 10, Width = 100 };

            System.Windows.Forms.Button btnGuardar = new System.Windows.Forms.Button
            {
                Text = curso == null ? "Adicionar" : "Atualizar",
                BackColor = Color.Teal,
                ForeColor = Color.White,
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                Width = 100,
                Height = 35
            };

            // Se estiver a editar, preenche os campos
            if (curso != null)
            {
                txtNome.Text = curso.NomeCurso;
                numPreco.Value = curso.Preco;
                txtDescricao.Text = curso.Descricao;
                dtInicio.Value = curso.DataInicio;
                dtFim.Value = curso.DataFim;
                comboFormato.SelectedItem = curso.Formato;
                numCapacidade.Value = curso.Capacidade;
            }

            btnGuardar.Click += (s, e) =>
            {
                string nome = txtNome.Text.Trim();
                decimal preco = numPreco.Value;
                string descricao = txtDescricao.Text.Trim();
                DateTime dataInicio = dtInicio.Value.Date;
                DateTime dataFim = dtFim.Value.Date;
                string formato = comboFormato.Text;
                int capacidade = (int)numCapacidade.Value;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GerirCurso", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    int idCurso = curso == null ? ObterProximoIdDisponivel("Curso") : curso.Id;

                    cmd.Parameters.AddWithValue("@Id", idCurso);
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@Preco", preco);
                    cmd.Parameters.AddWithValue("@Descricao", descricao);
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFim", dataFim);
                    cmd.Parameters.AddWithValue("@Formato", formato);
                    cmd.Parameters.AddWithValue("@Capacidade", capacidade);
                    cmd.Parameters.AddWithValue("@IdArtista", idUtilizadorAtual);

                    string acao = curso == null ? "INSERIR" : "ATUALIZAR";
                    cmd.Parameters.AddWithValue("@Acao", acao);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Curso salvo com sucesso!", "Sucesso");
                ShowCursosDoArtista(); // Recarrega
            };

            // Layout
            FlowLayoutPanel inputs = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Padding = new Padding(10)
            };

            // Nome
            inputs.Controls.Add(new Label
            {
                Text = "Nome do Curso *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(txtNome);

            // Preço
            inputs.Controls.Add(new Label
            {
                Text = "Preço (€) *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(numPreco);

            // Descrição
            inputs.Controls.Add(new Label
            {
                Text = "Descrição",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(txtDescricao);

            // Datas
            inputs.Controls.Add(new Label
            {
                Text = "Data Início *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(dtInicio);

            inputs.Controls.Add(new Label
            {
                Text = "Data Fim *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(dtFim);

            // Formato
            inputs.Controls.Add(new Label
            {
                Text = "Formato *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(comboFormato);

            // Capacidade
            inputs.Controls.Add(new Label
            {
                Text = "Capacidade *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(numCapacidade);

            // Botão Guardar
            inputs.Controls.Add(btnGuardar);

            formPanel.Controls.Add(inputs);
            painel13.Controls.Add(formPanel);
            painel13.Controls.SetChildIndex(formPanel, 0);
        }




        private List<TurmaInfo> GetTurmasDoArtista(int idArtista)
        {
            List<TurmaInfo> lista = new List<TurmaInfo>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                                SELECT t.id, t.nome, t.capacidade, t.nivel, t.horario, c.nome AS nome_curso
                                FROM projeto.Turma t
                                INNER JOIN projeto.Curso c ON t.id_curso = c.id
                                WHERE c.id_artista = @idArtista";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idArtista", idArtista);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new TurmaInfo
                        {
                            Id = reader.GetInt32(0),
                            NomeTurma = reader.GetString(1),
                            Capacidade = reader.GetInt32(2),
                            Nivel = reader.GetString(3),
                            Horario = reader.GetString(4),
                            NomeCurso = reader.GetString(5)
                        });
                    }
                }
            }

            return lista;
        }

    private void ShowTurmasDoArtista()
    {
        if (idUtilizadorAtual == 0)
        {
            MessageBox.Show("É necessário iniciar sessão para ver as suas turmas.");
            return;
        }

        painel13.Controls.Clear();
        painel13.Dock = DockStyle.Fill;
        painel13.Padding = new Padding(20);

        FlowLayoutPanel lista = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            WrapContents = false,
            Padding = new Padding(10)
        };

        List<TurmaInfo> turmas = GetTurmasDoArtista(idUtilizadorAtual);

            System.Windows.Forms.Button btnNovaTurma = new System.Windows.Forms.Button
            {
                Text = "➕ Nova Turma",
                Size = new Size(150, 40),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 15)
            };


            btnNovaTurma.Click += (s, e) =>
            {
                MostrarFormularioTurma();
            };

            painel13.Controls.Add(btnNovaTurma);


            foreach (var turma in turmas)
            {
                Panel turmaPanel = new Panel
                {
                    Width = 700,
                    Height = 140,
                    BackColor = Color.White,
                    Margin = new Padding(0, 0, 0, 10),
                    Padding = new Padding(10),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label nomeLabel = new Label
                {
                    Text = $"#{turma.Id} - {turma.NomeTurma}  |  Curso: {turma.NomeCurso}",
                    Font = new Font("Tahoma", 11, FontStyle.Bold),
                    ForeColor = Color.FromArgb(25, 25, 112),
                    Location = new Point(10, 10),
                    AutoSize = true
                };

                Label capLabel = new Label
                {
                    Text = $"Capacidade: {turma.Capacidade}",
                    Font = new Font("Tahoma", 9),
                    Location = new Point(10, 40),
                    AutoSize = true
                };

                Label nivelHorario = new Label
                {
                    Text = $"Nível: {turma.Nivel}   |   Horário: {turma.Horario}",
                    Font = new Font("Tahoma", 9),
                    Location = new Point(10, 65),
                    AutoSize = true
                };

                // Botão Editar
                System.Windows.Forms.Button btnEditar = new System.Windows.Forms.Button
                {
                    Text = "Editar",
                    Size = new Size(80, 30),
                    Location = new Point(turmaPanel.Width - 190, 90),
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Tahoma", 8, FontStyle.Bold),
                    Tag = turma
                };

                btnEditar.Click += (s, e) =>
                {
                    TurmaInfo t = (TurmaInfo)((System.Windows.Forms.Button)s).Tag;
                    MostrarFormularioTurma(t);
                };


                // Botão Apagar
                System.Windows.Forms.Button btnApagar = new System.Windows.Forms.Button
                {
                    Text = "Eliminar",
                    Size = new Size(80, 30),
                    Location = new Point(turmaPanel.Width - 100, 90),
                    BackColor = Color.Firebrick,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Tahoma", 8, FontStyle.Bold),
                    Tag = turma.Id
                };

                btnApagar.Click += (s, e) =>
                {
                    int id = (int)((System.Windows.Forms.Button)s).Tag;
                    var confirm = MessageBox.Show("Tem certeza que deseja apagar esta turma?", "Confirmação", MessageBoxButtons.YesNo);
                    if (confirm == DialogResult.Yes)
                    {
                        ApagarTurma(id);
                    }
                };


                // Adicionar controls
                turmaPanel.Controls.Add(nomeLabel);
                turmaPanel.Controls.Add(capLabel);
                turmaPanel.Controls.Add(nivelHorario);
                turmaPanel.Controls.Add(btnEditar);
                turmaPanel.Controls.Add(btnApagar);
                lista.Controls.Add(turmaPanel);

            }
            painel13.Controls.Add(lista);
    }

        private void ApagarTurma(int idTurma)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GerirTurma", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", idTurma);
                cmd.Parameters.AddWithValue("@Nome", "");
                cmd.Parameters.AddWithValue("@Capacidade", 0); 
                cmd.Parameters.AddWithValue("@Nivel", "");
                cmd.Parameters.AddWithValue("@Horario", "");
                cmd.Parameters.AddWithValue("@CursoId", 0); 
                cmd.Parameters.AddWithValue("@Acao", "REMOVER");

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Turma eliminada com sucesso!", "Sucesso");
            ShowTurmasDoArtista(); // Recarrega
        }


        private void MostrarFormularioTurma(TurmaInfo turma = null)
            {

            var oldPanel = painel13.Controls.OfType<Panel>().FirstOrDefault(p => p.BackColor == Color.AliceBlue);
            if (oldPanel != null)
            {
                painel13.Controls.Remove(oldPanel);
            }
            Panel formPanel = new Panel
                {
                    Width = 700,
                    Height = 220,
                    BackColor = Color.AliceBlue,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(10),
                    Margin = new Padding(0, 0, 0, 10)
                };

                TextBox txtNome = new TextBox {Width = 300 };
                NumericUpDown numCapacidade = new NumericUpDown { Minimum = 1, Maximum = 100, Value = 10 };
                ComboBox comboNivel = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
                comboNivel.Items.AddRange(new[] { "Iniciante", "Intermédio", "Avançado" });

                TextBox txtHorario = new TextBox {Width = 300 };
                ComboBox comboCurso = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

                System.Windows.Forms.Button btnGuardar = new System.Windows.Forms.Button
                {
                    Text = turma == null ? "Adicionar" : "Atualizar",
                    BackColor = Color.Teal,
                    ForeColor = Color.White,
                    Font = new Font("Tahoma", 9, FontStyle.Bold),
                    Width = 100,
                    Height = 35
                };

                // Preencher cursos
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, nome FROM projeto.Curso WHERE id_artista = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", idUtilizadorAtual);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboCurso.Items.Add(new ComboBoxItem
                            {
                                Text = reader.GetString(1),
                                Value = reader.GetInt32(0)
                            });
                        }
                    }
                }

                // Se estiver a editar, preenche os campos
                if (turma != null)
                {
                    txtNome.Text = turma.NomeTurma;
                    numCapacidade.Value = turma.Capacidade;
                    comboNivel.SelectedItem = turma.Nivel;
                    txtHorario.Text = turma.Horario;

                    foreach (ComboBoxItem item in comboCurso.Items)
                    {
                        if ((int)item.Value == turma.IdCurso)
                        {
                            comboCurso.SelectedItem = item;
                            break;
                        }
                    }
                }


            btnGuardar.Click += (s, e) =>
                {
                    string nome = txtNome.Text.Trim();
                    string nivel = comboNivel.Text;
                    string horario = txtHorario.Text.Trim();
                    int capacidade = (int)numCapacidade.Value;
                    int idCurso = ((ComboBoxItem)comboCurso.SelectedItem).Value;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("sp_GerirTurma", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        int idTurma = turma == null ? ObterProximoIdDisponivel("Turma") : turma.Id;

                        cmd.Parameters.AddWithValue("@Id", idTurma);
                        cmd.Parameters.AddWithValue("@Nome", nome);
                        cmd.Parameters.AddWithValue("@Capacidade", capacidade);
                        cmd.Parameters.AddWithValue("@Nivel", nivel);
                        cmd.Parameters.AddWithValue("@Horario", horario);
                        cmd.Parameters.AddWithValue("@CursoId", idCurso);

                        string acao = turma == null ? "INSERIR" : "ATUALIZAR";
                        cmd.Parameters.AddWithValue("@Acao", acao);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }


                    MessageBox.Show("Turma salva com sucesso!", "Sucesso");
                    ShowTurmasDoArtista(); // Recarrega
                };

            // Layout
            FlowLayoutPanel inputs = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Padding = new Padding(10)
            };

            // Label + TextBox Nome
            inputs.Controls.Add(new Label
            {
                Text = "Nome da Turma *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(txtNome);

            // Label + Capacidade
            inputs.Controls.Add(new Label
            {
                Text = "Capacidade *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(numCapacidade);

            // Label + Nível
            inputs.Controls.Add(new Label
            {
                Text = "Nível *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(comboNivel);

            // Label + Horário
            inputs.Controls.Add(new Label
            {
                Text = "Horário *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(txtHorario);

            // Label + Curso
            inputs.Controls.Add(new Label
            {
                Text = "Curso *",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            });
            inputs.Controls.Add(comboCurso);

            // Botão Guardar
            inputs.Controls.Add(btnGuardar);


            inputs.Controls.Add(txtNome);
            inputs.Controls.Add(numCapacidade);
            inputs.Controls.Add(comboNivel);
            inputs.Controls.Add(txtHorario);
            inputs.Controls.Add(comboCurso);
            inputs.Controls.Add(btnGuardar);

            formPanel.Controls.Add(inputs);
            painel13.Controls.Add(formPanel);
            painel13.Controls.SetChildIndex(formPanel, 0);
        }

        private int ObterProximoIdDisponivel(string tabela)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand($"SELECT ISNULL(MAX(id), 0) + 1 FROM {tabela}", conn);
                return (int)cmd.ExecuteScalar();
            }
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
                        case "btnSettings":
                            ShowPainelGestaoArtista();
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

        private void btnSettings_Click(object sender, EventArgs e)
        {
            ShowPainelGestaoArtista();
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
                    case "btnSettings":
                        if (buttonName == "btnSettings") ShowPainelGestaoArtista();
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

        private void btnSettings_Paint_1(object sender, PaintEventArgs e){}

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void painel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button6_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    public class PortfolioInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public List<TrabalhoInfo> Trabalhos { get; set; }
    }

    public class TurmaInfo
    {
        public int Id { get; set; }
        public string NomeTurma { get; set; }
        public int Capacidade { get; set; }
        public string Nivel { get; set; }
        public string Horario { get; set; }
        public string NomeCurso { get; set; }
        public int IdCurso { get; set; }
    }


}
