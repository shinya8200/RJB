using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace RulerJB
{
	/// <summary>
	/// 各クラスから共通に使用する値を管理するクラスです
	/// </summary>
	class CommonValues
	{
		///// <summary>プロジェクトデータ（和書1冊分情報）を保持します</summary>
		//static public BookDataRJB DataRJB { get; set; }


		/// <summary>設定情報を保持します</summary>
		static public ConfigurationData ConfigData = new ConfigurationData();

		/// <summary> 既定の設定フォルダ </summary>
		static public string DefaultSettingFolder = Path.GetDirectoryName(Application.ExecutablePath);
		

		#region 固定値　----------------------------------------------------

		/// <summary>デフォルト設定フォルダ（アプリケーションフォルダ以下）を保持します</summary>
		static public string AppFolderName = "RJBv01";

		/// <summary>BookProjectファイルの拡張子</summary>
		/// <remarks>'.'を含む拡張子</remarks>
		static public string ExtBookProject = ".prb";


		#endregion  固定値　----------------------------------------------------
	}
}
