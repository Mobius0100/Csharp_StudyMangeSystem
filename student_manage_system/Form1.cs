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

namespace student_manage_system
{
    public partial class Form1 : Form
    {
        private string photoName = string.Empty;
        private string fileName = string.Empty; //= ""
        private List<string> objListStudent = new List<string>();
        private List<string> objListQuery = new List<string>();
        private int activeflag = 1;
        public Form1()
        {
            InitializeComponent();
            gbStudentDetail.Enabled = false;
        }
        //控件事件
        private void button1_Click(object sender, EventArgs e)  //导入文件展示到dataGridviews中
        {
            //1.选择文件
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "TXT文件哟(*.txt)|*.txt|CSV文件(*.csv)|*.csv|所有文件(*.*)|*.*";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                fileName = openfile.FileName; //把选择的文件路径赋值给全局变量 fileName
            }
            //2.文件导入到list中
            try
            {
                //清空表格
                dataGridView1.Rows.Clear();
                //读取文件
                objListStudent = ReadFileTolist(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取文件错误,具体如下:" + ex.Message, "系统消息", MessageBoxButtons.OK);
                return;
            }
            //3.把list展示在DateGridView中
            LoadDataToDateGridView(objListStudent);
            //4.将第一行数据明细展示在groupbox中
            string currentSNo = dataGridView1.Rows[0].Cells[0].Value.ToString();
            string[] currentDetail = GetStudentBySNo(currentSNo).Split(',');
            LoadDataToDetail(currentDetail[0], currentDetail[1], currentDetail[2], currentDetail[3], currentDetail[4],
                currentDetail[5], currentDetail[6], currentDetail[7]);

        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e) //根据选择行数改变学生明细信息
        {
            if (dataGridView1.CurrentRow.Selected == false) return;
            else
            {
                string currentSNo = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                string[] currentDetail = GetStudentBySNo(currentSNo).Split(',');
                LoadDataToDetail(currentDetail[0], currentDetail[1], currentDetail[2], currentDetail[3], currentDetail[4],
                currentDetail[5], currentDetail[6], currentDetail[7]);
            }
        }
        private void txtbSno_TextChanged(object sender, EventArgs e)  // 根据所输入学号查询信息
        {
            objListQuery.Clear(); //清空查询结果的list

            // 查看哪些满足条件并添加到querylist中
            foreach (string item in objListStudent)
            {
                if (item.StartsWith(txtbSno.Text))
                    objListQuery.Add(item);
            }
            //清空表格
            dataGridView1.Rows.Clear();
            //展示在DataGridView中
            LoadDataToDateGridView(objListQuery);
        }
        private void txtbName_TextChanged(object sender, EventArgs e)  //根据输入姓名查找信息
        {
            objListQuery.Clear();
            foreach(string item in objListStudent)
            {
                if (item.Contains(txtbName.Text))
                    objListQuery.Add(item);
            }
            dataGridView1.Rows.Clear();
            LoadDataToDateGridView(objListQuery);

        }
        private void txtbPhone_TextChanged(object sender, EventArgs e)  //根据输入手机号查找信息
        {
            objListQuery.Clear();
            foreach (string item in objListStudent)
            {
                if (item.Contains(txtbPhone.Text))
                    objListQuery.Add(item);
            }
            dataGridView1.Rows.Clear();
            LoadDataToDateGridView(objListQuery);
        }
        private void btnAdd_Click(object sender, EventArgs e)   //添加学生信息
        {
            DisableButton();

            txtSNO.Text = string.Empty;
            txtName.Text = string.Empty;
            rbMale.Checked = true;
            dtpBirthday.Text = DateTime.Now.ToString();
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtAddress.Text = string.Empty ;
            Photo.BackgroundImage = null;

            txtSNO.Focus();

            activeflag = 1;
        }

        private void btnChange_Click(object sender, EventArgs e)  //修改学生信息
        {
            DisableButton();
            txtSNO.Enabled = false;
            txtName.Focus();
            activeflag = 2;
        }

