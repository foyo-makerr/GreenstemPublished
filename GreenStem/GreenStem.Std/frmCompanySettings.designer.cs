namespace GreenStem.Std
{
    partial class frmCompanySettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCompanySettings));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnItemSave = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemClose = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemNew = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemDelete = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnGetAvailableServer = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.btnItemSave,
            this.btnItemClose,
            this.btnItemNew,
            this.btnItemDelete});
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
            this.ribbon.Size = new System.Drawing.Size(686, 158);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            this.ribbon.Toolbar.ShowCustomizeItem = false;
            // 
            // btnItemSave
            // 
            this.btnItemSave.Caption = "Save";
            this.btnItemSave.Id = 1;
            this.btnItemSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemSave.ImageOptions.Image")));
            this.btnItemSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemSave.ImageOptions.LargeImage")));
            this.btnItemSave.Name = "btnItemSave";
            this.btnItemSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnItemSave_ItemClick);
            // 
            // btnItemClose
            // 
            this.btnItemClose.Caption = "Exit";
            this.btnItemClose.Id = 2;
            this.btnItemClose.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemClose.ImageOptions.Image")));
            this.btnItemClose.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemClose.ImageOptions.LargeImage")));
            this.btnItemClose.Name = "btnItemClose";
            // 
            // btnItemNew
            // 
            this.btnItemNew.Caption = "New";
            this.btnItemNew.Id = 3;
            this.btnItemNew.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemNew.ImageOptions.Image")));
            this.btnItemNew.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemNew.ImageOptions.LargeImage")));
            this.btnItemNew.Name = "btnItemNew";
            this.btnItemNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnItemNew_ItemClick);
            // 
            // btnItemDelete
            // 
            this.btnItemDelete.Caption = "Delete";
            this.btnItemDelete.Id = 4;
            this.btnItemDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemDelete.ImageOptions.Image")));
            this.btnItemDelete.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemDelete.ImageOptions.LargeImage")));
            this.btnItemDelete.Name = "btnItemDelete";
            this.btnItemDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnItemDelete_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btnItemNew);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnItemSave);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnItemDelete);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnItemClose);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 605);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(686, 24);
            // 
            // txtServerName
            // 
            this.txtServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerName.BackColor = System.Drawing.Color.Honeydew;
            this.txtServerName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerName.Location = new System.Drawing.Point(92, 180);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(310, 22);
            this.txtServerName.TabIndex = 536;
            this.txtServerName.TextChanged += new System.EventHandler(this.txtServerName_TextChanged);
            this.txtServerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtServerName_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 535;
            this.label2.Text = "Server Name:";
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(0, 235);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.MenuManager = this.ribbon;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(686, 364);
            this.gridControl1.TabIndex = 539;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // btnGetAvailableServer
            // 
            this.btnGetAvailableServer.Location = new System.Drawing.Point(439, 184);
            this.btnGetAvailableServer.Name = "btnGetAvailableServer";
            this.btnGetAvailableServer.Size = new System.Drawing.Size(153, 23);
            this.btnGetAvailableServer.TabIndex = 542;
            this.btnGetAvailableServer.Text = "Get Available Server";
            this.btnGetAvailableServer.UseVisualStyleBackColor = true;
            this.btnGetAvailableServer.Click += new System.EventHandler(this.btnGetAvailableServer_Click);
            // 
            // frmCompanySettings
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 629);
            this.Controls.Add(this.btnGetAvailableServer);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.txtServerName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("frmCompanySettings.IconOptions.Image")));
            this.Name = "frmCompanySettings";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "frmCompanySettings";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        internal System.Windows.Forms.TextBox txtServerName;
        internal System.Windows.Forms.Label label2;
        private DevExpress.XtraBars.BarButtonItem btnItemSave;
        private DevExpress.XtraBars.BarButtonItem btnItemClose;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraBars.BarButtonItem btnItemNew;
        private DevExpress.XtraBars.BarButtonItem btnItemDelete;
        private System.Windows.Forms.Button btnGetAvailableServer;
    }
}