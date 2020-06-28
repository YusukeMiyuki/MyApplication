namespace OutDocChapter
{
    partial class MyRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public MyRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.MyTab = this.Factory.CreateRibbonTab();
            this.grpOutputBtn = this.Factory.CreateRibbonGroup();
            this.btnOutChapter = this.Factory.CreateRibbonButton();
            this.MyTab.SuspendLayout();
            this.grpOutputBtn.SuspendLayout();
            this.SuspendLayout();
            // 
            // MyTab
            // 
            this.MyTab.Groups.Add(this.grpOutputBtn);
            this.MyTab.Label = "出力とか";
            this.MyTab.Name = "MyTab";
            // 
            // grpOutputBtn
            // 
            this.grpOutputBtn.Items.Add(this.btnOutChapter);
            this.grpOutputBtn.Label = "出力系";
            this.grpOutputBtn.Name = "grpOutputBtn";
            // 
            // btnOutChapter
            // 
            this.btnOutChapter.Label = "章構成出力";
            this.btnOutChapter.Name = "btnOutChapter";
            this.btnOutChapter.ScreenTip = "現在の章構成をExcelに出力します。";
            this.btnOutChapter.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnOutChapter_Click);
            // 
            // MyRibbon
            // 
            this.Name = "MyRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.MyTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.myRibbon_Load);
            this.MyTab.ResumeLayout(false);
            this.MyTab.PerformLayout();
            this.grpOutputBtn.ResumeLayout(false);
            this.grpOutputBtn.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab MyTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpOutputBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnOutChapter;
    }

    partial class ThisRibbonCollection
    {
        internal MyRibbon MyRibbon
        {
            get { return this.GetRibbon<MyRibbon>(); }
        }
    }
}
