namespace RulerJB
{
	partial class FormBookMain
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBookMain));
			this.btnExit = new System.Windows.Forms.Button();
			this.dataGridViewMain = new System.Windows.Forms.DataGridView();
			this.PageIdx = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.clValid = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.folder1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.clFname = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.heightPL = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.heightGL = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.heightPR = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.heightGR = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.clChanged = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.clBlank = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.contextMenuGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.クリップボードにコピーToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.カーソル選択データの実行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnManualAdjust = new System.Windows.Forms.Button();
			this.btnOutCsv = new System.Windows.Forms.Button();
			this.menuStripMain = new System.Windows.Forms.MenuStrip();
			this.ファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.プロジェクトファイルの読み込みToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.プロジェクトファイル保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.画像データの読み込みToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.新規画像フォルダToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.新規画像ManifestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.rOI設定読み込みToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rOI設定保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.テスト計測カレント実行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ツールToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.データ設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.環境設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ヘルプToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.バージョンToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.labelDisplayRate = new System.Windows.Forms.Label();
			this.labelPixelForMm = new System.Windows.Forms.Label();
			this.labelPageCount = new System.Windows.Forms.Label();
			this.grpbxFormPix = new System.Windows.Forms.GroupBox();
			this.rbtnMmFromPerMm = new System.Windows.Forms.RadioButton();
			this.rbtnMmFromPaperH = new System.Windows.Forms.RadioButton();
			this.rbtnPx = new System.Windows.Forms.RadioButton();
			this.contextMenuPbox = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.データ指定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.画像を別のウィンドウで表示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rOI調整ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rOIサイズ合わせToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rOI高さ位置合わせToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rOI高さ位置合わせ一括左側優先ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rOI高さ位置合わせ一括右側優先ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rOI左右位置合わせToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rOI垂直モードToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rOI高さ連動モードToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.検出ライン表示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.紙左側ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.匡郭左側ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.紙右側ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.匡郭右側ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pictureBoxMain = new System.Windows.Forms.PictureBox();
			this.splitContainerMain = new System.Windows.Forms.SplitContainer();
			this.cboxMeasPoint = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.btnExec = new System.Windows.Forms.Button();
			this.labelPointInfo = new System.Windows.Forms.Label();
			this.labelCurInfo = new System.Windows.Forms.Label();
			this.cboxRoiInfo = new System.Windows.Forms.CheckBox();
			this.btnAutoSize = new System.Windows.Forms.Button();
			this.labelImageInf = new System.Windows.Forms.Label();
			this.grpFormat = new System.Windows.Forms.GroupBox();
			this.rbtnType1 = new System.Windows.Forms.RadioButton();
			this.rbtnType0 = new System.Windows.Forms.RadioButton();
			this.btnResetRoi = new System.Windows.Forms.Button();
			this.cboxBinMethod = new System.Windows.Forms.CheckBox();
			this.cboxMeasurmentMatch = new System.Windows.Forms.CheckBox();
			this.cboxDetectSel = new System.Windows.Forms.CheckBox();
			this.cboxDetectLineAll = new System.Windows.Forms.CheckBox();
			this.cboxOldMethod = new System.Windows.Forms.CheckBox();
			this.radioButtonROIFree = new System.Windows.Forms.RadioButton();
			this.radioButtonROIVertical = new System.Windows.Forms.RadioButton();
			this.radioButtonROIHorizon = new System.Windows.Forms.RadioButton();
			this.groupBoxROIMove = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewMain)).BeginInit();
			this.contextMenuGrid.SuspendLayout();
			this.menuStripMain.SuspendLayout();
			this.grpbxFormPix.SuspendLayout();
			this.contextMenuPbox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
			this.splitContainerMain.Panel1.SuspendLayout();
			this.splitContainerMain.Panel2.SuspendLayout();
			this.splitContainerMain.SuspendLayout();
			this.grpFormat.SuspendLayout();
			this.groupBoxROIMove.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnExit
			// 
			this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExit.Location = new System.Drawing.Point(1133, 558);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(115, 51);
			this.btnExit.TabIndex = 0;
			this.btnExit.Text = "終了";
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// dataGridViewMain
			// 
			this.dataGridViewMain.AllowUserToAddRows = false;
			this.dataGridViewMain.AllowUserToDeleteRows = false;
			this.dataGridViewMain.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewMain.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridViewMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewMain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PageIdx,
            this.clValid,
            this.folder1,
            this.clFname,
            this.heightPL,
            this.heightGL,
            this.heightPR,
            this.heightGR,
            this.clChanged,
            this.clBlank});
			this.dataGridViewMain.ContextMenuStrip = this.contextMenuGrid;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridViewMain.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridViewMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewMain.Location = new System.Drawing.Point(5, 0);
			this.dataGridViewMain.Name = "dataGridViewMain";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewMain.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dataGridViewMain.RowHeadersVisible = false;
			this.dataGridViewMain.RowTemplate.Height = 21;
			this.dataGridViewMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewMain.Size = new System.Drawing.Size(545, 400);
			this.dataGridViewMain.TabIndex = 2;
			this.dataGridViewMain.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMain_CellClick);
			this.dataGridViewMain.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMain_CellValueChanged);
			this.dataGridViewMain.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewMain_CurrentCellDirtyStateChanged);
			this.dataGridViewMain.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMain_RowEnter);
			// 
			// PageIdx
			// 
			this.PageIdx.HeaderText = "No.";
			this.PageIdx.Name = "PageIdx";
			this.PageIdx.ReadOnly = true;
			this.PageIdx.Width = 30;
			// 
			// clValid
			// 
			this.clValid.HeaderText = "除外";
			this.clValid.Name = "clValid";
			this.clValid.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.clValid.Width = 38;
			// 
			// folder1
			// 
			this.folder1.HeaderText = "フォルダ１";
			this.folder1.Name = "folder1";
			this.folder1.Width = 150;
			// 
			// clFname
			// 
			this.clFname.HeaderText = "ファイル名";
			this.clFname.MinimumWidth = 86;
			this.clFname.Name = "clFname";
			this.clFname.ReadOnly = true;
			this.clFname.Width = 150;
			// 
			// heightPL
			// 
			this.heightPL.HeaderText = "L紙高";
			this.heightPL.Name = "heightPL";
			this.heightPL.ReadOnly = true;
			this.heightPL.Width = 65;
			// 
			// heightGL
			// 
			this.heightGL.HeaderText = "L匡郭高";
			this.heightGL.Name = "heightGL";
			this.heightGL.ReadOnly = true;
			this.heightGL.Width = 75;
			// 
			// heightPR
			// 
			this.heightPR.HeaderText = "R紙高";
			this.heightPR.Name = "heightPR";
			this.heightPR.Width = 65;
			// 
			// heightGR
			// 
			this.heightGR.HeaderText = "R匡郭高";
			this.heightGR.Name = "heightGR";
			this.heightGR.Width = 75;
			// 
			// clChanged
			// 
			this.clChanged.HeaderText = "変更";
			this.clChanged.Name = "clChanged";
			this.clChanged.ReadOnly = true;
			this.clChanged.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.clChanged.Width = 35;
			// 
			// clBlank
			// 
			this.clBlank.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.clBlank.HeaderText = "";
			this.clBlank.Name = "clBlank";
			// 
			// contextMenuGrid
			// 
			this.contextMenuGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.クリップボードにコピーToolStripMenuItem,
            this.toolStripSeparator5,
            this.カーソル選択データの実行ToolStripMenuItem});
			this.contextMenuGrid.Name = "contextMenuGrid";
			this.contextMenuGrid.Size = new System.Drawing.Size(195, 54);
			// 
			// クリップボードにコピーToolStripMenuItem
			// 
			this.クリップボードにコピーToolStripMenuItem.Name = "クリップボードにコピーToolStripMenuItem";
			this.クリップボードにコピーToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.クリップボードにコピーToolStripMenuItem.Text = "クリップボードにコピー";
			this.クリップボードにコピーToolStripMenuItem.Click += new System.EventHandler(this.クリップボードにコピーToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(191, 6);
			// 
			// カーソル選択データの実行ToolStripMenuItem
			// 
			this.カーソル選択データの実行ToolStripMenuItem.Name = "カーソル選択データの実行ToolStripMenuItem";
			this.カーソル選択データの実行ToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.カーソル選択データの実行ToolStripMenuItem.Text = "カーソル選択データの実行";
			this.カーソル選択データの実行ToolStripMenuItem.Click += new System.EventHandler(this.カーソル選択データの実行ToolStripMenuItem_Click);
			// 
			// btnManualAdjust
			// 
			this.btnManualAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnManualAdjust.Location = new System.Drawing.Point(548, 543);
			this.btnManualAdjust.Name = "btnManualAdjust";
			this.btnManualAdjust.Size = new System.Drawing.Size(101, 29);
			this.btnManualAdjust.TabIndex = 0;
			this.btnManualAdjust.Text = "測定点リセット";
			this.toolTip1.SetToolTip(this.btnManualAdjust, "未設定の測定点をリセットします。全測定点をリセットする場合はCTRLキーを押しながらクリックします。");
			this.btnManualAdjust.UseVisualStyleBackColor = true;
			this.btnManualAdjust.Click += new System.EventHandler(this.btnManualAdjust_Click);
			// 
			// btnOutCsv
			// 
			this.btnOutCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOutCsv.Location = new System.Drawing.Point(867, 558);
			this.btnOutCsv.Name = "btnOutCsv";
			this.btnOutCsv.Size = new System.Drawing.Size(115, 51);
			this.btnOutCsv.TabIndex = 0;
			this.btnOutCsv.Text = "CSV出力";
			this.btnOutCsv.UseVisualStyleBackColor = true;
			this.btnOutCsv.Click += new System.EventHandler(this.btnOutCsv_Click);
			// 
			// menuStripMain
			// 
			this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.ツールToolStripMenuItem,
            this.ヘルプToolStripMenuItem});
			this.menuStripMain.Location = new System.Drawing.Point(0, 0);
			this.menuStripMain.Name = "menuStripMain";
			this.menuStripMain.Size = new System.Drawing.Size(1278, 24);
			this.menuStripMain.TabIndex = 3;
			this.menuStripMain.Text = "menuStrip1";
			// 
			// ファイルToolStripMenuItem
			// 
			this.ファイルToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.プロジェクトファイルの読み込みToolStripMenuItem,
            this.プロジェクトファイル保存ToolStripMenuItem,
            this.toolStripSeparator4,
            this.画像データの読み込みToolStripMenuItem,
            this.新規画像フォルダToolStripMenuItem,
            this.新規画像ManifestToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripSeparator2,
            this.rOI設定読み込みToolStripMenuItem,
            this.rOI設定保存ToolStripMenuItem,
            this.toolStripSeparator3,
            this.テスト計測カレント実行ToolStripMenuItem,
            this.終了ToolStripMenuItem});
			this.ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
			this.ファイルToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.ファイルToolStripMenuItem.Text = "ファイル";
			// 
			// プロジェクトファイルの読み込みToolStripMenuItem
			// 
			this.プロジェクトファイルの読み込みToolStripMenuItem.Name = "プロジェクトファイルの読み込みToolStripMenuItem";
			this.プロジェクトファイルの読み込みToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.プロジェクトファイルの読み込みToolStripMenuItem.Text = "作業状態の復元";
			this.プロジェクトファイルの読み込みToolStripMenuItem.Click += new System.EventHandler(this.プロジェクトファイルの読み込みToolStripMenuItem_Click);
			// 
			// プロジェクトファイル保存ToolStripMenuItem
			// 
			this.プロジェクトファイル保存ToolStripMenuItem.Name = "プロジェクトファイル保存ToolStripMenuItem";
			this.プロジェクトファイル保存ToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.プロジェクトファイル保存ToolStripMenuItem.Text = "作業状態の保存";
			this.プロジェクトファイル保存ToolStripMenuItem.Click += new System.EventHandler(this.プロジェクトファイル保存ToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(214, 6);
			// 
			// 画像データの読み込みToolStripMenuItem
			// 
			this.画像データの読み込みToolStripMenuItem.Name = "画像データの読み込みToolStripMenuItem";
			this.画像データの読み込みToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.画像データの読み込みToolStripMenuItem.Text = "新規・画像ファイル";
			this.画像データの読み込みToolStripMenuItem.Click += new System.EventHandler(this.画像データの読み込みToolStripMenuItem_Click);
			// 
			// 新規画像フォルダToolStripMenuItem
			// 
			this.新規画像フォルダToolStripMenuItem.Name = "新規画像フォルダToolStripMenuItem";
			this.新規画像フォルダToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.新規画像フォルダToolStripMenuItem.Text = "新規・画像フォルダ";
			this.新規画像フォルダToolStripMenuItem.Click += new System.EventHandler(this.新規画像フォルダToolStripMenuItem_Click);
			// 
			// 新規画像ManifestToolStripMenuItem
			// 
			this.新規画像ManifestToolStripMenuItem.Name = "新規画像ManifestToolStripMenuItem";
			this.新規画像ManifestToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.新規画像ManifestToolStripMenuItem.Text = "新規・IIIF Manifest URI";
			this.新規画像ManifestToolStripMenuItem.Click += new System.EventHandler(this.新規画像URLToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(214, 6);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(214, 6);
			// 
			// rOI設定読み込みToolStripMenuItem
			// 
			this.rOI設定読み込みToolStripMenuItem.Name = "rOI設定読み込みToolStripMenuItem";
			this.rOI設定読み込みToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.rOI設定読み込みToolStripMenuItem.Text = "ROI設定読み込み";
			this.rOI設定読み込みToolStripMenuItem.Click += new System.EventHandler(this.rOI設定読み込みToolStripMenuItem_Click);
			// 
			// rOI設定保存ToolStripMenuItem
			// 
			this.rOI設定保存ToolStripMenuItem.Name = "rOI設定保存ToolStripMenuItem";
			this.rOI設定保存ToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.rOI設定保存ToolStripMenuItem.Text = "ROI設定保存";
			this.rOI設定保存ToolStripMenuItem.Click += new System.EventHandler(this.rOI設定保存ToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(214, 6);
			// 
			// テスト計測カレント実行ToolStripMenuItem
			// 
			this.テスト計測カレント実行ToolStripMenuItem.Name = "テスト計測カレント実行ToolStripMenuItem";
			this.テスト計測カレント実行ToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.テスト計測カレント実行ToolStripMenuItem.Text = "テスト計測（表示画像のみ）";
			this.テスト計測カレント実行ToolStripMenuItem.Click += new System.EventHandler(this.テスト計測カレント実行ToolStripMenuItem_Click);
			// 
			// 終了ToolStripMenuItem
			// 
			this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
			this.終了ToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.終了ToolStripMenuItem.Text = "終了";
			this.終了ToolStripMenuItem.Click += new System.EventHandler(this.終了ToolStripMenuItem_Click);
			// 
			// ツールToolStripMenuItem
			// 
			this.ツールToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.データ設定ToolStripMenuItem,
            this.環境設定ToolStripMenuItem});
			this.ツールToolStripMenuItem.Name = "ツールToolStripMenuItem";
			this.ツールToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.ツールToolStripMenuItem.Text = "ツール";
			// 
			// データ設定ToolStripMenuItem
			// 
			this.データ設定ToolStripMenuItem.Name = "データ設定ToolStripMenuItem";
			this.データ設定ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.データ設定ToolStripMenuItem.Text = "データ設定";
			this.データ設定ToolStripMenuItem.Click += new System.EventHandler(this.データ設定ToolStripMenuItem_Click);
			// 
			// 環境設定ToolStripMenuItem
			// 
			this.環境設定ToolStripMenuItem.Enabled = false;
			this.環境設定ToolStripMenuItem.Name = "環境設定ToolStripMenuItem";
			this.環境設定ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.環境設定ToolStripMenuItem.Text = "環境設定";
			// 
			// ヘルプToolStripMenuItem
			// 
			this.ヘルプToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.バージョンToolStripMenuItem});
			this.ヘルプToolStripMenuItem.Name = "ヘルプToolStripMenuItem";
			this.ヘルプToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.ヘルプToolStripMenuItem.Text = "ヘルプ";
			// 
			// バージョンToolStripMenuItem
			// 
			this.バージョンToolStripMenuItem.Name = "バージョンToolStripMenuItem";
			this.バージョンToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.バージョンToolStripMenuItem.Text = "バージョン";
			this.バージョンToolStripMenuItem.Click += new System.EventHandler(this.バージョンToolStripMenuItem_Click);
			// 
			// labelDisplayRate
			// 
			this.labelDisplayRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDisplayRate.AutoSize = true;
			this.labelDisplayRate.Location = new System.Drawing.Point(597, 454);
			this.labelDisplayRate.Name = "labelDisplayRate";
			this.labelDisplayRate.Size = new System.Drawing.Size(35, 12);
			this.labelDisplayRate.TabIndex = 5;
			this.labelDisplayRate.Text = "-----";
			// 
			// labelPixelForMm
			// 
			this.labelPixelForMm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPixelForMm.AutoSize = true;
			this.labelPixelForMm.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelPixelForMm.Location = new System.Drawing.Point(1064, 497);
			this.labelPixelForMm.Name = "labelPixelForMm";
			this.labelPixelForMm.Size = new System.Drawing.Size(168, 13);
			this.labelPixelForMm.TabIndex = 6;
			this.labelPixelForMm.Text = "1mm当たりPixel: ---.---";
			// 
			// labelPageCount
			// 
			this.labelPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPageCount.AutoSize = true;
			this.labelPageCount.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelPageCount.Location = new System.Drawing.Point(1064, 473);
			this.labelPageCount.Name = "labelPageCount";
			this.labelPageCount.Size = new System.Drawing.Size(84, 13);
			this.labelPageCount.TabIndex = 6;
			this.labelPageCount.Text = "画像数　---";
			// 
			// grpbxFormPix
			// 
			this.grpbxFormPix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.grpbxFormPix.Controls.Add(this.rbtnMmFromPerMm);
			this.grpbxFormPix.Controls.Add(this.rbtnMmFromPaperH);
			this.grpbxFormPix.Controls.Add(this.rbtnPx);
			this.grpbxFormPix.Location = new System.Drawing.Point(812, 454);
			this.grpbxFormPix.Name = "grpbxFormPix";
			this.grpbxFormPix.Size = new System.Drawing.Size(159, 94);
			this.grpbxFormPix.TabIndex = 7;
			this.grpbxFormPix.TabStop = false;
			this.grpbxFormPix.Text = "表示方式";
			// 
			// rbtnMmFromPerMm
			// 
			this.rbtnMmFromPerMm.AutoSize = true;
			this.rbtnMmFromPerMm.Location = new System.Drawing.Point(18, 62);
			this.rbtnMmFromPerMm.Name = "rbtnMmFromPerMm";
			this.rbtnMmFromPerMm.Size = new System.Drawing.Size(126, 16);
			this.rbtnMmFromPerMm.TabIndex = 1;
			this.rbtnMmFromPerMm.TabStop = true;
			this.rbtnMmFromPerMm.Text = "mm　（1mm当たりpx）";
			this.rbtnMmFromPerMm.UseVisualStyleBackColor = true;
			this.rbtnMmFromPerMm.CheckedChanged += new System.EventHandler(this.OnCheckedChanbeRbtn);
			// 
			// rbtnMmFromPaperH
			// 
			this.rbtnMmFromPaperH.AutoSize = true;
			this.rbtnMmFromPaperH.Location = new System.Drawing.Point(18, 40);
			this.rbtnMmFromPaperH.Name = "rbtnMmFromPaperH";
			this.rbtnMmFromPaperH.Size = new System.Drawing.Size(126, 16);
			this.rbtnMmFromPaperH.TabIndex = 1;
			this.rbtnMmFromPaperH.TabStop = true;
			this.rbtnMmFromPaperH.Text = "mm　（実測紙高より）";
			this.rbtnMmFromPaperH.UseVisualStyleBackColor = true;
			this.rbtnMmFromPaperH.CheckedChanged += new System.EventHandler(this.OnCheckedChanbeRbtn);
			// 
			// rbtnPx
			// 
			this.rbtnPx.AutoSize = true;
			this.rbtnPx.Location = new System.Drawing.Point(18, 19);
			this.rbtnPx.Name = "rbtnPx";
			this.rbtnPx.Size = new System.Drawing.Size(48, 16);
			this.rbtnPx.TabIndex = 0;
			this.rbtnPx.TabStop = true;
			this.rbtnPx.Text = "Pixel";
			this.rbtnPx.UseVisualStyleBackColor = true;
			this.rbtnPx.CheckedChanged += new System.EventHandler(this.OnCheckedChanbeRbtn);
			// 
			// contextMenuPbox
			// 
			this.contextMenuPbox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.データ指定ToolStripMenuItem,
            this.画像を別のウィンドウで表示ToolStripMenuItem,
            this.rOI調整ToolStripMenuItem,
            this.検出ライン表示ToolStripMenuItem});
			this.contextMenuPbox.Name = "contextMenuPbox";
			this.contextMenuPbox.Size = new System.Drawing.Size(206, 92);
			this.contextMenuPbox.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuPbox_Closed);
			this.contextMenuPbox.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuPbox_Opening);
			// 
			// データ指定ToolStripMenuItem
			// 
			this.データ指定ToolStripMenuItem.Name = "データ指定ToolStripMenuItem";
			this.データ指定ToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.データ指定ToolStripMenuItem.Text = "データ設定";
			this.データ指定ToolStripMenuItem.Click += new System.EventHandler(this.データ設定ToolStripMenuItem_Click);
			// 
			// 画像を別のウィンドウで表示ToolStripMenuItem
			// 
			this.画像を別のウィンドウで表示ToolStripMenuItem.Name = "画像を別のウィンドウで表示ToolStripMenuItem";
			this.画像を別のウィンドウで表示ToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.画像を別のウィンドウで表示ToolStripMenuItem.Text = "画像を別のウィンドウで表示";
			this.画像を別のウィンドウで表示ToolStripMenuItem.Click += new System.EventHandler(this.画像を別のウィンドウで表示ToolStripMenuItem_Click);
			// 
			// rOI調整ToolStripMenuItem
			// 
			this.rOI調整ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rOIサイズ合わせToolStripMenuItem,
            this.rOI高さ位置合わせToolStripMenuItem,
            this.rOI高さ位置合わせ一括左側優先ToolStripMenuItem,
            this.rOI高さ位置合わせ一括右側優先ToolStripMenuItem,
            this.rOI左右位置合わせToolStripMenuItem,
            this.rOI垂直モードToolStripMenuItem,
            this.rOI高さ連動モードToolStripMenuItem});
			this.rOI調整ToolStripMenuItem.Name = "rOI調整ToolStripMenuItem";
			this.rOI調整ToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.rOI調整ToolStripMenuItem.Text = "ROI調整";
			// 
			// rOIサイズ合わせToolStripMenuItem
			// 
			this.rOIサイズ合わせToolStripMenuItem.Name = "rOIサイズ合わせToolStripMenuItem";
			this.rOIサイズ合わせToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
			this.rOIサイズ合わせToolStripMenuItem.Text = "ROIサイズ合わせ";
			this.rOIサイズ合わせToolStripMenuItem.Click += new System.EventHandler(this.rOIサイズ合わせToolStripMenuItem_Click);
			// 
			// rOI高さ位置合わせToolStripMenuItem
			// 
			this.rOI高さ位置合わせToolStripMenuItem.Name = "rOI高さ位置合わせToolStripMenuItem";
			this.rOI高さ位置合わせToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
			this.rOI高さ位置合わせToolStripMenuItem.Text = "ROI上下位置合わせ";
			this.rOI高さ位置合わせToolStripMenuItem.Click += new System.EventHandler(this.rOI高さ位置合わせToolStripMenuItem_Click);
			// 
			// rOI高さ位置合わせ一括左側優先ToolStripMenuItem
			// 
			this.rOI高さ位置合わせ一括左側優先ToolStripMenuItem.Name = "rOI高さ位置合わせ一括左側優先ToolStripMenuItem";
			this.rOI高さ位置合わせ一括左側優先ToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
			this.rOI高さ位置合わせ一括左側優先ToolStripMenuItem.Text = "ROI上下位置合わせ（一括・左側優先）";
			this.rOI高さ位置合わせ一括左側優先ToolStripMenuItem.Click += new System.EventHandler(this.rOI高さ位置合わせ一括左側優先ToolStripMenuItem_Click);
			// 
			// rOI高さ位置合わせ一括右側優先ToolStripMenuItem
			// 
			this.rOI高さ位置合わせ一括右側優先ToolStripMenuItem.Name = "rOI高さ位置合わせ一括右側優先ToolStripMenuItem";
			this.rOI高さ位置合わせ一括右側優先ToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
			this.rOI高さ位置合わせ一括右側優先ToolStripMenuItem.Text = "ROI上下位置合わせ（一括・右側優先）";
			this.rOI高さ位置合わせ一括右側優先ToolStripMenuItem.Click += new System.EventHandler(this.rOI高さ位置合わせ一括右側優先ToolStripMenuItem_Click);
			// 
			// rOI左右位置合わせToolStripMenuItem
			// 
			this.rOI左右位置合わせToolStripMenuItem.Name = "rOI左右位置合わせToolStripMenuItem";
			this.rOI左右位置合わせToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
			this.rOI左右位置合わせToolStripMenuItem.Text = "ROI左右位置合わせ";
			this.rOI左右位置合わせToolStripMenuItem.Click += new System.EventHandler(this.rOI左右位置合わせToolStripMenuItem_Click);
			// 
			// rOI垂直モードToolStripMenuItem
			// 
			this.rOI垂直モードToolStripMenuItem.Checked = true;
			this.rOI垂直モードToolStripMenuItem.CheckOnClick = true;
			this.rOI垂直モードToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.rOI垂直モードToolStripMenuItem.Name = "rOI垂直モードToolStripMenuItem";
			this.rOI垂直モードToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
			this.rOI垂直モードToolStripMenuItem.Text = "ROI垂直モード";
			// 
			// rOI高さ連動モードToolStripMenuItem
			// 
			this.rOI高さ連動モードToolStripMenuItem.Checked = true;
			this.rOI高さ連動モードToolStripMenuItem.CheckOnClick = true;
			this.rOI高さ連動モードToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.rOI高さ連動モードToolStripMenuItem.Name = "rOI高さ連動モードToolStripMenuItem";
			this.rOI高さ連動モードToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
			this.rOI高さ連動モードToolStripMenuItem.Text = "ROI上下連動モード";
			// 
			// 検出ライン表示ToolStripMenuItem
			// 
			this.検出ライン表示ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.紙左側ToolStripMenuItem,
            this.匡郭左側ToolStripMenuItem,
            this.紙右側ToolStripMenuItem,
            this.匡郭右側ToolStripMenuItem});
			this.検出ライン表示ToolStripMenuItem.Name = "検出ライン表示ToolStripMenuItem";
			this.検出ライン表示ToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.検出ライン表示ToolStripMenuItem.Text = "検出ライン 追加/削除";
			// 
			// 紙左側ToolStripMenuItem
			// 
			this.紙左側ToolStripMenuItem.Checked = true;
			this.紙左側ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.紙左側ToolStripMenuItem.Name = "紙左側ToolStripMenuItem";
			this.紙左側ToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.紙左側ToolStripMenuItem.Text = "左側 - 紙";
			this.紙左側ToolStripMenuItem.Click += new System.EventHandler(this.紙左側ToolStripMenuItem_Click);
			// 
			// 匡郭左側ToolStripMenuItem
			// 
			this.匡郭左側ToolStripMenuItem.Checked = true;
			this.匡郭左側ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.匡郭左側ToolStripMenuItem.Name = "匡郭左側ToolStripMenuItem";
			this.匡郭左側ToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.匡郭左側ToolStripMenuItem.Text = "左側 - 匡郭";
			this.匡郭左側ToolStripMenuItem.Click += new System.EventHandler(this.匡郭左側ToolStripMenuItem_Click);
			// 
			// 紙右側ToolStripMenuItem
			// 
			this.紙右側ToolStripMenuItem.Checked = true;
			this.紙右側ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.紙右側ToolStripMenuItem.Name = "紙右側ToolStripMenuItem";
			this.紙右側ToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.紙右側ToolStripMenuItem.Text = "右側 - 紙";
			this.紙右側ToolStripMenuItem.Click += new System.EventHandler(this.紙右側ToolStripMenuItem_Click);
			// 
			// 匡郭右側ToolStripMenuItem
			// 
			this.匡郭右側ToolStripMenuItem.Checked = true;
			this.匡郭右側ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.匡郭右側ToolStripMenuItem.Name = "匡郭右側ToolStripMenuItem";
			this.匡郭右側ToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.匡郭右側ToolStripMenuItem.Text = "右側 - 匡郭";
			this.匡郭右側ToolStripMenuItem.Click += new System.EventHandler(this.匡郭右側ToolStripMenuItem_Click);
			// 
			// pictureBoxMain
			// 
			this.pictureBoxMain.BackColor = System.Drawing.Color.Black;
			this.pictureBoxMain.ContextMenuStrip = this.contextMenuPbox;
			this.pictureBoxMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBoxMain.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxMain.Name = "pictureBoxMain";
			this.pictureBoxMain.Size = new System.Drawing.Size(684, 400);
			this.pictureBoxMain.TabIndex = 1;
			this.pictureBoxMain.TabStop = false;
			this.pictureBoxMain.DoubleClick += new System.EventHandler(this.pictureBoxMain_DoubleClick);
			this.pictureBoxMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMain_MouseUp);
			// 
			// splitContainerMain
			// 
			this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainerMain.Location = new System.Drawing.Point(12, 38);
			this.splitContainerMain.Name = "splitContainerMain";
			// 
			// splitContainerMain.Panel1
			// 
			this.splitContainerMain.Panel1.Controls.Add(this.pictureBoxMain);
			this.splitContainerMain.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
			// 
			// splitContainerMain.Panel2
			// 
			this.splitContainerMain.Panel2.Controls.Add(this.dataGridViewMain);
			this.splitContainerMain.Panel2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.splitContainerMain.Size = new System.Drawing.Size(1245, 400);
			this.splitContainerMain.SplitterDistance = 689;
			this.splitContainerMain.SplitterWidth = 6;
			this.splitContainerMain.TabIndex = 14;
			// 
			// cboxMeasPoint
			// 
			this.cboxMeasPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboxMeasPoint.AutoSize = true;
			this.cboxMeasPoint.Checked = true;
			this.cboxMeasPoint.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cboxMeasPoint.Location = new System.Drawing.Point(47, 578);
			this.cboxMeasPoint.Name = "cboxMeasPoint";
			this.cboxMeasPoint.Size = new System.Drawing.Size(96, 16);
			this.cboxMeasPoint.TabIndex = 13;
			this.cboxMeasPoint.Text = "計測点（編集）";
			this.cboxMeasPoint.UseVisualStyleBackColor = true;
			this.cboxMeasPoint.CheckedChanged += new System.EventHandler(this.CheckedChange_DispDebug);
			// 
			// btnExec
			// 
			this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExec.Location = new System.Drawing.Point(687, 554);
			this.btnExec.Name = "btnExec";
			this.btnExec.Size = new System.Drawing.Size(148, 55);
			this.btnExec.TabIndex = 0;
			this.btnExec.Text = "自動計測";
			this.toolTip1.SetToolTip(this.btnExec, "自動計測を行います。(Ctrlキー同時押しで、手動・確定データも対象）");
			this.btnExec.UseVisualStyleBackColor = true;
			this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
			// 
			// labelPointInfo
			// 
			this.labelPointInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelPointInfo.AutoSize = true;
			this.labelPointInfo.Location = new System.Drawing.Point(23, 454);
			this.labelPointInfo.Name = "labelPointInfo";
			this.labelPointInfo.Size = new System.Drawing.Size(83, 12);
			this.labelPointInfo.TabIndex = 15;
			this.labelPointInfo.Text = "(-----, ----- )";
			// 
			// labelCurInfo
			// 
			this.labelCurInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelCurInfo.AutoSize = true;
			this.labelCurInfo.Location = new System.Drawing.Point(23, 477);
			this.labelCurInfo.Name = "labelCurInfo";
			this.labelCurInfo.Size = new System.Drawing.Size(29, 12);
			this.labelCurInfo.TabIndex = 15;
			this.labelCurInfo.Text = "----";
			// 
			// cboxRoiInfo
			// 
			this.cboxRoiInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboxRoiInfo.Location = new System.Drawing.Point(171, 578);
			this.cboxRoiInfo.Name = "cboxRoiInfo";
			this.cboxRoiInfo.Size = new System.Drawing.Size(84, 16);
			this.cboxRoiInfo.TabIndex = 13;
			this.cboxRoiInfo.Text = "ROI情報";
			this.cboxRoiInfo.UseVisualStyleBackColor = true;
			this.cboxRoiInfo.CheckedChanged += new System.EventHandler(this.CheckedChange_RoiInfo);
			// 
			// btnAutoSize
			// 
			this.btnAutoSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAutoSize.Location = new System.Drawing.Point(412, 494);
			this.btnAutoSize.Name = "btnAutoSize";
			this.btnAutoSize.Size = new System.Drawing.Size(101, 48);
			this.btnAutoSize.TabIndex = 16;
			this.btnAutoSize.Text = "全体表示";
			this.btnAutoSize.UseVisualStyleBackColor = true;
			this.btnAutoSize.Click += new System.EventHandler(this.btnAutoSize_Click);
			// 
			// labelImageInf
			// 
			this.labelImageInf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelImageInf.AutoSize = true;
			this.labelImageInf.Location = new System.Drawing.Point(448, 454);
			this.labelImageInf.Name = "labelImageInf";
			this.labelImageInf.Size = new System.Drawing.Size(29, 12);
			this.labelImageInf.TabIndex = 17;
			this.labelImageInf.Text = "----";
			// 
			// grpFormat
			// 
			this.grpFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.grpFormat.Controls.Add(this.rbtnType1);
			this.grpFormat.Controls.Add(this.rbtnType0);
			this.grpFormat.Location = new System.Drawing.Point(1000, 544);
			this.grpFormat.Name = "grpFormat";
			this.grpFormat.Size = new System.Drawing.Size(119, 70);
			this.grpFormat.TabIndex = 18;
			this.grpFormat.TabStop = false;
			this.grpFormat.Text = "Format";
			// 
			// rbtnType1
			// 
			this.rbtnType1.AutoSize = true;
			this.rbtnType1.Location = new System.Drawing.Point(6, 41);
			this.rbtnType1.Name = "rbtnType1";
			this.rbtnType1.Size = new System.Drawing.Size(110, 16);
			this.rbtnType1.TabIndex = 0;
			this.rbtnType1.Text = "TypeB　(左右別）";
			this.rbtnType1.UseVisualStyleBackColor = true;
			// 
			// rbtnType0
			// 
			this.rbtnType0.AutoSize = true;
			this.rbtnType0.Checked = true;
			this.rbtnType0.Location = new System.Drawing.Point(7, 19);
			this.rbtnType0.Name = "rbtnType0";
			this.rbtnType0.Size = new System.Drawing.Size(110, 16);
			this.rbtnType0.TabIndex = 0;
			this.rbtnType0.TabStop = true;
			this.rbtnType0.Text = "TypeA　(左右同）";
			this.rbtnType0.UseVisualStyleBackColor = true;
			// 
			// btnResetRoi
			// 
			this.btnResetRoi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnResetRoi.Location = new System.Drawing.Point(548, 578);
			this.btnResetRoi.Name = "btnResetRoi";
			this.btnResetRoi.Size = new System.Drawing.Size(101, 31);
			this.btnResetRoi.TabIndex = 0;
			this.btnResetRoi.Text = "ROI初期化";
			this.btnResetRoi.UseVisualStyleBackColor = true;
			this.btnResetRoi.Click += new System.EventHandler(this.btnResetRoi_Click);
			// 
			// cboxBinMethod
			// 
			this.cboxBinMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cboxBinMethod.AutoSize = true;
			this.cboxBinMethod.Checked = true;
			this.cboxBinMethod.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cboxBinMethod.Location = new System.Drawing.Point(687, 526);
			this.cboxBinMethod.Name = "cboxBinMethod";
			this.cboxBinMethod.Size = new System.Drawing.Size(84, 16);
			this.cboxBinMethod.TabIndex = 19;
			this.cboxBinMethod.Text = "Bin-Method";
			this.cboxBinMethod.UseVisualStyleBackColor = true;
			this.cboxBinMethod.Visible = false;
			// 
			// cboxMeasurmentMatch
			// 
			this.cboxMeasurmentMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboxMeasurmentMatch.Checked = true;
			this.cboxMeasurmentMatch.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cboxMeasurmentMatch.Location = new System.Drawing.Point(283, 578);
			this.cboxMeasurmentMatch.Name = "cboxMeasurmentMatch";
			this.cboxMeasurmentMatch.Size = new System.Drawing.Size(104, 16);
			this.cboxMeasurmentMatch.TabIndex = 13;
			this.cboxMeasurmentMatch.Text = "測定位置合わせ";
			this.cboxMeasurmentMatch.UseVisualStyleBackColor = true;
			this.cboxMeasurmentMatch.CheckedChanged += new System.EventHandler(this.CboxMeasurmentMatch_CheckedChanged);
			// 
			// cboxDetectSel
			// 
			this.cboxDetectSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboxDetectSel.AutoSize = true;
			this.cboxDetectSel.Location = new System.Drawing.Point(412, 552);
			this.cboxDetectSel.Name = "cboxDetectSel";
			this.cboxDetectSel.Size = new System.Drawing.Size(74, 16);
			this.cboxDetectSel.TabIndex = 12;
			this.cboxDetectSel.Text = "検出ライン";
			this.cboxDetectSel.UseVisualStyleBackColor = true;
			this.cboxDetectSel.Visible = false;
			this.cboxDetectSel.CheckedChanged += new System.EventHandler(this.CheckedChange_DispDebug);
			// 
			// cboxDetectLineAll
			// 
			this.cboxDetectLineAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboxDetectLineAll.AutoSize = true;
			this.cboxDetectLineAll.Location = new System.Drawing.Point(412, 578);
			this.cboxDetectLineAll.Name = "cboxDetectLineAll";
			this.cboxDetectLineAll.Size = new System.Drawing.Size(84, 16);
			this.cboxDetectLineAll.TabIndex = 13;
			this.cboxDetectLineAll.Text = "検出補助線";
			this.cboxDetectLineAll.UseVisualStyleBackColor = true;
			this.cboxDetectLineAll.Visible = false;
			this.cboxDetectLineAll.CheckedChanged += new System.EventHandler(this.CheckedChange_DispDebug);
			// 
			// cboxOldMethod
			// 
			this.cboxOldMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cboxOldMethod.AutoSize = true;
			this.cboxOldMethod.Location = new System.Drawing.Point(687, 497);
			this.cboxOldMethod.Name = "cboxOldMethod";
			this.cboxOldMethod.Size = new System.Drawing.Size(92, 16);
			this.cboxOldMethod.TabIndex = 19;
			this.cboxOldMethod.Text = "(Old-Method)";
			this.cboxOldMethod.UseVisualStyleBackColor = true;
			this.cboxOldMethod.Visible = false;
			// 
			// radioButtonROIFree
			// 
			this.radioButtonROIFree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonROIFree.AutoSize = true;
			this.radioButtonROIFree.Checked = true;
			this.radioButtonROIFree.Location = new System.Drawing.Point(15, 23);
			this.radioButtonROIFree.Name = "radioButtonROIFree";
			this.radioButtonROIFree.Size = new System.Drawing.Size(95, 16);
			this.radioButtonROIFree.TabIndex = 20;
			this.radioButtonROIFree.TabStop = true;
			this.radioButtonROIFree.Text = "上下左右移動";
			this.radioButtonROIFree.UseVisualStyleBackColor = true;
			// 
			// radioButtonROIVertical
			// 
			this.radioButtonROIVertical.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonROIVertical.AutoSize = true;
			this.radioButtonROIVertical.Location = new System.Drawing.Point(139, 23);
			this.radioButtonROIVertical.Name = "radioButtonROIVertical";
			this.radioButtonROIVertical.Size = new System.Drawing.Size(92, 16);
			this.radioButtonROIVertical.TabIndex = 21;
			this.radioButtonROIVertical.Text = "上下のみ移動";
			this.radioButtonROIVertical.UseVisualStyleBackColor = true;
			// 
			// radioButtonROIHorizon
			// 
			this.radioButtonROIHorizon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonROIHorizon.AutoSize = true;
			this.radioButtonROIHorizon.Location = new System.Drawing.Point(251, 23);
			this.radioButtonROIHorizon.Name = "radioButtonROIHorizon";
			this.radioButtonROIHorizon.Size = new System.Drawing.Size(92, 16);
			this.radioButtonROIHorizon.TabIndex = 22;
			this.radioButtonROIHorizon.Text = "左右のみ移動";
			this.radioButtonROIHorizon.UseVisualStyleBackColor = true;
			// 
			// groupBoxROIMove
			// 
			this.groupBoxROIMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBoxROIMove.Controls.Add(this.radioButtonROIHorizon);
			this.groupBoxROIMove.Controls.Add(this.radioButtonROIVertical);
			this.groupBoxROIMove.Controls.Add(this.radioButtonROIFree);
			this.groupBoxROIMove.Location = new System.Drawing.Point(32, 526);
			this.groupBoxROIMove.Name = "groupBoxROIMove";
			this.groupBoxROIMove.Size = new System.Drawing.Size(355, 45);
			this.groupBoxROIMove.TabIndex = 23;
			this.groupBoxROIMove.TabStop = false;
			this.groupBoxROIMove.Text = "ROI 操作方式";
			// 
			// FormBookMain
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1278, 621);
			this.Controls.Add(this.groupBoxROIMove);
			this.Controls.Add(this.cboxMeasurmentMatch);
			this.Controls.Add(this.cboxOldMethod);
			this.Controls.Add(this.cboxBinMethod);
			this.Controls.Add(this.grpFormat);
			this.Controls.Add(this.labelImageInf);
			this.Controls.Add(this.btnAutoSize);
			this.Controls.Add(this.labelCurInfo);
			this.Controls.Add(this.labelPointInfo);
			this.Controls.Add(this.splitContainerMain);
			this.Controls.Add(this.cboxMeasPoint);
			this.Controls.Add(this.cboxRoiInfo);
			this.Controls.Add(this.cboxDetectLineAll);
			this.Controls.Add(this.cboxDetectSel);
			this.Controls.Add(this.grpbxFormPix);
			this.Controls.Add(this.labelPageCount);
			this.Controls.Add(this.labelPixelForMm);
			this.Controls.Add(this.labelDisplayRate);
			this.Controls.Add(this.btnExec);
			this.Controls.Add(this.btnResetRoi);
			this.Controls.Add(this.btnManualAdjust);
			this.Controls.Add(this.btnOutCsv);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.menuStripMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStripMain;
			this.Name = "FormBookMain";
			this.Text = "メインフォーム";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBookMain_FormClosing);
			this.Load += new System.EventHandler(this.FormBookMain_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FormMain_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FormMain_DragEnter);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormBookMain_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormBookMain_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormBookMain_KeyUp);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewMain)).EndInit();
			this.contextMenuGrid.ResumeLayout(false);
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			this.grpbxFormPix.ResumeLayout(false);
			this.grpbxFormPix.PerformLayout();
			this.contextMenuPbox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
			this.splitContainerMain.Panel1.ResumeLayout(false);
			this.splitContainerMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
			this.splitContainerMain.ResumeLayout(false);
			this.grpFormat.ResumeLayout(false);
			this.grpFormat.PerformLayout();
			this.groupBoxROIMove.ResumeLayout(false);
			this.groupBoxROIMove.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.PictureBox pictureBoxMain;
		private System.Windows.Forms.DataGridView dataGridViewMain;
		private System.Windows.Forms.Button btnManualAdjust;
		private System.Windows.Forms.Button btnOutCsv;
		private System.Windows.Forms.MenuStrip menuStripMain;
		private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ツールToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ヘルプToolStripMenuItem;
		private System.Windows.Forms.Label labelDisplayRate;
		private System.Windows.Forms.Label labelPixelForMm;
		private System.Windows.Forms.Label labelPageCount;
		private System.Windows.Forms.GroupBox grpbxFormPix;
		private System.Windows.Forms.RadioButton rbtnMmFromPaperH;
		private System.Windows.Forms.RadioButton rbtnPx;
		private System.Windows.Forms.ToolStripMenuItem プロジェクトファイルの読み込みToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 画像データの読み込みToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 新規画像フォルダToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem プロジェクトファイル保存ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem 終了ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem データ設定ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem バージョンToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuPbox;
		private System.Windows.Forms.ToolStripMenuItem 画像を別のウィンドウで表示ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem データ指定ToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuGrid;
		private System.Windows.Forms.ToolStripMenuItem クリップボードにコピーToolStripMenuItem;
		private System.Windows.Forms.RadioButton rbtnMmFromPerMm;
		private System.Windows.Forms.SplitContainer splitContainerMain;
		private System.Windows.Forms.CheckBox cboxMeasPoint;
		private System.Windows.Forms.ToolStripMenuItem 環境設定ToolStripMenuItem;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label labelPointInfo;
		private System.Windows.Forms.Label labelCurInfo;
		private System.Windows.Forms.Button btnExec;
		private System.Windows.Forms.CheckBox cboxRoiInfo;
		private System.Windows.Forms.Button btnAutoSize;
		private System.Windows.Forms.Label labelImageInf;
		private System.Windows.Forms.GroupBox grpFormat;
		private System.Windows.Forms.RadioButton rbtnType1;
		private System.Windows.Forms.RadioButton rbtnType0;
		private System.Windows.Forms.Button btnResetRoi;
		private System.Windows.Forms.CheckBox cboxBinMethod;
		private System.Windows.Forms.ToolStripMenuItem rOI設定読み込みToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rOI設定保存ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem テスト計測カレント実行ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rOI調整ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rOIサイズ合わせToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rOI高さ位置合わせToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rOI左右位置合わせToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rOI垂直モードToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem カーソル選択データの実行ToolStripMenuItem;
		private System.Windows.Forms.CheckBox cboxMeasurmentMatch;
		private System.Windows.Forms.CheckBox cboxDetectSel;
		private System.Windows.Forms.CheckBox cboxDetectLineAll;
		private System.Windows.Forms.CheckBox cboxOldMethod;
		private System.Windows.Forms.ToolStripMenuItem rOI高さ位置合わせ一括左側優先ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rOI高さ位置合わせ一括右側優先ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rOI高さ連動モードToolStripMenuItem;
		private System.Windows.Forms.RadioButton radioButtonROIFree;
		private System.Windows.Forms.RadioButton radioButtonROIVertical;
		private System.Windows.Forms.RadioButton radioButtonROIHorizon;
		private System.Windows.Forms.GroupBox groupBoxROIMove;
		private System.Windows.Forms.ToolStripMenuItem 新規画像ManifestToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 検出ライン表示ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 紙左側ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 匡郭左側ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 紙右側ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 匡郭右側ToolStripMenuItem;
		private System.Windows.Forms.DataGridViewTextBoxColumn PageIdx;
		private System.Windows.Forms.DataGridViewCheckBoxColumn clValid;
		private System.Windows.Forms.DataGridViewTextBoxColumn folder1;
		private System.Windows.Forms.DataGridViewTextBoxColumn clFname;
		private System.Windows.Forms.DataGridViewTextBoxColumn heightPL;
		private System.Windows.Forms.DataGridViewTextBoxColumn heightGL;
		private System.Windows.Forms.DataGridViewTextBoxColumn heightPR;
		private System.Windows.Forms.DataGridViewTextBoxColumn heightGR;
		private System.Windows.Forms.DataGridViewCheckBoxColumn clChanged;
		private System.Windows.Forms.DataGridViewTextBoxColumn clBlank;
	}
}

