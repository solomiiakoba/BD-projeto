using App.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=SOLOMIIA;Initial Catalog=Projeto;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
            BtnClick(null, EventArgs.Empty);
            LoadCategoriesFromDatabase();
        }

        private void painel13_Paint(object sender, PaintEventArgs e)
        {
        }

        private void LoadCategoriesFromDatabase()
        {
            try
            {
                List<CategoryInfo> categories = GetCategories();
                painel13.Controls.Clear();
                painel13.Padding = new Padding(20);

                foreach (var category in categories)
                {
                    CreateCategoryButton(category, painel13);
                }
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
                string query = "SELECT c.id, c.nome_categoria, COUNT(t.id) as trabalhos_count\r\nFROM Categoria c\r\nLEFT JOIN Trabalho t ON c.id = t.id_categoria\r\nGROUP BY c.id, c.nome_categoria\r\nORDER BY c.id\r\n";

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

        private void CreateCategoryButton(CategoryInfo category, Panel parentPanel)
        {
            Panel categoryPanel = new Panel();
            categoryPanel.Width = 100;
            categoryPanel.Height = 100;
            categoryPanel.BackColor = Color.White;
            categoryPanel.Margin = new Padding(10);
            categoryPanel.Tag = category.Id;

            int categoriesPerRow = 5;
            int margin = 35;
            int buttonWidth = categoryPanel.Width;
            int buttonHeight = categoryPanel.Height;

            int index = parentPanel.Controls.Count;
            int row = index / categoriesPerRow;
            int col = index % categoriesPerRow;

            categoryPanel.Location = new Point(
                margin + col * (buttonWidth + margin),
                margin + row * (buttonHeight + margin)
            );

            categoryPanel.Paint += (sender, e) => {
                var panel = sender as Panel;
                var graphics = e.Graphics;

                // Draw rounded rectangle
                using (var pen = new Pen(Color.FromArgb(255, 180, 180), 2))
                {
                    var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    var path = CreateRoundedRectangle(rect, 10);
                    graphics.DrawPath(pen, path);

                    // Fill with gradient
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        rect, Color.White, Color.FromArgb(255, 235, 235), 45))
                    {
                        graphics.FillPath(brush, path);
                    }
                }
            };

            // Add category name
            Label nameLabel = new Label();
            nameLabel.Text = category.Name;
            nameLabel.Font = new Font("Tahoma", 12, FontStyle.Bold);
            nameLabel.ForeColor = Color.FromArgb(170, 50, 50);
            nameLabel.TextAlign = ContentAlignment.MiddleCenter;
            nameLabel.Width = categoryPanel.Width;
            nameLabel.Height = 30;
            nameLabel.Location = new Point(0, 50);
            nameLabel.BackColor = Color.Transparent;

            // Add item count
            Label countLabel = new Label();
            countLabel.Text = category.ItemCount + " trabalhos";
            countLabel.Font = new Font("Tahoma", 10, FontStyle.Regular);
            countLabel.ForeColor = Color.FromArgb(100, 100, 100);
            countLabel.TextAlign = ContentAlignment.MiddleCenter;
            countLabel.Width = categoryPanel.Width;
            countLabel.Height = 20;
            countLabel.Location = new Point(0, 80);
            countLabel.BackColor = Color.Transparent;

            // Add event handlers
            categoryPanel.Click += CategoryPanel_Click;
            categoryPanel.MouseEnter += (s, e) => {
                categoryPanel.Cursor = Cursors.Hand;
            };

            // Add components to panel
            categoryPanel.Controls.Add(nameLabel);
            categoryPanel.Controls.Add(countLabel);

            // Add to parent panel
            parentPanel.Controls.Add(categoryPanel);
        }

        private System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

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

        private void CategoryPanel_Click(object sender, EventArgs e)
        {
            Panel clickedPanel = (Panel)sender;
            int categoryId = (int)clickedPanel.Tag;

            // Handle category selection
            MessageBox.Show("Categoria selecionada: " + categoryId, "Navegação",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Here you would typically navigate to a category detail view
            // LoadCategoryDetails(categoryId);
        }

        private void button2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void button1_Paint_1(object sender, PaintEventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void painel11_Paint(object sender, PaintEventArgs e)
        {
        }

        private void ArtistsButton_Paint(object sender, PaintEventArgs e)
        {
        }

        private void CursesBtn_Paint(object sender, PaintEventArgs e)
        {
        }

        private void FavouritesBtn_Paint(object sender, PaintEventArgs e)
        {
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
                if (btn.Name == buttonName)
                {
                    btn.BackColor = Color.MistyRose;
                    switch (btn.Name)
                    {
                        case "Homebtn": btn.ButtonImage = Resources.house_solid; break;
                        case "ArtistsButton": btn.ButtonImage = Resources.paintbrush_solid; break;
                        case "CursesBtn": btn.ButtonImage = Resources.graduation_cap_solid; break;
                        case "FavouritesBtn": btn.ButtonImage = Resources.heart_solid__2_; break;
                    }
                }
                else
                {
                    btn.BackColor = Color.Transparent;
                    switch (btn.Name)
                    {
                        case "Homebtn": btn.ButtonImage = Resources.house_solid; break;
                        case "ArtistsButton": btn.ButtonImage = Resources.paintbrush_solid; break;
                        case "CursesBtn": btn.ButtonImage = Resources.graduation_cap_solid; break;
                        case "FavouritesBtn": btn.ButtonImage = Resources.heart_solid__2_; break;
                    }
                }
            }
        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Paint_1(object sender, PaintEventArgs e)
        {
        }

        private void button2_Paint_2(object sender, PaintEventArgs e)
        {

        }
    }

    public class CategoryInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ItemCount { get; set; }
    }
}