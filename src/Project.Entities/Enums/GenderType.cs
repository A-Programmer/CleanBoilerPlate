using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Entities
{
    public enum GenderType
    {
        [Display(Name = "Not specified")]
        NotSpecified,

        [Display(Name = "Male")]
        Male,

        [Display(Name = "Female")]
        Female,

        [Display(Name = "Others")]
        Others
    }
}
