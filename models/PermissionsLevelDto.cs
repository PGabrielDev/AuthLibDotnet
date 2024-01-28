namespace CheckPermissionsLib.models
{
    public class PermissivosLevelDto
    {
        public string Name { get; set; }
        public string Email{ get; set;}
        public ICollection<RoleDto> Roles { get; set;}
        public ICollection<ProductLevelDto> LevelAcesses { get; set; }
    }
}
