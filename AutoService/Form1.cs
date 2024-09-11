using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }
}
