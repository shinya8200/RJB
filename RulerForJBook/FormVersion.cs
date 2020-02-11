using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// 修正履歴
//			labelVersion.Text= "Ver. 2.06.00";  // 2020/1/15 dpi設定追加、URL→RUI
//			labelVersion.Text= "Ver. 2.05.00";  // 2019/12/20 高速化、ぼかしをガウシアン→バイラテラルフィルタに変更、PDF、IIIFの画像ファイル保存
//			labelVersion.Text= "Ver. 2.04.00";  // 2019/11/5 ROI上下、左右固定移動モード、PDF、IIIF対応
//			labelVersion.Text= "Ver. 2.03.00";  // 2019/10/30 UI周り修正　ROI高さ連動、矢印平行移動
//			labelVersion.Text= "Ver. 2.02.00";  // 2019/10/18 UI周りの操作性改善
//			labelVersion.Text= "Ver. 2.01.00";  // 2019/09/02 ROI初期サイズ変更
// 			labelVersion.Text= "Ver. 2.00.00";  // 2019/08/21 新規作成（VS2019,OpenCV3対応済みソース）



namespace RulerJB
{
	/// <summary>
	/// バージョン表示ダイアログクラスです</summary>
	public partial class FormVersion : Form
	{
		/// <summary>
		/// コンストラクタです</summary>
		public FormVersion()
		{
			InitializeComponent();
		}


		/// <summary>
		/// ロード時の処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void FormVersion_Load(object sender, EventArgs e)
		{
			labelVersion.Text= "Ver. 2.06.00";  // 2020/1/15 dpi設定追加、URL→RUI
		}
	}
}
