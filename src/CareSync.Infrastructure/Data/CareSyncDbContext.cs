using CareSync.Domain.Entities;
using CareSync.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareSync.Infrastructure.Data;

public class CareSyncDbContext(DbContextOptions<CareSyncDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<Staff> Staff { get; set; } = null!;

    // Medical Records & Clinical Data
    public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
    public DbSet<VitalSigns> VitalSigns { get; set; } = null!;
    public DbSet<Diagnosis> Diagnoses { get; set; } = null!;
    public DbSet<Prescription> Prescriptions { get; set; } = null!;
    public DbSet<Treatment> Treatments { get; set; } = null!;
    public DbSet<Lab> Labs { get; set; } = null!;

    // Billing & Financial
    public DbSet<Bill> Bills { get; set; } = null!;
    public DbSet<BillItem> BillItems { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;

    // Insurance
    public DbSet<InsuranceClaim> InsuranceClaims { get; set; } = null!;

    // Philippine Geographic Data (API-first approach - Barangays from PSGC Cloud API)
    public DbSet<Province> Provinces { get; set; } = null!;
    public DbSet<City> Cities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignore domain events (value objects for in-memory dispatch only)
        modelBuilder.Ignore<CareSync.Domain.Events.DomainEvent>();

        // Apply basic configurations for now - will expand later
        ConfigureValueObjectsBasic(modelBuilder);

        // Configure decimal properties to eliminate warnings
        ConfigureDecimalPrecision(modelBuilder);

        // Basic entity configurations
        ConfigureBasicEntities(modelBuilder);

        // Philippine Geographic Data configurations
        ConfigurePhilippineGeographicData(modelBuilder);
    }

    private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
    {
        // Configure all decimal properties with proper precision and scale
        // This eliminates the EF Core warnings about unspecified decimal types

        // Bill entity decimals - using 18,2 for currency values
        modelBuilder.Entity<Bill>()
            .Property(b => b.BalanceAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Bill>()
            .Property(b => b.DiscountAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Bill>()
            .Property(b => b.PaidAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Bill>()
            .Property(b => b.SubTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Bill>()
            .Property(b => b.TaxAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Bill>()
            .Property(b => b.TaxRate)
            .HasPrecision(5, 4); // e.g. 0.1250 = 12.50%

        modelBuilder.Entity<Bill>()
            .Property(b => b.TotalAmount)
            .HasPrecision(18, 2);

        // BillItem entity decimals
        modelBuilder.Entity<BillItem>()
            .Property(bi => bi.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<BillItem>()
            .Property(bi => bi.UnitPrice)
            .HasPrecision(18, 2);

        // InsuranceClaim entity decimals
        modelBuilder.Entity<InsuranceClaim>()
            .Property(ic => ic.ApprovedAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<InsuranceClaim>()
            .Property(ic => ic.ClaimAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<InsuranceClaim>()
            .Property(ic => ic.PaidAmount)
            .HasPrecision(18, 2);

        // Payment entity decimals
        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        // Staff entity decimals (Salary)
        modelBuilder.Entity<Staff>()
            .Property(s => s.Salary)
            .HasPrecision(18, 2);

        // Treatment entity decimals (BaseCost)
        modelBuilder.Entity<Treatment>()
            .Property(t => t.BaseCost)
            .HasPrecision(18, 2);

        // TreatmentRecord entity decimals (ActualCost)
        modelBuilder.Entity<TreatmentRecord>()
            .Property(tr => tr.ActualCost)
            .HasPrecision(18, 2);

        // VitalSigns entity decimals - using different precision for medical measurements
        modelBuilder.Entity<VitalSigns>()
            .Property(vs => vs.Height)
            .HasPrecision(5, 2); // e.g., 999.99 cm

        modelBuilder.Entity<VitalSigns>()
            .Property(vs => vs.Weight)
            .HasPrecision(6, 2); // e.g., 9999.99 kg

        modelBuilder.Entity<VitalSigns>()
            .Property(vs => vs.Temperature)
            .HasPrecision(4, 1); // e.g., 999.9 °F

        modelBuilder.Entity<VitalSigns>()
            .Property(vs => vs.OxygenSaturation)
            .HasPrecision(5, 2); // e.g., 100.00 %
    }

    private void ConfigureValueObjectsBasic(ModelBuilder modelBuilder)
    {
        // Configure FullName value object for Patient
        modelBuilder.Entity<Patient>().OwnsOne(p => p.FullName, fn =>
        {
            fn.Property(f => f.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(100)
                .IsRequired()
                .HasDefaultValue("Unknown"); // Provide default for null values

            fn.Property(f => f.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(100)
                .IsRequired()
                .HasDefaultValue("Unknown"); // Provide default for null values

            fn.Property(f => f.MiddleName)
                .HasColumnName("MiddleName")
                .HasMaxLength(100)
                .IsRequired(false);
        });

        // Configure Email and PhoneNumber value objects for Patient using converters (avoids optional owned warnings)
        var emailConverter = new ValueConverter<Email?, string?>(
            email => email == null ? null : email.Value,
            value => string.IsNullOrWhiteSpace(value) ? null : new Email(value));
        var emailComparer = new ValueComparer<Email?>(
            (left, right) =>
                (left == null && right == null) ||
                (left != null && right != null && left.Value == right.Value),
            value => value == null ? 0 : value.Value.GetHashCode(),
            value => value == null ? null : new Email(value.Value));

        modelBuilder.Entity<Patient>()
            .Property(p => p.Email)
            .HasConversion(emailConverter)
            .HasColumnName("Email")
            .HasMaxLength(255)
            .IsRequired(false)
            .Metadata.SetValueComparer(emailComparer);

        var phoneConverter = new ValueConverter<PhoneNumber?, string?>(
            phone => phone == null ? null : phone.Number,
            value => string.IsNullOrWhiteSpace(value) ? null : new PhoneNumber(value));
        var phoneComparer = new ValueComparer<PhoneNumber?>(
            (left, right) =>
                (left == null && right == null) ||
                (left != null && right != null && left.Number == right.Number),
            value => value == null ? 0 : value.Number.GetHashCode(),
            value => value == null ? null : new PhoneNumber(value.Number));

        modelBuilder.Entity<Patient>()
            .Property(p => p.PhoneNumber)
            .HasConversion(phoneConverter)
            .HasColumnName("PhoneNumber")
            .HasMaxLength(20)
            .IsRequired(false)
            .Metadata.SetValueComparer(phoneComparer);

        // Configure Patient properties as simple columns
        modelBuilder.Entity<Patient>()
            .Property(p => p.ProvinceName)
            .HasColumnName("ProvinceName")
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Patient>()
            .Property(p => p.CityName)
            .HasColumnName("CityName")
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Patient>()
            .Property(p => p.CityZipCode)
            .HasColumnName("CityZipCode")
            .HasMaxLength(10)
            .IsRequired(); // NOT NULL - required field

        modelBuilder.Entity<Patient>()
            .Property(p => p.BarangayName)
            .HasColumnName("BarangayName")
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Patient>()
            .Property(p => p.BarangayCode)
            .HasColumnName("BarangayCode")
            .IsRequired();

        modelBuilder.Entity<Patient>()
            .Property(p => p.ProvinceCode)
            .HasColumnName("ProvinceCode")
            .IsRequired();

        modelBuilder.Entity<Patient>()
            .Property(p => p.CityCode)
            .HasColumnName("CityCode")
            .IsRequired();

        modelBuilder.Entity<Patient>()
            .Property(p => p.Street)
            .HasColumnName("Street")
            .HasMaxLength(200)
            .IsRequired(false); // Allow NULL values

        modelBuilder.Entity<Patient>()
            .Property(p => p.Gender)
            .HasColumnName("Gender")
            .HasMaxLength(20)
            .IsRequired(); // NOT NULL - required field

        // Configure Doctor properties
        // Title is now fixed as "Dr" and not stored in database

        // Configure FullName value object for Doctor
        modelBuilder.Entity<Doctor>().OwnsOne(d => d.Name, fn =>
        {
            fn.Property(f => f.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(100)
                .IsRequired()
                .HasDefaultValue("Unknown"); // Provide default for null values

            fn.Property(f => f.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(100)
                .IsRequired()
                .HasDefaultValue("Unknown"); // Provide default for null values

            fn.Property(f => f.MiddleName)
                .HasColumnName("MiddleName")
                .HasMaxLength(100)
                .IsRequired(false);
        });

        // Configure Email value object for Doctor
        modelBuilder.Entity<Doctor>().OwnsOne(d => d.Email, e =>
        {
            e.Property(em => em.Value).HasColumnName("Email").HasMaxLength(255);
        });

        // Configure PhoneNumber value object for Doctor
        modelBuilder.Entity<Doctor>().OwnsOne(d => d.PhoneNumber, pn =>
        {
            pn.Property(ph => ph.Number).HasColumnName("PhoneNumber").HasMaxLength(20);
        });

        // Configure Lab value objects
        modelBuilder.Entity<Lab>().OwnsOne(l => l.Address, a =>
        {
            a.Property(ad => ad.Street).HasColumnName("Street").HasMaxLength(200);
            a.Property(ad => ad.Barangay).HasColumnName("Barangay").HasMaxLength(100);
            a.Property(ad => ad.City).HasColumnName("City").HasMaxLength(100);
            a.Property(ad => ad.Province).HasColumnName("Province").HasMaxLength(100);
            a.Property(ad => ad.ZipCode).HasColumnName("ZipCode").HasMaxLength(20);
        });

        modelBuilder.Entity<Lab>().OwnsOne(l => l.PhoneNumber, pn =>
        {
            pn.Property(ph => ph.Number).HasColumnName("PhoneNumber").HasMaxLength(20);
        });

        modelBuilder.Entity<Lab>().OwnsOne(l => l.Email, e =>
        {
            e.Property(em => em.Value).HasColumnName("Email").HasMaxLength(255);
        });

        // Configure Staff value objects (if it has them)
        ConfigureStaffValueObjects(modelBuilder);
    }

    private void ConfigureStaffValueObjects(ModelBuilder modelBuilder)
    {
        // Configure Staff value objects
        modelBuilder.Entity<Staff>().OwnsOne(s => s.Name, fn =>
        {
            fn.Property(f => f.FirstName).HasColumnName("FirstName").HasMaxLength(100);
            fn.Property(f => f.LastName).HasColumnName("LastName").HasMaxLength(100);
        });

        modelBuilder.Entity<Staff>().OwnsOne(s => s.Email, e =>
        {
            e.Property(em => em.Value).HasColumnName("Email").HasMaxLength(255);
        });

        modelBuilder.Entity<Staff>().OwnsOne(s => s.PhoneNumber, pn =>
        {
            pn.Property(ph => ph.Number).HasColumnName("PhoneNumber").HasMaxLength(20);
        });
    }

    private void ConfigureBasicEntities(ModelBuilder modelBuilder)
    {
        // Configure Patient - CORE ENTITY FOR ZIPCODE TESTING
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.DateOfBirth).IsRequired(false); // Make nullable

            // Configure optional fields as nullable
            entity.Property(p => p.PhilHealthNumber)
                .HasMaxLength(20)
                .IsRequired(false);

            entity.Property(p => p.SssNumber)
                .HasMaxLength(20)
                .IsRequired(false);

            entity.Property(p => p.Tin)
                .HasMaxLength(20)
                .IsRequired(false);

            entity.Property(p => p.EmergencyContactName)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(p => p.EmergencyContactNumber)
                .HasMaxLength(20)
                .IsRequired(false);

            entity.Property(p => p.BloodType)
                .HasMaxLength(10)
                .IsRequired(false);

            // Configure audit properties as shadow properties
            entity.Property<DateTime>("CreatedAt").IsRequired();
            entity.Property<DateTime?>("UpdatedAt");
        });

        // Configure Doctor - CORE ENTITY
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Specialty).HasMaxLength(100).IsRequired();
            entity.Property(d => d.IsActive).IsRequired();

            // Configure audit properties as shadow properties
            entity.Property<DateTime>("CreatedAt").IsRequired();
            entity.Property<DateTime?>("UpdatedAt");
        });

        // Configure Lab - CORE ENTITY FOR ZIPCODE TESTING
        modelBuilder.Entity<Lab>(entity =>
        {
            entity.HasKey(l => l.Id);
        });

        // SIMPLE CONFIGURATIONS - NO COMPLEX RELATIONSHIPS FOR NOW
        // This avoids foreign key cascade conflicts while we test ZipCode functionality

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Status).HasConversion<int>().IsRequired();
            // Domain events are in-memory only
            entity.Ignore(a => a.DomainEvents);

            // Configure audit properties as shadow properties
            entity.Property<DateTime>("CreatedAt").IsRequired();
            entity.Property<DateTime?>("UpdatedAt");

            // Configure relationships
            entity.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(mr => mr.Id);
            // Domain events are in-memory only
            entity.Ignore(mr => mr.DomainEvents);

            // Configure relationships with strict data integrity
            entity.HasOne(mr => mr.Patient)
                .WithMany()
                .HasForeignKey(mr => mr.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent patient deletion if medical records exist

            entity.HasOne(mr => mr.Doctor)
                .WithMany()
                .HasForeignKey(mr => mr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent doctor deletion if medical records exist

            entity.HasOne(mr => mr.Appointment)
                .WithMany()
                .HasForeignKey(mr => mr.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull); // Allow appointment deletion, set to null
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Status).HasConversion<int>().IsRequired();

            // Configure relationships with data integrity
            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(b => b.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent patient deletion if bills exist
        });

        // MINIMAL CONFIGURATIONS FOR OTHER ENTITIES
        modelBuilder.Entity<Staff>(entity => { entity.HasKey(s => s.Id); });
        modelBuilder.Entity<VitalSigns>(entity =>
        {
            entity.HasKey(vs => vs.Id);
            entity.Ignore(vs => vs.Bmi);
            entity.Ignore(vs => vs.MeasurementDate);
            // Vital signs cascade delete with medical record
            entity.HasOne<MedicalRecord>()
                .WithMany(mr => mr.VitalSigns)
                .HasForeignKey(vs => vs.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Diagnosis>(entity => { entity.HasKey(d => d.Id); });
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(p => p.Id);
            // Prescriptions cascade delete with medical record
            entity.HasOne<MedicalRecord>()
                .WithMany(mr => mr.Prescriptions)
                .HasForeignKey(p => p.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Treatment>(entity => { entity.HasKey(t => t.Id); });
        // Configure Bill entity relationships to prevent shadow properties
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(b => b.Id);

            // Ignore navigation properties to prevent automatic relationship detection
            entity.Ignore(b => b.BillItems);
            entity.Ignore(b => b.Payments);
            entity.Ignore(b => b.InsuranceClaims);
            entity.Ignore(b => b.Items);
            entity.Ignore(b => b.Claims);
        });

        modelBuilder.Entity<BillItem>(entity =>
        {
            entity.HasKey(bi => bi.Id);
            entity.Property(bi => bi.BillId).IsRequired();
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.BillId).IsRequired();
        });

        modelBuilder.Entity<InsuranceClaim>(entity =>
        {
            entity.HasKey(ic => ic.Id);
            entity.Property(ic => ic.BillId).IsRequired();
        });

        // Add Performance Indexes
        ConfigureIndexes(modelBuilder);
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Patient indexes for search optimization
        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Patients_IsActive");

        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.DateOfBirth)
            .HasDatabaseName("IX_Patients_DateOfBirth");

        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.Gender)
            .HasDatabaseName("IX_Patients_Gender");

        // Appointment indexes for scheduling queries
        modelBuilder.Entity<Appointment>()
            .HasIndex(a => a.PatientId)
            .HasDatabaseName("IX_Appointments_PatientId");

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => a.DoctorId)
            .HasDatabaseName("IX_Appointments_DoctorId");

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => a.ScheduledDate)
            .HasDatabaseName("IX_Appointments_ScheduledDate");

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.DoctorId, a.ScheduledDate })
            .HasDatabaseName("IX_Appointments_Doctor_Date");

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => a.Status)
            .HasDatabaseName("IX_Appointments_Status");

        // Bill indexes for financial queries
        modelBuilder.Entity<Bill>()
            .HasIndex(b => b.PatientId)
            .HasDatabaseName("IX_Bills_PatientId");

        modelBuilder.Entity<Bill>()
            .HasIndex(b => b.BillDate)
            .HasDatabaseName("IX_Bills_BillDate");

        modelBuilder.Entity<Bill>()
            .HasIndex(b => b.DueDate)
            .HasDatabaseName("IX_Bills_DueDate");

        modelBuilder.Entity<Bill>()
            .HasIndex(b => b.Status)
            .HasDatabaseName("IX_Bills_Status");

        modelBuilder.Entity<Bill>()
            .HasIndex(b => b.BillNumber)
            .IsUnique()
            .HasDatabaseName("IX_Bills_BillNumber");

        // Payment indexes
        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.BillId)
            .HasDatabaseName("IX_Payments_BillId");

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.PaymentDate)
            .HasDatabaseName("IX_Payments_PaymentDate");

        // Doctor indexes
        modelBuilder.Entity<Doctor>()
            .HasIndex(d => d.Specialty)
            .HasDatabaseName("IX_Doctors_Specialty");

        modelBuilder.Entity<Doctor>()
            .HasIndex(d => d.IsActive)
            .HasDatabaseName("IX_Doctors_IsActive");

        // Medical record indexes
        modelBuilder.Entity<MedicalRecord>()
            .HasIndex(mr => mr.PatientId)
            .HasDatabaseName("IX_MedicalRecords_PatientId");

        modelBuilder.Entity<MedicalRecord>()
            .HasIndex(mr => mr.RecordDate)
            .HasDatabaseName("IX_MedicalRecords_RecordDate");
    }

    private void ConfigurePhilippineGeographicData(ModelBuilder modelBuilder)
    {
        // Configure Province entity
        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Code).HasMaxLength(10).IsRequired();
            entity.Property(p => p.Region).HasMaxLength(100).IsRequired();
            entity.HasIndex(p => p.Code).IsUnique().HasDatabaseName("IX_Provinces_Code");
            entity.HasIndex(p => p.Name).HasDatabaseName("IX_Provinces_Name");
        });

        // Configure City entity
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Code).HasMaxLength(10).IsRequired();
            entity.HasIndex(c => c.Code).IsUnique().HasDatabaseName("IX_Cities_Code");
            entity.HasIndex(c => c.Name).HasDatabaseName("IX_Cities_Name");
            entity.HasIndex(c => c.ProvinceId).HasDatabaseName("IX_Cities_ProvinceId");

            // Configure relationship with Province
            entity.HasOne(c => c.Province)
                .WithMany(p => p.Cities)
                .HasForeignKey(c => c.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Note: Barangay entity removed - using API-first approach with PSGC Cloud API
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is not null &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
            try
            {
                if (entry.State == EntityState.Added) entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;

                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
            catch (InvalidOperationException)
            {
                // Skip entities that don't have CreatedAt/UpdatedAt properties (like owned entities)
            }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
