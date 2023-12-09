namespace LaucherV2;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        tableLayoutPanel1 = new TableLayoutPanel();
        ConsoleTextBox = new TextBox();
        tableLayoutPanel2 = new TableLayoutPanel();
        verifyGameFilesButton = new Button();
        UpdateOrInstallButton = new Button();
        tableLayoutPanel4 = new TableLayoutPanel();
        locationTextBox = new TextBox();
        progressBar1 = new ProgressBar();
        tableLayoutPanel3 = new TableLayoutPanel();
        changeLocationButton = new Button();
        startCrpgButton = new Button();
        comboBox1 = new ComboBox();
        tableLayoutPanel1.SuspendLayout();
        tableLayoutPanel2.SuspendLayout();
        tableLayoutPanel4.SuspendLayout();
        tableLayoutPanel3.SuspendLayout();
        SuspendLayout();
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 2;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 78.6783F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.3216972F));
        tableLayoutPanel1.Controls.Add(ConsoleTextBox, 0, 1);
        tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 1);
        tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 0);
        tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 0);
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 2;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.Size = new Size(680, 302);
        tableLayoutPanel1.TabIndex = 0;
        // 
        // ConsoleTextBox
        // 
        ConsoleTextBox.Location = new Point(3, 103);
        ConsoleTextBox.Multiline = true;
        ConsoleTextBox.Name = "ConsoleTextBox";
        ConsoleTextBox.Size = new Size(529, 196);
        ConsoleTextBox.TabIndex = 3;
        ConsoleTextBox.TextChanged += textBox2_TextChanged;
        // 
        // tableLayoutPanel2
        // 
        tableLayoutPanel2.ColumnCount = 1;
        tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel2.Controls.Add(verifyGameFilesButton, 0, 0);
        tableLayoutPanel2.Controls.Add(UpdateOrInstallButton, 0, 1);
        tableLayoutPanel2.Location = new Point(538, 103);
        tableLayoutPanel2.Name = "tableLayoutPanel2";
        tableLayoutPanel2.RowCount = 2;
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tableLayoutPanel2.Size = new Size(139, 196);
        tableLayoutPanel2.TabIndex = 4;
        // 
        // verifyGameFilesButton
        // 
        verifyGameFilesButton.Location = new Point(3, 3);
        verifyGameFilesButton.Name = "verifyGameFilesButton";
        verifyGameFilesButton.Size = new Size(133, 92);
        verifyGameFilesButton.TabIndex = 5;
        verifyGameFilesButton.Text = "Verify Game Files";
        verifyGameFilesButton.UseVisualStyleBackColor = true;
        verifyGameFilesButton.Click += VerifyGameFilesButton_Click_1;
        // 
        // UpdateOrInstallButton
        // 
        UpdateOrInstallButton.Location = new Point(3, 101);
        UpdateOrInstallButton.Name = "UpdateOrInstallButton";
        UpdateOrInstallButton.Size = new Size(133, 92);
        UpdateOrInstallButton.TabIndex = 6;
        UpdateOrInstallButton.Text = "Check For Update";
        UpdateOrInstallButton.UseVisualStyleBackColor = true;
        UpdateOrInstallButton.Click += UpdateOrInstallButton_Click;
        // 
        // tableLayoutPanel4
        // 
        tableLayoutPanel4.ColumnCount = 1;
        tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel4.Controls.Add(locationTextBox, 0, 1);
        tableLayoutPanel4.Controls.Add(progressBar1, 0, 0);
        tableLayoutPanel4.Location = new Point(3, 3);
        tableLayoutPanel4.Name = "tableLayoutPanel4";
        tableLayoutPanel4.RowCount = 2;
        tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        tableLayoutPanel4.Size = new Size(529, 94);
        tableLayoutPanel4.TabIndex = 6;
        // 
        // locationTextBox
        // 
        locationTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        locationTextBox.Location = new Point(3, 67);
        locationTextBox.Name = "locationTextBox";
        locationTextBox.ReadOnly = true;
        locationTextBox.Size = new Size(523, 23);
        locationTextBox.TabIndex = 0;
        locationTextBox.TextChanged += textBox1_TextChanged;
        // 
        // progressBar1
        // 
        progressBar1.Location = new Point(3, 3);
        progressBar1.Name = "progressBar1";
        progressBar1.Size = new Size(523, 58);
        progressBar1.TabIndex = 1;
        // 
        // tableLayoutPanel3
        // 
        tableLayoutPanel3.ColumnCount = 1;
        tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel3.Controls.Add(changeLocationButton, 0, 2);
        tableLayoutPanel3.Controls.Add(startCrpgButton, 0, 0);
        tableLayoutPanel3.Controls.Add(comboBox1, 0, 1);
        tableLayoutPanel3.Location = new Point(538, 3);
        tableLayoutPanel3.Name = "tableLayoutPanel3";
        tableLayoutPanel3.RowCount = 3;
        tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
        tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        tableLayoutPanel3.Size = new Size(139, 94);
        tableLayoutPanel3.TabIndex = 5;
        // 
        // changeLocationButton
        // 
        changeLocationButton.Location = new Point(3, 66);
        changeLocationButton.Name = "changeLocationButton";
        changeLocationButton.Size = new Size(133, 24);
        changeLocationButton.TabIndex = 2;
        changeLocationButton.Text = "Change Location";
        changeLocationButton.UseVisualStyleBackColor = true;
        changeLocationButton.Click += ChangeLocationButton_Click;
        // 
        // startCrpgButton
        // 
        startCrpgButton.Location = new Point(3, 3);
        startCrpgButton.Name = "startCrpgButton";
        startCrpgButton.Size = new Size(133, 23);
        startCrpgButton.TabIndex = 7;
        startCrpgButton.Text = "Start cRPG";
        startCrpgButton.UseVisualStyleBackColor = true;
        startCrpgButton.Click += StartCrpgButton_Click;
        // 
        // comboBox1
        // 
        comboBox1.FormattingEnabled = true;
        comboBox1.Location = new Point(3, 33);
        comboBox1.Name = "comboBox1";
        comboBox1.Size = new Size(133, 23);
        comboBox1.TabIndex = 8;
        comboBox1.SelectedIndexChanged += platformComboBox1_SelectedIndexChanged;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(680, 300);
        Controls.Add(tableLayoutPanel1);
        Name = "Form1";
        Text = "Form1";
        Load += Form1_Load;
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        tableLayoutPanel2.ResumeLayout(false);
        tableLayoutPanel4.ResumeLayout(false);
        tableLayoutPanel4.PerformLayout();
        tableLayoutPanel3.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion
    private TableLayoutPanel tableLayoutPanel1;
    private TextBox locationTextBox;
    private ProgressBar progressBar1;
    private TextBox ConsoleTextBox;
    private TableLayoutPanel tableLayoutPanel2;
    private Button verifyGameFilesButton;
    private TableLayoutPanel tableLayoutPanel3;
    private TableLayoutPanel tableLayoutPanel4;
    private Button changeLocationButton;
    private Button UpdateOrInstallButton;
    private Button startCrpgButton;
    private ComboBox comboBox1;
}
