using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PT_BLL;

namespace PT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //hello
        private void Form1_Load(object sender, EventArgs e)
        {
            gunaDataGridView1.DataSource =clsProject.GetAllProjects();
        }

        private void gunaButton1_Click(object sender, EventArgs e)
        {
            clsProject project = new clsProject();

            project.Name = "Project 1";
            project.Goal = "Goal 1";
            project.Description = "Description 1";
            project.StartDate = DateTime.Now;
            project.EndDate = DateTime.Now;

            if (project.Save())
            {
                MessageBox.Show("Project saved successfully.");
                gunaDataGridView1.DataSource = clsProject.GetAllProjects();
            }
            else
            {
                MessageBox.Show("Failed to save project.");

            }
        }

        private void gunaButton2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
