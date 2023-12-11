namespace LauncherV2;

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
        tableLayoutPanel3 = new TableLayoutPanel();
        changeLocationButton = new Button();
        startCrpgButton = new Button();
        platformComboBox1 = new ComboBox();
        label1 = new Label();
        darkModecheckBox1 = new CheckBox();
        devModeCheckBox = new CheckBox();
        close = new Label();
        minimize = new Label();
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
        tableLayoutPanel1.Location = new Point(0, 18);
        tableLayoutPanel1.Margin = new Padding(4);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 2;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 127F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.Size = new Size(874, 383);
        tableLayoutPanel1.TabIndex = 0;
        // 
        // ConsoleTextBox
        // 
        ConsoleTextBox.Location = new Point(4, 131);
        ConsoleTextBox.Margin = new Padding(4);
        ConsoleTextBox.Multiline = true;
        ConsoleTextBox.Name = "ConsoleTextBox";
        ConsoleTextBox.Size = new Size(679, 247);
        ConsoleTextBox.TabIndex = 3;
        ConsoleTextBox.TextChanged += textBox2_TextChanged;
        // 
        // tableLayoutPanel2
        // 
        tableLayoutPanel2.ColumnCount = 1;
        tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel2.Controls.Add(verifyGameFilesButton, 0, 0);
        tableLayoutPanel2.Controls.Add(UpdateOrInstallButton, 0, 1);
        tableLayoutPanel2.Location = new Point(691, 131);
        tableLayoutPanel2.Margin = new Padding(4);
        tableLayoutPanel2.Name = "tableLayoutPanel2";
        tableLayoutPanel2.RowCount = 2;
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        tableLayoutPanel2.Size = new Size(179, 248);
        tableLayoutPanel2.TabIndex = 4;
        // 
        // verifyGameFilesButton
        // 
        verifyGameFilesButton.Location = new Point(4, 4);
        verifyGameFilesButton.Margin = new Padding(4);
        verifyGameFilesButton.Name = "verifyGameFilesButton";
        verifyGameFilesButton.Size = new Size(171, 116);
        verifyGameFilesButton.TabIndex = 5;
        verifyGameFilesButton.Text = "Verify Game Files";
        verifyGameFilesButton.UseVisualStyleBackColor = false;
        verifyGameFilesButton.EnabledChanged += buttonEnableStatusChange;
        verifyGameFilesButton.Click += VerifyGameFilesButton_Click_1;
        // 
        // UpdateOrInstallButton
        // 
        UpdateOrInstallButton.Location = new Point(4, 128);
        UpdateOrInstallButton.Margin = new Padding(4);
        UpdateOrInstallButton.Name = "UpdateOrInstallButton";
        UpdateOrInstallButton.Size = new Size(171, 116);
        UpdateOrInstallButton.TabIndex = 6;
        UpdateOrInstallButton.Text = "Check For Update";
        UpdateOrInstallButton.UseVisualStyleBackColor = false;
        UpdateOrInstallButton.Click += UpdateOrInstallButton_Click;
        // 
        // tableLayoutPanel4
        // 
        tableLayoutPanel4.ColumnCount = 1;
        tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel4.Controls.Add(locationTextBox, 0, 1);
        tableLayoutPanel4.Location = new Point(4, 4);
        tableLayoutPanel4.Margin = new Padding(4);
        tableLayoutPanel4.Name = "tableLayoutPanel4";
        tableLayoutPanel4.RowCount = 2;
        tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        tableLayoutPanel4.Size = new Size(679, 119);
        tableLayoutPanel4.TabIndex = 6;
        tableLayoutPanel4.Paint += tableLayoutPanel4_Paint;
        // 
        // locationTextBox
        // 
        locationTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        locationTextBox.Location = new Point(4, 85);
        locationTextBox.Margin = new Padding(4);
        locationTextBox.Name = "locationTextBox";
        locationTextBox.ReadOnly = true;
        locationTextBox.Size = new Size(671, 26);
        locationTextBox.TabIndex = 0;
        locationTextBox.TextChanged += textBox1_TextChanged;
        // 
        // tableLayoutPanel3
        // 
        tableLayoutPanel3.ColumnCount = 1;
        tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel3.Controls.Add(changeLocationButton, 0, 2);
        tableLayoutPanel3.Controls.Add(startCrpgButton, 0, 0);
        tableLayoutPanel3.Controls.Add(platformComboBox1, 0, 1);
        tableLayoutPanel3.Location = new Point(691, 4);
        tableLayoutPanel3.Margin = new Padding(4);
        tableLayoutPanel3.Name = "tableLayoutPanel3";
        tableLayoutPanel3.RowCount = 3;
        tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));
        tableLayoutPanel3.Size = new Size(179, 119);
        tableLayoutPanel3.TabIndex = 5;
        // 
        // changeLocationButton
        // 
        changeLocationButton.Location = new Point(4, 84);
        changeLocationButton.Margin = new Padding(4);
        changeLocationButton.Name = "changeLocationButton";
        changeLocationButton.Size = new Size(171, 30);
        changeLocationButton.TabIndex = 2;
        changeLocationButton.Text = "Change Location";
        changeLocationButton.UseVisualStyleBackColor = false;
        changeLocationButton.Click += ChangeLocationButton_Click;
        // 
        // startCrpgButton
        // 
        startCrpgButton.Location = new Point(4, 4);
        startCrpgButton.Margin = new Padding(4);
        startCrpgButton.Name = "startCrpgButton";
        startCrpgButton.Size = new Size(171, 29);
        startCrpgButton.TabIndex = 7;
        startCrpgButton.Text = "Start cRPG";
        startCrpgButton.UseVisualStyleBackColor = false;
        startCrpgButton.Click += StartCrpgButton_Click;
        // 
        // platformComboBox1
        // 
        platformComboBox1.Anchor = AnchorStyles.None;
        platformComboBox1.FormattingEnabled = true;
        platformComboBox1.Location = new Point(4, 45);
        platformComboBox1.Margin = new Padding(4);
        platformComboBox1.Name = "platformComboBox1";
        platformComboBox1.Size = new Size(170, 27);
        platformComboBox1.TabIndex = 8;
        platformComboBox1.SelectedIndexChanged += platformComboBox1_SelectedIndexChanged;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(1, -1);
        label1.Margin = new Padding(4, 0, 4, 0);
        label1.Name = "label1";
        label1.Size = new Size(106, 19);
        label1.TabIndex = 1;
        label1.Text = "cRPG Launcher";
        // 
        // darkModecheckBox1
        // 
        darkModecheckBox1.AutoSize = true;
        darkModecheckBox1.Location = new Point(2, 405);
        darkModecheckBox1.Name = "darkModecheckBox1";
        darkModecheckBox1.Size = new Size(96, 23);
        darkModecheckBox1.TabIndex = 2;
        darkModecheckBox1.Text = "DarkMode";
        darkModecheckBox1.UseVisualStyleBackColor = true;
        darkModecheckBox1.CheckedChanged += checkBox1_CheckedChanged;
        // 
        // devModeCheckBox
        // 
        devModeCheckBox.AutoSize = true;
        devModeCheckBox.Location = new Point(776, 405);
        devModeCheckBox.Name = "devModeCheckBox";
        devModeCheckBox.Size = new Size(90, 23);
        devModeCheckBox.TabIndex = 3;
        devModeCheckBox.Text = "DevMode";
        devModeCheckBox.UseVisualStyleBackColor = true;
        devModeCheckBox.CheckedChanged += checkBox2_CheckedChanged;
        // 
        // close
        // 
        close.AutoSize = true;
        close.Font = new Font("Times New Roman", 16F);
        close.Location = new Point(851, -7);
        close.Name = "close";
        close.Size = new Size(22, 25);
        close.TabIndex = 4;
        close.Text = "x";
        close.Click += close_Click;
        // 
        // minimize
        // 
        minimize.AutoSize = true;
        minimize.Font = new Font("Times New Roman", 14F);
        minimize.Location = new Point(830, -5);
        minimize.Name = "minimize";
        minimize.Size = new Size(20, 21);
        minimize.TabIndex = 5;
        minimize.Text = "–";
        minimize.Click += minimize_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(9F, 19F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(874, 425);
        Controls.Add(minimize);
        Controls.Add(close);
        Controls.Add(devModeCheckBox);
        Controls.Add(darkModecheckBox1);
        Controls.Add(label1);
        Controls.Add(tableLayoutPanel1);
        Font = new Font("Times New Roman", 12F);
        Margin = new Padding(4);
        Name = "Form1";
        Text = "cRPG Launcher";
        Load += Form1_Load;
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        tableLayoutPanel2.ResumeLayout(false);
        tableLayoutPanel4.ResumeLayout(false);
        tableLayoutPanel4.PerformLayout();
        tableLayoutPanel3.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private TableLayoutPanel tableLayoutPanel1;
    private TextBox locationTextBox;
    private TextBox ConsoleTextBox;
    private TableLayoutPanel tableLayoutPanel2;
    private Button verifyGameFilesButton;
    private TableLayoutPanel tableLayoutPanel3;
    private TableLayoutPanel tableLayoutPanel4;
    private Button changeLocationButton;
    private Button UpdateOrInstallButton;
    private Button startCrpgButton;
    private ComboBox platformComboBox1;
    private Label label1;
    private CheckBox darkModecheckBox1;
    private CheckBox devModeCheckBox;
    private Label close;
    private Label minimize;
}
