using RawMat.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    public class imgCls
    {
        // ฟังก์ชันสำหรับการปรับขนาดของรูปภาพ
        private Image _defaultImage;
        //private Image _defaultImage = Image.FromFile("img/no_photo.png"); // ประกาศตัวแปรสำหรับเก็บรูปภาพ Default
        
        //private PictureBox _pictureBox = new PictureBox();

        public imgCls()
        {
            // โหลดภาพเริ่มต้นเมื่อสร้างอ็อบเจกต์
            InitializeDefaultImage();
        }

        // ฟังก์ชันสำหรับกำหนดภาพเริ่มต้น
        private void InitializeDefaultImage()
        {
            try
            {
                // พยายามโหลดภาพเริ่มต้นจากไฟล์
                string defaultImagePath = Path.Combine(Application.StartupPath, "img/no_photo.png");
                if (File.Exists(defaultImagePath))
                {
                    _defaultImage = Image.FromFile(defaultImagePath);
                }
                else
                {
                    // หากไม่พบไฟล์ภาพเริ่มต้น ให้สร้างกากบาทสีแดง
                    _defaultImage = CreateDefaultImage();
                    Console.WriteLine("คำเตือน: ไม่พบไฟล์ภาพเริ่มต้น (img/no_photo.png) ใช้กากบาทสีแดงแทน");
                }
            }
            catch (Exception ex)
            {
                // หากเกิดข้อผิดพลาด ให้สร้างกากบาทสีแดง
                _defaultImage = CreateDefaultImage();
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพเริ่มต้น: {ex.Message}");
            }
        }

        // ฟังก์ชันสำหรับสร้างภาพกากบาทสีแดง (เหมือนใน userControlProfile.cs)
        private Image CreateDefaultImage()
        {
            Bitmap defaultImage = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(defaultImage))
            {
                g.Clear(Color.White); // พื้นหลังสีขาว
                using (Pen pen = new Pen(Color.Red, 5))
                {
                    // วาดกากบาทสีแดง
                    g.DrawLine(pen, 10, 10, 90, 90);
                    g.DrawLine(pen, 90, 10, 10, 90);
                }
            }
            return defaultImage;
        }

        // เพิ่มเมธอด CreateDefaultPictureBox ใน imgCls
        public PictureBox CreateDefaultPictureBox()
        {
            Size pictureBoxSize = new Size(100, 100);
            PictureBox pictureBox = new PictureBox
            {
                Size = pictureBoxSize,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            pictureBox.Image = _defaultImage; // ใช้ _defaultImage ที่โหลดไว้แล้ว
            return pictureBox;
        }

        public Image ResizeImage(Image image, int width, int height)
        {
            // สร้าง Bitmap ใหม่ตามขนาดที่ต้องการ
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                // วาดรูปภาพลงใน Bitmap ใหม่ด้วยขนาดที่ต้องการ
                g.DrawImage(image, 0, 0, width, height);
            }
            return resizedImage;
        }

        public Image LoadPackingImage(string fileName)
        {
            try
            {
                // ดึงพาธโฟลเดอร์จาก app.config
                string folderPackingPath = ConfigurationManager.AppSettings["PackingPath"];

                if (string.IsNullOrEmpty(folderPackingPath))
                {
                    Console.WriteLine("ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ PackingPath ใน app.config");
                    return _defaultImage;
                }

                // สร้างพาธสำหรับการค้นหาไฟล์ที่มีชื่อเดียวกัน แต่รองรับหลายนามสกุล
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                foreach (string extension in allowedExtensions)
                {
                    string fullPath = Path.Combine(folderPackingPath, fileName + extension);

                    if (File.Exists(fullPath))
                    {
                        using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }

                // ถ้าไม่พบไฟล์ภาพ ให้คืนภาพเริ่มต้น
                Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ PackingPath");
                return _defaultImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ PackingPath สำหรับ {fileName}: {ex.Message}");
                return _defaultImage;
            }
        }

        public Image LoadRegularImage(string fileName)
        {
            try
            {
                // ดึงพาธโฟลเดอร์จาก app.config
                string folderRegularPath = ConfigurationManager.AppSettings["RegularPath"];

                if (string.IsNullOrEmpty(folderRegularPath))
                {
                    Console.WriteLine("ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ RegularPath ใน app.config");
                    return _defaultImage;
                }

                // สร้างพาธสำหรับการค้นหาไฟล์ที่มีชื่อเดียวกัน แต่รองรับหลายนามสกุล
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                foreach (string extension in allowedExtensions)
                {
                    string fullPath = Path.Combine(folderRegularPath, fileName + extension);

                    if (File.Exists(fullPath))
                    {
                        using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }

                // ถ้าไม่พบไฟล์ภาพ ให้คืนภาพเริ่มต้น
                Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ RegularPath");
                return _defaultImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ RegularPath สำหรับ {fileName}: {ex.Message}");
                return _defaultImage;
            }
        }

        public Image LoadDimensionImage(string fileName)
        {
            try
            {
                // ดึงพาธโฟลเดอร์จาก app.config
                string folderRegularPath = ConfigurationManager.AppSettings["DimensionPath"];

                if (string.IsNullOrEmpty(folderRegularPath))
                {
                    Console.WriteLine("ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ DimensionPath ใน app.config");
                    return _defaultImage;
                }

                // สร้างพาธสำหรับการค้นหาไฟล์ที่มีชื่อเดียวกัน แต่รองรับหลายนามสกุล
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                foreach (string extension in allowedExtensions)
                {
                    string fullPath = Path.Combine(folderRegularPath, fileName + extension);

                    if (File.Exists(fullPath))
                    {
                        using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }

                // ถ้าไม่พบไฟล์ภาพ ให้คืนภาพเริ่มต้น
                Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ DimensionPath");
                return _defaultImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ DimensionPath สำหรับ {fileName}: {ex.Message}");
                return _defaultImage;
            }
        }


        public Image LoadFunctionImage(string fileName)
        {
            try
            {
                // ดึงพาธโฟลเดอร์จาก app.config
                string folderFunctionPath = ConfigurationManager.AppSettings["FunctionPath"];

                if (string.IsNullOrEmpty(folderFunctionPath))
                {
                    Console.WriteLine("ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ FunctionPath ใน app.config");
                    return _defaultImage;
                }

                // สร้างพาธสำหรับการค้นหาไฟล์ที่มีชื่อเดียวกัน แต่รองรับหลายนามสกุล
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                foreach (string extension in allowedExtensions)
                {
                    string fullPath = Path.Combine(folderFunctionPath, fileName + extension);

                    if (File.Exists(fullPath))
                    {
                        using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }

                // ถ้าไม่พบไฟล์ภาพ ให้คืนภาพเริ่มต้น
                Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ FunctionPath");
                return _defaultImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ FunctionPath สำหรับ {fileName}: {ex.Message}");
                return _defaultImage;
            }
        }

        public async Task<List<Image>> LoadFunctionImagesAsync(string fileName)
        {
            return await Task.Run(() => LoadFunctionImages(fileName)); // Wrap method เก่าให้ async
        }

        public List<Image> LoadFunctionImages(string fileName)
        {
            List<Image> images = new List<Image>();
            try
            {
                // ดึงพาธโฟลเดอร์จาก app.config
                string folderFunctionPath = ConfigurationManager.AppSettings["FunctionPath"];

                if (string.IsNullOrEmpty(folderFunctionPath))
                {
                    Console.WriteLine("ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ FunctionPath ใน app.config");
                    return images; // return empty list
                }

                // ค้นหาไฟล์ที่ match fileName + "_*" ด้วย extension ที่รองรับ
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                string[] files = Directory.GetFiles(folderFunctionPath, fileName + "_*.*");

                // เรียงไฟล์ตามตัวเลขหลัง "_" เพื่อลำดับถูกต้อง (เช่น AAA_1, AAA_2, AAA_10)
                var orderedFiles = files.OrderBy(f =>
                {
                    string name = Path.GetFileNameWithoutExtension(f);
                    if (name.StartsWith(fileName + "_"))
                    {
                        if (int.TryParse(name.Substring(fileName.Length + 1), out int num))
                            return num;
                    }
                    return 0; // ถ้า parse ไม่ได้ ให้เรียงก่อน
                });

                foreach (string filePath in orderedFiles)
                {
                    // Skip ถ้าไฟล์หาย (ช่วยป้องกัน hang จาก network path ที่หลุดชั่วคราว)
                    if (!File.Exists(filePath)) continue;

                    string extension = Path.GetExtension(filePath).ToLower();
                    if (allowedExtensions.Contains(extension))
                    {
                        try
                        {
                            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                Image img = Image.FromStream(stream);
                                images.Add(img);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ข้ามไฟล์เสีย: {filePath} - {ex.Message}");
                            // ไม่ add ภาพที่โหลดไม่ได้
                        }
                    }
                }

                if (images.Count == 0)
                {
                    // Fallback: ลองโหลดไฟล์ปกติโดยไม่มี "_*" (ใช้ method LoadFunctionImage เก่า)
                    Image defaultImg = LoadFunctionImage(fileName);
                    if (defaultImg != null && defaultImg != _defaultImage) // สมมติ _defaultImage เป็น null หรือภาพ placeholder
                    {
                        images.Add(defaultImg);
                    }
                    else
                    {
                        Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ FunctionPath");
                    }
                }

                return images;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ FunctionPath สำหรับ {fileName}: {ex.Message}");
                return images; // return empty list on error
            }
        }


        public async Task<List<Image>> LoadImagesAsync(string appSettingPath, string fileName)
        {
            return await Task.Run(() => LoadImages(appSettingPath,fileName)); // Wrap method เก่าให้ async
        }

        //public List<Image> LoadImages(string appSettingPath, string fileName)
        //{
        //    List<Image> images = new List<Image>();
        //    try
        //    {
        //        // ดึงพาธโฟลเดอร์จาก app.config โดยใช้ appSettingPath ที่ส่งมา (เช่น "FunctionPath", "DimensionPath")
        //        string folderPath = ConfigurationManager.AppSettings[appSettingPath];

        //        if (string.IsNullOrEmpty(folderPath))
        //        {
        //            Console.WriteLine($"ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ {appSettingPath} ใน app.config");
        //            return images; // return empty list
        //        }

        //        // ค้นหาไฟล์ที่ match fileName + "_*" ด้วย extension ที่รองรับ
        //        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
        //        string[] files = Directory.GetFiles(folderPath, fileName + "_*.*");

        //        // เรียงไฟล์ตามตัวเลขหลัง "_" เพื่อลำดับถูกต้อง (เช่น AAA_1, AAA_2, AAA_10)
        //        var orderedFiles = files.OrderBy(f =>
        //        {
        //            string name = Path.GetFileNameWithoutExtension(f);
        //            if (name.StartsWith(fileName + "_"))
        //            {
        //                if (int.TryParse(name.Substring(fileName.Length + 1), out int num))
        //                    return num;
        //            }
        //            return 0; // ถ้า parse ไม่ได้ ให้เรียงก่อน
        //        });

        //        foreach (string filePath in orderedFiles)
        //        {
        //            // Skip ถ้าไฟล์หาย (ช่วยป้องกัน hang จาก network path ที่หลุดชั่วคราว)
        //            if (!File.Exists(filePath)) continue;

        //            string extension = Path.GetExtension(filePath).ToLower();
        //            if (allowedExtensions.Contains(extension))
        //            {
        //                try
        //                {
        //                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //                    {
        //                        Image img = Image.FromStream(stream);
        //                        images.Add(img);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine($"ข้ามไฟล์เสีย: {filePath} - {ex.Message}");
        //                    // ไม่ add ภาพที่โหลดไม่ได้
        //                }
        //            }
        //        }

        //        if (images.Count == 0)
        //        {
        //            // Fallback: ลองโหลดไฟล์ปกติโดยไม่มี "_*" (logic คล้าย LoadFunctionImage เดิม)
        //            Image defaultImg = LoadSingleImage(folderPath, fileName);
        //            if (defaultImg != null && defaultImg != _defaultImage)
        //            {
        //                images.Add(defaultImg);
        //            }
        //            else
        //            {
        //                Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ {appSettingPath}");
        //            }
        //        }

        //        return images;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ {appSettingPath} สำหรับ {fileName}: {ex.Message}");
        //        return images; // return empty list on error
        //    }
        //}

        public List<Image> LoadImages(string appSettingPath, string fileName)
        {
            List<Image> images = new List<Image>();
            try
            {
                // ดึงพาธโฟลเดอร์จาก app.config โดยใช้ appSettingPath ที่ส่งมา (เช่น "FunctionPath", "DimensionPath")
                string folderPath = ConfigurationManager.AppSettings[appSettingPath];

                if (string.IsNullOrEmpty(folderPath))
                {
                    Console.WriteLine($"ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ {appSettingPath} ใน app.config");
                    return images; // return empty list
                }

                // ค้นหาไฟล์ที่เริ่มต้นด้วย fileName + ตามด้วยอะไรก็ได้ (ไม่สนใจส่วนข้างหลัง) ด้วย extension ที่รองรับ
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                string[] files = Directory.GetFiles(folderPath, fileName + "*.*");

                // เรียงไฟล์ตามส่วนหลัง fileName: ถ้าเป็นตัวเลขให้เรียงตามตัวเลข (pad เป็น string เพื่อ sort numerical) มิเช่นนั้นเรียงตาม alphabetical ของส่วนนั้น
                var orderedFiles = files.OrderBy(f =>
                {
                    string fullName = Path.GetFileNameWithoutExtension(f);
                    if (fullName.StartsWith(fileName))
                    {
                        string afterPart = fullName.Substring(fileName.Length);
                        if (int.TryParse(afterPart, out int num))
                        {
                            return num.ToString("D10"); // pad ด้วย 0 เพื่อ sort numerical เป็น lexical (D10 สำหรับตัวเลขสูงสุด 10 หลัก)
                        }
                        else
                        {
                            return afterPart; // เรียงตาม string ถ้าไม่ใช่ตัวเลข
                        }
                    }
                    return fullName; // fallback ถ้าไม่ match
                });

                foreach (string filePath in orderedFiles)
                {
                    // Skip ถ้าไฟล์หาย (ช่วยป้องกัน hang จาก network path ที่หลุดชั่วคราว)
                    if (!File.Exists(filePath)) continue;

                    string extension = Path.GetExtension(filePath).ToLower();
                    if (allowedExtensions.Contains(extension))
                    {
                        try
                        {
                            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                Image img = Image.FromStream(stream);
                                images.Add(img);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ข้ามไฟล์เสีย: {filePath} - {ex.Message}");
                            // ไม่ add ภาพที่โหลดไม่ได้
                        }
                    }
                }

                if (images.Count == 0)
                {
                    // Fallback: ลองโหลดไฟล์ปกติโดยตรง (logic คล้าย LoadFunctionImage เดิม)
                    Image defaultImg = LoadSingleImage(folderPath, fileName);
                    if (defaultImg != null && defaultImg != _defaultImage)
                    {
                        images.Add(defaultImg);
                    }
                    else
                    {
                        Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ {appSettingPath}");
                    }
                }

                return images;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ {appSettingPath} สำหรับ {fileName}: {ex.Message}");
                return images; // return empty list on error
            }
        }

        // Helper method ใหม่สำหรับ fallback (load single file) - ใช้แทน logic ซ้ำๆ ใน method เดิม
        public Image LoadSingleImage(string appSettingPath, string fileName)
        {

            string folderPath = ConfigurationManager.AppSettings[appSettingPath];

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

            foreach (string extension in allowedExtensions)
            {
                string fullPath = Path.Combine(folderPath, fileName + extension);

                if (File.Exists(fullPath))
                {
                    try
                    {
                        using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ข้ามไฟล์ single เสีย: {fullPath} - {ex.Message}");
                    }
                }
            }

            return _defaultImage;
        }

        public Image LoadCavityImage(string fileName)
        {
            try
            {
                // ดึงพาธโฟลเดอร์จาก app.config
                string folderCavityPath = ConfigurationManager.AppSettings["CavityPath"];

                if (string.IsNullOrEmpty(folderCavityPath))
                {
                    Console.WriteLine("ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ CavityPath ใน app.config");
                    return _defaultImage;
                }

                // สร้างพาธสำหรับการค้นหาไฟล์ที่มีชื่อเดียวกัน แต่รองรับหลายนามสกุล
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                foreach (string extension in allowedExtensions)
                {
                    string fullPath = Path.Combine(folderCavityPath, fileName + extension);

                    if (File.Exists(fullPath))
                    {
                        using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }

                // ถ้าไม่พบไฟล์ภาพ ให้คืนภาพเริ่มต้น
                Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ CavityPath");
                return _defaultImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ CavityPath สำหรับ {fileName}: {ex.Message}");
                return _defaultImage;
            }
        }

        public Image LoadMaterialImage(string fileName)
        {
            try
            {
                // ดึงพาธโฟลเดอร์จาก app.config
                string folderMaterialPath = ConfigurationManager.AppSettings["MaterialPath"];

                if (string.IsNullOrEmpty(folderMaterialPath))
                {
                    Console.WriteLine("ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ RegularPath ใน app.config");
                    return _defaultImage;
                }

                // สร้างพาธสำหรับการค้นหาไฟล์ที่มีชื่อเดียวกัน แต่รองรับหลายนามสกุล
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                foreach (string extension in allowedExtensions)
                {
                    string fullPath = Path.Combine(folderMaterialPath, fileName + extension);

                    if (File.Exists(fullPath))
                    {
                        using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }

                // ถ้าไม่พบไฟล์ภาพ ให้คืนภาพเริ่มต้น
                Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ MaterialPath");
                return _defaultImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ MaterialPath สำหรับ {fileName}: {ex.Message}");
                return _defaultImage;
            }
        }

        public Image LoadAppearImage(string fileName)
        {
            try
            {
                // ดึงพาธโฟลเดอร์จาก app.config
                string folderRegularPath = ConfigurationManager.AppSettings["AppearancePath"];

                if (string.IsNullOrEmpty(folderRegularPath))
                {
                    Console.WriteLine("ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ AppearancePath ใน app.config");
                    return _defaultImage;
                }

                // สร้างพาธสำหรับการค้นหาไฟล์ที่มีชื่อเดียวกัน แต่รองรับหลายนามสกุล
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                foreach (string extension in allowedExtensions)
                {
                    string fullPath = Path.Combine(folderRegularPath, fileName + extension);

                    if (File.Exists(fullPath))
                    {
                        using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }

                // ถ้าไม่พบไฟล์ภาพ ให้คืนภาพเริ่มต้น
                Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ AppearancePath");
                return _defaultImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ AppearancePath สำหรับ {fileName}: {ex.Message}");
                return _defaultImage;
            }
        }

        public PictureBox LoadEmployeeImage(string fileName)
        {
            Size pictureBoxSize = new Size(100, 100);
            PictureBox pictureBox = new PictureBox
            {
                Size = pictureBoxSize,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            try
            {
                string folderEmployeePath = ConfigurationManager.AppSettings["EmpImgPath"];
                if (string.IsNullOrEmpty(folderEmployeePath))
                {
                    Console.WriteLine("ข้อผิดพลาด: ไม่พบพาธโฟลเดอร์ EmpImgPath ใน app.config");
                    return CreateDefaultPictureBox();
                }

                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                foreach (string extension in allowedExtensions)
                {
                    string fullPath = Path.Combine(folderEmployeePath, fileName + extension);
                    if (File.Exists(fullPath))
                    {
                        using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            pictureBox.Image = Image.FromStream(stream);
                            return pictureBox;
                        }
                    }
                }

                Console.WriteLine($"ไม่พบไฟล์ภาพสำหรับ {fileName} ในโฟลเดอร์ EmpImgPath");
                return CreateDefaultPictureBox();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ข้อผิดพลาดในการโหลดภาพ EmpImgPath สำหรับ {fileName}: {ex.Message}");
                return CreateDefaultPictureBox();
            }
        }
    }
}
