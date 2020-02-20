using System;
namespace CUSTOMRP.Model
{
	/// <summary>
	/// USERGROUP:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class USERGROUP
	{
		public USERGROUP()
		{}
		#region Model
		private int _id;
		private int _databaseid;
		private string _name;
		private string _description;
		private DateTime _audodate= DateTime.Now;
		private string _extend;
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
		public int DATABASEID
		{
			set{ _databaseid=value;}
			get{return _databaseid;}
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
		public string DESCRIPTION
		{
			set{ _description=value;}
			get{return _description;}
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
		public string EXTEND
		{
			set{ _extend=value;}
			get{return _extend;}
		}
		#endregion Model

	}
}

