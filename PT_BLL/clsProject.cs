using PT_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_BLL
{
    public class clsProject
    {
        enum enMode { AddNew, Update }
        enMode _Mode;
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public string Goal { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public clsProject()
        {

            ProjectID = -1;
            Name = string.Empty;
            Goal = string.Empty;
            Description = string.Empty;
            StartDate = DateTime.MinValue;
            EndDate = null;

            _Mode = enMode.AddNew;
        }

        clsProject(int projectID, string name, string goal, string description, DateTime startDate, DateTime? endDate)
        {
            ProjectID = projectID;
            Name = name;
            Goal = goal;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;

            _Mode = enMode.Update;

        }

        static public clsProject FindByID(int projectID)
        {
            string name = string.Empty;
            string goal = string.Empty;
            string description = string.Empty;
            DateTime startDate = DateTime.MinValue;
            DateTime? endDate = null;

            bool isFound = clsProjectData.GetProjectByID(projectID ,ref name ,ref goal ,ref description ,ref startDate ,ref endDate);

            if (isFound)
                return new clsProject(projectID, name, goal, description, startDate, endDate);


            return null;
        }

        bool _AddNewProject()
        {
            this.ProjectID = clsProjectData.AddNewProject(this.Name, this.Goal, this.Description, this.StartDate, this.EndDate);

            return this.ProjectID != -1;
        }

        bool _UpdateProject()
        {
            return clsProjectData.UpdateProjectByID(this.ProjectID, this.Name, this.Goal, this.Description, this.StartDate, this.EndDate);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewProject())
                        {
                            _Mode = enMode.Update;
                            return true;
                        }
                        else
                            return false;
                    }

                case enMode.Update:
                    {
                        return _UpdateProject();
                    }
            }

            return false;
        }

        public static bool DeleteByID(int projectID)
        {
            return clsProjectData.DeleteProject(projectID);
        }

        public static DataTable GetAllProjects()
        {
            return clsProjectData.GetAllProjects();
        }

        public static bool IsExistByName(string name)
        {
            return clsProjectData.IsProjectExistByName(name);
        }

        public static bool IsExistByNameAndID(string name, int excludeProjectID)
        {
            return clsProjectData.IsProjectExistByNameAndID(name, excludeProjectID);
        }

    }
}
