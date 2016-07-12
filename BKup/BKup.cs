using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BKup {
    public static class BKup {
        public static ArrayList ReadEntry(string fullpath_config) {
            ArrayList arr = new ArrayList();
            Common.Entry data = new Common.Entry();
            try {
                StreamReader reader = new StreamReader(fullpath_config, System.Text.Encoding.Default);

                string line = reader.ReadLine();
                while (line != null) {
                    switch (IsPathToPath(line)) {
                        case Common.Status.Normal://正常行
                            data.status = Common.Status.Normal;
                            data.src_path = line.Split('>')[0];
                            data.tar_path = line.Split('>')[1];
                            arr.Add(data);
                            break;
                        case Common.Status.Paused://暂停行
                            data.status = Common.Status.Paused;
                            data.src_path = line.Split('>')[0].Substring(2, line.Split('>')[0].Length - 2);
                            data.tar_path = line.Split('>')[1];
                            arr.Add(data);
                            break;
                        case Common.Status.SrcNotExists://备份文件不存在
                            data.status = Common.Status.SrcNotExists;
                            data.src_path = line.Split('>')[0];
                            data.tar_path = line.Split('>')[1];
                            arr.Add(data);
                            break;
                    }

                    line = reader.ReadLine();
                }

                reader.Close();
            } catch (Exception e) {
                //MsgWriter.LogWriter("ERROR:0x000009 " + e.Message);//data文件读取出错
            }

            return arr;
        }

        /// <summary>
        /// 为备份创建的进程,分析备份类型(默认为增益备份),复制文件等操作
        /// </summary>
        /// <param name="obj">DATA单项</param>
        public static void Backup(string src_path, string tar_path) {
            #region 增量备份
            /* try
            {
            }
            catch (Exception e)
            {

            }*/
            #endregion

            #region 增益备份
            try {
                if (File.Exists(src_path)) {//文件备份

                    CopyFile(src_path, tar_path);
                    //return;//文件备份成功
                } else{ //文件夹备份
                    ////////////////////////////////////////////////////
                    CopyDirectory(src_path, tar_path);///
                    //////////////////////////////////////////////////
                    //return;//文件夹备份成功
                }
            } catch (Exception e) {
                //MsgWriter.LogWriter("ERROR:0x000002 " + e.Message);
                //return;//参数错误/文件/文件夹访问失败
            }
            #endregion

            return;
        }

        /// <summary>
        /// 备份文件夹成目标文件夹,备份时会检查LastAccessTime是否一致,一致则不备份
        /// </summary>
        private static int CopyDirectory(string sourcePath, string targetPath)//复制文件夹
        {//C:\NINI  D:\NINI
            try {
                if (Directory.Exists(targetPath) == false) {
                    Directory.CreateDirectory(targetPath);
                }
                /*else if (Directory.GetLastWriteTime(sourcePath) == Directory.GetLastWriteTime(targetPath))
                { 
                    return 1;//文件夹不用复制
                }*/

                string[] files = Directory.GetFileSystemEntries(sourcePath);
                foreach (string file in files) {
                    string fileName = System.IO.Path.GetFileName(file);
                    if (Directory.Exists(file))//如果是文件夹
                    {
                        if (Directory.GetLastWriteTime(file) != Directory.GetLastWriteTime(targetPath)) {
                            CopyDirectory(file, targetPath + "\\" + fileName);
                        }
                    } else//是文件
                    {
                        if (Directory.Exists(targetPath) == false) {
                            Directory.CreateDirectory(targetPath);
                        }

                        CopyFile(file, targetPath + "\\" + fileName);
                    }
                }
            } catch (Exception e) {
                //MsgWriter.LogWriter("ERROR:0x000007 " + e.Message);
                return -1;//文件夹复制错误
            }

            return 0;
        }

        /// <summary>
        /// 备份文件成目标文件,备份时会检查LastAccessTime是否一致,一致则不备份
        /// </summary>
        private static int CopyFile(string sourcePath, string targetPath)//复制文件
        {
            try {
                if (File.Exists(targetPath) && File.GetLastWriteTime(sourcePath) == File.GetLastWriteTime(targetPath))
                    return 1;//文件不用复制 

                File.Copy(sourcePath, targetPath, true);

                //MsgWriter.InfWriter(sourcePath);
            } catch (Exception e) {
                //MsgWriter.LogWriter("ERROR:0x000008 " + e.Message);
                return -1;//文件夹复制错误
            }

            return 0;//文件复制成功
        }

        /// <summary>
        /// 检测备份项是否合法
        /// </summary>
        /// <param name="pathToPath">格式为"[//]SourceFile>TargetFile"</param>
        private static Common.Status IsPathToPath(string pathToPath) {
            try {
                if (pathToPath.Contains('>') == false || pathToPath.Contains(':') == false || pathToPath.Contains('\\') == false)
                    return Common.Status.Error;//错误行

                string from = pathToPath.Split('>')[0];

                if (File.Exists(from) == false && Directory.Exists(from) == false) {
                    if (pathToPath[0] == '/' && pathToPath[1] == '/')
                        return Common.Status.Ignored;//备注行
                    else if (pathToPath[0] == '\\' && pathToPath[1] == 'P') {
                        return Common.Status.Paused;//暂停行
                    } else {
                        //MsgWriter.LogWriter("ERROR:0x000005 " + pathToPath);
                        return Common.Status.SrcNotExists;//备份文件/文件夹不存在
                    }
                }
            } catch (Exception e) {
                //MsgWriter.LogWriter("ERROR:0x000006 " + e.Message);
                return Common.Status.Unknown;//未知错误
            }
            return 0;//正确
        }
    }
}
