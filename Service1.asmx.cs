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
	// �C������
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� 
	//	disposeReader(reader);
	//	reader = null;
	//--------------------------------------------------------------------------
	// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g�� 
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	//--------------------------------------------------------------------------
	// MOD 2009.11.12 ���s�j���� �`�l�w��A�o�l�w��̒ǉ� 
	//--------------------------------------------------------------------------
	// MOD 2009.11.25 ���s�j���� ���Ԏw��`�F�b�N�̒ǉ� 
	//--------------------------------------------------------------------------
	// MOD 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
	//--------------------------------------------------------------------------

	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2kiji")]

	public class Service1 : is2common.CommService
	{
		public Service1()
		{
			//CODEGEN: ���̌Ăяo���́AASP.NET Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
			InitializeComponent();

			connectService();
		}

		#region �R���|�[�l���g �f�U�C�i�Ő������ꂽ�R�[�h 
		
		//Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
		private IContainer components = null;
				
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
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
		 * �L���ꗗ�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�ꗗ�i�L���b�c�A�L���A�X�V�����j...
		 *
		 *********************************************************************/
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H START
//		private static string GET_KIJI_SELECT
//			= "SELECT �L���b�c, �L��, TO_CHAR(�X�V����) \n"
//			+  " FROM �r�l�O�R�L�� \n";
		private static string GET_KIJI_SELECT
			= "SELECT /*+ INDEX(�r�l�O�R�L�� SM03PKEY) */ \n"
			+  " �L���b�c, �L��, �X�V���� \n"
			+  " FROM �r�l�O�R�L�� \n";
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H END

		private static string GET_KIJI_ORDER
			=   " AND �폜�e�f = '0' \n"
			+ " ORDER BY �L���b�c \n";

		[WebMethod]
		public String[] Get_kiji(string[] sUser, string sKCode, string sBCode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�L���ꗗ�擾�J�n");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[1];
			// ADD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� END

			try
			{
				StringBuilder sbQuery = new StringBuilder(512);
				sbQuery.Append(GET_KIJI_SELECT);
				sbQuery.Append(" WHERE ����b�c = '" + sKCode + "' \n");
				sbQuery.Append(  " AND ����b�c = '" + sBCode + "' \n");
				sbQuery.Append(GET_KIJI_ORDER);

				// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
				//OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);
				logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + sbQuery.ToString());	//�C���O��UPDATE�������O�o��

				sbQuery = new StringBuilder(512);
				sbQuery.Append(GET_KIJI_SELECT);
				sbQuery.Append(" WHERE ����b�c = :p_KaiinCD \n");
				sbQuery.Append(  " AND ����b�c = :p_BumonCD \n");
				sbQuery.Append(GET_KIJI_ORDER);

				wk_opOraParam = new OracleParameter[2];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKCode, ParameterDirection.Input);
				wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBCode, ParameterDirection.Input);

				OracleDataReader	reader = CmdSelect(sUser, conn2, sbQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

				StringBuilder sbData = new StringBuilder(64); // 4+30+14+4 = 52
				while (reader.Read())
				{
					sbData = new StringBuilder(64);
					sbData.Append("|" + reader.GetString(0).Trim());
					sbData.Append("|" + reader.GetString(1).TrimEnd());
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H START
//					sbData.Append("|" + reader.GetString(2));
					sbData.Append("|" + reader.GetDecimal(2).ToString().Trim());
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H END
					sbData.Append("|");
					sList.Add(sbData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "�Y���f�[�^������܂���";
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * �L���f�[�^�擾
		 * �����F����b�c�A����b�c�A�L���b�c
		 * �ߒl�F�X�e�[�^�X�A�L���b�c�A�X�V�����A��ԋ敪
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_Skiji(string[] sUser, string sKCode,string sBCode,string sCode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�L���f�[�^�擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� END

			try
			{
				string cmdQuery
					= "SELECT �L��, TO_CHAR(�X�V����) \n"
					+  " FROM �r�l�O�R�L�� \n"
					+ " WHERE ����b�c = '" + sKCode + "' \n"
					+   " AND ����b�c = '" + sBCode + "' \n"
					+   " AND �L���b�c = '" + sCode  + "' \n"
					+   " AND �폜�e�f = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				bool bRead = reader.Read();
				if(bRead == true)
				{
					sRet[1] = reader.GetString(0).TrimEnd();
					sRet[2] = reader.GetString(1);
					sRet[0] = "�X�V";
					sRet[3] = "U";
				}
				else
				{
					sRet[0] = "�o�^";
					sRet[3] = "I";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * �L���f�[�^�X�V
		 * �����F����b�c�A����b�c�A�L���b�c...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Upd_kiji(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�L���f�[�^�X�V�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery 
					= "UPDATE �r�l�O�R�L�� \n"
					+   " SET �L��     = '" + sData[3] +"', \n"
					+       " �폜�e�f = '0', \n"
					+       " �X�V�o�f = '" + sData[4] +"', \n"
					+       " �X�V��   = '" + sData[5] +"', \n"
					+       " �X�V���� =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')  \n"
					+ " WHERE ����b�c = '" + sData[0] +"' \n"
					+   " AND ����b�c = '" + sData[1] +"' \n"
					+   " AND �L���b�c = '" + sData[2] +"' \n"
					+   " AND �X�V���� =  " + sData[6] +" \n";

				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);

				tran.Commit();
				if(iUpdRow == 0)
					sRet[0] = "�f�[�^�ҏW���ɑ��̒[�����X�V����Ă��܂��B\r\n�ēx�A�ŐV�f�[�^���Ăяo���čX�V���Ă��������B";
				else				
					sRet[0] = "����I��";

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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * �L���f�[�^�o�^
		 * �����F����b�c�A����b�c�A�L���b�c...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Ins_kiji(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�L���f�[�^�o�^�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery 
					= "DELETE FROM �r�l�O�R�L�� \n"
					+ " WHERE ����b�c = '" + sData[0] +"' \n"
					+   " AND ����b�c = '" + sData[1] +"' \n"
					+   " AND �L���b�c = '" + sData[2] +"' \n"
					+   " AND �폜�e�f = '1' \n";

				CmdUpdate(sUser, conn2, cmdQuery);

				cmdQuery 
					= "INSERT INTO �r�l�O�R�L��  \n"
					+ "VALUES ('" + sData[0] +"','" + sData[1] +"','" + sData[2] +"','" + sData[3] +"', \n"
					+         "'0',TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[4] +"','" + sData[5] +"', \n"
					+         "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[4] +"','" + sData[5] +"') \n";

				CmdUpdate(sUser, conn2, cmdQuery);

				tran.Commit();
				sRet[0] = "����I��";
				
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
// DEL 2005.05.31 ���s�j���� ���Ӗ��Ȃ̂ō폜 START
//				string sErr = ex.Message.Substring(0,9);
//				if(sErr == "ORA-00001")
//					sRet[0] = "����̃R�[�h�����ɑ��̒[�����o�^����Ă��܂��B\r\n�ēx�A�ŐV�f�[�^���Ăяo���čX�V���Ă��������B";
//				else
// DEL 2005.05.31 ���s�j���� ���Ӗ��Ȃ̂ō폜 END
					sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * �L���f�[�^�폜
		 * �����F����b�c�A����b�c�A�L���b�c�A�X�V�o�f�A�X�V��
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Del_kiji(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�L���f�[�^�폜�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�����J ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				StringBuilder sbQuery = new StringBuilder(1024);
/*
				string cmdQuery 
					= "UPDATE �r�l�O�R�L�� \n"
					+   " SET �폜�e�f = '1', \n"
					+       " �X�V�o�f = '" + sData[3] +"', \n"
					+       " �X�V��   = '" + sData[4] +"', \n"
					+       " �X�V���� =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
					+ " WHERE ����b�c = '" + sData[0] +"' \n"
					+   " AND ����b�c = '" + sData[1] +"' \n"
					+   " AND �L���b�c = '" + sData[2] +"' \n";
*/
				sbQuery.Append
					( "UPDATE �r�l�O�R�L�� \n"
					+   " SET �폜�e�f = '1', \n"
					+       " �X�V�o�f = '" + sData[3] +"', \n"
					+       " �X�V��   = '" + sData[4] +"', \n"
					+       " �X�V���� =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
					+ " WHERE ����b�c = '" + sData[0] +"' \n"
					+   " AND ����b�c = '" + sData[1] +"' \n"
					+   " AND �L���b�c = '" + sData[2] +"' \n");

//				CmdUpdate(sUser, conn2, cmdQuery);
				CmdUpdate(sUser, conn2, sbQuery);

				tran.Commit();				
				sRet[0] = "����I��";

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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

// ADD 2005.06.20 ���s�j�ɉ� �A�����i�R�[�h�����ǉ� START
		/*********************************************************************
		 * �A�����i�R�[�h����
		 * �����F����b�c�A�L��
		 * �ߒl�F�X�e�[�^�X
		 *       �A�����i������A�����i�R�[�h���������܂�
		 *********************************************************************/
		[WebMethod]
		public String[] Get_kijiCD(string[] sUser, string sBcode, string sKname)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�A�����i�R�[�h�����J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

// MOD 2009.11.12 ���s�j���� �`�l�w��A�o�l�w��̒ǉ� START
//			OracleTransaction tran;
//			tran = conn2.BeginTransaction();
// MOD 2009.11.12 ���s�j���� �`�l�w��A�o�l�w��̒ǉ� END

			try
			{
				StringBuilder sbQuery = new StringBuilder(1024);
				string sKcode = "";

// MOD 2009.11.12 ���s�j���� �`�l�w��A�o�l�w��̒ǉ� START
//				if (!sBcode.Equals("0000") && sKname.StartsWith("���Ԏw��"))
//				{
//					if (sKname.EndsWith("�܂�"))
//					{
//						sKcode = sBcode.Substring(0,1) + "1X";
//					}
//					if (sKcode.EndsWith("�ȍ~"))
//					{
//						sKcode = sBcode.Substring(0,1) + "2X";
//					}
//				}
				if(sBcode.Equals("100")){
// MOD 2009.11.25 ���s�j���� ���Ԏw��`�F�b�N�̒ǉ� START
//					if(sKname.StartsWith("�`�l")){
//						sKcode = "130";
//					}else if(sKname.StartsWith("�o�l")){
//						sKcode = "132";
//					}else if(sKname.StartsWith("���Ԏw��")){
					if(sKname.StartsWith("���Ԏw��")){
// MOD 2009.11.25 ���s�j���� ���Ԏw��`�F�b�N�̒ǉ� END
						if(sKname.EndsWith("�܂�")){
							sKcode = "11X";
						}else if(sKname.EndsWith("�ȍ~")){
							sKcode = "12X";
						}
					}
				}else if(sBcode.Equals("200")){
					if(sKname.StartsWith("���Ԏw��")){
						if(sKname.EndsWith("�܂�")){
							sKcode = "21X";
						}else if(sKname.EndsWith("�ȍ~")){
							sKcode = "22X";
						}
					}
				}
// MOD 2009.11.12 ���s�j���� �`�l�w��A�o�l�w��̒ǉ� END

				sbQuery.Append( "SELECT �L���b�c" );
                sbQuery.Append(  " FROM �r�l�O�R�L�� \n" );
				sbQuery.Append( " WHERE ����b�c = 'yusoshohin' \n" );
				sbQuery.Append(   " AND ����b�c = '" + sBcode +"' \n" );
				if (sKcode.Length != 0)
				{
					sbQuery.Append(   " AND �L���b�c = '" + sKcode +"' \n" );
				}
				else
				{
					sbQuery.Append(   " AND �L��     = '" + sKname +"' \n" );
				}
				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				if(reader.Read())
				{
					sRet[0] = "����I��";
					sRet[1] = reader.GetString(0).Trim();
				}
				else
				{
					sRet[0] = "�Y���f�[�^������܂���";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.06.20 ���s�j�ɉ� �A�����i�R�[�h�����ǉ� END
	}
}
