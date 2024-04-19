namespace GreenStem
{
    partial class frmSalesmanForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSalesmanForm));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnSave = new DevExpress.XtraBars.BarButtonItem();
            this.btnSaveClose = new DevExpress.XtraBars.BarButtonItem();
            this.btnResetChange = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.btnClose = new DevExpress.XtraBars.BarButtonItem();
            this.btnClear = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtDays = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCreditLimit = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSalesTarget = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkMobile = new System.Windows.Forms.CheckBox();
            this.txtCommision = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.masterSalesmanLookUp = new System.Windows.Forms.PictureBox();
            this.txtMasterSalesman = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSalesmanCode = new System.Windows.Forms.TextBox();
            this.txtContact = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSalesmanName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.salesManLookUp = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterSalesmanLookUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesManLookUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.btnSave,
            this.btnSaveClose,
            this.btnResetChange,
            this.btnDelete,
            this.btnClose,
            this.btnClear});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 7;
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
            this.ribbon.Size = new System.Drawing.Size(644, 158);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            this.ribbon.Toolbar.ShowCustomizeItem = false;
            // 
            // btnSave
            // 
            this.btnSave.Caption = "Save";
            this.btnSave.Id = 1;
            this.btnSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.ImageOptions.Image")));
            this.btnSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnSave.ImageOptions.LargeImage")));
            this.btnSave.Name = "btnSave";
            this.btnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSave_ItemClick);
            // 
            // btnSaveClose
            // 
            this.btnSaveClose.Caption = "Save And Close";
            this.btnSaveClose.Id = 2;
            this.btnSaveClose.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveClose.ImageOptions.Image")));
            this.btnSaveClose.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnSaveClose.ImageOptions.LargeImage")));
            this.btnSaveClose.Name = "btnSaveClose";
            this.btnSaveClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSaveClose_ItemClick);
            // 
            // btnResetChange
            // 
            this.btnResetChange.Caption = "Reset Change";
            this.btnResetChange.Id = 3;
            this.btnResetChange.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnResetChange.ImageOptions.Image")));
            this.btnResetChange.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnResetChange.ImageOptions.LargeImage")));
            this.btnResetChange.Name = "btnResetChange";
            this.btnResetChange.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnResetChange_ItemClick);
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "Delete";
            this.btnDelete.Id = 4;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
            this.btnDelete.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.LargeImage")));
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_ItemClick);
            // 
            // btnClose
            // 
            this.btnClose.Caption = "Close";
            this.btnClose.Id = 5;
            this.btnClose.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.ImageOptions.Image")));
            this.btnClose.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnClose.ImageOptions.LargeImage")));
            this.btnClose.Name = "btnClose";
            this.btnClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnClose_ItemClick);
            // 
            // btnClear
            // 
            this.btnClear.Caption = "Clear";
            this.btnClear.Id = 6;
            this.btnClear.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.ImageOptions.Image")));
            this.btnClear.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnClear.ImageOptions.LargeImage")));
            this.btnClear.Name = "btnClear";
            this.btnClear.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnClear_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.btnSave);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnSaveClose);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Save";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.btnResetChange);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnClear);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Reset";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add(this.btnDelete);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "Delete";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.ItemLinks.Add(this.btnClose);
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            this.ribbonPageGroup4.Text = "Close";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 646);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(644, 24);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.BackColor = System.Drawing.Color.Honeydew;
            this.txtPassword.Location = new System.Drawing.Point(123, 513);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(498, 21);
            this.txtPassword.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(26, 521);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 62;
            this.label9.Text = "Password:";
            // 
            // txtDays
            // 
            this.txtDays.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDays.BackColor = System.Drawing.Color.Honeydew;
            this.txtDays.Location = new System.Drawing.Point(123, 468);
            this.txtDays.Name = "txtDays";
            this.txtDays.Size = new System.Drawing.Size(498, 21);
            this.txtDays.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(29, 476);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 60;
            this.label8.Text = "Days:";
            // 
            // txtCreditLimit
            // 
            this.txtCreditLimit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCreditLimit.BackColor = System.Drawing.Color.Honeydew;
            this.txtCreditLimit.Location = new System.Drawing.Point(124, 429);
            this.txtCreditLimit.Name = "txtCreditLimit";
            this.txtCreditLimit.Size = new System.Drawing.Size(497, 21);
            this.txtCreditLimit.TabIndex = 8;
            this.txtCreditLimit.Text = "0.00";
            this.txtCreditLimit.Enter += new System.EventHandler(this.txtCreditLimit_Enter);
            this.txtCreditLimit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCreditLimit_KeyPress);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 437);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 58;
            this.label7.Text = "Credit Limit:";
            // 
            // txtSalesTarget
            // 
            this.txtSalesTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSalesTarget.BackColor = System.Drawing.Color.Honeydew;
            this.txtSalesTarget.Location = new System.Drawing.Point(123, 377);
            this.txtSalesTarget.Name = "txtSalesTarget";
            this.txtSalesTarget.Size = new System.Drawing.Size(498, 21);
            this.txtSalesTarget.TabIndex = 7;
            this.txtSalesTarget.Text = "0.00";
            this.txtSalesTarget.Enter += new System.EventHandler(this.txtSalesTarget_Enter);
            this.txtSalesTarget.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSalesTarget_KeyPress);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 385);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 56;
            this.label6.Text = "Sales Target:";
            // 
            // chkMobile
            // 
            this.chkMobile.AutoSize = true;
            this.chkMobile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkMobile.Location = new System.Drawing.Point(195, 201);
            this.chkMobile.Name = "chkMobile";
            this.chkMobile.Size = new System.Drawing.Size(56, 17);
            this.chkMobile.TabIndex = 3;
            this.chkMobile.Text = "Mobile";
            this.chkMobile.UseVisualStyleBackColor = true;
            // 
            // txtCommision
            // 
            this.txtCommision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommision.BackColor = System.Drawing.Color.Honeydew;
            this.txtCommision.Location = new System.Drawing.Point(123, 323);
            this.txtCommision.Name = "txtCommision";
            this.txtCommision.Size = new System.Drawing.Size(498, 21);
            this.txtCommision.TabIndex = 6;
            this.txtCommision.Text = "0.00";
            this.txtCommision.Enter += new System.EventHandler(this.txtCommision_Enter);
            this.txtCommision.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCommision_KeyPress);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 331);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 53;
            this.label5.Text = "Commision%:";
            // 
            // masterSalesmanLookUp
            // 
            this.masterSalesmanLookUp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.masterSalesmanLookUp.Image = ((System.Drawing.Image)(resources.GetObject("masterSalesmanLookUp.Image")));
            this.masterSalesmanLookUp.Location = new System.Drawing.Point(123, 568);
            this.masterSalesmanLookUp.Name = "masterSalesmanLookUp";
            this.masterSalesmanLookUp.Size = new System.Drawing.Size(38, 25);
            this.masterSalesmanLookUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.masterSalesmanLookUp.TabIndex = 52;
            this.masterSalesmanLookUp.TabStop = false;
            this.masterSalesmanLookUp.WaitOnLoad = true;
            this.masterSalesmanLookUp.Click += new System.EventHandler(this.masterSalesmanLookUp_Click);
            // 
            // txtMasterSalesman
            // 
            this.txtMasterSalesman.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMasterSalesman.BackColor = System.Drawing.Color.Honeydew;
            this.txtMasterSalesman.Location = new System.Drawing.Point(165, 572);
            this.txtMasterSalesman.Name = "txtMasterSalesman";
            this.txtMasterSalesman.Size = new System.Drawing.Size(456, 21);
            this.txtMasterSalesman.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 580);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 50;
            this.label4.Text = "Master Salesman:";
            // 
            // txtSalesmanCode
            // 
            this.txtSalesmanCode.AcceptsTab = true;
            this.txtSalesmanCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSalesmanCode.BackColor = System.Drawing.Color.Honeydew;
            this.txtSalesmanCode.Location = new System.Drawing.Point(165, 175);
            this.txtSalesmanCode.Name = "txtSalesmanCode";
            this.txtSalesmanCode.Size = new System.Drawing.Size(456, 21);
            this.txtSalesmanCode.TabIndex = 1;
            this.txtSalesmanCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtSalesmanCode_Validating);
            // 
            // txtContact
            // 
            this.txtContact.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContact.BackColor = System.Drawing.Color.Honeydew;
            this.txtContact.Location = new System.Drawing.Point(123, 270);
            this.txtContact.Name = "txtContact";
            this.txtContact.Size = new System.Drawing.Size(498, 21);
            this.txtContact.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 273);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 47;
            this.label3.Text = "Contact No:";
            // 
            // txtSalesmanName
            // 
            this.txtSalesmanName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSalesmanName.BackColor = System.Drawing.Color.Honeydew;
            this.txtSalesmanName.Location = new System.Drawing.Point(123, 224);
            this.txtSalesmanName.Name = "txtSalesmanName";
            this.txtSalesmanName.Size = new System.Drawing.Size(498, 21);
            this.txtSalesmanName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 45;
            this.label2.Text = "Name:";
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkActive.Location = new System.Drawing.Point(123, 201);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(56, 17);
            this.chkActive.TabIndex = 2;
            this.chkActive.Text = "Active";
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // salesManLookUp
            // 
            this.salesManLookUp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.salesManLookUp.Image = ((System.Drawing.Image)(resources.GetObject("salesManLookUp.Image")));
            this.salesManLookUp.Location = new System.Drawing.Point(123, 170);
            this.salesManLookUp.Name = "salesManLookUp";
            this.salesManLookUp.Size = new System.Drawing.Size(38, 25);
            this.salesManLookUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.salesManLookUp.TabIndex = 43;
            this.salesManLookUp.TabStop = false;
            this.salesManLookUp.WaitOnLoad = true;
            this.salesManLookUp.Click += new System.EventHandler(this.salesManLookUp_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 175);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "SalesMan Code";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(104, 170);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(13, 13);
            this.label10.TabIndex = 66;
            this.label10.Text = "*";
            // 
            // frmSalesmanForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 670);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtMasterSalesman);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtDays);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtCreditLimit);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtSalesTarget);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtCommision);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.masterSalesmanLookUp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtContact);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSalesmanName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.salesManLookUp);
            this.Controls.Add(this.chkMobile);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.txtSalesmanCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "frmSalesmanForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Salesman Form";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterSalesmanLookUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesManLookUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtDays;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCreditLimit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSalesTarget;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkMobile;
        private System.Windows.Forms.TextBox txtCommision;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox masterSalesmanLookUp;
        private System.Windows.Forms.TextBox txtMasterSalesman;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSalesmanCode;
        private System.Windows.Forms.TextBox txtContact;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSalesmanName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.PictureBox salesManLookUp;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraBars.BarButtonItem btnSave;
        private DevExpress.XtraBars.BarButtonItem btnSaveClose;
        private DevExpress.XtraBars.BarButtonItem btnResetChange;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.BarButtonItem btnClose;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.BarButtonItem btnClear;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label10;
    }
}