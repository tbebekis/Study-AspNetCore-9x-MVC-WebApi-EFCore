namespace CommonLib.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    [Table(nameof(MeasureUnit))]
    public class MeasureUnit: BaseEntity
    {
        public MeasureUnit() { }
        public MeasureUnit(string Name)
        {
            this.SetId();
            this.Name = Name;
        }

        [Required, MaxLength(128)]
        public string Name { get; set; }
    }

    public class MeasureUnitEntityTypeConfiguration : IEntityTypeConfiguration<MeasureUnit>
    {
        public void Configure(EntityTypeBuilder<MeasureUnit> builder)
        {
            //
        }
    }
}
