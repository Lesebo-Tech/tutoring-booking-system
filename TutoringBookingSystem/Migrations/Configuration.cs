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
            if (!context.Tutors.Any())
            {
                context.Tutors.Add(new TutoringBookingSystem.Models.Tutor
                {
                    Username = "lesebo",
                    PasswordHash = System.Web.Helpers.Crypto.HashPassword("25$Andisa"),
                    Email = "owamixaba01@gmail.com"
                });
                context.SaveChanges();
            }
        }
    }
}
