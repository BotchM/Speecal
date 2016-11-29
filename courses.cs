using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_and_write
{
    class Courses
    {
        String preReq;
        String description;
        int courseNum;

        public Courses(int courseNum , String description, String preReq){
            this.description = description;
            this.preReq = preReq;
            this.courseNum = courseNum;
        }

        public String toString()
        {
            return "Description of course is: " + description + " and " + preReq;
        }
    }
}
