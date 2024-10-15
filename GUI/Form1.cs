using BUS;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class Form1 : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();
        public Form1()
        {
            InitializeComponent();
        }
        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            //listFacultys.Insert(0, new Faculty());
            this.comboBox1.DataSource = listFacultys;
            this.comboBox1.DisplayMember = "FacultyName";
            this.comboBox1.ValueMember = "FacultyID";
        }
        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dgvStudent.Rows[index].Cells[4].Value = item.Major.Name;
                dgvStudent.Rows[index].Cells[5].Value = !string.IsNullOrEmpty(item.Avatar) ? item.Avatar : "Đã có ảnh";
                ShowAvatar(item.Avatar);
            }
        }

        private void Form1_Load(object sender, EventArgs e)     
        {
            try
            {

                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            SaveStudent();
        }
        
        private void SaveStudent()
        {
            try
            {
                // Tạo đối tượng SinhVien từ dữ liệu người dùng nhập
                Student student = new Student()
                {
                    StudentID = txtMSSV.Text,
                    FullName = txtHoten.Text,
                    FacultyID = (int)comboBox1.SelectedValue,
                    AverageScore = GetAverageScore(txtDTB.Text),
                    Avatar = pictureBox1.Image != null ? Path.GetFileName(pictureBox1.ImageLocation) : null
                };

                // Gọi phương thức InsertUpdate để thêm hoặc cập nhật sinh viên
                studentService.InsertUpdate(student);

                // Thông báo thành công
                MessageBox.Show("Lưu thành công!");

                // Làm mới danh sách sinh viên sau khi thêm mới
                RefreshStudentList();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu có
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}");
            }
        }
        private double GetAverageScore(string input)
        {
            double averageScore;

            // Kiểm tra giá trị nhập vào và gán giá trị mặc định nếu không hợp lệ
            if (!double.TryParse(input, out averageScore))
            {
                averageScore = 0.0; // Giá trị mặc định mà bạn muốn
            }

            return averageScore;
        }
        private void RefreshStudentList()
        {
            try
            {
                // Lấy danh sách sinh viên từ database
                var listStudents = studentService.GetAll();

                // Gọi hàm BindGrid để cập nhật DataGridView
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi tải danh sách sinh viên: {ex.Message}");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy MSSV của sinh viên cần xóa từ TextBox
                string mssv = txtMSSV.Text;

                // Gọi hàm Delete để xóa sinh viên
                studentService.Delete(mssv);

                // Thông báo thành công
                MessageBox.Show("Xóa thành công!");

                // Cập nhật lại danh sách sinh viên trên DataGridView
                RefreshStudentList();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu có
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvStudent.Rows[e.RowIndex].Cells[0].Value != null)
                {
                    txtMSSV.Text = dgvStudent.Rows[e.RowIndex].Cells[0].Value.ToString();
                    txtHoten.Text = dgvStudent.Rows[e.RowIndex].Cells[1].Value.ToString();
                    comboBox1.Text=dgvStudent.Rows[e.RowIndex].Cells[2].Value.ToString();
                    txtDTB.Text = dgvStudent.Rows[e.RowIndex].Cells[3].Value.ToString();

                }

            }
        }

        private void ShowAvatar(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                pictureBox1.Image = null; // Không có ảnh để hiển thị
            }
            else
            {
                // Xây dựng đường dẫn đầy đủ đến hình ảnh
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images", imageName);

                if (File.Exists(imagePath)) // Kiểm tra xem file có tồn tại không
                {
                    pictureBox1.Image = System.Drawing.Image.FromFile(imagePath); // Tải và hiển thị ảnh
                }
                else
                {
                    pictureBox1.Image = null; // Không tìm thấy file, xóa PictureBox
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.jfif";
            openFileDialog.Title = "Chọn một ảnh";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Lấy phần mở rộng file (vd: .jpg, .png)
                string fileExtension = Path.GetExtension(openFileDialog.FileName);

                // Lấy mã sinh viên từ TextBox
                string studentID = txtMSSV.Text;

                // Đặt tên file theo định dạng {studentID}.{fileExtension}
                string fileName = $"{studentID}{fileExtension}";

                // Đường dẫn tới thư mục Images
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagesFolderPath = Path.Combine(parentDirectory, "Images");
                string imagePath = Path.Combine(imagesFolderPath, fileName);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                // Sao chép ảnh vào thư mục với tên mới
                File.Copy(openFileDialog.FileName, imagePath, true);

                // Hiển thị ảnh trong PictureBox
                pictureBox1.Image = System.Drawing.Image.FromFile(imagePath);

                // Cập nhật tên file vào cơ sở dữ liệu
                SaveAvatarToDatabase(studentID, fileName);
            }
        }
        private void SaveAvatarToDatabase(string studentID, string avatarFileName)
        {
            // Code sử dụng Entity Framework để cập nhật thông tin vào database
            using (var db = new SinhVienDB())
            {
                var student = db.Students.Find(studentID);
                if (student != null)
                {
                    student.Avatar = avatarFileName;  // Cập nhật tên file avatar
                    db.SaveChanges();  // Lưu thay đổi vào CSDL
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.checkBox1.Checked)
                listStudents = studentService.GetAllHasNoMajor(1);
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }
    }
}

