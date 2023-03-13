namespace Acme.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Acme.Models.AcmeContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Acme.Models.AcmeContext context)
        {
          
        }
    }
}
