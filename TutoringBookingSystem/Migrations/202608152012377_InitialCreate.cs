namespace TutoringBookingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: false),
                        TimeSlotId = c.Int(nullable: false),
                        Status = c.String(),
                        PaymentStatus = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .ForeignKey("dbo.TimeSlots", t => t.TimeSlotId, cascadeDelete: true)
                .Index(t => t.StudentId)
                .Index(t => t.TimeSlotId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Contact = c.String(nullable: false),
                        Subject = c.String(nullable: false),
                        Grade = c.String(),
                    })
                .PrimaryKey(t => t.StudentId);
            
            CreateTable(
                "dbo.TimeSlots",
                c => new
                    {
                        TimeSlotId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                        IsBooked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TimeSlotId);
            
            CreateTable(
                "dbo.Tutors",
                c => new
                    {
                        TutorId = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        PasswordHash = c.String(nullable: false),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.TutorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "TimeSlotId", "dbo.TimeSlots");
            DropForeignKey("dbo.Bookings", "StudentId", "dbo.Students");
            DropIndex("dbo.Bookings", new[] { "TimeSlotId" });
            DropIndex("dbo.Bookings", new[] { "StudentId" });
            DropTable("dbo.Tutors");
            DropTable("dbo.TimeSlots");
            DropTable("dbo.Students");
            DropTable("dbo.Bookings");
        }
    }
}
