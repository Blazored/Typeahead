using System.ComponentModel.DataAnnotations;

namespace Sample.Shared
{
    public class FormExample
    {
        [Required]
        public Person SelectedPerson { get; set; }
    }
    
}