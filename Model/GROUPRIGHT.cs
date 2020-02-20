using System;
namespace CUSTOMRP.Model
{
	/// <summary>
	/// GROUPRIGHT:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class GROUPRIGHT
	{
		public GROUPRIGHT()
		{}
		#region Model
		private int _id;
		private int _gid;
		private string _company="";
		private string _reportgroup="";
		private string _categary="";
		private string _security;
		private string _query="";
		private string _usergroup="";
		private string _usergroupright="";
		private string _usersetup="";
		private DateTime _audodate= DateTime.Now;
		private string _extend1="";
		private string _extend2="";
		private string _extend3="";
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int GID
		{
			set{ _gid=value;}
			get{return _gid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string COMPANY
		{
			set{ _company=value;}
			get{return _company;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string REPORTGROUP
		{
			set{ _reportgroup=value;}
			get{return _reportgroup;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CATEGARY
		{
			set{ _categary=value;}
			get{return _categary;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string SECURITY
		{
			set{ _security=value;}
			get{return _security;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string QUERY
		{
			set{ _query=value;}
			get{return _query;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string USERGROUP
		{
			set{ _usergroup=value;}
			get{return _usergroup;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string USERGROUPRIGHT
		{
			set{ _usergroupright=value;}
			get{return _usergroupright;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string USERSETUP
		{
			set{ _usersetup=value;}
			get{return _usersetup;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime AUDODATE
		{
			set{ _audodate=value;}
			get{return _audodate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string EXTEND1
		{
			set{ _extend1=value;}
			get{return _extend1;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string EXTEND2
		{
			set{ _extend2=value;}
			get{return _extend2;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string EXTEND3
		{
			set{ _extend3=value;}
			get{return _extend3;}
		}
		#endregion Model

	}
}

