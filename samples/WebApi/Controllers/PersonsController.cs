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
            var janeSmith = new Person("Jane", "Smith", 32, "London");
            People.Add(janeSmith);

            var harroldJones = new Person("Harrold", "Jones", 21, "Newcastle");
            People.Add(harroldJones);

            var jennyGarrod = new Person("Jenny", "Garrod", 46, "Bristol");
            People.Add(jennyGarrod);

            var jamesSmith = new Person("James", "Smith", 29, "Newquey");
            People.Add(jamesSmith);

            var janeAustin = new Person("Jane", "Austin", 63, "Plymouth");
            People.Add(janeAustin);

            var steveLongman = new Person("Steve", "Longman", 32, "Oxford");
            People.Add(steveLongman);

            var jeniJones = new Person("Jeni", "Jones", 27, "London");
            People.Add(jeniJones);

            var harroldGains = new Person("Harrold", "Gains", 44, "London");
            People.Add(harroldGains);

            var tomAdams = new Person("Tom", "Adams", 19, "Leeds");
            People.Add(tomAdams);

            var tomYates = new Person("Tom", "Yates", 65, "Manchester");
            People.Add(tomYates);

            var janetSmith = new Person("Janet", "Smith", 23, "London");
            People.Add(janetSmith);

            var harroldWise = new Person("Harrold", "Wise", 17, "Newcastle");
            People.Add(harroldWise);

            var jennyGoldman = new Person("Jenny", "Goldman", 68, "Barnham");
            People.Add(jennyGoldman);

            var jakeSmith = new Person("Jake", "Smith", 59, "Cambridge");
            People.Add(jakeSmith);

            var janetAustin = new Person("Janet", "Austin", 36, "Plymouth");
            People.Add(janetAustin);

            var stevenLongarm = new Person("Steven", "Longarm", 22, "Norwich");
            People.Add(stevenLongarm);

            var lucyJones = new Person("Lucy", "Jones", 71, "Liverpool");
            People.Add(lucyJones);

            var oliverGains = new Person("Oliver", "Gains", 40, "Ipswich");
            People.Add(oliverGains);

            var tomAdnams = new Person("Tom", "Adnams", 28, "Portsmouth");
            People.Add(tomAdnams);

            var thomasFind = new Person("Thomas", "Find", 56, "Manchester");
            People.Add(thomasFind);
        }
    }
}
