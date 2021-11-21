using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string[] services = { "PolicyAgent", "IKEEXT", "WdiServiceHost", "WdiSystemHost", "wscsvc", "wuauserv" };
            foreach (string i in services)
            {
                ServiceController servis = new ServiceController(i);
                if (servis.Status.ToString() == "Running")
                {
                    //код
                }
                if (servis.Status.ToString() == "Stopped")
                {
                    //код
                }
                
            }

            string r = "";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                ManagementObjectCollection information = searcher.Get();
                if (information != null)
                {
                    foreach (ManagementObject obj in information)
                    {
                        r = obj["Caption"].ToString() + " - " + obj["OSArchitecture"].ToString();
                    }
                }
                r = r.Replace("NT 5.1.2600", "XP");
                r = r.Replace("NT 5.2.3790", "Server 2003");
                label14.Text = label14.Text + r;

            }

            ManagementObjectSearcher searcher8 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");

            foreach (ManagementObject queryObj in searcher8.Get())
            {
                label15.Text = label15.Text + queryObj["Name"].ToString();
            }

            ManagementObjectSearcher searcher11 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");

            foreach (ManagementObject queryObj in searcher11.Get())
            {
                label18.Text = label18.Text + queryObj["VideoProcessor"].ToString();
            }

            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");

            foreach (ManagementObject objram in ramMonitor.Get())
            {
                ulong totalRam = (Convert.ToUInt64(objram["TotalVisibleMemorySize"]) / 1024);
                label19.Text = label19.Text + totalRam + " MB";
            }
        }

        public static long GetDirectorySize(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] subdirectories = Directory.GetDirectories(path);

            long size = files.Sum(x => new FileInfo(x).Length);
            foreach (string s in subdirectories)
                size += GetDirectorySize(s);

            return size;
        }
        int i3 = 0;
        private void guna2GradientCircleButton2_Click(object sender, EventArgs e)
        {
            if (Cleaner.IsBusy == false)
            {
                i3++;
                ScanSize.RunWorkerAsync();
                guna2GradientCircleButton2.Text = "Очистить";
                Console.WriteLine("Сканирование");
            }
            if (ScanSize.IsBusy == false)
            {
                i3 = 0;
                Cleaner.RunWorkerAsync();
                guna2GradientCircleButton2.Text = "Сканировать";
                Console.WriteLine("Очистка");
            }
        }


        private void iconButton3_Click(object sender, EventArgs e)
        {
            panelClean.BringToFront();
        }

        private void TempCleaner_DoWork(object sender, DoWorkEventArgs e)
        {
            guna2GradientCircleButton2.Invoke(new Action(() =>
            {
                guna2GradientCircleButton2.Enabled = false;
            }));

            string tempPath = Path.GetTempPath();
            deleteFiles(tempPath);

            string UpdatesCache = "C:\\Windows\\SoftwareDistribution\\Download";
            deleteFiles(UpdatesCache);

            string DefenderHistory = "C:\\ProgramData\\Microsoft\\Windows Defender\\Scans\\History";
            deleteFiles(DefenderHistory);

            string IssuesWindows1 = "C:\\ProgramData\\Microsoft\\Windows\\WER\\ReportArchive";
            string IssuesWindows4 = "C:\\ProgramData\\Microsoft\\Windows\\WER\\ReportQueue";
            string username = Environment.UserName;
            string IssuesWindows2 = "C:\\Users\\" + username + "\\AppData\\Local\\Microsoft\\Windows\\WER\\ReportArchive";
            string IssuesWindows3 = "C:\\Users\\" + username + "\\AppData\\Local\\Microsoft\\Windows\\WER\\ReportQueue";
            deleteFiles(IssuesWindows1);
            deleteFiles(IssuesWindows2);
            deleteFiles(IssuesWindows3);
            deleteFiles(IssuesWindows4);

            string AppCrashes = "C:\\Users\\"+ username + "\\AppData\\Local\\CrashDumps";
            deleteFiles(AppCrashes);

            guna2GradientCircleButton2.Invoke(new Action(() =>
            {
                guna2GradientCircleButton2.Enabled = true;
            }));
            ScanSize.RunWorkerAsync();
        }

        private void ScanSize_DoWork(object sender, DoWorkEventArgs e)
        {
            string username = Environment.UserName;
            int i = 0;

            guna2GradientCircleButton2.Invoke(new Action(() =>
            {
                guna2GradientCircleButton2.Enabled = false;
            }));

            i = CoutFiles(Path.GetTempPath().ToString()) + CoutFiles("C:\\Windows\\SoftwareDistribution\\Download") + CoutFiles("C:\\ProgramData\\Microsoft\\Windows Defender\\Scans\\History") + CoutFiles("C:\\ProgramData\\Microsoft\\Windows\\WER") + CoutFiles("C:\\Users\\" + username + "\\AppData\\Local\\Microsoft\\Windows\\WER");


            // label5.Text = i.ToString() + " ШТ";

            //var Size1 = GetDirectorySize(Path.GetTempPath()) / 1024 / 1024;
            //var Size2 = GetDirectorySize("C:\\Windows\\SoftwareDistribution\\Download") / 1024 / 1024;
           // var Size3 = GetDirectorySize("C:\\ProgramData\\Microsoft\\Windows Defender\\Scans\\History") / 1024 / 1024;
            var Size1 = SafeEnumerateFiles(Path.GetTempPath(), "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length) / 1024 / 1024;
            var Size2 = SafeEnumerateFiles("C:\\Windows\\SoftwareDistribution\\Download", "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length) / 1024 / 1024;
            var Size3 = SafeEnumerateFiles("C:\\ProgramData\\Microsoft\\Windows Defender\\Scans\\History", "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length) / 1024 / 1024;

            var Size4 = SafeEnumerateFiles("C:\\ProgramData\\Microsoft\\Windows\\WER", "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length) / 1024 / 1024;
            var Size5 = SafeEnumerateFiles("C:\\Users\\" + username + "\\AppData\\Local\\Microsoft\\Windows\\WER", "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length) / 1024 / 1024;

            var Size6 = Size4 + Size5;

            var Size7 = SafeEnumerateFiles("C:\\Users\\"+ username + "\\AppData\\Local\\CrashDumps", "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length) / 1024 / 1024; ;

            var TotalSize = Size1 + Size2 + Size3 + Size6 + Size7;

            label5.Invoke(new Action(() =>
            {
                label5.Text = "Итого: " + i.ToString() + " шт/ " + TotalSize.ToString() + " МБ";
            }));

            label8.Invoke(new Action(() =>
            {
                label8.Text = i.ToString() + " шт/ " + TotalSize.ToString() + " МБ";
            }));


            label2.Invoke(new Action(() =>
            {
                label2.Text = "Папка Temp: " + Size1.ToString() + " МБ";
            }));

            label3.Invoke(new Action(() =>
            {
                label3.Text = "Кэш обновлений: " + Size2.ToString() + " МБ";
            }));

            label4.Invoke(new Action(() =>
            {
                label4.Text = "Кэш защитника: " + Size3.ToString() + " МБ";
            }));

            label22.Invoke(new Action(() =>
            {
                label22.Text = "Отчёты об ошибках: " + Size6.ToString() + " МБ";
            }));

            label22.Invoke(new Action(() =>
            {
                label23.Text = "Отчёты о cбоях программ: " + Size7.ToString() + " МБ";
            }));

            //label6.Text = SizeTemp.ToString() + " МБ";
            guna2GradientCircleButton2.Invoke(new Action(() =>
            {
                guna2GradientCircleButton2.Enabled = true;
            }));
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private bool isDragging = false;
        private Point lastCursor;
        private Point lastForm;

        private void TopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            lastCursor = Cursor.Position;
            lastForm = this.Location;
        }

        private void TopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                this.Location =
                    Point.Add(lastForm, new Size(Point.Subtract(Cursor.Position, new Size(lastCursor))));
            }
        }

        private void TopPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }


        private void panelOptimisator_Paint(object sender, PaintEventArgs e)
        {

        }


        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private async void guna2GradientCircleButton1_Click(object sender, EventArgs e)
        {


            if (guna2CheckBox1.Checked == true)
            {
                string folder = Environment.CurrentDirectory + "\\Bin";
                string app = "-searchprinters.bat";

                //ProcessStartInfo myProcess = new ProcessStartInfo();
                // myProcess.Verb = "runas";
                //myProcess.FileName = "cmd.exe";
                // myProcess.Arguments = @"/k cd " + Application.StartupPath + @"/Bin & -searchprinters.bat";
                //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //myProcess.StartInfo.CreateNoWindow = true;
                // Process.Start(myProcess);

                //System.Diagnostics.Process proc = new System.Diagnostics.Process();
                //proc.StartInfo.FileName = "cmd.exe";
                //proc.StartInfo.Arguments = "/k cd " + Application.StartupPath + "/Bin & -searchprinters.bat";
                //proc.StartInfo.WorkingDirectory = @"C:\Programs";
                //proc.Start();
                Registry.LocalMachine.DeleteSubKeyTree(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\RemoteComputer\NameSpace\{863aa9fd-42df-457b-8e4d-0de1b8015c60}");
            }

            if (guna2CheckBox3.Checked == true)
            {
                //var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\");
                //to do
            }

            if (guna2CheckBox2.Checked == true)
            {
                var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer");
                key.SetValue("AlwaysUnloadDll", "1", RegistryValueKind.DWord);
                key.Close();
            }

            if (guna2CheckBox4.Checked == true)
            {
                var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters");
                key.SetValue("EnableSuperfetch", "0", RegistryValueKind.DWord);
                key.Close();
            }

            if (guna2CheckBox5.Checked == true)
            {
                var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters");
                key.SetValue("EnablePrefetcher", "0", RegistryValueKind.DWord);
                key.Close();
            }

            if (guna2CheckBox6.Checked == true)
            {
                var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl");
                key.SetValue("Win32PrioritySeparation", "6", RegistryValueKind.DWord);
                key.Close();
            }

            if (guna2CheckBox7.Checked == true)
            {
                var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management");
                key.SetValue("DisablePagingExecutive", "1", RegistryValueKind.DWord);
                key.Close();
            }

            if (guna2CheckBox8.Checked == true)
            {
                var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System");
                key.SetValue("EnableSmartScreen", "0", RegistryValueKind.DWord);
                key.Close();
            }




            guna2CheckBox1.Checked = false;
            guna2CheckBox2.Checked = false;
            guna2CheckBox3.Checked = false;
            guna2CheckBox4.Checked = false;
            guna2CheckBox5.Checked = false;
            guna2CheckBox6.Checked = false;
            guna2CheckBox7.Checked = false;
            guna2CheckBox8.Checked = false;

            IProgress<Action> callback = new Progress<Action>(action => action()); // заменитель инвока
            try
            {
                await Task.Run(() => RegistryChecker(callback));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\RemoteComputer\NameSpace\{863aa9fd-42df-457b-8e4d-0de1b8015c60}");
            key.SetValue("", "Printers");
            key.Close();
        }

        private void guna2CheckBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer");
            key.SetValue("AlwaysUnloadDll", 0, RegistryValueKind.DWord);
            key.Close();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters");
            key.SetValue("EnableSuperfetch", "3", RegistryValueKind.DWord);
            key.Close();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters");
            key.SetValue("EnablePrefetcher", "3", RegistryValueKind.DWord);
            key.Close();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl");
            key.SetValue("Win32PrioritySeparation", "2", RegistryValueKind.DWord);
            key.Close();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management");
            key.SetValue("DisablePagingExecutive", "0", RegistryValueKind.DWord);
            key.Close();
        }

        public static long DirSize(DirectoryInfo d)
        {
            long Size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System");
            key.SetValue("EnableSmartScreen", "1", RegistryValueKind.DWord);
            key.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void iconPictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void RamAndCpuChecker_DoWork(object sender, DoWorkEventArgs e)
        {
            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("root\\CIMV2", "Select TotalVisibleMemorySize, FreePhysicalMemory from Win32_OPeratingSystem");
            foreach (var x in searcher2.Get())
            {
                var totalMemory = (ulong)x["TotalVisibleMemorySize"];
                var freeMemory = (ulong)x["FreePhysicalMemory"];

                var totalmem = (double)totalMemory / 1024;
                var freemem = Convert.ToInt32(Math.Round(Convert.ToDecimal((((double)freeMemory / 1024)))));
                var usedmem = Convert.ToInt32(Math.Round(Convert.ToDecimal((((totalmem - freemem))))));

                double UsedMemoryFinally = usedmem / totalmem * 100;
                int UsedMemoryFinallyFinally = Convert.ToInt32(UsedMemoryFinally);

                if (UsedMemoryFinallyFinally > 85)
                {
                    label21.ForeColor = Color.Red;
                }
                else
                {
                    if (UsedMemoryFinallyFinally > 50)
                    {
                        label21.ForeColor = Color.Yellow;
                    }
                    else
                    {
                        label21.ForeColor = Color.Silver;
                    }
                }


                label21.Invoke(new Action(() =>
                {
                    label21.Text = ("RAM: " + Convert.ToString(UsedMemoryFinallyFinally) + "%");
                }));

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    var usage = obj["PercentProcessorTime"];
                    var name = obj["Name"];
                    string usage1 = Convert.ToString(usage);
                    if (Convert.ToInt32(usage1) > 85)
                    {
                        label20.ForeColor = Color.Red;
                    }
                    else
                    {
                        if (Convert.ToInt32(usage1) > 50)
                        {
                            label20.ForeColor = Color.Yellow;
                        }
                        else
                        {
                            label20.ForeColor = Color.Silver;
                        }
                    }


                    label20.Invoke(new Action(() =>
                    {
                        label20.Text = ("CPU: " + usage + "%");
                    }));

                }
            }
        }

        private void Checker_Tick(object sender, EventArgs e)
        {
            if (RamAndCpuChecker.IsBusy == false)
            {
                RamAndCpuChecker.RunWorkerAsync();
            }
        }

        int CoutFiles(string path)
        {
            int filesCount;
            return filesCount = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length;                                                                                                         
        }
        /// <summary>
        /// Возвращает перечисляемую коллекцию имен файлов которые соответствуют шаблону в указанном каталоге, с дополнительным просмотром вложенных каталогов
        /// </summary>
        /// <param name="path">Полный или относительный путь катага в котором выполняется поиск</param>
        /// <param name="searchPattern">Шаблон поиска файлов</param>
        /// <param name="searchOption">Одно из значений перечисления SearchOption указывающее нужно ли выполнять поиск во вложенных каталогах или только в указанном каталоге</param>
        /// <returns>Возвращает перечисляемую коллекцию полных имен файлов</returns>
        private static IEnumerable<string> SafeEnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var dirs = new Stack<string>();
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                string currentDirPath = dirs.Pop();
                if (searchOption == SearchOption.AllDirectories)
                {
                    try
                    {
                        string[] subDirs = Directory.GetDirectories(currentDirPath);
                        foreach (string subDirPath in subDirs)
                        {
                            dirs.Push(subDirPath);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentDirPath, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string filePath in files)
                {
                    yield return filePath;
                }
            }
        }

        void deleteFiles(string Path)
        {
            foreach (var dirPath in Directory.GetDirectories(Path))
            {
                try
                {
                    Directory.Delete(dirPath, recursive: true);
                }
                catch { }
            }

            foreach (var filePath in Directory.EnumerateFiles(Path, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    File.SetAttributes(filePath, File.GetAttributes(Path) & ~FileAttributes.ReadOnly);
                    File.Delete(filePath);
                }
                catch { }
            }

            foreach (var dirPath in Directory.GetDirectories(Path, "*.*", SearchOption.AllDirectories).OrderByDescending(path => path))
            {
                try
                {
                    Directory.Delete(dirPath);
                }
                catch { }
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            IProgress<Action> callback = new Progress<Action>(action => action()); // заменитель инвока
            try
            {
                guna2GradientCircleButton2.PerformClick();
                await Task.Run(() => RegistryChecker(callback));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            panelMain.BringToFront();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            panelOptimisator.BringToFront();
        }


        private void RegistryChecker_DoWork(object sender, DoWorkEventArgs e)
        {
            int CanOptimisated = 0;

            var printers = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\RemoteComputer\NameSpace\{863aa9fd-42df-457b-8e4d-0de1b8015c60}");
            //var onedrive = Registry.LocalMachine;
            string unloaddlls = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";
            string superfetch = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters";
            string prioritycontrol = @"SYSTEM\CurrentControlSet\Control\PriorityControl";
            string disableexecuting = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
            string enablesmartscreen = @"SOFTWARE\Policies\Microsoft\Windows\System";

            Console.WriteLine(1);
            try
            {
                printers.GetValue("");
                CanOptimisated++;
                guna2CheckBox1.Invoke(new Action(() =>
                {
                    guna2CheckBox1.Enabled = true;
                }));
            }
            catch
            {
                guna2CheckBox1.Invoke(new Action(() =>
                {
                    guna2CheckBox1.Enabled = false;
                }));
            }

            Console.WriteLine(2);
            if (CheckRegistry(unloaddlls, "AlwaysUnloadDll", "1") == true)
            {
                guna2CheckBox2.Invoke(new Action(() =>
                {
                    guna2CheckBox2.Enabled = false;
                }));
            }
            else
            {
                CanOptimisated++;
                guna2CheckBox2.Invoke(new Action(() =>
                {
                    guna2CheckBox2.Enabled = true;
                }));
            }

            Console.WriteLine(3);
            if (CheckRegistry(superfetch, "EnableSuperfetch", "0") == true)
            {
                guna2CheckBox4.Invoke(new Action(() =>
                {
                    guna2CheckBox4.Enabled = false;
                }));
            }
            else
            {
                CanOptimisated++;
                guna2CheckBox4.Invoke(new Action(() =>
                {
                    guna2CheckBox4.Enabled = true;
                }));
            }

            Console.WriteLine(4);
            if (CheckRegistry(superfetch, "EnablePrefetcher", "0") == true)
            {
                guna2CheckBox5.Invoke(new Action(() =>
                {
                    guna2CheckBox5.Enabled = false;
                }));
            }
            else
            {
                CanOptimisated++;
                guna2CheckBox5.Invoke(new Action(() =>
                {
                    guna2CheckBox5.Enabled = true;
                }));
            }

            Console.WriteLine(5);
            if (CheckRegistry(prioritycontrol, "Win32PrioritySeparation", "6") == true)
            {
                guna2CheckBox6.Invoke(new Action(() =>
                {
                    guna2CheckBox6.Enabled = false;
                }));
            }
            else
            {
                CanOptimisated++;
                guna2CheckBox6.Invoke(new Action(() =>
                {
                    guna2CheckBox6.Enabled = true;
                }));
            }

            Console.WriteLine(6);
            if (CheckRegistry(disableexecuting, "DisablePagingExecutive", "1") == true)
            {
                guna2CheckBox7.Invoke(new Action(() =>
                {
                    guna2CheckBox7.Enabled = false;
                }));
            }
            else
            {
                CanOptimisated++;
                guna2CheckBox7.Invoke(new Action(() =>
                {
                    guna2CheckBox7.Enabled = true;
                }));
            }

            Console.WriteLine(7);
            if (CheckRegistry(enablesmartscreen, "EnableSmartScreen", "0") == true)
            {
                guna2CheckBox8.Invoke(new Action(() =>
                {
                    guna2CheckBox8.Enabled = false;
                }));
            }
            else
            {
                CanOptimisated++;
                guna2CheckBox8.Invoke(new Action(() =>
                {
                    guna2CheckBox8.Enabled = true;
                }));
            }

            Console.WriteLine(8);

            label9.Invoke(new Action(() =>
            {
                label9.Text = CanOptimisated + " Пунктов";
            }));

        }

        bool CheckRegistry(string path, string nameValue, string value)
        {
            using (RegistryKey path1 = Registry.LocalMachine.OpenSubKey(path))
            {
                return path1.GetValue(nameValue).ToString() == value;
            }
        }

        private void panelOptimisator_Move(object sender, EventArgs e)
        {

        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void RegistryChecker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void RegistryChecker(IProgress<Action> status)
        {
            int CanOptimisated = 0;
            //var onedrive = Registry.LocalMachine;

            const string printerspath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\RemoteComputer\NameSpace\{863aa9fd-42df-457b-8e4d-0de1b8015c60}";
            const string unloaddlls = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";
            const string superfetch = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters";
            const string prioritycontrol = @"SYSTEM\CurrentControlSet\Control\PriorityControl";
            const string disableexecuting = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
            const string enablesmartscreen = @"SOFTWARE\Policies\Microsoft\Windows\System";

            Debug.WriteLine(1); // используйте вместо Console в GUI приложениях
            bool check1 = false;
            try
            {
                // RegistryKey - IDisposable, с ним можно вот так.
                using (RegistryKey printers = Registry.LocalMachine.OpenSubKey(printerspath))
                {
                    printers.GetValue("");
                }
                CanOptimisated++;
            }
            catch { }
            status.Report(() => guna2CheckBox1.Enabled = check1); // было 15 инвоков, стало 8

            Debug.WriteLine(2);
            bool check2 = !CheckRegistry(unloaddlls, "AlwaysUnloadDll", "1");
            if (check2)
                CanOptimisated++;
            status.Report(() => guna2CheckBox2.Enabled = check2);

            Debug.WriteLine(3);
            bool check4 = !CheckRegistry(superfetch, "EnableSuperfetch", "0");
            if (check4)
                CanOptimisated++;
            status.Report(() => guna2CheckBox4.Enabled = check4);

            Debug.WriteLine(4);
            bool check5 = !CheckRegistry(superfetch, "EnablePrefetcher", "0");
            if (check5)
                CanOptimisated++;
            status.Report(() => guna2CheckBox5.Enabled = check5);

            Debug.WriteLine(5);
            bool check6 = !CheckRegistry(prioritycontrol, "Win32PrioritySeparation", "6");
            if (check6)
                CanOptimisated++;
            status.Report(() => guna2CheckBox6.Enabled = check6);

            Debug.WriteLine(6);
            bool check7 = !CheckRegistry(disableexecuting, "DisablePagingExecutive", "1");
            if (check7)
                CanOptimisated++;
            status.Report(() => guna2CheckBox7.Enabled = check7);

            Debug.WriteLine(7);
            bool check8 = !CheckRegistry(enablesmartscreen, "EnableSmartScreen", "0");
            if (check8)
                CanOptimisated++;
            status.Report(() => guna2CheckBox8.Enabled = check8);

            Debug.WriteLine(8);
            status.Report(() => label9.Text = CanOptimisated + " Пунктов");
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            iconButton3.PerformClick();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            iconButton2.PerformClick();
        }
    }
}
