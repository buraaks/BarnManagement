using BarnManagement.UI.CustomControls;
namespace BarnManagement.UI.Forms
{
    partial class MainForm
    {
        // Gerekli tasarımcı değişkeni.
        private System.ComponentModel.IContainer components = null;

        // Kullanılan tüm kaynakları temizle.
        // disposing true ise yönetilen kaynaklar atılmalı, aksi halde false.
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // Tasarımcı desteği için gerekli metot - değiştirmeyin
        // Bu metodun içeriğini kod düzenleyici ile değiştirmeyin.
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            animalsGrid = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewProgressBarColumn();
            productsGrid = new DataGridView();
            dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn10 = new DataGridViewTextBoxColumn();
            cashLabel = new Label();
            buyAnimalButton = new Button();
            sellProductsButton = new Button();
            resetGameButton = new Button();
            simulationTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)animalsGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)productsGrid).BeginInit();
            SuspendLayout();
            // 
            // animalsGrid
            // 
            animalsGrid.AllowUserToAddRows = false;
            animalsGrid.AllowUserToDeleteRows = false;
            animalsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            animalsGrid.ColumnHeadersHeight = 29;
            animalsGrid.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn6 });
            animalsGrid.Location = new Point(20, 20);
            animalsGrid.Name = "animalsGrid";
            animalsGrid.ReadOnly = true;
            animalsGrid.RowHeadersWidth = 51;
            animalsGrid.Size = new Size(600, 200);
            animalsGrid.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "ID";
            dataGridViewTextBoxColumn1.MinimumWidth = 6;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Name";
            dataGridViewTextBoxColumn2.MinimumWidth = 6;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Age";
            dataGridViewTextBoxColumn3.MinimumWidth = 6;
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Type";
            dataGridViewTextBoxColumn4.MinimumWidth = 6;
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.HeaderText = "Production Progress";
            dataGridViewTextBoxColumn6.MinimumWidth = 6;
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // productsGrid
            // 
            productsGrid.AllowUserToAddRows = false;
            productsGrid.AllowUserToDeleteRows = false;
            productsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            productsGrid.ColumnHeadersHeight = 29;
            productsGrid.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn8, dataGridViewTextBoxColumn9, dataGridViewTextBoxColumn10 });
            productsGrid.Location = new Point(20, 250);
            productsGrid.Name = "productsGrid";
            productsGrid.ReadOnly = true;
            productsGrid.RowHeadersWidth = 51;
            productsGrid.Size = new Size(600, 200);
            productsGrid.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn8
            // 
            dataGridViewTextBoxColumn8.HeaderText = "Product Type";
            dataGridViewTextBoxColumn8.MinimumWidth = 6;
            dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            dataGridViewTextBoxColumn9.HeaderText = "Quantity";
            dataGridViewTextBoxColumn9.MinimumWidth = 6;
            dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            dataGridViewTextBoxColumn10.HeaderText = "Price";
            dataGridViewTextBoxColumn10.MinimumWidth = 6;
            dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // cashLabel
            // 
            cashLabel.Font = new Font("Arial", 12F, FontStyle.Bold);
            cashLabel.Location = new Point(650, 20);
            cashLabel.Name = "cashLabel";
            cashLabel.Size = new Size(300, 30);
            cashLabel.TabIndex = 2;
            cashLabel.Text = "Cash: 0.00 TL";
            // 
            // buyAnimalButton
            // 
            buyAnimalButton.Font = new Font("Arial", 10F);
            buyAnimalButton.Location = new Point(650, 70);
            buyAnimalButton.Name = "buyAnimalButton";
            buyAnimalButton.Size = new Size(120, 40);
            buyAnimalButton.TabIndex = 3;
            buyAnimalButton.Text = "Buy Animal";
            buyAnimalButton.Click += BuyAnimalButton_Click;
            // 
            // sellProductsButton
            // 
            sellProductsButton.Font = new Font("Arial", 10F);
            sellProductsButton.Location = new Point(650, 220);
            sellProductsButton.Name = "sellProductsButton";
            sellProductsButton.Size = new Size(120, 40);
            sellProductsButton.TabIndex = 8;
            sellProductsButton.Text = "Sell Products";
            sellProductsButton.Click += SellProductsButton_Click;
            // 
            // resetGameButton
            // 
            resetGameButton.BackColor = Color.OrangeRed;
            resetGameButton.Font = new Font("Arial", 10F);
            resetGameButton.Location = new Point(650, 270);
            resetGameButton.Name = "resetGameButton";
            resetGameButton.Size = new Size(120, 40);
            resetGameButton.TabIndex = 10;
            resetGameButton.Text = "Reset Game";
            resetGameButton.UseVisualStyleBackColor = false;
            resetGameButton.Click += ResetGameButton_Click;
            // 
            // simulationTimer
            // 
            simulationTimer.Interval = 1000;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(808, 478);
            Controls.Add(animalsGrid);
            Controls.Add(productsGrid);
            Controls.Add(cashLabel);
            Controls.Add(buyAnimalButton);
            Controls.Add(sellProductsButton);
            Controls.Add(resetGameButton);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Farm Simulation";
            ((System.ComponentModel.ISupportInitialize)animalsGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)productsGrid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView animalsGrid;
        private DataGridView productsGrid;
        private Label cashLabel;
        private Button buyAnimalButton;
        private Button sellProductsButton;
        private Button resetGameButton;
        private System.Windows.Forms.Timer simulationTimer;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewProgressBarColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
    }
}