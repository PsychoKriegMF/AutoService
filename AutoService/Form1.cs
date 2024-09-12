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

namespace AutoService
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            if (!FSWork.IsFileExist("Автосервис.db")) MakeStore();            
            FillMechanicNames();
        }
        private void MakeStore()
        {
            if(DBWork.MakeDB())
            {
                MessageBox.Show($"База данных существует.");
            }
        }
        private void FillMechanicNames()
        {
            foreach (var name in DBWork.GetMechanics())
            {
                cmbMechanics.Items.Add(name);
            }                    
        }

        private void picBoxAvatar_Click(object sender, EventArgs e)
        {
            if(cmbMechanics.SelectedItem != null) 
            {
                byte[] _image = FSWork.GetImage();
                string _name = cmbMechanics.SelectedItem.ToString();
                DBWork.AddAvatar(_name,_image);
            }           
        }

        private void SetImage2PictureBox()
        {
            string _name = cmbMechanics.SelectedItem.ToString();
            MemoryStream ms = DBWork.GetAvatar(_name);
            if(ms != null)
            {
                picBoxAvatar.Image = Image.FromStream(DBWork.GetAvatar(_name));
            }
            else
            {
                picBoxAvatar.BackColor = Color.Black;
            }
        }

        private void cmbMechanics_SelectedValueChanged(object sender, EventArgs e)
        {
            SetImage2PictureBox();
        }
    }
}
