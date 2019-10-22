using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sample.Shared;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private List<Person> People = new List<Person>();

        public PersonsController()
        {
            CreatePeople();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Person>> GetPeopleWith(string name)
        {
            return People.Where(x => x.FullName.ToLower().Contains(name.ToLower())).ToList();
        }

        private void CreatePeople()
        {
            var janeSmith = new Person(1, "Jane", "Smith", 32, "London");
            People.Add(janeSmith);

            var harroldJones = new Person(2, "Harrold", "Jones", 21, "Newcastle");
            People.Add(harroldJones);

            var jennyGarrod = new Person(3, "Jenny", "Garrod", 46, "Bristol");
            People.Add(jennyGarrod);

            var jamesSmith = new Person(4, "James", "Smith", 29, "Newquey");
            People.Add(jamesSmith);

            var janeAustin = new Person(5, "Jane", "Austin", 63, "Plymouth");
            People.Add(janeAustin);

            var steveLongman = new Person(6, "Steve", "Longman", 32, "Oxford");
            People.Add(steveLongman);

            var jeniJones = new Person(7, "Jeni", "Jones", 27, "London");
            People.Add(jeniJones);

            var harroldGains = new Person(8, "Harrold", "Gains", 44, "London");
            People.Add(harroldGains);

            var tomAdams = new Person(9, "Tom", "Adams", 19, "Leeds");
            People.Add(tomAdams);

            var tomYates = new Person(10, "Tom", "Yates", 65, "Manchester");
            People.Add(tomYates);

            var janetSmith = new Person(11, "Janet", "Smith", 23, "London");
            People.Add(janetSmith);

            var harroldWise = new Person(12, "Harrold", "Wise", 17, "Newcastle");
            People.Add(harroldWise);

            var jennyGoldman = new Person(13, "Jenny", "Goldman", 68, "Barnham");
            People.Add(jennyGoldman);

            var jakeSmith = new Person(14, "Jake", "Smith", 59, "Cambridge");
            People.Add(jakeSmith);

            var janetAustin = new Person(15, "Janet", "Austin", 36, "Plymouth");
            People.Add(janetAustin);

            var stevenLongarm = new Person(16, "Steven", "Longarm", 22, "Norwich");
            People.Add(stevenLongarm);

            var lucyJones = new Person(17, "Lucy", "Jones", 71, "Liverpool");
            People.Add(lucyJones);

            var oliverGains = new Person(18, "Oliver", "Gains", 40, "Ipswich");
            People.Add(oliverGains);

            var tomAdnams = new Person(19, "Tom", "Adnams", 28, "Portsmouth");
            People.Add(tomAdnams);

            var thomasFind = new Person(20, "Thomas", "Find", 56, "Manchester");
            People.Add(thomasFind);
        }
    }
}
