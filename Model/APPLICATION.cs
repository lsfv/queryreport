using System;
namespace CUSTOMRP.Model
{
	/// <summary>
	/// APPLICATION:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class APPLICATION
	{
		public APPLICATION()
		{}
		#region Model
		private int _id;
		private string _name;
		private string _desc;
		private string _config_view1;
		private string _config_view2;
		private string _config_view3;
		private DateTime _lastmodifydate;
		private int _lastmodifyuser;
		private DateTime _audodate= DateTime.Now;
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
		public string DESC
		{
			set{ _desc=value;}
			get{return _desc;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CONFIG_VIEW1
		{
			set{ _config_view1=value;}
			get{return _config_view1;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CONFIG_VIEW2
		{
			set{ _config_view2=value;}
			get{return _config_view2;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CONFIG_VIEW3
		{
			set{ _config_view3=value;}
			get{return _config_view3;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime LASTMODIFYDATE
		{
			set{ _lastmodifydate=value;}
			get{return _lastmodifydate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int LASTMODIFYUSER
		{
			set{ _lastmodifyuser=value;}
			get{return _lastmodifyuser;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime AUDODATE
		{
			set{ _audodate=value;}
			get{return _audodate;}
		}
		#endregion Model

	}
}

