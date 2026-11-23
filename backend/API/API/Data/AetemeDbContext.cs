using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Contexto de persistencia del sistema. Esquema derivado del diagrama de clases actual:
/// 8 tablas, roles y estados como texto, módulo de auditoría como texto y FK IdUsuario
/// en RefreshToken, TokenVerificacionCorreo y RegistroAuditoria.
/// </summary>
public sealed class AetemeDbContext(DbContextOptions<AetemeDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<TokenVerificacionCorreo> TokensVerificacionCorreo => Set<TokenVerificacionCorreo>();

    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    public DbSet<Noticia> Noticias => Set<Noticia>();

    public DbSet<Evento> Eventos => Set<Evento>();

    public DbSet<Patrocinador> Patrocinadores => Set<Patrocinador>();

    public DbSet<InformacionInstitucional> InformacionInstitucional => Set<InformacionInstitucional>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario", t => t.HasCheckConstraint(
                "CK_Usuario_Rol", "\"Rol\" IN ('Administrador', 'SuperAdministrador')"));
            entity.HasKey(u => u.IdUsuario);
            entity.Property(u => u.IdUsuario).ValueGeneratedOnAdd();

            entity.Property(u => u.Rol)
                .HasConversion<string>()
                .HasColumnType("varchar(20)")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(u => u.NombreCompleto).HasColumnType("varchar(150)").HasMaxLength(150).IsRequired();
            entity.Property(u => u.Correo).HasColumnType("varchar(254)").HasMaxLength(254).IsRequired();
            entity.HasIndex(u => u.Correo).IsUnique();

            entity.Property(u => u.ContrasenaHash).HasColumnType("bytea").IsRequired();
            entity.Property(u => u.ContrasenaSalt).HasColumnType("bytea").IsRequired();

            entity.Property(u => u.CorreoVerificado).HasColumnType("boolean").HasDefaultValue(false);
            entity.Property(u => u.Activo).HasColumnType("boolean").HasDefaultValue(true);
            entity.Property(u => u.FechaCreacion).HasColumnType("timestamptz").HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshToken");
            entity.HasKey(t => t.IdRefreshToken);
            entity.Property(t => t.IdRefreshToken).ValueGeneratedOnAdd();

            entity.Property(t => t.TokenHash).HasColumnType("bytea").IsRequired();
            entity.HasIndex(t => t.TokenHash).IsUnique();
            entity.Property(t => t.FechaExpiracion).HasColumnType("timestamptz").IsRequired();
            entity.Property(t => t.Revocado).HasColumnType("boolean").HasDefaultValue(false);

            entity.HasOne(t => t.Usuario)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(t => t.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasIndex(t => t.IdUsuario);
        });

        modelBuilder.Entity<TokenVerificacionCorreo>(entity =>
        {
            entity.ToTable("TokenVerificacionCorreo");
            entity.HasKey(t => t.IdTokenVerificacion);
            entity.Property(t => t.IdTokenVerificacion).ValueGeneratedOnAdd();

            entity.Property(t => t.TokenHash).HasColumnType("bytea").IsRequired();
            entity.HasIndex(t => t.TokenHash).IsUnique();
            entity.Property(t => t.FechaExpiracion).HasColumnType("timestamptz").IsRequired();
            entity.Property(t => t.FechaUso).HasColumnType("timestamptz");

            entity.HasOne(t => t.Usuario)
                .WithMany(u => u.TokensVerificacionCorreo)
                .HasForeignKey(t => t.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasIndex(t => t.IdUsuario);
        });

        modelBuilder.Entity<RegistroAuditoria>(entity =>
        {
            entity.ToTable("RegistroAuditoria");
            entity.HasKey(a => a.IdRegistroAuditoria);
            entity.Property(a => a.IdRegistroAuditoria).ValueGeneratedOnAdd();

            entity.Property(a => a.Modulo).HasColumnType("varchar(60)").HasMaxLength(60).IsRequired();
            entity.Property(a => a.Accion).HasColumnType("varchar(200)").HasMaxLength(200).IsRequired();
            entity.Property(a => a.Fecha).HasColumnType("timestamptz").HasDefaultValueSql("now()");

            entity.HasOne(a => a.Usuario)
                .WithMany(u => u.RegistrosAuditoria)
                .HasForeignKey(a => a.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasIndex(a => a.Fecha);
            entity.HasIndex(a => a.IdUsuario);
            entity.HasIndex(a => a.Modulo);
        });

        modelBuilder.Entity<Noticia>(entity =>
        {
            entity.ToTable("Noticia", t => t.HasCheckConstraint(
                "CK_Noticia_Estado", "\"Estado\" IN ('Borrador', 'Programada', 'Publicada', 'Archivada')"));
            entity.HasKey(n => n.IdNoticia);
            entity.Property(n => n.IdNoticia).ValueGeneratedOnAdd();

            entity.Property(n => n.Titulo).HasColumnType("varchar(200)").HasMaxLength(200).IsRequired();
            entity.Property(n => n.Cuerpo).HasColumnType("text").IsRequired();
            entity.Property(n => n.FechaPublicacion).HasColumnType("timestamptz");
            entity.Property(n => n.RutaImagenDestacada).HasColumnType("varchar(300)").HasMaxLength(300);
            entity.Property(n => n.Activo).HasColumnType("boolean").HasDefaultValue(true);
            entity.Property(n => n.Categoria).HasColumnType("varchar(80)").HasMaxLength(80).IsRequired();

            entity.Property(n => n.Estado)
                .HasConversion<string>()
                .HasColumnType("varchar(20)")
                .HasMaxLength(20)
                .IsRequired();
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.ToTable("Evento");
            entity.HasKey(e => e.IdEvento);
            entity.Property(e => e.IdEvento).ValueGeneratedOnAdd();

            entity.Property(e => e.Titulo).HasColumnType("varchar(200)").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Fecha).HasColumnType("date").IsRequired();
            entity.Property(e => e.Hora).HasColumnType("time").IsRequired();
            entity.Property(e => e.Lugar).HasColumnType("varchar(200)").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("text").IsRequired();
            entity.Property(e => e.EsPublico).HasColumnType("boolean").IsRequired();
            entity.Property(e => e.EnlaceInscripcion).HasColumnType("varchar(500)").HasMaxLength(500);
            entity.Property(e => e.Activo).HasColumnType("boolean").HasDefaultValue(true);
            entity.Property(e => e.Categoria).HasColumnType("varchar(80)").HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<Patrocinador>(entity =>
        {
            entity.ToTable("Patrocinador", t => t.HasCheckConstraint(
                "CK_Patrocinador_Vencimiento", "\"FechaVencimiento\" >= \"FechaInicio\""));
            entity.HasKey(p => p.IdPatrocinador);
            entity.Property(p => p.IdPatrocinador).ValueGeneratedOnAdd();

            entity.Property(p => p.Nombre).HasColumnType("varchar(150)").HasMaxLength(150).IsRequired();
            entity.Property(p => p.RutaLogotipo).HasColumnType("varchar(300)").HasMaxLength(300).IsRequired();
            entity.Property(p => p.EnlaceWeb).HasColumnType("varchar(500)").HasMaxLength(500).IsRequired();
            entity.Property(p => p.UrlInformacion).HasColumnType("varchar(500)").HasMaxLength(500).IsRequired();
            entity.Property(p => p.FechaInicio).HasColumnType("date").IsRequired();
            entity.Property(p => p.FechaVencimiento).HasColumnType("date").IsRequired();
            entity.Property(p => p.Contacto).HasColumnType("varchar(200)").HasMaxLength(200).IsRequired();
            entity.Property(p => p.Observaciones).HasColumnType("varchar(500)").HasMaxLength(500);
            entity.Property(p => p.BeneficiosAcordados).HasColumnType("text").IsRequired();
            entity.Property(p => p.Activo).HasColumnType("boolean").HasDefaultValue(true);
            entity.Property(p => p.FechaUltimoRecordatorio).HasColumnType("timestamptz");
            entity.Property(p => p.Categoria).HasColumnType("varchar(60)").HasMaxLength(60).IsRequired();
            entity.Property(p => p.TipoPatrocinio).HasColumnType("varchar(60)").HasMaxLength(60).IsRequired();
        });

        modelBuilder.Entity<InformacionInstitucional>(entity =>
        {
            entity.ToTable("InformacionInstitucional", t => t.HasCheckConstraint(
                "CK_InformacionInstitucional_Id", "\"IdInformacionInstitucional\" = 1"));
            entity.HasKey(i => i.IdInformacionInstitucional);
            entity.Property(i => i.IdInformacionInstitucional).ValueGeneratedNever();
            entity.Property(i => i.Nombre).HasColumnType("varchar(150)").HasMaxLength(150).IsRequired();
            entity.Property(i => i.Texto).HasColumnType("text").IsRequired();
        });
    }
}
