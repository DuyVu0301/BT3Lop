﻿using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class FacultyService
    {
        public List<Faculty> GetAll()
        {
            SinhVienDB sinhVienDB = new SinhVienDB();
            return sinhVienDB.Faculties.ToList();
        }
    }
}