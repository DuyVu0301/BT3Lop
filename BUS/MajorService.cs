using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class MajorService
    {
        public List<Major> GetAllByFaculty(int facultyID)
        {
            SinhVienDB sinhVienDB = new SinhVienDB();
            return sinhVienDB.Majors.Where(p=>p.FacultyID == facultyID).ToList();   
        }
    }
}
