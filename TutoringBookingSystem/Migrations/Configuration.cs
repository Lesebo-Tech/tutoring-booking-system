namespace TutoringBookingSystem.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TutoringBookingSystem.Models.TutoringContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TutoringBookingSystem.Models.TutoringContext context)
        {
           
        }
    }
}
