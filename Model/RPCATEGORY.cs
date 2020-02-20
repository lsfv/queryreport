using System;
namespace CUSTOMRP.Model
{
	/// <summary>
	/// RPCATEGORY:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class RPCATEGORY
	{
		public RPCATEGORY()
		{}
		#region Model
		private int _id;
		private string _name;
		private int _databaseid=1;
		private DateTime? _audodate= DateTime.Now;
		private string _description="";
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
		public string NAME
		{
			set{ _name=value;}
			get{return _name;}
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
		public DateTime? AUDODATE
		{
			set{ _audodate=value;}
			get{return _audodate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string DESCRIPTION
		{
			set{ _description=value;}
			get{return _description;}
		}
		#endregion Model

	}
}

