using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Common
{
    /// <summary> 
    /// 上传类 
    /// </summary> 
    public class IncUpfile
    {
        public IncUpfile(){ }
        #region enum
        /// <summary> 
        /// 允许文件上传的类型枚举 
        /// </summary> 
        public enum FileType
        {
            jpg, gif, bmp, png
        }

        public int FileSize = 1024; 
        #endregion

        #region 取得文件后缀
        /// <summary> 
        /// 取得文件后缀 
        /// </summary> 
        /// <param name="filename">文件名称</param> 
        /// <returns></returns> 
        public static string GetFileExtends(string filename)
        {
            string ext = null;
            if (filename.IndexOf('.') > 0)
            {
                string[] fs = filename.Split('.');
                ext = fs[fs.Length - 1];
            }
            return ext;
        }
        #endregion

        #region getname
    
        /// <summary> 
        /// 取得上传文件名 
        /// </summary> 
        /// <param name="filename">文件名称</param> 
        /// <returns></returns> 
        public static string GetFilename(string filename)
        {
            string ext = null;
            if (filename.IndexOf('/') > 0)
            {
                string[] fs = filename.Split('/');
                ext = fs[fs.Length - 1];
            }
            return ext;
        } 
        #endregion

        #region 检测文件是否合法
        /// <summary> 
        /// 检测上传文件是否合法 
        /// </summary> 
        /// <param name="fileExtends">文件后缀名</param> 
        /// <returns></returns> 
        public static bool CheckFileExtends(string fileExtends)
        {
            bool status = false;
            fileExtends = fileExtends.ToLower(CultureInfo.InvariantCulture);
            string[] fe = Enum.GetNames(typeof(FileType));
            for (int i = 0; i < fe.Length; i++)
            {
                if (fe[i].ToLower(CultureInfo.InvariantCulture) == fileExtends)
                {
                    status = true;
                    break;
                }
            }
            return status;
        }
        #endregion

        #region 保存文件
        /// <summary> 
        /// 保存文件 
        /// </summary> 
        /// <param name="fpath">全路径,Server.MapPath()</param> 
        /// <param name="myFileUpload">上传控件</param> 
        /// <returns></returns> 
        public static string PhotoSave(string fpath, FileUpload myFileUpload)
        {
            string s = "";
            string fileExtends = "";
            string fileName = myFileUpload.FileName;
            if (fileName != "")
            {
                //取得文件后缀 
                fileExtends = GetFileExtends(fileName);
                if (!CheckFileExtends(fileExtends))
                {
                    return "格式错误";
                }

                if (myFileUpload.FileBytes.Length >= 1024*1024)
                {
                    return "超过大小";
                }

                Random rd = new Random();
                s = DateTime.Now.ToFileTimeUtc() + "." + fileExtends;
                string file = fpath + "\\" + s;
                try
                {
                    myFileUpload.SaveAs(file);
                }
                catch (Exception ee)
                {
                    throw new Exception(ee.ToString());
                }
            }
            return s;
        }
        #endregion

        #region 加入文字水印
        /// <summary> 
        /// 加入文字水印 
        /// </summary> 
        /// <param name="fileName">文件名称路径(全路径)</param> 
        /// <param name="text">文件</param> 
        public void AddTextToImg(string fileName, string text)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("文件不存在");
            }
            if (text == string.Empty)
            {
                return;
            }
            //判断文件类型是否为图像类型 
            System.Drawing.Image image = System.Drawing.Image.FromFile(fileName);
            Bitmap bitmap = new Bitmap(image, image.Width, image.Height);
            Graphics g = Graphics.FromImage(bitmap);
            float fontSize = 12.0f;//字体大小 
            float textWidth = text.Length * fontSize;//文本的长度 
            //下面定义一个矩形区域,以后在这个矩形里面画上白底黑字 
            float rectX = 0;
            float rectY = 0;
            float rectWidth = text.Length * (fontSize + 8);
            float rectHeight = fontSize + 8;
            //声明矩形域 
            RectangleF textArea = new RectangleF(rectX, rectY, rectWidth, rectHeight);
            Font font = new Font("宋体", fontSize);//定义字体 
            Brush whiteBrush = new SolidBrush(Color.White);//白笔刷,画文字用 
            Brush blackBrush = new SolidBrush(Color.Black);//黑笔刷，画背景用 
            g.FillRectangle(blackBrush, rectX, rectY, rectWidth, rectHeight);
            g.DrawString(text, font, whiteBrush, textArea);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            //输出处理后的图像，这里为了演示方便，我将图片显示在页面中了 
            //Response.Clear(); 
            //Response.ContentType = "image/jpeg"; 
            //Response.BinaryWrite(ms.ToArray()); 
            g.Dispose();
            bitmap.Dispose();
            image.Dispose();
        }
        #endregion

        #region 保存文件
        /// <summary> 
        /// 保存文件 
        /// </summary> 
        /// <param name="fpath">全路径,Server.MapPath()</param> 
        /// <param name="myFileUpload">上传控件</param> 
        /// <returns></returns> 
        public static string PhotoSaveinput(string fpath, HtmlInputFile  myFileUpload)
        {
            string s = "";
            string fileExtends = "";
            string fileName = myFileUpload.PostedFile.FileName;
            if (fileName != "")
            {
                //取得文件后缀 
                fileExtends = GetFileExtends(fileName);
                if (!CheckFileExtends(fileExtends))
                {
                    return "格式错误";
                }
                //1m图片
                if (myFileUpload.PostedFile.ContentLength>= 1024*1024)
                {
                    return "超过大小";
                }

                Random rd = new Random();
                s = DateTime.Now.ToFileTimeUtc() + "." + fileExtends;
                string file = fpath + "\\" + s;
                try
                {
                    myFileUpload.PostedFile.SaveAs(file);
                }
                catch (Exception ee)
                {
                    throw new Exception(ee.ToString());
                }
            }
            return s;
        }
        #endregion

        #region save
        private static bool CheckFileExtends(string fileExtends, List<string> fileExtendsType)
        {
            bool status = false;
            fileExtends = fileExtends.ToLower(CultureInfo.InvariantCulture);

            for (int i = 0; i < fileExtendsType.Count; i++)
            {
                if (fileExtendsType[i].ToLower(CultureInfo.InvariantCulture) == fileExtends)
                {
                    status = true;
                    break;
                }
            }
            return status;
        }

        /// <summary> 
        /// 保存文件 
        /// </summary> 
        /// <param name="fpath">全路径,Server.MapPath()</param> 
        /// <param name="myFileUpload">上传控件</param> 
        /// <returns></returns> 
        public static bool SaveFile(string fpath, FileUpload myFileUpload, List<string> fileExtendsType, out string newFileName, out string errmsg, int filesizeM = 10)
        {
            //check file type 2.generate new filename 3. save.

            bool res = false;

            newFileName = "";
            errmsg = "";
            string fileName = myFileUpload.FileName;
            if (fileName != "")
            {
                string fileExtends = GetFileExtends(fileName);
                if (CheckFileExtends(fileExtends, fileExtendsType))
                {
                    if (myFileUpload.FileBytes.Length <= 1024 * 1024 * filesizeM)
                    {
                        newFileName = Path.GetFileNameWithoutExtension(fileName).Replace(' ', '_') + "." + fileExtends;
                        //newFileName = Path.GetFileNameWithoutExtension(fileName).Replace(' ', '_') + "_" + DateTime.Today.ToString("yyyyMMdd") + "." + fileExtends;
                        string file = fpath + "\\" + newFileName;
                        try
                        {
                            if (Directory.Exists(fpath) == false)
                            {
                                Directory.CreateDirectory(fpath);
                            }
                            myFileUpload.SaveAs(file);
                            res = true;
                        }
                        catch (Exception ee)
                        {
                            res = false;
                            errmsg = ee.Message;
                        }
                    }
                    else
                    {
                        errmsg = "the file is too big.";
                    }
                }
                else
                {
                    errmsg = "invalid file type.";
                }
            }
            else
            {
                errmsg = "empty file";
            }
            return res;
        } 
        #endregion
    }
}