
When model changed or for initial migration, run the following commands in the toplevel folder of the OidcTemplate.Data project:
 
 dotnet ef migrations add MigrationName -c IdentityContext --startup-project ..\Auth.Server\Auth.Server.csproj
 dotnet ef migrations add MigrationName -c PersistedGrantDbContext --startup-project ..\Auth.Server\Auth.Server.csproj

