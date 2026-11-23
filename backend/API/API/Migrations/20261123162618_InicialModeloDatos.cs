using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InicialModeloDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evento",
                columns: table => new
                {
                    IdEvento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    Lugar = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    EsPublico = table.Column<bool>(type: "boolean", nullable: false),
                    EnlaceInscripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Categoria = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.IdEvento);
                });

            migrationBuilder.CreateTable(
                name: "InformacionInstitucional",
                columns: table => new
                {
                    IdInformacionInstitucional = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformacionInstitucional", x => x.IdInformacionInstitucional);
                    table.CheckConstraint("CK_InformacionInstitucional_Id", "\"IdInformacionInstitucional\" = 1");
                });

            migrationBuilder.CreateTable(
                name: "Noticia",
                columns: table => new
                {
                    IdNoticia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Cuerpo = table.Column<string>(type: "text", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    RutaImagenDestacada = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Categoria = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noticia", x => x.IdNoticia);
                    table.CheckConstraint("CK_Noticia_Estado", "\"Estado\" IN ('Borrador', 'Programada', 'Publicada', 'Archivada')");
                });

            migrationBuilder.CreateTable(
                name: "Patrocinador",
                columns: table => new
                {
                    IdPatrocinador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    RutaLogotipo = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    EnlaceWeb = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    UrlInformacion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaVencimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Contacto = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    BeneficiosAcordados = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaUltimoRecordatorio = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    Categoria = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    TipoPatrocinio = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrocinador", x => x.IdPatrocinador);
                    table.CheckConstraint("CK_Patrocinador_Vencimiento", "\"FechaVencimiento\" >= \"FechaInicio\"");
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rol = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    NombreCompleto = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Correo = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    ContrasenaHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    ContrasenaSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    CorreoVerificado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                    table.CheckConstraint("CK_Usuario_Rol", "\"Rol\" IN ('Administrador', 'SuperAdministrador')");
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    IdRefreshToken = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    Revocado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.IdRefreshToken);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistroAuditoria",
                columns: table => new
                {
                    IdRegistroAuditoria = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    Modulo = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    Accion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroAuditoria", x => x.IdRegistroAuditoria);
                    table.ForeignKey(
                        name: "FK_RegistroAuditoria_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokenVerificacionCorreo",
                columns: table => new
                {
                    IdTokenVerificacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    FechaUso = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenVerificacionCorreo", x => x.IdTokenVerificacion);
                    table.ForeignKey(
                        name: "FK_TokenVerificacionCorreo_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_IdUsuario",
                table: "RefreshToken",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_TokenHash",
                table: "RefreshToken",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAuditoria_Fecha",
                table: "RegistroAuditoria",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAuditoria_IdUsuario",
                table: "RegistroAuditoria",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAuditoria_Modulo",
                table: "RegistroAuditoria",
                column: "Modulo");

            migrationBuilder.CreateIndex(
                name: "IX_TokenVerificacionCorreo_IdUsuario",
                table: "TokenVerificacionCorreo",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_TokenVerificacionCorreo_TokenHash",
                table: "TokenVerificacionCorreo",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Correo",
                table: "Usuario",
                column: "Correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evento");

            migrationBuilder.DropTable(
                name: "InformacionInstitucional");

            migrationBuilder.DropTable(
                name: "Noticia");

            migrationBuilder.DropTable(
                name: "Patrocinador");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RegistroAuditoria");

            migrationBuilder.DropTable(
                name: "TokenVerificacionCorreo");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
