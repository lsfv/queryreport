using QueryReport.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueryReport.Admin
{
    public partial class DbUpdate : LoginUserPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //check contain dbversion's table.

                //chekc update msg.
                int currentVersion = 2;
                int needUpdate = 3;
                if (needUpdate>0)
                {
                    this.Label_Tip.Text = "Current version:" + currentVersion + ".      Need to update.";
                    this.btn_update.Enabled = true;
                }
                else
                {
                    this.Label_Tip.Text = "It is lasted version.";
                    this.btn_update.Enabled = false;
                }
            }
        }


        protected void btn_update_Click(object sender, EventArgs e)
        {

        }


        private string sql_create = @"CREATE TABLE [dbo].[DBVersion_Script](
                                    [script_version][int] NOT NULL,

                                   [script_title] [nvarchar] (50) NOT NULL,

                                    [script_desc] [nvarchar] (250) NOT NULL,
 
                                     [script_sql] [nvarchar] (max) NOT NULL,
	                                [script_extend] [nvarchar] (250) NOT NULL,
                                  CONSTRAINT[PK_DBVersion_Script] PRIMARY KEY CLUSTERED
                                (
                                    [script_version] ASC
                                )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
                                ) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]
                                GO";

        private string sql_create2 = @"CREATE TABLE [dbo].[DBVersion_History](
	                                [history_id] [int] IDENTITY(1,1) NOT NULL,
	                                [history_version] [int] NOT NULL,
	                                [history_extend] [nvarchar](250) NOT NULL,
	                                [history_datetime] [datetime] NOT NULL,
                                 CONSTRAINT [PK_DBVersion_History] PRIMARY KEY CLUSTERED 
                                (
	                                [history_id] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                ) ON [PRIMARY]
                                GO
                                ALTER TABLE [dbo].[DBVersion_History] ADD  CONSTRAINT [DF_DBVersion_History_history_datetime]  DEFAULT (getdate()) FOR [history_datetime]
                                GO";


    }
}