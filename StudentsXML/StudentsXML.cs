using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudentsXML
{
    class StudentsXML
    {
        static void Main(string[] args)
        {
            XDocument xmlStudent = new XDocument();

            xmlStudent.Add(new XElement("students",
                new XElement("student",
                new XElement("name", "Ivan Ivanov"),
                new XElement("gender", "Male"),
                new XElement("birthDate", "1999/12/23"),
                new XElement("phoneNumber", "0000000000"),
                new XElement("email", "ivan@abv.bg"),
                new XElement("university", "Software University"),
                new XElement("specialty", "C# Web Developer"),
                new XElement("facultyNumber", "0123456789"),
                new XElement("exams",
                new XElement("exam",
                new XElement("name", "Programming Basics"),
                new XElement("dateTaken", "2017/01/01"),
                new XElement("grade", "5.0")))
                )));

            xmlStudent.Save("../../myStudents.xml");
        }
    }
}
