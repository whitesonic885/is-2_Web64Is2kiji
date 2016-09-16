using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Services;
using Oracle.DataAccess.Client;

namespace is2kiji
{
	/// <summary>
	/// [is2kiji]
	/// </summary>
	//--------------------------------------------------------------------------
	// 修正履歴
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 東都）高木 オブジェクトの破棄 
	//	disposeReader(reader);
	//	reader = null;
	//--------------------------------------------------------------------------
	// DEL 2007.05.10 東都）高木 未使用関数のコメント化 
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	//--------------------------------------------------------------------------
	// MOD 2009.11.12 東都）高木 ＡＭ指定、ＰＭ指定の追加 
	//--------------------------------------------------------------------------
	// MOD 2009.11.25 東都）高木 時間指定チェックの追加 
	//--------------------------------------------------------------------------
	// MOD 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
	//--------------------------------------------------------------------------

	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2kiji")]

	public class Service1 : is2common.CommService
	{
		public Service1()
		{
			//CODEGEN: この呼び出しは、ASP.NET Web サービス デザイナで必要です。
			InitializeComponent();

			connectService();
		}

		#region コンポーネント デザイナで生成されたコード 
		
		//Web サービス デザイナで必要です。
		private IContainer components = null;
				
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		/*********************************************************************
		 * 記事一覧取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、一覧（記事ＣＤ、記事、更新日時）...
		 *
		 *********************************************************************/
// MOD 2005.05.11 東都）高木 ORA-03113対策？ START
//		private static string GET_KIJI_SELECT
//			= "SELECT 記事ＣＤ, 記事, TO_CHAR(更新日時) \n"
//			+  " FROM ＳＭ０３記事 \n";
		private static string GET_KIJI_SELECT
			= "SELECT /*+ INDEX(ＳＭ０３記事 SM03PKEY) */ \n"
			+  " 記事ＣＤ, 記事, 更新日時 \n"
			+  " FROM ＳＭ０３記事 \n";
// MOD 2005.05.11 東都）高木 ORA-03113対策？ END

		private static string GET_KIJI_ORDER
			=   " AND 削除ＦＧ = '0' \n"
			+ " ORDER BY 記事ＣＤ \n";

		[WebMethod]
		public String[] Get_kiji(string[] sUser, string sKCode, string sBCode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "記事一覧取得開始");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[1];
			// ADD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 END

			try
			{
				StringBuilder sbQuery = new StringBuilder(512);
				sbQuery.Append(GET_KIJI_SELECT);
				sbQuery.Append(" WHERE 会員ＣＤ = '" + sKCode + "' \n");
				sbQuery.Append(  " AND 部門ＣＤ = '" + sBCode + "' \n");
				sbQuery.Append(GET_KIJI_ORDER);

				// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
				//OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);
				logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + sbQuery.ToString());	//修正前のUPDATE文をログ出力

				sbQuery = new StringBuilder(512);
				sbQuery.Append(GET_KIJI_SELECT);
				sbQuery.Append(" WHERE 会員ＣＤ = :p_KaiinCD \n");
				sbQuery.Append(  " AND 部門ＣＤ = :p_BumonCD \n");
				sbQuery.Append(GET_KIJI_ORDER);

