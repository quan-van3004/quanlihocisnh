using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace Name.Models
{
    [Serializable]
    public abstract class Person : ISerializable
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        public Person() { }

        public Person(string name, int age, string address)
        {
            Name = name;
            Age = age;
            Address = address;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Age", Age);
            info.AddValue("Address", Address);
        }

        protected Person(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Age = info.GetInt32("Age");
            Address = info.GetString("Address");
        }

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"Tên: {Name}, Tuổi: {Age}, Địa chỉ: {Address}");
        }
    }

    [Serializable]
    public class Teacher : Person
    {
        public List<string> Subjects { get; set; }

        public Teacher()
        {
            Subjects = new List<string>();
        }

        public Teacher(string name, int age, string address, string subject)
            : base(name, age, address)
        {
            Subjects = new List<string> { subject };
        }

        public void AddSubject(string subject)
        {
            if (!Subjects.Contains(subject))
            {
                Subjects.Add(subject);
            }
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Môn giảng dạy: {string.Join(", ", Subjects)}");
        }
    }

    [Serializable]
    public class Class
    {
        public string ClassName { get; set; }
        public List<Student> Students { get; set; }

        public Class(string className)
        {
            ClassName = className;
            Students = new List<Student>();
        }

        public void AddStudent(Student student)
        {
            Students.Add(student);
        }

        public void RemoveStudent(string studentId)
        {
            var student = Students.Find(s => s.StudentId == studentId);
            if (student != null)
            {
                Students.Remove(student);
            }
        }
    }

    [Serializable]
    public class Score : ISerializable
    {
        public string Subject { get; set; }
        public double MidtermScore { get; set; }
        public double FinalScore { get; set; }

        public Score() { }

        public Score(string subject, double midtermScore, double finalScore)
        {
            Subject = subject;
            MidtermScore = midtermScore;
            FinalScore = finalScore;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Subject", Subject);
            info.AddValue("MidtermScore", MidtermScore);
            info.AddValue("FinalScore", FinalScore);
        }

        public double CalculateFinalScore()
        {
            return (MidtermScore * 0.4) + (FinalScore * 0.6);
        }

        public void DisplayScore()
        {
            Console.WriteLine($"Môn học: {Subject}, Giữa kỳ: {MidtermScore}, Cuối kỳ: {FinalScore}, Điểm cuối: {CalculateFinalScore():F2}");
        }
    }
    [Serializable]
    public class Report
    {
        public string StudentId { get; set; }
        public string TeacherName { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }

        public Report(string studentId, string teacherName, string content, string subject)
        {
            StudentId = studentId;
            TeacherName = teacherName;
            Content = content;
            Date = DateTime.Now;
            Subject = subject;
        }
    }


    [Serializable]
    public class Student : Person
    {
        public string StudentId { get; set; }
        public string Class { get; set; }
        public List<Score> Scores { get; set; }

        public Student() { }

        public Student(string name, int age, string address, string studentId, string className)
            : base(name, age, address)
        {
            StudentId = studentId;
            Class = className;
            Scores = new List<Score>();
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Mã học sinh: {StudentId}, Lớp: {Class}");
        }

        public double CalculateAverageScore()
        {
            double total = 0;
            foreach (Score score in Scores)
            {
                total += score.CalculateFinalScore();
            }
            return Scores.Count > 0 ? total / Scores.Count : 0;
        }

        public void AddScore(Score score)
        {
            Scores.Add(score);
        }

        public bool ScoreExists(string subject)
        {
            return Scores.Exists(s => s.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));
        }

        public void UpdateScore(string subject, double midtermScore, double finalScore)
        {
            Score score = Scores.Find(s => s.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));
            if (score != null)
            {
                score.MidtermScore = midtermScore;
                score.FinalScore = finalScore;
                Console.WriteLine("Điểm môn học đã được cập nhật.");
            }
            else
            {
                Console.WriteLine("Không tìm thấy môn học này.");
            }
        }
    }

    public class FileHandler
    {
        public static void SaveDataToFile(string filePath, List<Student> students)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(students, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static List<Student> LoadDataFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return new List<Student>();

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Student>>(jsonString) ?? new List<Student>();
        }

        public static void SaveLoginData(string filePath, List<LoginInfo> loginInfos)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(loginInfos, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static List<LoginInfo> LoadLoginData(string filePath)
        {
            if (!File.Exists(filePath)) return new List<LoginInfo>();

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<LoginInfo>>(jsonString) ?? new List<LoginInfo>();
        }

        public static void SaveReportsToFile(string filePath, List<Report> reports)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(reports, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static List<Report> LoadReports(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File báo cáo không tồn tại.");
                return new List<Report>();
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Report>>(jsonString) ?? new List<Report>();
        }
    }


    [Serializable]
    public class LoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Subject { get; set; }

        public LoginInfo() { }

        public LoginInfo(string username, string password, string role, string subject = "")
        {
            Username = username;
            Password = password;
            Role = role;
            Subject = subject;
        }
    }


    public class Menu
    {
        public static void ShowAdminMenu()
        {
            Console.WriteLine("=== Hệ thống quản lý học sinh ===");
            Console.WriteLine("1. Thêm học sinh");
            Console.WriteLine("2. Thêm hoặc sửa điểm");
            Console.WriteLine("3. Sửa thông tin của học sinh");
            Console.WriteLine("4. Tìm kiếm và xem điểm của học sinh");
            Console.WriteLine("5. Danh sách học sinh");
            Console.WriteLine("6. Thoát");
        }

        public static void ShowTeacherMenu()
        {
            Console.WriteLine("=== Hệ thống quản lý học sinh - Giáo viên ===");
            Console.WriteLine("1. Thêm hoặc sửa điểm môn học");
            Console.WriteLine("2. Tổng kết môn học");
            Console.WriteLine("3. Gửi báo cáo tình hình học tập cho học sinh");
            Console.WriteLine("4. Thoát");
        }


        public static void ShowStudentMenu()
        {
            Console.WriteLine("=== Hệ thống quản lý học sinh - Học sinh ===");
            Console.WriteLine("1. Xem điểm");
            Console.WriteLine("2. Xem báo cáo từ giáo viên");
            Console.WriteLine("3. Thoát");
        }

    }

    public class Program
    {
        static List<Student> students = new List<Student>();
        static string studentFilePath = "students.json";
        static string loginFilePath = "login.json";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            List<LoginInfo> loginInfos = FileHandler.LoadLoginData(loginFilePath);
            LoginInfo loginInfo = Login(loginInfos);
            students = FileHandler.LoadDataFromFile(studentFilePath);

            if (loginInfo == null) return;

            if (loginInfo.Role == "student")
            {
                while (true)
                {
                    Menu.ShowStudentMenu();
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            SearchStudentAndDisplayScores();
                            break;
                        case "2":

                            ViewReports(loginInfo.Username);

                            break;
                        case "3":
                            return;
                        default:
                            Console.WriteLine("Tùy chọn không hợp lệ, vui lòng thử lại.");
                            break;
                    }
                }
            }

            if (loginInfo.Role == "teacher")
            {

                if (string.IsNullOrEmpty(loginInfo.Subject))
                {
                    Console.WriteLine("Vui lòng nhập môn học bạn giảng dạy:");
                    loginInfo.Subject = Console.ReadLine();
                    FileHandler.SaveLoginData(loginFilePath, loginInfos);
                }

                while (true)
                {
                    Menu.ShowTeacherMenu();
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            AddOrUpdateScore(loginInfo.Subject);
                            break;
                        case "2":
                            SummarizeBySubject(loginInfo.Subject);
                            break;
                        case "3":
                            SendReport(loginInfo.Username, loginInfo.Subject);

                            break;

                        case "4":
                            return;
                        default:
                            Console.WriteLine("Tùy chọn không hợp lệ, vui lòng thử lại.");
                            break;
                    }
                }
            }


            else if (loginInfo.Role == "admin")
            {
                while (true)
                {
                    Menu.ShowAdminMenu();
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            AddStudent();
                            break;
                        case "2":
                            AddOrUpdateScore();
                            break;
                        case "3":
                            UpdateStudentInfo();
                            break;
                        case "4":
                            SearchStudentAndDisplayScores();
                            break;
                        case "5":
                            SummarizeByClass();
                            break;
                        case "6":
                            return;
                        default:
                            Console.WriteLine("Tùy chọn không hợp lệ, vui lòng thử lại.");
                            break;
                    }
                }
            }
        }

        public static void SendReport(string teacherName, string subject)
        {
            Console.WriteLine("Nhập mã học sinh:");
            string studentId = Console.ReadLine();

            Console.WriteLine("Nhập nội dung báo cáo:");
            string content = Console.ReadLine();


            Report report = new Report(studentId, teacherName, content, subject);


            SaveReport(report);

            Console.WriteLine("Báo cáo đã được gửi thành công.");
        }




        public static void ViewReports(string studentId)
        {
            List<Report> reports = LoadReports("Report.json");
            var studentReports = reports
                .Where(r => r.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase))
                .ToList();



            foreach (var report in reports)
            {
                Console.WriteLine($"Báo cáo từ {report.TeacherName} vào {report.Date}: {report.Content} (Môn học: {report.Subject})");
            }

            if (studentReports.Any())
            {
                Console.WriteLine("Các báo cáo từ giáo viên:");
                foreach (var report in studentReports)
                {
                    Console.WriteLine($"- Báo cáo từ {report.TeacherName} vào {report.Date}: {report.Content} (Môn học: {report.Subject})");
                }
            }
            else
            {
                Console.WriteLine("Không có báo cáo nào từ giáo viên cho bạn.");
            }
        }



        public static void SaveReport(Report report)
        {
            string filePath = "Report.json";
            List<Report> reports = LoadReports(filePath);
            reports.Add(report);
            FileHandler.SaveReportsToFile(filePath, reports);
        }


        public static List<Report> LoadReports(string filePath)
        {
            if (!File.Exists(filePath)) return new List<Report>();

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Report>>(jsonString) ?? new List<Report>();
        }

        public static LoginInfo Login(List<LoginInfo> loginInfos)
        {
            Console.WriteLine("Vui lòng đăng nhập:");
            Console.Write("Tên đăng nhập: ");
            string username = Console.ReadLine();
            Console.Write("Mật khẩu: ");
            string password = Console.ReadLine();

            foreach (var info in loginInfos)
            {
                if (info.Username == username && info.Password == password)
                {
                    Console.WriteLine("Đăng nhập thành công!");
                    return info;
                }
            }
            Console.WriteLine("Sai tên đăng nhập hoặc mật khẩu.");
            return null;
        }

        public static void AddStudent()
        {
            Console.WriteLine("Nhập tên học sinh:");
            string name = Console.ReadLine();
            Console.WriteLine("Nhập tuổi:");
            int age = int.Parse(Console.ReadLine());
            Console.WriteLine("Nhập địa chỉ:");
            string address = Console.ReadLine();
            Console.WriteLine("Nhập mã học sinh:");
            string studentId = Console.ReadLine();
            Console.WriteLine("Nhập lớp:");
            string className = Console.ReadLine();

            Student student = new Student(name, age, address, studentId, className);
            students.Add(student);
            FileHandler.SaveDataToFile(studentFilePath, students);
            Console.WriteLine("Thêm học sinh thành công.");
        }

        public static void AddOrUpdateScore(string teacherSubject = "")
        {
            Console.WriteLine("Nhập mã học sinh:");
            string studentId = Console.ReadLine();
            Student student = students.Find(s => s.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase));

            if (student != null)
            {
                Console.WriteLine("Nhập môn học:");
                string subject = Console.ReadLine();

                if (!string.IsNullOrEmpty(teacherSubject) && !subject.Equals(teacherSubject, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Bạn không được phép sửa điểm cho môn học này.");
                    return;
                }

                Console.WriteLine("Nhập điểm giữa kỳ:");
                double midtermScore = double.Parse(Console.ReadLine());
                Console.WriteLine("Nhập điểm cuối kỳ:");
                double finalScore = double.Parse(Console.ReadLine());

                if (student.ScoreExists(subject))
                {
                    student.UpdateScore(subject, midtermScore, finalScore);
                }
                else
                {
                    student.AddScore(new Score(subject, midtermScore, finalScore));
                    Console.WriteLine("Điểm môn học đã được thêm.");
                }
                FileHandler.SaveDataToFile(studentFilePath, students);
            }
            else
            {
                Console.WriteLine("Không tìm thấy học sinh.");
            }
        }


        public static void SearchStudentAndDisplayScores()
        {
            students = FileHandler.LoadDataFromFile(studentFilePath);
            Console.WriteLine("Nhập mã học sinh:");
            string studentId = Console.ReadLine();
            Student student = students.Find(s => s.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase));

            if (student != null)
            {
                student.DisplayInfo();
                foreach (var score in student.Scores)
                {
                    score.DisplayScore();
                }
            }
            else
            {
                Console.WriteLine("Không tìm thấy học sinh.");
            }
        }

        public static void SummarizeBySubject(string subject)
        {
            Console.WriteLine($"Tổng kết điểm cho môn {subject}:");
            foreach (var student in students)
            {
                Score score = student.Scores.Find(s => s.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));
                if (score != null)
                {
                    Console.WriteLine($"{student.Name}: {score.CalculateFinalScore():F2}");
                }
            }
        }

        public static void SummarizeByClass()
        {
            Console.WriteLine("Danh sách học sinh:");
            foreach (var student in students)
            {
                student.DisplayInfo();
                Console.WriteLine("Điểm:");
                foreach (var score in student.Scores)
                {
                    score.DisplayScore();
                }
                Console.WriteLine();
            }
        }

        public static void UpdateStudentInfo()
        {
            Console.WriteLine("Nhập mã học sinh:");
            string studentId = Console.ReadLine();
            Student student = students.Find(s => s.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase));

            if (student != null)
            {
                Console.WriteLine("Nhập tên mới:");
                student.Name = Console.ReadLine();
                Console.WriteLine("Nhập tuổi mới:");
                student.Age = int.Parse(Console.ReadLine());
                Console.WriteLine("Nhập địa chỉ mới:");
                student.Address = Console.ReadLine();
                Console.WriteLine("Nhập lớp mới:");
                student.Class = Console.ReadLine();

                FileHandler.SaveDataToFile(studentFilePath, students);
                Console.WriteLine("Cập nhật thông tin học sinh thành công.");
            }
            else
            {
                Console.WriteLine("Không tìm thấy học sinh.");
            }
        }
    }
}