        private void btnDelete_Click(object sender, EventArgs e)  //删除学生信息
        {
            if(dataGridView1.CurrentRow.Selected == false)
            {
                MessageBox.Show("删除数据必须选中一行", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                string info = "您确定要删除[学号:" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + " 姓名:" +
                    dataGridView1.CurrentRow.Cells[1].Value.ToString() + "]的信息吗?";
                DialogResult result = MessageBox.Show(info, "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    string currentStudent = GetStudentBySNo(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                    foreach (string item in objListStudent)
                    {
                        if (item.Equals(currentStudent))
                        {
                            objListStudent.Remove(item);
                            break;
                        }
                    }
                    dataGridView1.Rows.Clear();
                    LoadDataToDateGridView(objListStudent);
                    MessageBox.Show("学生信息删除成功", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void btnSubmit_Click(object sender, EventArgs e)  //提交操作
        {
            if (!VerifyData()) return;
            else
            {
                string photoPath;
                if(Photo.BackgroundImage != null)
                {
                    //另存为 保存图片名称为日期+时间+随机码
                    photoPath = DateTime.Now.ToString("yyyyMMddHHmmss");
                    Random objRandom = new Random();
                    photoPath += objRandom.Next(0, 100).ToString("00");
                    photoPath = ".\\image\\" + photoPath + photoName.Substring(photoName.Length - 4);
                    //存储
                    Bitmap objBitmap = new Bitmap(Photo.BackgroundImage);
                    objBitmap.Save(photoPath, Photo.BackgroundImage.RawFormat);
                    objBitmap.Dispose();
                }
                else
                {
                    photoPath = string.Empty;
                }

                //组合数据存入list
                string sno = txtSNO.Text.Trim();
                string sname = txtName.Text.Trim();
                string sex = rbMale.Checked == true ? "男" : "女";
                string birthday = dtpBirthday.Text;
                string mobile = txtPhone.Text;
                string email = txtEmail.Text;
                string homeAddress = txtAddress.Text;
                string photo = photoPath;

                string currentStudent = sno + "," + sname + "," + sex + "," + birthday + "," + mobile + "," + email + ","
                    + homeAddress + "," + photo;

                switch (activeflag)
                {
                    case 1:
                        objListStudent.Add(currentStudent);
                        dataGridView1.Rows.Clear();
                        LoadDataToDateGridView(objListStudent);
                        MessageBox.Show("学生信息添加成功", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        EnableButton();
                        break;
                    case 2:
                        for (int i = 0; i < objListStudent.Count; i++)   //for循环能记录循环的位置(i),foreach不能.
                        {
                            if(objListStudent[i].StartsWith(sno))
                            {
                                objListStudent.RemoveAt(i);   //删除原有
                                objListStudent.Insert(i, currentStudent);  //添加到list
                            }
                        }
                        dataGridView1.Rows.Clear();
                        LoadDataToDateGridView(objListStudent);
                        MessageBox.Show("学生信息修改成功", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtSNO.Enabled = true;
                        EnableButton();

                        break;
                    default:
                        break;
                }
             }


        }

        private void btnCancel_Click(object sender, EventArgs e)  //取消操作
        {
            EnableButton();
        }

        private void btsClose_Click(object sender, EventArgs e)  //退出程序
        {
            DialogResult result = MessageBox.Show("是否要保存文件?", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                File.WriteAllText(fileName, string.Empty); //清空原文件

                StreamWriter sw = new StreamWriter(fileName,true,Encoding.Default);
                foreach (string item in objListStudent)
                {
                    sw.WriteLine(item);
                }
                sw.Close();
                MessageBox.Show("保存成功!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Close();
        }

        private void pbSelect_Click(object sender, EventArgs e)  //选择照片展示
        {
            OpenFileDialog openImg = new OpenFileDialog();
            openImg.Filter = "图片格式(*.jpg;*.bmp;*.jpeg;*.png)|*.jpg;*.bmp;*.jpeg;*.png|所有文件(*.*)|*.*";
            if(openImg.ShowDialog() ==  DialogResult.OK)
            {
                try
                {
                    photoName = openImg.FileName;
                    Photo.BackgroundImage = Image.FromFile(openImg.FileName);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        //用户自定义方法
        private List<string> ReadFileTolist(string filePath) //读取文件,以list返回调用者
        {
            List<string> objList = new List<string>();
            string line = string.Empty;

            try
            {
                StreamReader file = new StreamReader(filePath,Encoding.Default);
                line = file.ReadLine();
                while (line != null)        //while((line=file.ReadLine()) != null)
                {
                    objList.Add(line);             
                    line = file.ReadLine();       //  null
                }
                file.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objList;

        }
        private void LoadDataToDateGridView(List<string> objList) //将list中信息展示到datagridview中
        {
            foreach (string item in objList)
            {
                //依次读取list数据
                string[] studentArray = item.Split(',');
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = studentArray[0];
                row.Cells[1].Value = studentArray[1];
                row.Cells[2].Value = studentArray[2];
                row.Cells[3].Value = studentArray[3];
                row.Cells[4].Value = studentArray[4];
                dataGridView1.Rows.Add(row);
            }

        }
        private void LoadDataToDetail(string sno,string sname,string sex,string birthday,string mobile,string email,
                                        string homeAddress,string photo) //将信息展示在groupbox 中
        {
            txtSNO.Text = sno;
            txtName.Text = sname;
            if (sex == "男") rbMale.Checked = true;
            else rbFemale.Checked = true;
            dtpBirthday.Text = birthday;
            txtPhone.Text = mobile;
            txtEmail.Text = email;
            txtAddress.Text = homeAddress;
            if (photo == string.Empty) Photo.BackgroundImage = null;
            else Photo.BackgroundImage = Image.FromFile(photo);
        }
        private string GetStudentBySNo(string sno) //根据学号查询学生具体信息
        {
            string currentStudent = string.Empty;
            foreach (string item in objListStudent)
            {
                if (item.StartsWith(sno))
                {
                    currentStudent = item;
                    break;
                }
            }
            return currentStudent;
        }
        private void DisableButton()  //禁用按钮
        {
            //禁用按钮
            btnAdd.Enabled = false;
            btnImport.Enabled = false;
            btnChange.Enabled = false;
            btnDelete.Enabled = false;

            //启用明细区
            gbStudentDetail.Enabled = true;
        }
        private void EnableButton()  //启用按钮
        {
            //启用按钮
            btnAdd.Enabled = true;
            btnImport.Enabled = true;
            btnChange.Enabled = true;
            btnDelete.Enabled = true;

            //关闭明细区
            gbStudentDetail.Enabled = false;
        }
        private bool VerifyData()  //验证数据是否正确
        {
            bool b = true;
            if (txtSNO.Text == "")
            {
                MessageBox.Show("学号不能为空!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSNO.Focus();
                b = false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("姓名不能为空!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtName.Focus();
                b = false;
            }

            if (activeflag == 1)
            {
                if (GetStudentBySNo(txtSNO.Text.Trim()) != string.Empty)
                {
                    MessageBox.Show("该学号已存在,请重新输入!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSNO.Focus();
                    b = false;
                }
            }
            return b;
        }


    }
}
