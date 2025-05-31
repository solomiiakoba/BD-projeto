namespace App
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Main_Pnl = new Component.Painel1();
            this.cmbMetodoPagamento = new System.Windows.Forms.ComboBox();
            this.pnlPrecoCurso = new System.Windows.Forms.Panel();
            this.lblPrecoFinal = new System.Windows.Forms.Label();
            this.btnCancelar = new Component.Button();
            this.cmbHorario = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboNivel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTurma = new System.Windows.Forms.ComboBox();
            this.lblTamanho = new System.Windows.Forms.Label();
            this.lblSecaoPersonalizacao = new System.Windows.Forms.Label();
            this.Inscricao_Pnl = new System.Windows.Forms.Panel();
            this.lblDetalhesCurso = new System.Windows.Forms.Label();
            this.lblSecaoDetalhes = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnConfirmarCurso = new Component.Button();
            this.lblDetalhesCurso1 = new System.Windows.Forms.Label();
            this.lblCapacidadeInfo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.Main_Pnl.SuspendLayout();
            this.pnlPrecoCurso.SuspendLayout();
            this.Inscricao_Pnl.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Main_Pnl
            // 
            this.Main_Pnl.BackColor = System.Drawing.Color.White;
            this.Main_Pnl.BorderRadius = 30;
            this.Main_Pnl.Controls.Add(this.lblCapacidadeInfo);
            this.Main_Pnl.Controls.Add(this.cmbMetodoPagamento);
            this.Main_Pnl.Controls.Add(this.pnlPrecoCurso);
            this.Main_Pnl.Controls.Add(this.btnCancelar);
            this.Main_Pnl.Controls.Add(this.cmbHorario);
            this.Main_Pnl.Controls.Add(this.label4);
            this.Main_Pnl.Controls.Add(this.label2);
            this.Main_Pnl.Controls.Add(this.comboNivel);
            this.Main_Pnl.Controls.Add(this.label1);
            this.Main_Pnl.Controls.Add(this.cmbTurma);
            this.Main_Pnl.Controls.Add(this.lblTamanho);
            this.Main_Pnl.Controls.Add(this.lblSecaoPersonalizacao);
            this.Main_Pnl.Controls.Add(this.Inscricao_Pnl);
            this.Main_Pnl.Controls.Add(this.lblSecaoDetalhes);
            this.Main_Pnl.Controls.Add(this.lblTitulo);
            this.Main_Pnl.Controls.Add(this.btnConfirmarCurso);
            this.Main_Pnl.Controls.Add(this.lblDetalhesCurso1);
            this.Main_Pnl.Controls.Add(this.panel1);
            this.Main_Pnl.ForeColor = System.Drawing.Color.Black;
            this.Main_Pnl.GradientAngle = 90F;
            this.Main_Pnl.GradientBottomColor = System.Drawing.Color.White;
            this.Main_Pnl.GradientTopColor = System.Drawing.Color.White;
            this.Main_Pnl.Location = new System.Drawing.Point(8, 6);
            this.Main_Pnl.Name = "Main_Pnl";
            this.Main_Pnl.Size = new System.Drawing.Size(1047, 705);
            this.Main_Pnl.TabIndex = 1;
            this.Main_Pnl.Paint += new System.Windows.Forms.PaintEventHandler(this.Main_Pnl_Paint);
            // 
            // cmbMetodoPagamento
            // 
            this.cmbMetodoPagamento.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cmbMetodoPagamento.FormattingEnabled = true;
            this.cmbMetodoPagamento.Location = new System.Drawing.Point(35, 542);
            this.cmbMetodoPagamento.Name = "cmbMetodoPagamento";
            this.cmbMetodoPagamento.Size = new System.Drawing.Size(276, 24);
            this.cmbMetodoPagamento.TabIndex = 27;
            this.cmbMetodoPagamento.SelectedIndexChanged += new System.EventHandler(this.cmbMetodoPagamento_SelectedIndexChanged);
            // 
            // pnlPrecoCurso
            // 
            this.pnlPrecoCurso.BackColor = System.Drawing.Color.MistyRose;
            this.pnlPrecoCurso.Controls.Add(this.lblPrecoFinal);
            this.pnlPrecoCurso.Location = new System.Drawing.Point(35, 606);
            this.pnlPrecoCurso.Name = "pnlPrecoCurso";
            this.pnlPrecoCurso.Size = new System.Drawing.Size(484, 61);
            this.pnlPrecoCurso.TabIndex = 24;
            this.pnlPrecoCurso.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlPrecoCurso_Paint);
            // 
            // lblPrecoFinal
            // 
            this.lblPrecoFinal.AutoSize = true;
            this.lblPrecoFinal.Location = new System.Drawing.Point(16, 23);
            this.lblPrecoFinal.Name = "lblPrecoFinal";
            this.lblPrecoFinal.Size = new System.Drawing.Size(0, 16);
            this.lblPrecoFinal.TabIndex = 0;
            this.lblPrecoFinal.Click += new System.EventHandler(this.lblPrecoFinal_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackgroundColor = System.Drawing.Color.Transparent;
            this.btnCancelar.BorderColor = System.Drawing.Color.Firebrick;
            this.btnCancelar.BorderRadius = 30;
            this.btnCancelar.BorderSize = 0;
            this.btnCancelar.ButtonImage = null;
            this.btnCancelar.ButtonText = "Cancelar";
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.GradientAngle = 90F;
            this.btnCancelar.GradientBottomColor = System.Drawing.Color.Firebrick;
            this.btnCancelar.GradientTopColor = System.Drawing.Color.Firebrick;
            this.btnCancelar.IsSelected = false;
            this.btnCancelar.Location = new System.Drawing.Point(780, 617);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(150, 50);
            this.btnCancelar.TabIndex = 23;
            this.btnCancelar.TextColor = System.Drawing.Color.White;
            this.btnCancelar.Paint += new System.Windows.Forms.PaintEventHandler(this.btnCancelar_Paint);
            // 
            // cmbHorario
            // 
            this.cmbHorario.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cmbHorario.FormattingEnabled = true;
            this.cmbHorario.Location = new System.Drawing.Point(35, 443);
            this.cmbHorario.Name = "cmbHorario";
            this.cmbHorario.Size = new System.Drawing.Size(276, 24);
            this.cmbHorario.TabIndex = 20;
            this.cmbHorario.SelectedIndexChanged += new System.EventHandler(this.cmbHorario_SelectedIndexChanged_1);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(31, 414);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 21);
            this.label4.TabIndex = 19;
            this.label4.Text = "Horário";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(31, 509);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(203, 21);
            this.label2.TabIndex = 18;
            this.label2.Text = "Método de pagamento";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // comboNivel
            // 
            this.comboNivel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.comboNivel.FormattingEnabled = true;
            this.comboNivel.Location = new System.Drawing.Point(563, 443);
            this.comboNivel.Name = "comboNivel";
            this.comboNivel.Size = new System.Drawing.Size(276, 24);
            this.comboNivel.TabIndex = 17;
            this.comboNivel.SelectedIndexChanged += new System.EventHandler(this.comboNivel_SelectedIndexChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(559, 414);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 21);
            this.label1.TabIndex = 16;
            this.label1.Text = "Nível";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // cmbTurma
            // 
            this.cmbTurma.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cmbTurma.FormattingEnabled = true;
            this.cmbTurma.Location = new System.Drawing.Point(35, 343);
            this.cmbTurma.Name = "cmbTurma";
            this.cmbTurma.Size = new System.Drawing.Size(276, 24);
            this.cmbTurma.TabIndex = 15;
            this.cmbTurma.SelectedIndexChanged += new System.EventHandler(this.cmbTurma_SelectedIndexChanged_1);
            // 
            // lblTamanho
            // 
            this.lblTamanho.AutoSize = true;
            this.lblTamanho.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblTamanho.ForeColor = System.Drawing.Color.Black;
            this.lblTamanho.Location = new System.Drawing.Point(31, 314);
            this.lblTamanho.Name = "lblTamanho";
            this.lblTamanho.Size = new System.Drawing.Size(64, 21);
            this.lblTamanho.TabIndex = 14;
            this.lblTamanho.Text = "Turma";
            this.lblTamanho.Click += new System.EventHandler(this.lblTamanho_Click);
            // 
            // lblSecaoPersonalizacao
            // 
            this.lblSecaoPersonalizacao.AutoSize = true;
            this.lblSecaoPersonalizacao.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecaoPersonalizacao.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblSecaoPersonalizacao.Location = new System.Drawing.Point(31, 281);
            this.lblSecaoPersonalizacao.Name = "lblSecaoPersonalizacao";
            this.lblSecaoPersonalizacao.Size = new System.Drawing.Size(83, 24);
            this.lblSecaoPersonalizacao.TabIndex = 13;
            this.lblSecaoPersonalizacao.Text = "Opções";
            this.lblSecaoPersonalizacao.Click += new System.EventHandler(this.lblSecaoPersonalizacao_Click);
            // 
            // Inscricao_Pnl
            // 
            this.Inscricao_Pnl.BackColor = System.Drawing.Color.MistyRose;
            this.Inscricao_Pnl.Controls.Add(this.lblDetalhesCurso);
            this.Inscricao_Pnl.Location = new System.Drawing.Point(35, 117);
            this.Inscricao_Pnl.Name = "Inscricao_Pnl";
            this.Inscricao_Pnl.Size = new System.Drawing.Size(985, 145);
            this.Inscricao_Pnl.TabIndex = 12;
            this.Inscricao_Pnl.Paint += new System.Windows.Forms.PaintEventHandler(this.Inscricao_Pnl_Paint);
            // 
            // lblDetalhesCurso
            // 
            this.lblDetalhesCurso.AutoSize = true;
            this.lblDetalhesCurso.Location = new System.Drawing.Point(16, 15);
            this.lblDetalhesCurso.Name = "lblDetalhesCurso";
            this.lblDetalhesCurso.Size = new System.Drawing.Size(0, 16);
            this.lblDetalhesCurso.TabIndex = 0;
            this.lblDetalhesCurso.Click += new System.EventHandler(this.lblDetalhesCurso_Click);
            // 
            // lblSecaoDetalhes
            // 
            this.lblSecaoDetalhes.AutoSize = true;
            this.lblSecaoDetalhes.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecaoDetalhes.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblSecaoDetalhes.Location = new System.Drawing.Point(31, 81);
            this.lblSecaoDetalhes.Name = "lblSecaoDetalhes";
            this.lblSecaoDetalhes.Size = new System.Drawing.Size(190, 24);
            this.lblSecaoDetalhes.TabIndex = 11;
            this.lblSecaoDetalhes.Text = "Detalhes do curso";
            this.lblSecaoDetalhes.Click += new System.EventHandler(this.lblSecaoDetalhes_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Tahoma", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblTitulo.Location = new System.Drawing.Point(29, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(285, 34);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Inscrição em Curso";
            this.lblTitulo.Click += new System.EventHandler(this.lblTitulo_Click_1);
            // 
            // btnConfirmarCurso
            // 
            this.btnConfirmarCurso.BackgroundColor = System.Drawing.Color.Transparent;
            this.btnConfirmarCurso.BorderColor = System.Drawing.Color.Firebrick;
            this.btnConfirmarCurso.BorderRadius = 30;
            this.btnConfirmarCurso.BorderSize = 0;
            this.btnConfirmarCurso.ButtonImage = null;
            this.btnConfirmarCurso.ButtonText = "Confirmar";
            this.btnConfirmarCurso.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmarCurso.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmarCurso.GradientAngle = 90F;
            this.btnConfirmarCurso.GradientBottomColor = System.Drawing.Color.Firebrick;
            this.btnConfirmarCurso.GradientTopColor = System.Drawing.Color.Firebrick;
            this.btnConfirmarCurso.IsSelected = false;
            this.btnConfirmarCurso.Location = new System.Drawing.Point(563, 617);
            this.btnConfirmarCurso.Name = "btnConfirmarCurso";
            this.btnConfirmarCurso.Size = new System.Drawing.Size(150, 50);
            this.btnConfirmarCurso.TabIndex = 10;
            this.btnConfirmarCurso.TextColor = System.Drawing.Color.White;
            this.btnConfirmarCurso.Paint += new System.Windows.Forms.PaintEventHandler(this.btnConfirmarCurso_Paint);
            // 
            // lblDetalhesCurso1
            // 
            this.lblDetalhesCurso1.AutoSize = true;
            this.lblDetalhesCurso1.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetalhesCurso1.Location = new System.Drawing.Point(51, 138);
            this.lblDetalhesCurso1.Name = "lblDetalhesCurso1";
            this.lblDetalhesCurso1.Size = new System.Drawing.Size(0, 28);
            this.lblDetalhesCurso1.TabIndex = 4;
            this.lblDetalhesCurso1.Click += new System.EventHandler(this.lblDetalhesCurso1_Click);
            // 
            // lblCapacidadeInfo
            // 
            this.lblCapacidadeInfo.AutoSize = true;
            this.lblCapacidadeInfo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCapacidadeInfo.Location = new System.Drawing.Point(568, 341);
            this.lblCapacidadeInfo.Name = "lblCapacidadeInfo";
            this.lblCapacidadeInfo.Size = new System.Drawing.Size(0, 24);
            this.lblCapacidadeInfo.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(425, 341);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(591, 53);
            this.panel1.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 16);
            this.label3.TabIndex = 0;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCoral;
            this.ClientSize = new System.Drawing.Size(1062, 723);
            this.Controls.Add(this.Main_Pnl);
            this.Name = "Form2";
            this.Main_Pnl.ResumeLayout(false);
            this.Main_Pnl.PerformLayout();
            this.pnlPrecoCurso.ResumeLayout(false);
            this.pnlPrecoCurso.PerformLayout();
            this.Inscricao_Pnl.ResumeLayout(false);
            this.Inscricao_Pnl.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Component.Painel1 Main_Pnl;
        private System.Windows.Forms.Panel pnlPrecoCurso;
        private Component.Button btnCancelar;
        private System.Windows.Forms.ComboBox cmbHorario;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboNivel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTurma;
        private System.Windows.Forms.Label lblTamanho;
        private System.Windows.Forms.Label lblSecaoPersonalizacao;
        private System.Windows.Forms.Panel Inscricao_Pnl;
        private System.Windows.Forms.Label lblSecaoDetalhes;
        private System.Windows.Forms.Label lblTitulo;
        private Component.Button btnConfirmarCurso;
        private System.Windows.Forms.Label lblDetalhesCurso1;
        private System.Windows.Forms.ComboBox cmbMetodoPagamento;
        private System.Windows.Forms.Label lblDetalhesCurso;
        private System.Windows.Forms.Label lblPrecoFinal;
        private System.Windows.Forms.Label lblCapacidadeInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
    }
}