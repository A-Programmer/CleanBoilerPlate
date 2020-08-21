using System;
namespace Project.Entities
{
    public class MyEntity : BaseEntityWithDetails<int>
    {
        public string MyName { get; set; }
        public string MyLastName { get; set; }
    }
}
