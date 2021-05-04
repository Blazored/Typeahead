using System;

namespace Sample.Shared
{
    public class Person
    {
        public Person() { }
        public Person(int id, string firstname, string lastname, int age, string location)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Location = location;
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string FullName { get => Firstname + " " + Lastname; }
        public int Age { get; set; }
        public string Location { get; set; }

        // Override the equals as the reference to the person object is different when deserializing the BlazoredTypeaheadConfigModel
        public override bool Equals(object obj)
        {
            if (obj is Person person)
            {
                return person.Id == Id;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
