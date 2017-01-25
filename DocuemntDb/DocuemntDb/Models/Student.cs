using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocuemntDb.Models
{
    public class Student
    {
        public string id { get; set; }
        public Name Studentname { get; set; }
        public Address studentAddress { get; set; }
        public string Institute { get; set; }
        public DateTime BirthDay { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Address
    {
        public string Number { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
    }

    public class Name
    {
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string OtherNames { get; set; }
    }
}