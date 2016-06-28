using System.IO;
using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace MegaZord.Library.Helpers
{
    public class MZHelperFile
    {
        /// <summary>
        /// Remove o arquivo informado como parâmetro
        /// </summary>
        /// <param name="fileName">Caminho completo do arquivo para ser exlcuido</param>
        public static void Delete(string fileName)
        {
            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
        }
        /// <summary>
        /// Define se existe ou não um determinado arquivo
        /// </summary>
        /// <param name="fileName">Caminho completo do arquivo</param>
        /// <returns>True se o arquivo existir e false se não existir</returns>
        public static bool Exists(string fileName)
        {
            return System.IO.File.Exists(fileName);
        }

        /// <summary>
        /// Cria um arquivo no caminho completo informado com o conteudo informado
        /// </summary>
        /// <param name="filename">Caminho do arquivo a ser criado</param>
        /// <param name="content">Conteúdo do arquivos</param>
        /// <returns>True se consegue criar o arquivo e false caso não consiga</returns>
        public static bool Create(string filename, string content)
        {
            var bcreated = true;

            if (!MZHelperFile.Exists(filename))
            {
                var fi = new System.IO.FileInfo(filename);
                try
                {
                    //Create the file.
                    using (FileStream fs = fi.Create())
                    {
                        Byte[] info =
                            new UTF8Encoding(true).GetBytes(content);

                        //Add some information to the file.
                        fs.Write(info, 0, info.Length);
                        fs.Flush();
                        
                    }
                }
                catch
                {
                    bcreated = false;

                }
            }
            return bcreated;
        }


        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static byte[] resizeImage(int newWidth, int newHeight, byte[] originalFIle)
        {
            Bitmap bmPhoto = null;
            Graphics grPhoto = null;
            Image imgPhoto = null;
            MemoryStream stream = null;
            try
            {
                using (stream = new MemoryStream(originalFIle))
                {


                    using (imgPhoto = Image.FromStream(stream))
                    {

                        int sourceWidth = imgPhoto.Width;
                        int sourceHeight = imgPhoto.Height;

                        //Consider vertical pics
                        if (sourceWidth < sourceHeight)
                        {
                            int buff = newWidth;

                            newWidth = newHeight;
                            newHeight = buff;
                        }


                        int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
                        float nPercent = 0, nPercentW = 0, nPercentH = 0;

                        nPercentW = ((float)newWidth / (float)sourceWidth);
                        nPercentH = ((float)newHeight / (float)sourceHeight);
                        if (nPercentH < nPercentW)
                        {
                            nPercent = nPercentH;
                            destX = System.Convert.ToInt16((newWidth -
                                      (sourceWidth * nPercent)) / 2);
                        }
                        else
                        {
                            nPercent = nPercentW;
                            destY = System.Convert.ToInt16((newHeight -
                                      (sourceHeight * nPercent)) / 2);
                        }

                        int destWidth = (int)(sourceWidth * nPercent);
                        int destHeight = (int)(sourceHeight * nPercent);



                        
                        

                        bmPhoto = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
                        bmPhoto.MakeTransparent(Color.Transparent);    
                        using (grPhoto = Graphics.FromImage(bmPhoto))
                        {
                            grPhoto.Clear(Color.Transparent);

                            grPhoto.Flush();
                            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            grPhoto.DrawImage(imgPhoto,
                                new Rectangle(destX, destY, destWidth, destHeight),
                                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                                GraphicsUnit.Pixel);
                        }
                    }

                }
            }
            finally
            {
                //grPhoto.Dispose();
                //imgPhoto.Dispose();
                //stream.Close();
            }
            ImageConverter converter = new ImageConverter();
            var retorno = (byte[])converter.ConvertTo(bmPhoto, typeof(byte[]));
            return retorno;

        }
    }
}
