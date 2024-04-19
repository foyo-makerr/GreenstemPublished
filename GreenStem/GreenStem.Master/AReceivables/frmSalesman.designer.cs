namespace GreenStem.Master
{
    partial class frmSalesman
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSalesman));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnItemNew = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemSave = new DevExpress.XtraBars.BarButtonItem();
            this.q = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemDelete = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemFirst = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemPrevious = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemNext = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemLast = new DevExpress.XtraBars.BarButtonItem();
            this.btnItemExit = new DevExpress.XtraBars.BarButtonItem();
            this.btnEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btnFirst = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gvSalesman = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSalesman)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.btnItemNew,
            this.btnItemSave,
            this.q,
            this.btnItemDelete,
            this.btnItemFirst,
            this.btnItemPrevious,
            this.btnItemNext,
            this.btnItemLast,
            this.btnItemExit,
            this.btnEdit,
            this.btnFirst});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 13;
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
            this.ribbon.Size = new System.Drawing.Size(1087, 158);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            this.ribbon.Toolbar.ShowCustomizeItem = false;
            // 
            // btnItemNew
            // 
            this.btnItemNew.Caption = "Add";
            this.btnItemNew.Id = 1;
            this.btnItemNew.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemNew.ImageOptions.Image")));
            this.btnItemNew.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemNew.ImageOptions.LargeImage")));
            this.btnItemNew.Name = "btnItemNew";
            this.btnItemNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnItemNew_ItemClick);
            // 
            // btnItemSave
            // 
            this.btnItemSave.Caption = "Save";
            this.btnItemSave.Id = 2;
            this.btnItemSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemSave.ImageOptions.Image")));
            this.btnItemSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemSave.ImageOptions.LargeImage")));
            this.btnItemSave.Name = "btnItemSave";
            // 
            // q
            // 
            this.q.Caption = "Save And Print";
            this.q.Hint = "q";
            this.q.Id = 3;
            this.q.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("q.ImageOptions.Image")));
            this.q.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("q.ImageOptions.LargeImage")));
            this.q.Name = "q";
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
            // btnItemFirst
            // 
            this.btnItemFirst.Caption = "First";
            this.btnItemFirst.Id = 5;
            this.btnItemFirst.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemFirst.ImageOptions.Image")));
            this.btnItemFirst.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemFirst.ImageOptions.LargeImage")));
            this.btnItemFirst.Name = "btnItemFirst";
            // 
            // btnItemPrevious
            // 
            this.btnItemPrevious.Caption = "Prev";
            this.btnItemPrevious.Id = 6;
            this.btnItemPrevious.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemPrevious.ImageOptions.Image")));
            this.btnItemPrevious.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemPrevious.ImageOptions.LargeImage")));
            this.btnItemPrevious.Name = "btnItemPrevious";
            // 
            // btnItemNext
            // 
            this.btnItemNext.Caption = "Next";
            this.btnItemNext.Id = 7;
            this.btnItemNext.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemNext.ImageOptions.Image")));
            this.btnItemNext.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemNext.ImageOptions.LargeImage")));
            this.btnItemNext.Name = "btnItemNext";
            // 
            // btnItemLast
            // 
            this.btnItemLast.Caption = "Last";
            this.btnItemLast.Id = 8;
            this.btnItemLast.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemLast.ImageOptions.Image")));
            this.btnItemLast.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemLast.ImageOptions.LargeImage")));
            this.btnItemLast.Name = "btnItemLast";
            // 
            // btnItemExit
            // 
            this.btnItemExit.Caption = "Exit";
            this.btnItemExit.Id = 9;
            this.btnItemExit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnItemExit.ImageOptions.Image")));
            this.btnItemExit.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnItemExit.ImageOptions.LargeImage")));
            this.btnItemExit.Name = "btnItemExit";
            // 
            // btnEdit
            // 
            this.btnEdit.Caption = "Edit";
            this.btnEdit.Id = 11;
            this.btnEdit.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnEdit.ImageOptions.SvgImage")));
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnEdit_ItemClick);
            // 
            // btnFirst
            // 
            this.btnFirst.Caption = "First";
            this.btnFirst.Id = 12;
            this.btnFirst.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnFirst.ImageOptions.Image")));
            this.btnFirst.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnFirst.ImageOptions.LargeImage")));
            this.btnFirst.Name = "btnFirst";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2,
            this.ribbonPageGroup4,
            this.ribbonPageGroup5});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btnItemNew);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnItemSave);
            this.ribbonPageGroup1.ItemLinks.Add(this.q);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnEdit);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Action";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.btnItemDelete);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Delete";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.ItemLinks.Add(this.btnFirst);
            this.ribbonPageGroup4.ItemLinks.Add(this.btnItemPrevious);
            this.ribbonPageGroup4.ItemLinks.Add(this.btnItemNext);
            this.ribbonPageGroup4.ItemLinks.Add(this.btnItemLast);
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            this.ribbonPageGroup4.Text = "Control";
            // 
            // ribbonPageGroup5
            // 
            this.ribbonPageGroup5.ItemLinks.Add(this.btnItemExit, true);
            this.ribbonPageGroup5.Name = "ribbonPageGroup5";
            this.ribbonPageGroup5.Text = "Exit";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 694);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1087, 24);
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 158);
            this.gridControl1.MainView = this.gvSalesman;
            this.gridControl1.MenuManager = this.ribbon;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1087, 536);
            this.gridControl1.TabIndex = 34;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvSalesman});
            // 
            // gvSalesman
            // 
            this.gvSalesman.GridControl = this.gridControl1;
            this.gvSalesman.Name = "gvSalesman";
            this.gvSalesman.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            // 
            // frmSalesman
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 718);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "frmSalesman";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Salesman";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSalesman)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem btnItemNew;
        private DevExpress.XtraBars.BarButtonItem btnItemSave;
        private DevExpress.XtraBars.BarButtonItem q;
        private DevExpress.XtraBars.BarButtonItem btnItemDelete;
        private DevExpress.XtraBars.BarButtonItem btnItemFirst;
        private DevExpress.XtraBars.BarButtonItem btnItemPrevious;
        private DevExpress.XtraBars.BarButtonItem btnItemNext;
        private DevExpress.XtraBars.BarButtonItem btnItemLast;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem btnItemExit;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gvSalesman;
        private DevExpress.XtraBars.BarButtonItem btnEdit;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
       
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.BarButtonItem btnFirst;
    }
}