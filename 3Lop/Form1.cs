using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using BUS;
namespace _3Lop
{
    public partial class Form1 : Form
    {
        private readonly StudentService studentService = new StudentService();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadFaculties(); // Load danh sách khoa vào ComboBox
            
            LoadData();
        }
        private void LoadData()
        {
            List<StudentDTO> students;

            if (cbKhongCHuyenNganh.Checked)  // Kiểm tra xem CheckBox có được chọn không
            {
                students = studentService.GetStudentsWithoutMajor();
            }
            else
            {
                students = studentService.GetAll();
            }

            dataGridView1.DataSource = students;

            dataGridView1.Columns[0].HeaderText = "MSSV";
            dataGridView1.Columns[1].HeaderText = "Họ Tên";
            dataGridView1.Columns[2].HeaderText = "Khoa";
            dataGridView1.Columns[3].HeaderText = "DTB";
            dataGridView1.Columns[4].HeaderText = "Chuyên ngành";
        }
        // Đổ dữ liệu vào ComboBox Khoa từ bảng Faculty
        private void LoadFaculties()
        {
            using (Model1 context = new Model1())
            {
                var faculties = context.Faculties.Select(f => new
                {
                    f.FacultyID,
                    f.FacultyName
                }).ToList();

                cmbKhoa.DataSource = faculties;
                cmbKhoa.DisplayMember = "FacultyName";
                cmbKhoa.ValueMember = "FacultyID";
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                txtMSSV.Text = selectedRow.Cells["StudentID"].Value.ToString();
                txtHoTen.Text = selectedRow.Cells["FullName"].Value.ToString();
                txtDiemTB.Text = selectedRow.Cells["AverageScore"].Value.ToString();
                cmbKhoa.SelectedValue = selectedRow.Cells["FacultyID"].Value; // Sửa đổi để lấy FacultyID
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                int facultyID = (int)cmbKhoa.SelectedValue; // Lấy FacultyID từ ComboBox

                Student newStudent = new Student
                {
                    StudentID = int.Parse(txtMSSV.Text),
                    FullName = txtHoTen.Text,
                    FacultyID = facultyID,
                    AverageScore = decimal.Parse(txtDiemTB.Text),
                    MajorID = null // Sinh viên không cần chọn chuyên ngành, MajorID để null
                };

                studentService.AddStudent(newStudent);
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                int facultyID = (int)cmbKhoa.SelectedValue;

                Student updatedStudent = new Student
                {
                    StudentID = int.Parse(txtMSSV.Text),
                    FullName = txtHoTen.Text,
                    FacultyID = facultyID,
                    AverageScore = decimal.Parse(txtDiemTB.Text),
                    MajorID = null // Sinh viên không có chuyên ngành
                };

                studentService.UpdateStudent(updatedStudent);
                MessageBox.Show("Sửa thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                int studentID = int.Parse(txtMSSV.Text);

                studentService.DeleteStudent(studentID);
                MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFields()
        {
            txtMSSV.Clear();
            txtHoTen.Clear();
            txtDiemTB.Clear();
            cmbKhoa.SelectedIndex = -1;  // Đặt ComboBox về trạng thái mặc định (không chọn)
        }

        private void cbKhongCHuyenNganh_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        // Lấy FacultyID dựa trên tên khoa từ ComboBox
        private int GetSelectedFacultyID()
        {
            // Lấy FacultyID dựa vào FacultyName trong ComboBox
            // Có thể thực hiện truy vấn vào cơ sở dữ liệu để lấy FacultyID từ tên khoa
            using (Model1 context = new Model1())
            {
                var faculty = context.Faculties.FirstOrDefault(f => f.FacultyName == cmbKhoa.SelectedItem.ToString());
                return faculty != null ? faculty.FacultyID : 0;
            }
        }       
    }
}