				wk_opOraParam = new OracleParameter[2];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKCode, ParameterDirection.Input);
				wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBCode, ParameterDirection.Input);

				OracleDataReader	reader = CmdSelect(sUser, conn2, sbQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

				StringBuilder sbData = new StringBuilder(64); // 4+30+14+4 = 52
				while (reader.Read())
				{
					sbData = new StringBuilder(64);
					sbData.Append("|" + reader.GetString(0).Trim());
					sbData.Append("|" + reader.GetString(1).TrimEnd());
// MOD 2005.05.11 東都）高木 ORA-03113対策？ START
//					sbData.Append("|" + reader.GetString(2));
					sbData.Append("|" + reader.GetDecimal(2).ToString().Trim());
// MOD 2005.05.11 東都）高木 ORA-03113対策？ END
					sbData.Append("|");
					sList.Add(sbData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "該当データがありません";
				else
				{
					sRet[0] = "正常終了";
					int iCnt = 1;
					IEnumerator enumList = sList.GetEnumerator();
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();
						iCnt++;
					}
				}


				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 記事データ取得
		 * 引数：会員ＣＤ、部門ＣＤ、記事ＣＤ
		 * 戻値：ステータス、記事ＣＤ、更新日時、状態区分
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_Skiji(string[] sUser, string sKCode,string sBCode,string sCode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "記事データ取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 END

			try
			{
				string cmdQuery
					= "SELECT 記事, TO_CHAR(更新日時) \n"
					+  " FROM ＳＭ０３記事 \n"
					+ " WHERE 会員ＣＤ = '" + sKCode + "' \n"
					+   " AND 部門ＣＤ = '" + sBCode + "' \n"
					+   " AND 記事ＣＤ = '" + sCode  + "' \n"
					+   " AND 削除ＦＧ = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				bool bRead = reader.Read();
				if(bRead == true)
				{
					sRet[1] = reader.GetString(0).TrimEnd();
					sRet[2] = reader.GetString(1);
					sRet[0] = "更新";
					sRet[3] = "U";
				}
				else
				{
					sRet[0] = "登録";
					sRet[3] = "I";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 記事データ更新
		 * 引数：会員ＣＤ、部門ＣＤ、記事ＣＤ...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Upd_kiji(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "記事データ更新開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery 
					= "UPDATE ＳＭ０３記事 \n"
					+   " SET 記事     = '" + sData[3] +"', \n"
					+       " 削除ＦＧ = '0', \n"
					+       " 更新ＰＧ = '" + sData[4] +"', \n"
					+       " 更新者   = '" + sData[5] +"', \n"
					+       " 更新日時 =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')  \n"
					+ " WHERE 会員ＣＤ = '" + sData[0] +"' \n"
					+   " AND 部門ＣＤ = '" + sData[1] +"' \n"
					+   " AND 記事ＣＤ = '" + sData[2] +"' \n"
					+   " AND 更新日時 =  " + sData[6] +" \n";

				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);

				tran.Commit();
				if(iUpdRow == 0)
					sRet[0] = "データ編集中に他の端末より更新されています。\r\n再度、最新データを呼び出して更新してください。";
				else				
					sRet[0] = "正常終了";

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 記事データ登録
		 * 引数：会員ＣＤ、部門ＣＤ、記事ＣＤ...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Ins_kiji(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "記事データ登録開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery 
					= "DELETE FROM ＳＭ０３記事 \n"
					+ " WHERE 会員ＣＤ = '" + sData[0] +"' \n"
					+   " AND 部門ＣＤ = '" + sData[1] +"' \n"
					+   " AND 記事ＣＤ = '" + sData[2] +"' \n"
					+   " AND 削除ＦＧ = '1' \n";

				CmdUpdate(sUser, conn2, cmdQuery);

				cmdQuery 
					= "INSERT INTO ＳＭ０３記事  \n"
					+ "VALUES ('" + sData[0] +"','" + sData[1] +"','" + sData[2] +"','" + sData[3] +"', \n"
					+         "'0',TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[4] +"','" + sData[5] +"', \n"
					+         "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[4] +"','" + sData[5] +"') \n";

				CmdUpdate(sUser, conn2, cmdQuery);

				tran.Commit();
				sRet[0] = "正常終了";
				
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
// DEL 2005.05.31 東都）高木 無意味なので削除 START
//				string sErr = ex.Message.Substring(0,9);
//				if(sErr == "ORA-00001")
//					sRet[0] = "同一のコードが既に他の端末より登録されています。\r\n再度、最新データを呼び出して更新してください。";
//				else
// DEL 2005.05.31 東都）高木 無意味なので削除 END
					sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 記事データ削除
		 * 引数：会員ＣＤ、部門ＣＤ、記事ＣＤ、更新ＰＧ、更新者
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Del_kiji(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "記事データ削除開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）小童谷 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				StringBuilder sbQuery = new StringBuilder(1024);
/*
				string cmdQuery 
					= "UPDATE ＳＭ０３記事 \n"
					+   " SET 削除ＦＧ = '1', \n"
					+       " 更新ＰＧ = '" + sData[3] +"', \n"
					+       " 更新者   = '" + sData[4] +"', \n"
					+       " 更新日時 =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
					+ " WHERE 会員ＣＤ = '" + sData[0] +"' \n"
					+   " AND 部門ＣＤ = '" + sData[1] +"' \n"
					+   " AND 記事ＣＤ = '" + sData[2] +"' \n";
*/
				sbQuery.Append
					( "UPDATE ＳＭ０３記事 \n"
					+   " SET 削除ＦＧ = '1', \n"
					+       " 更新ＰＧ = '" + sData[3] +"', \n"
					+       " 更新者   = '" + sData[4] +"', \n"
					+       " 更新日時 =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
					+ " WHERE 会員ＣＤ = '" + sData[0] +"' \n"
					+   " AND 部門ＣＤ = '" + sData[1] +"' \n"
					+   " AND 記事ＣＤ = '" + sData[2] +"' \n");

//				CmdUpdate(sUser, conn2, cmdQuery);
				CmdUpdate(sUser, conn2, sbQuery);

				tran.Commit();				
				sRet[0] = "正常終了";

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

// ADD 2005.06.20 東都）伊賀 輸送商品コード検索追加 START
		/*********************************************************************
		 * 輸送商品コード検索
		 * 引数：部門ＣＤ、記事
		 * 戻値：ステータス
		 *       輸送商品名から輸送商品コードを検索します
		 *********************************************************************/
		[WebMethod]
		public String[] Get_kijiCD(string[] sUser, string sBcode, string sKname)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "輸送商品コード検索開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

// MOD 2009.11.12 東都）高木 ＡＭ指定、ＰＭ指定の追加 START
//			OracleTransaction tran;
//			tran = conn2.BeginTransaction();
// MOD 2009.11.12 東都）高木 ＡＭ指定、ＰＭ指定の追加 END

			try
			{
				StringBuilder sbQuery = new StringBuilder(1024);
				string sKcode = "";

// MOD 2009.11.12 東都）高木 ＡＭ指定、ＰＭ指定の追加 START
//				if (!sBcode.Equals("0000") && sKname.StartsWith("時間指定"))
//				{
//					if (sKname.EndsWith("まで"))
//					{
//						sKcode = sBcode.Substring(0,1) + "1X";
//					}
//					if (sKcode.EndsWith("以降"))
//					{
//						sKcode = sBcode.Substring(0,1) + "2X";
//					}
//				}
				if(sBcode.Equals("100")){
// MOD 2009.11.25 東都）高木 時間指定チェックの追加 START
//					if(sKname.StartsWith("ＡＭ")){
//						sKcode = "130";
//					}else if(sKname.StartsWith("ＰＭ")){
//						sKcode = "132";
//					}else if(sKname.StartsWith("時間指定")){
					if(sKname.StartsWith("時間指定")){
// MOD 2009.11.25 東都）高木 時間指定チェックの追加 END
						if(sKname.EndsWith("まで")){
							sKcode = "11X";
						}else if(sKname.EndsWith("以降")){
							sKcode = "12X";
						}
					}
				}else if(sBcode.Equals("200")){
					if(sKname.StartsWith("時間指定")){
						if(sKname.EndsWith("まで")){
							sKcode = "21X";
						}else if(sKname.EndsWith("以降")){
							sKcode = "22X";
						}
					}
				}
// MOD 2009.11.12 東都）高木 ＡＭ指定、ＰＭ指定の追加 END

				sbQuery.Append( "SELECT 記事ＣＤ" );
                sbQuery.Append(  " FROM ＳＭ０３記事 \n" );
				sbQuery.Append( " WHERE 会員ＣＤ = 'yusoshohin' \n" );
				sbQuery.Append(   " AND 部門ＣＤ = '" + sBcode +"' \n" );
				if (sKcode.Length != 0)
				{
					sbQuery.Append(   " AND 記事ＣＤ = '" + sKcode +"' \n" );
				}
				else
				{
					sbQuery.Append(   " AND 記事     = '" + sKname +"' \n" );
				}
				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				if(reader.Read())
				{
					sRet[0] = "正常終了";
					sRet[1] = reader.GetString(0).Trim();
				}
				else
				{
					sRet[0] = "該当データがありません";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.06.20 東都）伊賀 輸送商品コード検索追加 END
	}
}
