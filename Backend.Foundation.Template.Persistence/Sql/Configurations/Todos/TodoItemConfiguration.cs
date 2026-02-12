using Backend.Foundation.Template.Domain.Todos.Entities;
using Backend.Foundation.Template.Domain.Todos.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Foundation.Template.Persistence.Sql.Configurations.Todos;

internal sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems");

        builder.HasKey(todo => todo.Id);
        builder.Property(todo => todo.Id).ValueGeneratedNever();

        builder.Property(todo => todo.IsCompleted)
            .IsRequired();

        builder.OwnsOne<TodoTitle>("_title", ownedBuilder =>
        {
            ownedBuilder.Property(title => title.Value)
                .HasColumnName("Title")
                .HasMaxLength(TodoTitle.MaxLength)
                .IsRequired();
        });

        builder.Navigation("_title")
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(todo => todo.IsCompleted);
    }
}
