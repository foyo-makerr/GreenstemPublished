namespace GreenStem.LookUp
{
    partial class frmLookUpSettings
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLookUpSettings));
            this.gSTSTDDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lookupSettingDetailTblBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.btnSave = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.label1 = new System.Windows.Forms.Label();
            this.SettingLookUp = new System.Windows.Forms.PictureBox();
            this.lookUpName = new System.Windows.Forms.Label();
            this.Description = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.tableName = new System.Windows.Forms.Label();
            this.txtTableName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboReturnField = new System.Windows.Forms.ComboBox();
            this.cboOrderby = new System.Windows.Forms.ComboBox();
            this.cbosortType = new System.Windows.Forms.ComboBox();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.chkTrimStock = new System.Windows.Forms.CheckBox();
            this.chkMultiSelect = new System.Windows.Forms.CheckBox();
            this.txtLookUpName = new System.Windows.Forms.TextBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.btnForeColor = new System.Windows.Forms.Button();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtFontSize = new System.Windows.Forms.TextBox();
            this.txtFontStyle = new System.Windows.Forms.TextBox();
            this.txtFontName = new System.Windows.Forms.TextBox();
            this.btnFont = new System.Windows.Forms.Button();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_preview = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gSTSTDDataSetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookupSettingDetailTblBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingLookUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            this.panel1.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lookupSettingDetailTblBindingSource
            // 
            this.lookupSettingDetailTblBindingSource.DataMember = "LookupSetting_Detail_Tbl";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2,
            this.ribbonPageGroup3,
            this.ribbonPageGroup4});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btnAdd);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // btnAdd
            // 
            this.btnAdd.Caption = "Add";
            this.btnAdd.Id = 1;
            this.btnAdd.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.ImageOptions.Image")));
            this.btnAdd.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnAdd.ImageOptions.LargeImage")));
            this.btnAdd.Name = "btnAdd";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.btnSave);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // btnSave
            // 
            this.btnSave.Caption = "Save";
            this.btnSave.Id = 2;
            this.btnSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.ImageOptions.Image")));
            this.btnSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnSave.ImageOptions.LargeImage")));
            this.btnSave.Name = "btnSave";
            this.btnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSave_ItemClick);
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add(this.btnDelete);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "Delete";
            this.btnDelete.Id = 3;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
            this.btnDelete.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.LargeImage")));
            this.btnDelete.Name = "btnDelete";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.ItemLinks.Add(this.barButtonItem4);
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            // 
            // barButtonItem4
            // 
            this.barButtonItem4.Caption = "Exit";
            this.barButtonItem4.Id = 4;
            this.barButtonItem4.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem4.ImageOptions.Image")));
            this.barButtonItem4.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem4.ImageOptions.LargeImage")));
            this.barButtonItem4.Name = "barButtonItem4";
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.btnAdd,
            this.btnSave,
            this.btnDelete,
            this.barButtonItem4});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 5;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbon.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbon.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbon.ShowMoreCommandsButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbon.ShowPageHeadersInFormCaption = DevExpress.Utils.DefaultBoolean.False;
            this.ribbon.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            this.ribbon.ShowQatLocationSelector = false;
            this.ribbon.ShowToolbarCustomizeItem = false;
            this.ribbon.Size = new System.Drawing.Size(810, 158);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            this.ribbon.Toolbar.ShowCustomizeItem = false;
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 775);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(810, 24);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 222;
            this.label1.Text = "Code:";
            // 
            // SettingLookUp
            // 
            this.SettingLookUp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SettingLookUp.Image = global::GreenStem.LookUp.Properties.Resources.Search_24_256;
            this.SettingLookUp.Location = new System.Drawing.Point(109, 170);
            this.SettingLookUp.Name = "SettingLookUp";
            this.SettingLookUp.Size = new System.Drawing.Size(36, 25);
            this.SettingLookUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SettingLookUp.TabIndex = 223;
            this.SettingLookUp.TabStop = false;
            this.SettingLookUp.WaitOnLoad = true;
            this.SettingLookUp.Click += new System.EventHandler(this.SettingLookUp_Click);
            // 
            // lookUpName
            // 
            this.lookUpName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lookUpName.AutoSize = true;
            this.lookUpName.Location = new System.Drawing.Point(12, 220);
            this.lookUpName.Name = "lookUpName";
            this.lookUpName.Size = new System.Drawing.Size(76, 13);
            this.lookUpName.TabIndex = 225;
            this.lookUpName.Text = "LookUp Name:";
            // 
            // Description
            // 
            this.Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Description.AutoSize = true;
            this.Description.Location = new System.Drawing.Point(12, 265);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(64, 13);
            this.Description.TabIndex = 227;
            this.Description.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.BackColor = System.Drawing.Color.Honeydew;
            this.txtDescription.Location = new System.Drawing.Point(107, 101);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(354, 21);
            this.txtDescription.TabIndex = 228;
            // 
            // tableName
            // 
            this.tableName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableName.AutoSize = true;
            this.tableName.Location = new System.Drawing.Point(12, 309);
            this.tableName.Name = "tableName";
            this.tableName.Size = new System.Drawing.Size(67, 13);
            this.tableName.TabIndex = 229;
            this.tableName.Text = "Table Name:";
            // 
            // txtTableName
            // 
            this.txtTableName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTableName.BackColor = System.Drawing.Color.Honeydew;
            this.txtTableName.Location = new System.Drawing.Point(107, 142);
            this.txtTableName.Name = "txtTableName";
            this.txtTableName.Size = new System.Drawing.Size(354, 21);
            this.txtTableName.TabIndex = 230;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 369);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 231;
            this.label5.Text = "Remark:";
            // 
            // txtRemark
            // 
            this.txtRemark.AcceptsReturn = true;
            this.txtRemark.AllowDrop = true;
            this.txtRemark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemark.BackColor = System.Drawing.Color.Honeydew;
            this.txtRemark.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRemark.Location = new System.Drawing.Point(105, 183);
            this.txtRemark.MaxLength = 255;
            this.txtRemark.Multiline = true;
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRemark.Size = new System.Drawing.Size(356, 60);
            this.txtRemark.TabIndex = 232;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(12, 468);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(54, 13);
            this.Label4.TabIndex = 234;
            this.Label4.Text = "Order by:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 427);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 237;
            this.label2.Text = "Return Field:";
            // 
            // cboReturnField
            // 
            this.cboReturnField.BackColor = System.Drawing.Color.Honeydew;
            this.cboReturnField.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboReturnField.FormattingEnabled = true;
            this.cboReturnField.Location = new System.Drawing.Point(109, 263);
            this.cboReturnField.Name = "cboReturnField";
            this.cboReturnField.Size = new System.Drawing.Size(143, 22);
            this.cboReturnField.TabIndex = 236;
            // 
            // cboOrderby
            // 
            this.cboOrderby.FormattingEnabled = true;
            this.cboOrderby.Location = new System.Drawing.Point(111, 304);
            this.cboOrderby.Name = "cboOrderby";
            this.cboOrderby.Size = new System.Drawing.Size(141, 21);
            this.cboOrderby.TabIndex = 243;
            // 
            // cbosortType
            // 
            this.cbosortType.FormattingEnabled = true;
            this.cbosortType.Location = new System.Drawing.Point(279, 304);
            this.cbosortType.Name = "cbosortType";
            this.cbosortType.Size = new System.Drawing.Size(98, 21);
            this.cbosortType.TabIndex = 244;
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // gridControl1
            // 
            this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl1.Location = new System.Drawing.Point(0, 511);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.MenuManager = this.ribbon;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(810, 267);
            this.gridControl1.TabIndex = 247;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // chkTrimStock
            // 
            this.chkTrimStock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTrimStock.AutoSize = true;
            this.chkTrimStock.Checked = true;
            this.chkTrimStock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTrimStock.Location = new System.Drawing.Point(22, 259);
            this.chkTrimStock.Name = "chkTrimStock";
            this.chkTrimStock.Size = new System.Drawing.Size(86, 17);
            this.chkTrimStock.TabIndex = 214;
            this.chkTrimStock.Text = "Trim \'.\' Stock";
            this.chkTrimStock.UseVisualStyleBackColor = true;
            // 
            // chkMultiSelect
            // 
            this.chkMultiSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMultiSelect.AutoSize = true;
            this.chkMultiSelect.Location = new System.Drawing.Point(22, 283);
            this.chkMultiSelect.Name = "chkMultiSelect";
            this.chkMultiSelect.Size = new System.Drawing.Size(114, 17);
            this.chkMultiSelect.TabIndex = 215;
            this.chkMultiSelect.Text = "MultiSelect Lookup";
            this.chkMultiSelect.UseVisualStyleBackColor = true;
            // 
            // txtLookUpName
            // 
            this.txtLookUpName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLookUpName.BackColor = System.Drawing.Color.Honeydew;
            this.txtLookUpName.Location = new System.Drawing.Point(107, 53);
            this.txtLookUpName.Name = "txtLookUpName";
            this.txtLookUpName.Size = new System.Drawing.Size(356, 21);
            this.txtLookUpName.TabIndex = 226;
            // 
            // txtCode
            // 
            this.txtCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCode.BackColor = System.Drawing.Color.Honeydew;
            this.txtCode.Location = new System.Drawing.Point(151, 10);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(319, 21);
            this.txtCode.TabIndex = 253;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtCode);
            this.panel1.Controls.Add(this.txtLookUpName);
            this.panel1.Controls.Add(this.txtDescription);
            this.panel1.Controls.Add(this.cboOrderby);
            this.panel1.Controls.Add(this.cbosortType);
            this.panel1.Controls.Add(this.txtTableName);
            this.panel1.Controls.Add(this.txtRemark);
            this.panel1.Controls.Add(this.cboReturnField);
            this.panel1.Location = new System.Drawing.Point(0, 164);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(501, 341);
            this.panel1.TabIndex = 256;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.Controls.Add(this.btnForeColor);
            this.GroupBox1.Controls.Add(this.PictureBox1);
            this.GroupBox1.Controls.Add(this.txtFontSize);
            this.GroupBox1.Controls.Add(this.txtFontStyle);
            this.GroupBox1.Controls.Add(this.txtFontName);
            this.GroupBox1.Controls.Add(this.btnFont);
            this.GroupBox1.Controls.Add(this.Label14);
            this.GroupBox1.Controls.Add(this.Label13);
            this.GroupBox1.Controls.Add(this.Label12);
            this.GroupBox1.Controls.Add(this.Label11);
            this.GroupBox1.Location = new System.Drawing.Point(22, 40);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(225, 199);
            this.GroupBox1.TabIndex = 250;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Font";
            // 
            // btnForeColor
            // 
            this.btnForeColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnForeColor.Location = new System.Drawing.Point(182, 96);
            this.btnForeColor.Name = "btnForeColor";
            this.btnForeColor.Size = new System.Drawing.Size(30, 24);
            this.btnForeColor.TabIndex = 39;
            this.btnForeColor.Text = "...";
            this.btnForeColor.UseVisualStyleBackColor = true;
            this.btnForeColor.Click += new System.EventHandler(this.btnForeColor_Click);
            // 
            // PictureBox1
            // 
            this.PictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureBox1.BackColor = System.Drawing.Color.Black;
            this.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureBox1.Location = new System.Drawing.Point(82, 96);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(100, 24);
            this.PictureBox1.TabIndex = 33;
            this.PictureBox1.TabStop = false;
            // 
            // txtFontSize
            // 
            this.txtFontSize.Enabled = false;
            this.txtFontSize.Location = new System.Drawing.Point(82, 71);
            this.txtFontSize.Name = "txtFontSize";
            this.txtFontSize.Size = new System.Drawing.Size(100, 21);
            this.txtFontSize.TabIndex = 38;
            // 
            // txtFontStyle
            // 
            this.txtFontStyle.Enabled = false;
            this.txtFontStyle.Location = new System.Drawing.Point(82, 46);
            this.txtFontStyle.Name = "txtFontStyle";
            this.txtFontStyle.Size = new System.Drawing.Size(100, 21);
            this.txtFontStyle.TabIndex = 37;
            // 
            // txtFontName
            // 
            this.txtFontName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFontName.Enabled = false;
            this.txtFontName.Location = new System.Drawing.Point(82, 21);
            this.txtFontName.Name = "txtFontName";
            this.txtFontName.Size = new System.Drawing.Size(100, 21);
            this.txtFontName.TabIndex = 36;
            // 
            // btnFont
            // 
            this.btnFont.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFont.Location = new System.Drawing.Point(182, 20);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(30, 24);
            this.btnFont.TabIndex = 35;
            this.btnFont.Text = "...";
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Location = new System.Drawing.Point(6, 101);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(57, 13);
            this.Label14.TabIndex = 31;
            this.Label14.Text = "Fore Color";
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Location = new System.Drawing.Point(6, 73);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(51, 13);
            this.Label13.TabIndex = 30;
            this.Label13.Text = "Font Size";
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Location = new System.Drawing.Point(6, 45);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(56, 13);
            this.Label12.TabIndex = 28;
            this.Label12.Text = "Font Style";
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Location = new System.Drawing.Point(6, 21);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(59, 13);
            this.Label11.TabIndex = 26;
            this.Label11.Text = "Font Name";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btn_preview);
            this.panel2.Controls.Add(this.chkMultiSelect);
            this.panel2.Controls.Add(this.GroupBox1);
            this.panel2.Controls.Add(this.chkTrimStock);
            this.panel2.Location = new System.Drawing.Point(516, 151);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(259, 330);
            this.panel2.TabIndex = 257;
            // 
            // btn_preview
            // 
            this.btn_preview.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_preview.ImageOptions.SvgImage")));
            this.btn_preview.Location = new System.Drawing.Point(22, 0);
            this.btn_preview.Name = "btn_preview";
            this.btn_preview.Size = new System.Drawing.Size(114, 34);
            this.btn_preview.TabIndex = 251;
            this.btn_preview.Text = "Test Preview";
            this.btn_preview.Click += new System.EventHandler(this.btn_preview_Click);
            // 
            // frmLookUpSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 799);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tableName);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.lookUpName);
            this.Controls.Add(this.SettingLookUp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.ribbon);
            this.Name = "frmLookUpSettings";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "LookUpSettings";
            ((System.ComponentModel.ISupportInitialize)(this.gSTSTDDataSetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookupSettingDetailTblBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingLookUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.BindingSource gSTSTDDataSetBindingSource;
        private System.Windows.Forms.BindingSource lookupSettingDetailTblBindingSource;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem btnAdd;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem btnSave;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.BarButtonItem barButtonItem4;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox SettingLookUp;

        private System.Windows.Forms.Label lookUpName;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label tableName;
        private System.Windows.Forms.TextBox txtTableName;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox txtRemark;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.ComboBox cboReturnField;
        private System.Windows.Forms.ComboBox cboOrderby;
        private System.Windows.Forms.ComboBox cbosortType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        internal System.Windows.Forms.CheckBox chkTrimStock;
        internal System.Windows.Forms.CheckBox chkMultiSelect;
        private System.Windows.Forms.TextBox txtLookUpName;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Button btnForeColor;
        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.TextBox txtFontSize;
        internal System.Windows.Forms.TextBox txtFontStyle;
        internal System.Windows.Forms.TextBox txtFontName;
        internal System.Windows.Forms.Button btnFont;
        internal System.Windows.Forms.Label Label14;
        internal System.Windows.Forms.Label Label13;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.Label Label11;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.SimpleButton btn_preview;
    }
}