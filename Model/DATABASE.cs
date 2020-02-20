using System;
namespace CUSTOMRP.Model
{
	/// <summary>
	/// DATABASE:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class DATABASE
	{
		public DATABASE()
		{}
		#region Model
		private int _id;
		private int _applicationid;
		private string _name;
		private string _desc;
		private int _status;
		private DateTime _lastmodifydate;
		private int _lastmodifyuser;
		private DateTime _audotime= DateTime.Now;
        private string _hashkey;
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
		public int APPLICATIONID
		{
			set{ _applicationid=value;}
			get{return _applicationid;}
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
		public int STATUS
		{
			set{ _status=value;}
			get{return _status;}
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
		public DateTime AUDOTIME
		{
			set{ _audotime=value;}
			get{return _audotime;}
		}

        public string HASHKEY
        {
            set { _hashkey = value; }
            get { return _hashkey; }
        }
		#endregion Model

	}
}

