using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data;



namespace RulerJB
{
	/// <summary>
	/// ユーティリティメソッド群
	/// </summary>
	public class RJB_Utility
	{

		/// <summary>
		/// データグリッドの内容をCSVで保存します。（ファイル選択などの共通処理部分のみ。書き込みは指定のFuncで行います）
		/// </summary>
		/// <param name="funcSaveCsv">保存関数 bool Func( DataGridView datagrid, string filePath )</param>
		static public void SaveGridToCsv( DataGridView datagridview, Func<DataGridView, string, bool> funcSaveCsv )
		{
			string fname;
			var sfd = new SaveFileDialog();
			sfd.Filter =    "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
			//[ファイルの種類]ではじめに
			//「すべてのファイル」が選択されているようにする
			sfd.FilterIndex = 1;
			//タイトルを設定する
			sfd.Title = "保存先のファイルを選択してください";
			if( sfd.ShowDialog()== System.Windows.Forms.DialogResult.OK )
			{
				fname= sfd.FileName;
				if( funcSaveCsv( datagridview, sfd.FileName )== false )
				{
					MessageBox.Show( String.Format( "ファイル（{0}）書き込みが失敗しました", Path.GetFileName( fname ) ), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}


		/// <summary>
		/// DataGridViewの内容をCSVで保存します。
		/// </summary>
		static public void SaveGridToCsv( DataGridView datagridview )
		{
			string fname;
			var sfd = new SaveFileDialog();
			sfd.Filter =    "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
			//[ファイルの種類]ではじめに
			//「すべてのファイル」が選択されているようにする
			sfd.FilterIndex = 1;
			//タイトルを設定する
			sfd.Title = "保存先のファイルを選択してください";
			if( sfd.ShowDialog()== System.Windows.Forms.DialogResult.OK )
			{
				fname= sfd.FileName;
				if( ExportCsvFileData( datagridview, sfd.FileName )== false )
				{
					MessageBox.Show( String.Format( "ファイル（{0}）書き込みが失敗しました", Path.GetFileName( fname ) ), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}


		/// <summary> CSVファイルにExportします </summary>
		/// <param title="filepath">ファイル名</param>
		/// <param name="datagridview">データグリッドビュー</param>
		/// <returns>成否を返します</returns>
		static private bool ExportCsvFileData( DataGridView datagridview, string filename )
		{

			bool ret = false;
			try
			{
				using( StreamWriter sw = new StreamWriter( filename, false, Encoding.GetEncoding( "Shift-JIS" ) ) )
				{
					//　ヘッダーもDataGridViewから取得する
					sw.WriteLine( String.Join( ",", datagridview.Columns.Cast<DataGridViewColumn>().Select( c => c.HeaderCell.Value.ToString() ) ) );
					foreach( var r in datagridview.Rows.Cast<DataGridViewRow>() )
					{
						var line = String.Join( ",", r.Cells.Cast<DataGridViewCell>().Select( n => n.Value.ToString() ) );
						sw.WriteLine( line );
					}
					sw.Close();
					ret = true;
				}
			}
			catch // ( Exception ex  )
			{
				MessageBox.Show( "CSVファイルの書き込みに失敗しました", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				ret = false;
			}
			return ret;
		}




		/// <summary>
		/// クリップボードに(CSV形式で）コピーします
		/// </summary>
		static public void DataGridDataToClipBoard(DataGridView datagirdview, string separator= "\t")
		{
			using( var sw = new StringWriter() )
			{
				//　ヘッダーもDataGridViewから取得する
				sw.WriteLine( String.Join( separator, datagirdview.Columns.Cast<DataGridViewColumn>().Select( c => c.HeaderCell.Value.ToString() ) ) );
				foreach( var r in datagirdview.Rows.Cast<DataGridViewRow>() )
				{
					var line = String.Join( separator, r.Cells.Cast<DataGridViewCell>().Select( n => n.Value.ToString() ) );
					sw.WriteLine( line );
				}
				Clipboard.SetDataObject( sw.ToString() );
				sw.Close();
			}

		}





	}
}
