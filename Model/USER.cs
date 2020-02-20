using System;
namespace CUSTOMRP.Model
{
	/// <summary>
	/// USER:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class USER
	{
		public USER()
		{}
		#region Model
		private int _id;
		private string _uid;
		private int _gid=2;
		private int _databaseid=1;
		private string _password;
		private string _viewlevel;
		private string _reportgrouplist;
		private string _usergrouplevel;
		private int _setupuser;
		private int _reportright;
		private DateTime _autodate= DateTime.Now;
		private string _email;
		private string _usergroup="";
		private string _name;
		private decimal? _sensitivitylevel;
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
		public string UID
		{
			set{ _uid=value;}
			get{return _uid;}
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
		public int DATABASEID
		{
			set{ _databaseid=value;}
			get{return _databaseid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string PASSWORD
		{
			set{ _password=value;}
			get{return _password;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string VIEWLEVEL
		{
			set{ _viewlevel=value;}
			get{return _viewlevel;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string REPORTGROUPLIST
		{
			set{ _reportgrouplist=value;}
			get{return _reportgrouplist;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string USERGROUPLEVEL
		{
			set{ _usergrouplevel=value;}
			get{return _usergrouplevel;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int SETUPUSER
		{
			set{ _setupuser=value;}
			get{return _setupuser;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int REPORTRIGHT
		{
			set{ _reportright=value;}
			get{return _reportright;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime AUTODATE
		{
			set{ _autodate=value;}
			get{return _autodate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string EMAIL
		{
			set{ _email=value;}
			get{return _email;}
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
		public string NAME
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal? SENSITIVITYLEVEL
		{
			set{ _sensitivitylevel=value;}
			get{return _sensitivitylevel;}
		}
		#endregion Model

	}
}

