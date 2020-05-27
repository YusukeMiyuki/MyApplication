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
            this.tabTest = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.tabTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabTest
            // 
            this.tabTest.Groups.Add(this.group1);
            this.tabTest.Label = "tab1";
            this.tabTest.Name = "tabTest";
            // 
            // group1
            // 
            this.group1.Label = "group1";
            this.group1.Name = "group1";
            // 
            // MyRibbon
            // 
            this.Name = "MyRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tabTest);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.MyRibbon_Load);
            this.tabTest.ResumeLayout(false);
            this.tabTest.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabTest;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
    }

    partial class ThisRibbonCollection
    {
        internal MyRibbon MyRibbon
        {
            get { return this.GetRibbon<MyRibbon>(); }
        }
    }
}
