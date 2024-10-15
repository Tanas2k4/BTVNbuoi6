using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace BUS
{
    public class StudentService
    {
        // Lấy tất cả sinh viên
        public List<StudentDTO> GetAll()
        {
            using (Model1 context = new Model1())
            {
                var query = from student in context.Students
                            join faculty in context.Faculties on student.FacultyID equals faculty.FacultyID
                            join major in context.Majors on student.MajorID equals major.MajorID into majors
                            from major in majors.DefaultIfEmpty()
                            select new StudentDTO
                            {
                                StudentID = student.StudentID,
                                FullName = student.FullName,
                                FacultyName = faculty.FacultyName,
                                AverageScore = (decimal)student.AverageScore,
                                MajorName = major != null ? major.MajorName : "Không có chuyên ngành"
                            };

                return query.ToList();
            }
        }

        // Lấy sinh viên không có chuyên ngành (MajorID = null)
        public List<StudentDTO> GetStudentsWithoutMajor()
        {
            using (Model1 context = new Model1())
            {
                var query = from student in context.Students
                            join faculty in context.Faculties on student.FacultyID equals faculty.FacultyID
                            where student.MajorID == null
                            select new StudentDTO
                            {
                                StudentID = student.StudentID,
                                FullName = student.FullName,
                                FacultyName = faculty.FacultyName,
                                AverageScore = (decimal)student.AverageScore,
                                MajorName = "Không có chuyên ngành"
                            };

                return query.ToList();
            }
        }
        public void AddStudent(Student student)
        {
            using (Model1 context = new Model1())
            {
                context.Students.Add(student);
                context.SaveChanges(); // Lưu thay đổi vào database
            }
        }

        // Sửa sinh viên
        public void UpdateStudent(Student student)
        {
            using (Model1 context = new Model1())
            {
                var existingStudent = context.Students.Find(student.StudentID);
                if (existingStudent != null)
                {
                    existingStudent.FullName = student.FullName;
                    existingStudent.FacultyID = student.FacultyID;
                    existingStudent.AverageScore = student.AverageScore;
                    existingStudent.MajorID = student.MajorID;
                    context.SaveChanges(); // Lưu thay đổi vào database
                }
            }
        }

        // Xóa sinh viên
        public void DeleteStudent(int studentID)
        {
            using (Model1 context = new Model1())
            {
                var student = context.Students.Find(studentID);
                if (student != null)
                {
                    context.Students.Remove(student);
                    context.SaveChanges(); // Lưu thay đổi vào database
                }
            }
        }
    }
    public class StudentDTO
    {
        public int StudentID { get; set; }
        public string FullName { get; set; }
        public string FacultyName { get; set; }
        public decimal AverageScore { get; set; }
        public string MajorName { get; set; }
    }
}
