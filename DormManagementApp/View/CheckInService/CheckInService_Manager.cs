﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;

namespace DormManagementApp
{
    public partial class CheckInService_Manager : Form
    {
        public CheckInService_Manager()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            //int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度
            //int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度
            //this.Location = new Point((xWidth - Width) / 2, (yHeight - Height) / 2);
            RefreshData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            string no = this.textBox1.Text;
            string domitoryNo = this.textBox2.Text;

            var result = APIHelper.Get<ApiResult<List<Student>>>(WebSetting.GetUrl("Student/List"),
                new Dictionary<string, object> {
                    { "Id",no},
                    { "DomitoryId",domitoryNo}
                });

            List<Student> list = new List<Student>();
            if (result.Code == 0)
            {
                list = result.Data;
            }
            else
                MessageBox.Show(result.Msg);

            this.dataGridView1.DataSource = list;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddStudent addStudentForm = new AddStudent();
            addStudentForm.StartPosition = FormStartPosition.CenterScreen;
            if (addStudentForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("添加学生成功");
                RefreshData();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (dataGridView != null && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                var selectedRow = dataGridView.Rows[e.RowIndex];
                var selectedColumn = dataGridView.Columns[e.ColumnIndex];
                if (selectedRow != null && selectedColumn != null)
                {
                    string no = selectedRow.Cells["No"].Value.ToString();
                    if (!string.IsNullOrEmpty(no))
                    {
                        Student student = null;
                        var result = APIHelper.Get<ApiResult<Student>>(WebSetting.GetUrl("Student/Single"), new Dictionary<string, object> { { "Id", no } });
                        if (result.Code == 0)
                            student = result.Data;
                        else
                            MessageBox.Show(result.Msg);

                        if (student == null)
                        {
                            RefreshData();
                        }
                        else
                        {
                            switch (selectedColumn.Name)
                            {
                                case "CheckOut":

                                    if (student.RoomNumber < 1)
                                    {
                                        MessageBox.Show("此学生未入住宿舍");
                                        return;
                                    }

                                    bool success = false;
                                    var checkOutResult = APIHelper.Post<ApiResult>(WebSetting.GetUrl("Student/CheckOut"), new Dictionary<string, object> { { "Id", no } });
                                    if (checkOutResult.Code == 0)
                                        success = true;

                                    if (success)
                                    {
                                        MessageBox.Show("退宿成功");
                                        RefreshData();
                                    }
                                    else
                                    {
                                        MessageBox.Show("退宿失败");
                                        return;
                                    }
                                    break;
                                case "CheckIn":
                                    CheckIn checkIn = new CheckIn(student);
                                    if (checkIn.ShowDialog() == DialogResult.OK)
                                    {
                                        MessageBox.Show("分配宿舍成功");
                                        RefreshData();
                                    }
                                    else
                                    {
                                        MessageBox.Show("分配宿舍失败");
                                        return;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DomitoryMain domitory = new DomitoryMain();
            domitory.StartPosition = FormStartPosition.CenterScreen;
            domitory.Show();
        }
    }
}