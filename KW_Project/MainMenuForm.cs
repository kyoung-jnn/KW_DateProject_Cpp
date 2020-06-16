﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient; // Mysql 사용
using System.IO;

namespace KW_Project
{
    public partial class MainMenuForm : Form
    {
        private string currentUserId;
        private string currentUserGender;
        private const int CS_DROPSHADOW = 0x00020000;
        public string idealId = string.Empty;
        
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect
                                                    , int nTopRect
                                                    , int nRightRect
                                                    , int nBottomRect
                                                    , int nWidthEllipse
                                                    , int nHeightEllipse);

        public MainMenuForm(string id,string gender)
        {
            currentUserId = id;
            currentUserGender = gender;
            //btnHome.Enabled = false;

            InitializeComponent();
            lblProfile1.Parent = idealPic;
            lblProfile2.Parent = idealPic;

        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {
            //테두리 둥글게
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));

            // 여기에 알고리즘 메소드 추가해야함

            if (currentUserGender == "남자")
                idealId = "201584001";
            else if (currentUserGender == "여자")
                idealId = "201619035";

            //매칭 알고리즘 구현시 31~35줄 수정해서 ideal_id를 매칭된 사람으로 해주면 댈듯?? -->6/16일 추가 : 전역변수로 idealId만들었으니 여기 할당해주면 댐
            //취소버튼 누르면 다시 다른사람 매칭해서 밑에 함수 두개 실행시켜주는식으로
            LoadIdealPhoto();
            LoadIdealProfile();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        private void MainMenuForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            ProfileEditForm profileeditform = new ProfileEditForm(currentUserId, currentUserGender);
            profileeditform.ShowDialog();
        }

        

        // mysql에서 프로필 사진 불러오기
        private void LoadIdealPhoto()
        {
            string insertQuery = "";
            byte[] Image = null;

            MySqlConnection connection = new MySqlConnection("Server=localhost;Database=project_data;Uid=root;Pwd=1234");
            MySqlCommand command = new MySqlCommand();

            if (currentUserGender == "남자")
                insertQuery = "SELECT file from profile_photo_data_f WHERE id=@id";
            else if (currentUserGender == "여자")
                insertQuery = "SELECT file from profile_photo_data_m WHERE id=@id";
            try
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = insertQuery;
                command.Parameters.AddWithValue("@id", idealId);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Image = (byte[])reader[0];
                    idealPic.Image = new Bitmap(new MemoryStream(Image));
                }

                reader.Close();
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }
        private void LoadIdealProfile()
        {
            string insertQuery = "";

            MySqlConnection connection = new MySqlConnection("Server=localhost;Database=project_data;Uid=root;Pwd=1234");
            MySqlCommand command = new MySqlCommand();

            if (currentUserGender == "남자")
                insertQuery = "SELECT * from user_data_f WHERE id=@id";
            else if (currentUserGender == "여자")
                insertQuery = "SELECT * from user_data_m WHERE id=@id";
            try
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = insertQuery;
                command.Parameters.AddWithValue("@id", idealId);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    lblProfile1.BackColor = System.Drawing.Color.Transparent;
                    lblProfile1.Text = reader["department"].ToString();
                    lblProfile1.Text += " ";
                    lblProfile1.Text += reader["age"].ToString();

                    lblProfile2.BackColor = System.Drawing.Color.Transparent;
                    lblProfile2.Text = reader["name"].ToString();
                }

                reader.Close();
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }


        private void btnBoard_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            BoardForm boardForm = new BoardForm(currentUserId,currentUserGender);
            DialogResult result = boardForm.ShowDialog();

            if(result == DialogResult.Cancel)
            {
                this.Visible = true;
            }
        }


        private void MatchingAlgorithm()
        {

        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            /*
            //this.Visible = false;
            ChatClientForm clientForm = new ChatClientForm();
            DialogResult result = clientForm.ShowDialog();

            if (result == DialogResult.Cancel)
            {
                this.Visible = true;
            }
            */
            IdealList idealList = new IdealList(currentUserId, currentUserGender, idealId);
            idealList.ShowDialog();

        }
    }

}

