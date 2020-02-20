using System;
namespace CUSTOMRP.Model
{
	/// <summary>
	/// REPORTGROUP:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class REPORTGROUP
	{
		public REPORTGROUP()
		{}
		#region Model
		private int _id;
		private int _databaseid;
		private string _name;
		private string _description;
		private DateTime _audotime= DateTime.Now;
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
		public DateTime AUDOTIME
		{
			set{ _audotime=value;}
			get{return _audotime;}
		}
		#endregion Model

	}
}

