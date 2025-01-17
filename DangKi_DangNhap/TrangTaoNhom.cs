﻿using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;

namespace DangKi_DangNhap
{
    public partial class TrangTaoNhom : Form
    {
        public event EventHandler<string> TenNhomCreated;
        private string userName;
        private IFirebaseClient firebaseClient; // Thêm biến firebaseClient

        public TrangTaoNhom(string userName, IFirebaseClient firebaseClient) // Thêm tham số firebaseClient vào constructor
        {
            this.userName = userName;
            this.firebaseClient = firebaseClient; // Gán giá trị cho biến firebaseClient
            InitializeComponent();
            errorLabel.Text = "";
        }

        private async void bunifuButton21_Click(object sender, EventArgs e)
        {
            try
            {
                // Khi Button ô nhóm được nhấp, thêm nhóm mới vào Firebase
                string tenNhom = textBox1.Text;
                string nhomID = textBox2.Text;

                // Kiểm tra xem người dùng đã nhập đủ thông tin chưa
                if (string.IsNullOrEmpty(tenNhom) || string.IsNullOrEmpty(nhomID))
                {
                    //MessageBox.Show("Vui lòng nhập tên nhóm và ID nhóm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    errorLabel.Text = "Vui long nhập đầy đủ thông tin !";
                    return;
                }

                bool userExists = await CheckGroupNameExists(tenNhom);
                if (userExists)
                {
                    //MessageBox.Show("Tên nhóm đã tồn tại, vui lòng chọn tên khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    errorLabel.Text = "Tên nhóm đã tồn tại !";
                    return;
                }

                FirebaseResponse groupResponse = await firebaseClient.GetAsync($"group/{tenNhom}");
                var existingGroup = groupResponse.ResultAs<Dictionary<string, object>>();

                if (existingGroup != null)
                {
                    //MessageBox.Show("nhóm đã tồn tại Vui lòng đặt tên nhóm khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    errorLabel.Text = "Nhóm đã tồn tại vui lòng đặt tên khác !";
                    return;
                }

                // Tạo dữ liệu để thêm vào Firebase
                var data1 = new Dictionary<string, object>
                {
                    { tenNhom, nhomID }
                };

                // Thực hiện thêm dữ liệu vào Firebase
                FirebaseResponse response1 = await firebaseClient.UpdateAsync($"nhoms/{userName}", data1);

                // Hiển thị thông báo tạo nhóm thành công
                MessageBox.Show("Tạo nhóm mới thành công ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                errorLabel.Text = "";

                // Thêm button nhóm mới vào form
              
                var data = new Dictionary<string, object>
                {
                    { userName, true }
                };

                // Thực hiện thêm dữ liệu vào Firebase
                FirebaseResponse response = await firebaseClient.UpdateAsync($"group /{tenNhom}/", data);
                TenNhomCreated?.Invoke(this, tenNhom);
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tạo nhóm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
     
        private void TrangTaoNhom_Load(object sender, EventArgs e)
        {

        }

        private async Task<bool> CheckGroupNameExists(string name)
        {
            FirebaseResponse response = await firebaseClient.GetAsync($"group /{name}");
            return response.Body != "null";
        }

    }
}
