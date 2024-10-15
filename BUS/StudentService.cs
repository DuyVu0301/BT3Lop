using DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public  class StudentService
    {
            public List<Student> GetAll()
        {
            SinhVienDB sinhVienDB = new SinhVienDB();
            return sinhVienDB.Students.ToList();
        }  
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            SinhVienDB context = new SinhVienDB();
            return context.Students.Where(p => p.MajorID == null).ToList(); //&& p.FacultyID == facultyID
        }
        public void InsertUpdate(Student s)
        {
            SinhVienDB context = new SinhVienDB();

            // Kiểm tra xem sinh viên đã tồn tại hay chưa
            var existingStudent = context.Students.FirstOrDefault(x => x.StudentID == s.StudentID);

            if (existingStudent != null)
            {
                // Cập nhật sinh viên đã tồn tại
                context.Entry(existingStudent).CurrentValues.SetValues(s);
            }
            else
            {
                // Thêm sinh viên mới
                context.Students.Add(s);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            context.SaveChanges();
        }
        public void Delete(string studentID)
        {
            SinhVienDB context = new SinhVienDB();

            // Tìm sinh viên trong cơ sở dữ liệu dựa vào MSSV
            var student = context.Students.FirstOrDefault(x => x.StudentID == studentID);

            if (student != null)
            {
                // Nếu sinh viên tồn tại, tiến hành xóa
                context.Students.Remove(student);
                context.SaveChanges();
            }
            else
            {
                // Nếu sinh viên không tồn tại, có thể báo lỗi hoặc thông báo không tìm thấy
                throw new Exception("Sinh viên không tồn tại.");
            }
        }
    }
}
