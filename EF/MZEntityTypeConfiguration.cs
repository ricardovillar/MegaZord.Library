using System.Data.Entity.ModelConfiguration;
using MegaZord.Library.Interfaces;


namespace MegaZord.Library.EF
{
    public abstract class MZEntityTypeConfiguration<TEntityType> : EntityTypeConfiguration<TEntityType>
        where TEntityType : class, IMZEntity
    {
        protected MZEntityTypeConfiguration()
        {
            if (!string.IsNullOrEmpty(TableName)) 
                ToTable(TableName);

            if (!IsDerivedType)
            {


                if (HasIdentityKey)
                {
                    HasKey(k => k.ID);
                    Property(p => p.ID)
                        .HasColumnName(PrimaryKeyColName)
                        .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)
                        .IsRequired();

                }
            }
            ConfigureFields();
        }

        protected virtual string PrimaryKeyColName { get { return "ID"; } }
        protected virtual bool HasIdentityKey { get { return true; } }
        protected virtual bool IsDerivedType { get { return false; } }

        protected abstract void ConfigureFields();
        protected abstract string TableName { get; }
    }
}
